using BasicSocialMedia.Application.Abstractions.ThirdPartyService.CloudinaryService;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Application.Settings.CloudinaryService;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BasicSocialMedia.Application.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinarySetting _cloudinarySetting;
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinarySetting> cloudinaryConfig)
        {
            var account = new Account(cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinarySetting = cloudinaryConfig.Value;
        }

        public async Task<DeletionResult> DeleteFileAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deletionParams);
        }

        public async Task<CloudinaryResponse?> UploadImage(IFormFile file, string folderName, string? format = null)
        {
            if (file is null)
                return null;

            await using var fileStream = file.OpenReadStream();

            var publicId = $"{Guid.NewGuid()}/{Path.GetFileNameWithoutExtension(file.FileName)}";
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, fileStream),
                PublicId = publicId,
                Overwrite = true,
                Folder = folderName
            };

            if (!string.IsNullOrWhiteSpace(format))
            {
                uploadParams.Format = format;
            }

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return new CloudinaryResponse
                {
                    ErrorMessage = uploadResult.Error.Message
                };
            }

            var fileUrl = uploadResult.SecureUrl?.ToString();
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                fileUrl = uploadResult.Url?.ToString();
            }

            var publicFileId = uploadResult.PublicId;
            if (string.IsNullOrWhiteSpace(publicFileId))
            {
                publicFileId = $"{folderName}/{publicId}".Trim('/');
            }

            if (string.IsNullOrWhiteSpace(fileUrl) || string.IsNullOrWhiteSpace(publicFileId))
            {
                return new CloudinaryResponse
                {
                    ErrorMessage = "Cloudinary upload did not return a file URL or public id."
                };
            }

            return new CloudinaryResponse
            {
                FileUrl = fileUrl,
                PublicFileId = publicFileId
            };
        }
    }
}
