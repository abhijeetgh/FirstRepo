using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class WriteOffTypeMasterViewModel
    {
        public IEnumerable<WriteOffTypeViewModel> WriteOffTypes { get; set; }
        public WriteOffTypeViewModel WriteOffTypeViewModel { get; set; }
    }
}