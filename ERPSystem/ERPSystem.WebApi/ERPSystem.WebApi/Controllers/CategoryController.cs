using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        => Ok(await _categoryService.GetAllAsync());

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResultDto<CategoryDto>>> GetPaged([FromQuery] CategoryFilterDto filter)
        => Ok(await _categoryService.GetPagedAsync(filter));

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
        => Ok(await _categoryService.GetByIdAsync(id));

    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetRootCategories()
        => Ok(await _categoryService.GetRootCategoriesAsync());

    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetChildCategories(int parentId)
        => Ok(await _categoryService.GetChildCategoriesAsync(parentId));

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<CategoryTreeDto>>> GetCategoryTree([FromQuery] int? parentId = null, [FromQuery] int? maxLevel = null)
        => Ok(await _categoryService.GetCategoryTreeAsync(parentId, maxLevel));

    [HttpGet("{id}/path")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoryPath(int id)
        => Ok(await _categoryService.GetCategoryPathAsync(id));

    [HttpGet("{id}/fullpath")]
    public async Task<ActionResult<string>> GetCategoryFullPath(int id)
        => Ok(new { fullPath = await _categoryService.GetCategoryFullPathAsync(id) });

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto createDto)
    {
        var category = await _categoryService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto updateDto)
        => Ok(await _categoryService.UpdateAsync(id, updateDto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/can-delete")]
    public async Task<ActionResult<bool>> CanDelete(int id)
        => Ok(new { canDelete = await _categoryService.CanDeleteAsync(id) });

    [HttpGet("{id}/statistics")]
    public async Task<ActionResult> GetStatistics(int id)
    {
        var productCount = await _categoryService.GetProductCountAsync(id);
        var childrenCount = await _categoryService.GetChildrenCountAsync(id);
        var level = await _categoryService.GetLevelAsync(id);
        var hasChildren = await _categoryService.HasChildrenAsync(id);
        var hasProducts = await _categoryService.HasProductsAsync(id);

        return Ok(new
        {
            productCount,
            childrenCount,
            level,
            hasChildren,
            hasProducts
        });
    }
}
