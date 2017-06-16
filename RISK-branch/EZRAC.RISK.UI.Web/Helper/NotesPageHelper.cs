using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class NotesPageHelper<T> where T : class
    {
        public T NotesViewModel { get; set; }

        public int NumberOfPages { get; set; }
        public Nullable<int> CurrentPage { get; set; }
        public int TotalRecordCount { get; set; }
        public int RecordsToShow { get; set; }
        public string SortBy { get; set; }
        public bool SortOrder { get; set; }
        public string PageInfo { get; set; }
    }
}