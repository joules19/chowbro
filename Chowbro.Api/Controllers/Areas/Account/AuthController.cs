using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using static Chowbro.Api.Filters.DeviceValidationFilter;

namespace Chowbro.Api.Controllers.Areas.Account
{
    [Area("Accounts")]
    [Route("api/account/[controller]")]
    [EnableRateLimiting("strict-auth")]
    //[DisableRateLimiting] 
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [SkipDeviceValidationAttribute]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {

            var userDeviceId = HttpContext.Items.TryGetValue("DeviceId", out var deviceId)
           ? deviceId?.ToString() ?? string.Empty
           : string.Empty;

            model.DeviceId = userDeviceId;

            var command = new RegisterCommand(model);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("register/verify-registration-otp")]
        public async Task<IActionResult> VerifyRegistrationOtp([FromBody] OtpVerification request)
        {
            var command = new VerifyRegistrationOtpCommand(request.Email!, request.Otp!);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] OtpRequestDto request)
        {
            var command = new LoginCommand(request.PhoneNumber!);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login/verify-login-otp")]
        [AuthenticationAction]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerification request)
        {
            var userDeviceId = HttpContext.Items.TryGetValue("DeviceId", out var deviceId)
            ? deviceId?.ToString() ?? string.Empty
            : string.Empty;
            var contactInfo = request.PhoneNumber ?? request.Email!;
            var command = new VerifyOtpCommand(contactInfo, request.Otp!, userDeviceId!);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("refresh-token")]
        [AuthenticationAction]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {

            var userDeviceId = HttpContext.Items.TryGetValue("DeviceId", out var deviceId)
            ? deviceId?.ToString() ?? string.Empty
            : string.Empty;
            var command = new RefreshTokenCommand(request.RefreshToken!, userDeviceId!);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var command = new RevokeRefreshTokenCommand(refreshToken);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] OtpRequestDto request)
        {
            var command = new ResendOtpCommand(request.PhoneNumber!, true);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}