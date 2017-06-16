using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class DriverViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public int DriverType { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public string LicenceNumber { get; set; }
        public string LicenceState { get; set; }
        public InsuranceViewModel InsuranceViewModel { get; set; }
    }

    public class InsuranceViewModel
    {
        public long InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceClaimNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }        
        public string Zip { get; set; }
        public string PolicyNumber { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
    }
}