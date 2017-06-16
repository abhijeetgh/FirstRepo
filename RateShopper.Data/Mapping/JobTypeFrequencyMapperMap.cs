using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class JobTypeFrequencyMapperMap : BaseEntityConfiguration<JobTypeFrequencyMapper>
    {
        public JobTypeFrequencyMapperMap()
        {
            
            this.ToTable("JobTypeFrequencyMapper");

            this.Property(t => t.ScheduleJobFrequencyId).HasColumnName("ScheduleJobFrequencyId");
            this.Property(t => t.JobType).HasColumnName("JobType");

            this.HasRequired(t => t.ScheduledJobFrequency)
              .WithMany(t => t.JobTypeFrequencyMapper)
              .HasForeignKey(d => d.ScheduleJobFrequencyId);
        }
    }
}
