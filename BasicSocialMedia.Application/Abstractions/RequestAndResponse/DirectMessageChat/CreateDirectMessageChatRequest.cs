namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.DirectMessageChat;

public class CreateDirectMessageChatRequest
{
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
}
