using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class CompanyDTO
    {
        public CompanyDTO()
        {
            lstLocations = new List<LocationCompanyDTO>();
        }

        public string Code { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public long CreatedBy { get; set; }
        public List<LocationCompanyDTO> lstLocations { get; set; }
        public bool SelectedForQuickView { get; set; }
    }
    public class CompanyMasterDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool IsBrand { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
    }
}
