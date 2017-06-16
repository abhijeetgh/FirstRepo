using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class LocationCompanyDTO
    {
        public long? ID { get; set; }
        public long LocationID { get; set; }
        public long CompanyID { get; set; }
        public bool? IsTerminalInside { get; set; }
        public string LocationName { get; set; }
    }
}
