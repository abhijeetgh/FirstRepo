using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class PostJSONResponseLogMap : BaseEntityConfiguration<Domain.Entities.PostJSONResponseLog>
    {
        public PostJSONResponseLogMap()
        {
            this.ToTable("PostJSONResponseLogs");
            this.Property(t => t.SearchSummaryIDs).HasColumnName("SearchSummaryIDs");
            this.Property(t => t.Response).HasColumnName("Response");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
        }
    }
}
