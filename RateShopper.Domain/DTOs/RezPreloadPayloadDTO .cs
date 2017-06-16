using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RezPreloadPayloadDTO
    {
        public string RateSource { get; set; }
        public string LocationCode { get; set; }
        public string RateCode { get; set; }
        public string RequestDate { get; set; }
    }
}
