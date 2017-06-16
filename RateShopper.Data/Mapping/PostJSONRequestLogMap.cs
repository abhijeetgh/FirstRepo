using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class PostJSONRequestLogMap : BaseEntityConfiguration<Domain.Entities.PostJSONRequestLog>
    {
        public PostJSONRequestLogMap()
        {
            this.ToTable("PostJSONRequestLogs");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.JSONRequest).HasColumnName("JSONRequest");
            this.Property(t => t.IsDataSent).HasColumnName("IsDataSent");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
        }
    }
}
