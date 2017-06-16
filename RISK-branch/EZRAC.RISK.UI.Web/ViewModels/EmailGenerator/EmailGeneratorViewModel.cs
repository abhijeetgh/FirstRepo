using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.EmailGenerator
{
    public class EmailGeneratorViewModel
    {
        public IEnumerable<SelectListItem> Recipients { get; set; }


        [Required(ErrorMessage = "Please select status")]
        public int SelectedRecipient { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string CustomEmailAddress { get; set; }

        public IEnumerable<SelectListItem> RiskUserEmails { get; set; }

        public IEnumerable<string> SelectedUserEmails { get; set; }

        public IEnumerable<InformationToShowViewModel> InformationToSendList { get; set; }

        public ClaimsConstant.EmailInfoToSend[] SelectedInfoToSend { get; set; }

        public IEnumerable<NotesViewModel> Notes { get; set; }

        public int[] SelectedNotesToSend { get; set; }

        public IEnumerable<RiskFileModel> Files { get; set; }

        public int[] SelectedFilesToSend { get; set; }

        public long ClaimId { get; set; }

        public string Remarks { get; set; }

    }
}