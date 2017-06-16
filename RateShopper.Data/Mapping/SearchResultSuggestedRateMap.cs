using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class SearchResultSuggestedRateMap : BaseEntityConfiguration<SearchResultSuggestedRate>
    {
        public SearchResultSuggestedRateMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("SearchResultSuggestedRates");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.BrandID).HasColumnName("BrandID");
            this.Property(t => t.RentalLengthID).HasColumnName("RentalLengthID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.BaseRate).HasColumnName("BaseRate");
            this.Property(t => t.MinBaseRate).HasColumnName("MinBaseRate");
            this.Property(t => t.MaxBaseRate).HasColumnName("MaxBaseRate");
            this.Property(t => t.TotalRate).HasColumnName("TotalRate");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.RuleSetName).HasColumnName("RuleSetName");

            this.Property(t => t.CompanyBaseRate).HasColumnName("CompanyBaseRate");
            this.Property(t => t.CompanyTotalRate).HasColumnName("CompanyTotalRate");
            
			this.Property(t => t.TSDUpdateDateTime).HasColumnName("TSDUpdateDateTime");
			this.Property(t => t.TSDUpdatedBy).HasColumnName("TSDUpdatedBy");
            this.Property(t => t.NoCompitetorRateFound).HasColumnName("NoCompitetorRateFound");
            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.SearchResultSuggestedRates)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.Company)
                .WithMany(t => t.SearchResultSuggestedRates)
                .HasForeignKey(d => d.BrandID);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.SearchResultSuggestedRates)
                .HasForeignKey(d => d.LocationID);
            this.HasRequired(t => t.RentalLength)
                .WithMany(t => t.SearchResultSuggestedRates)
                .HasForeignKey(d => d.RentalLengthID);
            this.HasRequired(t => t.SearchSummary)
                .WithMany(t => t.SearchResultSuggestedRates)
                .HasForeignKey(d => d.SearchSummaryID);

        }
    }
}
