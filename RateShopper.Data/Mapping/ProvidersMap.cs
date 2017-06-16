using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class ProvidersMap : BaseEntityConfiguration<Domain.Entities.Providers>
    {
        public ProvidersMap()
        {
            // Table & Column Mappings
            this.ToTable("Providers");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.IsOneWay).HasColumnName("IsOneWay");
            this.Property(t => t.IsGov).HasColumnName("IsGov");
            this.Property(t => t.Code).HasColumnName("Code");
        }
    }
}
