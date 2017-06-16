namespace EZRAC.RISK.Domain
{
    using System;
    using System.Collections.Generic;

    public class Company 
    {
        public int Id { get; set; }
        public string Abbr { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Zurich { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }

        public IList<Location> Locations { get; set; }
    }
}
