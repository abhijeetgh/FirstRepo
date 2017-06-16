using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class NotesDto
    {
        public string Description { get; set; }
        public int NoteTypeId { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Int64 ClaimId { get; set; }
        public string NoteTypeDescription { get; set; }
        public long NoteId { get; set; }
        public bool IsPrivilege { get; set; }

        public IEnumerable<NotesTypeDto> NoteTypes { get; set; }

    }
}
