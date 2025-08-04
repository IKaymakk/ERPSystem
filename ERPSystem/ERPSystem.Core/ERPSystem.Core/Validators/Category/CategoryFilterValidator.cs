using ERPSystem.Core.DTOs.Category;
using FluentValidation;

public class CategoryFilterValidator : AbstractValidator<CategoryFilterDto>
{
    public CategoryFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası 1'den küçük olamaz");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Sayfa boyutu 1'den küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz");

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage("Kategori kodu en fazla 20 karakter olabilir");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir");

        RuleFor(x => x.MaxLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Maksimum seviye 0'dan küçük olamaz")
            .LessThanOrEqualTo(10).WithMessage("Maksimum seviye 10'dan büyük olamaz")
            .When(x => x.MaxLevel.HasValue);

        RuleFor(x => x.CreatedDateFrom)
            .LessThanOrEqualTo(x => x.CreatedDateTo).WithMessage("Başlangıç tarihi bitiş tarihinden büyük olamaz")
            .When(x => x.CreatedDateFrom.HasValue && x.CreatedDateTo.HasValue);
    }
}