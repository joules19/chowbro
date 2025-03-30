using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Chowbro.Modules.Accounts.Commands.Handlers
{
    public class VerifyRegistrationOtpCommandHandler : IRequestHandler<VerifyRegistrationOtpCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyRegistrationOtpCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<bool>> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return ApiResponse<bool>.Fail(false, "User not found.", HttpStatusCode.NotFound);

            if (user.OtpCode == null || user.OtpExpires < DateTime.UtcNow || user.OtpCode != request.Otp)
                return ApiResponse<bool>.Fail(false, "Invalid or expired OTP.", statusCode: HttpStatusCode.BadRequest);

            user.EmailConfirmed = true;
            user.OtpCode = null;
            user.OtpExpires = null;

            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.Success(true, "OTP verified successfully, registration completed.", statusCode: HttpStatusCode.OK);
        }
    }
}