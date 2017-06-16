using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class PaymentViewModel
    {
        public string Charges { get; set; }
        public string Payments { get; set; }
        public string Estimated { get; set; }
        public string OtherChanrges { get; set; }
        public string AdminFee { get; set; }
        public string DiminFee { get; set; }
        public string AppFee { get; set; }
        public string PayForm { get; set; }
        public string PaidDate { get; set; }
        public string CheckAmount { get; set; }
        public string Billed { get; set; }
        public string Balance { get; set; }
        public string Location { get; set; }
        public string Contract { get; set; }
        public string Claim { get; set; }
        public string CompanyAbbr { get; set; }
    }
}