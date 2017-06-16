using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DocumentsReceivedDto
    {
        public long ClaimId { get; set; }
        public Nullable<bool> PoliceReport { get; set; }
        public Nullable<bool> ClaimFolder { get; set; }
        public Nullable<bool> EstimateReceived { get; set; }
        public Nullable<bool> EstimateApproved { get; set; }

    }
}
