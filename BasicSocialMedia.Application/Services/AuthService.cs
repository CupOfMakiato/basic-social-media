
using BasicSocialMedia.Application;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.DTOs.Auth;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Domain.Enums;
using System.Security.Cryptography;
namespace BasicSocialMedia.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultUserRoleName = "User";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEncryptionService _encryptionService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _encryptionService = encryptionService;
        }

        public async Task<Result<AuthResult>> RegisterAsync(RegisterRequest request)
        {
            var validationError = ValidateRegisterRequest(request);
            if (validationError != null)
            {
                return Failed(validationError);
            }

            var email = NormalizeEmail(request.Email);
            var userName = request.UserName.Trim();
            var isTaken = await _unitOfWork.UserRepository.IsEmailTakenAsync(email);
            if (isTaken)
            {
                return Failed("Email or username already exists.");
            }

            var userRole = await _unitOfWork.UserRepository.GetRoleByNameAsync(DefaultUserRoleName);
            if (userRole == null)
            {
                return Failed("Default user role is not configured.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = _encryptionService.Encrypt(email),
                Password = _passwordHasher.HashPassword(request.Password),
                RoleId = userRole.Id,
                Status = UserStatus.Active
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangeAsync();

            var tokens = await _jwtTokenService.GenerateAndStoreTokensAsync(user, userRole.RoleName);
            return Succeeded("Register successfully.", BuildAuthResult(user, userRole.RoleName, tokens));
        }

        public async Task<Result<AuthResult>> LoginAsync(LoginRequest request)
        {
            var validationError = ValidateLoginRequest(request);
            if (validationError != null)
            {
                return Failed(validationError);
            }

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return Failed("Email/username or password is incorrect.");
            }

            return await CreateSessionAsync(user);
        }

        public async Task<Result<AuthResult>> LoginWithCognitoAsync(
            string subject,
            string email,
            string roleName)
        {
            if (string.IsNullOrWhiteSpace(subject)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(roleName))
            {
                return Failed("Cognito subject, email, and role are required.");
            }

            subject = subject.Trim();
            email = NormalizeEmail(email);

            var userRole = await _unitOfWork.UserRepository.GetRoleByNameAsync(roleName);
            if (userRole == null)
            {
                return Failed("Cognito role is not configured.");
            }

            var shouldSave = false;
            var user = await _unitOfWork.UserRepository.GetByCognitoSubjectAsync(subject);
            if (user == null)
            {
                user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
                if (user != null
                    && !string.IsNullOrWhiteSpace(user.CognitoSubject)
                    && user.CognitoSubject != subject)
                {
                    return Failed("This email is already linked to another Cognito account.");
                }

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = $"cognito-{subject}",
                        Email = _encryptionService.Encrypt(email),
                        Password = _passwordHasher.HashPassword(
                            Convert.ToHexString(RandomNumberGenerator.GetBytes(32))),
                        CognitoSubject = subject,
                        RoleId = userRole.Id,
                        Role = userRole,
                        Status = UserStatus.Active
                    };
                    await _unitOfWork.UserRepository.AddAsync(user);
                    shouldSave = true;
                }
                else
                {
                    user.CognitoSubject = subject;
                    user.RoleId = userRole.Id;
                    user.Role = userRole;
                    _unitOfWork.UserRepository.Update(user);
                    shouldSave = true;
                }
            }
            else if (user.RoleId != userRole.Id)
            {
                user.RoleId = userRole.Id;
                user.Role = userRole;
                _unitOfWork.UserRepository.Update(user);
                shouldSave = true;
            }

            if (shouldSave)
            {
                try
                {
                    await _unitOfWork.SaveChangeAsync();
                }
                catch
                {
                    var concurrentUser = await _unitOfWork.UserRepository.GetByCognitoSubjectAsync(subject);
                    if (concurrentUser == null)
                    {
                        throw;
                    }

                    user = concurrentUser;
                }
            }

            return await CreateSessionAsync(user);
        }

        private async Task<Result<AuthResult>> CreateSessionAsync(User user)
        {
            if (user.Status != UserStatus.Active)
            {
                return Failed("User account is not active.");
            }

            var role = user.Role ?? await _unitOfWork.UserRepository.GetRoleByIdAsync(user.RoleId);
            if (role == null)
            {
                return Failed("User role is not configured.");
            }

            var tokens = await _jwtTokenService.GenerateAndStoreTokensAsync(user, role.RoleName);
            return Succeeded("Login successfully.", BuildAuthResult(user, role.RoleName, tokens));
        }

        public async Task<Result<object>> LogoutAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return new Result<object>
                {
                    Error = 1,
                    Message = "JWT session is missing."
                };
            }

            await _jwtTokenService.RevokeSessionAsync(sessionId);
            return new Result<object>
            {
                Error = 0,
                Message = "Logout successfully."
            };
        }

        private AuthResult BuildAuthResult(User user, string roleName, JwtTokenResult tokens)
        {
            return new AuthResult
            {
                Tokens = tokens,
                Response = new AuthResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = _encryptionService.Decrypt(user.Email),
                    RoleName = roleName,
                    SessionId = tokens.SessionId,
                    AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
                    RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt
                }
            };
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static string? ValidateRegisterRequest(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                return "Username is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return "Email is required.";
            }

            // if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 5)
            // {
            //     return "Password must be at least 5 characters.";
            // }

            // Strict password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return "Password is required.";
            }

            if (request.Password.Length < 5)
            {
                return "Password must be at least 5 characters.";
            }

            if (!request.Password.Any(char.IsUpper))
            {
                return "Password must contain at least 1 uppercase character.";
            }

            if (!request.Password.Any(char.IsDigit))
            {
                return "Password must contain at least 1 number.";
            }

            if (!request.Password.Any(character => !char.IsLetterOrDigit(character)))
            {
                return "Password must contain at least 1 special character.";
            }

            return null;
        }

        private static string? ValidateLoginRequest(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return "Email or username is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return "Password is required.";
            }

            return null;
        }

        private static Result<AuthResult> Failed(string message)
        {
            return new Result<AuthResult>
            {
                Error = 1,
                Message = message
            };
        }

        private static Result<AuthResult> Succeeded(string message, AuthResult authResult)
        {
            return new Result<AuthResult>
            {
                Error = 0,
                Message = message,
                Data = authResult
            };
        }
    }
}
