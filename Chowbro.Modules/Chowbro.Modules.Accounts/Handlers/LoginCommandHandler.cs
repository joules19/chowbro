﻿using System.Net;
using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Core.Services.Interfaces.Notifications;
using Chowbro.Infrastructure.Helpers;
using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Modules.Accounts.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<OtpResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService)
        {
            _userManager = userManager;
            _otpService = otpService;
        }

        public async Task<ApiResponse<OtpResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.ContactInfo || u.PhoneNumber == request.ContactInfo);

            if (user == null)
                return ApiResponse<OtpResponse>.Fail(null, "We couldn't find an account with that phone number.", HttpStatusCode.NotFound);

            if (!user.PhoneNumberConfirmed)
            {
                var newOtp = OtpHelper.GenerateOtp(4);
                user.OtpCode = newOtp;
                user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                await _otpService.SendOtpAsync(user.Email!, newOtp, true);

                return ApiResponse<OtpResponse>.Success(new OtpResponse
                {
                    Message = "OTP sent successfully.",
                    RequiresOtp = true
                }, statusCode: HttpStatusCode.OK);
            }
            //if (!user.EmailConfirmed)
            //{
            //    var newOtp = OtpHelper.GenerateOtp();
            //    user.OtpCode = newOtp;
            //    user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            //    await _userManager.UpdateAsync(user);

            //    await _otpService.SendOtpAsync(user.Email!, newOtp, true);

            //    return ApiResponse<OtpResponse>.Fail(new OtpResponse
            //    {
            //        Message = "Email not verified. A new OTP has been sent to your email.",
            //        RequiresOtp = true
            //    }, "Email not verified", HttpStatusCode.Forbidden);
            //}

            var otp = OtpHelper.GenerateOtp(4);
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            // await _otpService.SendOtpAsync(request.ContactInfo, otp, true);

            return ApiResponse<OtpResponse>.Success(new OtpResponse
            {
                Message = "OTP sent successfully.",
                RequiresOtp = true
            }, statusCode: HttpStatusCode.OK);
        }
    }
}