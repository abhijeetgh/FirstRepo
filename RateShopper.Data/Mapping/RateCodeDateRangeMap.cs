using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class RateCodeDateRangeMap : BaseEntityConfiguration<RateCodeDateRange>
    {
        public RateCodeDateRangeMap()
        {
            //Table and Column mapping
            this.ToTable("RateCodeDateRange");
            this.Property(t => t.RateCodeID).HasColumnName("RateCodeID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");

            // Relationships
            this.HasRequired(t => t.RateCode)
                .WithMany(t => t.RateCodeDateRange)
                .HasForeignKey(d => d.RateCodeID);
        }
    }
}
