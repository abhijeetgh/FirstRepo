using EZRAC.Core.Caching;
using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Admin;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class InsuranceCompanyController : Controller
    {
         #region  Private Members
        
        private ILookUpService _lookUpService;
        private IInsuranceCompanyService _insuranceService;
       

        #endregion
        public InsuranceCompanyController(ILookUpService lookUpService, IInsuranceCompanyService insuranceService)
        {
            _lookUpService =  lookUpService;
            _insuranceService = insuranceService;
           
        }

        public async Task<ActionResult> Index()
        {
            var masterViewModel = new InsuranceCompanyMasteViewModel();

            var viewModels = await GetInsuranceCompaniesViewModel();
            masterViewModel.InsuranceCompanyList = viewModels;

            masterViewModel.InsuranceComapnyViewModel = new InsuranceCompanyViewModel();
            masterViewModel.InsuranceComapnyViewModel.IsNew = true;
            return View(masterViewModel);
        }

        public async Task<PartialViewResult> InsuranceCompany(long id)
        {
           var insuranceDto = await _insuranceService.GetInsuranceCompanyDetailById(id);
           var viewModel = AdminHelper.GetInsuranceCompanyViewModel(insuranceDto);
           return PartialView("_AddUpdateInsuranceCompany", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateInsuranceCompany(InsuranceCompanyViewModel viewModel)
        {
            var userId = SecurityHelper.GetUserIdFromContext();
            var dto = AdminHelper.GetInsuranceDto(viewModel);
            var success = await _insuranceService.AddOrUpdateInsuranceCompany(dto, userId);
            Cache.Remove(Constants.CacheConstants.InsuranceCompanies);
            if (success)
            {
                return RedirectToAction("GetInsuranceCompanyList");
            }
            return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteInsuranceCompany(int id)
        {
            var userId = SecurityHelper.GetUserIdFromContext();

            var success = await _insuranceService.DeleteInsuranceCompany(
                new InsuranceDto { 
                    Id = id
                }, userId);
            Cache.Remove(Constants.CacheConstants.InsuranceCompanies);
            if (!success)
            {
                return Json(new { Data = false }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("GetInsuranceCompanyList");
        }


        public async Task<PartialViewResult> GetInsuranceCompanyList()
        {
            var viewModels = await GetInsuranceCompaniesViewModel();
            return PartialView("_InsuranceCompanyTable", viewModels);
        }

        public PartialViewResult AddInsuranceCompany()
        {
            var viewModel = new InsuranceCompanyViewModel() {  IsNew = true};
            return PartialView("_AddUpdateInsuranceCompany", viewModel);
        }

        public JsonResult InsuranceCompanyAlreadyUsed(long id)
        {
            var alreadyUsed = _insuranceService.IsInsuranceCompanyAlreadyUsed(id);
            return Json(new { Data = alreadyUsed }, JsonRequestBehavior.AllowGet);
        }

        private async Task<IEnumerable<InsuranceCompanyViewModel>> GetInsuranceCompaniesViewModel()
        {
              var insuranceComapny = await _lookUpService.GetAllInsuranceCompaniesAsync();
              var viewModel = AdminHelper.GetInsuranceCompanyListVideModel(insuranceComapny);
              return viewModel;
        }

	}
}