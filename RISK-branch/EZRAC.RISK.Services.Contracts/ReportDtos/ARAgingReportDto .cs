using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class ARAgingReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitNumber { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public Nullable<DateTime> DateofLoss { get; set; }
        public double TotalCollected { get; set; }
        public double TotalBilled { get; set; }
        public double TotalDue { get; set; }
        public string Renter { get; set; }
        public int NoOfDays { get; set; }
        public string CompanyAbbr { get; set; }
        public string AssignedTo { get; set; }
    }
    [Serializable]
    public class ARAgingCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<ARAgingReportDto> aRAgingReportDto { get; set; }
        public double TotalClaims { get; set; }
        public double TotalReceivables { get; set; }
        //public long LocationTotal { get; set; }
        //public double FinalRenterTotal { get; set; }
        //public double FinalTotalCollected { get; set; }
        //public double FinalTotalDue { get; set; }
    }
}
