using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class FTBRatesTSDLogs:BaseEntity
    {
        public long ScheduleJobId { get; set; }
        public DateTime ShopDate { get; set; }
        public long ReservationCount { get; set; }
        public decimal Target { get; set; }
        public long TargetDetailsID { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
