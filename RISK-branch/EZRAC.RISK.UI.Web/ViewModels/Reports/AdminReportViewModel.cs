using EZRAC.Risk.UI.Web.ViewModels.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class AdminReportViewModel
    {
        public string ReportTypeKey { get; set; }
        public DateTime ReportDate { get; set; }
        public IEnumerable<PaymentViewModel> PaymentViewModels { get; set; }
        public IEnumerable<ClaimViewModel> ReportClaimViewModels { get; set; } //report view model Dto
        public IEnumerable<ClaimInfoViewModel> ClaimInfoViewModels { get; set; } //report master claim view model Dto
        public IEnumerable<UserActionLogViewModel> UserActionLogViewModel { get; set; } 
    }
}