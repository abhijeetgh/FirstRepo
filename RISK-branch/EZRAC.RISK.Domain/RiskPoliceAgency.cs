using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskPoliceAgency:AuditableEntity
    {
        public int Id { get; set; }
        public string AgencyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

        public IList<RiskIncident> Incidents { get; set; }
        
    }
}
