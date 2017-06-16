using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class PermissionDto
    {
        public long PermissionId { get; set; }
        public string PermissionName { get; set; }
        public long PermissionLevelId { get; set; }
        public long CategoryId { get; set; }
    }
}
