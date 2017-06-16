using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class CommonPaymentViewModel
    {
        public CommonViewModel CommonViewModel { get; set; }
        public PaymentViewModel PaymentViewModel { get; set; }
    }
}