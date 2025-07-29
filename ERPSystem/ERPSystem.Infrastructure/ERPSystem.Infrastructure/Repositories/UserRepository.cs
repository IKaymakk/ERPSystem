using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using ERPSystem.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.DTOs.Common;

namespace ERPSystem.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ErpDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await FirstOrDefaultAsync(
                x => x.Email == email,
                x => x.Role
            );
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await FirstOrDefaultAsync(
                x => x.Username == username,
                x => x.Role
            );
        }

        //public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        //{
        //    return await FirstOrDefaultAsync(
        //        x => x.RefreshToken == refreshToken,
        //        x => x.Role
        //    );
        //}

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await ExistsAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await ExistsAsync(x => x.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int excludeUserId)
        {
            return await ExistsAsync(x => x.Email == email && x.Id != excludeUserId);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, int excludeUserId)
        {
            return await ExistsAsync(x => x.Username == username && x.Id != excludeUserId);
        }

        public async Task<PagedResultDto<User>> GetPagedUsersAsync(UserFilterDto filter)
        {
            //filter 
            Expression<Func<User, bool>>? filterExpression = null;
            if (filter.RoleId.HasValue || filter.IsActive.HasValue ||
                filter.CreatedDateFrom.HasValue || filter.CreatedDateTo.HasValue ||
                !string.IsNullOrEmpty(filter.SearchTerm))
            {
                filterExpression = x =>
                    (!filter.RoleId.HasValue || x.RoleId == filter.RoleId.Value) &&
                    (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value) &&
                    (!filter.CreatedDateFrom.HasValue || x.CreatedDate >= filter.CreatedDateFrom.Value) &&
                    (!filter.CreatedDateTo.HasValue || x.CreatedDate <= filter.CreatedDateTo.Value) &&
                    (string.IsNullOrEmpty(filter.SearchTerm) ||
                     x.FirstName.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                     x.LastName.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                     x.Username.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                     x.Email.ToLower().Contains(filter.SearchTerm.ToLower()));
            }

            //order by
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null;
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "firstname":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.FirstName) :
                            q => q.OrderBy(x => x.FirstName);
                        break;
                    case "lastname":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.LastName) :
                            q => q.OrderBy(x => x.LastName);
                        break;
                    case "email":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Email) :
                            q => q.OrderBy(x => x.Email);
                        break;
                    case "username":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Username) :
                            q => q.OrderBy(x => x.Username);
                        break;
                    case "createddate":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.CreatedDate) :
                            q => q.OrderBy(x => x.CreatedDate);
                        break;
                    default:
                        orderBy = q => q.OrderBy(x => x.Id);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(x => x.Id);
            }

            // Base repository'den veriyi al
            var (data, totalCount) = await GetPagedAsync(
                filter.PageNumber,
                filter.PageSize,
                filterExpression,
                orderBy,
                x => x.Role
            );

            // PagedResultDto'ya dönüştür
            return new PagedResultDto<User>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
        {
            return await FindAsync(
                x => x.RoleId == roleId,
                x => x.Role
            );
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await GetAllAsync(
                filter: x => x.IsActive,
                includes: x => x.Role
            );
        }

        public async Task UpdateLastLoginAsync(int userId, DateTime loginDate)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = loginDate;
                await UpdateAsync(user);
            }
        }

        //public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user != null)
        //    {
        //        user.RefreshToken = refreshToken;
        //        user.RefreshTokenExpiryTime = expiryTime;
        //        await UpdateAsync(user);
        //    }
        //}

        //public async Task ClearRefreshTokenAsync(int userId)
        //{
        //    var user = await GetByIdAsync(userId);
        //    if (user != null)
        //    {
        //        user.RefreshToken = null;
        //        user.RefreshTokenExpiryTime = null;
        //        await UpdateAsync(user);
        //    }
        //}

        // Override Role
        public override async Task<User?> GetByIdAsync(int id, params Expression<Func<User, object>>[] includes)
        {
            var allIncludes = new List<Expression<Func<User, object>>> { x => x.Role };
            if (includes != null && includes.Length > 0)
            {
                allIncludes.AddRange(includes);
            }

            return await base.GetByIdAsync(id, allIncludes.ToArray());
        }
    }
}

