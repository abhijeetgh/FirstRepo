using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class RateCode : BaseAuditableEntity
    {
        public string Description { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public bool IsDeleted { get; set; }
        public string SupportedBrandIDs { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RateCodeDateRange> RateCodeDateRange { get; set; }
    }
}
