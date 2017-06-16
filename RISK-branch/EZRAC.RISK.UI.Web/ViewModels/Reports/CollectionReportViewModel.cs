using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class CollectionReportViewModel
    {
        public string PayDate { get; set; }
        public string ContractNo { get; set; }
        public string Renter { get; set; }
        public string Collected { get; set; }
        public string TotalPayment { get; set; }
        public string PayFrom { get; set; }
        public string Damage { get; set; }
        public string LossOfUse { get; set; }
        public string AdminFee { get; set; }
        public string DiminFee { get; set; }
        public string OtherFee { get; set; }
        public string SubroTotal { get; set; }
        public string Status { get; set; }
        public string Agent { get; set; }

        public long LocationId { get; set; }
        public string CompanyAbbr { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public long ClaimId { get; set; }
        public long WriteOffId { get; set; }
        public string WriteOffTotal { get; set; }
    }

    [Serializable]
    public class CollectionCustomReportViewModel
    {
        public long LocationId { get; set; }
        public string Location { get; set; }
        public List<CollectionReportViewModel> collectionReportViewModel { get; set; }
        public long LocationTotal { get; set; }
        public string FinalRenterTotal { get; set; }
        public string FinalTotalCollected { get; set; }
        public string FinalTotalDamage { get; set; }
        public string FinalTotalLossOfUse { get; set; }
        public string FinalTotalAdminFee { get; set; }
        public string FinalTotalDiminFee { get; set; }
        public string FinalTotalOtherFee { get; set; }
        public string FinalTotalSubroTotal { get; set; }
        public string FinalTotalClosed { get; set; }
        public string FinalTotalPending { get; set; }
        public string FinalTotalWriteOffTotal { get; set; }
    }
}