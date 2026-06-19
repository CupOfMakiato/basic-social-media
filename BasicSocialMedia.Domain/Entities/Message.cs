namespace BasicSocialMedia.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string? MessageContent { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime ReadAt { get; set; }

        // List of images
        public ICollection<Media> Media { get; set; } = new List<Media>();
        public DirectMessageChat DirectMessageChat{ get; set; }
    }
}