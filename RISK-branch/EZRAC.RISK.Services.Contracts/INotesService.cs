using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts
{
    public interface INotesService
    {
        Task<bool> AddQuickNotes(NotesDto noteDto);

        Task<IEnumerable<NotesDto>> GetNotesAsync(long claimNumber, ClaimsConstant.GetNotesType notesType);

        Task<int> GetNotesCountList(long claimNumber);

        Task<IEnumerable<NotesDto>> SearchNotes(NotesSearchCriteria notesCriteria);        

        Task<bool> DeleteNoteById(long noteId);

        Task<int> GetSearchedNotesCount(long claimNumber, string searchText);

        Task<string> GetNoteTypeFromNoteId(int id);

        Task<IEnumerable<NotesDto>> GetNotesByNoteIds(int[] ids);

        Task<bool> IsClaimAssignedToCurrentUser(long claimId, long user);
    }
}
