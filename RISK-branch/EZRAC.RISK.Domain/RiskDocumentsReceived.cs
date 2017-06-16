using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskDocumentsReceived
    {
        public long ClaimId { get; set; }
        public Nullable<bool> PoliceReport { get; set; }
        public Nullable<bool> ClaimFolder { get; set; }
        public Nullable<bool> EstimateReceived { get; set; }
        public Nullable<bool> EstimateApproved { get; set; }

        public Claim Claim { get; set; }

    }
}
