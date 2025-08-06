using ERPSystem.Core.DTOs.Unit;
using ERPSystem.Core.Interfaces;
using FluentValidation;

namespace ERPSystem.Core.Validators.Unit
{
    // CreateUnitDto için validator
    public class CreateUnitDtoValidator : AbstractValidator<CreateUnitDto>
    {
        private readonly IUnitRepository _unitRepository;

        public CreateUnitDtoValidator(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;

            // Name validasyonları
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Unit adı zorunludur")
                .Length(2, 50)
                .WithMessage("Unit adı 2-50 karakter arasında olmalıdır")
                .MustAsync(BeUniqueNameAsync)
                .WithMessage("Bu unit adı zaten kullanılmaktadır");

            // Symbol validasyonları  
            RuleFor(x => x.Symbol)
                .NotEmpty()
                .WithMessage("Unit sembolü zorunludur")
                .Length(1, 10)
                .WithMessage("Unit sembolü 1-10 karakter arasında olmalıdır")
                .MustAsync(BeUniqueSymbolAsync)
                .WithMessage("Bu unit sembolü zaten kullanılmaktadır");

            // Description validasyonu
            RuleFor(x => x.Description)
                .MaximumLength(100)
                .WithMessage("Açıklama en fazla 100 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }

        // Async validation: Name benzersizlik kontrolü
        private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true; // Empty kontrolü zaten yukarıda yapıldı

            return !await _unitRepository.ExistsByNameAsync(name);
        }

        // Async validation: Symbol benzersizlik kontrolü  
        private async Task<bool> BeUniqueSymbolAsync(string symbol, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return true; // Empty kontrolü zaten yukarıda yapıldı

            return !await _unitRepository.ExistsBySymbolAsync(symbol);
        }
    }

 
}