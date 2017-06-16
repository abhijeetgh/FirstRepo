using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class NationalCasualtyInsuranceCoverViewModel
    {
        public DocumentTemplateViewModel HeaderViewModel { get; set; }
        public IEnumerable<DriverViewModel> Drivers { get; set; }
        public string PolicyNumber { get; set; }

    }
}