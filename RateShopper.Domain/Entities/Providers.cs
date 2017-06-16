using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public class Providers : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public bool IsOneWay { get; set; }
        public bool IsGov { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
    }
}
