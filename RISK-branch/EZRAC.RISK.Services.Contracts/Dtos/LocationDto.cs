using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class LocationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CompanyAbbr { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        public string State { get; set; }

        public long AddedOrUpdatedUserId { get; set; }
       
    }
}
