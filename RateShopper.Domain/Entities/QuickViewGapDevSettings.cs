using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public partial class QuickViewGapDevSettings : BaseEntity
    {
        public long CompetitorGroupId { get; set; }
        public long CarClassGroupId { get; set; }
        //public decimal GapValue { get; set; }
        public decimal DeviationValue { get; set; }
        public virtual QuickViewGroup QuickViewGroup { get; set; }
    }
}
