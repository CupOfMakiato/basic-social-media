using BasicSocialMedia.Application.DTOs.User;

namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.DirectMessageChat;

public class DirectMessageChatResponse
{
    public Guid Id { get; set; }
    public Guid ParticipantOneId { get; set; }
    public ViewUser ParticipantOne { get; set; } = null!;
    public Guid ParticipantTwoId { get; set; }
    public ViewUser ParticipantTwo { get; set; } = null!;
    public DateTime CreationDate { get; set; }
}
