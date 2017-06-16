using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class CreateClaimViewModel
    {
       
        [Display(Name = "Location")]
        public IEnumerable<SelectListItem> Locations { get; set; }

        [Required(ErrorMessage = "Please select location")]
        public Int64 SelectedLocation { get; set; }
       

        [Display(Name = "Loss Type")]
        public IEnumerable<SelectListItem> LossTypes { get; set; }

        [Required(ErrorMessage = "Please select loss type")]
        public int SelectedLossType { get; set; }


        [Required(ErrorMessage = "Please select date of loss")]
        [Display(Name = "Date of Loss")]
        public DateTime DateOfLoss { get; set; }

       //[Required(ErrorMessage = "Please select Contract")]
        [ContractNumberValidator(ErrorMessage = "Please provide valid contract number")]
        [Display(Name = "Contract no")]
        public string ContractNo { get; set; }

        
        [Display(Name = "Assigned To")]
        public IEnumerable<SelectListItem> Users { get; set; }

        [Required(ErrorMessage = "Please select user to assign")]
        public Int32 SelectedAssignedUser { get; set; }

        public string SelectedUnitNumber { get; set; }

    }

}