using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class ViewClaimsModel
    {
        public IEnumerable<ClaimInfoViewModel> FollowUpClaims { get; set; }
        public IEnumerable<ClaimInfoViewModel> ClaimsForApproval { get; set; }
    }
}