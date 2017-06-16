using System;
using System.Collections.Generic;


namespace EZRAC.RISK.Domain
{
    public class UserRole : BaseEntity
    {      
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Permission> Permissions { get; set; }


    }
}