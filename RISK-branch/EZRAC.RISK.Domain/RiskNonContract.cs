using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskNonContract
    {
        public long Id { get; set; }
        public string NonContractNumber { get; set; }

        public IList<Claim> Claims { get; set; }
    }
}
