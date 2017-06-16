using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class LossTypeMasterViewModel
    {
        public IEnumerable<LossTypeViewModel> LossTypes { get; set; }
        public LossTypeViewModel LossTypeViewModel { get; set; }
    }
}