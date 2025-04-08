using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Core.Services.Interfaces.Media
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<ImageUploadResult> UpdateImageAsync(string publicId, IFormFile newFile); 
        Task<bool> ImageExistsAsync(string publicId);
    }
}