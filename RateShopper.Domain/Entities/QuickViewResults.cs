using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class QuickViewResults : BaseEntity
    {
        [Required]
        public long SearchSummaryId { get; set; }
        [Required]
        public long QuickViewId { get; set; }
        [Required]
		public long ScrapperSourceId { get; set; }
		[Required]
		public long LocationBrandId { get; set; }
        [Required]
        public long RentalLengthId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public bool? IsMovedUp { get; set; }
        public bool? IsReviewed { get; set; }
        public bool? IsPositionChange { get; set; }
    }
}
