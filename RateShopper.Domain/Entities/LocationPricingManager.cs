using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class LocationPricingManager :BaseEntity
    {
        public long UserID { get; set; }
        public long LocationBrandID { get; set; }

        //public virtual LocationBrand LocationBrands { get; set; }
        //public virtual User User { get; set; }
    }
}
