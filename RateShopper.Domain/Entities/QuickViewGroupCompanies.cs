using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class QuickViewGroupCompanies:BaseEntity
    {
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
        public virtual QuickViewGroup QuickViewGroup { get; set; }
    }
}
