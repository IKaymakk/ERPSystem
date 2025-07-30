using FluentValidation;

namespace ERPSystem.Core.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.")
            .Matches(@"^[a-zA-ZÇĞıİÖŞÜçğıiöşü\s]+$").WithMessage("Ad sadece harf içerebilir.")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.")
            .Matches(@"^[a-zA-ZÇĞıİÖŞÜçğıiöşü\s]+$").WithMessage("Soyad sadece harf içerebilir.")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.Username)
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
            .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olabilir.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.")
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.Phone)
            .Matches(@"^(\+90|0)?[1-9]\d{9}$").WithMessage("Geçerli bir telefon numarası giriniz.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Geçerli bir rol seçiniz.");
    }
}