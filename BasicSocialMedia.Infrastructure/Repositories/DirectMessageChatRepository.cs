using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BasicSocialMedia.Infrastructure.Repositories
{
    public class DirectMessageChatRepository : GenericRepository<DirectMessageChat>, IDirectMessageChatRepository
    {
        public DirectMessageChatRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimService claimsService)
            : base(context, timeService, claimsService)
        {
        }

        public Task<List<DirectMessageChat>> GetChatsForUserAsync(Guid userId)
        {
            return QueryWithParticipants()
                .Where(chat =>
                    !chat.IsDeleted
                    && (chat.ParticipantOneId == userId || chat.ParticipantTwoId == userId))
                .ToListAsync();
        }

        public Task<DirectMessageChat?> GetChatByIdAsync(Guid chatId)
        {
            return QueryWithParticipants()
                .FirstOrDefaultAsync(chat => chat.Id == chatId && !chat.IsDeleted);
        }

        public Task<DirectMessageChat?> GetChatWithMessagesByIdAsync(Guid chatId)
        {
            return QueryWithParticipants()
                .Include(chat => chat.Messages)
                    .ThenInclude(message => message.Media)
                .FirstOrDefaultAsync(chat => chat.Id == chatId && !chat.IsDeleted);
        }

        public Task<List<DirectMessageChat>> GetChatsBetweenUsersAsync(Guid participantOneId, Guid participantTwoId)
        {
            return QueryWithParticipants()
                .Where(chat =>
                    (chat.ParticipantOneId == participantOneId && chat.ParticipantTwoId == participantTwoId)
                    || (chat.ParticipantOneId == participantTwoId && chat.ParticipantTwoId == participantOneId))
                .ToListAsync();
        }

        public async Task<DirectMessageChat?> GetExistingChatBetweenUsersAsync(Guid participantOneId, Guid participantTwoId)
        {
            var existingChats = await GetChatsBetweenUsersAsync(participantOneId, participantTwoId);
            return existingChats.FirstOrDefault(chat => !chat.IsDeleted)
                ?? existingChats.FirstOrDefault();
        }

        private IQueryable<DirectMessageChat> QueryWithParticipants()
        {
            return _dbSet
                .Include(chat => chat.ParticipantOne)
                    .ThenInclude(user => user.ProfilePicture)
                .Include(chat => chat.ParticipantTwo)
                    .ThenInclude(user => user.ProfilePicture);
        }
    }
}
