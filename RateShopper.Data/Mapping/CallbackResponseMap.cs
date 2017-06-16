using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    class CallbackResponseMap:BaseEntityConfiguration<CallbackResponse>
    {
        public CallbackResponseMap()
        {
            this.ToTable("CallbackResponse");

            this.Property(t => t.SearchSummaryId).HasColumnName("SearchSummaryId");
            this.Property(t => t.RawData).HasColumnName("RawData");
            this.Property(t => t.ShopRequestId).HasColumnName("ShopRequestId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
                
        }
    }
}
