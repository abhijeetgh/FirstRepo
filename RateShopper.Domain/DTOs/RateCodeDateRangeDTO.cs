using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RateCodeDateRangeDTO
    {
        public long ID { get; set; }
        public long RateCodeID { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isUpdated { get; set; }
    }
}
