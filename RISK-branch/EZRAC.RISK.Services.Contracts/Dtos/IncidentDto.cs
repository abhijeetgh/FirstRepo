using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class IncidentDto
    {
        public Nullable<DateTime> LossDate { get; set; }

      
        public Nullable<Int64> SelectedLocationId { get; set; }
        public string SelectedLocationName { get; set; }

        public string SelectedPoliceAgencyName { get; set; }
        public Nullable<int> SelectedPoliceAgencyId { get; set; }

        public string CaseNumber { get; set; }
        public bool RenterFault { get; set; }
        public bool ThirdPartyFault { get; set; }
        public Nullable<DateTime> ReportedDate { get; set; }

        public Int64 Id { get; set; }
    }
}
