using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class ScheduleJobLocationBrandDTO
    {
        public long BrandLocationId { get; set; }
        public string Alias { get; set; }
        public long BrandId { get; set; }
    }

    public class ScheduleJobUserDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
