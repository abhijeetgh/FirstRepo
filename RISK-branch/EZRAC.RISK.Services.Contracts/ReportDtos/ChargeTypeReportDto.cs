using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class ChargeTypeReportDto
    {
        public long ClaimID { get; set; }
        public string   Claim { get; set; }
        public double ChargeAmount { get; set; }
        public DateTime LossDate  { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class TSDOpeningAgentName
    {
        public string ContractNo { get; set; }
        public string OpeningAgentName { get; set; }
    }
}
