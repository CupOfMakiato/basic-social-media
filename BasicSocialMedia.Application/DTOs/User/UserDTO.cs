using BasicSocialMedia.Domain.Enums;

namespace BasicSocialMedia.Application.DTOs.User
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public UserStatus Status { get; set; }
        public int RoleId { get; set; }

    }
}
