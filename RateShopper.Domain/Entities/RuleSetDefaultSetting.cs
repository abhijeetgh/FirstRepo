using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RuleSetDefaultSetting : BaseAuditableEntity
    {
        // public long ID { get; set; }
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
        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual RangeInterval RangeInterval { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
