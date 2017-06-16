using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class LocationCompany:BaseEntity
    {
       // public long ID { get; set; }
        public long LocationID { get; set; }
        public long CompanyID { get; set; }
        public bool IsTerminalInside { get; set; }
        public virtual Company Company { get; set; }
        public virtual Location Location { get; set; }
    }
}
