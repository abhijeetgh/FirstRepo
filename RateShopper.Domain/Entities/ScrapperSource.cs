using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RateShopper.Domain.Entities
{
    public partial class ScrapperSource:BaseEntity
    {
        public ScrapperSource()
        {
            this.SearchResults = new List<SearchResult>();
            this.UserScrapperSources = new List<UserScrapperSources>();
        }

        //public long ID { get; set; }
        [Required,MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(200)]
        public string Description { get; set; }
        [Required, MaxLength(30)]
        public string Code { get; set; }
        [Required]
        public long ProviderId { get; set; }
        public bool? IsGov { get; set; }
        public virtual ICollection<SearchResult> SearchResults { get; set; }
        public virtual ICollection<UserScrapperSources> UserScrapperSources { get; set; }
    }
}
