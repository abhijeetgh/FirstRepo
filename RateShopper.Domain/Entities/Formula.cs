using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class Formula:BaseAuditableEntity
    {
        //public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public Nullable<decimal> SalesTax { get; set; }
        public Nullable<decimal> AirportFee { get; set; }
        public Nullable<decimal> Arena { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public Nullable<decimal> VLRF { get; set; }
        public Nullable<decimal> CFC { get; set; }        
        public string TotalCostToBase { get; set; }
        public string BaseToTotalCost { get; set; }
        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
