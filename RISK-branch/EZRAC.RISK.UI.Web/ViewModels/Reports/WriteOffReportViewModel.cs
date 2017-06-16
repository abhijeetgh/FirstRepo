﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class WriteOffReportViewModel
    {
        public long Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitNumber { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Code { get; set; }
        public string ClosedDate { get; set; }
        public string TotalCollected { get; set; }
        public string TotalDue { get; set; }
        public string ClosedReason { get; set; }
        public string Renter { get; set; }
        public string CompanyAbbr { get; set; }
    }
    [Serializable]
    public class WriteOffCustomReportViewModel
    {
        public DateTime ReportDate { get; set; }
        public long LocationId { get; set; }
        public string Location { get; set; }
        public IEnumerable<WriteOffReportViewModel> writeOffReportViewModel { get; set; }
        public long LocationTotal { get; set; }
        public string FinalRenterTotal { get; set; }
        public string FinalTotalCollected { get; set; }
        public string FinalTotalDue { get; set; }
        public string FinalTotalClaims { get; set; }
    }
}