using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.ClaimBasicInfo
{
    public class ClaimInfoForViewEditViewModel
    {
        public Int64 ClaimID { get; set; }

        [Display(Name = "Open Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public DateTime OpenDate { get; set; }

        [Display(Name = "Close Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> CloseDate { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Open Location")]
        public Nullable<Int64> SelectedOpenLocationId { get; set; }
        public IEnumerable<SelectListItem> OpenLocations { get; set; }

        [Display(Name = "Close Location")]
        public Nullable<Int64> SelectedCloseLocationId { get; set; }
        public IEnumerable<SelectListItem> CloseLocations { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be minimum of 0")]
        [Display(Name = "Total Labor Hours")]
        public Nullable<double> LabourHour { get; set; }

        [Display(Name = "Loss Type")]
        public string LossTypeDesc { get; set; }

        [Required(ErrorMessage = "Please select Loss Type")]
        public int SelectedLossTypeId { get; set; }
        public IEnumerable<SelectListItem> LossTypes { get; set; }

        [Display(Name = "Collectable")]
        public bool IsCollectable { get; set; }

        [Display(Name = "Has Contract?")]
        public bool HasContract { get; set; }

        public bool DisableHasContract { get; set; }

        [Display(Name = "Estimated Return Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> EstReturnDate { get; set; }
    }
}