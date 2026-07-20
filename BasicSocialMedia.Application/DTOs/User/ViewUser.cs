using BasicSocialMedia.Domain.Enums;

namespace BasicSocialMedia.Application.DTOs.User
{
    public class ViewUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }

    }
}
