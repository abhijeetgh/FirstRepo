using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class RiskFileUploadModel
    {        
        public IEnumerable<SelectListItem> FileTypes { get; set; }

        [Required(ErrorMessage = "Please select type")]
        public int SelectedFileTypeId { get; set; }

        public long ClaimId { get; set; }
        
    }

      public enum RiskFileType
      {
          Pictures = 1,
          Legal = 2,
          ClaimDocuments = 3,
          SalvageDocument = 4,
          ImpoundDocuments = 5,
          PoliceDocument = 6
      }
}