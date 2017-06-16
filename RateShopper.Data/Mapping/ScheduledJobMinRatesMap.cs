using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class ScheduledJobMinRatesMap : BaseEntityConfiguration<ScheduledJobMinRates>
    {
        public ScheduledJobMinRatesMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("ScheduledJobMinRates");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ScheduleJobID).HasColumnName("ScheduleJobID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.DayMin).HasColumnName("DayMin");
            this.Property(t => t.WeekMin).HasColumnName("WeekMin");
            this.Property(t => t.MonthMin).HasColumnName("MonthMin");
            this.Property(t => t.DayMax).HasColumnName("DayMax");
            this.Property(t => t.WeekMax).HasColumnName("WeekMax");
            this.Property(t => t.MonthMax).HasColumnName("MonthMax");
            this.Property(t => t.Day2Min).HasColumnName("Day2Min");
            this.Property(t => t.Day2Max).HasColumnName("Day2Max");
            this.Property(t => t.Week2Min).HasColumnName("Week2Min");
            this.Property(t => t.Week2Max).HasColumnName("Week2Max");
            this.Property(t => t.Month2Min).HasColumnName("Month2Min");
            this.Property(t => t.Month2Max).HasColumnName("Month2Max");
            this.Property(t => t.Days1).HasColumnName("Days1");
            this.Property(t => t.Days2).HasColumnName("Days2");
            // Relationships

            // Relationships
            this.HasRequired(t => t.ScheduledJob)
                .WithMany(t => t.ScheduledJobMinRates)
                .HasForeignKey(d => d.ScheduleJobID);

        }
    }
}
