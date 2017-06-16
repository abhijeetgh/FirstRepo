using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Domain
{
    public class SliLocation
    {
        public int Id { get; set; }
        public int SliId { get; set; }
        public long LocationId { get; set; }

        public Location Location { get; set; }
        public Sli Sli { get; set; }
    }
}
