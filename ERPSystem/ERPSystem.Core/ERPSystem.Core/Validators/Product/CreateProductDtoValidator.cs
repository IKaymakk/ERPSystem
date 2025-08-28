using ERPSystem.Core.DTOs.Product;
using ERPSystem.Core.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Validators.Product
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductDtoValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Product code is required")
                .Length(1, 50).WithMessage("Product code must be between 1 and 50 characters")
                .MustAsync(BeUniqueCode).WithMessage("Product code already exists");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .Length(1, 200).WithMessage("Product name must be between 1 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required")
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit is required")
                .GreaterThan(0).WithMessage("Unit ID must be greater than 0");

            RuleFor(x => x.SalePrice)
                .GreaterThan(0).WithMessage("Sale price must be greater than 0");

            RuleFor(x => x.PurchasePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Purchase price must be greater than or equal to 0");

            RuleFor(x => x.CurrentStock)
                .GreaterThanOrEqualTo(0).WithMessage("Current stock must be greater than or equal to 0");

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be greater than or equal to 0");

            RuleFor(x => x.Barcode)
                .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters")
                .MustAsync(BeUniqueBarcode).WithMessage("Barcode already exists")
                .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.VatRate)
                .InclusiveBetween(0, 100).WithMessage("VAT rate must be between 0 and 100");

            RuleFor(x => x.ImagePath)
                .MaximumLength(255).WithMessage("Image path cannot exceed 255 characters");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _productRepository.ExistsByCodeAsync(code);
        }

        private async Task<bool> BeUniqueBarcode(string barcode, CancellationToken cancellationToken)
        {
            return !await _productRepository.ExistsByBarcodeAsync(barcode);
        }
    }
}
