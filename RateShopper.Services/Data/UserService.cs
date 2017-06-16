using RateShopper.Data;
using RateShopper.Domain.DTOs;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Data.Entity;
using RateShopper.Domain.Entities;
using RateShopper;
using RateShopper.Core.Cache;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace RateShopper.Services.Data
{
    public class UserService : BaseService<User>, IUserService
    {
        private IUserRolesService _userRolesService;
        private IUserScrapperSourcesService userScrapperSourcesService;
        private IUserLocationBrandsService userLocationBrandsService;
        public UserService(IEZRACRateShopperContext context, ICacheManager cacheManager, IUserRolesService _userRolesService, IUserScrapperSourcesService userScrapperSourcesService, IUserLocationBrandsService userLocationBrandsService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<User>();
            _cacheManager = cacheManager;
            this._userRolesService = _userRolesService;
            this.userScrapperSourcesService = userScrapperSourcesService;
            this.userLocationBrandsService = userLocationBrandsService;
        }

        public User ValidateUser(string UserName, string Password)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    //Initialize password hasher 
                    PasswordHasher passwordHasher = new PasswordHasher();
                    User user = _context.Users.Where(a => a.UserName == UserName && !a.IsDeleted).ToList().Where(a => passwordHasher.VerifyHashedPassword(a.Password, Password).ToString().ToLower() == "success").Select(a => a).FirstOrDefault<User>();

                    if (user != null)
                    {
                        return user;

                    }
                }
            }

            return null;
        }

        public ClaimsIdentity SignInUser(User user)
        {
            //Create dictionary for User Roles to identify the user role
            Dictionary<long, string> Roles = new Dictionary<long, string>();
            Roles = _context.UserRoles.Where(a => !(a.IsDeleted)).Select(a => new { key = a.ID, value = a.Role }).ToDictionary(a => a.key, a => a.value);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.UserData, user.UserName));
            claims.Add(new Claim(ClaimTypes.Sid, Convert.ToString(user.ID)));
            claims.Add(new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName));
            claims.Add(new Claim("IsAutomation", user.IsAutomationUser.ToString()));
            claims.Add(new Claim("IsTetheringAccess", user.IsTetheringAccess.ToString()));
            claims.Add(new Claim("IsTSDUpdateAccess", Convert.ToString(user.IsTSDUpdateAccess)));
            claims.Add(new Claim("UserName", Convert.ToString(user.UserName)));

            IEnumerable<UserPermissions> userPermissionMapper = (from permission in _context.UserPermissions
                                                                 join permissionmap in _context.UserPermissionMappers on permission.ID equals permissionmap.UserPermissionID
                                                                 where permissionmap.UserID == user.ID && !(permission.IsDeleted)
                                                                 select permission).ToList();
            foreach (var permission in userPermissionMapper)
            {
                claims.Add(new Claim(permission.PermissionKey, "true"));
            }

            if (Roles != null && Roles.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, Roles[user.UserRoleID]));
            }

            ClaimsIdentity claimId = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            return claimId;
        }

        public UserDTO selectedUserData(long userID, long userRoleID)
        {
            UserDTO userDTO = new UserDTO();

            userDTO.UserRoleID = userRoleID;
            userDTO.UserRole = _context.UserRoles.Where(obj => obj.ID == userRoleID).Select(obj => obj.Role).FirstOrDefault(); // _userRolesService.GetById(userRoleID).Role;

            List<UserLocationBrands> userLocationBrands = (from userlocation in _context.UserLocationBrands
                                                           join locationBrand in _context.LocationBrands on userlocation.LocationBrandID equals locationBrand.ID
                                                           where !(locationBrand.IsDeleted) && userlocation.UserID == userID
                                                           select userlocation).ToList();
            //Get UserPermision List
            userDTO.SelectedPermissionID = string.Join(",", (_context.UserPermissionMappers.Where(o => o.UserID == userID).Select(x => x.UserPermissionID).ToArray()));

            //_context.UserLocationBrands.Where(obj => obj.UserID == userID).ToList();
            if (userLocationBrands != null && userLocationBrands.Count() > 0)
            {
                userDTO.LocationBrandID = string.Join(",", userLocationBrands.Select(obj => obj.LocationBrandID).ToList());
            }
            List<UserScrapperSources> userScrapperSource = _context.UserScrapperSources.Where(obj => obj.UserID == userID).ToList();
            if (userScrapperSource != null && userScrapperSource.Count() > 0)
            {
                userDTO.SourceID = string.Join(",", userScrapperSource.Select(obj => obj.ScrapperSourceID).ToList());
            }
            userDTO.PriceMgrLocationID = string.Join(",", _context.LocationPricingManagers.Where(x => x.UserID.Equals(userID)).Select(obj => obj.LocationBrandID).ToList());
            return userDTO;
        }
        public List<User> GetUserList()
        {
            List<User> lstUser = (from user in _context.Users
                                  join userrole in _context.UserRoles on user.UserRoleID equals userrole.ID
                                  where !(userrole.IsDeleted) && !(user.IsDeleted)
                                  select user).ToList();
            return lstUser;
        }
        public long InsertUser(User user, UserDTO userDTO)
        {
            PasswordHasher passwordHasher = new PasswordHasher();
            user.Password = passwordHasher.HashPassword(user.Password);
            user.CreatedBy = userDTO.LoggedUserID;
            user.UpdatedBy = userDTO.LoggedUserID;

            user.CreatedDateTime = DateTime.Now;
            user.UpdatedDateTime = DateTime.Now;

            base.Add(user);
            //_context.SaveChanges();

            long UserID = user.ID;

            if (userDTO.AddUserPermission != null)
            {
                foreach (var ID in userDTO.AddUserPermission.Split(','))
                {
                    UserPermissionMapper userPermissionMapper = new UserPermissionMapper();
                    userPermissionMapper.UserID = UserID;
                    userPermissionMapper.UserPermissionID = long.Parse(ID);
                    _context.UserPermissionMappers.Add(userPermissionMapper);
                }
                _context.SaveChanges();
            }
            if (userDTO.AddLocationBrandID != null)
            {
                foreach (var locationBrandID in userDTO.AddLocationBrandID.Split(','))
                {
                    long LBID = long.Parse(locationBrandID);
                    UserLocationBrands userLocationBrands = new UserLocationBrands();
                    userLocationBrands.LocationBrandID = LBID;
                    userLocationBrands.UserID = UserID;
                    //userLocationBrandsService.Add(userLocationBrands);
                    _context.UserLocationBrands.Add(userLocationBrands);
                }
                _context.SaveChanges();
            }
            if (userDTO.AddSourceID != null)
            {
                foreach (var sourceID in userDTO.AddSourceID.Split(','))
                {
                    long SID = long.Parse(sourceID);
                    UserScrapperSources userSource = new UserScrapperSources();
                    userSource.ScrapperSourceID = SID;
                    userSource.UserID = UserID;
                    //userScrapperSourcesService.Add(userSource);
                    _context.UserScrapperSources.Add(userSource);
                }
                _context.SaveChanges();
            }
            AddLocationPriceManager(userDTO.AddPriceMgrLocationID, UserID);
            return UserID;
        }
        public long UpdateUser(User user, UserDTO userDTO)
        {
            User oUser = _context.Users.Where(obj => obj.ID == user.ID).FirstOrDefault();// base.GetById(user.ID);
            oUser.FirstName = user.FirstName;
            oUser.LastName = user.LastName;
            oUser.EmailAddress = user.EmailAddress;
            oUser.UserName = user.UserName;
            oUser.IsAutomationUser = user.IsAutomationUser;
            oUser.IsTetheringAccess = user.IsTetheringAccess;
            oUser.IsTSDUpdateAccess = user.IsTSDUpdateAccess;
            oUser.UserRoleID = user.UserRoleID;
            if (user.Password != null && user.Password != "")
            {
                PasswordHasher passwordHasher = new PasswordHasher();
                oUser.Password = passwordHasher.HashPassword(user.Password);
            }
            oUser.UpdatedBy = userDTO.LoggedUserID;
            oUser.UpdatedDateTime = DateTime.Now;

            base.Update(oUser);

            #region Delete
            if (userDTO.DeleteUserPermission != null)
            {
                foreach (var ID in userDTO.DeleteUserPermission.Split(','))
                {
                    long permissionID = long.Parse(ID);
                    UserPermissionMapper userPermissionMapper = _context.UserPermissionMappers.Where(obj => obj.UserPermissionID == permissionID && obj.UserID == userDTO.UserID).FirstOrDefault();
                    if (userPermissionMapper != null)
                    {
                        _context.Entry(userPermissionMapper).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(UserPermissionMapper).ToString());
            }
            if (userDTO.DeleteSourceID != null)
            {
                foreach (var ID in userDTO.DeleteSourceID.Split(','))
                {
                    long sourceID = long.Parse(ID);
                    UserScrapperSources userScrapperSources = _context.UserScrapperSources.Where(obj => obj.ScrapperSourceID == sourceID && obj.UserID == userDTO.UserID).FirstOrDefault();
                    if (userScrapperSources != null)
                    {
                        _context.Entry(userScrapperSources).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(UserScrapperSources).ToString());
            }
            if (userDTO.DeletLocationBrandID != null)
            {
                foreach (var ID in userDTO.DeletLocationBrandID.Split(','))
                {
                    long locationBrandID = long.Parse(ID);
                    UserLocationBrands userLocationBrands = _context.UserLocationBrands.Where(obj => obj.LocationBrandID == locationBrandID && obj.UserID == userDTO.UserID).FirstOrDefault();
                    if (userLocationBrands != null)
                    {
                        _context.Entry(userLocationBrands).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(UserLocationBrands).ToString());
            }
            if (userDTO.DeletePriceMgrLocationID != null)
            {
                foreach (var priceMgrLocID in userDTO.DeletePriceMgrLocationID.Split(','))
                {
                    long locationID = long.Parse(priceMgrLocID);
                    LocationPricingManager locationPricingManager = _context.LocationPricingManagers.Where(obj => obj.UserID == userDTO.UserID && obj.LocationBrandID == locationID).FirstOrDefault();
                    if (locationPricingManager != null)
                    {
                        _context.Entry(locationPricingManager).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                _context.SaveChanges();
            }
            #endregion

            #region Add
            if (userDTO.AddUserPermission != null)
            {
                foreach (var ID in userDTO.AddUserPermission.Split(','))
                {
                    UserPermissionMapper userPermissionMapper = new UserPermissionMapper();
                    userPermissionMapper.UserID = userDTO.UserID;
                    userPermissionMapper.UserPermissionID = long.Parse(ID);
                    _context.UserPermissionMappers.Add(userPermissionMapper);
                }
                _context.SaveChanges();
            }
            if (userDTO.AddSourceID != null)
            {
                foreach (var ID in userDTO.AddSourceID.Split(','))
                {
                    UserScrapperSources userScrapperSources = new UserScrapperSources();
                    userScrapperSources.UserID = userDTO.UserID;
                    userScrapperSources.ScrapperSourceID = long.Parse(ID);
                    //userScrapperSourcesService.Add(userScrapperSources);
                    _context.UserScrapperSources.Add(userScrapperSources);
                }
                _context.SaveChanges();
            }
            if (userDTO.AddLocationBrandID != null)
            {
                foreach (var ID in userDTO.AddLocationBrandID.Split(','))
                {
                    UserLocationBrands userLocationBrands = new UserLocationBrands();
                    userLocationBrands.UserID = userDTO.UserID;
                    userLocationBrands.LocationBrandID = long.Parse(ID);
                    //userLocationBrandsService.Add(userLocationBrands);
                    _context.UserLocationBrands.Add(userLocationBrands);
                }
                _context.SaveChanges();
            }

            AddLocationPriceManager(userDTO.AddPriceMgrLocationID, userDTO.UserID);

            #endregion

            return user.ID;
        }

        public void AddLocationPriceManager(string AddPermission, long userID)
        {
            if (AddPermission != null)
            {
                foreach (var priceMgrLocID in AddPermission.Split(','))
                {
                    long locationBrandID = long.Parse(priceMgrLocID);
                    LocationPricingManager locationPricingManager = new Domain.Entities.LocationPricingManager();
                    locationPricingManager = _context.LocationPricingManagers.Where(x => x.LocationBrandID == locationBrandID).SingleOrDefault();
                    if (locationPricingManager != null)
                    {
                        locationPricingManager.UserID = userID;
                        _context.Entry(locationPricingManager).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        locationPricingManager = new Domain.Entities.LocationPricingManager();
                        locationPricingManager.LocationBrandID = locationBrandID;
                        locationPricingManager.UserID = userID;
                        _context.LocationPricingManagers.Add(locationPricingManager);
                    }
                }
                _context.SaveChanges();
            }
        }

        public List<ScrapperSourceDTO> GetScrapperSource(long UserId)
        {
            //var ScrapperSources = _context.ScrapperSources.Join(_context.UserScrapperSources, Source => Source.ID, Users => Users.ScrapperSourceID,
            //    (Source, Users) => new { Code = Source.Code, ID = Source.ID, Name = Source.Name, UserId = Users.UserID })
            //    .Where(Users => Users.UserId == UserId)
            //    .Select(obj => new { Code = obj.Code, ID = obj.ID, Name = obj.Name }).ToList();
            //return ScrapperSources.Select(obj => new ScrapperSource { Code = obj.Code, ID = obj.ID, Name = obj.Name }).ToList();

            var ScrapperSources = _context.ScrapperSources.Join(_context.UserScrapperSources, Source => Source.ID, Users => Users.ScrapperSourceID,
                (Source, Users) => new { Code = Source.Code, ID = Source.ID, Name = Source.Name, UserId = Users.UserID, ProviderId = Source.ProviderId, IsGov = Source.IsGov })
                .Join(_context.Providers, scrapper => scrapper.ProviderId, providers => providers.ID,
                (scrapper, providers) => new { Code = scrapper.Code, ID = scrapper.ID, Name = scrapper.Name, UserId = scrapper.UserId, ProviderCode = providers.Code, ProviderId = providers.ID, IsGov = scrapper.IsGov })
                .Where(Users => Users.UserId == UserId)
                .Select(obj => new { Code = obj.Code, ID = obj.ID, Name = obj.Name, ProviderCode = obj.ProviderCode, ProviderId = obj.ProviderId, IsGov = obj.IsGov }).ToList();
            return ScrapperSources.Select(obj => new ScrapperSourceDTO { Code = obj.Code, ID = obj.ID, Name = obj.Name, ProviderCode = obj.ProviderCode, ProviderId = obj.ProviderId, IsGov = obj.IsGov.HasValue ? obj.IsGov.Value : false }).ToList();
        }

        public List<LocationBrandModel> GetBrandLocation(long userId)
        {

            IQueryable<LocationBrandModel> locationBrands = _context.LocationBrands
                .Join(_context.UserLocationBrands, LB => LB.ID, Users => Users.LocationBrandID, (LB, ULB)
                => new { LocationID = LB.LocationID, LocationBrandAlias = LB.LocationBrandAlias, ID = LB.ID, userID = ULB.UserID, IsDeleted = LB.IsDeleted, BrandID = LB.BrandID })
                .Where(LB1 => LB1.userID == userId && !LB1.IsDeleted)
                .Join(_context.Locations, LB2 => LB2.LocationID, Location => Location.ID, (LB3, L)
                 => new { LocationID = LB3.LocationID, LocationBrandAlias = LB3.LocationBrandAlias, ID = LB3.ID, LocationCode = L.Code, IsDeleted = L.IsDeleted, BrandID = LB3.BrandID })
                 .Where(L1 => !L1.IsDeleted)
         .Select(obj => new LocationBrandModel { LocationID = obj.LocationID, LocationBrandAlias = obj.LocationBrandAlias, ID = obj.ID, LocationCode = obj.LocationCode, BrandID = obj.BrandID }
                   );

            var locationBrand = locationBrands.OrderBy(obj => obj.LocationBrandAlias).ToList<LocationBrandModel>();
            var rentalLengths = _context.RentalLengths.ToList();
            var locationBrandRentalLengths = _context.LocationBrandRentalLength.ToList();
            foreach (var location in locationBrand)
            {

                var rentalLengths2 = (from lBrand in locationBrandRentalLengths
                                      where lBrand.LocationBrandID == location.ID
                                      select lBrand.RentalLengthID).ToList();
                string LORs2 = "";
                List<string> lorList = new List<string>();
                foreach (long lor in rentalLengths2)
                {
                    var lorlist = string.Join(",", ((from rLengths in rentalLengths
                                                     where rLengths.AssociateMappedId == lor
                                                     orderby rLengths.Code descending
                                                     select rLengths.ID).ToList()).Skip(1));
                    if (lorlist != "")
                    {
                        lorList.Add(lorlist);
                    }

                }
                LORs2 = string.Join(",", lorList);
                location.LOR = LORs2;
            }

            return locationBrand.OrderBy(obj => obj.LocationBrandAlias).ToList<LocationBrandModel>();
        }

        public List<UserBrandLocationDTO> GetBrandLocationsByLoggedInUserId(long loggedInUserId)
        {
            if (loggedInUserId <= 0)
            {
                return null;
            }

            IQueryable<UserBrandLocationDTO> locationBrands = _context.LocationBrands.Where(a => !a.IsDeleted)
                .Join(_context.UserLocationBrands.Where(a => a.UserID == loggedInUserId), LB => LB.ID, ULB => ULB.LocationBrandID, (LB, ULB)
                => new { locationBrand = LB, userlocation = ULB })
                .Join(_context.Locations, Locbrand => Locbrand.locationBrand.LocationID, loc => loc.ID, (locbr, loc) =>
                    new { BrandLocationId = locbr.locationBrand.ID, LocationBrandAlias = locbr.locationBrand.LocationBrandAlias, userID = locbr.userlocation.UserID, IsLBDeleted = locbr.locationBrand.IsDeleted, BrandId = locbr.locationBrand.BrandID, Code = loc.Code, BranchCode = locbr.locationBrand.BranchCode })
                .Where(a => a.userID == loggedInUserId)
                .Select(obj => new UserBrandLocationDTO { BrandLocationId = obj.BrandLocationId, Alias = obj.LocationBrandAlias, BrandId = obj.BrandId, LocationCode = obj.BranchCode != null ? obj.BranchCode : obj.Code }
                   ).OrderBy(a => a.Alias);

            return locationBrands.ToList<UserBrandLocationDTO>();

        }

        public List<UserBrandLocationDTO> GetFTBBrandLocationsByLoggedInUserId(long loggedInUserId)
        {
            if (loggedInUserId <= 0)
            {
                return null;
            }

            IQueryable<UserBrandLocationDTO> locationBrands = _context.LocationBrands.Where(d => d.IsFTBDominantBrand)
                .Join(_context.UserLocationBrands, LB => LB.ID, ULB => ULB.LocationBrandID, (LB, ULB)
                => new { BrandLocationId = LB.ID, LocationBrandAlias = LB.LocationBrandAlias, userID = ULB.UserID, IsLBDeleted = LB.IsDeleted, BrandId = LB.BrandID })
                .Where(a => a.userID == loggedInUserId && !a.IsLBDeleted)
         .Select(obj => new UserBrandLocationDTO { BrandLocationId = obj.BrandLocationId, Alias = obj.LocationBrandAlias, BrandId = obj.BrandId }
                   ).OrderBy(a => a.Alias);
            return locationBrands.ToList<UserBrandLocationDTO>();
        }

        public string DeleteUser(long UserID, int statusID, int getLastDaysRecord, long LoggedInUserId)
        {
            string msg = "fail";

            if (UserID != 0 && LoggedInUserId != 0)
            {

                User user = base.GetAll().Where(a => a.ID == UserID).FirstOrDefault();

                if (user != null)
                {
                    user.IsDeleted = true;
                    user.UpdatedBy = LoggedInUserId;
                    user.UpdatedDateTime = DateTime.Now;
                    base.Update(user);
                    msg = "success";
                }
            }
            return msg;
        }

        public async Task<List<LocationBrandJobTether>> GetBrandLocationWithTether(long userId)
        {
            const string getLocationBrandTetherJob = "EXEC GetLocationBrandTetherJob @UserId = @UserId";

            var queryParameters = new[] { new SqlParameter("@UserId", userId) };
            var data = await _context.ExecuteSQLQuery<LocationBrandJobTether>(getLocationBrandTetherJob, queryParameters);

            return data.ToList<LocationBrandJobTether>();


        }
    }
}
