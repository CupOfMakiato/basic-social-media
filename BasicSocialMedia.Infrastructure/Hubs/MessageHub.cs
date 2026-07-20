using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BasicSocialMedia.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BasicSocialMedia.Infrastructure.Hubs
{
    public interface IMessageClient
    {
        Task ReceiveDirectMessage(DirectMessagePayload message);
        Task ReceiveConversationMessage(ConversationMessagePayload message);
        Task UserJoinedConversation(ConversationMemberPayload member);
        Task UserLeftConversation(ConversationMemberPayload member);
    }

    public sealed record DirectMessagePayload(
        Guid MessageId,
        Guid SenderUserId,
        Guid RecipientUserId,
        string MessageContent,
        DateTimeOffset SentAtUtc);

    public sealed record ConversationMessagePayload(
        Guid MessageId,
        Guid ConversationId,
        Guid SenderUserId,
        string MessageContent,
        DateTimeOffset SentAtUtc);

    public sealed record ConversationMemberPayload(
        Guid ConversationId,
        Guid UserId,
        DateTimeOffset AtUtc);

    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        private const int MaxMessageLength = 2000;
        private readonly AppDbContext _dbContext;

        public MessageHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            _ = RequireCurrentUserId();
            await base.OnConnectedAsync();
        }

        public async Task SendDirectMessage(Guid recipientUserId, string messageContent)
        {
            var senderUserId = RequireCurrentUserId();
            ValidateMessage(messageContent);
            await EnsureCanSendDirectMessageAsync(senderUserId, recipientUserId);

            var payload = new DirectMessagePayload(
                Guid.NewGuid(),
                senderUserId,
                recipientUserId,
                messageContent,
                DateTimeOffset.UtcNow);

            await Clients.Users(new[]
            {
                senderUserId.ToString(),
                recipientUserId.ToString()
            }).ReceiveDirectMessage(payload);
        }

        public async Task JoinConversation(Guid conversationId)
        {
            var userId = RequireCurrentUserId();
            var groupName = ConversationGroup(conversationId);

            await EnsureConversationMemberAsync(conversationId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).UserJoinedConversation(
                new ConversationMemberPayload(conversationId, userId, DateTimeOffset.UtcNow));
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            var userId = RequireCurrentUserId();
            var groupName = ConversationGroup(conversationId);

            await EnsureConversationMemberAsync(conversationId, userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).UserLeftConversation(
                new ConversationMemberPayload(conversationId, userId, DateTimeOffset.UtcNow));
        }

        public async Task SendConversationMessage(Guid conversationId, string messageContent)
        {
            var senderUserId = RequireCurrentUserId();
            ValidateMessage(messageContent);
            await EnsureConversationMemberAsync(conversationId, senderUserId);

            var payload = new ConversationMessagePayload(
                Guid.NewGuid(),
                conversationId,
                senderUserId,
                messageContent,
                DateTimeOffset.UtcNow);

            await Clients.Group(ConversationGroup(conversationId)).ReceiveConversationMessage(payload);
        }

        private Guid RequireCurrentUserId()
        {
            if (Guid.TryParse(Context.UserIdentifier, out var userId))
            {
                return userId;
            }

            throw new HubException("Authenticated user id is missing or invalid.");
        }

        private static void ValidateMessage(string messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                throw new HubException("Message content is required.");
            }

            if (messageContent.Length > MaxMessageLength)
            {
                throw new HubException($"Message content must be {MaxMessageLength} characters or fewer.");
            }
        }

        private async Task EnsureCanSendDirectMessageAsync(Guid senderUserId, Guid recipientUserId)
        {
            var canSendDirectMessage = await _dbContext.DirectMessageChat
                .AsNoTracking()
                .AnyAsync(chat =>
                    !chat.IsDeleted
                    && ((chat.ParticipantOneId == senderUserId && chat.ParticipantTwoId == recipientUserId)
                        || (chat.ParticipantOneId == recipientUserId && chat.ParticipantTwoId == senderUserId)),
                    Context.ConnectionAborted);

            if (!canSendDirectMessage)
            {
                throw new HubException("Direct message chat was not found or you are not a participant.");
            }
        }

        private async Task EnsureConversationMemberAsync(Guid conversationId, Guid userId)
        {
            var isConversationMember = await _dbContext.DirectMessageChat
                .AsNoTracking()
                .AnyAsync(chat =>
                    chat.Id == conversationId
                    && !chat.IsDeleted
                    && (chat.ParticipantOneId == userId || chat.ParticipantTwoId == userId),
                    Context.ConnectionAborted);

            if (!isConversationMember)
            {
                throw new HubException("Conversation was not found or you are not a member.");
            }
        }

        private static string ConversationGroup(Guid conversationId)
        {
            return $"conversation:{conversationId:N}";
        }
    }
}
