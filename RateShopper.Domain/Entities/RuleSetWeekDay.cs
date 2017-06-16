using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSetWeekDay:BaseEntity
    {
        //public long ID { get; set; }
        public long RuleSetID { get; set; }
        public long WeekDayID { get; set; }
        public virtual RuleSet RuleSet { get; set; }
        public virtual WeekDay WeekDay { get; set; }
    }
}
