using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Product;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services;

public class ProductService : GenericService<Product>, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public ProductService(IProductRepository productRepository, IMapper mapper) : base(productRepository)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<decimal> CalculateProfitMarginAsync(decimal salePrice, decimal purchasePrice)
    {
        if (purchasePrice == 0)
            return 100;

        return Math.Round(((salePrice - purchasePrice) / purchasePrice) * 100, 2);
    }

    public async Task<string> GenerateBarcodeAsync()
    {
        string barcode;
        do
        {
            // 13 haneli EAN barkodu
            var random = new Random();
            barcode = string.Join("", Enumerable.Range(0, 13).Select(_ => random.Next(0, 10)));
        }
        while (await _productRepository.ExistsByBarcodeAsync(barcode));

        return barcode;
    }

    public async Task<string> GenerateProductCodeAsync()
    {
        string code;
        int counter = 1;

        do
        {
            code = $"PRD{counter:D6}"; // PRD000001 formatında
            counter++;
        }
        while (await _productRepository.ExistsByCodeAsync(code));

        return code;
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId)
    {
        var product = await _productRepository.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(product);
    }

    public async Task<IEnumerable<ProductStockInfoDto>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetLowStockProductsAsync();
        return _mapper.Map<IEnumerable<ProductStockInfoDto>>(products.Select(p => new ProductStockInfoDto
        {
            ProductId = p.Id,
            ProductCode = p.Code,
            ProductName = p.Name,
            CurrentStock = p.CurrentStock,
            MinStockLevel = p.MinStockLevel,
            IsLowStock = p.CurrentStock <= p.MinStockLevel,
            UnitSymbol = p.Unit.Symbol
        }));
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(ProductFilterDto filter)
    {
        var pagedResult = await _productRepository.GetPagedProductsAsync(filter);

        return new PagedResultDto<ProductDto>
        {
            Items = _mapper.Map<IEnumerable<ProductDto>>(pagedResult.Items),
            TotalCount = pagedResult.TotalCount,
            PageSize = pagedResult.PageSize,
            PageNumber = pagedResult.PageNumber
        };
    }

    public async Task<IEnumerable<ProductStockInfoDto>> GetStockInfoAsync()
    {
        return await _productRepository.GetStockInfoAsync();
    }

    public async Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeProductId = null)
    {
        if (excludeProductId.HasValue)
            return !await _productRepository.ExistsByBarcodeAsync(barcode, excludeProductId.Value);

        return !await _productRepository.ExistsByBarcodeAsync(barcode);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeProductId = null)
    {
        if (excludeProductId.HasValue)
            return !await _productRepository.ExistsByCodeAsync(code, excludeProductId.Value);

        return !await _productRepository.ExistsByCodeAsync(code);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return new List<ProductDto>();

        var products = await _productRepository.SearchProductsAsync(searchTerm);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task UpdateStockAsync(int productId, decimal newStockAmount)
    {
        if (newStockAmount < 0)
            throw new BusinessException("Stok Bilgisi 0 Dan Küçük Olamaz.");

        await _productRepository.UpdateStockAsync(productId, newStockAmount);
    }

}
