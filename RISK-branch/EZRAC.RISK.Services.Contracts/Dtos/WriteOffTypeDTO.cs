using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class WriteOffTypeDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
