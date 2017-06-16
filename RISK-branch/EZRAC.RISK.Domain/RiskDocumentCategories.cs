using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class RiskDocumentCategory : BaseEntity
    {
      
        public string Category { get; set; }

        public IList<RiskDocumentType> RiskDocumentTypes { get; set; }
    }
}
