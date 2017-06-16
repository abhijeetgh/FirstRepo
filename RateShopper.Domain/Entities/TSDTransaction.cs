using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class TSDTransaction:BaseAuditableEntity
    {
        //public long ID { get; set; }
        public long LocationBrandID { get; set; }
        public long SearchSummaryID { get; set; }
        [MaxLength(500)]
        public string ResponseCode { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        [Required]
        public string XMLRequest { get; set; }
        public string RequestStatus { get; set; }
        public string XMLResponse { get; set; }
        public string ErrorFound { get; set; }
        public bool? IsRezCentralUpdate { get; set; }
        public bool? IsOpaqueUpdate { get; set; }
        //Below Properties inherited from BaseAuditableEntity
        //public long CreatedBy { get; set; }
        //public long UpdatedBy { get; set; }
        //public System.DateTime CreatedDateTime { get; set; }
        //public System.DateTime UpdatedDateTime { get; set; }
        public virtual LocationBrand LocationBrand { get; set; }
        public virtual SearchSummary SearchSummary { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
