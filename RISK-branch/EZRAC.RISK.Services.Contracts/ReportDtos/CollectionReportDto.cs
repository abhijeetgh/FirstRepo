using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.ReportDtos
{
    [Serializable]
    public class CollectionReportDto
    {
        public DateTime PayDate { get; set; }
        public Nullable<DateTime> CloseDate { get; set; }
        public string ContractNo { get; set; }
        public string Renter { get; set; }
        public double Collected { get; set; }
        public double TotalPayment { get; set; }
        public int ClaimStatusId { get; set; }
        public string PayFrom { get; set; }
        public decimal? Damage { get; set; }
        public decimal? LossOfUse { get; set; }
        public decimal? AdminFee { get; set; }
        public decimal? DiminFee { get; set; }
        public decimal? OtherFee { get; set; }
        public decimal? SubroTotal { get; set; }
        public string Status { get; set; }
        public string Agent { get; set; }

        public long LocationId { get; set; }
        public string CompanyAbbr { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public long ClaimId { get; set; }
        public long? WriteOffId { get; set; }
        public double? WriteOffTotal { get; set; }
    }
    [Serializable]
    public class CollectionCustomReportDto
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<CollectionReportDto> collectionReportDto { get; set; }
        public long LocationTotal { get; set; }
        public double FinalRenterTotal { get; set; }
        public double FinalTotalCollected { get; set; }
        public double? FinalTotalDamage { get; set; }
        public double? FinalTotalLossOfUse { get; set; }
        public double? FinalTotalAdminFee { get; set; }
        public double? FinalTotalDiminFee { get; set; }
        public double? FinalTotalOtherFee { get; set; }
        public double? FinalTotalSubroTotal { get; set; }
        public double? FinalTotalWriteOffTotal { get; set; }
        public long FinalTotalClosed { get; set; }
        public long FinalTotalPending { get; set; }
    }
}
