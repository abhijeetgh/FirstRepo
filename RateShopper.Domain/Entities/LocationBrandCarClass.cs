using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class LocationBrandCarClass : BaseEntity
    {
        //public long ID { get; set; }
        public long CarClassID { get; set; }
        public long LocationBrandID { get; set; }
        public virtual CarClass CarClass { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
    }
}
