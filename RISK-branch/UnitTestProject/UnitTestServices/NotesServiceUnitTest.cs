using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.Risk.Services.Test.Helpers;
using EZRAC.RISK.Services.Implementation;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using EZRAC.RISK.Util.Common;

namespace UnitTestProject.UnitTestServices
{
    [TestClass]
    public class NotesServiceUnitTest
    {
        #region Private variables
        IGenericRepository<RiskNote> _mockNotesRepository = null;
        IGenericRepository<Claim> _mockClaimRepository = null;
        IGenericRepository<RiskNoteTypes> _mockRiskNoteTypeRepository = null;
        NotesService _notesService = null;

        #endregion

        [TestInitialize]
        public void Setup()
        {
            _mockClaimRepository = new MockGenericRepository<Claim>(DomainBuilder.GetClaims()).SetUpRepository();
            _mockNotesRepository = new MockGenericRepository<RiskNote>(DomainBuilder.GetRiskNotes()).SetUpRepository();
            _mockRiskNoteTypeRepository = new MockGenericRepository<RiskNoteTypes>(DomainBuilder.GetRiskNotesTypes()).SetUpRepository();

            _notesService = new NotesService(_mockClaimRepository, _mockNotesRepository, _mockRiskNoteTypeRepository);
        }

        [TestMethod]
         public void Test_method_for_get_all_notes()
        {
            long claimNumber = _mockNotesRepository.AsQueryable.Select(x => x.ClaimId).Max();

            var notesdtolist =  _notesService.GetNotesAsync(claimNumber, ClaimsConstant.GetNotesType.AllNotes).Result;

            Assert.IsTrue(notesdtolist != null && notesdtolist.Count() > 0);

        }

        //second input
        [TestMethod]
        public void Test_method_for_get_quick_notes()
        {
            long claimNumber = _mockNotesRepository.AsQueryable.Select(x => x.ClaimId).Max();

            var notesdtolist =  _notesService.GetNotesAsync(claimNumber, ClaimsConstant.GetNotesType.QuickNotes).Result;

            Assert.IsTrue(notesdtolist != null && notesdtolist.Count()>0);

        }

      
 
 

        [TestMethod]
        public void Test_method_for_add_quick_notes()
        {
            
            
            var notes =  _notesService.AddQuickNotes(DomainBuilder.Get<NotesDto>()).Result;

            Assert.IsTrue(notes);
        }


        [TestMethod]
        public void Test_method_for_search_notes()
        {
            NotesSearchCriteria searchCriteria = DtoBuilder.GetNotesSearchCriteria();

            var note =  _mockNotesRepository.GetByIdAsync(1).Result;

            searchCriteria.ClaimId = (int)note.ClaimId;

            searchCriteria.SearchText = note.Description;

           var notes =  _notesService.SearchNotes(searchCriteria).Result;

           Assert.IsTrue(notes != null && notes.Count<NotesDto>() > 0);

           notes = null;

           searchCriteria.SearchText = note.ClaimId.ToString();

           notes =  _notesService.SearchNotes(searchCriteria).Result;

           Assert.IsTrue(notes != null && notes.Count<NotesDto>() > 0);

           notes = null;

           searchCriteria.SearchText = note.NoteTypeDescription;

           notes =  _notesService.SearchNotes(searchCriteria).Result;

           Assert.IsTrue(notes != null && notes.Count<NotesDto>() > 0);

           notes = null;

           searchCriteria.SearchText = note.UpdatedBy;

           notes =  _notesService.SearchNotes(searchCriteria).Result;

           Assert.IsTrue(notes != null && notes.Count<NotesDto>() > 0);



        }
        
       
        [TestMethod]
        public void Test_method_for_delete_note_by_id()
        {
            long noteId = _mockNotesRepository.AsQueryable.Select(x => x.Id).Max();

            var delnote =   _notesService.DeleteNoteById(noteId).Result;

            Assert.IsTrue(delnote);
        }

        [TestMethod]
        public void Test_method_for_get_note_type_from_note_id()
        {
            int id = _mockNotesRepository.AsQueryable.Select(x => x.NoteTypeId).Max();

            var notetype =  _notesService.GetNoteTypeFromNoteId(id).Result;

            Assert.IsNotNull(notetype);

        }

        [TestMethod]
        public void Test_method_for_get_notes_by_note_ids()
        {
            int[] getnotes = _mockNotesRepository.AsQueryable.Select(x => x.NoteTypeId).ToArray();

            var notesdtolist =   _notesService.GetNotesByNoteIds(getnotes).Result;

            Assert.IsNotNull(notesdtolist);
        }

        [TestMethod]
        public void Test_method_for_get_notes_count_list()
        {
            long claimNumber = _mockNotesRepository.AsQueryable.Select(x => x.ClaimId).Max();

            var notesCount =  _mockNotesRepository.AsQueryable.Where(x => x.ClaimId == claimNumber).Count();

            var notescount =  _notesService.GetNotesCountList(claimNumber).Result;

            Assert.AreEqual(notescount, notesCount);
            
        }


        [TestMethod]
        public void Test_method_for_get_searched_notes_count()
        {
            long claimNumber = _mockNotesRepository.AsQueryable.Select(x => x.ClaimId).FirstOrDefault();

            string searchtext = _mockNotesRepository.AsQueryable.Select(x => x.Description).FirstOrDefault();

            var count =  _mockNotesRepository.AsQueryable.Where(x => x.ClaimId == claimNumber && (x.Description.Contains(searchtext) || x.NoteTypeDescription.Contains(searchtext) || x.UpdatedBy.Contains(searchtext))).Count();

            var searchednotescount =  _notesService.GetSearchedNotesCount(claimNumber, searchtext).Result;

            Assert.AreEqual(count, searchednotescount);

        }

     
        [TestMethod]
        public void Test_method_for_check_is_notes_assigned_to_current_user()
        {
           


            long claimId = _mockNotesRepository.AsQueryable.Select(x => x.ClaimId).Max();

            long user = _mockClaimRepository.AsQueryable.Select(x => x.AssignedTo).Max();

            var notes =  _notesService.IsClaimAssignedToCurrentUser(claimId, user).Result;

            Assert.IsTrue(notes);
        }
    }
}
