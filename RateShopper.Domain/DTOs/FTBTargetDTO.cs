using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FTBTargetDTO
    {
        public long FTBTargetId { get; set; }
        public long LocationBrandId { get; set; }
        public string Date { get; set; }
        public long DayOfWeekId { get; set; }
        public string Day { get; set; }
        public long? Target { get; set; }
        public long LoggedUserId { get; set; }
        public bool IsUpdate { get; set; }
        public IEnumerable<FTBTargetsDetailDTO> FTBTargetsDetailDTOs { get; set; }
    }
    public class FTBTargetsDetailDTO
    {
        public long FTBTargetDetailId { get; set; }
        public long FTBTargetId { get; set; }
        public decimal? PercentTarget { get; set; }
        public decimal? PercentRateIncrease { get; set; }
        public int SlotOrder { get; set; }

    }
}
