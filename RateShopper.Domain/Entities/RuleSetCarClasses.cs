using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSetCarClasses : BaseEntity
    {
        //public long ID { get; set; }
        public long RuleSetID { get; set; }
        public long CarClassID { get; set; }
        public virtual CarClass CarClass { get; set; }
        public virtual RuleSet RuleSet { get; set; }
    }
}
