using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class RevokeRefreshTokenCommand : IRequest<ApiResponse<bool>>
    {
        public string RefreshToken { get; }

        public RevokeRefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}