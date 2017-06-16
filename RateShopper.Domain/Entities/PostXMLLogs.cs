using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class PostXMLLogs : BaseEntity
    {
        public string SearchSummaryIds { get; set; }
        public long HighestTSDId { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
