using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class VerifyRegistrationOtpCommand : IRequest<ApiResponse<AuthResponse>>
    {
        public string Email { get; }
        public string Otp { get; }

        public VerifyRegistrationOtpCommand(string email, string otp)
        {
            Email = email;
            Otp = otp;
        }
    }
}