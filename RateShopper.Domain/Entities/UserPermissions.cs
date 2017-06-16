using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public class UserPermissions : BaseEntity
    {
        public string Permission { get; set; }
        public string PermissionKey { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<UserPermissionMapper> UserPermissionMappers { get; set; }
    }
}
