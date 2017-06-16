using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class UserDTO
    {
        public long UserID { get; set; }
        public long UserRoleID { get; set; }
        public string UserRole { get; set; }
        public long LoggedUserID {get;set;}

        public string LocationBrandID { get; set; }
        public string AddLocationBrandID { get; set; }
        public string DeletLocationBrandID { get; set; }
        public string PriceMgrLocationID { get; set; }

        public string SourceID { get; set; }
        public string AddSourceID { get; set; }
        public string DeleteSourceID { get; set; }
        public string AddUserPermission { get; set; }
        public string DeleteUserPermission { get; set; }

        public string SelectedPermissionID { get; set; }

        public string AddPriceMgrLocationID { get; set; }
        public string DeletePriceMgrLocationID { get; set; }
     
    }
}
