using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class LocationBrandRentalLength : BaseAuditableEntity
    {
        //public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public long RentalLengthID { get; set; }
        [Required]
        public decimal RateValue { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual RentalLength RentalLength { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
