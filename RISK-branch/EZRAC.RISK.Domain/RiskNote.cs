namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskNote : BaseEntity
    {
        public string Description { get; set; }
        public int NoteTypeId { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Int64 ClaimId { get; set; }
        public string NoteTypeDescription { get; set; }
        public bool IsPrivilege { get; set; }

        public Claim Claim { get; set; }
    }
}
