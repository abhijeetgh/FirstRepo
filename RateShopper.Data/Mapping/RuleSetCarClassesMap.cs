using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RuleSetCarClassesMap : BaseEntityConfiguration<RuleSetCarClasses>
    {
        public RuleSetCarClassesMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetCarClasses");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.CarClassID).HasColumnName("CarClassID");

            // Relationships
            this.HasRequired(t => t.CarClass)
                .WithMany(t => t.RuleSetCarClasses)
                .HasForeignKey(d => d.CarClassID);
            this.HasRequired(t => t.RuleSet)
                .WithMany(t => t.RuleSetCarClasses)
                .HasForeignKey(d => d.RuleSetID);

        }
    }
}
