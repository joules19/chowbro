using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Chowbro.Api.Controllers.Areas.Accounts
{
    [Area("Accounts")]
    [Route("api/accounts/[controller]")]
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
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
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
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerification request)
        {
            var contactInfo = request.PhoneNumber ?? request.Email!;
            var command = new VerifyOtpCommand(contactInfo, request.Otp!);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand(request.RefreshToken!);
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