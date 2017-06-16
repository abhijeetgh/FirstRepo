using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class UserRoleViewModel
    {
        public IEnumerable<UserRoleModel> UserRoleModel { get; set; }
        public IEnumerable<PermissionModel> PermissionModel { get; set; }
        public IEnumerable<PermissionCategoryModel> PermissionCategoryModel { get; set; }
    }
    public class UserRoleModel
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string PermissionIds { get; set; }
    }
    public class PermissionModel
    {
        public long PermissionId { get; set; }
        public string PermissionName { get; set; }
        public long PermissionLevelId { get; set; }
        public long CategoryId { get; set; }
    }
    public class PermissionCategoryModel
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<PermissionModel> ViewPermission { get; set; }
        public IEnumerable<PermissionModel> EditPermission { get; set; }
        public IEnumerable<PermissionModel> DeletePermission { get; set; }
        public IEnumerable<PermissionModel> CreatePermission { get; set; }
        public IEnumerable<PermissionModel> OtherPermission { get; set; }
    }
}