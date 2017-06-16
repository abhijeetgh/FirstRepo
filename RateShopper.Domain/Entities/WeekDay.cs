using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class WeekDay:BaseEntity
    {
        public WeekDay()
        {
            this.RuleSetWeekDay = new List<RuleSetWeekDay>();
            this.FTBTarget = new List<FTBTarget>();
        }

        //public long ID { get; set; }
        [Required,MaxLength(100)]
        public string Day { get; set; }
        public virtual ICollection<RuleSetWeekDay> RuleSetWeekDay { get; set; }
        public virtual ICollection<FTBTarget> FTBTarget { get; set; }
    }
}
