using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using EZRAC.RISK.Util.Common;
namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class DocumentGeneratorViewModel
    {
        public DocumentTypes DocumentTypeId { get; set; }
        public string Title { get; set; }
        public BillingViewModel Billings { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        [Required(ErrorMessage="Please select driver.")]
        public long SelectedDriverId { get; set; }
        public long[] SelectedBillings { get; set; }

        [Required(ErrorMessage="Please enter number of months.")]
        [Range(1,int.MaxValue,ErrorMessage="Invalid number of months.")]
        public Nullable<int> NumberOfMonths { get; set; }

        [Required(ErrorMessage = "Please enter bid amount.")]
        public Nullable<double> SalvageBidAmount { get; set; }

        public bool TreatAsCaliforniaTicket { get; set; }
        public long ClaimId { get; set; }

        public Nullable<double> TotalBill { get; set; }
        public Nullable<double> TotalPayment { get; set; }
        public Nullable<double> TotalDue { get; set; }
    }
}