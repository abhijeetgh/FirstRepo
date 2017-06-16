using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class ClaimTrackings : BaseEntity
    {
        public long ClaimId { get; set; }
        public long TrackingId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long TrackingTypeId { get; set; }
        public Nullable <double> timeTaken { get; set; }

        public RiskTrackings RiskTrackings { get; set; }
        public Claim Claim { get; set; }
        public User User { get; set; }
    }
}
