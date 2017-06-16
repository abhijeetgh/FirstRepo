using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EZRAC.RISK.Services.Contracts;
using System.Threading.Tasks;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Models.Email;
using EZRAC.RISK.Util.Common;
using EZRAC.Risk.UI.Web.Attributes;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        #region  Private Members

        private IClaimService _claimService;
        private INotesService _notesService;
        private ILookUpService _lookUpService;

        #endregion


        public NotesController(IClaimService claimService, INotesService notesService, ILookUpService lookupService)
        {
            _claimService = claimService;
            _notesService = notesService;
            _lookUpService = lookupService;
        }

        public async Task<PartialViewResult> GetNotesTable(int claimNumber, int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {

            NotesPageHelper<NotesViewModel> noteviewModel = new NotesPageHelper<NotesViewModel>();
            var user = SecurityHelper.GetUserIdFromContext();
            var isAssignedToSelf = await _notesService.IsClaimAssignedToCurrentUser(claimNumber, user);

            noteviewModel = await GetNotesPagingInfo(page, sortBy, true, recordCountToDisplay, claimNumber, false, string.Empty);

            noteviewModel.NotesViewModel.NotesType = LookUpHelpers.GetAllNotesListItem();

            noteviewModel.NotesViewModel.ClaimId = claimNumber;
            noteviewModel.NotesViewModel.IsNotesTab = true;
            noteviewModel.NotesViewModel.NoteAssignedToLoggedInUser = isAssignedToSelf;
            return PartialView("_NotesTab", noteviewModel);
        }

        public async Task<PartialViewResult> GetCurrentNotesGridData(int claimNumber, int page, string sortBy, bool SortOrder, int recordCountToDisplay, string searchText)
        {
            NotesPageHelper<NotesViewModel> noteviewModel = new NotesPageHelper<NotesViewModel>();


            var isSearchtext = true;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                isSearchtext = false;
            }

            noteviewModel = await GetNotesPagingInfo(page, sortBy, SortOrder, recordCountToDisplay, claimNumber, isSearchtext, searchText);

            return PartialView("_NotesTable", noteviewModel);
        }

        [HttpGet]

        public async Task<PartialViewResult> GetQuickNotes(int claimNumber)
        {
            var notesViewModel = new NotesViewModel();

            var notes = await _notesService.GetNotesAsync(claimNumber, ClaimsConstant.GetNotesType.QuickNotes);

            var user = SecurityHelper.GetUserIdFromContext();
            var isAssignedToSelf = await _notesService.IsClaimAssignedToCurrentUser(claimNumber, user);

            if (notes != null)
            {
                var notesModel = ClaimHelper.GetNotesViewModel(notes);
                notesViewModel.Notes = notesModel.ToList();
                notesViewModel.NoteAssignedToLoggedInUser = isAssignedToSelf;
            }
            notesViewModel.NotesType = LookUpHelpers.GetAllNotesListItem();
            return PartialView("_QuickNotes", notesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.AddQuickNote)]
        public async Task<ActionResult> AddQuickNotes(NotesViewModel noteviewModel)
        {
            var success = false;
            var userName = SecurityHelper.GetUserNameFromContext();

            NotesDto noteDto = new NotesDto()
            {
                ClaimId = noteviewModel.ClaimId,
                Description = noteviewModel.Description,
                NoteTypeId = noteviewModel.SelectedNoteTypeId,
                IsPrivilege = noteviewModel.IsPrivilege,
                UpdatedBy = userName
            };

            if (!string.IsNullOrWhiteSpace(noteviewModel.Description))
            {
                success = await _notesService.AddQuickNotes(noteDto);
            }

            if (noteviewModel.SendNotification)
            {
                var claim = await _claimService.GetClaimInfoByClaimIdAsync(noteviewModel.ClaimId);
                NotesEmailRequest emailRequest = new NotesEmailRequest
                {
                    ClaimId = noteviewModel.ClaimId,
                    NoteAddedBy = userName,
                    NoteDescription = noteviewModel.Description,
                    NoteType = await _notesService.GetNoteTypeFromNoteId(noteviewModel.SelectedNoteTypeId),
                    RecipientId = claim.SelectedAssignedUserId
                };

                await SendNotificationEmailForQuickNotes(emailRequest);
            }

            if (noteviewModel.IsNotesTab)
            {
                return RedirectToAction("GetCurrentNotesGridData", new
                {
                    claimNumber = noteviewModel.ClaimId,
                    page = Constants.PageInfoConstants.PageNumber,
                    sortBy = Constants.PageInfoConstants.SortByDate,
                    sortOrder = Constants.PageInfoConstants.SortOrder,
                    recordCountToDisplay = Constants.PageInfoConstants.DefaultRecordToShow
                });
            }
            return Json(success, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteNotes)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteNote)]
        public async Task<ActionResult> DeleteNoteById(long claimId, long noteId, int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {
            await _notesService.DeleteNoteById(noteId);

            return RedirectToAction("GetCurrentNotesGridData", new { claimNumber = claimId, page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay });
        }

        public async Task<ActionResult> SearchNotesByText(int claimNumber, string text)
        {
            var isSearchNotes = true;
            if (string.IsNullOrWhiteSpace(text))
            {
                isSearchNotes = false;
            }
            var pageHelper = await GetNotesPagingInfo(1, "Date", true, 5, claimNumber, isSearchNotes, text);
            return PartialView("_NotesTable", pageHelper);
        }

        private async Task SendNotificationEmailForQuickNotes(NotesEmailRequest request)
        {
            UserDto assignedUser = await _lookUpService.GetUserbyIdAsync(request.RecipientId);
            request.FirstName = assignedUser.FirstName;
            request.LastName = assignedUser.LastName;
            request.RecipientEmailId = assignedUser.Email;


            //Creating Email Request
            EmailRequest emailTemplate = EmailTemplatesHelper.CreateEmailForNewClaimNotes(request);

            bool emailStatus;
            if (emailTemplate != null)
            {
                //Sending Email
                emailStatus = await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(emailTemplate));
            }

        }

        private async Task<NotesPageHelper<NotesViewModel>> GetNotesPagingInfo(int page, string sortBy, bool sortOrder, int recordCountToDisplay, int claimId, bool isSearch, string text)
        {
            NotesPageHelper<NotesViewModel> pageHelp = new NotesPageHelper<NotesViewModel>();

            if (!isSearch)
            {
                pageHelp.TotalRecordCount = await _notesService.GetNotesCountList(claimId);
            }
            else
            {
                pageHelp.TotalRecordCount = await _notesService.GetSearchedNotesCount(claimId, text);
            }


            pageHelp.SortOrder = sortOrder ? false : true;

            recordCountToDisplay = recordCountToDisplay > 0 ? recordCountToDisplay : pageHelp.TotalRecordCount;

            double pages = (double)pageHelp.TotalRecordCount / recordCountToDisplay;

            pageHelp.CurrentPage = recordCountToDisplay > 0 ? page : 1;
            pageHelp.NumberOfPages = recordCountToDisplay > 0 ? (int)Math.Ceiling(pages) : 1;


            //int startPageIndex = (int)((page - 1) * recordCountToDisplay) + 1;

            //int endPageIndex = ((startPageIndex - 1) + recordCountToDisplay) > pageHelp.TotalRecordCount ? pageHelp.TotalRecordCount : ((startPageIndex - 1) + recordCountToDisplay);

            //if (pageHelp.TotalRecordCount > 0)
            //    pageHelp.PageInfo = string.Format("{0} - {1} of {2} items", startPageIndex, endPageIndex, pageHelp.TotalRecordCount);
            //else
            //    pageHelp.PageInfo = Constants.StringConstants.NoNotesFound;

            NotesSearchCriteria noteCriteria = new NotesSearchCriteria
            {

                PageCount = page,
                PageSize = recordCountToDisplay,
                SortOrder = sortOrder,
                SortType = sortBy,
                ClaimId = claimId,
                SearchText = text
            };

            pageHelp.NotesViewModel = new NotesViewModel();
            IEnumerable<NotesDto> notesList = null;

            notesList = await _notesService.SearchNotes(noteCriteria);
            var notesViewModel = ClaimHelper.GetNotesViewModel(notesList);
            pageHelp.NotesViewModel.Notes = notesViewModel.ToList();
            pageHelp.NotesViewModel.ClaimId = claimId;
            pageHelp.RecordsToShow = recordCountToDisplay > 0 ? recordCountToDisplay : 1;
            pageHelp.SortBy = sortBy;

            int startPageIndex = (int)((noteCriteria.PageCount - 1) * recordCountToDisplay) + 1;

            int endPageIndex = ((startPageIndex - 1) + recordCountToDisplay) > pageHelp.TotalRecordCount ? pageHelp.TotalRecordCount : ((startPageIndex - 1) + recordCountToDisplay);

            if (pageHelp.TotalRecordCount > 0)
                pageHelp.PageInfo = string.Format("{0} - {1} of {2} items", startPageIndex, endPageIndex, pageHelp.TotalRecordCount);
            else
                pageHelp.PageInfo = Constants.StringConstants.NoNotesFound;

            pageHelp.CurrentPage = noteCriteria.PageCount;

            return pageHelp;
        }

        public PartialViewResult RejectClaimNote()
        {
            return PartialView("_RejectNotes");
        }
        public PartialViewResult ApproveClaimNote()
        {
            return PartialView("_ApproveNote");
        }

        public PartialViewResult AddFollowupNotesClaimList()
        {
            return PartialView("_AddFollowupNotesClaimList");
        }

        public PartialViewResult AddFollowupNotesViewClaim()
        {
            return PartialView("_AddFollowupNotesViewClaim");
        }

        public PartialViewResult RejectClaimNoteForViewClaim()
        {
            return PartialView("_RejectNotesForViewClaim");
        }
        public PartialViewResult ApprovalClaimNoteForViewClaim()
        {
            return PartialView("_ApprovalNotesForViewClaim");
        }
    }
}