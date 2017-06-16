using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Claims
{
    public class RiskFileModel
    {
        public string CategoryName { get; set; }

        public IEnumerable<FileModel> FileList { get; set; }

        public int CategoryId { get; set; }
    }

    public class FileModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }        
    }
}