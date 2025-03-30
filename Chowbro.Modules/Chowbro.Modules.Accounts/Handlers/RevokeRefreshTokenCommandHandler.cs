using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Add this namespace
using System.Net;

namespace Chowbro.Modules.Accounts.Commands.Handlers
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RevokeRefreshTokenCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Fail(false, "Invalid refresh token.", statusCode: HttpStatusCode.NotFound);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.Success(true, "Refresh token revoked successfully.", statusCode: HttpStatusCode.OK);
        }
    }
}