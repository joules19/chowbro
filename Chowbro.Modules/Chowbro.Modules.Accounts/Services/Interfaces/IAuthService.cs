using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;

namespace Chowbro.Modules.Accounts.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<OtpResponse>> RegisterAsync(RegisterUser model);
        Task<ApiResponse<bool>> VerifyRegistrationOtpAsync(string email, string otp);
        Task<ApiResponse<OtpResponse>> LoginAsync(string contactInfo);
        Task<ApiResponse<string>> SendOtpToUserAsync(string contactInfo, bool isEmail = false);
        Task<ApiResponse<AuthResponse>> VerifyOtpAsync(string contactInfo, string otp);
        Task<ApiResponse<AuthResponse>> GenerateJwtToken(ApplicationUser user);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string token);
        Task<ApiResponse<bool>> RevokeRefreshTokenAsync(string refreshToken);
    }
}