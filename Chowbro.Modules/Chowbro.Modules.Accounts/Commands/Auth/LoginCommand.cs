using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class LoginCommand : IRequest<ApiResponse<OtpResponse>>
    {
        public string ContactInfo { get; }

        public LoginCommand(string contactInfo)
        {
            ContactInfo = contactInfo;
        }
    }
}