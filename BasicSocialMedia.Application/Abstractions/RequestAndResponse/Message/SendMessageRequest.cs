using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;

public class SendMessageRequest
{
    public string? Content { get; set; }
    public List<IFormFile> Images { get; set; } = new();
}
