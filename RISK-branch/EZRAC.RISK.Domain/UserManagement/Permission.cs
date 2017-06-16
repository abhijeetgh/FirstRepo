using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public long PermissionLevelId { get; set; }

        public long CategoryId { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<PermissionLevel> PermissionLevel { get; set; }

        public ICollection<RiskCategory> RiskCategory { get; set; }
    }
}
