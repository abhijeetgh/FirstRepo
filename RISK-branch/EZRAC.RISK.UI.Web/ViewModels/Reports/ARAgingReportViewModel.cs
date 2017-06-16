using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class ARAgingReportViewModel
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitNumber { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public string DateOfLoss { get; set; }
        public string TotalCollected { get; set; }
        public string TotalBilled { get; set; }
        public string TotalDue { get; set; }
        public string Renter { get; set; }
        public long NoOfDays { get; set; }
        public string CompanyAbbr { get; set; }
        public string AssignedTo { get; set; }
    }
    [Serializable]
    public class ARAgingCustomReportViewModel
    {
        public DateTime ReportDate { get; set; }
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<ARAgingReportViewModel> ARAgingReportViewModel { get; set; }
        public double TotalClaims { get; set; }
        public double TotalReceivables { get; set; }
    }
}