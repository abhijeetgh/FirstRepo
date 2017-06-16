using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;


namespace RateShopper.Data.Mapping
{
    public class UserRoleMap : BaseEntityConfiguration<UserRole>
    {
        public UserRoleMap()
        {
            // Table & Column Mappings
            this.ToTable("UserRoles");
            this.Property(t => t.Role).HasColumnName("Role");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
