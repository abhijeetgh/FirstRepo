using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class RepoAuthorziationViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public string PolicyAgency { get; set; }
        public string CaseNumber { get; set; }
    }
}