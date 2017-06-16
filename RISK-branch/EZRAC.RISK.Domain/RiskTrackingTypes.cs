using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskTrackingTypes: BaseEntity
    {
        public string TrackingType { get; set; }

        public long PermissionId { get; set; }

        public IList<RiskTrackings> RiskTrackings { get; set; }

        public Permission Permissions { get; set; }
    }
}
