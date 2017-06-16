using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class WriteOffDto
    {
        public long WriteOffId { get; set; }
        public Nullable<double> Amount { get; set; }
        public double? TotalWriteOff { get; set; }
        public double? TotalDue { get; set; }
        public DateTime WriteOffDate { get; set; }
        public long WriteOffTypeId { get; set; }
        public string WriteOffType { get; set; }
        public long ClaimId { get; set; }
        public string ReferenceNumber { get; set; }

        public IEnumerable<WriteOffDto> WriteOffInfo { get; set; }
    }
}
