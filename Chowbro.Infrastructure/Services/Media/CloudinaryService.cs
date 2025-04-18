using Chowbro.Core.Services.Interfaces.Media;
using Chowbro.Infrastructure.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chowbro.Infrastructure.Services.Media
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public CloudinaryService(IOptions<CloudinarySettings> settings, ILogger<CloudinaryService> logger)
        {
            _settings = settings.Value;
            logger.LogDebug($"Cloudinary Config: {_settings.CloudName}, {_settings.ApiKey?.Substring(0, 3)}...");

            if (string.IsNullOrWhiteSpace(_settings.CloudName))
                throw new ApplicationException("Cloudinary CloudName is not configured");

            _cloudinary = new Cloudinary(new Account(
                   _settings.CloudName,
                   _settings.ApiKey,
                   _settings.ApiSecret));

            _cloudinary.Api.Secure = true;
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(800)
                    .Width(800)
                    .Crop("limit"),
                Folder = _settings.UploadFolder,
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("Public ID cannot be empty");

            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<ImageUploadResult> UpdateImageAsync(string publicId, IFormFile newFile)
        {
            // First delete the old image
            await DeleteImageAsync(publicId);

            // Then upload the new one
            return await UploadImageAsync(newFile);
        }

        public async Task<bool> ImageExistsAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return false;

            try
            {
                var result = await _cloudinary.GetResourceAsync(publicId);
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
    }
}