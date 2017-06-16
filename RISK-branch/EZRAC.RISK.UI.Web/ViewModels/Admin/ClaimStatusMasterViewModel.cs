using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class ClaimStatusMasterViewModel
    {
        public IEnumerable<ClaimStatusViewModel> ClaimStatusListViewModel { get; set; }
        public ClaimStatusViewModel ClaimStatusViewModel { get; set; }
    }
}