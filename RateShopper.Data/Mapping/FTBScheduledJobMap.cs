using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class FTBScheduledJobMap : BaseEntityConfiguration<FTBScheduledJob>
    {
        public FTBScheduledJobMap()
        {
            this.ToTable("FTBScheduledJobs");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.ScrapperSourceID).HasColumnName("ScrapperSourceID");
            this.Property(t => t.ScheduledJobFrequencyID).HasColumnName("ScheduledJobFrequencyID");
            this.Property(t => t.LastRunDateTime).HasColumnName("LastRunDateTime");
            this.Property(t => t.NextRunDateTime).HasColumnName("NextRunDateTime");
            this.Property(t => t.DaysToRunStartDate).HasColumnName("DaysToRunStartDate");
            this.Property(t => t.DaysToRunEndDate).HasColumnName("DaysToRunEndDate");
            this.Property(t => t.RunTime).HasColumnName("RunTime");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.IsActiveTethering).HasColumnName("IsActiveTethering");
            this.Property(t => t.IsEnabled).HasColumnName("IsEnabled");
            this.Property(t => t.ExecutionInProgress).HasColumnName("ExecutionInProgress");
            this.Property(t => t.JobScheduleWeekDays).HasColumnName("JobScheduleWeekDays");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");


            this.HasRequired(t => t.ScheduledJobFrequency)
             .WithMany(t => t.FTBScheduledJob)
             .HasForeignKey(d => d.ScheduledJobFrequencyID);
            this.HasRequired(t => t.User)
            .WithMany(t => t.FTBScheduledJob)
            .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
            .WithMany(t => t.FTBScheduledJob1)
            .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
