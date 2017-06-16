using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class UserPermissionMapperMap : BaseEntityConfiguration<UserPermissionMapper>
    {
        public UserPermissionMapperMap()
        {
            this.ToTable("UserPermissionMapper");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.UserPermissionID).HasColumnName("UserPermissionID");

            this.HasRequired(t => t.UserPermissions)
               .WithMany(t => t.UserPermissionMappers)
               .HasForeignKey(d => d.UserPermissionID);

             this.HasRequired(t => t.Users)
               .WithMany(t => t.UserPermissionMappers)
               .HasForeignKey(d => d.UserID);
        }
    }
}
