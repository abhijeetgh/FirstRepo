using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class VehicleToBeReleasedReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitDetails { get; set; }
        public Nullable<DateTime> ClosedDate { get; set; }
        public DateTime LossDate { get; set; }
        public DateTime OpenDate { get; set; }
        public string LossType { get; set; }
        public string Status { get; set; }
        public string RiskAgent { get; set; }
        public string CompanyAbbr { get; set; }
    }
}
