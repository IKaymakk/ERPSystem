using ERPSystem.Core.DTOs.Unit;
using ERPSystem.Core.Interfaces;
using FluentValidation;

namespace ERPSystem.Core.Validators.Unit;

public class UpdateUnitDtoValidator : AbstractValidator<UpdateUnitDto>
{
    private readonly IUnitRepository _unitRepository;

    public UpdateUnitDtoValidator(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;

        // Id validasyonu
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Geçerli bir Unit ID'si gereklidir");

        // Name validasyonları - güncelleme için (mevcut kaydı hariç tut)
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Unit adı zorunludur")
            .Length(2, 50)
            .WithMessage("Unit adı 2-50 karakter arasında olmalıdır")
            .MustAsync(BeUniqueNameForUpdateAsync)
            .WithMessage("Bu unit adı zaten kullanılmaktadır");

        // Symbol validasyonları - güncelleme için (mevcut kaydı hariç tut)
        RuleFor(x => x.Symbol)
            .NotEmpty()
            .WithMessage("Unit sembolü zorunludur")
            .Length(1, 10)
            .WithMessage("Unit sembolü 1-10 karakter arasında olmalıdır")
            .MustAsync(BeUniqueSymbolForUpdateAsync)
            .WithMessage("Bu unit sembolü zaten kullanılmaktadır");

        // Description validasyonu
        RuleFor(x => x.Description)
            .MaximumLength(100)
            .WithMessage("Açıklama en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    // Güncelleme için benzersizlik kontrolü - mevcut kaydı hariç tut
    private async Task<bool> BeUniqueNameForUpdateAsync(UpdateUnitDto dto, string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            return true;

        return !await _unitRepository.ExistsByNameAsync(name, dto.Id);
    }

    private async Task<bool> BeUniqueSymbolForUpdateAsync(UpdateUnitDto dto, string symbol, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return true;

        return !await _unitRepository.ExistsBySymbolAsync(symbol, dto.Id);
    }
}