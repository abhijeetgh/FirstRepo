using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class PageInfo
    {
        public int PageNumber { get; set; }
        public string SortBy { get; set; }
        public bool SortOrder { get; set; }
        public string RecordsToDisplay { get; set; }
    }
}