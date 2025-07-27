using ERPSystem.Core.DTOs.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Validators.Common;

public class PagedRequestDtoValidator : AbstractValidator<PagedRequestDto>
{
    public PagedRequestDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 1'den büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1-100 arasında olmalıdır.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Arama terimi en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));
    }
}