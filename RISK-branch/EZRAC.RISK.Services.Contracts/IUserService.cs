using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDto> ValidateUserAsync(string userId, string password);
        Task AddUserAsync(UserDto userDto);
        Task AddUserRoleAsync(UserRoleDto role);
        Task<ClaimsIdentity> GetClaimsIdentityAsync(UserDto user);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserRoleDto>> GetUserRoles();
        Task<UserDto> GetUserbyIdAsync(long userId);
        Task<bool> AddUpdatedUserAsync(UserDto userDto);
        Task<bool> DeleteUser(long userId);
        Task<bool> IsUserNameExist(string userName);
        Task<bool> IsUserMapped(long userId);
        Task<IEnumerable<UserActionLogDto>> GetUserActionsLogByClaimIdAsync(long claimId, DateTime fromDate, DateTime toDate);
        bool AddUserActionLog(UserActionLogDto userActionLogDto);
        Task<IEnumerable<LocationDto>> GetUserLocationsByUserId(long userId);
        Task<List<string>> GetUserEmailIdByLocationAsync(long locationId);

        
    }
}
