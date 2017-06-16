using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.ClaimBasicInfo;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class ClaimBasicInfoController : Controller
    {

        #region  Private Members

        private IClaimService _claimService;
       
        #endregion

        /// <summary>
        /// public Claims controller Constructor
        /// </summary>
        #region Constructors
        public ClaimBasicInfoController(IClaimService claimService)
        {

            _claimService = claimService;
        }
        #endregion

        //
        // GET: /ClaimBasicInfo/
        public ActionResult Index()
        {
            return View();
        }

       
        [HttpGet]
        public async Task<PartialViewResult> ViewBasicInfoClaim(Int64 id)
        {
            ClaimBasicInfoDto claimBasicInfo = await _claimService.GetClaimBasicInfoByClaimIdAsync(id);
            ClaimBasicInfoViewModel model = new ClaimBasicInfoViewModel();

            if (claimBasicInfo != null)
            {
                Session["ClaimId"] = id;
                model = ClaimHelper.GetClaimBasicInfoViewModel(claimBasicInfo);
            }
            return PartialView(model);
        }


        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditClaimInfo)]
        public async Task<PartialViewResult> EditClaimInfo(Int64 claimId)
        {
            ClaimDto claimInfo = await _claimService.GetClaimInfoByClaimIdAsync(claimId);

            ClaimInfoForViewEditViewModel model = ClaimHelper.GetClaimInfoForViewEditViewModel(claimInfo, true);

            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditClaimInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.EditClaimInfo)]
        public async Task<ActionResult> EditClaimInfo(ClaimInfoForViewEditViewModel claimViewModel)
        {
            if (ModelState.IsValid)
            {
                ClaimDto claimDto = ClaimHelper.GetClaimInfoDto(claimViewModel);

                Nullable<Int64> claimID = await _claimService.UpdateClaimInfoAsync(claimDto, SecurityHelper.GetUserIdFromContext());

                return RedirectToAction("ViewBasicInfoClaim", new { id = claimID });
            }
            else
            {
                //IEnumerable<LossTypesDto> lossTypes = await _lookUpService.GetAllLossTypesAsync();

                claimViewModel = ClaimHelper.GetListsInClaimViewModel(claimViewModel);

                return PartialView(claimViewModel);
            }

        }


        public async Task<PartialViewResult> ViewClaimInfo(Int64 claimId)
        {

            ClaimDto claimInfo = await _claimService.GetClaimInfoByClaimIdAsync(claimId);

            ClaimInfoForViewEditViewModel model = ClaimHelper.GetClaimInfoForViewEditViewModel(claimInfo, false);

            return PartialView(model);
        }

        public async Task<PartialViewResult> ViewContractInfo(Int64 contractId, Int64 claimId)
        {
            ContractDto contractDto = await _claimService.GetContractInfoByIdAsync(contractId);
            var claim = await _claimService.GetClaimInfoByClaimIdAsync(claimId);
            ContractInfoViewModel model = ClaimHelper.GetContractInfoViewModel(contractDto, claimId);
            model.TotalDue = claim.TotalDue;
            return PartialView(model);
        }


        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        public async Task<PartialViewResult> EditContractInfo(Int64 contractId, Int64 claimId)
        {

            ContractDto contractDto = await _claimService.GetContractInfoByIdAsync(contractId);

            ContractInfoViewModel model = ClaimHelper.GetContractInfoViewModel(contractDto, claimId);

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.EditContractInformation)]
        public async Task<ActionResult> EditContractInfo(ContractInfoViewModel claimViewModel)
        {
            if (ModelState.IsValid)
            {
                ContractDto contractDto = ClaimHelper.GetContractInfoDto(claimViewModel);

                Nullable<Int64> contractId = await _claimService.UpdateContractInfoAsync(contractDto, claimViewModel.ClaimId);

                return RedirectToAction("ViewContractInfo", new { contractId = contractId, claimId = claimViewModel.ClaimId });

            }

            return PartialView(claimViewModel);

        }



        public async Task<PartialViewResult> ViewNonContractInfo(Int64 nonContractId, Int64 claimId)
        {
            NonContractDto nonContractDto = await _claimService.GetNonContractInfoByIdAsync(nonContractId);

            NonContractInfoViewModel model = ClaimHelper.GetNonContractInfoViewModel(nonContractDto, claimId);
           
            return PartialView(model);
        }


        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        public async Task<PartialViewResult> EditNonContractInfo(Int64? nonContractId, Int64 claimId)
        {

            NonContractDto nonContractDto = await _claimService.GetNonContractInfoByIdAsync(nonContractId.Value);

            NonContractInfoViewModel model = ClaimHelper.GetNonContractInfoViewModel(nonContractDto, claimId);

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.EditNonContractInformation)]
        public async Task<ActionResult> EditNonContractInfo(NonContractInfoViewModel claimViewModel)
        {
            if (ModelState.IsValid)
            {
                NonContractDto nonContractDto = ClaimHelper.GetNonContractInfoDto(claimViewModel);

                Nullable<Int64> nonContractId = await _claimService.UpdateNonContractInfoAsync(nonContractDto, claimViewModel.ClaimId);

                return RedirectToAction("ViewNonContractInfo", new { nonContractId = nonContractId, claimId = claimViewModel.ClaimId });

            }

            return PartialView(claimViewModel);

        }

        [HttpGet]
        public async Task<ActionResult> GenerateNonContractNumber(long claimId) {

            string nonContractNumber = await _claimService.GenerateNonContractNumber(claimId);

            return Json(new { nonContractNumber = nonContractNumber }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        public PartialViewResult DownloadContractInfo(Int64 claimId, Int64 contractId, string contractNumber)
        {
            DownloadContractInfoViewModel model = ClaimHelper.GetDownloadContractInfoViewModel(claimId, contractId, contractNumber);

            return PartialView(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditDownloadContractInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.DownloadContractInformation)]
        public async Task<ActionResult> DownloadContractInfo(DownloadContractInfoViewModel downloadContractInfo)
        {
            if (ModelState.IsValid)
            {

                await _claimService.DownloadContractInfo(downloadContractInfo.ClaimId, downloadContractInfo.ContractNumber);

                //return RedirectToAction("ViewBasicInfoClaim", new { id = downloadContractInfo.ClaimID });

                return Json("True", JsonRequestBehavior.AllowGet);
            }

            return PartialView(downloadContractInfo);

        }

        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.DownloadVehicleInfo)]
        public PartialViewResult DownloadVehicleInfo(Int64 claimId, Int64 vehicleId, string contractNumber, string unitNumber)
        {
            DownloadVehicleInfoViewModel model = ClaimHelper.GetDownloadVehicleInfoViewModel(claimId, vehicleId, contractNumber, unitNumber);

            return PartialView(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.DownloadVehicleInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.DownloadVehicleInformation)]
        public async Task<ActionResult> DownloadVehicleInfo(DownloadVehicleInfoViewModel downloadVehicleInfo)
        {
            if (ModelState.IsValid)
            {

                Nullable<Int64> vehicleId = await _claimService.DownloadVehicleInfo(downloadVehicleInfo.ClaimID, downloadVehicleInfo.UnitNumber);

                return RedirectToAction("ViewVehicleInfo", new { vehicleId = vehicleId, contractNumber = downloadVehicleInfo.ContractNumber, claimId = downloadVehicleInfo.ClaimID });
            }

            return PartialView(downloadVehicleInfo);

        }


        public async Task<PartialViewResult> ViewVehicleInfo(Int64 vehicleId, string contractNumber, Int64 claimId)
        {
            VehicleDto vehicleDto = await _claimService.GetVehicleInfoByIdAsync(vehicleId, contractNumber);

            VehicleInfoViewModel model = ClaimHelper.GetVehicleInfoViewModel(vehicleDto, claimId);

            //Need to get the swapped vehicles of the contract
            model.ContractNumber = contractNumber;

            return PartialView(model);
        }


        [HttpGet]
        public async Task<PartialViewResult> EditVehicleInfo(Int64 vehicleId, string contractNumber, Int64 claimId)
        {
            VehicleInfoViewModel model = null;

            if (vehicleId != 0)
            {
                VehicleDto vehicleDto = await _claimService.GetVehicleInfoByIdAsync(vehicleId, null);

                model = ClaimHelper.GetVehicleInfoViewModel(vehicleDto, claimId);
            }
            else
                model = new VehicleInfoViewModel();

            //Need to get the swapped vehicles of the contract
            model.ContractNumber = contractNumber;

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.EditVehicleInformation)]
        public async Task<ActionResult> EditVehicleInfo(VehicleInfoViewModel vehicleViewModel)
        {
            if (ModelState.IsValid)
            {
                VehicleDto vehicleDto = ClaimHelper.GetVehicleInfoDto(vehicleViewModel);

                Nullable<Int64> vehicleId = await _claimService.AddOrUpdateVehicleInfoAsync(vehicleDto, vehicleViewModel.ClaimId);

                return RedirectToAction("ViewVehicleInfo", new { vehicleId = vehicleId, contractNumber = vehicleViewModel.ContractNumber, claimId = vehicleViewModel.ClaimId });
            }

            return PartialView(vehicleViewModel);


        }

        public async Task<PartialViewResult> ViewIncidentInfo(Int64 incidentId)
        {
            IncidentDto incidentDto = await _claimService.GetIncidentInfoByIdAsync(incidentId);

            IncidentInfoViewModel model = ClaimHelper.GetIncidentInfoViewModel(incidentDto, false);

            return PartialView(model);
        }


        [HttpGet]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditIncidentInfo)]
        public async Task<PartialViewResult> EditIncidentInfo(Int64 incidentId)
        {
            IncidentInfoViewModel model = null;

            if (incidentId != 0)
            {
                IncidentDto incidentDto = await _claimService.GetIncidentInfoByIdAsync(incidentId);

                model = ClaimHelper.GetIncidentInfoViewModel(incidentDto, true);
            }
            else
            {

                model = ClaimHelper.GetIncidentInfoViewModel(null, true);
            }

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroizeAttribute(ClaimType = ClaimsConstant.EditIncidentInfo)]
        [UserActionLog(UserAction = UserActionLogConstant.EditIncidentInfo)]
        public async Task<ActionResult> EditIncidentInfo(IncidentInfoViewModel incidentInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                IncidentDto incidentDto = ClaimHelper.GetIncidentInfoDto(incidentInfoViewModel);

                Nullable<Int64> incidentId = await _claimService.UpdateIncidentInfoAsync(incidentDto);

                return RedirectToAction("ViewIncidentInfo", new { incidentId = incidentId });
            }
            else
            {

                incidentInfoViewModel = ClaimHelper.GetListsInIncidentViewModel(incidentInfoViewModel);

                return PartialView(incidentInfoViewModel);
            }


        }

    }
}