using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class PicturesAndFilesViewModel
    {
        //Need to have property for files
        public IEnumerable<SelectListItem> CategoryType { get; set; }
        public string Desc { get; set; }
    }
}