using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.Services
{
    public class MessageService : IMessageService
    {
        private const int MaxMessageLength = 2000;
        private const string MessageImagesFolderName = "message-images";
        private const string WebpFormat = "webp";

        private readonly IMessageRepository _messageRepository;
        private readonly IDirectMessageChatRepository _directMessageChatRepository;
        private readonly ICurrentTime _currentTime;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        private readonly ICloudinaryService _cloudinaryService;

        public MessageService(
            IMessageRepository messageRepository,
            IDirectMessageChatRepository directMessageChatRepository,
            ICurrentTime currentTime,
            IUnitOfWork unitOfWork,
            IClaimService claimService,
            ICloudinaryService cloudinaryService)
        {
            _messageRepository = messageRepository;
            _directMessageChatRepository = directMessageChatRepository;
            _currentTime = currentTime;
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _cloudinaryService = cloudinaryService;
        }

        // public async Task<List<Message>> GetMessagesForChatAsync(Guid chatId)
        // {
        //     return await _messageRepository.GetMessagesForChatAsync(chatId);
        // }

        // public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        // {
        //     return await _messageRepository.GetMessageByIdAsync(messageId);
        // }

        public async Task<Result<SendMessageResponse>> SendMessageAsync(Guid chatId, SendMessageRequest request)
        {
            var senderId = _claimService.GetCurrentUserId;
            if (senderId == Guid.Empty)
            {
                return Failed("Invalid token.");
            }

            if (chatId == Guid.Empty)
            {
                return Failed("Chat id is required.");
            }

            if (request == null)
            {
                return Failed("Message content or image is required.");
            }

            var content = string.IsNullOrWhiteSpace(request.Content)
                ? null
                : request.Content.Trim();
            var images = request.Images?
                .Where(image => image is { Length: > 0 })
                .ToList() ?? new List<IFormFile>();

            if (content == null && images.Count == 0)
            {
                return Failed("Message content or image is required.");
            }

            if (content?.Length > MaxMessageLength)
            {
                return Failed($"Message content cannot exceed {MaxMessageLength} characters.");
            }

            var chat = await _directMessageChatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return Failed("Direct message chat was not found.");
            }

            if (!IsParticipant(chat, senderId))
            {
                return Failed("You are not a participant of this direct message chat.");
            }

            var now = _currentTime.GetCurrentTime();
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                MessageContent = content,
                SentAt = now,
                CreationDate = now
            };

            var uploadedPublicFileIds = new List<string>();
            var mediaItems = new List<Media>();

            foreach (var image in images)
            {
                var shouldKeepGif = ShouldKeepGifFormat(image);
                var storedFileType = shouldKeepGif ? "image/gif" : "image/webp";
                var uploadFormat = shouldKeepGif ? null : WebpFormat;
                var uploadedImage = await _cloudinaryService.UploadImage(image, MessageImagesFolderName, uploadFormat);

                if (string.IsNullOrWhiteSpace(uploadedImage?.FileUrl)
                    || string.IsNullOrWhiteSpace(uploadedImage.PublicFileId))
                {
                    await DeleteUploadedImages(uploadedPublicFileIds);
                    return Failed(uploadedImage?.ErrorMessage ?? "Could not upload message image.");
                }

                uploadedPublicFileIds.Add(uploadedImage.PublicFileId);
                mediaItems.Add(new Media
                {
                    Id = Guid.NewGuid(),
                    MessageId = message.Id,
                    FileName = BuildStoredFileName(image.FileName, storedFileType),
                    FileUrl = uploadedImage.FileUrl,
                    FileType = storedFileType,
                    FilePublicId = uploadedImage.PublicFileId
                });
            }

            await _messageRepository.AddAsync(message);

            if (mediaItems.Count > 0)
            {
                await _unitOfWork.MediaRepository.AddRangeAsync(mediaItems);
                message.Media = mediaItems;
            }

            try
            {
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                await DeleteUploadedImages(uploadedPublicFileIds);
                throw;
            }

            return new Result<SendMessageResponse>
            {
                Error = 0,
                Message = "Message sent successfully.",
                Data = toSendMessageResponse(message)
            };
        }

        private static SendMessageResponse toSendMessageResponse(Message message)
        {
            return new SendMessageResponse
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                Content = message.MessageContent,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                CreationDate = message.CreationDate,
                ReadAt = message.ReadAt,
                MediaUrls = message.Media
                    .Where(media => !media.IsDeleted)
                    .Select(media => media.FileUrl)
                    .ToList()
            };
        }

        private static bool IsParticipant(DirectMessageChat chat, Guid userId)
        {
            return chat.ParticipantOneId == userId || chat.ParticipantTwoId == userId;
        }

        private static Result<SendMessageResponse> Failed(string message)
        {
            return new Result<SendMessageResponse>
            {
                Error = 1,
                Message = message
            };
        }

        private async Task DeleteUploadedImages(IEnumerable<string> publicFileIds)
        {
            foreach (var publicFileId in publicFileIds)
            {
                if (!string.IsNullOrWhiteSpace(publicFileId))
                {
                    await _cloudinaryService.DeleteFileAsync(publicFileId);
                }
            }
        }

        private static bool ShouldKeepGifFormat(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            return extension == ".gif" && contentType == "image/gif";
        }

        private static string BuildStoredFileName(string fileName, string storedFileType)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                fileNameWithoutExtension = "message-image";
            }

            var storedExtension = storedFileType == "image/gif" ? ".gif" : ".webp";
            return $"{fileNameWithoutExtension}{storedExtension}";
        }
    }
}
