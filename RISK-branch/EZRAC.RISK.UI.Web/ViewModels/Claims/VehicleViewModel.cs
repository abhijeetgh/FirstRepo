using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class VehicleInfoViewModel
    {
        public Int64 Id { get; set; }

        [Required(ErrorMessage = "Please enter unit number")]
        [Display(Name = "Unit Number")]
        public string UnitNumber { get; set; }

        [Display(Name = "Tag Number")]
        public string TagNumber { get; set; }

        [Display(Name = "Tag Number - Expiration")]
        public string TagNumberAndExp { get; set; }

         [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        [Display(Name="Tag Expires")]
        public Nullable<DateTime> TagExpires { get; set; }

        [Display(Name = "VIN")]
        public string VIN { get; set; }

        [Display(Name = "Vehicle")]
        public string VehicleShortDesc { get; set; }

         [Range(0, long.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Mileage")]
        public Nullable<long> Mileage { get; set; }

        public string Location { get; set; }

        [Display(Name = "Location/Status")]
        public string LocationNameAndStatus { get; set; }

    
        public string Status { get; set; }

        [Display(Name = "Swapped Vehicles")]
        public IEnumerable<string> SwappedVehicles { get; set; }

        public string Description { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Year must be a numeric value")]
        public string Year { get; set; }

        public string Color { get; set; }

        [Display(Name = "Purchase Type")]
        public Nullable<PurchaseType> PurchaseType { get; set; }

        public string ContractNumber { get; set; }

        public Int64 ClaimId { get; set; }

    }
}