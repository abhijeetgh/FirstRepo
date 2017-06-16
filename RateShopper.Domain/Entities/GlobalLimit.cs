using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class GlobalLimit : BaseAuditableEntity
    {
        public GlobalLimit()
        {
            this.GlobalLimitDetails = new List<GlobalLimitDetail>();
        }

        //public long ID { get; set; }
        [Required]
        public long LocationBrandID { get; set; }
        [Required]
        public System.DateTime StartDate { get; set; }
        [Required]
        public System.DateTime EndDate { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual ICollection<GlobalLimitDetail> GlobalLimitDetails { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
