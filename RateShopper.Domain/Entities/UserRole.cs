using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class UserRole : BaseEntity
    {
        public UserRole()
        {
            this.Users = new List<User>();
        }
        [Required,MaxLength(50)]        
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
