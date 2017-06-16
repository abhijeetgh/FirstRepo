using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class NotesSearchCriteria
    {
        public int ClaimId { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public string SortType { get; set; }

        public bool SortOrder { get; set; }

        public string  SearchText { get; set; }
    }
}
