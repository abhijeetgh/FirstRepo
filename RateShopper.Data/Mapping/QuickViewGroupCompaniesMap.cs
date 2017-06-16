using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public partial class QuickViewGroupCompaniesMap : BaseEntityConfiguration<QuickViewGroupCompanies>
    {
        public QuickViewGroupCompaniesMap()
        {
            this.ToTable("QuickViewGroupCompanies");

            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");

            this.HasRequired(t => t.QuickViewGroup)
                .WithMany(t => t.QuickViewGroupCompanies)
                .HasForeignKey(d => d.CompanyId);
        }
    }
}
