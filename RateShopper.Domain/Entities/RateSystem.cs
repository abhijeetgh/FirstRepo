using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class RateSystem : BaseAuditableEntity
    {
        public RateSystem()
        {
            //this.RateCodes = new List<RateCode>();
        }

        //public long ID { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, MaxLength(50)]
        public string Code { get; set; }
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdateDateTime { get; set; }
        //public virtual ICollection<RateCode> RateCodes { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
