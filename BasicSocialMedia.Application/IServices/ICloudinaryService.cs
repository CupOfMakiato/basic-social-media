using BasicSocialMedia.Application.Abstractions.ThirdPartyService.CloudinaryService;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BasicSocialMedia.Application.IServices
{
    public interface ICloudinaryService
    {
        Task<DeletionResult> DeleteFileAsync(string publicId);
        Task<CloudinaryResponse?> UploadImage(IFormFile file, string folderName, string? format = null);
    }
}
