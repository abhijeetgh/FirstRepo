using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class UserActionLogMapping : EntityTypeConfiguration<UserActionLog>
    {
        public UserActionLogMapping()
        {
            ToTable("UserActionLog");
            this.HasRequired(s => s.User)
                     .WithMany()
                     .HasForeignKey(s => s.UserId);
        }
    }
}
