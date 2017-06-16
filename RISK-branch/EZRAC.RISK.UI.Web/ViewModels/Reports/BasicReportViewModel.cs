using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class BasicReportViewModel
    {        
        public string Claim { get; set; }
        public string Contract { get; set; }
        public string Customer { get; set; }
        public string Unit { get; set; }
        public string MakeModel { get; set; }
        public string Tag { get; set; }
        public string Location { get; set; }
        public double Charges { get; set; }
        public double Payment { get; set; }
        public double Balance { get; set; }
        public string Status { get; set; }
        public DateTime NextCallDate { get; set; }
        public string CompanyAbbr { get; set; }

    }
}