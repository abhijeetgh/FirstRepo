
namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class RiskFileTypes//:BaseEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public IList<RiskFile> RiskFiles { get; set; }
    }
}
