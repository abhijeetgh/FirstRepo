using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class TrackingDto
    {
        public long ClaimId { get; set; }
        public long Id { get; set; }
        public long ClaimTrackingId { get; set; }
        public long TrackingTypeId { get; set; }
        public string TrackingDescription { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
        public long TrackingId { get; set; }
        public bool CanUndo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public int SequenceId { get; set; }
    }
}
