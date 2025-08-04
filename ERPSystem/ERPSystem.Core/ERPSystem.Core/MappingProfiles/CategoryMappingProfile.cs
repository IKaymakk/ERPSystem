using AutoMapper;
using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.MappingProfiles
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
                .ForMember(dest => dest.FullPath, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.Level, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.HasChildren, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.Children, opt => opt.Ignore()); // Service'te set edilecek

            CreateMap<Category, CategoryTreeDto>()
                .ForMember(dest => dest.Level, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.HasChildren, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore()) // Service'te set edilecek
                .ForMember(dest => dest.Children, opt => opt.Ignore()); // Service'te set edilecek

            // DTO to Entity mappings
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());
        }
    }
}
