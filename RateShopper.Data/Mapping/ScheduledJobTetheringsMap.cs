using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;


namespace RateShopper.Data.Mapping
{
    public class ScheduledJobTetheringsMap : BaseEntityConfiguration<ScheduledJobTetherings>
    {
        public ScheduledJobTetheringsMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("ScheduledJobTetherings");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ScheduleJobID).HasColumnName("ScheduleJobID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.DominentBrandID).HasColumnName("DominentBrandID");
            this.Property(t => t.DependantBrandID).HasColumnName("DependantBrandID");
            this.Property(t => t.TetherValue).HasColumnName("TetherValue");
            this.Property(t => t.IsTetherValueinPercentage).HasColumnName("IsTetherValueinPercentage");
            // Relationships

            // Relationships
            this.HasRequired(t => t.ScheduledJob)
                .WithMany(t => t.ScheduledJobTetherings)
                .HasForeignKey(d => d.ScheduleJobID);

            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.ScheduledJobTetherings)
                .HasForeignKey(d => d.CarClassID);

            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.ScheduledJobTetherings)
                .HasForeignKey(d => d.LocationBrandID);

            this.HasRequired(t => t.Company)
               .WithMany(t => t.ScheduledJobTetherings)
               .HasForeignKey(d => d.DominentBrandID);
            this.HasRequired(t => t.Company1)
                .WithMany(t => t.ScheduledJobTetherings1)
                .HasForeignKey(d => d.DependantBrandID);
        }
    }   
}
