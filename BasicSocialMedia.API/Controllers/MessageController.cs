using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicSocialMedia.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("{chatId:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(200, Type = typeof(Result<SendMessageResponse>))]
    [ProducesResponseType(400, Type = typeof(Result<SendMessageResponse>))]
    public async Task<IActionResult> SendMessage(Guid chatId, [FromForm] SendMessageRequest request)
    {
        var result = await _messageService.SendMessageAsync(chatId, request);
        if (result.Error != 0)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
