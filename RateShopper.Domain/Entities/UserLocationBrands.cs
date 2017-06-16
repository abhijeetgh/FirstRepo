using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class UserLocationBrands : BaseEntity
    {
        //public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public long UserID { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual User User { get; set; }
    }
}
