using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class ScrappingServers : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public Nullable<bool> IsReadOnlyShop { get; set; }
        public Nullable<bool> IsAutomation { get; set; }
        public Nullable<bool> IsQuickView { get; set; }
        public Nullable<bool> IsSummaryShop { get; set; }
        public Nullable<bool> IsNormalShop { get; set; }
        public DateTime LastUsedDateTime { get; set; }
    }
}
