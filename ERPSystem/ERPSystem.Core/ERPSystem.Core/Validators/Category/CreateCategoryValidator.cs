using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.Interfaces;
using FluentValidation;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kategori kodu zorunludur")
            .Length(1, 20).WithMessage("Kategori kodu 1-20 karakter arasında olmalıdır")
            .Matches("^[A-Z0-9_-]+$").WithMessage("Kategori kodu sadece büyük harf, rakam, tire ve alt tire içerebilir")
            .MustAsync(BeUniqueCode).WithMessage("Bu kategori kodu zaten kullanılıyor");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur")
            .Length(1, 100).WithMessage("Kategori adı 1-100 karakter arasında olmalıdır")
            .MustAsync(BeUniqueName).WithMessage("Bu kategori adı zaten kullanılıyor");

        RuleFor(x => x.Description)
            .MaximumLength(255).WithMessage("Açıklama en fazla 255 karakter olabilir");

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(BeValidParentCategory).WithMessage("Geçersiz üst kategori")
            .When(x => x.ParentCategoryId.HasValue);
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _categoryRepository.ExistsByCodeAsync(code);
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _categoryRepository.ExistsByNameAsync(name);
    }

    private async Task<bool> BeValidParentCategory(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        var parent = await _categoryRepository.GetByIdAsync(parentId.Value);
        return parent != null && parent.IsActive;
    }
}