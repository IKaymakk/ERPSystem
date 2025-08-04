using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category", id);

            var categoryDto = _mapper.Map<CategoryDto>(category);

            categoryDto.HasChildren = await _categoryRepository.HasChildrenAsync(id);
            categoryDto.ProductCount = await _categoryRepository.GetProductCountAsync(id);
            categoryDto.Level = await _categoryRepository.GetLevelAsync(id);
            categoryDto.FullPath = await GetCategoryFullPathAsync(id);

            return categoryDto;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto createDto)
        {
            // Business validations
            if (await _categoryRepository.ExistsByCodeAsync(createDto.Code))
                throw new BusinessException($"Kategori kodu '{createDto.Code}' zaten kullanılıyor.");

            if (await _categoryRepository.ExistsByNameAsync(createDto.Name))
                throw new BusinessException($"Kategori adı '{createDto.Name}' zaten kullanılıyor.");

            // Parent validation
            if (createDto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(createDto.ParentCategoryId.Value);
                if (parent == null || !parent.IsActive)
                    throw new BusinessException("Geçersiz üst kategori seçildi.");

                // Level kontrolü (maksimum 5 seviye)
                var parentLevel = await _categoryRepository.GetLevelAsync(createDto.ParentCategoryId.Value);
                if (parentLevel >= 4) // 0,1,2,3,4 = 5 seviye
                    throw new BusinessException("Maksimum 5 seviye kategori hiyerarşisi oluşturulabilir.");
            }

            var category = _mapper.Map<Core.Entities.Category>(createDto);
            category.Code = createDto.Code.ToUpper(); // Kod her zaman büyük harf

            var createdCategory = await _categoryRepository.AddAsync(category);
            return await GetByIdAsync(createdCategory.Id);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateDto)
        {
            var category = await _categoryRepository.GetByIdForUpdateAsync(id);
            if (category == null)
                throw new NotFoundException("Category", id);

            // Business validations
            if (await _categoryRepository.ExistsByCodeAsync(updateDto.Code, id))
                throw new BusinessException($"Kategori kodu '{updateDto.Code}' zaten kullanılıyor.");

            if (await _categoryRepository.ExistsByNameAsync(updateDto.Name, id))
                throw new BusinessException($"Kategori adı '{updateDto.Name}' zaten kullanılıyor.");

            // Parent validation
            if (updateDto.ParentCategoryId.HasValue)
            {
                // Kendisini parent yapamaz
                if (updateDto.ParentCategoryId.Value == id)
                    throw new BusinessException("Kategori kendisinin üst kategorisi olamaz.");

                // Alt kategorilerinden birini parent yapamaz (circular reference)
                if (await _categoryRepository.IsDescendantOfAsync(updateDto.ParentCategoryId.Value, id))
                    throw new BusinessException("Döngüsel referans yaratılamaz. Alt kategorilerden biri üst kategori olarak seçilemez.");

                var parent = await _categoryRepository.GetByIdAsync(updateDto.ParentCategoryId.Value);
                if (parent == null || !parent.IsActive)
                    throw new BusinessException("Geçersiz üst kategori seçildi.");

                // Level kontrolü
                var parentLevel = await _categoryRepository.GetLevelAsync(updateDto.ParentCategoryId.Value);
                if (parentLevel >= 4)
                    throw new BusinessException("Maksimum 5 seviye kategori hiyerarşisi oluşturulabilir.");
            }

            // Update entity
            _mapper.Map(updateDto, category);
            category.Code = updateDto.Code.ToUpper();

            await _categoryRepository.UpdateAsync(category);
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category", id);

            // Business validations
            if (!await CanDeleteAsync(id))
                throw new BusinessException("Bu kategori silinemez. Alt kategoriler veya ürünler mevcut.");

            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<CategoryDto>> GetPagedAsync(CategoryFilterDto filter)
        {
            var pagedCategories = await _categoryRepository.GetPagedCategoriesAsync(filter);

            var categoryDtos = new List<CategoryDto>();
            foreach (var category in pagedCategories.Items)
            {
                var dto = _mapper.Map<CategoryDto>(category);
                dto.HasChildren = await _categoryRepository.HasChildrenAsync(category.Id);
                dto.ProductCount = await _categoryRepository.GetProductCountAsync(category.Id);
                dto.Level = await _categoryRepository.GetLevelAsync(category.Id);
                dto.FullPath = await GetCategoryFullPathAsync(category.Id);
                categoryDtos.Add(dto);
            }

            return new PagedResultDto<CategoryDto>
            {
                Items = categoryDtos,
                TotalCount = pagedCategories.TotalCount,
                PageNumber = pagedCategories.PageNumber,
                PageSize = pagedCategories.PageSize
            };
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var dto = _mapper.Map<CategoryDto>(category);
                dto.HasChildren = await _categoryRepository.HasChildrenAsync(category.Id);
                dto.ProductCount = await _categoryRepository.GetProductCountAsync(category.Id);
                dto.Level = await _categoryRepository.GetLevelAsync(category.Id);
                dto.FullPath = await GetCategoryFullPathAsync(category.Id);
                categoryDtos.Add(dto);
            }

            return categoryDtos;
        }

        public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<CategoryDto>> GetChildCategoriesAsync(int parentId)
        {
            var parent = await _categoryRepository.GetByIdAsync(parentId);
            if (parent == null)
                throw new NotFoundException("Category", parentId);

            var categories = await _categoryRepository.GetChildCategoriesAsync(parentId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync(int? parentId = null, int? maxLevel = null)
        {
            var categories = await _categoryRepository.GetCategoryTreeAsync(parentId, maxLevel);

            var treeDtos = new List<CategoryTreeDto>();
            foreach (var category in categories)
            {
                var treeDto = _mapper.Map<CategoryTreeDto>(category);
                treeDto.HasChildren = await _categoryRepository.HasChildrenAsync(category.Id);
                treeDto.ProductCount = await _categoryRepository.GetProductCountAsync(category.Id);
                treeDto.Level = await _categoryRepository.GetLevelAsync(category.Id);

                // Recursive children mapping
                if (category.SubCategories.Any())
                {
                    treeDto.Children = await MapCategoryTreeChildren(category.SubCategories);
                }

                treeDtos.Add(treeDto);
            }

            return treeDtos;
        }

        private async Task<List<CategoryTreeDto>> MapCategoryTreeChildren(ICollection<Core.Entities.Category> categories)
        {
            var result = new List<CategoryTreeDto>();
            foreach (var category in categories)
            {
                var treeDto = _mapper.Map<CategoryTreeDto>(category);
                treeDto.HasChildren = await _categoryRepository.HasChildrenAsync(category.Id);
                treeDto.ProductCount = await _categoryRepository.GetProductCountAsync(category.Id);
                treeDto.Level = await _categoryRepository.GetLevelAsync(category.Id);

                if (category.SubCategories.Any())
                {
                    treeDto.Children = await MapCategoryTreeChildren(category.SubCategories);
                }

                result.Add(treeDto);
            }
            return result;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoryPathAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category", categoryId);

            var path = await _categoryRepository.GetCategoryPathAsync(categoryId);
            return _mapper.Map<IEnumerable<CategoryDto>>(path);
        }

        public async Task<string> GetCategoryFullPathAsync(int categoryId)
        {
            var path = await _categoryRepository.GetCategoryPathAsync(categoryId);
            return string.Join(" > ", path.Select(x => x.Name));
        }

        // Validation Operations
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _categoryRepository.ExistsByCodeAsync(code);
        }

        public async Task<bool> ExistsByCodeAsync(string code, int excludeId)
        {
            return await _categoryRepository.ExistsByCodeAsync(code, excludeId);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _categoryRepository.ExistsByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId)
        {
            return await _categoryRepository.ExistsByNameAsync(name, excludeId);
        }

        public async Task<bool> IsDescendantOfAsync(int childId, int ancestorId)
        {
            return await _categoryRepository.IsDescendantOfAsync(childId, ancestorId);
        }

        public async Task<bool> HasChildrenAsync(int categoryId)
        {
            return await _categoryRepository.HasChildrenAsync(categoryId);
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _categoryRepository.HasProductsAsync(categoryId);
        }

        public async Task<bool> CanDeleteAsync(int categoryId)
        {
            // Alt kategorisi veya ürünü varsa silinemez
            var hasChildren = await _categoryRepository.HasChildrenAsync(categoryId);
            var hasProducts = await _categoryRepository.HasProductsAsync(categoryId);

            return !hasChildren && !hasProducts;
        }

        // Statistics
        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _categoryRepository.GetProductCountAsync(categoryId);
        }

        public async Task<int> GetChildrenCountAsync(int categoryId)
        {
            return await _categoryRepository.GetChildrenCountAsync(categoryId);
        }

        public async Task<int> GetLevelAsync(int categoryId)
        {
            return await _categoryRepository.GetLevelAsync(categoryId);
        }
    }
}
