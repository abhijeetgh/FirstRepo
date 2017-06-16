using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class Sli
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string PolicyNumber { get; set; }
        public string State { get; set; }

        public List<Location> SliLocations { get; set; }
    }
}
