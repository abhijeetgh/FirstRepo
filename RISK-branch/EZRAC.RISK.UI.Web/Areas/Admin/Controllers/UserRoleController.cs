using EZRAC.Risk.UI.Web.Helper;
//using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Models;
using EZRAC.RISK.Util.Common;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.Risk.UI.Web.Attributes;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class UserRoleController : Controller
    {
        private IUserRoleService _userRoleService;
        #region  Private Members

        #endregion
        //
        // GET: /Admin/UserRole/
        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }
        public async Task<ActionResult> Index()
        {
            ViewModels.Admin.UserRoleViewModel userRoleModel = new ViewModels.Admin.UserRoleViewModel();
            //userRoleModel.permissionModel = (await _userRoleService.GetPermissionList()).Select(x => new PermissionModel
            //{
            //    PermissionId = x.PermissionId,
            //    PermissionName = x.PermissionName
            //}).ToList<PermissionModel>();

            userRoleModel.PermissionCategoryModel = (await _userRoleService.GetAllPermissionAsync()).Select(x => new PermissionCategoryModel
            {
                CategoryId = x.CategoryID,
                CategoryName = x.CategoryName,
                ViewPermission = getPermissionList(x.ViewPermission.ToList()),
                CreatePermission = getPermissionList(x.CreatePermission.ToList()),
                EditPermission = getPermissionList(x.EditPermission.ToList()),
                DeletePermission = getPermissionList(x.DeletePermission.ToList()),
                OtherPermission = getPermissionList(x.OtherPermission.ToList())
            }).ToList<PermissionCategoryModel>();

            IEnumerable<UserRoleModel> allUserRoleModel = MappedUserRoleList(await _userRoleService.GetAllUserRoleAsync());
            ViewBag.UserRoleList = allUserRoleModel;
            userRoleModel.UserRoleModel = allUserRoleModel;

            return View(userRoleModel);
        }

        public async Task<ActionResult> GetRolePermissionById(long roleID)
        {
            UserRoleDto userDto = new UserRoleDto();
            userDto = await _userRoleService.GetRolePermissionByIdAsync(roleID);

            return Json(userDto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateRole(string addPermission, string deletePermission, long roleId, bool isCopyCreate, bool isNew, string roleName)
        {
            UserRoleDto userRoleDto = new UserRoleDto();

            if (isCopyCreate || isNew)
            {
                ///This condition includes copy and create and new role creation operation.
                userRoleDto = await _userRoleService.CreateUserRolePermissionAsync(addPermission, roleName);
            }
            else
            {
                //This calling method used for update permission as well as rolename. 
                userRoleDto = await _userRoleService.UpdateUserRolePermissionAsync(addPermission, deletePermission, roleId, roleName);
            }

            UserRoleModel userRoleViewModel = new UserRoleModel();
            userRoleViewModel = MapedUserRoleModel(userRoleDto);

            return Json(userRoleViewModel, JsonRequestBehavior.AllowGet);
        }

        #region perivate method
        private static IEnumerable<UserRoleModel> MappedUserRoleList(IEnumerable<UserRoleDto> userRoleDtoList)
        {
            IEnumerable<UserRoleModel> userRoleModel = null;
            userRoleModel = userRoleDtoList.Select(x => new UserRoleModel
            {
                RoleId = x.Id,
                RoleName = x.Name,
                PermissionIds = x.PermissionIds
            }).ToList<UserRoleModel>();
            return userRoleModel;
        }

        /// <summary>
        /// Used for Mapped view model entity
        /// </summary>
        /// <param name="permissionList"></param>
        /// <returns></returns>
        private static IEnumerable<PermissionModel> getPermissionList(IEnumerable<PermissionDto> permissionList)
        {
            IEnumerable<PermissionModel> permissionDto = null;
            if (permissionList != null && permissionList.Count() > 0)
            {
                permissionDto = permissionList.ToList().Select(x => new PermissionModel
                {
                    PermissionId = x.PermissionId,
                    PermissionName = x.PermissionName,
                    PermissionLevelId = x.PermissionLevelId,
                    CategoryId = x.CategoryId
                });
            }
            return permissionDto;
        }
        private static UserRoleModel MapedUserRoleModel(UserRoleDto userRoleDto)
        {
            UserRoleModel userRoleViewModel = new UserRoleModel();
            userRoleViewModel.RoleId = userRoleDto.Id;
            userRoleViewModel.RoleName = userRoleDto.Name;
            userRoleViewModel.PermissionIds = userRoleDto.PermissionIds;
            return userRoleViewModel;
        }
        #endregion
    }
}