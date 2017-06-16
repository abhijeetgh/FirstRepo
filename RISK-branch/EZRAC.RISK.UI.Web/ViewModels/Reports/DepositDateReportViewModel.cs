using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class DepositDateReportViewModel
    {
        public string ClaimId { get; set; }
        public string  Claim { get; set; }
        public string Unit { get; set; }
        public string UnitDescription { get; set; }
        public string LossType { get; set; }
        public string ClaimStatus { get; set; }
        public DateTime LossDate { get; set; }
        public string PaidFrom { get; set; }
        public DateTime PaidDate { get; set; }
        public double CheckAmount { get; set; }
        public double Billed { get; set; }
        public double Balance { get; set; }
        public string CompanyAbbr { get; set; }
    }
}