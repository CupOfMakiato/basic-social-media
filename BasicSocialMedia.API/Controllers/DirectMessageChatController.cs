using BasicSocialMedia.Application.Abstractions.RequestAndResponse.DirectMessageChat;
using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicSocialMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DirectMessageChatController : ControllerBase
    {
        private readonly IDirectMessageChatService _directMessageChatService;
        private readonly IClaimService _claimService;
        private readonly IMessageService _messageService;

        public DirectMessageChatController(IDirectMessageChatService directMessageChatService, IClaimService claimService, IMessageService messageService)
        {
            _directMessageChatService = directMessageChatService;
            _claimService = claimService;
            _messageService = messageService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Result<List<DirectMessageChatResponse>>))]
        [ProducesResponseType(400, Type = typeof(Result<List<DirectMessageChatResponse>>))]
        public async Task<IActionResult> GetDirectMessageChatsForUserAsync()
        {
            var userId = _claimService.GetCurrentUserId;
            var result = await _directMessageChatService.GetDirectMessageChatsForUserAsync(userId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{chatId:guid}")]
        [ProducesResponseType(200, Type = typeof(Result<DirectMessageChatResponse?>))]
        [ProducesResponseType(400, Type = typeof(Result<DirectMessageChatResponse?>))]
        public async Task<IActionResult> GetDirectMessageChatById(Guid chatId)
        {
            var result = await _directMessageChatService.GetDirectMessageChatByIdAsync(chatId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{chatId:guid}/messages")]
        [ProducesResponseType(200, Type = typeof(Result<DirectMessageChatWithMessagesResponse?>))]
        [ProducesResponseType(400, Type = typeof(Result<DirectMessageChatWithMessagesResponse?>))]
        public async Task<IActionResult> GetDirectMessageChatWithMessagesById(Guid chatId)
        {
            var result = await _directMessageChatService.GetDirectMessageChatWithMessagesByIdAsync(chatId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Result<CreateDirectMessageChatResponse>))]
        [ProducesResponseType(400, Type = typeof(Result<CreateDirectMessageChatResponse>))]
        public async Task<IActionResult> CreateDirectMessageChat(CreateDirectMessageChatRequest request)
        {
            var result = await _directMessageChatService.CreateDirectMessageChatAsync(request.UserId1, request.UserId2);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
