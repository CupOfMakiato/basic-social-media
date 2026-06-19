using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.User
{
    public class ProfilePictureUploadRequest
    {
        public IFormFile? File { get; set; }
    }
}
