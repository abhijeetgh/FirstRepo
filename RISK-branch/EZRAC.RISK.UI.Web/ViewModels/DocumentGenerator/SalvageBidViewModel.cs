using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class SalvageBidViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public double SalvageAmount { get; set; }
    }
}