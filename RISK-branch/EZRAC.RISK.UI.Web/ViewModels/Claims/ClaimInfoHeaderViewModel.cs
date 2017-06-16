using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ClaimInfoHeaderViewModel
    {
        public Int64 ClaimID { get; set; }

        [Display(Name = "Status")]
        public IEnumerable<SelectListItem> Status { get; set; }

        [Required(ErrorMessage = "Please select status")]
        public int SelectedStatusId { get; set; }

        public string SelectedStatusName { get; set; }


        public IEnumerable<SelectListItem> Users { get; set; }

        public IEnumerable<SelectListItem> ApprovalUsers { get; set; }

        [Display(Name = "Assigned To")]
        [Required(ErrorMessage = "Please select AssignedTo")]
        public Int64 SelectedAssignedUserId { get; set; }
        public string SelectedAssignedUserName { get; set; }

        [Display(Name = "Approver")]
        public Nullable<Int64> SelectedApprover { get; set; }

        public string ContractNumber { get; set; }

       
        [Required(ErrorMessage = "Please select Followup Date")]
        [Display(Name = "Follow Up Date")]
        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public Nullable<DateTime> FollowupDate { get; set; }

        public string CompanyAbbr { get; set; }

        public Nullable<double> TotalDue { get; set; }

        public bool IsCollectable { get; set; }

        public bool HasAccessToApprove { get; set; }
    }
}