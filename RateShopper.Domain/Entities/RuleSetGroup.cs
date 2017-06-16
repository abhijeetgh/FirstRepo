using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSetGroup:BaseEntity
    {
        public RuleSetGroup()
        {
            this.RuleSetGroupCompanies = new List<RuleSetGroupCompany>();
            this.RuleSetGapSettings = new List<RuleSetGapSetting>();
        }

        //public long ID { get; set; }
        public long RuleSetID { get; set; }

        [Required, MaxLength(200)]
        public string GroupName { get; set; }

        public virtual RuleSet RuleSet { get; set; }
        public virtual ICollection<RuleSetGroupCompany> RuleSetGroupCompanies { get; set; }
        public virtual ICollection<RuleSetGapSetting> RuleSetGapSettings { get; set; }
    }
}
