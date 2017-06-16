using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class SearchTagPlateDto
    {
        public string UnitNumber { get; set; }
        public string Messages { get; set; }
        public Nullable<DateTime> TransDate { get; set; }
    }
}
