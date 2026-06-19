namespace BasicSocialMedia.Domain.Entities
{
    public class Media : BaseEntity
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string FilePublicId { get; set; }
        
        public Guid? TweetId { get; set; }
        public Tweet? Tweet { get; set; }
        public Guid? UserProfilePictureId { get; set; } 
        public User? User { get; set; }
        public Guid? MessageId { get; set; }
        public Message? Message { get; set; }
    }
}