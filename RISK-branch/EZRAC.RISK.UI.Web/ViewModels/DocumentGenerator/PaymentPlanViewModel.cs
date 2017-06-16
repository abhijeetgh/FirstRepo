using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class PaymentPlanViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public int NumberOfMonths { get; set; }
        public IEnumerable<PaymentsDuePlan> Payments { get; set; }
    }

    public class PaymentsDuePlan
    {
        public string DateOfPayment { get; set; }
        public double Amount { get; set; }
    }
}