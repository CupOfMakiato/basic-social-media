using AutoMapper;
using BasicSocialMedia.Application.Abstractions.RequestAndResponse.User;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.DTOs.User;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.Services
{
    public class UserService : IUserService
    {
        private const string ProfilePictureFolderName = "profile-pictures";
        private const string WebpFormat = "webp";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IClaimService _claimService;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEncryptionService encryptionService,
            ICloudinaryService cloudinaryService,
            IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _encryptionService = encryptionService;
            _cloudinaryService = cloudinaryService;
            _claimService = claimService;
        }

        public async Task<Result<UserDTO>> GetCurrentUserById()
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return new Result<UserDTO> { Error = 1, Message = "Invalid token" };
            }

            var user = await _unitOfWork.UserRepository.GetUserById(currentUserId);
            if (user == null)
            {
                return new Result<UserDTO> { Error = 1, Message = "User not found" };
            }

            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Email = _encryptionService.Decrypt(user.Email);

            return new Result<UserDTO>
            {
                Error = 0,
                Message = "Success",
                Data = userDto
            };
        }

        public async Task<Result<ProfilePictureUploadResponse>> UploadProfilePicture(IFormFile? file)
        {
            if (file == null)
            {
                return FailedProfilePictureUpload("Profile picture is required.");
            }

            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return FailedProfilePictureUpload("Invalid token");
            }

            var user = await _unitOfWork.UserRepository.GetUserById(currentUserId);
            if (user == null)
            {
                return FailedProfilePictureUpload("User not found");
            }

            var shouldKeepGif = ShouldKeepGifFormat(file);
            var uploadFormat = shouldKeepGif ? null : WebpFormat;
            var uploadedImage = await _cloudinaryService.UploadImage(file, ProfilePictureFolderName, uploadFormat);
            if (string.IsNullOrWhiteSpace(uploadedImage?.FileUrl)
                || string.IsNullOrWhiteSpace(uploadedImage.PublicFileId))
            {
                return FailedProfilePictureUpload(uploadedImage?.ErrorMessage ?? "Could not upload profile picture.");
            }

            var storedFileType = shouldKeepGif ? "image/gif" : "image/webp";
            var storedFileName = BuildStoredFileName(file.FileName, storedFileType);
            var oldPublicFileId = user.ProfilePicture?.FilePublicId;
            var profilePicture = user.ProfilePicture;

            if (profilePicture == null)
            {
                profilePicture = new Media
                {
                    Id = Guid.NewGuid(),
                    UserProfilePictureId = user.Id
                };

                await _unitOfWork.MediaRepository.AddAsync(profilePicture);
            }
            else
            {
                profilePicture.ModificationDate = DateTime.UtcNow;
                profilePicture.ModificationBy = currentUserId;
            }

            profilePicture.FileName = storedFileName;
            profilePicture.FileUrl = uploadedImage.FileUrl;
            profilePicture.FileType = storedFileType;
            profilePicture.FilePublicId = uploadedImage.PublicFileId;

            try
            {
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                await DeleteCloudinaryFileIfPresent(uploadedImage.PublicFileId);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldPublicFileId)
                && oldPublicFileId != uploadedImage.PublicFileId)
            {   
                await DeleteCloudinaryFileIfPresent(oldPublicFileId);
            }

            return new Result<ProfilePictureUploadResponse>
            {
                Error = 0,
                Message = "Profile picture uploaded successfully.",
                Data = new ProfilePictureUploadResponse
                {
                    MediaId = profilePicture.Id,
                    FileUrl = profilePicture.FileUrl,
                    FileType = profilePicture.FileType
                }
            };
        }

        private static Result<ProfilePictureUploadResponse> FailedProfilePictureUpload(string message)
        {
            return new Result<ProfilePictureUploadResponse>
            {
                Error = 1,
                Message = message
            };
        }

        private async Task DeleteCloudinaryFileIfPresent(string? publicFileId)
        {
            if (!string.IsNullOrWhiteSpace(publicFileId))
            {
                await _cloudinaryService.DeleteFileAsync(publicFileId);
            }
        }

        private static bool ShouldKeepGifFormat(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            return extension == ".gif" && contentType == "image/gif";
        }

        private static string BuildStoredFileName(string fileName, string storedFileType)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                fileNameWithoutExtension = "profile-picture";
            }

            var storedExtension = storedFileType == "image/gif" ? ".gif" : ".webp";
            return $"{fileNameWithoutExtension}{storedExtension}";
        }

    }
}
