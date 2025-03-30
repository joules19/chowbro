using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class ResendOtpCommand : IRequest<ApiResponse<string>>
    {
        public string ContactInfo { get; }
        public bool IsEmail { get; }

        public ResendOtpCommand(string contactInfo, bool isEmail)
        {
            ContactInfo = contactInfo;
            IsEmail = isEmail;
        }
    }
}