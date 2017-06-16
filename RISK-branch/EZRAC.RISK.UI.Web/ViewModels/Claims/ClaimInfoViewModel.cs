using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ClaimInfoViewModel
    {

        public Int64 ClaimID { get; set; }

        [Display(Name = "Status")]
        public IEnumerable<SelectListItem> Status { get; set; }

        [Required(ErrorMessage = "Please select status")]
        public int SelectedStatusId { get; set; }

        public string SelectedStatusName { get; set; }

        
        public IEnumerable<SelectListItem> Users { get; set; }

        public IEnumerable<SelectListItem> ApprovalUsers { get; set; }

        [Display(Name = "AssignedTo")]
        [Required(ErrorMessage = "Please select AssignedTo")]
        public Int64 SelectedAssignedUserId { get; set; }
        public string SelectedAssignedUserName { get; set; }

        [Display(Name = "Approver")]
        public Nullable<Int64> SelectedApprover { get; set; }

        public string ContractNumber { get; set; }

        public string DriverName { get; set; }
        public string UnitNumber { get; set; }
        public string VehicleName { get; set; }

         [Required(ErrorMessage = "Please select Followup Date")]
         [Display(Name = "Follow Up Date")]
         [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> FollowupDate { get; set; }

        public Boolean Completed { get; set; }

        [Display(Name = "Open Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public DateTime OpenDate { get; set; }

        [Display(Name = "Close Date")]
        [DisplayFormat(DataFormatString =Constants.CrmsDateFormates.MMDDYYYYForAttr)]
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
        [Display(Name = "Total Labor")]
        public Nullable<double> LabourHour { get; set; }


        [Required(ErrorMessage = "Please select Loss Type")]
        [Display(Name = "Loss Type")]
        public string LossTypeDesc { get; set; }
        public int SelectedLossTypeId { get; set; }
        public IEnumerable<SelectListItem> LossTypes { get; set; }


        public string ApprovalStatus { get; set; }

        public PageHelper<ClaimInfoViewModel> ClaimInfo { get; set; }

        public PendingClaimPageHelper<ClaimInfoViewModel> PendingClaimInfo { get; set; }


        public string CompanyAbbr { get; set; }
    }
}