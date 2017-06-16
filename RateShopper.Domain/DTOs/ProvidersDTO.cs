using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ProvidersDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsOneWay { get; set; }
        public bool IsGov { get; set; }
        public string Code { get; set; }
    }
}
