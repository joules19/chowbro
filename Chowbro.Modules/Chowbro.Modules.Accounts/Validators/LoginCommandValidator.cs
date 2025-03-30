using Chowbro.Modules.Accounts.Commands.Auth;
using FluentValidation;

namespace Chowbro.Modules.Accounts.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.ContactInfo).NotEmpty();
        }
    }
}