using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class UserRoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PermissionIds { get; set; }
    }
    public class RiskCategoryDto
    {
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<PermissionDto> ViewPermission { get; set; }
        public IEnumerable<PermissionDto> EditPermission { get; set; }
        public IEnumerable<PermissionDto> DeletePermission { get; set; }
        public IEnumerable<PermissionDto> CreatePermission { get; set; }
        public IEnumerable<PermissionDto> OtherPermission { get; set; }
    }
}
