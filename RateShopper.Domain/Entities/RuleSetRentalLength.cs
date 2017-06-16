using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSetRentalLength : BaseEntity
    {
        //public long ID { get; set; }
        public long RuleSetID { get; set; }
        public long RentalLengthID { get; set; }
        public virtual RentalLength RentalLength { get; set; }
        public virtual RuleSet RuleSet { get; set; }
    }
}
