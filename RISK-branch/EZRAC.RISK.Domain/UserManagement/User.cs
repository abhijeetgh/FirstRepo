using System;
using System.Collections.Generic;


namespace EZRAC.RISK.Domain
{
    public class User : AuditableEntity
    {        
        /// <summary>
        /// The username 
        /// </summary>
        public string UserName { get; set; } 

        /// <summary>
        /// The users email address
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>
        public string PasswordHash { get; set; }       

        /// <summary>
        /// The users first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The users surname
        /// </summary>
        public string LastName { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public Int64 UserRoleID { get; set; }

        public UserRole Role { get; set; }

        public IList<Claim> Claims { get; set; }

        public IList<Location> Locations { get; set; }

        public IList<ClaimTrackings> ClaimTrackings { get; set; }
    }
}
