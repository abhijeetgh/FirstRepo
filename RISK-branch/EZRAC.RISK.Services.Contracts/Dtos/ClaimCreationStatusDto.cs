using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class ClaimCreationStatusDto
    {
        public bool Status { get; set; }
        public Int64 ClaimId { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedUserEmail { get; set; }

    }
}
