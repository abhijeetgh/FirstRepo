using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{

    public class LocationBrandModel
    {
        public long ID { get; set; }
        public long LocationID { get; set; }
        public string LocationBrandAlias { get; set; }
        public string LocationCode { get; set; }
        public long BrandID { get; set; }
        public string LOR { get; set; }
    }

    public class LocationBrandJobTether
    {
        public long LocationBrandID { get; set; }
        public long LocationID { get; set; }
        public string LocationBrandAlias { get; set; }
        public long? DominentBrandID { get; set; }
        public int Flag { get; set; }
        public string BranchCode { get; set; }
    }
}
