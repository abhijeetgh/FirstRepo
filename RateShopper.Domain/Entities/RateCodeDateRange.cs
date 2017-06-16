using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class RateCodeDateRange : BaseEntity
    {
        [Required]
        public long RateCodeID { get; set; }
        [Required]
        public System.DateTime StartDate { get; set; }
        [Required]
        public System.DateTime EndDate { get; set; }

        public virtual RateCode RateCode { get; set; }
    }
}
