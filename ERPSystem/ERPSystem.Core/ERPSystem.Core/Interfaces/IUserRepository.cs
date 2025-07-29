using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Interfaces;

public interface IUserRepository:IGenericRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    //Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email, int excludeUserId);
    Task<bool> ExistsByUsernameAsync(string username, int excludeUserId);
    Task<PagedResultDto<User>> GetPagedUsersAsync(UserFilterDto filter);
    Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task UpdateLastLoginAsync(int userId, DateTime loginDate);
    //Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime);
    //Task ClearRefreshTokenAsync(int userId);
}