using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class EventTrackingCategory : BaseEntity
    {
        public string CategoryName { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
