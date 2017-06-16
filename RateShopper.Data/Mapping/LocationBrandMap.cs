using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class LocationBrandMap : BaseEntityConfiguration<LocationBrand>
    {
        public LocationBrandMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            //Moved to LocationBrand Entity
            //this.Property(t => t.TSDCustomerNumber)
            //    .IsRequired()
            //    .HasMaxLength(200);

            //this.Property(t => t.TSDPassCode)
            //    .IsRequired()
            //    .HasMaxLength(200);

            //this.Property(t => t.LocationBrandAlias)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("LocationBrand");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BrandID).HasColumnName("BrandID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.WeeklyExtraDenom).HasColumnName("WeeklyExtraDenom");
            this.Property(t => t.DailyExtraDayFactor).HasColumnName("DailyExtraDayFactor");
            this.Property(t => t.TSDCustomerNumber).HasColumnName("TSDCustomerNumber");
            this.Property(t => t.TSDPassCode).HasColumnName("TSDPassCode");
            this.Property(t => t.Description).HasColumnName("Description");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");
            this.Property(t => t.LocationBrandAlias).HasColumnName("LocationBrandAlias");
            this.Property(t => t.UseLORRateCode).HasColumnName("UseLORRateCode");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.BranchCode).HasColumnName("BranchCode");
            this.Property(t => t.CompetitorCompanyIDs).HasColumnName("CompetitorCompanyIDs");
            this.Property(t => t.QuickViewCompetitorCompanyIds).HasColumnName("QuickViewCompetitorCompanyIds");
            this.Property(t => t.IsFTBDominantBrand).HasColumnName("IsFTBDominantBrand");
            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.LocationBrands)
                .HasForeignKey(d => d.BrandID);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.LocationBrands)
                .HasForeignKey(d => d.LocationID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.LocationBrands)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.LocationBrands1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
