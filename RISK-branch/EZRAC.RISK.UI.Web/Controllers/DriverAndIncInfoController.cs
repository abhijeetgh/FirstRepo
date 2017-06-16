using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    [Authorize]
    public class DriverAndIncInfoController : Controller
    {

        private IDriverAndIncService _driverAndIncService = null;
        
        public DriverAndIncInfoController(IDriverAndIncService driverAndIncService)
        {

            _driverAndIncService = driverAndIncService;
           

        }
       
        public async Task<PartialViewResult> Index(long claimId)
        {
            IEnumerable<DriverAndInsuranceViewModel> model = null;

            IEnumerable<DriverInfoDto> driversAndInc = await _driverAndIncService.GetDriverListByClaimIdAsync(claimId);

            model = ClaimHelper.GetDriversAndInsuranceInfoViewModel(driversAndInc, claimId);

            return PartialView(model);
        }

        [HttpGet]
        [CRMSAuthroize(ClaimType = ClaimsConstant.EditDriverInfo)]
        public async Task<PartialViewResult> EditDriverAndIncInfo(Int64 driverId, int driverType, Int64 claimId)
        {
            DriverAndInsuranceViewModel model = null;

            if (driverId != 0)
            {
                DriverInfoDto driverAndIncInfoDto = await _driverAndIncService.GetDriverByIdAsync(driverId);

                model = ClaimHelper.GetDriverAndInsuranceInfoViewModel(driverAndIncInfoDto, claimId);
            }
            else
            {
                model = ClaimHelper.GetEmptyDriverInsuranceViewModel(driverType, claimId);
            }

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserActionLog(UserAction = UserActionLogConstant.EditDriverInsuranceInfo)]
        public async Task<ActionResult> EditDriverAndIncInfo(DriverAndInsuranceViewModel driverAndInsuranceViewModel)
        {
            if (ModelState.IsValid) {

                DriverInfoDto driverAndIncInfoDto = ClaimHelper.GetDriverAndInsuranceInfoDto(driverAndInsuranceViewModel);

                Nullable<Int64> driverId = await _driverAndIncService.UpdateDriverAsync(driverAndIncInfoDto, driverAndInsuranceViewModel.ClaimId);


                TempData["RequestedForAddThirdPartyDriver"] = driverAndInsuranceViewModel != null && driverAndInsuranceViewModel.DriverId == 0 && driverAndInsuranceViewModel.DriverType == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty) ? true : false;
                
              
                return RedirectToAction("ViewDriverAndIncInfo", new { driverId = driverId, driverType = driverAndInsuranceViewModel.DriverType, claimId = driverAndInsuranceViewModel.ClaimId });

            } else {                


                driverAndInsuranceViewModel.InsuranceCompanies = LookUpHelpers.GetAllInsuranceComapanyListItems();

                return PartialView(driverAndInsuranceViewModel);
            }

        }

        [HttpGet]
        public async Task<ActionResult> ViewDriverAndIncInfo(Int64 driverId, int driverType, Int64 claimId)
        {
            DriverAndInsuranceViewModel model = null;
            if (driverId != 0)
            {               

                DriverInfoDto driverAndIncInfoDto = await _driverAndIncService.GetDriverByIdAsync(driverId);

                model = ClaimHelper.GetDriverAndInsuranceInfoViewModel(driverAndIncInfoDto, claimId);
            }
                //If user clicks on cancel for empty driver info
                //TODO:Need to analyse and change the code when user clicks on cancel for aditional driver
            else if (driverId == 0 && driverType == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty))
            {
                return Content("");
            }
            else
            {

                model = ClaimHelper.GetEmptyDriverInsuranceViewModel(driverType, claimId);
            }

            return PartialView(model);
        }

        [HttpGet]
        public async Task<PartialViewResult> AddThirdPartyDriver(int driverType, Int64 claimId)
        {   

            DriverAndInsuranceViewModel model = ClaimHelper.GetEmptyDriverInsuranceViewModel(driverType, claimId);

            return PartialView(model);
        }
	}
}