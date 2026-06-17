using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.DTOs.User;

namespace BasicSocialMedia.Application.IServices
{
    public interface IUserService
    {
        Task<Result<UserDTO>> GetCurrentUserById();
    }
}
