using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class PostJSONResponseLog : BaseEntity
    {
        public string SearchSummaryIDs { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
