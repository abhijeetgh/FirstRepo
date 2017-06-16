using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ContractInfoViewModel
    {

        public Int64 Id { get; set; }

         //[Required(ErrorMessage = "Please enter contract number")]
        [Display(Name = "Contract Number")]
        public string ContractNumber { get; set; }

        [Display(Name = "Pickup Date")]
        public Nullable<DateTime> PickupDate { get; set; }

        [Display(Name = "Return Date")]
        public Nullable<DateTime> ReturnDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Days Out")]
        public Nullable<int> DaysOut { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Miles Driven")]
        public Nullable<int> Miles { get; set; }

        [Display(Name = "Rates")]
        public string Rates { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Daily Rate")]
        public Nullable<double> DailyRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Weekly Rate")]
        public Nullable<double> WeeklyRate { get; set; }

        [Display(Name = "CDW")]
        public bool CDW { get; set; }

        [Display(Name = "CDW Voided ?")]
        public bool CDWVoided { get; set; }

        [Display(Name = "LDW Voided ?")]
        public bool LDWVoided { get; set; }

        [Display(Name = "LDW")]
        public bool LDW { get; set; }

        [Display(Name = "SLI")]
        public bool SLI { get; set; }

        [Display(Name = "LPC")]
        public bool LPC { get; set; }

        [Display(Name = "LPC-2")]
        public bool LPC2 { get; set; }

        [Display(Name = "GARS")]
        public bool GARS { get; set; }

        [Range(1000,9999, ErrorMessage = "Please enter last four digits of credit card number")]
        [Display(Name = "Renters Credit Card Number")]
        public Nullable<int> CardNumber { get; set; }

        [Display(Name = "Renters Credit Card Type")]
        public string CardType { get; set; }

        [Display(Name = "Renters Credit Card Expiration Date")]
        public string CardExpDate { get; set; }

        public Int64 ClaimId { get; set; }

        public Nullable<double> TotalDue { get; set; }
        
    }
}