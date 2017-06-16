using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RuleSetDefaultSettingMap : BaseEntityConfiguration<RuleSetDefaultSetting>
    {
        public RuleSetDefaultSettingMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetDefaultSettings");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RangeIntervalID).HasColumnName("RangeIntervalID");
            this.Property(t => t.MinAmount).HasColumnName("MinAmount");
            this.Property(t => t.MaxAmount).HasColumnName("MaxAmount");
            this.Property(t => t.GapAmount).HasColumnName("GapAmount");
            this.Property(t => t.BaseMinAmount).HasColumnName("BaseMinAmount");
            this.Property(t => t.BaseMaxAmount).HasColumnName("BaseMaxAmount");
            this.Property(t => t.BaseGapAmount).HasColumnName("BaseGapAmount");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.RangeInterval)
                .WithMany(t => t.RuleSetDefaultSettings)
                .HasForeignKey(d => d.RangeIntervalID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.RuleSetDefaultSettings)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.RuleSetDefaultSettings1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
