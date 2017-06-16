using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RateShopper.Domain.Entities
{
    public partial class SearchJobUpdateAll : BaseEntity
    {
        [Required]
        public long SearchSummaryId { get; set; }
        [Required]
        public long CarClassId { get; set; }
        public decimal? BaseEdit { get; set; }
        public decimal? BaseEditTwo { get; set; }
        [Required]
        public bool IsDaily { get; set; }
    }
}
