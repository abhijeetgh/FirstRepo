using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class RuleSetMap : BaseEntityConfiguration<RuleSet>
    {
        public RuleSetMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("RuleSets");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CompanyPositionAbvAvg).HasColumnName("CompanyPositionAbvAvg");
            this.Property(t => t.IsPositionOffset).HasColumnName("IsPositionOffset");
            this.Property(t => t.IsWideGapTemplate).HasColumnName("IsWideGapTemplate");
            this.Property(t => t.IsGov).HasColumnName("IsGov");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.SelectedCompanyIDs).HasColumnName("SelectedCompanyIDs");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
           // this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.RuleSets)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.RuleSets)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.RuleSets1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
