using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class PermissionMapping : EntityTypeConfiguration<Permission>
    {
        public PermissionMapping()
        {
            this.ToTable("Permissions");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PermissionLevelId).HasColumnName("PermissionLevelId");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
        }
    }
}
