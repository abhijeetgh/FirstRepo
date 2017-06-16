using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{
    public partial class Statuses : BaseEntity
    {
        //inherited from BaseAuditableEntity
        public Statuses()
        {
            //this.SearchSummary4 = new List<SearchSummary>();
        }
        [Required, MaxLength(50)]
        public string Status { get; set; }
        //public virtual ICollection<SearchSummary> SearchSummary4 { get; set; }
    }
}
