using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponse>>
    {
        public string RefreshToken { get; }
        public string DeviceId { get; }

        public RefreshTokenCommand(string refreshToken, string deviceId)
        {
            RefreshToken = refreshToken;
            DeviceId = deviceId;
        }

    }
}