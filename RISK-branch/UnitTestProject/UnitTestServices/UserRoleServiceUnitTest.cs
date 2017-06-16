using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using System.Threading.Tasks;
using System.Linq;
using EZRAC.RISK.Services.Contracts.Dtos;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class UserRoleServiceUnitTest
    {
        #region Private variables
        IGenericRepository<UserRole> _mockUserRoleRepository = null;
        IGenericRepository<Permission> _mockPermissionsRepository = null;
        IGenericRepository<RolePermission> _mockRolePermissionsRepository = null;
        IGenericRepository<RiskCategory> _mockRiskCategoryRepository = null;

        IUserRoleService _userroleservice = null;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            var userRoles = (DomainBuilder.GetUserRoleInfo());

            userRoles.First().Id = 0;


            _mockUserRoleRepository = new MockGenericRepository<UserRole>(userRoles).SetUpRepository();

            _mockPermissionsRepository = new MockGenericRepository<Permission>(DomainBuilder.GetPermissions()).SetUpRepository();

            _mockRolePermissionsRepository = new MockGenericRepository<RolePermission>(DomainBuilder.GetList<RolePermission>()).SetUpRepository();

            _mockRiskCategoryRepository = new MockGenericRepository<RiskCategory>(DomainBuilder.GetList<RiskCategory>()).SetUpRepository();

            _userroleservice = new UserRoleService(_mockUserRoleRepository, _mockPermissionsRepository, _mockRolePermissionsRepository, _mockRiskCategoryRepository);
        }

        
        [TestMethod]
        public void Test_method_for_get_user_role_permission_by_id()
        {
            long RoleId = _mockRolePermissionsRepository.AsQueryable.Select(x => x.RoleId).Max();

            var rolepermission =  _userroleservice.GetRolePermissionByIdAsync(RoleId).Result;

            Assert.IsNotNull(rolepermission);
        }

        
        [TestMethod]
        public void Test_method_for_get_all_user_role()
        {
            var allusersroles =  _userroleservice.GetAllUserRoleAsync().Result;

            Assert.IsNotNull(allusersroles);

        }


        [TestMethod]
        public void Test_method_for_get_all_user_permission()
        {

            var allpermissions =  _userroleservice.GetAllPermissionAsync().Result;

            Assert.IsNotNull(allpermissions);
        }


        [TestMethod]
        public void Test_method_for_add_and_update_user_role_permission()
        {

            string AddPermission = String.Join(",", _mockPermissionsRepository.AsQueryable.Select(x => x.Id).ToArray());

            string DeletePermission = null;

            long RoleId = _mockRolePermissionsRepository.AsQueryable.Select(x => x.RoleId).Max();

            string RoleName = _mockUserRoleRepository.AsQueryable.Select(x => x.Name).First();

            var updated =  _userroleservice.UpdateUserRolePermissionAsync(AddPermission, DeletePermission, RoleId, RoleName).Result;

            Assert.IsNotNull(updated);
        }


        [TestMethod]
        public void Test_method_for_delete_and_update_user_role_Permission()
        {

            string DeletePermission = String.Join(",", _mockPermissionsRepository.AsQueryable.Select(x => x.Id).ToArray());

            string AddPermission = null;

            long RoleId = _mockRolePermissionsRepository.AsQueryable.Select(x => x.RoleId).Max();

            string RoleName = _mockUserRoleRepository.AsQueryable.Select(x => x.Name).First();

            var delupdated =  _userroleservice.UpdateUserRolePermissionAsync(AddPermission, DeletePermission, RoleId, RoleName).Result;

            Assert.IsNotNull(delupdated);
        }


        [TestMethod]
        public void Test_method_for_create_user_role_permission()
        {

            string AddPermission = String.Join(",", _mockPermissionsRepository.AsQueryable.Select(x => x.Id).ToArray());

            string RoleName = _mockUserRoleRepository.AsQueryable.Select(x => x.Name).First();

            var userrolepermission =  _userroleservice.CreateUserRolePermissionAsync(AddPermission, RoleName).Result;

            Assert.IsNotNull(userrolepermission);
        }
    }
}
