using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class MinRatesDTO
    {
        //public string CarClass { get; set; }
        public long CarClassId { get; set; }
        public decimal? DayMinRate { get; set; }
        public decimal? WeekMin { get; set; }
        public decimal? MonthMin { get; set; }
        public decimal? DayMax { get; set; }
        public decimal? WeekMax { get; set; }
        public decimal? MonthMax { get; set; }
        public decimal? Day2Min { get; set; }
        public decimal? Day2Max { get; set; }
        public decimal? Week2Min { get; set; }
        public decimal? Week2Max { get; set; }
        public decimal? Month2Min { get; set; }
        public decimal? Month2Max { get; set; }
        public string Days1 { get; set; }
        public string Days2 { get; set; }
    }
}
