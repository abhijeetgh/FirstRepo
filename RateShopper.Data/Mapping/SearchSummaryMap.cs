using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class SearchSummaryMap : BaseEntityConfiguration<SearchSummary>
    {
        public SearchSummaryMap()
        {
            // Table & Column Mappings
            this.ToTable("SearchSummary");

            this.Property(t => t.ScrapperSourceIDs).HasColumnName("ScrapperSourceIDs");
            this.Property(t => t.LocationBrandIDs).HasColumnName("LocationBrandIDs");
            this.Property(t => t.CarClassesIDs).HasColumnName("CarClassesIDs");
            this.Property(t => t.RentalLengthIDs).HasColumnName("RentalLengthIDs");
            this.Property(t => t.ScheduledJobID).HasColumnName("ScheduledJobID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.StatusID).HasColumnName("StatusID");

            this.Property(t => t.RetryCount).HasColumnName("RetryCount");
            this.Property(t => t.Response).HasColumnName("Response");
            this.Property(t => t.RequestURL).HasColumnName("RequestURL");
            this.Property(t => t.IsReviewed).HasColumnName("IsReviewed");
            this.Property(t => t.IsQuickView).HasColumnName("IsQuickView");
            this.Property(t => t.ProviderId).HasColumnName("ProviderId");
            this.Property(t => t.ShopRequestId).HasColumnName("ShopRequestId");
            this.Property(t => t.HasQuickViewChild).HasColumnName("HasQuickViewChild");
            this.Property(t => t.PostData).HasColumnName("PostData");
            this.Property(t => t.IsGov).HasColumnName("IsGov");
            this.Property(t => t.ShopType).HasColumnName("ShopType");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.SearchSummaries)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.SearchSummaries1)
                .HasForeignKey(d => d.UpdatedBy);           
        }
    }
}
