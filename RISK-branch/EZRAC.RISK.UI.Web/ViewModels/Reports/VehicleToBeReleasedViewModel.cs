using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class VehicleToBeReleasedViewModel
    {
        public string Claim { get; set; }
        public string ContractNo { get; set; }
        public string UnitDetails { get; set; }
        public string ClosedDate { get; set; }
        public string LossDate { get; set; }
        public string OpenDate { get; set; }
        public string LossType { get; set; }
        public string Status { get; set; }
        public string RiskAgent { get; set; }
        public string CompanyAbbr { get; set; }
    }
}