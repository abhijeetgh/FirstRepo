using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class QuickViewResultsMap : BaseEntityConfiguration<QuickViewResults>
    {
        public QuickViewResultsMap()
        {
            this.ToTable("QuickViewResults");
            this.Property(t => t.SearchSummaryId).HasColumnName("SearchSummaryId");
            this.Property(t => t.QuickViewId).HasColumnName("QuickViewId");
            this.Property(t => t.ScrapperSourceId).HasColumnName("ScrapperSourceID");
			this.Property(t => t.LocationBrandId).HasColumnName("LocationBrandID");			
            this.Property(t => t.RentalLengthId).HasColumnName("RentalLengthId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.IsMovedUp).HasColumnName("IsMovedUp");
            this.Property(t => t.IsReviewed).HasColumnName("IsReviewed");
            this.Property(t => t.IsPositionChange).HasColumnName("IsPositionChange");
        }
    }
}
