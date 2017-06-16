using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class ScheduledJobTetherings : BaseEntity
    {
        [Required]
        public long ScheduleJobID { get; set; }
        [Required]
        public long LocationBrandID { get; set; }
        [Required]
        public long DominentBrandID { get; set; }
        [Required]
        public long DependantBrandID { get; set; }
        [Required]
        public long CarClassID { get; set; }
        public decimal? TetherValue { get; set; }
        public bool IsTetherValueinPercentage { get; set; }

        public virtual CarClass CarClass { get; set; }
        public virtual ScheduledJob ScheduledJob { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Company1 { get; set; }
    }
}
