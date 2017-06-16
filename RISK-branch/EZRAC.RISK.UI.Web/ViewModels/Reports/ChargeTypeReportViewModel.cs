using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class ChargeTypeReportViewModel
    {
        public string ClaimID { get; set; }
        public string   Claim { get; set; }
        public double   ChargeAmount { get; set; }
        public DateTime LossDate { get; set; }
        public string   Unit { get; set; }
        public string   Status { get; set; }
        public string CompanyAbbr { get; set; }
    }
}