using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class StatusMap : BaseEntityConfiguration<Statuses>
    {
        public StatusMap()
        {
            // Table & Column Mappings
            this.ToTable("Status");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
