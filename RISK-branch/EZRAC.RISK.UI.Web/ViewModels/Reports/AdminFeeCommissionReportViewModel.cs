using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class AdminFeeCommissionReportViewModel
    {
        public List<ClaimWiseAdminFeeCommissionReportViewModel> ClaimWiseAdminFeeCommissionReportViewModel { get; set; }
        public List<UserWiseAdminFeeCommissionReportViewModel> UserWiseAdminFeeCommissionReportViewModel { get; set; }

        public string TotalContracts { get; set; }
        public string TotalPayments { get; set; }
    }

    [Serializable]
    public class ClaimWiseAdminFeeCommissionReportViewModel
    {
        public long Claim { get; set; }
        public string Contract { get; set; }
        public string Location { get; set; }
        public string CompanyAbbr { get; set; }
        public double Payment { get; set; }
        

    }

    [Serializable]
    public class UserWiseAdminFeeCommissionReportViewModel
    {
        public string Employee { get; set; }
        public string AmountEach { get; set; }
        public string Quantity { get; set; }
        public string Commission { get; set; }

    }

}