using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RateCodeDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public long CreatedBy { get; set; }
        public string SupportedBrandIDs { get; set; }
        public bool IsActive { get; set; }
        public List<long> DeletedDateRangeIds { get; set; }

        public List<RateCodeDateRangeDTO> DateRangeList{ get; set; }
    }
}
