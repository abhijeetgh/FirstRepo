namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;

    public class RiskClaimApproval : BaseEntity
    {
        public Int64 ClaimId { get; set; }
        public Int64 ApproverId { get; set; }
        public Int64 RequestedUserId { get; set; }
        public int ClaimStatusId { get; set; }
        public Nullable<bool> ApprovalStatus { get; set; }
    

        public Claim Claim { get; set; }

        public User RequestedUser { get; set; }

        public User Approver { get; set; }

       
        public RiskClaimStatus ClaimStatus { get; set; }
    }
}
