using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class StolenRecoveredReportDto
    {
        public long ClaimID { get; set; }
        public string Claim { get; set; }
        public string Unit { get; set; }
        public Nullable<DateTime> RAExpectedCloseDate { get; set; }
        public Nullable<DateTime> ReportedToPD { get; set; }
        public DateTime OpenDate { get; set; }
        public string LossType { get; set; }        
        public string Status { get; set; }
        public string RiskAgent { get; set; }
        public string CompanyAbbr { get; set; }
    }
}
