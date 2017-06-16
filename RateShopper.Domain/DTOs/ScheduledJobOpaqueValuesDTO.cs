using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ScheduledJobOpaqueValuesDTO
    {
        public long Id { get; set; }
        public long CarClassId { get; set; }
        public long ScheduledJobId { get; set; }
        public string CarCode { get; set; }
        public decimal PercenValue { get; set; }
    }
}
