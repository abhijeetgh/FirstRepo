using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class RentalLengthDTO
    {
        public long ID { get; set; }
        public long MappedID { get; set; }
        public string Code { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<bool> IsSearchEnable { get; set; }
        public long? AssociateMappedId { get; set; }
    }
    public class RentalLengthMasterDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long MappedID { get; set; }
        public Nullable<bool> IsSearchEnable { get; set; }
    }
}
