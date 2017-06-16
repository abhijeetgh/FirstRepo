using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class LocationCompanyMap : BaseEntityConfiguration<LocationCompany>
    {
        public LocationCompanyMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("LocationCompany");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.IsTerminalInside).HasColumnName("IsTerminalInside");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.LocationCompany)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.LocationCompany)
                .HasForeignKey(d => d.LocationID);

        }
    }
}
