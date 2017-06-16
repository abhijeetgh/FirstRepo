namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskFileMapping :BaseEntity
    {
        public Claim Claim { get; set; }
        public RiskFile File { get; set; }
        public RiskFileTypes Risk_FileTypes { get; set; }
    }
}
