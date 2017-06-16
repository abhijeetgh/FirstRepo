using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class FTBTagetsDetailMap : BaseEntityConfiguration<FTBTargetsDetail>
    {
        public FTBTagetsDetailMap()
        {
            this.ToTable("FTBTargetsDetails");
            this.Property(t => t.TargetId).HasColumnName("TargetId");
            this.Property(t => t.PercentTarget).HasColumnName("PercentTarget");
            this.Property(t => t.PercentRateIncrease).HasColumnName("PercentRateIncrease");
            this.Property(t => t.SlotOrder).HasColumnName("SlotOrder");

            this.HasRequired(t => t.FTBTargets)
              .WithMany(t => t.FTBTargetsDetail)
              .HasForeignKey(d => d.TargetId);

        }
    }
}
