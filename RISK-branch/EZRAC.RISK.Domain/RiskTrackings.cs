using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskTrackings : BaseEntity
    {
        public long TrackingTypeId { get; set; }
        public string TrackingDescription { get; set; }
        public int SequenceId { get; set; }

        public List<RiskTrackings> Next { get; set; }
        public List<RiskTrackings> Previous { get; set; }

        public RiskTrackingTypes RiskTrackingTypes { get; set; }
        public IList<ClaimTrackings> ClaimTrackings { get; set; }
    }
}
