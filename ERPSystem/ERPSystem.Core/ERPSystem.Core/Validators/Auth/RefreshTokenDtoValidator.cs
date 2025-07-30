using ERPSystem.Core.DTOs.Auth;
using FluentValidation;

namespace ERPSystem.Core.Validators.Auth;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        //RuleFor(x => x.AccessToken)
        //    .NotEmpty().WithMessage("Access token zorunludur.");

        //RuleFor(x => x.RefreshToken)
        //    .NotEmpty().WithMessage("Refresh token zorunludur.");
    }
}