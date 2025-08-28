// ===================================================================
// PRODUCT API CONTROLLER
// ===================================================================

using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.FileUpload;
using ERPSystem.Core.DTOs.Product;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            IFileUploadService fileUploadService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with id: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the product" });
            }
        }

        //// GET: api/Product/code/PRD000001
        //[HttpGet("code/{code}")]
        //public async Task<ActionResult<ProductDto>> GetProductByCode(string code)
        //{
        //    try
        //    {
        //        var product = await _productService.GetByCodeAsync(code);
        //        return Ok(product);
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting product with code: {ProductCode}", code);
        //        return StatusCode(500, new { message = "An error occurred while retrieving the product" });
        //    }
        //}

        //// GET: api/Product/barcode/1234567890123
        //[HttpGet("barcode/{barcode}")]
        //public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
        //{
        //    try
        //    {
        //        var product = await _productService.GetByBarcodeAsync(barcode);
        //        return Ok(product);
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting product with barcode: {Barcode}", barcode);
        //        return StatusCode(500, new { message = "An error occurred while retrieving the product" });
        //    }
        //}

        // GET: api/Product/paged
        [HttpPost("paged")]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetPagedProducts([FromBody] ProductFilterDto filter)
        {
            try
            {
                var result = await _productService.GetPagedAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged products");
                return StatusCode(500, new { message = "An error occurred while retrieving products" });
            }
        }

        // GET: api/Product/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetByCategoryIdAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category: {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred while retrieving products" });
            }
        }

        // GET: api/Product/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetActiveProducts()
        {
            try
            {
                var products = await _productService.GetActiveProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active products");
                return StatusCode(500, new { message = "An error occurred while retrieving active products" });
            }
        }

        // GET: api/Product/search?term=laptop
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string term)
        {
            try
            {
                var products = await _productService.SearchAsync(term);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", term);
                return StatusCode(500, new { message = "An error occurred while searching products" });
            }
        }

        // GET: api/Product/low-stock
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductStockInfoDto>>> GetLowStockProducts()
        {
            try
            {
                var products = await _productService.GetLowStockProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock products");
                return StatusCode(500, new { message = "An error occurred while retrieving low stock products" });
            }
        }

        // GET: api/Product/stock-info
        [HttpGet("stock-info")]
        public async Task<ActionResult<IEnumerable<ProductStockInfoDto>>> GetStockInfo()
        {
            try
            {
                var stockInfo = await _productService.GetStockInfoAsync();
                return Ok(stockInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock info");
                return StatusCode(500, new { message = "An error occurred while retrieving stock information" });
            }
        }

        //// POST: api/Product
        //[HttpPost]
        //public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
        //{
        //    try
        //    {
        //        var product = await _productService.CreateAsync(dto);
        //        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating product");
        //        return StatusCode(500, new { message = "An error occurred while creating the product" });
        //    }
        //}

        //// PUT: api/Product/5
        //[HttpPut("{id}")]
        //public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        //{
        //    if (id != dto.Id)
        //    {
        //        return BadRequest(new { message = "Product ID mismatch" });
        //    }
        //    try
        //    {
        //        var product = await _productService.UpdateAsync(dto);
        //        return Ok(product);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating product with id: {ProductId}", id);
        //        return StatusCode(500, new { message = "An error occurred while updating the product" });
        //    }
        //}

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with id: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the product" });
            }
        }

        //// POST: api/Product/5/upload-image
        //[HttpPost("{id}/upload-image")]
        //public async Task<ActionResult<FileUploadResultDto>> UploadProductImage(int id, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest(new { message = "No file was uploaded" });
        //    }

        //    try
        //    {
        //        // Önce product var mı kontrol et
        //        var productExists = await _productService.GetByIdAsync(id);
        //        if (productExists is null)
        //        {
        //            return NotFound(new { message = "Product not found" });
        //        }

        //        // Dosyayı yükle
        //        using (var stream = file.OpenReadStream())
        //        {
        //            var filePath = await _fileUploadService.UploadProductImageAsync(stream, file.FileName, file.ContentType);

        //            // Product'ın image path'ini güncelle
        //            var product = await _productService.GetByIdAsync(id);
        //            var updateDto = new UpdateProductDto
        //            {
        //                Id = product.Id,
        //                Code = product.Code,
        //                Name = product.Name,
        //                Description = product.Description,
        //                CategoryId = product.CategoryId,
        //                UnitId = product.UnitId,
        //                SalePrice = product.SalePrice,
        //                PurchasePrice = product.PurchasePrice,
        //                MinStockLevel = product.MinStockLevel,
        //                Barcode = product.Barcode,
        //                VatRate = product.VatRate,
        //                ImagePath = filePath,
        //                IsActive = product.IsActive
        //            };

        //            await _productService.UpdateAsync(updateDto);

        //            var result = new FileUploadResultDto
        //            {
        //                Success = true,
        //                FilePath = filePath,
        //                FileName = file.FileName,
        //                FileUrl = _fileUploadService.GetFileUrl(filePath),
        //                FileSize = file.Length,
        //                ContentType = file.ContentType
        //            };

        //            return Ok(result);
        //        }
        //    }
        //    catch (BusinessException ex)
        //    {
        //        return BadRequest(new FileUploadResultDto
        //        {
        //            Success = false,
        //            ErrorMessage = ex.Message
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error uploading image for product: {ProductId}", id);
        //        return StatusCode(500, new FileUploadResultDto
        //        {
        //            Success = false,
        //            ErrorMessage = "An error occurred while uploading the image"
        //        });
        //    }
        //}

        // POST: api/Product/upload-base64-image
        [HttpPost("upload-base64-image")]
        public async Task<ActionResult<FileUploadResultDto>> UploadBase64Image([FromBody] Base64UploadDto dto)
        {
            try
            {
                var filePath = await _fileUploadService.SaveBase64ImageAsync(dto.Base64Data, dto.FileName);

                var result = new FileUploadResultDto
                {
                    Success = true,
                    FilePath = filePath,
                    FileName = dto.FileName,
                    FileUrl = _fileUploadService.GetFileUrl(filePath),
                    ContentType = dto.ContentType
                };

                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new FileUploadResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading base64 image");
                return StatusCode(500, new FileUploadResultDto
                {
                    Success = false,
                    ErrorMessage = "An error occurred while uploading the image"
                });
            }
        }

        // PUT: api/Product/5/stock
        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            if (id != dto.ProductId)
            {
                return BadRequest(new { message = "Product ID mismatch" });
            }

            try
            {
                await _productService.UpdateStockAsync(dto.ProductId, dto.NewStockAmount);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while updating stock" });
            }
        }

        // GET: api/Product/generate-code
        [HttpGet("generate-code")]
        public async Task<ActionResult<string>> GenerateProductCode()
        {
            try
            {
                var code = await _productService.GenerateProductCodeAsync();
                return Ok(new { code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating product code");
                return StatusCode(500, new { message = "An error occurred while generating product code" });
            }
        }

        // GET: api/Product/generate-barcode
        [HttpGet("generate-barcode")]
        public async Task<ActionResult<string>> GenerateBarcode()
        {
            try
            {
                var barcode = await _productService.GenerateBarcodeAsync();
                return Ok(new { barcode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode");
                return StatusCode(500, new { message = "An error occurred while generating barcode" });
            }
        }

        // GET: api/Product/validate-code?code=PRD000001&excludeId=5
        [HttpGet("validate-code")]
        public async Task<ActionResult<bool>> ValidateCode([FromQuery] string code, [FromQuery] int? excludeId)
        {
            try
            {
                var isUnique = await _productService.IsCodeUniqueAsync(code, excludeId);
                return Ok(new { isUnique, message = isUnique ? "Code is available" : "Code already exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating product code: {Code}", code);
                return StatusCode(500, new { message = "An error occurred while validating code" });
            }
        }

        // GET: api/Product/validate-barcode?barcode=1234567890123&excludeId=5
        [HttpGet("validate-barcode")]
        public async Task<ActionResult<bool>> ValidateBarcode([FromQuery] string barcode, [FromQuery] int? excludeId)
        {
            try
            {
                var isUnique = await _productService.IsBarcodeUniqueAsync(barcode, excludeId);
                return Ok(new { isUnique, message = isUnique ? "Barcode is available" : "Barcode already exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating barcode: {Barcode}", barcode);
                return StatusCode(500, new { message = "An error occurred while validating barcode" });
            }
        }
    }


    public class UpdateStockDto
    {
        public int ProductId { get; set; }
        public decimal NewStockAmount { get; set; }
    }
}