using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Reports
{
    [Serializable]
    public class SearchTagPlateViewModel
    {
        public string UnitNumber { get; set; }
        public string Messages { get; set; }
        public Nullable<DateTime> TransDate { get; set; }
    }
}