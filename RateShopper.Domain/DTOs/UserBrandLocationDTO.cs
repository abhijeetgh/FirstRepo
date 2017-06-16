using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class UserBrandLocationDTO
    {
        public long BrandLocationId { get; set; }
        public string Alias { get; set; }
        public long BrandId { get; set; }
        public string LocationCode { get; set; }
    }
}
