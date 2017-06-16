using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class ClaimVehiclePaymentViewModel
    {
        public ClaimViewModel ClaimViewModel { get; set; }
        public VehicleViewModel VehicleViewModel { get; set; }
        public PaymentViewModel PaymentViewModel { get; set; }
    }
}