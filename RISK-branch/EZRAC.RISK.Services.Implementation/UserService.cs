using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class UserService : IUserService
    {
        IGenericRepository<User> _userRepository = null;
        IGenericRepository<UserRole> _userRoleRepository = null;
        IGenericRepository<Permission> _permissionsRepository = null;
        IGenericRepository<UserActionLog> _userActionLogRepository = null;
        IGenericRepository<EZRAC.RISK.Domain.Claim> _claimRepository = null;
        IGenericRepository<Location> _locationRepository = null;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IGenericRepository<Permission> permissionsRepository,
            IGenericRepository<UserActionLog> userActionLogRepository,
            IGenericRepository<EZRAC.RISK.Domain.Claim> claimRepository,
            IGenericRepository<Location> locationRepository
            )
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _permissionsRepository = permissionsRepository;
            _userActionLogRepository = userActionLogRepository;
            _claimRepository = claimRepository;
            _locationRepository = locationRepository;
        }
        /// <summary>
        /// This method is used to Validate username and password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserDto> ValidateUserAsync(string userId, string password)
        {
            UserDto userModel = null;
            PasswordHasher passwordHasher = new PasswordHasher();
            var user = _userRepository.AsQueryable.Where(x => x.UserName.Equals(userId) && !(x.IsDeleted)).ToList().Where(a => passwordHasher.VerifyHashedPassword(a.PasswordHash, password).ToString().ToLower() == "success").SingleOrDefault();

            if (user != null && user.IsActive == true)
            {
                userModel = new UserDto { UserName = user.UserName, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, ErroMsg = "IsValid" };
            }
            else if (user != null && user.IsActive == false)
            {
                userModel = new UserDto { ErroMsg = "IsActive" };
            }
            else
            {
                userModel = new UserDto { ErroMsg = "IsInValid" };
            }
            return userModel;
        }

        /// <summary>
        /// This method is used to ClaimsIdentity for an User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GetClaimsIdentityAsync(UserDto user)
        {
            var claims = new List<System.Security.Claims.Claim>();

            var userObj = _userRepository.AsQueryable.Where(x => x.UserName.Equals(user.UserName) && x.IsActive == true && !(x.IsDeleted)).ToList().SingleOrDefault();

            //var claims = roleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).ToListAsync().Result;
            var userRoleObj = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(u => u.Id.Equals(userObj.UserRoleID)).SingleOrDefault();

            claims.Add(new System.Security.Claims.Claim(ClaimTypes.UserData, user.UserName));
            claims.Add(new System.Security.Claims.Claim(ClaimTypes.Sid, Convert.ToString(userObj.Id)));
            claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, userRoleObj.Name));
            claims.Add(new System.Security.Claims.Claim(ClaimTypes.Name, userObj.FirstName + " " + userObj.LastName));
            var claimsList = userRoleObj.Permissions.Select(x =>
                new System.Security.Claims.Claim(x.Name, x.Key)
                );
            claims.AddRange(claimsList);

            ClaimsIdentity claimIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return claimIdentity;
        }

        /// <summary>
        /// This method is used create new User Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AddUserRoleAsync(UserRoleDto role)
        {
            var userRole = new UserRole { Name = role.Name, IsDeleted = false };
            await _userRoleRepository.InsertAsync(userRole);
        }

        /// <summary>
        /// This method is used to create new User
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task AddUserAsync(UserDto userDto)
        {
            PasswordHasher passwordHasher = new PasswordHasher();

            var user = new User
            {
                UserName = userDto.UserName,
                PasswordHash = passwordHasher.HashPassword(userDto.Password),
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                IsDeleted = false,
                IsActive = userDto.IsActive,
                UserRoleID = userDto.UserRoleID,
                CreatedBy = userDto.CurrentUserId,
                UpdatedBy = userDto.CurrentUserId,
                UpdatedDateTime = DateTime.Now,
                CreatedDateTime = DateTime.Now,
                Locations = await UpdateUsersLocations(userDto)
            };

            var userExist = await _userRepository.AsQueryable.Where(x => x.UserName.Equals(userDto.UserName) && !(x.IsDeleted)).ToListAsync();
            if (userExist.Any())
                return;
            await _userRepository.InsertAsync(user);
        }

        /// <summary>
        /// This method is used to get all active user list
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(x => x.IsDeleted == false);

            var userList = await _userRepository.AsQueryable.IncludeMultiple(x => x.Role).Where(predicate).ToListAsync();


            var userDtos = userList.Select(x => new UserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Password = x.PasswordHash,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                UserRoleID = x.UserRoleID,
                UserRole = x.Role.Name,
                IsActive = x.IsActive
            });


            return (IEnumerable<UserDto>)userDtos;
        }

        public async Task<IEnumerable<UserRoleDto>> GetUserRoles()
        {
            var predicate = PredicateBuilder.True<UserRole>();
            predicate = predicate.And(x => x.IsDeleted == false);

            var userRoleList = _userRoleRepository.AsQueryable.Where(predicate).ToList();
            var userRoleDtos = userRoleList.Select(x => new UserRoleDto
            {
                Id = x.Id,
                Name = x.Name
            });
            return userRoleDtos;
        }

        public async Task<UserDto> GetUserbyIdAsync(long userId)
        {
            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(x => x.IsDeleted == false && x.Id == userId);
            UserDto userDto = new UserDto();
            userDto = await _userRepository.AsQueryable.IncludeMultiple(x => x.Role).Where(predicate).Select(x => new UserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Password = x.PasswordHash,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                UserRoleID = x.UserRoleID,
                UserRole = x.Role.Name,
                IsActive = x.IsActive
            }).SingleOrDefaultAsync();


            return userDto;
        }

        public async Task<bool> AddUpdatedUserAsync(UserDto userDto)
        {
            bool flag = false;
            if (userDto.Id > 0)
            {
                //Update operation
                flag = await UpdateUser(userDto);
            }
            else
            {
                //Insert operation
                await AddUserAsync(userDto);
                flag = true;
            }
            return flag;
        }

        public async Task<bool> DeleteUser(long userId)
        {
            bool flag = false;
            User user = new User();
            user = await _userRepository.GetByIdAsync(userId);
            user.IsDeleted = true;

            await _userRepository.UpdateAsync(user);
            flag = true;

            return flag;
        }

        public async Task<bool> IsUserNameExist(string userName)
        {
            bool flag = false;
            var userExist = await _userRepository.AsQueryable.Where(x => x.UserName.ToLower().Equals(userName.ToLower()) && !(x.IsDeleted)).ToListAsync();
            if (userExist.Any())
            {
                flag = true;
            }
            return flag;
        }
        public async Task<bool> IsUserMapped(long userId)
        {
            bool flag = false;
            var userExist = await _claimRepository.AsQueryable.Where(x => x.AssignedTo == userId && x.IsDeleted == false).ToListAsync();
            if (userExist.Any())
            {
                flag = true;
            }
            return flag;
        }

        //Private Methods
        public async Task<bool> UpdateUser(UserDto userDto)
        {
            bool flag = false;
            PasswordHasher passwordHasher = new PasswordHasher();
            User user = new User();
            user = await _userRepository.GetByIdAsync(userDto.Id);

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            //user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.IsActive = userDto.IsActive;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = passwordHasher.HashPassword(userDto.Password);
            }
            user.UserRoleID = userDto.UserRoleID;
            user.UpdatedDateTime = DateTime.Now;
            user.UpdatedBy = userDto.CurrentUserId;
            user.Locations = await UpdateUsersLocations(userDto);
            await _userRepository.UpdateAsync(user);
            flag = true;
            return flag;
        }
        //End private methods


        public async Task<IEnumerable<UserActionLogDto>> GetUserActionsLogByClaimIdAsync(long claimId, DateTime fromDate, DateTime toDate)
        {
            var predicate = PredicateBuilder.True<UserActionLog>();

            predicate = predicate.And(x => x.ClaimId == claimId);

            if (fromDate == DateTime.MinValue && toDate > DateTime.MinValue)
            {
                predicate = predicate.And(x => x.Date <= toDate);
            }
            else if (fromDate > DateTime.MinValue && toDate > DateTime.MinValue)
            {
                DateTime tempDate = toDate.AddDays(1);
                predicate = predicate.And(x => x.Date >= fromDate && x.Date <= tempDate);
            }
            var userActionModelList = await _userActionLogRepository.AsQueryable.
                                        Include(x => x.User).Where(predicate).OrderByDescending(x => x.Date).ToListAsync();

            var userActionDtos = userActionModelList.Select(x =>
                new UserActionLogDto
                {
                    ClaimId = x.ClaimId,
                    Date = x.Date,
                    Name = x.User.FirstName + " " + x.User.LastName,
                    UserName = x.User.UserName,
                    UserAction = x.UserAction,
                    UserId = x.UserId
                }).ToList();
            return userActionDtos;
        }

        public bool AddUserActionLog(UserActionLogDto userActionLogDto)
        {
            
            var userActionLog = new UserActionLog
            {
                ClaimId = userActionLogDto.ClaimId,
                Date = userActionLogDto.Date,
                UserAction = userActionLogDto.UserAction,
                UserId = userActionLogDto.UserId
            };

            var result = _userActionLogRepository.InsertAsync(userActionLog).Result;

            return result != null;
        }

        private async Task<List<Location>> UpdateUsersLocations(UserDto userDto) 
        {
            var user = await _userRepository.AsQueryable.Include(x => x.Locations).Where(x => x.Id == userDto.Id).FirstOrDefaultAsync();
            var listLocations = new List<Location>();
            if (user != null && user.Locations != null)
            {
                
                foreach (var locationId in userDto.LocationIds)
                {
                    var locationToAdd = await _locationRepository.GetByIdAsync(locationId);
                    listLocations.Add(locationToAdd);
                }
            }
            return listLocations;
        }

        public async Task<IEnumerable<LocationDto>> GetUserLocationsByUserId(long userId)
        {
            IEnumerable<Location> userLocations = await _userRepository.AsQueryable.Include(x => x.Locations).Where(x => x.Id == userId).Select(x => x.Locations).FirstOrDefaultAsync();

            IEnumerable<LocationDto> locationDtos = userLocations.Select(x => new LocationDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            });
            return locationDtos;
        }

        public async Task<List<string>> GetUserEmailIdByLocationAsync(long locationId)
        {
            List<string> emailIdList = new List<string>();
            var location = await _locationRepository.AsQueryable.Include(x => x.Users).Where(x => x.Id == locationId).FirstOrDefaultAsync();

            if (location != null && location.Users != null)
            {
                foreach (var item in location.Users)
                {
                    emailIdList.Add(item.Email);
                }
            }
            return emailIdList;
        }
    }
}
