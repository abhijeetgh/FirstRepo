using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RateShopper.Services.Data;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System.Globalization;

namespace RateShopper.Controllers
{
    [Authorize]
    [HandleError]
    public class RuleSetController : Controller
    {
        //
        // GET: /RuleSet/
        private ICompanyService companyService;
        private IWeekDayService weekDayService;
        private IRuleSetService ruleSetService;
        private IRuleSetsAppliedService rulesetAppliedService;
        private ICarClassService carClassService;
        private IRentalLengthService rentalLengthService;

        public RuleSetController(ICompanyService companyService, IWeekDayService weekDayService, IRuleSetService ruleSetService, IRuleSetsAppliedService rulesetAppliedService, ICarClassService carClassService, IRentalLengthService rentalLengthService)
        {
            this.companyService = companyService;
            this.weekDayService = weekDayService;
            this.ruleSetService = ruleSetService;
            this.rulesetAppliedService = rulesetAppliedService;
            this.carClassService = carClassService;
            this.rentalLengthService = rentalLengthService;
        }
        [Authorize(Roles = "Admin")]
        public ActionResult RuleSetIndex()
        {
            return View();
        }

        public ActionResult GetCompanies()
        {
            List<CompanyMasterDTO> companyMasterDTO = new List<CompanyMasterDTO>();
            companyMasterDTO = companyService.GetAll().Where(a => !(a.IsDeleted)).Select(company => new CompanyMasterDTO
                                                       {
                                                           ID = company.ID,
                                                           Code = company.Code,
                                                           Name = company.Name,
                                                           Logo = company.Logo,
                                                           IsBrand = company.IsBrand,
                                                           IsDeleted = company.IsDeleted,
                                                           CreatedBy = company.CreatedBy,
                                                           UpdatedBy = company.UpdatedBy,
                                                           CreatedDateTime = company.CreatedDateTime,
                                                           UpdatedDateTime = company.UpdatedDateTime
                                                       }).ToList();
            return Json(companyMasterDTO, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWeekDays()
        {
            List<WeekDaysDTO> lstWeekDay = weekDayService.GetAll().Select(wd => new WeekDaysDTO
            {
                ID = wd.ID,
                Day = wd.Day
            }).ToList();

            return Json(lstWeekDay, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBrandLocation()
        {
            List<LocationBrandModel> locationBrandModel = ruleSetService.GetBrandLocation();
            return Json(locationBrandModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRuleSet()
        {
            List<RuleSetDTO> ruleSet = ruleSetService.GetRuleSet();
            return Json(ruleSet, JsonRequestBehavior.AllowGet);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Method overloading is not allowed in controller with same name and same http type")]
        public ActionResult GetAutomationRuleSet(long LocationBrandID, bool IsWideGap, long ScheduledJobID, bool IsGov=false)
        {
            List<RuleSetDTO> ruleSet = ruleSetService.GetAutomationRuleSet(LocationBrandID, IsWideGap, ScheduledJobID, IsGov);
            return Json(ruleSet, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRuleSetDefaultSetting()
        {
            return Json(ruleSetService.GetRuleSetDefaultSetting(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCarClasses()
        {
            List<CarClassMasterDTO> carclass = carClassService.GetAll().Where(a => !(a.IsDeleted)).Select(cc => new CarClassMasterDTO
                                                                        {
                                                                            ID = cc.ID,
                                                                            Code = cc.Code,
                                                                            Description = cc.Description,
                                                                            Logo = cc.Logo,
                                                                            IsDeleted = cc.IsDeleted,
                                                                            CreatedBy = cc.CreatedBy,
                                                                            UpdatedBy = cc.UpdatedBy,
                                                                            CreatedDateTime = cc.CreatedDateTime,
                                                                            UpdatedDateTime = cc.UpdatedDateTime,
                                                                            CarClassOrder = cc.DisplayOrder
                                                                        }).OrderBy(car => car.CarClassOrder).ToList();
            return Json(carclass, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRentalLengths()
        {
            List<RentalLengthMasterDTO> rentallength = rentalLengthService.GetAll(false).Select(rl => new RentalLengthMasterDTO
            {
                ID = rl.ID,
                Code = rl.Code,
                Description = rl.Description,
                MappedID = rl.MappedID
            }).ToList();
            return Json(rentallength, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplyRuleSet()
        {
            List<LocationBrandModel> locationBrands = rulesetAppliedService.GetLocationBrands();
            if (locationBrands != null && locationBrands.Count > 0)
            {
                ViewBag.LocationBrands = locationBrands;
            }
            List<CarClassDTO> carClasses = carClassService.GetAllCarClasses();
            if (carClasses != null && carClasses.Count > 0)
            {
                ViewBag.CarClasses = carClasses;
            }
            return View();
        }

        public JsonResult GetAppliedRuleSets(string locationBrandId)
        {
            long LocationBrandId = 0;
            List<AppliedRuleSetsDTO> appliedRuleSets = new List<AppliedRuleSetsDTO>();
            if (long.TryParse(locationBrandId, out LocationBrandId))
            {
                appliedRuleSets = rulesetAppliedService.GetAppliedRuleSets(LocationBrandId);
            }

            return Json(appliedRuleSets, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAppliedRuleSetDetails(string locationBrandId)
        {
            long LocationBrandId = 0;
            List<AppliedRuleSetDetailsDTO> appliedRuleSets = new List<AppliedRuleSetDetailsDTO>();
            if (long.TryParse(locationBrandId, out LocationBrandId))
            {
                appliedRuleSets = rulesetAppliedService.GetAppliedRuleSetDetails(LocationBrandId);
            }

            return Json(appliedRuleSets, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ActivateDeactivateRuleSet(string rulesetId, string loggedInUserId, bool Activate)
        {
            long RulesetId = 0, LoggedInUserId = 0;
            if (long.TryParse(rulesetId, out RulesetId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                RuleSetsApplied ruleSet = rulesetAppliedService.GetAll().Where(a => a.ID == RulesetId).FirstOrDefault();

                if (ruleSet != null)
                {
                    ruleSet.IsActive = Activate;
                    ruleSet.UpdatedBy = LoggedInUserId;
                    ruleSet.UpdatedDateTime = DateTime.Now;
                    rulesetAppliedService.Update(ruleSet);
                }
            }

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ApplyNewRuleSet(string rulesetId, string startDate, string endDate, string loggedInUserId)
        {
            long RulseSetId = 0, UserId = 0;

            DateTime StartDate = DateTime.ParseExact(startDate + " 00:00:00", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime EndDate = DateTime.ParseExact(endDate + " 23:59:59", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            if (long.TryParse(rulesetId, out RulseSetId) && long.TryParse(loggedInUserId, out UserId))
            {
                rulesetAppliedService.ApplyRuleSets(RulseSetId, StartDate, EndDate, UserId);
            }

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeleteRuleSet(string rulesetId, string loggedInUserId)
        {
            long RulesetId = 0, LoggedInUserId = 0;
            if (long.TryParse(rulesetId, out RulesetId) && long.TryParse(loggedInUserId, out LoggedInUserId))
            {
                RuleSetsApplied ruleSet = rulesetAppliedService.GetAll().Where(a => a.ID == RulesetId).FirstOrDefault();

                if (ruleSet != null)
                {
                    ruleSet.IsDeleted = true;
                    ruleSet.IsActive = false;
                    ruleSet.UpdatedBy = LoggedInUserId;
                    ruleSet.UpdatedDateTime = DateTime.Now;
                    rulesetAppliedService.Update(ruleSet);
                }
            }

            return Json("success", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateUpdateRuleSet(ICollection<RuleSetGroupDTO> lstRuleSetGroupDTO, RuleSetTemplateDTO ruleSetTemplate)
        {
            List<RuleSetGroupDTO> listRuleSetGroupDTO = lstRuleSetGroupDTO.ToList();
            //long RuleSetID = 0;
            RuleSetTemplateDTO ruleSetTemplateReturn = new RuleSetTemplateDTO();
            if (ruleSetTemplate != null && ruleSetTemplate.ID == 0)
            {
                ruleSetTemplateReturn = ruleSetService.CreateRuleSet(listRuleSetGroupDTO, ruleSetTemplate, Resource_Files.CustomMessages.RuleSetGroupNameDB, Convert.ToDecimal(Resource_Files.CustomMessages.RuleSetGroupMaxValue));
            }
            else
            {
                ruleSetTemplateReturn = ruleSetService.UpdateRuleSet(listRuleSetGroupDTO, ruleSetTemplate, Resource_Files.CustomMessages.RuleSetGroupNameDB, Convert.ToDecimal(Resource_Files.CustomMessages.RuleSetGroupMaxValue));
            }
            return Json(ruleSetTemplateReturn, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSelectedRuleSetData(long RuleSetID)
        {
            UpdateRuleSet UpdateRuleSet = ruleSetService.GetSelectedRuleSetData(RuleSetID);
            return Json(UpdateRuleSet, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteSelectedRuleSetData(long RuleSetID, long LoggedInUserId)
        {
            bool flag = false;
            if (RuleSetID != 0 && LoggedInUserId != 0)
            {
                flag = ruleSetService.DeletRuleSet(RuleSetID, LoggedInUserId);

                RuleSetsApplied ruleSetApplied = rulesetAppliedService.GetAll().Where(a => a.RuleSetID == RuleSetID).FirstOrDefault();

                if (ruleSetApplied != null)
                {
                    ruleSetApplied.IsDeleted = true;
                    ruleSetApplied.IsActive = false;
                    ruleSetApplied.UpdatedBy = LoggedInUserId;
                    ruleSetApplied.UpdatedDateTime = DateTime.Now;
                    rulesetAppliedService.Update(ruleSetApplied);
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

    }
}