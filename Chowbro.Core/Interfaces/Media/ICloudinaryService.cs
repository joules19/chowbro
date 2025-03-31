using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Core.Interfaces.Media
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<ImageUploadResult> UpdateImageAsync(string publicId, IFormFile newFile); 
        Task<bool> ImageExistsAsync(string publicId);
    }
}