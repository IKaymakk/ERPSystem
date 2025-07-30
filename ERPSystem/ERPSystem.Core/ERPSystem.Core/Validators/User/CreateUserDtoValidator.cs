using ERPSystem.Core.DTOs.User;
using FluentValidation;
namespace ERPSystem.Core.Validators.User;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.")
            .Matches(@"^[a-zA-ZÇĞıİÖŞÜçğıiöşü\s]+$").WithMessage("Ad sadece harf içerebilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad zorunludur.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.")
            .Matches(@"^[a-zA-ZÇĞıİÖŞÜçğıiöşü\s]+$").WithMessage("Soyad sadece harf içerebilir.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
            .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olabilir.")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Şifre en fazla 50 karakter olabilir.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası zorunludur.")
            .Matches(@"^(\+90|0)?[1-9]\d{9}$").WithMessage("Geçerli bir telefon numarası giriniz.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Geçerli bir rol seçiniz.");
    }
}