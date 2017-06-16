using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class NotesService : INotesService
    {
        IGenericRepository<RiskNote> _notesRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskNoteTypes> _riskNoteTypeRepository = null;

        public NotesService(IGenericRepository<Claim> claimRepository, IGenericRepository<RiskNote> notesRepository, IGenericRepository<RiskNoteTypes> riskNoteTypeRepository)
        {
            _claimRepository = claimRepository;
            _notesRepository = notesRepository;
            _riskNoteTypeRepository = riskNoteTypeRepository;
        }


        /// <summary>
        /// This method is used to get Notes for a Particular Claim
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="getNotesType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NotesDto>> GetNotesAsync(long claimNumber, ClaimsConstant.GetNotesType notesType)
        {
            var notes = await _notesRepository.AsQueryable.Where(x => x.ClaimId == claimNumber).ToListAsync();
            IEnumerable<NotesDto> notesDtoList = await MapNotesDto(notes, notesType);
            return notesDtoList;
        }

        /// <summary>
        /// This method is used to Add Notes for particular Claim
        /// </summary>
        /// <param name="noteDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> AddQuickNotes(NotesDto noteDto)
        {
            bool success = false;

            var claim = await _claimRepository.GetByIdAsync(noteDto.ClaimId);

            var noteTypeDescription = await GetNoteTypeFromNoteId(noteDto.NoteTypeId);

            if (noteDto != null && claim != null)
            {
                await _notesRepository.InsertAsync(
                    new RiskNote
                    {
                        ClaimId = noteDto.ClaimId,
                        Date = DateTime.Now,
                        Description = noteDto.Description,
                        NoteTypeId = noteDto.NoteTypeId,
                        UpdatedBy = noteDto.UpdatedBy,
                        IsPrivilege = noteDto.IsPrivilege,
                        NoteTypeDescription = noteTypeDescription
                    });

                success = true;
            }
            return success;
        }

        /// <summary>
        /// This method is used to Search Notes by Criteria
        /// </summary>
        /// <param name="notesCriteria"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NotesDto>> SearchNotes(NotesSearchCriteria notesCriteria)
        {

            IEnumerable<NotesDto> noteDtoList = null;

            IEnumerable<RiskNote> notesList = null;

            notesList = await GetNotes(notesCriteria);

            if (!notesList.Any() && notesCriteria.PageCount > 1)
            {
                notesCriteria.PageCount = notesCriteria.PageCount - 1;
                notesList = await GetNotes(notesCriteria);
            }

            noteDtoList = MapNotesList(notesList);

            return noteDtoList;

        }

        private async Task<IEnumerable<RiskNote>> GetNotes(NotesSearchCriteria notesCriteria)
        {
            IEnumerable<RiskNote> notesList = null;
            if (string.IsNullOrWhiteSpace(notesCriteria.SearchText))
            {
                notesList = await _notesRepository.AsQueryable.Where(x => x.ClaimId == notesCriteria.ClaimId)
                                                                      .OrderUsingSortExpression(notesCriteria.SortType, notesCriteria.SortOrder).Skip(notesCriteria.PageSize * (notesCriteria.PageCount - 1))
                                                                      .Take(notesCriteria.PageSize).ToListAsync();
            }
            else
            {
                notesList = await _notesRepository.AsQueryable.Where(x => x.ClaimId == notesCriteria.ClaimId &&
                                                                    (x.Description.Contains(notesCriteria.SearchText) ||
                                                                        x.NoteTypeDescription.Contains(notesCriteria.SearchText) ||
                                                                        x.UpdatedBy.Contains(notesCriteria.SearchText) ||
                                                                        x.Id.ToString().Contains(notesCriteria.SearchText.ToString())))
                    .OrderUsingSortExpression(notesCriteria.SortType, notesCriteria.SortOrder).Skip(notesCriteria.PageSize * (notesCriteria.PageCount - 1))
                                                                          .Take(notesCriteria.PageSize).ToListAsync();

            }
            return notesList;
        }

        /// <summary>
        /// This method is used to calculate number of notes for a particular claim.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns></returns>
        public async Task<int> GetNotesCountList(long claimNumber)
        {
            var notesCount = await _notesRepository.AsQueryable.Where(x => x.ClaimId == claimNumber).CountAsync();
            return notesCount;
        }

        /// <summary>
        /// This method is used to delete a particular notes
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNoteById(long noteId)
        {
            var sucess = false;

            if (noteId != null)
            {
                var notes = await _notesRepository.GetByIdAsync(noteId);
                if (notes != null)
                {
                    await _notesRepository.DeleteAsync(notes);
                    sucess = true;
                }
            }
            return sucess;
        }

        /// <summary>
        /// This method is used to get count of filtered Notes
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<int> GetSearchedNotesCount(long claimNumber, string searchText)
        {
            var count = await _notesRepository.AsQueryable.Where(x => x.ClaimId == claimNumber && (x.Description.Contains(searchText) || x.NoteTypeDescription.Contains(searchText) || x.UpdatedBy.Contains(searchText))).CountAsync();
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetNoteTypeFromNoteId(int id)
        {
            var notesDescription = string.Empty;

            var riskNoteType = await _riskNoteTypeRepository.GetByIdAsync(id);
            if (riskNoteType != null)
            {
                notesDescription = riskNoteType.Description;
            }
            return notesDescription;
        }
        #region Private Methods
        private async Task<IList<NotesDto>> MapNotesDto(IEnumerable<RiskNote> notes, ClaimsConstant.GetNotesType notesType)
        {
            var notesDtoList = notes.Select(
               x => new NotesDto
               {
                   ClaimId = x.ClaimId,
                   Description = x.Description,
                   NoteTypeId = x.NoteTypeId,
                   UpdatedBy = x.UpdatedBy,
                   Date = x.Date,
                   NoteTypeDescription = x.NoteTypeDescription,
                   NoteId = x.Id

               });
            if (notesType.Equals(ClaimsConstant.GetNotesType.QuickNotes))
                return notesDtoList.OrderByDescending(x => x.Date).Take(3).ToList();

            return notesDtoList.OrderBy(x => x.Date).ToList();
        }

        private IEnumerable<NotesDto> MapNotesList(IEnumerable<RiskNote> NotesList)
        {
            var claimDtoList = NotesList.Select(
                 x => new NotesDto
                 {
                     ClaimId = x.ClaimId,
                     Description = x.Description,
                     NoteTypeId = x.NoteTypeId,
                     UpdatedBy = x.UpdatedBy,
                     Date = x.Date,
                     NoteTypeDescription = x.NoteTypeDescription,
                     IsPrivilege = x.IsPrivilege,
                     NoteId = x.Id
                 });
            return claimDtoList.ToList();
        }

        #endregion

        public async Task<IEnumerable<NotesDto>> GetNotesByNoteIds(int[] ids)
        {
            IEnumerable<NotesDto> notesDtoList = null;
            if (ids != null && ids.Any())
            {
                var predicate = PredicateBuilder.False<RiskNote>();

                foreach (var item in ids)
                    predicate = predicate.Or(y => y.Id == item);

                var notes = await _notesRepository.AsQueryable.Where(predicate).ToListAsync();
                notesDtoList = await MapNotesDto(notes, ClaimsConstant.GetNotesType.AllNotes);
            }

            return notesDtoList;
        }

        public async Task<bool> IsClaimAssignedToCurrentUser(long claimId, long user)
        {
            var claim = await _claimRepository.AsQueryable.Where(x => x.Id == claimId).FirstOrDefaultAsync();
            var assignToUser = false;
            if (claim.AssignedTo == user)
            {
                assignToUser = true;
            }
            return assignToUser;
        }
    }
}
