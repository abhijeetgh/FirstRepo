using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public partial class QuickViewGroupMap : BaseEntityConfiguration<QuickViewGroup>
    {
        public QuickViewGroupMap()
        {
            this.ToTable("QuickViewGroups");

            this.Property(t => t.QuickViewId).HasColumnName("QuickViewId");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
        }
    }
}
