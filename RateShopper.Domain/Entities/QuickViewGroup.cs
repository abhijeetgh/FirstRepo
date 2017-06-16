using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class QuickViewGroup :BaseEntity
    {
        public long QuickViewId { get; set; }

        [MaxLength(50)]
        public string GroupName { get; set; }

        public virtual ICollection<QuickViewGroupCompanies> QuickViewGroupCompanies { get; set; }
        public virtual ICollection<QuickViewGapDevSettings> QuickViewGapDevSettings { get; set; }
    }
}
