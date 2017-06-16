using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class CommonClaimViewModel
    {
        public CommonViewModel CommonViewModel { get; set; }
        public ClaimViewModel ClaimViewModel { get; set; }
    }
}