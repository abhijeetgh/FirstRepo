using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RuleSetWeekDayMap : BaseEntityConfiguration<RuleSetWeekDay>
    {
        public RuleSetWeekDayMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetWeekDay");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.WeekDayID).HasColumnName("WeekDayID");

            // Relationships
            this.HasRequired(t => t.RuleSet)
                .WithMany(t => t.RuleSetWeekDays)
                .HasForeignKey(d => d.RuleSetID);
            this.HasRequired(t => t.WeekDay)
                .WithMany(t => t.RuleSetWeekDay)
                .HasForeignKey(d => d.WeekDayID);

        }
    }
}
