using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class UserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }        
        public string Password { get; set; }
        public long UserRoleID { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public long CurrentUserId { get; set; }
        public string ErroMsg { get; set; }
        public List<long> LocationIds { get; set; }
        //public UserRoleDto userRoleDto { get; set; }
    }
}
