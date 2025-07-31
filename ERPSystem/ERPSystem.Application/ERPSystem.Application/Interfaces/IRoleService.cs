using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.Entities;

namespace ERPSystem.Application.Interfaces;

public interface IRoleService : IGenericService<Role>
{
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<PagedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterDto filter);
    Task<RoleDto> GetRoleByIdAsync(int id);
    Task<RoleDto> UpdateRoleAsync(int roleId, UpdateRoleDto dto);
    Task DeleteRoleAsync(int roleId);
    Task<List<RoleDto>> GetAllRolesForDropdownAsync();
    Task AssignRoleToUserAsync(AssignRoleDto dto);
    Task<RoleDto> GetRoleWithUsersAsync(int roleId);
}