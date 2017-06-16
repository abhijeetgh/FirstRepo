using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public partial class QuickviewCarClassGroupMap : BaseEntityConfiguration<QuickviewCarClassGroup>
    {
        public QuickviewCarClassGroupMap()
        {
            this.ToTable("QuickviewCarClassGroup");

            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.CarClassId).HasColumnName("CarClassId");
        }
    }
}
