namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.User
{
    public class ProfilePictureUploadResponse
    {
        public Guid MediaId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}
