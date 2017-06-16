using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public class UserPermissionMapper : BaseEntity
    {
        public long UserID { get; set; }
        public long UserPermissionID { get; set; }

        public virtual User Users { get; set; }
        public virtual UserPermissions UserPermissions { get; set; }
    }
}
 