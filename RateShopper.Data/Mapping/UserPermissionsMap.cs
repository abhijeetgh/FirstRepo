using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class UserPermissionsMap : BaseEntityConfiguration<UserPermissions>
    {
        public UserPermissionsMap()
        {
            this.ToTable("UserPermissions");
            this.Property(t => t.Permission).HasColumnName("Permission");
            this.Property(t => t.PermissionKey).HasColumnName("PermissionKey");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
            