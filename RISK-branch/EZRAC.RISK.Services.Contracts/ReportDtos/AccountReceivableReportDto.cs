using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class AccountReceivableReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public double TotalCollected { get; set; }
        public double TotalDue { get; set; }
        public string Renter { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class AccountReceivableCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<AccountReceivableReportDto> accountReceivableReportDto { get; set; }
        public long LocationTotal { get; set; }
        public double FinalRenterTotal { get; set; }
        public double FinalTotalCollected { get; set; }
        public double FinalTotalDue { get; set; }
        public long FinalTotalClaims { get; set; }
    }
}
