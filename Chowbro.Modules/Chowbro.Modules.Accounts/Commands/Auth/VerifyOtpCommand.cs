using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class VerifyOtpCommand : IRequest<ApiResponse<AuthResponse>>
    {
        public string ContactInfo { get; }
        public string Otp { get; }
        public string DeviceId { get; }


        public VerifyOtpCommand(string contactInfo, string otp, string? deviceId)
        {
            ContactInfo = contactInfo;
            Otp = otp;
            DeviceId = deviceId;
        }
    }
}