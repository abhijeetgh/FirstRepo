using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class TicketAffidavitViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public ContractViewModel Contract { get; set; }
    }
}