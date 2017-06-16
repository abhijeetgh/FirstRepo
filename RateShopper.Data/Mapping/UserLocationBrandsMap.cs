using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class UserLocationBrandsMap : BaseEntityConfiguration<UserLocationBrands>
    {
        public UserLocationBrandsMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("UserLocationBrands");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.UserID).HasColumnName("UserID");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.UserLocationBrands)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserLocationBrands)
                .HasForeignKey(d => d.UserID);

        }
    }
}
