using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.Risk.UI.Web.ViewModels.Claims;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class PaymentDueViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public IEnumerable<PaymentInfoViewModel> Payments { get; set; }
        public double OriginalDue { get; set; }
    }
}