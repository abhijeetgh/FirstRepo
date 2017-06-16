using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class GlobalTetherSetting:BaseAuditableEntity
    {
        //public long ID { get; set; }
        public long LocationID { get; set; }
        public long DominentBrandID { get; set; }
        public long DependantBrandID { get; set; }
        public long CarClassID { get; set; }
        [Required]
        public decimal TetherValue { get; set; }
        [Required]
        public bool IsTeatherValueinPercentage { get; set; }
        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual CarClass CarClass { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Company1 { get; set; }
        public virtual Location Location { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
