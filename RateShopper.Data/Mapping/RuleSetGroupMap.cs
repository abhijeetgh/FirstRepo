using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class RuleSetGroupMap : BaseEntityConfiguration<RuleSetGroup>
    {
        public RuleSetGroupMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.GroupName)
            //    .IsRequired()
            //    .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("RuleSetGroups");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");

            // Relationships
            this.HasRequired(t => t.RuleSet)
                .WithMany(t => t.RuleSetGroups)
                .HasForeignKey(d => d.RuleSetID);

        }
    }
}
