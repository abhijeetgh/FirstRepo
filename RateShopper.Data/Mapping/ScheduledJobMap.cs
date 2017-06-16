using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class ScheduledJobMap : BaseEntityConfiguration<ScheduledJob>
    {
        public ScheduledJobMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            // Table & Column Mappings
            this.ToTable("ScheduledJobs");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ScheduledJobFrequencyID).HasColumnName("ScheduledJobFrequencyID");
            this.Property(t => t.LastRunDateTime).HasColumnName("LastRunDateTime");
            this.Property(t => t.NextRunDateTime).HasColumnName("NextRunDateTime");
            this.Property(t => t.CustomHoursToRun).HasColumnName("CustomHoursToRun");
            this.Property(t => t.CustomMinutesToRun).HasColumnName("CustomMinutesToRun");
            this.Property(t => t.RunTime).HasColumnName("RunTime");
            this.Property(t => t.RunDay).HasColumnName("RunDay");
            this.Property(t => t.DaysToRunStartDate).HasColumnName("DaysToRunStartDate");
            this.Property(t => t.DaysToRunEndDate).HasColumnName("DaysToRunEndDate");
            this.Property(t => t.IsStandardShop).HasColumnName("IsStandardShop");
            this.Property(t => t.StartDateOffset).HasColumnName("StartDateOffset");
            this.Property(t => t.EndDateOffset).HasColumnName("EndDateOffset");
            this.Property(t => t.ScrapperSourceIDs).HasColumnName("ScrapperSourceIDs");
            this.Property(t => t.LocationBrandIDs).HasColumnName("LocationBrandIDs");
            this.Property(t => t.CarClassesIDs).HasColumnName("CarClassesIDs");
            this.Property(t => t.RentalLengthIDs).HasColumnName("RentalLengthIDs");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.PickUpTime).HasColumnName("PickUpTime");
            this.Property(t => t.DropOffTime).HasColumnName("DropOffTime");
            this.Property(t => t.RequestURL).HasColumnName("RequestURL");
            this.Property(t => t.IsEnabled).HasColumnName("IsEnabled");
            this.Property(t => t.IsWideGapTemplate).HasColumnName("IsWideGapTemplate");
            this.Property(t => t.IsActiveTethering).HasColumnName("IsActiveTethering");
            this.Property(t => t.ExecutionInProgress).HasColumnName("ExecutionInProgress");
            this.Property(t => t.JobScheduleWeekDays).HasColumnName("JobScheduleWeekDays");
            this.Property(t => t.TSDUpdateWeekDay).HasColumnName("TSDUpdateWeekDay");

            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.PostData).HasColumnName("PostData");
            this.Property(t => t.ProviderId).HasColumnName("ProviderId");
            this.Property(t => t.IsGov).HasColumnName("IsGov");
            this.Property(t => t.IsGovTemplate).HasColumnName("IsGovTemplate");
            this.Property(t => t.IsStopByFTB).HasColumnName("IsStopByFTB");
            this.Property(t => t.CompeteOnBase).HasColumnName("CompeteOnBase");
            this.Property(t => t.IsOpaqueActive).HasColumnName("IsOpaqueActive");
            this.Property(t => t.OpaqueRateCodes).HasColumnName("OpaqueRateCodes");

            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships

            this.HasRequired(t => t.ScheduledJobFrequency)
                .WithMany(t => t.ScheduledJobs)
                .HasForeignKey(d => d.ScheduledJobFrequencyID);


        }
    }
}
