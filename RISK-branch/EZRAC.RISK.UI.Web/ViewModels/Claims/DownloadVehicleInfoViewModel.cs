using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class DownloadVehicleInfoViewModel
    {
        public Int64 ClaimID { get; set; }

        public string ContractNumber { get; set; }

        [Display(Name = "Unit Number")]
        [UnitNumberValidator(ErrorMessage = "Please provide valid unit number")]
        public string UnitNumber { get; set; }

        public Int64 VehicleId { get; set; }
    }
}