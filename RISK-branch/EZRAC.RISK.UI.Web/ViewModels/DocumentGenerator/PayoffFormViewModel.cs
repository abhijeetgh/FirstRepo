using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.Risk.UI.Web.ViewModels.Claims;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class PayoffFormViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public double EstimatedAmount { get; set; }
        public DateTime OpenDate { get; set; }
        public ContractViewModel Contract { get; set; }
        public Nullable<double> ActualCashValue { get; set; }
    }
}