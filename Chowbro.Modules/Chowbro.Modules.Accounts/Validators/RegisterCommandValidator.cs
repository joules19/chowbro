using Chowbro.Modules.Accounts.Commands.Auth;
using FluentValidation;

namespace Chowbro.Modules.Accounts.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Model.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Model.PhoneNumber).NotEmpty();
            RuleFor(x => x.Model.FirstName).NotEmpty();
            RuleFor(x => x.Model.LastName).NotEmpty();
        }
    }
}