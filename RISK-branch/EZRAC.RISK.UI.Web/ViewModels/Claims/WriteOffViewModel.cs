using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class WriteOffViewModel
    {
        public IEnumerable<SelectListItem> WriteOffTypes { get; set; }

        [Required(ErrorMessage = "Please select WriteOff")]
        [Display(Name = "Type Of WriteOff")]
        public int SelectedWriteOffTypeId { get; set; }


        [Required(ErrorMessage = "Please enter amount")]
        [Display(Name = "Amount")]
        public Nullable<double> Amount { get; set; }

        [Display(Name = "Total WriteOff")]
        public Nullable<double> TotalWriteOff { get; set; }

        [Display(Name = "Total Due")]
        public Nullable<double> TotalDue { get; set; }

        public Int64 ClaimId { get; set; }

        public string WriteOffType { get; set; }

        public IEnumerable<WriteOffInfoViewModel> WriteOffs { get; set; }
    }
}