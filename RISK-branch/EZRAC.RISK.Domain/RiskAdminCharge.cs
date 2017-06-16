using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskAdminCharge
    {
        public int Id { get; set; }
        public double EstimatedAmount { get; set; }
        public double AdminAmount { get; set; }
    }
}
