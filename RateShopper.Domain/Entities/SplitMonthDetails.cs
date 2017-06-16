using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class SplitMonthDetails: BaseEntity
    {
        public int SplitIndex { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string Label { get; set; }        
    }
}
