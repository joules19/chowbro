using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;

namespace Chowbro.Modules.Accounts.Commands.Auth
{
    public class RegisterCommand : IRequest<ApiResponse<OtpResponse>>
    {
        public RegisterUser Model { get; }

        public RegisterCommand(RegisterUser model)
        {
            Model = model;
        }
    }
}