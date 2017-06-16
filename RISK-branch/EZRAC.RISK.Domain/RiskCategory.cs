using System;
using System.Collections.Generic;

namespace EZRAC.RISK.Domain
{
    public class RiskCategory : BaseEntity
    {
        public string CategoryName { get; set; }

        public Permission Permission { get; set; }
    }
}
