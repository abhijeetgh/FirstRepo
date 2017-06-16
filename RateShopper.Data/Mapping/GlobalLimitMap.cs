using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class GlobalLimitMap : BaseEntityConfiguration<GlobalLimit>
    {
        public GlobalLimitMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("GlobalLimits");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.GlobalLimits)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.GlobalLimits)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.GlobalLimits1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
