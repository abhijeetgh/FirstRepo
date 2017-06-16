using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSet : BaseAuditableEntity
    {
        public RuleSet()
        {
            this.RuleSetCarClasses = new List<RuleSetCarClasses>();
  
            this.RuleSetGroups = new List<RuleSetGroup>();
            this.RuleSetRentalLengths = new List<RuleSetRentalLength>();
            this.RuleSetWeekDays = new List<RuleSetWeekDay>();
            this.RuleSetsApplieds = new List<RuleSetsApplied>();
        }

        //public long ID { get; set; }
        public long LocationBrandID { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public int CompanyPositionAbvAvg { get; set; }
        public bool IsPositionOffset { get; set; }
        public bool IsWideGapTemplate { get; set; }
        public bool IsGov { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(500)]
        public string SelectedCompanyIDs { get; set; }
        public bool IsCopiedAutomationRuleSet { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual ICollection<RuleSetCarClasses> RuleSetCarClasses { get; set; }
      
        public virtual ICollection<RuleSetGroup> RuleSetGroups { get; set; }
        public virtual ICollection<RuleSetRentalLength> RuleSetRentalLengths { get; set; }
        public virtual ICollection<RuleSetWeekDay> RuleSetWeekDays { get; set; }
        public virtual ICollection<RuleSetsApplied> RuleSetsApplieds { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
