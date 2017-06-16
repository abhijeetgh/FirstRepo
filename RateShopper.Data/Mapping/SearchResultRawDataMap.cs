using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class SearchResultRawDataMap : BaseEntityConfiguration<SearchResultRawData>
    {
        public SearchResultRawDataMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            //this.Property(t => t.JSON)
            //       .IsRequired();

            // Table & Column Mappings
            this.ToTable("SearchResultRawData");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.JSON).HasColumnName("JSON");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");

            // Relationships
            this.HasRequired(t => t.SearchSummary)
                .WithMany(t => t.SearchResultRawDatas)
                .HasForeignKey(d => d.SearchSummaryID);

        }
    }
}
