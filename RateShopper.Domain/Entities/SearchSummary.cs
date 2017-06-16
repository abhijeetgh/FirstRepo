using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class SearchSummary : BaseAuditableEntity
    {
        public SearchSummary()
        {
            this.SearchResultProcessedDatas = new List<SearchResultProcessedData>();
            this.SearchResultRawDatas = new List<SearchResultRawData>();
            this.SearchResults = new List<SearchResult>();
            this.SearchResultSuggestedRates = new List<SearchResultSuggestedRate>();
            this.TSDTransactions = new List<TSDTransaction>();
        }

        [Required, MaxLength(500)]
        public string ScrapperSourceIDs { get; set; }

        [Required, MaxLength(500)]
        public string LocationBrandIDs { get; set; }

        [Required, MaxLength(500)]
        public string CarClassesIDs { get; set; }

        [Required, MaxLength(500)]
        public string RentalLengthIDs { get; set; }
        public Nullable<long> ScheduledJobID { get; set; }

        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        [Required]
        public int RetryCount { get; set; }
        public string Response { get; set; }
        [Required]
        public string RequestURL { get; set; }

        public long StatusID { get; set; }

        public Nullable<bool> IsReviewed { get; set; }
        public Nullable<bool> IsQuickView { get; set; }
        public Nullable<long> ProviderId { get; set; }
        public Nullable<long> ShopRequestId { get; set; }
        public Nullable<bool> HasQuickViewChild { get; set; }
        public string PostData { get; set; }

        public bool? IsGov { get; set; }

        [MaxLength(200)]
        public string ShopType { get; set; }

        public Nullable<long> FTBScheduledJobID { get; set; }
        //public virtual Statuses Status { get; set; }
        public virtual ICollection<SearchResultProcessedData> SearchResultProcessedDatas { get; set; }
        public virtual ICollection<SearchResultRawData> SearchResultRawDatas { get; set; }
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<SearchResultSuggestedRate> SearchResultSuggestedRates { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<TSDTransaction> TSDTransactions { get; set; }
    }
}
