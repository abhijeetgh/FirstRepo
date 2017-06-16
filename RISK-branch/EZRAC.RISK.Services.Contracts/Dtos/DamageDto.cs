using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DamageDto
    {
        public string Section { get; set; }
        public int SectionId { get; set; }
        public string Details { get; set; }
        public int VehicleId { get; set; }
        public int ClaimId { get; set; }
        public long DamageId { get; set; }
    }
}
