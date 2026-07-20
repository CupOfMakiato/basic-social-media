using BasicSocialMedia.Application.Abstractions.RequestAndResponse.User;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.IServices
{
    public interface IUserService
    {
        Task<Result<UserDTO>> GetCurrentUserById();
        Task<Result<ProfilePictureUploadResponse>> UploadProfilePicture(IFormFile? file);
    }
}
