using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class QuickViewMap:BaseEntityConfiguration<QuickView>
    {
        public QuickViewMap()
        {
            this.ToTable("QuickView");

            this.Property(t => t.ParentSearchSummaryId).HasColumnName("ParentSearchSummaryId");
            this.Property(t => t.ChildSearchSummaryId).HasColumnName("ChildSearchSummaryId");
            this.Property(t => t.CompetitorCompanyIds).HasColumnName("CompetitorCompanyIds");
            this.Property(t => t.LastRunDate).HasColumnName("LastRunDate");
            this.Property(t => t.NextRunDate).HasColumnName("NextRunDate");
            this.Property(t => t.IsEnabled).HasColumnName("IsEnabled");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.IsExecutionInProgress).HasColumnName("ExecutionInProgress");
            this.Property(t => t.StatusId).HasColumnName("StatusId");
            this.Property(t => t.UIControlId).HasColumnName("UIControlId");
            this.Property(t => t.CarClassIds).HasColumnName("CarClassIds");

            this.Property(t => t.MonitorBase).HasColumnName("MonitorBase");
            this.Property(t => t.NotifyEmail).HasColumnName("NotifyEmail");
            this.Property(t => t.PickupTime).HasColumnName("PickupTime");
            this.Property(t => t.DropoffTime).HasColumnName("DropoffTime");
        }
    }
}
