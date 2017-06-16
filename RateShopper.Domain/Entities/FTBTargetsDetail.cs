using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class FTBTargetsDetail : BaseEntity
    {
        public long TargetId { get; set; }
        public decimal? PercentTarget { get; set; }
        public decimal? PercentRateIncrease { get; set; }
        public int SlotOrder { get; set; }

        public virtual FTBTarget FTBTargets { get; set; }
    }
}
