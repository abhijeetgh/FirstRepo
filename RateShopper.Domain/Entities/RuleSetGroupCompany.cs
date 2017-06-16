using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class RuleSetGroupCompany:BaseEntity
    {
        //public long ID { get; set; }
        public long RuleSetGroupID { get; set; }
        public long CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual RuleSetGroup RuleSetGroup { get; set; }
    }
}
