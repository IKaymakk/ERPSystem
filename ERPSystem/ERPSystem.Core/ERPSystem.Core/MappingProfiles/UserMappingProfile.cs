using AutoMapper;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.User;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User -> UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // CreateUserDto -> User
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Will be set in service
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // UpdateUserDto -> User
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Email shouldn't be updated via this DTO
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // User -> LoginResponseDto.User
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap(typeof(PagedResultDto<>), typeof(PagedResultDto<>));
        }
    }
}
