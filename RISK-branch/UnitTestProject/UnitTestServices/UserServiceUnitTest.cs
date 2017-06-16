using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.Risk.Services.Test.Helpers;
using System.Threading.Tasks;
using System.Linq;
using EZRAC.RISK.Services.Implementation;
using EZRAC.RISK.Services.Contracts.Dtos;
using FizzWare.NBuilder;


namespace UnitTestProject.UnitTestServices
{
   
    [TestClass]
    public class UserServiceUnitTest
    {
        #region Private variables

        IGenericRepository<User> _mockUserRepository = null;
        IGenericRepository<UserRole> _mockUserRoleRepository = null;
        IGenericRepository<Permission> _mockPermissionsRepository = null;
        IGenericRepository<UserActionLog> _mockUserActionLogRepository = null;
        IGenericRepository<EZRAC.RISK.Domain.Claim> _mockClaimRepository = null;
        IGenericRepository<Location> _mockLocationRepository = null;

        IUserService _userservice = null;

         #endregion

         [TestInitialize]
         public void Setup()
         {
             _mockUserRepository = new MockGenericRepository<User>(DomainBuilder.GetUsers()).SetUpRepository();

             _mockUserRoleRepository = new MockGenericRepository<UserRole>(DomainBuilder.GetUserRoleInfo()).SetUpRepository();

             _mockPermissionsRepository = new MockGenericRepository<Permission>(DomainBuilder.GetList<Permission>()).SetUpRepository();

             _mockUserActionLogRepository = new MockGenericRepository<UserActionLog>(DomainBuilder.GetList<UserActionLog>()).SetUpRepository();

             _mockClaimRepository = new MockGenericRepository<EZRAC.RISK.Domain.Claim>(DomainBuilder.GetClaims()).SetUpRepository();

             _mockLocationRepository = new MockGenericRepository<Location>(DomainBuilder.GetLocations()).SetUpRepository();


             _userservice = new UserService(_mockUserRepository, _mockUserRoleRepository, _mockPermissionsRepository, _mockUserActionLogRepository, _mockClaimRepository, _mockLocationRepository);
         }



      
        [TestMethod]
        public void Test_method_for_find_invalid_user()
         {
             User user = _mockUserRepository.AsQueryable.FirstOrDefault();
            
             var userDto =  _userservice.ValidateUserAsync(user.UserName, "cybage").Result;

             Assert.IsTrue(userDto.ErroMsg == "IsInValid");
         }


        [TestMethod]
        public void Test_method_for_get_claim_identity()
        {
            

            var claimsIdentity =  _userservice.GetClaimsIdentityAsync(DtoBuilder.Get<UserDto>()).Result;

            Assert.IsNotNull(claimsIdentity);
          
        }

        //[TestMethod]
        //public void AddUserRole()
        //{
        //     _userservice.AddUserRoleAsync(DtoBuilder.Get<UserRoleDto>()).Result;

        //    Assert.IsNotNull(userRole);
        //}


        //[TestMethod]
        //public void AddUser()
        //{

        //    var Addeduser =  _userservice.AddUserAsync(DtoBuilder.Get<UserDto>()).Result;

        //    Assert.IsNotNull(Addeduser);
        //}



        [TestMethod]
        public void Test_method_for_get_all_users()
        {
            var allusers =  _userservice.GetAllUsersAsync().Result;

            Assert.IsNotNull(allusers);
        }


        [TestMethod]
        public void Test_method_for_get_all_user_role()
        {

            var userroles =  _userservice.GetUserRoles().Result;

            Assert.IsNotNull(userroles);
        }


        [TestMethod]
        public void Test_method_for_get_user_details_by_id()
        {

            long userId = _mockUserRepository.AsQueryable.Select(x => x.Id).Max();

            var userbyId =  _userservice.GetUserbyIdAsync(userId);

            Assert.IsNotNull(userbyId);
        }


        [TestMethod]
        public void Test_method_for_add_updated_user()
        {
            UserDto user = new UserDto();

            user.Id = 7;

            var updateduser =  _userservice.AddUpdatedUserAsync(user).Result;

            Assert.IsNotNull(updateduser);
        }

        [TestMethod]
        public void Test_method_for_delete_user()
        {
            long userId = _mockUserRepository.AsQueryable.Select(x => x.Id).Max();

            var deleteduser =  _userservice.DeleteUser(userId).Result;

            Assert.IsTrue(deleteduser);
        }

        [TestMethod]
        public void Test_method_for_verify_the_username_exist_or_not()
        {
            string userName = _mockUserRepository.AsQueryable.Select(x => x.UserName).Max();

            var exist =   _userservice.IsUserNameExist(userName).Result;

            Assert.IsTrue(exist);
        }

        [TestMethod]
        public void Test_method_for_verify_the_username_mapped_or_not()
        {
            long userId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var mapped =  _userservice.IsUserMapped(userId).Result;

            Assert.IsTrue(mapped);
        }

       [TestMethod]
        public void Test_method_for_get_user_actions_log_by_claim_id()
        {


            long claimId = _mockUserActionLogRepository.AsQueryable.Select(x => x.ClaimId).Max();

            DateTime fromDate = _mockUserActionLogRepository.AsQueryable.Select(x => x.Date).First();

            DateTime toDate = _mockUserActionLogRepository.AsQueryable.Select(x => x.Date).Last();

            var useractionlogdto =   _userservice.GetUserActionsLogByClaimIdAsync(claimId, fromDate, toDate);

            Assert.IsNotNull(useractionlogdto);
 
        }

        [TestMethod]
        public void Test_method_for_add_user_action_log()
        {
            UserActionLogDto user = new UserActionLogDto();

            var useractionlog =  _userservice.AddUserActionLog(user);

            Assert.IsTrue(useractionlog);
        }

        [TestMethod]
        public void Test_method_for_get_user_locations_by_user_id()
        {
            long userId = _mockClaimRepository.AsQueryable.Select(x => x.Id).Max();

            var userLocation =  _userservice.GetUserLocationsByUserId(userId).Result;

            Assert.IsNotNull(userLocation);
        }

        [TestMethod]
        public void Test_method_for_get_user_email_bu_location()
        {
            long locationId = _mockLocationRepository.AsQueryable.Select(x => x.Id).Max();

            var useremail =   _userservice.GetUserEmailIdByLocationAsync(locationId).Result;

            Assert.IsNotNull(useremail);

        }
    }
}
