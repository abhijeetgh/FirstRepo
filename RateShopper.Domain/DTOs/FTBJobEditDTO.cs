using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FTBJobEditDTO
    {

        public long? JobId { get; set; }
        public long LocationBrandID { get; set; }
        public long ScheduledJobFrequencyID { get; set; }
        //Days to Run
        //public DateTime DaysToRunStartDate { get; set; }
        //public DateTime DaysToRunEndDate { get; set; }
        public string RunTime { get; set; }
        public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public bool IsActiveTethering { get; set; }
        public bool IsEnabled { get; set; }
        public bool ExecutionInProgress { get; set; }
        public string JobScheduleWeekDays { get; set; }
        public long LoggedInUserId { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }

    }
}
