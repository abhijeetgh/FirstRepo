using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ScheduledJobTetheringsDTO
    {
        public long ScheduleJobID { get; set; }
        public long LocationBrandID { get; set; }
        public long DominentBrandID { get; set; }
        public long DependantBrandID { get; set; }
        public List<ScheduledJobTetherCarClassDTO> lstScheduledJobTetherCarClass { get; set; }
    }
    public class ScheduledJobTetherCarClassDTO
    {
        public long ID { get; set; }
        public long CarClassID { get; set; }
        public decimal? TetherValue { get; set; }
        public bool IsTetherValueinPercentage { get; set; }
    }
}
