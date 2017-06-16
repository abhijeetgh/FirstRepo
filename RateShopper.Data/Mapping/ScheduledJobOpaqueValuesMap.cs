using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class ScheduledJobOpaqueValuesMap : BaseEntityConfiguration<ScheduledJobOpaqueValues>
    {
        public ScheduledJobOpaqueValuesMap()
        {
            this.ToTable("ScheduledJobOpaqueValues");
            this.Property(t => t.CarClassId).HasColumnName("CarClassId");
            this.Property(t => t.ScheduledJobId).HasColumnName("ScheduledJobId");
            this.Property(t => t.PercentValue).HasColumnName("Percentvalue");

            // Relationships
            this.HasRequired(t => t.ScheduledJob)
                .WithMany(t => t.ScheduledJobOpaqueValues)
                .HasForeignKey(d => d.ScheduledJobId);
        }
    }
}
