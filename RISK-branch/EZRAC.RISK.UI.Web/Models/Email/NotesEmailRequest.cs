using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Models.Email
{
    public class NotesEmailRequest
    {
        public long ClaimId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NoteType { get; set; }
        public string NoteDescription { get; set; }
        public string NoteAddedBy { get; set; }
        public long RecipientId { get; set; }
        public string RecipientEmailId { get; set; }

    }
}