using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class DamageInfoHelper<T> where T : class 
    {
        public IEnumerable<T> Data { get; set; }
        public List<SelectListItem> Section { get; set; }
    }
}