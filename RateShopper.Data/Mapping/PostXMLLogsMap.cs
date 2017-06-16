using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class PostXMLLogsMap : BaseEntityConfiguration<PostXMLLogs>
    {
        public PostXMLLogsMap()
        {
            this.ToTable("PostXMLLogs");
            this.Property(d => d.SearchSummaryIds).HasColumnName("SearchSummaryIds");
            this.Property(d => d.HighestTSDId).HasColumnName("HighestTSDId");
            this.Property(d => d.Response).HasColumnName("Response");
            this.Property(d => d.CreatedDateTime).HasColumnName("CreatedDateTime");
        }
    }
}
