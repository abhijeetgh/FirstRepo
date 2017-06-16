using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class InsuranceCompanyMasteViewModel
    {
        public IEnumerable<InsuranceCompanyViewModel> InsuranceCompanyList { get; set; }
        public InsuranceCompanyViewModel InsuranceComapnyViewModel { get; set; }
    }
}