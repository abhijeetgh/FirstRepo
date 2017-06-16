using EZRAC.RISK.Domain;
using System.Data.Entity.ModelConfiguration;


namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class UserRoleMap : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMap()
        {
            ToTable("UserRole");
            Property(u => u.Name);
            HasMany<Permission>(x => x.Permissions).WithMany(u => u.UserRoles)
                .Map(c =>
                {
                    c.MapLeftKey("RoleId");
                    c.MapRightKey("PermissionId");
                    c.ToTable("RolePermission");
                });
            
        }
    }
}