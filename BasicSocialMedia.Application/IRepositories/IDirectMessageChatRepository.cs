using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.IRepositories
{
    public interface IDirectMessageChatRepository : IGenericRepository<DirectMessageChat>
    {
        Task<List<DirectMessageChat>> GetChatsForUserAsync(Guid userId);
        Task<DirectMessageChat?> GetChatByIdAsync(Guid chatId);
        Task<DirectMessageChat?> GetChatWithMessagesByIdAsync(Guid chatId);
        Task<List<DirectMessageChat>> GetChatsBetweenUsersAsync(Guid participantOneId, Guid participantTwoId);
        Task<DirectMessageChat?> GetExistingChatBetweenUsersAsync(Guid participantOneId, Guid participantTwoId);
    }
}
