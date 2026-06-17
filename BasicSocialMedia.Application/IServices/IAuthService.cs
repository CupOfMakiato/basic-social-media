using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.DTOs.Auth;

namespace BasicSocialMedia.Application.IServices
{
    public interface IAuthService
    {
        Task<Result<AuthResult>> LoginAsync(LoginRequest request);
        Task<Result<AuthResult>> RegisterAsync(RegisterRequest request);
        Task<Result<object>> LogoutAsync(string sessionId);
    }
}