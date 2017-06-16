using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Data.Mapping
{
    public class ScrappingServersMap : BaseEntityConfiguration<ScrappingServers>
    {
        public ScrappingServersMap()
        {
            // Table & Column Mappings
            this.ToTable("ScrappingServers");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.IsReadOnlyShop).HasColumnName("IsReadOnlyShop");
            this.Property(t => t.IsAutomation).HasColumnName("IsAutomation");
            this.Property(t => t.IsQuickView).HasColumnName("IsQuickView");
            this.Property(t => t.IsSummaryShop).HasColumnName("IsSummaryShop");
            this.Property(t => t.IsNormalShop).HasColumnName("IsNormalShop");
            this.Property(t => t.LastUsedDateTime).HasColumnName("LastUsedDateTime");
        }
    }
}
