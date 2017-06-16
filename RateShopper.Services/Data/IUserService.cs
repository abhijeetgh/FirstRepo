using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IUserService : IBaseService<User>
    {
        //Define method signature if the same is not defined in the base
        User ValidateUser(string UserId, string Password);
        ClaimsIdentity SignInUser(User user);
		List<ScrapperSourceDTO> GetScrapperSource(long UserId);
		List<LocationBrandModel> GetBrandLocation(long userId);
        List<User> GetUserList();
        UserDTO selectedUserData(long userID, long userRoleID);
        long InsertUser(User user, UserDTO userDTO);
        long UpdateUser(User user, UserDTO userDTO);
        List<UserBrandLocationDTO> GetBrandLocationsByLoggedInUserId(long loggedInUserId);
        string DeleteUser(long UserID, int statusID, int getLastDaysRecord,long LoggedInUserId);
        Task<List<LocationBrandJobTether>> GetBrandLocationWithTether(long userId);
        List<UserBrandLocationDTO> GetFTBBrandLocationsByLoggedInUserId(long loggedInUserId);
    }
}
