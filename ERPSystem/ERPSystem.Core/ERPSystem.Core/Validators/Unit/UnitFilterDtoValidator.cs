using ERPSystem.Core.DTOs.Unit;
using FluentValidation;

namespace ERPSystem.Core.Validators.Unit;

public class UnitFilterDtoValidator : AbstractValidator<UnitFilterDto>
{
    public UnitFilterDtoValidator()
    {
        // PageNumber validasyonu
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Sayfa numarası 1'den büyük olmalıdır");

        // PageSize validasyonu
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Sayfa boyutu 1-100 arasında olmalıdır");

        // SearchTerm validasyonu
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Arama terimi en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        // SortBy validasyonu - sadece belirli alanlar için izin ver
        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage("Geçersiz sıralama alanı. Geçerli alanlar: name, symbol, createddate")
            .When(x => !string.IsNullOrEmpty(x.SortBy));

        // Tarih aralığı validasyonu
        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage("Başlangıç tarihi bitiş tarihinden küçük olmalıdır")
            .When(x => x.CreatedDateFrom.HasValue && x.CreatedDateTo.HasValue);
    }

    // Geçerli sıralama alanları kontrolü
    private bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return true;

        var validSortFields = new[] { "name", "symbol", "createddate" };
        return validSortFields.Contains(sortBy.ToLower());
    }

    // Tarih aralığı mantık kontrolü
    private bool HaveValidDateRange(UnitFilterDto filter)
    {
        if (!filter.CreatedDateFrom.HasValue || !filter.CreatedDateTo.HasValue)
            return true;

        return filter.CreatedDateFrom.Value <= filter.CreatedDateTo.Value;
    }
}