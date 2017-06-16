using EZRAC.Risk.UI.Web.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class NotesViewModel
    {
        [Required(ErrorMessage="Please Select Note Type")]
        public IEnumerable<SelectListItem> NotesType { get; set; }

        public Int64 ClaimId { get; set; }

        [Required(ErrorMessage = "Please Enter Note Description")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = Constants.CrmsDateFormates.MMDDYYYYForAttr)]
        public DateTime Date { get; set; }

        public string UpdatedBy { get; set; }

        [Required(ErrorMessage = "Please Select Note Type")]
        public int SelectedNoteTypeId { get; set; }

        public string SelectedNoteType { get; set; }
        
        public bool SendNotification { get; set; }

        public bool IsPrivilege { get; set; }

        public bool IsNotesTab { get; set; }

        public long NoteId { get; set; }

        public bool NoteAssignedToLoggedInUser { get; set; }

        public IEnumerable<NotesViewModel> Notes { get; set; }

    }
}