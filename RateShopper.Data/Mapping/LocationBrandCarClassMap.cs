using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class LocationBrandCarClassMap : BaseEntityConfiguration<LocationBrandCarClass>
    {
        public LocationBrandCarClassMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("LocationBrandCarClass");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");

            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.LocationBrandCarClass)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.LocationBrandCarClass)
                .HasForeignKey(d => d.LocationBrandID);

        }
    }
}
