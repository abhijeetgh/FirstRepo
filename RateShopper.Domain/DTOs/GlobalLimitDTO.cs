using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class GlobalLimitDetailsDTO
    {
        public long? CarClassID { get; set; }
        public string CarClass { get; set; }
        public long BrandLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? GlobalLimitID { get; set; }
        public long? GlobalDetailsID { get; set; }
        public decimal? DayMin { get; set; }
        public decimal? DayMax { get; set; }
        public decimal? WeekMin { get; set; }
        public decimal? WeekMax { get; set; }
        public decimal? MonthlyMin { get; set; }
        public decimal? MonthlyMax { get; set; }
    }

    public class GlobalLimitDTO
    {
        public GlobalLimitDTO()
        {
            LstGlobalLimitDetails = new List<GlobalLimitDetailsDTO>();
        }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long? GlobalLimitID { get; set; }
        public long CreatedBy { get; set; }
        public long BrandLocation { get; set; }
        public List<GlobalLimitDetailsDTO> LstGlobalLimitDetails { get; set; }
    }
}
