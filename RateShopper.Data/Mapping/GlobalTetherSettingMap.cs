using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class GlobalTetherSettingMap : BaseEntityConfiguration<GlobalTetherSetting>
    {
        public GlobalTetherSettingMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("GlobalTetherSettings");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.DominentBrandID).HasColumnName("DominentBrandID");
            this.Property(t => t.DependantBrandID).HasColumnName("DependantBrandID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.TetherValue).HasColumnName("TetherValue");
            this.Property(t => t.IsTeatherValueinPercentage).HasColumnName("IsTeatherValueinPercentage");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.GlobalTetherSettings)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.Company)
                .WithMany(t => t.GlobalTetherSettings)
                .HasForeignKey(d => d.DominentBrandID);
            this.HasRequired(t => t.Company1)
                .WithMany(t => t.GlobalTetherSettings1)
                .HasForeignKey(d => d.DependantBrandID);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.GlobalTetherSettings)
                .HasForeignKey(d => d.LocationID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.GlobalTetherSettings)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.GlobalTetherSettings1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
