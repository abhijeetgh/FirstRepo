using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public partial class QuickViewGapDevSettingsMap : BaseEntityConfiguration<QuickViewGapDevSettings>
    {
        public QuickViewGapDevSettingsMap()
        {
            this.ToTable("QuickViewGapDevSettings");

            this.Property(t => t.CompetitorGroupId).HasColumnName("CompetitorGroupId");
            this.Property(t => t.CarClassGroupId).HasColumnName("CarClassGroupId");
            //this.Property(t => t.GapValue).HasColumnName("GapValue");
            this.Property(t => t.DeviationValue).HasColumnName("DeviationValue");

            this.HasRequired(t => t.QuickViewGroup)
                .WithMany(t => t.QuickViewGapDevSettings)
                .HasForeignKey(d => d.CompetitorGroupId);
        }
    }
}
