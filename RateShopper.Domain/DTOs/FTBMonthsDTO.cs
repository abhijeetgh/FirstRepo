using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
   public class FTBMonthsDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear { get; set; }
        public bool IsScheduledStopped { get; set; }
   }
}
