using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class ClaimViewModel
    {
        public string Claim { get; set; }
        public string Contract { get; set; }
        public string Status { get; set; }
        public string OpeningAgent { get; set; }
        public string ClosedReason { get; set; }
        public string FollowUpDate { get; set; }
        public string OpenDate { get; set; }
        public string CloseDate { get; set; }
        public string DriverName { get; set; }
        public string UserDetail { get; set; }
        public string CompanyAbbr { get; set; }
    }
}