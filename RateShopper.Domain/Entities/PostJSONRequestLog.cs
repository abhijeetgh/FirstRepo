using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class PostJSONRequestLog : BaseEntity
    {   
        public long SearchSummaryID { get; set; }
        public string JSONRequest { get; set; }
        public bool IsDataSent { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
