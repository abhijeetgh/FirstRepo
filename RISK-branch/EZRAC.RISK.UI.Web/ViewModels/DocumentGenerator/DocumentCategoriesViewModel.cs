using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.DocumentGenerator
{
    public class DocumentCategoriesViewModel
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public IEnumerable<DocumentTypeViewModel> DocumentTypes { get; set; }
    }
}