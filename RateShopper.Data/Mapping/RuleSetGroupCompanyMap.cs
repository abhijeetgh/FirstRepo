using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class RuleSetGroupCompanyMap : BaseEntityConfiguration<RuleSetGroupCompany>
    {
        public RuleSetGroupCompanyMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetGroupCompanies");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetGroupID).HasColumnName("RuleSetGroupID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.RuleSetGroupCompanies)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.RuleSetGroup)
                .WithMany(t => t.RuleSetGroupCompanies)
                .HasForeignKey(d => d.RuleSetGroupID);

        }
    }
}
