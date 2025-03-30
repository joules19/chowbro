using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Notifications;
using Chowbro.Core.Models;
using Chowbro.Infrastructure.Auth;
using Chowbro.Infrastructure.Helpers;
using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Chowbro.Modules.Accounts.Commands.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<OtpResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOtpService _otpService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOtpService otpService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _otpService = otpService;
        }

        public async Task<ApiResponse<OtpResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber);

            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                    return ApiResponse<OtpResponse>.Fail(null, "Email already in use. Please login.", HttpStatusCode.Conflict);

                // Resend OTP instead of creating a new user
                var newOtp = OtpHelper.GenerateOtp();
                existingUser.OtpCode = newOtp;
                existingUser.OtpExpires = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(existingUser);

                // await _otpService.SendOtpAsync(existingUser.Email!, newOtp, true);
                return ApiResponse<OtpResponse>.Success(new OtpResponse
                {
                    RequiresOtp = true,
                    Message = "An OTP has been resent to your email."
                }, statusCode: HttpStatusCode.OK);
            }

            // Generate a password using the email
            string generatedPassword = model.Email;

            // Default to "Customer" if no role is provided
            var userRole = string.IsNullOrEmpty(model.Role) ? Roles.Customer : model.Role;

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                RCNumber = model.RCNumber,
                ReferralCode = model.ReferralCode,
                DateOfBirth = model.DateOfBirth,
                State = model.State,
                City = model.City,
                Country = model.Country,
                Address = model.Address
            };

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, generatedPassword);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<OtpResponse>.Fail(null, "User registration failed.", HttpStatusCode.InternalServerError, result.Errors.Select(e => e.Description).ToList());

            if (await _roleManager.RoleExistsAsync(userRole))
            {
                await _userManager.AddToRoleAsync(user, userRole);
            }

            var otp = OtpHelper.GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            // await _otpService.SendOtpAsync(user.Email, otp, true);

            return ApiResponse<OtpResponse>.Success(new OtpResponse
            {
                RequiresOtp = true,
                Message = "An OTP has been sent to your email."
            }, statusCode: HttpStatusCode.Created);
        }
    }
}