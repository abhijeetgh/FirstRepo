using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class GlobalLimitDetailMap : BaseEntityConfiguration<GlobalLimitDetail>
    {
        public GlobalLimitDetailMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("GlobalLimitDetails");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.GlobalLimitID).HasColumnName("GlobalLimitID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");
            this.Property(t => t.DayMin).HasColumnName("DayMin");
            this.Property(t => t.DayMax).HasColumnName("DayMax");
            this.Property(t => t.WeekMin).HasColumnName("WeekMin");
            this.Property(t => t.WeekMax).HasColumnName("WeekMax");
            this.Property(t => t.MonthMin).HasColumnName("MonthMin");
            this.Property(t => t.MonthMax).HasColumnName("MonthMax");

            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.GlobalLimitDetails)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.GlobalLimit)
                .WithMany(t => t.GlobalLimitDetails)
                .HasForeignKey(d => d.GlobalLimitID);

        }
    }
}
