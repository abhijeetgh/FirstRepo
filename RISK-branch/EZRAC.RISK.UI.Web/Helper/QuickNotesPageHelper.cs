using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Helper
{
    public class QuickNotesPageHelper<T> where T: class
    {
        public IEnumerable<T> Data { get; set; }
        public IEnumerable<SelectListItem> NoteTypes { get; set; }
        public bool NotesPresent { get; set; }
        public string Result { get; set; }
    }
}