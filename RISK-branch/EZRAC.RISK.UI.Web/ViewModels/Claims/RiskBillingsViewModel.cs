using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class RiskBillingsViewModel
    {
        public Int64 Id { get; set; }
        public int BillingTypeId { get; set; }

        [Display(Name = "Type Of Charge")]
        public string BillingTypeDesc { get; set; }

        public Nullable<Int64> ClaimId { get; set; }

        [Display(Name = "Amount $")]
        public double Amount { get; set; }

        [Display(Name = "Discount %")]
        public double Discount { get; set; }

        [Display(Name = "Total")]
        public double SubTotal { get; set; }
    }
}
