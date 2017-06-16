using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class CallbackResponse : BaseEntity
    {
        public long SearchSummaryId { get; set; }
        public Nullable<long> ShopRequestId { get; set; }
        public string RawData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
