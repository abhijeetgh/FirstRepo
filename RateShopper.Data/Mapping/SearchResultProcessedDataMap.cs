using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class SearchResultProcessedDataMap : BaseEntityConfiguration<SearchResultProcessedData>
    {
        public SearchResultProcessedDataMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);
            // Properties
           

            // Table & Column Mappings
            this.ToTable("SearchResultProcessedData");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.ScrapperSourceID).HasColumnName("ScrapperSourceID");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.RentalLengthID).HasColumnName("RentalLengthID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.DateFilter).HasColumnName("DateFilter");
            this.Property(t => t.DailyViewJSONResult).HasColumnName("DailyViewJSONResult");
            this.Property(t => t.ClassicViewJSONResult).HasColumnName("ClassicViewJSONResult");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");

            // Relationships
            this.HasRequired(t => t.SearchSummary)
                .WithMany(t => t.SearchResultProcessedDatas)
                .HasForeignKey(d => d.SearchSummaryID);

        }
    }
}
