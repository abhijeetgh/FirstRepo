using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class RuleSetGapSettingMap : EntityTypeConfiguration<RuleSetGapSetting>
    {
        public RuleSetGapSettingMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetGapSettings");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetGroupID).HasColumnName("RuleSetGroupID");
            this.Property(t => t.RangeIntervalID).HasColumnName("RangeIntervalID");
            this.Property(t => t.MinAmount).HasColumnName("MinAmount");
            this.Property(t => t.MaxAmount).HasColumnName("MaxAmount");
            this.Property(t => t.GapAmount).HasColumnName("GapAmount");
            this.Property(t => t.BaseMinAmount).HasColumnName("BaseMinAmount");
            this.Property(t => t.BaseMaxAmount).HasColumnName("BaseMaxAmount");
            this.Property(t => t.BaseGapAmount).HasColumnName("BaseGapAmount");

            // Relationships
            this.HasRequired(t => t.RangeInterval)
                .WithMany(t => t.RuleSetGapSettings)
                .HasForeignKey(d => d.RangeIntervalID);
            this.HasRequired(t => t.RuleSetGroup)
                .WithMany(t => t.RuleSetGapSettings)
                .HasForeignKey(d => d.RuleSetGroupID);

        }
    }
}
