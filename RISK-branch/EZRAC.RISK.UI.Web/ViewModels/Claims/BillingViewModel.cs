using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class BillingViewModel
    {
        
        public IEnumerable<SelectListItem> BillingTypes { get; set; }

        [Required(ErrorMessage = "Please select charges")]
        [Display(Name = "Type Of Charge")]
        public int SelectedBillingTypeId { get; set; }

       
        [Required(ErrorMessage = "Please enter amount")]
        [Display(Name = "Amount")]
        public Nullable<double> Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Discount")]
        public Nullable<double> Discount { get; set; }

        [Display(Name = "If entering estimate amount, automatically create / overwrite admin fee.")]
        public bool AutoAdminFeeCalculate { get; set; }

         [Display(Name = "Total Billing")]
        public double TotalBilling { get; set; }

        [Display(Name = "Total Due")]
        public double TotalDue { get; set; }

        public Int64 ClaimId { get; set; }

        public Nullable<double> AdminFee { get; set; }

        public IEnumerable<RiskBillingsViewModel> Billings { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Total Labor Hours")]
        public Nullable<double> LabourHour { get; set; }
    }
}