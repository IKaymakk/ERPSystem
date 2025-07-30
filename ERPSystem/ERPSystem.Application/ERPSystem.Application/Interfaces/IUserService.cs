using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.User;

namespace ERPSystem.Application.Interfaces;

public interface IUserService : IGenericService<User>
{
    Task CreateUserAsync(CreateUserDto dto);
    Task<PagedResultDto<UserDto>> GetAllUsersAsync(UserFilterDto filter);
    Task<UpdateUserDto> UpdateUserAsync(int id,UpdateUserDto dto);
    Task<UserDto> GetUserByIdAsync(int id);
}