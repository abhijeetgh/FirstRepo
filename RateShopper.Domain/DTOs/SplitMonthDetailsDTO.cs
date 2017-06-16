using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class SplitMonthDetailsDTO
    {
        public long ID { get; set; }        
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string Label { get; set; }
        public int SplitIndex { get; set; }
    }
}
