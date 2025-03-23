using Microsoft.AspNetCore.Mvc;
using Chowbro.Modules.Accounts.DTOs;
using Chowbro.Modules.Accounts.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Chowbro.Api.Controllers.Areas.Accounts
{
    [Area("Accounts")]
    [Route("api/accounts/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var result = await _authService.RegisterAsync(model);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("verify-registration-otp")]
        public async Task<IActionResult> VerifyRegistrationOtp([FromBody] OtpVerification request)
        {
            var result = await _authService.VerifyRegistrationOtpAsync(request.Email!, request.Otp!);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] OtpRequestDto request)
        {
            var result = await _authService.LoginAsync(request.PhoneNumber!);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login/verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerification request)
        {
            var result = await _authService.VerifyOtpAsync(request.PhoneNumber != null ? request.PhoneNumber! : request.Email!, request.Otp!);
            return StatusCode((int)result.StatusCode, result);
        }
        
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken!);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var result = await _authService.RevokeRefreshTokenAsync(refreshToken);
            return StatusCode((int)result.StatusCode, result);
        }
        
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] OtpRequestDto request)
        {
            var result = await _authService.SendOtpToUserAsync(request.PhoneNumber!, true);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
