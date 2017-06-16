using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class ChargeBackLossReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public Nullable<DateTime> ClosedDate { get; set; }
        public double TotalCollected { get; set; }
        public double TotalDue { get; set; }
        public string ClosedReason { get; set; }
        public string Renter { get; set; }
        public string AgentName { get; set; }
        public string OpeningAgent { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class ChargeBackLossCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<ChargeBackLossReportDto> chargeBackLossReportDto { get; set; }
        public long LocationTotal { get; set; }
        public long FinalRenterTotal { get; set; }
        public string FinalTotalCollected { get; set; }
        public string FinalTotalDue { get; set; }
        public string FinalTotalClaims { get; set; }
    }
}
