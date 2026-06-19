using AutoMapper;
using BasicSocialMedia.Application.Abstractions.RequestAndResponse.DirectMessageChat;
using BasicSocialMedia.Application.DTOs.Message;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Application.DTOs.User;
using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.Services
{
    public class DirectMessageChatService : IDirectMessageChatService
    {
        private readonly IDirectMessageChatRepository _directMessageChatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        private readonly IMapper _mapper;

        public DirectMessageChatService(
            IDirectMessageChatRepository directMessageChatRepository,
            IUnitOfWork unitOfWork,
            IClaimService claimService,
            IMapper mapper)
        {
            _directMessageChatRepository = directMessageChatRepository;
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _mapper = mapper;
        }

        public async Task<Result<List<DirectMessageChatResponse>>> GetDirectMessageChatsForUserAsync(Guid userId)
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return Failed<List<DirectMessageChatResponse>>("Invalid token.");
            }

            if (userId == Guid.Empty)
            {
                return Failed<List<DirectMessageChatResponse>>("User id is required.");
            }

            if (currentUserId != userId)
            {
                return Failed<List<DirectMessageChatResponse>>("You can only retrieve direct message chats for yourself.");
            }

            var chats = await _directMessageChatRepository.GetChatsForUserAsync(userId);
            var response = chats
                .Select(ToDirectMessageChatResponse)
                .ToList();

            return Success("Direct message chats retrieved successfully.", response);
        }

        public async Task<Result<DirectMessageChatResponse?>> GetDirectMessageChatByIdAsync(Guid chatId)
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return Failed<DirectMessageChatResponse?>("Invalid token.");
            }

            if (chatId == Guid.Empty)
            {
                return Failed<DirectMessageChatResponse?>("Chat id is required.");
            }

            var chat = await _directMessageChatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return Failed<DirectMessageChatResponse?>("Direct message chat was not found.");
            }

            if (!IsParticipant(chat, currentUserId))
            {
                return Failed<DirectMessageChatResponse?>("You are not a participant of this direct message chat.");
            }

            return Success<DirectMessageChatResponse?>(
                "Direct message chat retrieved successfully.",
                ToDirectMessageChatResponse(chat));
        }
        public async Task<Result<DirectMessageChatWithMessagesResponse?>> GetDirectMessageChatWithMessagesByIdAsync(Guid chatId)
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return Failed<DirectMessageChatWithMessagesResponse?>("Invalid token.");
            }

            if (chatId == Guid.Empty)
            {
                return Failed<DirectMessageChatWithMessagesResponse?>("Chat id is required.");
            }

            var chat = await _directMessageChatRepository.GetChatWithMessagesByIdAsync(chatId);
            if (chat == null)
            {
                return Failed<DirectMessageChatWithMessagesResponse?>("Direct message chat was not found.");
            }

            if (!IsParticipant(chat, currentUserId))
            {
                return Failed<DirectMessageChatWithMessagesResponse?>("You are not a participant of this direct message chat.");
            }

            return Success<DirectMessageChatWithMessagesResponse?>(
                "Direct message chat retrieved successfully.",
                ToDirectMessageChatWithMessagesResponse(chat));
        }

        public async Task<Result<CreateDirectMessageChatResponse>> CreateDirectMessageChatAsync(Guid userId1, Guid userId2)
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return Failed<CreateDirectMessageChatResponse>("Invalid token.");
            }

            if (userId1 == Guid.Empty || userId2 == Guid.Empty)
            {
                return Failed<CreateDirectMessageChatResponse>("Both participant user ids are required.");
            }

            if (userId1 == userId2)
            {
                return Failed<CreateDirectMessageChatResponse>("A direct message chat requires two different users.");
            }

            if (currentUserId != userId1 && currentUserId != userId2)
            {
                return Failed<CreateDirectMessageChatResponse>("You can only create a direct message chat that includes yourself.");
            }

            var firstUser = await _unitOfWork.UserRepository.GetUserById(userId1);
            var secondUser = await _unitOfWork.UserRepository.GetUserById(userId2);
            if (firstUser == null || firstUser.IsDeleted || secondUser == null || secondUser.IsDeleted)
            {
                return Failed<CreateDirectMessageChatResponse>("One or both participant users were not found.");
            }

            var (participantOneId, participantTwoId) = SortParticipants(userId1, userId2);
            var existingChat = await _directMessageChatRepository.GetExistingChatBetweenUsersAsync(participantOneId, participantTwoId);

            if (existingChat is { IsDeleted: false })
            {
                return Success(
                    "Direct message chat already exists.",
                    ToCreateDirectMessageChatResponse(existingChat));
            }

            if (existingChat is { IsDeleted: true })
            {
                existingChat.IsDeleted = false;
                existingChat.DeletionDate = null;
                existingChat.DeleteBy = null;

                _directMessageChatRepository.Update(existingChat);
                await _unitOfWork.SaveChangeAsync();

                return Success(
                    "Direct message chat created successfully.",
                    ToCreateDirectMessageChatResponse(existingChat));
            }

            var participantOne = participantOneId == userId1 ? firstUser : secondUser;
            var participantTwo = participantTwoId == userId1 ? firstUser : secondUser;
            var directMessageChat = new DirectMessageChat
            {
                Id = Guid.NewGuid(),
                ParticipantOneId = participantOneId,
                ParticipantOne = participantOne,
                ParticipantTwoId = participantTwoId,
                ParticipantTwo = participantTwo
            };

            await _directMessageChatRepository.AddAsync(directMessageChat);
            await _unitOfWork.SaveChangeAsync();

            return Success(
                "Direct message chat created successfully.",
                ToCreateDirectMessageChatResponse(directMessageChat));
        }

        

        private DirectMessageChatResponse ToDirectMessageChatResponse(DirectMessageChat chat)
        {
            return new DirectMessageChatResponse
            {
                Id = chat.Id,
                ParticipantOneId = chat.ParticipantOneId,
                ParticipantOne = _mapper.Map<ViewUser>(chat.ParticipantOne),
                ParticipantTwoId = chat.ParticipantTwoId,
                ParticipantTwo = _mapper.Map<ViewUser>(chat.ParticipantTwo),
                CreationDate = chat.CreationDate
            };
        }

        private CreateDirectMessageChatResponse ToCreateDirectMessageChatResponse(DirectMessageChat chat)
        {
            var response = ToDirectMessageChatResponse(chat);
            return new CreateDirectMessageChatResponse
            {
                Id = response.Id,
                ParticipantOneId = response.ParticipantOneId,
                ParticipantOne = response.ParticipantOne,
                ParticipantTwoId = response.ParticipantTwoId,
                ParticipantTwo = response.ParticipantTwo,
                CreationDate = response.CreationDate
            };
        }
        private DirectMessageChatWithMessagesResponse ToDirectMessageChatWithMessagesResponse(DirectMessageChat chat)
        {
            var lastMessage = chat.Messages?
                .Where(message => !message.IsDeleted)
                .OrderByDescending(message => message.CreationDate)
                .FirstOrDefault();

            return new DirectMessageChatWithMessagesResponse
            {
                Id = chat.Id,
                ParticipantOneId = chat.ParticipantOneId,
                ParticipantOne = _mapper.Map<ViewUser>(chat.ParticipantOne),
                ParticipantTwoId = chat.ParticipantTwoId,
                ParticipantTwo = _mapper.Map<ViewUser>(chat.ParticipantTwo),
                LastMessage = _mapper.Map<MessageDTO?>(lastMessage)
            };
        }

        private static bool IsParticipant(DirectMessageChat chat, Guid userId)
        {
            return chat.ParticipantOneId == userId || chat.ParticipantTwoId == userId;
        }

        private static (Guid ParticipantOneId, Guid ParticipantTwoId) SortParticipants(Guid userId1, Guid userId2)
        {
            return userId1.CompareTo(userId2) <= 0
                ? (userId1, userId2)
                : (userId2, userId1);
        }

        private static Result<T> Success<T>(string message, T data)
        {
            return new Result<T>
            {
                Error = 0,
                Message = message,
                Data = data
            };
        }

        private static Result<T> Failed<T>(string message)
        {
            return new Result<T>
            {
                Error = 1,
                Message = message
            };
        }
    }
}
