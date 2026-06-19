namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;

public class SendMessageResponse
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string? Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ReadAt { get; set; }
    public List<string> MediaUrls { get; set; } = new();
}
