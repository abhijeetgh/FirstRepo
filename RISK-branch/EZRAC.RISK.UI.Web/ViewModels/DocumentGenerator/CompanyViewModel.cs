using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        public string Abbr { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Zurich { get; set; }
        public long CurrentUserId { get; set; }
        public string Logopath { get; set; }
        public string AdLogoPath { get; set; }
        public string EzLogoPath { get; set; }

    }

}