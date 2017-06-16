using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class LocationBrandRentalLengthMap : BaseEntityConfiguration<LocationBrandRentalLength>
    {
        public LocationBrandRentalLengthMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("LocationBrandRentalLength");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.RentalLengthID).HasColumnName("RentalLengthID");
            this.Property(t => t.RateValue).HasColumnName("RateValue");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.LocationBrandRentalLength)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.RentalLength)
                .WithMany(t => t.LocationBrandRentalLength)
                .HasForeignKey(d => d.RentalLengthID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.LocationBrandRentalLength)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.LocationBrandRentalLength1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
