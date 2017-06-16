using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class FTBTargetMap : BaseEntityConfiguration<FTBTarget>
    {
        public FTBTargetMap()
        {
            this.ToTable("FTBTargets");
            this.Property(t => t.LocationBrandId).HasColumnName("LocationBrandId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.DayOfWeekId).HasColumnName("DayOfWeekId");
            this.Property(t => t.Target).HasColumnName("Target");

            this.HasRequired(t => t.LocationBrands)
               .WithMany(t => t.FTBTarget)
               .HasForeignKey(d => d.LocationBrandId);

            this.HasRequired(t => t.WeekDays)
               .WithMany(t => t.FTBTarget)
               .HasForeignKey(d => d.DayOfWeekId);
            this.HasRequired(t => t.User)
               .WithMany(t => t.FTBTarget)
               .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
               .WithMany(t => t.FTBTarget1)
               .HasForeignKey(d => d.UpdatedBy);
        }
    }
}
