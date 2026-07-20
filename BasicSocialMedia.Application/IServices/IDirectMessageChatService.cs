using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.Abstractions.RequestAndResponse.DirectMessageChat;

namespace BasicSocialMedia.Application.IServices
{
    public interface IDirectMessageChatService
    {
        Task<Result<List<DirectMessageChatResponse>>> GetDirectMessageChatsForUserAsync(Guid userId);
        Task<Result<DirectMessageChatResponse?>> GetDirectMessageChatByIdAsync(Guid chatId);
        Task<Result<DirectMessageChatWithMessagesResponse?>> GetDirectMessageChatWithMessagesByIdAsync(Guid chatId);
        Task<Result<CreateDirectMessageChatResponse>> CreateDirectMessageChatAsync(Guid userId1, Guid userId2);
    }
}
