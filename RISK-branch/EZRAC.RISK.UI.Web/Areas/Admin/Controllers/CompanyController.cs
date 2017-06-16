using EZRAC.Risk.UI.Web.Helper;
//using EZRAC.Risk.UI.Web.ViewModels.Claims;
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
using EZRAC.Core.Caching;
namespace EZRAC.Risk.UI.Web.Areas.Admin.Controllers
{
    [CRMSAdminAccess]
    public class CompanyController : Controller
    {
        private ICompanyService _companyService;
        //
        // GET: /Admin/Company/
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        public async Task<ActionResult> Index()
        {
            IEnumerable<ViewModels.Admin.CompanyViewModel> userviewModel = await GetCompanyList();
            return View(userviewModel);
        }

        public async Task<ActionResult> GetCompanyById(long companyId)
        {
            ViewModels.Admin.CompanyViewModel companyviewModel = new ViewModels.Admin.CompanyViewModel();
            var result = await _companyService.GetCompanyById(companyId);
            companyviewModel = AdminHelpers.GetCompanyViewModel(result);
            return PartialView("_CompanyEditProfile", companyviewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUpdateCompany(ViewModels.Admin.CompanyViewModel companyViewModel)
        {
            CompanyDto companyDto = new CompanyDto();
            if (ModelState.IsValid)
            {
                companyDto = AdminHelpers.GetCompanyDto(companyViewModel);
                bool flag = false;
                if (companyDto.Id > 0)
                {
                    flag = await _companyService.UpdateCompanyAsync(companyDto);
                }
                else
                {
                    flag = await _companyService.AddCompanyAsync(companyDto);
                }
            }
            Cache.Remove(Constants.CacheConstants.Companies);
            IEnumerable<ViewModels.Admin.CompanyViewModel> userviewModel = await GetCompanyList();

            return PartialView("_CompanyList", userviewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCompanyById(long companyId)
        {
            bool flag = await _companyService.DeleteCompanyAsync(companyId);
            Cache.Remove(Constants.CacheConstants.Companies);
            return Json(flag.ToString(), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> IsCompanyMapped(long companyId)
        {
            bool flag = await _companyService.IsCompanyMapped(companyId);
            return Json(flag.ToString(), JsonRequestBehavior.AllowGet);
        }

        #region privatemethods
        private async Task<IEnumerable<ViewModels.Admin.CompanyViewModel>> GetCompanyList()
        {
            IEnumerable<ViewModels.Admin.CompanyViewModel> companyviewModel = null;
            var result = await _companyService.GetCompanyListAsync();
            if (result.Any())
            {
                companyviewModel = MappedCompanyViewModel(result);
            }
            return companyviewModel;
        }
        private static IEnumerable<ViewModels.Admin.CompanyViewModel> MappedCompanyViewModel(IEnumerable<CompanyDto> companylist)
        {
            IEnumerable<ViewModels.Admin.CompanyViewModel> companyviewModel = null;
            companyviewModel = companylist.Select(x => new ViewModels.Admin.CompanyViewModel
            {
                Id = x.Id,
                Abbr = x.Abbr,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Zip = x.Zip,
                Phone = x.Phone,
                Fax = x.Fax,
                Website = x.Website
                //Zurich = x.Zurich
            }).ToList<ViewModels.Admin.CompanyViewModel>();
            return companyviewModel;
        }
        #endregion
    }
}