using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class SearchResultRawData : BaseEntity
    {
        //public long ID { get; set; }
        public long SearchSummaryID { get; set; }
        
        [Required]
        public string JSON { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public virtual SearchSummary SearchSummary { get; set; }
    }
}
