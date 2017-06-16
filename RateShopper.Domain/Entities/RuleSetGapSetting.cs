using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RuleSetGapSetting : BaseEntity
    {
        //public long ID { get; set; }
        public long RuleSetGroupID { get; set; }
        public long RangeIntervalID { get; set; }
        [Required]
        public decimal MinAmount { get; set; }
        [Required]
        public decimal MaxAmount { get; set; }
        [Required]
        public decimal GapAmount { get; set; }
        [Required]
        public decimal BaseMinAmount { get; set; }
        [Required]
        public decimal BaseMaxAmount { get; set; }
        [Required]
        public decimal BaseGapAmount { get; set; }
        public virtual RangeInterval RangeInterval { get; set; }
        public virtual RuleSetGroup RuleSetGroup { get; set; }
    }
}
