using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class SearchResultMap : BaseEntityConfiguration<SearchResult>
    {
        public SearchResultMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("SearchResults");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.ScrapperSourceID).HasColumnName("ScrapperSourceID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.RentalLengthID).HasColumnName("RentalLengthID");
            //this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.BaseRate).HasColumnName("BaseRate");
            this.Property(t => t.TotalRate).HasColumnName("TotalRate");
            this.Property(t => t.ArrivalDate).HasColumnName("ArrivalDate");
            this.Property(t => t.ReturnDate).HasColumnName("ReturnDate");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.Company)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.LocationID);
            this.HasRequired(t => t.RentalLength)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.RentalLengthID);
            this.HasRequired(t => t.ScrapperSource)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.ScrapperSourceID);
            this.HasRequired(t => t.SearchSummary)
                .WithMany(t => t.SearchResults)
                .HasForeignKey(d => d.SearchSummaryID);

        }
    }
}
