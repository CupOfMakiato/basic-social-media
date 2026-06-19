using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;
using BasicSocialMedia.Application.Abstractions.Shared;

namespace BasicSocialMedia.Application.IServices
{
    public interface IMessageService
    {
        Task<Result<SendMessageResponse>> SendMessageAsync(Guid chatId, SendMessageRequest request);
    }
}
