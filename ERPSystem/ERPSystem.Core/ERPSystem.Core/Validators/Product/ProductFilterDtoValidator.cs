using ERPSystem.Core.DTOs.Product;
using FluentValidation;

namespace ERPSystem.Core.Validators.Product;

public class ProductFilterDtoValidator : AbstractValidator<ProductFilterDto>
{
    public ProductFilterDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to 0")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price must be greater than or equal to 0")
            .GreaterThanOrEqualTo(x => x.MinPrice).WithMessage("Maximum price must be greater than or equal to minimum price")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.CreatedDateFrom)
            .LessThanOrEqualTo(x => x.CreatedDateTo).WithMessage("Created date from must be less than or equal to created date to")
            .When(x => x.CreatedDateFrom.HasValue && x.CreatedDateTo.HasValue);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0")
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit ID must be greater than 0")
            .When(x => x.UnitId.HasValue);
    }
}
