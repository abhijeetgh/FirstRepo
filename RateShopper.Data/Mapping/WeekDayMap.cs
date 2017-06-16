using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class WeekDayMap : BaseEntityConfiguration<WeekDay>
    {
        public WeekDayMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Day)
            //    .IsRequired()
            //    .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WeekDays");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Day).HasColumnName("Day");
        }
    }
}
