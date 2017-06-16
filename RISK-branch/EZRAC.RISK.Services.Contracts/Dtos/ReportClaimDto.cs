using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ReportClaimDto
    {
        public long Claim { get; set; }
        public string Contract { get; set; }
        public string Status { get; set; }
        public string OpeningAgent { get; set; }
        public string ClosedReason { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string DriverName { get; set; }
        public string UserDetail { get; set; }
        public string CompanyAbbr { get; set; }
    }
}
