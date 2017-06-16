using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RuleSetsAppliedMap : BaseEntityConfiguration<RuleSetsApplied>
    {
        public RuleSetsAppliedMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetsApplied");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.RuleSet)
                .WithMany(t => t.RuleSetsApplieds)
                .HasForeignKey(d => d.RuleSetID);

        }
    }
}
