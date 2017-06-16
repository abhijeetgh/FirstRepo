using RateShopper.Domain.DTOs;
using RateShopper.Services.Data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RateShopper.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class FormulaController : Controller
    {
        IFormulaService _formulaService;

        public FormulaController(IFormulaService formulaService)
        {
            _formulaService = formulaService;
        }
                
        public ActionResult Index()
        {
            FormulaDTODetails objFormulaDTODetails = new FormulaDTODetails();

            List<CompanyDTO> lstCompanies = _formulaService.GetCompany();
            ViewBag.Companies = lstCompanies;
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            if (lstCompanies.Count > 0)
            {
                if (TempData["brandID"] != null)
                {
                    ViewBag.SelectedCompanyID = TempData["brandID"];
                    objFormulaDTODetails.LstFormulaDTO = _formulaService.GetAllFormulas(Convert.ToInt64(TempData["brandID"], System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    ViewBag.SelectedCompanyID = lstCompanies[0].ID;
                    objFormulaDTODetails.LstFormulaDTO = _formulaService.GetAllFormulas(lstCompanies[0].ID);
                }
                
                return View(objFormulaDTODetails);
            }
            return View();
        }

        public ActionResult GetFormulasOfCompany(long id)
        {
            TempData["brandID"] = id;
            return RedirectToAction("Index");
        }      

        [HttpPost]
        public ActionResult SaveFormula(FormulaDTODetails formulaDetails)
        {
            if (formulaDetails != null)
            {
                if (ModelState.IsValid)
                {
                    formulaDetails.LstFormulaDTO.RemoveAll(d => !d.IsEdited);
                    if (formulaDetails.LstFormulaDTO.Count > 0)
                    {
                        _formulaService.SaveFormulas(formulaDetails);
                    }
                    TempData["Message"] = "The formulas saved successfully.";
                }
                TempData["brandID"] = formulaDetails.BrandID;
            }
            return RedirectToAction("Index");
        }
	}
}