using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RangeInterval : BaseEntity
    {
        public RangeInterval()
        {
            this.RuleSetDefaultSettings = new List<RuleSetDefaultSetting>();
            this.RuleSetGapSettings = new List<RuleSetGapSetting>();
        }

        //public long ID { get; set; }
        [Required,MaxLength(50)]
        public string Range { get; set; }
        public virtual ICollection<RuleSetDefaultSetting> RuleSetDefaultSettings { get; set; }
        public virtual ICollection<RuleSetGapSetting> RuleSetGapSettings { get; set; }
    }
}
