using Chowbro.Modules.Accounts.Commands.Auth;
using FluentValidation;

namespace Chowbro.Modules.Accounts.Validators
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.ContactInfo).NotEmpty();
            RuleFor(x => x.Otp).NotEmpty().Length(6);
        }
    }
}