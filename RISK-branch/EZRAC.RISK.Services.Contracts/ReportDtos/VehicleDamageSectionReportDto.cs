using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class VehicleDamageSectionReportDto
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public string UnitNumber { get; set; }
        public Nullable<DateTime> OpenDate { get; set; }
        public double TotalCollected { get; set; }
        public double TotalDue { get; set; }
        public string Renter { get; set; }
        public string Status { get; set; }
        public string VehicleSection { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class VehicleDamageSectionCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<VehicleDamageSectionReportDto> vehicleDamageSectionReportDto { get; set; }
        public long LocationTotal { get; set; }
        public long FinalRenterTotal { get; set; }
        public string FinalTotalCollected { get; set; }
        public string FinalTotalDue { get; set; }
    }
}
