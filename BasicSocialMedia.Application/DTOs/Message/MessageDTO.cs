namespace BasicSocialMedia.Application.DTOs.Message
{
    public class MessageDTO
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string? MessageContent { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime ReadAt { get; set; }

        public List<string> MediaUrls { get; set; } = new();
    }
}
