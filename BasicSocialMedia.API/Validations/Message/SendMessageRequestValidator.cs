using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.API.Validations.Message;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    private const int MaxMessageLength = 2000;
    private const int MaxImageCount = 10;
    private const int MaxFileSizeInMegabytes = 5;
    private const long MaxFileSizeInBytes = MaxFileSizeInMegabytes * 1024L * 1024L;

    public SendMessageRequestValidator()
    {
        RuleFor(request => request)
            .Must(HaveContentOrImages)
            .WithMessage("Message content or image is required.");

        RuleFor(request => request.Content)
            .MaximumLength(MaxMessageLength)
            .WithMessage($"Message content cannot exceed {MaxMessageLength} characters.");

        RuleFor(request => request.Images)
            .Must(images => images == null || images.Count <= MaxImageCount)
            .WithMessage($"A message can include up to {MaxImageCount} images.");

        RuleForEach(request => request.Images)
            .NotNull()
            .WithMessage("Image is required.")
            .Must(file => file!.Length > 0)
            .WithMessage("Image is required.")
            .Must(file => file!.Length <= MaxFileSizeInBytes)
            .WithMessage($"Each image must be {MaxFileSizeInMegabytes} MB or smaller.")
            .Must(HaveAllowedImageType)
            .WithMessage("Only png, jpg, jpeg, and gif images are allowed.");
    }

    private static bool HaveContentOrImages(SendMessageRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Content)
            || request.Images?.Any(file => file is { Length: > 0 }) == true;
    }

    private static bool HaveAllowedImageType(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var header = ReadHeader(file, 12);

        return IsPng(header) && extension == ".png" && contentType == "image/png"
            || IsJpeg(header)
                && (extension == ".jpg" || extension == ".jpeg")
                && (contentType == "image/jpeg" || contentType == "image/jpg")
            || IsGif(header) && extension == ".gif" && contentType == "image/gif";
    }

    private static byte[] ReadHeader(IFormFile file, int length)
    {
        var header = new byte[length];
        using var stream = file.OpenReadStream();
        var offset = 0;

        while (offset < length)
        {
            var read = stream.Read(header, offset, length - offset);
            if (read == 0)
            {
                break;
            }

            offset += read;
        }

        return header;
    }

    private static bool IsPng(IReadOnlyList<byte> header)
    {
        return header.Count >= 8
            && header[0] == 0x89
            && header[1] == 0x50
            && header[2] == 0x4E
            && header[3] == 0x47
            && header[4] == 0x0D
            && header[5] == 0x0A
            && header[6] == 0x1A
            && header[7] == 0x0A;
    }

    private static bool IsJpeg(IReadOnlyList<byte> header)
    {
        return header.Count >= 3
            && header[0] == 0xFF
            && header[1] == 0xD8
            && header[2] == 0xFF;
    }

    private static bool IsGif(IReadOnlyList<byte> header)
    {
        return header.Count >= 6
            && header[0] == 0x47
            && header[1] == 0x49
            && header[2] == 0x46
            && header[3] == 0x38
            && (header[4] == 0x37 || header[4] == 0x39)
            && header[5] == 0x61;
    }
}
