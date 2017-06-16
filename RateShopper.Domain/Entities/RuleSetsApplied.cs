using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RuleSetsApplied:BaseAuditableEntity
    {
        //public long ID { get; set; }
        public long RuleSetID { get; set; }
        [Required]
        public System.DateTime StartDate { get; set; }
        [Required]
        public System.DateTime EndDate { get; set; }
        //public long CreatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public virtual RuleSet RuleSet { get; set; }
    }
}
