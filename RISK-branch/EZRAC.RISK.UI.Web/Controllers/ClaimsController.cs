using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using EZRAC.Core.Email;
using EZRAC.Risk.UI.Web.Models;
using EZRAC.RISK.Util.Common;
using EZRAC.Risk.UI.Web.Attributes;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        #region  Private Members

        private IClaimService _claimService;
        private ILookUpService _lookUpService;
        private ITSDService _tsdService;
        private IUserService _userService;

        #endregion

        /// <summary>
        /// public Claims controller Constructor
        /// </summary>
        #region Constructors
        public ClaimsController(IClaimService claimService, ILookUpService lookupService, ITSDService tsdService, IUserService userService)
        {

            _claimService = claimService;
            _lookUpService = lookupService;
            _tsdService = tsdService;
            _userService = userService;

        }
        #endregion


        //
        // GET: /Claims/
        public ActionResult Index()
        {
            return View();
        }

        [CRMSAuthroize(ClaimType = ClaimsConstant.NewClaim)]
        [HttpGet]
        public async Task<ActionResult> CreateClaim()
        {
            CreateClaimViewModel objModel = ClaimHelper.GetCreateClaimViewModel();
            return View(objModel);
        }

        [CRMSAuthroize(ClaimType = ClaimsConstant.NewClaim)]
        [HttpGet]
        public async Task<ActionResult> FetchContractInfo(string contractNumber)
        {
            FetchDetailsViewModel model = null;
            FetchedContractDetailsDto fetchedContractDetails = null;
            contractNumber = contractNumber.Trim();

            if (!String.IsNullOrEmpty(contractNumber))
            {

                if (_tsdService.IsContractNumberValid(contractNumber))
                    fetchedContractDetails = await _tsdService.GetContractInfoFromTSDAsync(contractNumber);

                if (fetchedContractDetails != null)
                {
                    IEnumerable<ClaimDto> claimList = await _lookUpService.GetClaimsByContractNumberOrUnitNumber(contractNumber.Trim(), fetchedContractDetails.UnitNo);
                    LocationDto location = await _lookUpService.GetLocationByCodeAsync(fetchedContractDetails.LocationCode);

                    fetchedContractDetails.LocationId = location != null ? location.Id : default(long);

                    model = ClaimHelper.GetContractInfoFromTSDViewModel(fetchedContractDetails, claimList);
                }
                else
                    return Content(Constants.FetchContract.ContractNotFound);

                return PartialView("_FetchedDetails", model);
            }
            else
            {
                return Content(Constants.FetchContract.InvalidContractNumber);
            }

        }

        [HttpPost]
        [CRMSAuthroize(ClaimType = ClaimsConstant.NewClaim)]
        [UserActionLog(UserAction = UserActionLogConstant.CreateClaim)]
        public async Task<ActionResult> CreateClaim(CreateClaimViewModel createClaimViewModel)
        {
            if (!ModelState.IsValid)
            {

                createClaimViewModel.Users = LookUpHelpers.GetAssignedToUsersListItem();
                createClaimViewModel.Locations = LookUpHelpers.GetLocationListItem();
                createClaimViewModel.LossTypes = LookUpHelpers.GetLossTypeListItems(); 

                return View(createClaimViewModel);

            }
            ClaimDto claimToCreate = ClaimHelper.GetClaimDto(createClaimViewModel);

            Nullable<Int64> claimId = await _claimService.CreateClaimAsync(claimToCreate,SecurityHelper.GetUserIdFromContext());

            List<string> userEmailList = await _userService.GetUserEmailIdByLocationAsync(createClaimViewModel.SelectedLocation);
            if (claimId != null)
               await SendNewClaimCreatedEmail(createClaimViewModel.SelectedAssignedUser, (Int64)claimId, userEmailList);

            return RedirectToAction("ViewClaim", new { id = claimId });
        }

        
        [UserActionLog(UserAction = UserActionLogConstant.ViewClaim)]
        [HttpGet]
        public async Task<ActionResult> ViewClaim(Int64 id)
        {
            IEnumerable<UserDto> users = await _lookUpService.GetAllUsersAsync();

            ClaimInfoHeaderViewModel model = null;

            //IEnumerable<ClaimStatusDto> claimStatuses = await _lookUpService.GetAllClaimStatusesAsync();
            ClaimDto claimInfo = await _claimService.GetClaimInfoForHeaderAsync(id);

            if (claimInfo != null)
            {
                Session["ClaimId"] = id;
                IEnumerable<UserDto> approvalUsers = await _lookUpService.GetUsersByRolesAsync(GetApproverRoles());
                model = ClaimHelper.GetClaimInfoHeaderViewModel(claimInfo, users, approvalUsers);                
                return View(model);
            }

            return View("ClaimNotFound");

        }


        public async Task<ActionResult> GetClaimList()
        {
            ClaimInfoViewModel model = new ClaimInfoViewModel();

            var pageHelp = new PageHelper<ClaimInfoViewModel>();

            pageHelp = await GetClaimAndPagingInfo(Constants.PageInfoConstants.PageNumber, Constants.PageInfoConstants.SortByColumn, Constants.PageInfoConstants.SortOrderTrue, Constants.PageInfoConstants.DefaultRecordToShow, pageHelp, ClaimType.FollowupClaim.ToString(), null);

            model.ClaimInfo = pageHelp;

            Dictionary<string, string> temp = ClaimsConstant.AdvanceSearchCriteria();

            ViewBag.AdvacedSearchCriteria = new SelectList(temp, "key", "value");

            return View("ClaimList", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> GetCurrentGridData(int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {
            var pageHelp = new PageHelper<ClaimInfoViewModel>();
            pageHelp = await GetClaimAndPagingInfo(page, sortBy, sortOrder, recordCountToDisplay, pageHelp, ClaimType.FollowupClaim.ToString(), null);

            return PartialView("_ClaimListGrid", pageHelp);
        }

       
        [HttpGet]
        public async Task<ActionResult> GetPendingClaims(int page, string sortBy, bool sortOrder, int recordCountToDisplay, string claimType)
        {
            PendingClaimPageHelper<ClaimInfoViewModel> pageHelp = new PendingClaimPageHelper<ClaimInfoViewModel>();
            pageHelp = await GetClaimAndPagingInfo(page, sortBy, sortOrder, recordCountToDisplay, pageHelp, claimType.ToString(), null);
            return PartialView("_PendingClaimsList", pageHelp);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.ChangeAssignedTo)]
        public async Task<ActionResult> ChangeClaimAssignedTo(string claimId, string assignedTo, int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {
            await _claimService.SetAssignedTo(Int64.Parse(claimId), Int64.Parse(assignedTo), SecurityHelper.GetUserIdFromContext());

            return RedirectToAction("GetCurrentGridData", new { page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.ChangeFollowUpDate)]
        public async Task<ActionResult> ChangeFollowUpDate(string claimId, string date, int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {
            DateTime FollowUpDate;

            DateTime.TryParse(date, out FollowUpDate);

            await _claimService.SetFollowUpdateByClaimIdAsync(Convert.ToInt64(claimId), FollowUpDate, SecurityHelper.GetUserIdFromContext());

            UserActionLogHelper.AddUserActionLog(UserActionLogConstant.ChangeFollowUpDate, Convert.ToInt64(claimId));

            return RedirectToAction("GetCurrentGridData", new { page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveOrRejectClaim(long claimId, bool approvedOrRejected, int page, string sortBy, bool sortOrder, int recordCountToDisplay)
        {
            await _claimService.ApproveOrRejectClaims(claimId, approvedOrRejected);

            var approvalStatus = approvedOrRejected ? UserActionLogConstant.ClaimApprove : UserActionLogConstant.ClaimReject;

            UserActionLogHelper.AddUserActionLog(approvalStatus, Convert.ToInt64(claimId));

            return RedirectToAction("GetPendingClaims", new { page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay, claimType = ClaimType.PendingApproval.ToString() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteClaim)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteClaim)]
        public async Task<ActionResult> DeleteClaims(List<Int64> claimIds, int? page, string sortBy, bool? sortOrder, int recordCountToDisplay)
        {
            if (claimIds != null && claimIds.Count > 0)
            {
                await _claimService.DeleteClaims(claimIds);
            }

            return RedirectToAction("GetCurrentGridData", new { page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay });
        }

        [HttpPost]
        [UserActionLog(UserAction = UserActionLogConstant.UpdateHeaderSection)]
        public async Task<ActionResult> SaveClaimHeaderInfo(ClaimInfoHeaderViewModel claimInfo)
        {
            if (ModelState.IsValid)
            {
                ClaimDto claimDto = ClaimHelper.GetClaimDtoOfHeader(claimInfo);

               await _claimService.SaveClaimHeaderInfo(claimDto, SecurityHelper.GetUserIdFromContext());

                if (claimInfo.SelectedApprover != null)
                    await SendSentForApprovalEmail((Int64)claimInfo.SelectedApprover, (Int64)claimInfo.ClaimID);

                return RedirectToAction("GetHeaderInfo", new { claimId = claimDto.Id });
            }
            else
            {

                IEnumerable<UserDto> users = await _lookUpService.GetAllUsersAsync();

                IEnumerable<UserDto> approvalUsers = await _lookUpService.GetUsersByRolesAsync(GetApproverRoles());

                claimInfo = ClaimHelper.GetClaimInfoHeaderViewModel(claimInfo, users, approvalUsers);

                return PartialView("_ClaimHeaderInfo", claimInfo);
            }

        }

        public async Task<PartialViewResult> GetHeaderInfo(Int64 claimId)
        {

            IEnumerable<UserDto> users = await _lookUpService.GetAllUsersAsync();

            ClaimDto claimInfo = await _claimService.GetClaimInfoForHeaderAsync(claimId);

            IEnumerable<UserDto> approvalUsers = await _lookUpService.GetUsersByRolesAsync(GetApproverRoles());

            ClaimInfoHeaderViewModel model = ClaimHelper.GetClaimInfoHeaderViewModel(claimInfo, users, approvalUsers);

            return PartialView("_ClaimHeaderInfo", model);
        }

        public async Task<JsonResult> SearchClaim(string id)
        {
            Int64 claimNumber;

            if (Int64.TryParse(id, out claimNumber))
            {
                var result = await _claimService.SearchClaimByClaimIdAsync(claimNumber);

                if (result)
                {
                    return Json("True", JsonRequestBehavior.AllowGet);
                }
            }

            return Json("False", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SearchClaimsByContract(string id)
        {
            var result = await _claimService.SearchClaimsByContractNumberAsync(id);
            if (result.Any())
            {
                var searchClaimsViewModel = ClaimHelper.GetClaimListViewModel(result);
                return PartialView("SearchClaims", searchClaimsViewModel);
            }
            else
            {
                return Json(new { message = "NoData" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetAdvancedSearchRecords(int page, string sortBy, bool sortOrder, int recordCountToDisplay, ClaimSearchDto claimSearchDto)
        {

            PendingClaimPageHelper<ClaimInfoViewModel> pageHelp = new PendingClaimPageHelper<ClaimInfoViewModel>();

            pageHelp = await GetClaimAndPagingInfo(page, sortBy, sortOrder, recordCountToDisplay, pageHelp, ClaimType.AdvancedSearch.ToString(), claimSearchDto);
            return PartialView("_AdvancedSearchClaimList", pageHelp);
        }
        [HttpPost]
         [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteClaim)]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAdvancedSearchClaims(List<Int64> claimIds, int page, string sortBy, bool sortOrder, int recordCountToDisplay, ClaimSearchDto claimSearchDto)
        {
            if (claimIds != null && claimIds.Count > 0)
            {
                await _claimService.DeleteClaims(claimIds);
            }
            PendingClaimPageHelper<ClaimInfoViewModel> pageHelp = new PendingClaimPageHelper<ClaimInfoViewModel>();

            pageHelp = await GetClaimAndPagingInfo(page, sortBy, sortOrder, recordCountToDisplay, pageHelp, ClaimType.AdvancedSearch.ToString(), claimSearchDto);
            return PartialView("_AdvancedSearchClaimList", pageHelp);
            //return RedirectToAction("GetAdvancedSearchRecords", new { page = page, sortBy = sortBy, sortOrder = sortOrder, recordCountToDisplay = recordCountToDisplay, claimSearchDto = claimSearchDto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdvancedSearchChangeClaimAssignedTo(string claimId, string assignedTo)
        {
            var success = await _claimService.SetAssignedTo(Int64.Parse(claimId), Int64.Parse(assignedTo), SecurityHelper.GetUserIdFromContext());
            UserActionLogHelper.AddUserActionLog(UserActionLogConstant.ChangeAssignedTo, Convert.ToInt64(claimId));

            return Json(success.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdvancedSearchChangeFollowUpDate(string claimId, string date)
        {
            DateTime FollowUpDate;

            DateTime.TryParse(date, out FollowUpDate);

            var success = await _claimService.SetFollowUpdateByClaimIdAsync(Convert.ToInt64(claimId), FollowUpDate, SecurityHelper.GetUserIdFromContext());

            UserActionLogHelper.AddUserActionLog(UserActionLogConstant.ChangeFollowUpDate, Convert.ToInt64(claimId));

            return Json(success.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteClaim)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteClaim)]
        public async Task<JsonResult> DeleteClaim(long id)
        {
            var user = SecurityHelper.GetUserIdFromContext();
            var success = await _claimService.DeleteClaimFromViewClaim(id,user);
            return Json(new { Success = success }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveClaim(long id,bool status)
        {
            var success = await _claimService.ApproveOrRejectClaims(id, status);
            if (success)
            {
                var action = status ? "Approved Claim" : "Rejected Claim";
                UserActionLogHelper.AddUserActionLog(action, id);
            }
            return RedirectToAction("GetHeaderInfo", new { claimId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RejectClaim(long id,bool status)
        {
            var success = await _claimService.ApproveOrRejectClaims(id, status);
            if (success)
            {
                var action = status ? "Approved Claim" : "Rejected Claim";
                UserActionLogHelper.AddUserActionLog(action, id);
            }
            return RedirectToAction("GetHeaderInfo", new { claimId = id });
        }

        #region Private Methods

        private async Task SendNewClaimCreatedEmail(Int64 recipientUserId, Int64 ClaimId,List<string> locationUsersEmail)
        {

            UserDto assignedUser = await _lookUpService.GetUserbyIdAsync(recipientUserId);

            var userName = SecurityHelper.GetUserNameFromContext();

            //Creating Email Request
            EmailRequest emailTemplate = EmailTemplatesHelper.CreateEmailForNewClaimAssigned(userName, assignedUser.Email, ClaimId, locationUsersEmail);

            bool emailStatus;
            if (emailTemplate != null)
            {
                //Sending Email
                emailStatus = await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(emailTemplate));
            }
        }



        private async Task SendSentForApprovalEmail(Int64 recipientUserId, Int64 ClaimId)
        {

            UserDto assignedUser = await _lookUpService.GetUserbyIdAsync(recipientUserId);

            var userName = SecurityHelper.GetUserNameFromContext();

            //Creating Email Request
            EmailRequest emailTemplate = EmailTemplatesHelper.CreateEmailForNewClaimSentforApproval(userName, assignedUser.Email, ClaimId);

            bool emailStatus;
            if (emailTemplate != null)
            {
                //Sending Email
                emailStatus = await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(emailTemplate));
            }
        }


        private static string GetSortByColumnName(string sortBy)
        {
            switch (sortBy)
            {
                case "Status":
                    return "RiskClaimStatus.Description";
                case "ContractNumber":
                    return "RiskContract.ContractNumber";
                case "DriverName":
                    return "RiskDriver.FirstName";
                case "UnitNumber":
                    return "RiskVehicle.UnitNumber";
                case "Model":
                    return "RiskVehicle.Model";
                default:
                    return sortBy;
            }
        }

        private async Task<dynamic> GetClaimAndPagingInfo(int page, string sortBy, bool sortOrder, int recordCountToDisplay, dynamic pageHelp, string type, ClaimSearchDto claimSearchDto)
        {
            var sortByColumn = GetSortByColumnName(sortBy);
            var userId = SecurityHelper.GetUserIdFromContext();

            ClaimType claimType = new ClaimType();
            Enum.TryParse(type, out  claimType);

            if (claimType.Equals(ClaimType.FollowupClaim))
            {
                pageHelp.TotalRecordCount = await _claimService.GetFollowUpCountList(userId);
            }
            else if (claimType.Equals(ClaimType.AdvancedSearch))
            {
                pageHelp.TotalRecordCount = await _claimService.GetAdvancedSearchRecordCount(claimSearchDto);
                pageHelp.Type = claimType.ToString();
            }
            else
            {
                pageHelp.TotalRecordCount = await _claimService.GetPendingApprovedCountList(userId, claimType);
                pageHelp.Type = claimType.ToString();
            }



            pageHelp.SortOrder = sortOrder ? false : true;

            recordCountToDisplay = recordCountToDisplay > 0 ? recordCountToDisplay : pageHelp.TotalRecordCount;

            double pageCount = (double)pageHelp.TotalRecordCount / recordCountToDisplay;

            pageHelp.CurrentPage = recordCountToDisplay > 0 ? page : 1;
            pageHelp.NumberOfPages = recordCountToDisplay > 0 ? (int)Math.Ceiling(pageCount) : 1;


            //int startPageIndex = (int)((page - 1) * recordCountToDisplay) + 1;

            //int endPageIndex = ((startPageIndex - 1) + recordCountToDisplay) > pageHelp.TotalRecordCount ? pageHelp.TotalRecordCount : ((startPageIndex - 1) + recordCountToDisplay);

            //if (pageHelp.TotalRecordCount > 0)
            //    pageHelp.PageInfo = string.Format("{0} - {1} of {2} items", startPageIndex, endPageIndex, pageHelp.TotalRecordCount);
            //else
            //    pageHelp.PageInfo = Constants.StringConstants.NoClaimsFound;


            SearchClaimsCriteria claimsCriteria = new SearchClaimsCriteria
            {

                ClaimType = claimType,
                PageCount = page,
                PageSize = recordCountToDisplay,
                SortOrder = sortOrder,
                UserId = userId,
                SortType = sortByColumn
            };

            IEnumerable<ClaimDto> list = await SearchTypeList(Convert.ToString(claimType), claimsCriteria, claimSearchDto);

            //if (claimType.Equals(ClaimType.FollowupClaim))
            //{
            //    list = await _claimService.GetClaimsByCriteria(claimsCriteria);
            //    IEnumerable<UserDto> users = await _lookUpService.GetAllUsersAsync();

            //    list.ToList().ForEach(x => x.Users = users);
            //}
            //else
            //    list = await _claimService.GetPendingApprovedClaimsByCriteria(claimsCriteria);



            //if (claimType.Equals(ClaimType.AdvancedSearch))
            //{
            //    pageHelp.TotalRecordCount = list.Count();

            //    recordCountToDisplay = recordCountToDisplay > 0 ? recordCountToDisplay : pageHelp.TotalRecordCount;

            //    double advancedPageCount = (double)pageHelp.TotalRecordCount / recordCountToDisplay;

            //    pageHelp.CurrentPage = recordCountToDisplay > 0 ? page : 1;
            //    pageHelp.NumberOfPages = recordCountToDisplay > 0 ? (int)Math.Ceiling(advancedPageCount) : 1;
            //    pageHelp.Type = claimType.ToString();
            //    list = list.Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1)).Take(claimsCriteria.PageSize);
            //}

            IEnumerable<ClaimInfoViewModel> model = ClaimHelper.GetClaimListViewModel(list);

            pageHelp.Data = model;

            pageHelp.RecordsToShow = recordCountToDisplay > 0 ? recordCountToDisplay : 1;
            pageHelp.SortBy = sortBy;

            int startPageIndex = (int)((claimsCriteria.PageCount - 1) * recordCountToDisplay) + 1;

            int endPageIndex = ((startPageIndex - 1) + recordCountToDisplay) > pageHelp.TotalRecordCount ? pageHelp.TotalRecordCount : ((startPageIndex - 1) + recordCountToDisplay);

            if (pageHelp.TotalRecordCount > 0)
                pageHelp.PageInfo = string.Format("{0} - {1} of {2} items", startPageIndex, endPageIndex, pageHelp.TotalRecordCount);
            else
                pageHelp.PageInfo = Constants.StringConstants.NoClaimsFound;


            pageHelp.CurrentPage = claimsCriteria.PageCount;

            return pageHelp;
        }

        private static List<long> GetApproverRoles()
        {
            var roles = new List<Int64>();

            roles.Add(Constants.Roles.RiskSupervisor);
            roles.Add(Constants.Roles.RiskManager);
            return roles;
        }

        private async Task<IEnumerable<ClaimDto>> SearchTypeList(string claimType, SearchClaimsCriteria claimsCriteria, ClaimSearchDto claimSearchDto)
        {
            IEnumerable<ClaimDto> list = null;
            switch (claimType)
            {
                case "FollowupClaim":
                    list = await _claimService.GetClaimsByCriteria(claimsCriteria);
                    IEnumerable<UserDto> users = await _lookUpService.GetAllUsersAsync();
                    list.ToList().ForEach(x => x.Users = users);
                    break;

                case "AdvancedSearch":
                    list = await _claimService.GetAdvancedSearchRecords(claimSearchDto, claimsCriteria);
                    IEnumerable<UserDto> AdvancedSearchUsers = await _lookUpService.GetAllUsersAsync();
                    list.ToList().ForEach(x => x.Users = AdvancedSearchUsers);
                    break;

                case "PendingApproval":
                    list = await _claimService.GetPendingApprovedClaimsByCriteria(claimsCriteria);
                    break;
                case "Approved":
                    list = await _claimService.GetPendingApprovedClaimsByCriteria(claimsCriteria);
                    break;
            }
            return list;
        }
        #endregion


    }
}