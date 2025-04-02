using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Notifications;
using Chowbro.Core.Models;
using Chowbro.Infrastructure.Helpers;
using Chowbro.Modules.Accounts.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Chowbro.Modules.Accounts.Commands.Handlers
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, ApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;

        public ResendOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService)
        {
            _userManager = userManager;
            _otpService = otpService;
        }

        public async Task<ApiResponse<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Email == request.ContactInfo || u.PhoneNumber == request.ContactInfo);

            if (user == null)
                return ApiResponse<string>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            var otp = OtpHelper.GenerateOtp(4);
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            // await _otpService.SendOtpAsync(request.ContactInfo, otp, request.IsEmail);

            return ApiResponse<string>.Success("OTP resent successfully.", statusCode: HttpStatusCode.OK);
        }
    }
}