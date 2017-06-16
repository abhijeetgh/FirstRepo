using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class LocationPricingManagerMap : BaseEntityConfiguration<LocationPricingManager>
    {
        public LocationPricingManagerMap()
        {
            this.ToTable("LocationPricingManagers");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
        }
    }
}
