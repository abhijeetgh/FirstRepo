using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class SearchClaimsCriteria
    {
        public long UserId { get; set; }

        public int PageSize {get; set;}

        public int PageCount{ get; set; }

        public string SortType { get; set; }       

        public bool SortOrder { get; set; }

        public ClaimType ClaimType { get; set; }

        //long userId, int pagsize, int pageCount, string sortType, bool sortOrder
    }

    public enum ClaimType
    {
        AllClaim,
        FollowupClaim,
        PendingApproval,
        Approved,
        AdvancedSearch
    }
}
