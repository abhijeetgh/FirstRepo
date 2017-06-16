using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class WriteOffReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitNumber { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public Nullable<DateTime> ClosedDate { get; set; }
        public double TotalCollected { get; set; }
        public double TotalDue { get; set; }
        public string ClosedReason { get; set; }
        public string Renter { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class WriteOffCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<WriteOffReportDto> writeOffReportDto { get; set; }
        public long LocationTotal { get; set; }
        public double FinalRenterTotal { get; set; }
        public double FinalTotalCollected { get; set; }
        public double FinalTotalDue { get; set; }
        public double FinalTotalClaims { get; set; }
    }
}
