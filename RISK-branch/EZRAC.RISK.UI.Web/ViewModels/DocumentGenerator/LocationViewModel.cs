using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class LocationViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CompanyAbbr { get; set; }
        public string State { get; set; }
    }
}