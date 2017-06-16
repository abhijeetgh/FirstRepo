using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IUserRoleService
    {
        Task<UserRoleDto> GetRolePermissionByIdAsync(long RoleId);
        Task<IEnumerable<UserRoleDto>> GetAllUserRoleAsync();
        Task<List<RiskCategoryDto>> GetAllPermissionAsync();

        Task<UserRoleDto> UpdateUserRolePermissionAsync(string AddPermission, string DeletePermission, long RoleId, string RoleName);
        Task<UserRoleDto> CreateUserRolePermissionAsync(string AddPermission, string RoleName);
    }
}
