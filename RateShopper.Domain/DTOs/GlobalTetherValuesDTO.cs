using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class GlobalTetherValuesDTO
    {
        public long CarClassId { get; set; }
        public decimal TetherValue { get; set; }
        public bool IsPercentage { get; set; }
    }

    public class ExistingTetheredLocationsDTO
    {
        public long DominantBrandId { get; set; }
        public long DependantBrandId { get; set; }
        public string LocationIds { get; set; }
        public string DominantBrand { get; set; }
        public string DependantBrand { get; set; }
        public string Locations { get; set; }
    }
}
