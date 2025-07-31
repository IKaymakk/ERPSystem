using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<PagedResultDto<Role>> GetPagedRolesAsync(RoleFilterDto filter);
        Task<Role?> GetRoleWithUsersAsync(int roleId);
        Task<List<Role>> GetAllRolesWithUserCountAsync();
        Task<bool> IsRoleAssignedToUsersAsync(int roleId);
    }
}
