using System;
using System.Collections.Generic;

namespace RateShopper.Domain.Entities
{
    public partial class UserScrapperSources:BaseEntity
    {
        //public long ID { get; set; }
        public long UserID { get; set; }
        public long ScrapperSourceID { get; set; }
        public virtual ScrapperSource ScrapperSource { get; set; }
        public virtual User User { get; set; }
    }
}
