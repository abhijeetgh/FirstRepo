using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class RuleSetService : BaseService<RuleSet>, IRuleSetService
    {
        IRangeIntervalsService _rangeIntervalsService;
        IRuleSetCarClassesService _ruleSetCarClassesService;
        IRuleSetRentalLengthService _ruleSetRentalLengthService;
        IRuleSetWeekDayService _ruleSetWeekDayService;
        IRuleSetGapSettingService ruleSetGapSettingService;
        public RuleSetService(IEZRACRateShopperContext context, ICacheManager cacheManager, IRangeIntervalsService _rangeIntervalsService, IRuleSetCarClassesService _ruleSetCarClassesService, IRuleSetRentalLengthService _ruleSetRentalLengthService, IRuleSetWeekDayService _ruleSetWeekDayService, IRuleSetGapSettingService ruleSetGapSettingService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RuleSet>();
            _cacheManager = cacheManager;
            this._rangeIntervalsService = _rangeIntervalsService;
            this._ruleSetCarClassesService = _ruleSetCarClassesService;
            this._ruleSetRentalLengthService = _ruleSetRentalLengthService;
            this._ruleSetWeekDayService = _ruleSetWeekDayService;
            this.ruleSetGapSettingService = ruleSetGapSettingService;
        }
        public List<LocationBrandModel> GetBrandLocation()
        {
            IQueryable<LocationBrandModel> locationBrands = _context.LocationBrands
                .Join(_context.Locations, Brand => Brand.LocationID, Location => Location.ID, (Brand, Location)
                 => new { LocationID = Brand.LocationID, LocationBrandAlias = Brand.LocationBrandAlias, ID = Brand.ID, LocationCode = Location.Code, BrandID = Brand.BrandID, LocationIsDeleted = Location.IsDeleted, LocationBrandIsDeleted = Brand.IsDeleted }).Where(a => !a.LocationIsDeleted && !a.LocationBrandIsDeleted)
         .Select(obj => new LocationBrandModel { LocationID = obj.LocationID, LocationBrandAlias = obj.LocationBrandAlias, ID = obj.ID, LocationCode = obj.LocationCode, BrandID = obj.BrandID }).OrderBy(a => a.LocationBrandAlias);
            return locationBrands.ToList<LocationBrandModel>();
        }
        public List<RuleSetDTO> GetRuleSet()
        {
            List<RuleSetDTO> lstRuleSetDTO = new List<RuleSetDTO>();

            //List<RuleSet> lstRuleSet = base.GetAll().Where(a => !(a.IsDeleted)).ToList();
            List<RuleSet> lstRuleSet = (from ruleset in base.GetAll()
                                        join locationBrand in _context.LocationBrands on ruleset.LocationBrandID equals locationBrand.ID
                                        where !(locationBrand.IsDeleted) && !(ruleset.IsDeleted) && !(ruleset.IsCopiedAutomationRuleSet)
                                        select ruleset).ToList();

            if (lstRuleSet != null && lstRuleSet.Count() > 0)
            {
                foreach (var lstruleSet in lstRuleSet)
                {
                    RuleSetDTO _oRuleSetDTO = new RuleSetDTO();
                    _oRuleSetDTO.ruleSetID = lstruleSet.ID;
                    _oRuleSetDTO.locationBrandID = lstruleSet.LocationBrandID;
                    _oRuleSetDTO.ruleSetName = lstruleSet.Name;
                    _oRuleSetDTO.companyPositionAbvAvg = lstruleSet.CompanyPositionAbvAvg;
                    _oRuleSetDTO.isPositionOffset = lstruleSet.IsPositionOffset;
                    _oRuleSetDTO.isWideGapTemple = lstruleSet.IsWideGapTemplate;
                    _oRuleSetDTO.isGov = lstruleSet.IsGov;
                    _oRuleSetDTO.selectedCompanyIDs = Convert.ToString(lstruleSet.SelectedCompanyIDs);

                    _oRuleSetDTO.carClassID = string.Join(",", (from rulesetcarclass in _context.RuleSetCarClasses
                                                                join ruleset in _context.RuleSets on rulesetcarclass.RuleSetID equals ruleset.ID
                                                                where ruleset.ID == lstruleSet.ID
                                                                orderby rulesetcarclass.ID
                                                                select rulesetcarclass.CarClassID).ToList());

                    _oRuleSetDTO.rentalLengthID = string.Join(",", (from rulesetrentallength in _context.RuleSetRentalLength
                                                                    join ruleset in _context.RuleSets on rulesetrentallength.RuleSetID equals ruleset.ID
                                                                    where ruleset.ID == lstruleSet.ID
                                                                    orderby rulesetrentallength.ID
                                                                    select rulesetrentallength.RentalLengthID).ToList());

                    _oRuleSetDTO.weekDaysID = string.Join(",", (from rulesetweekdays in _context.RuleSetWeekDay
                                                                join ruleset in _context.RuleSets on rulesetweekdays.RuleSetID equals ruleset.ID
                                                                where ruleset.ID == lstruleSet.ID
                                                                orderby rulesetweekdays.ID
                                                                select rulesetweekdays.WeekDayID).ToList());
                    lstRuleSetDTO.Add(_oRuleSetDTO);

                }
            }
            return lstRuleSetDTO;
        }
        public List<RuleSetDTO> GetAutomationRuleSet(long LocationBrandID, bool IsWideGap, long scheduledJobId, bool IsGov = false)
        {
            List<RuleSetDTO> ruleSetDetails = new List<RuleSetDTO>();
            //List<RuleSet> lstRuleSetMaster = base.GetAll().Where(a => !(a.IsDeleted) && a.IsWideGapTemplate == IsWideGap && a.LocationBrandID == LocationBrandID).ToList();

            //long scheduledJobId = 50123;

            List<long> applicableRuleSetIds = new List<long>();
            //Dictionary<long, DateTime> applicableRuleSets = new Dictionary<long, DateTime>();
            List<ScheduledJobRuleSetDTO> applicableRuleSets = new List<ScheduledJobRuleSetDTO>();
            if (scheduledJobId > 0)
            {
                //if Edit mode
                //Use GetAll for below line
                //Dictionary<long, long> dicScheduleJobRuleSets = _context.ScheduledJobRuleSets.Where(a => a.ScheduleJobID == scheduledJobId).Select(a => new { OriginalRuleSetId = a.OriginalRuleSetID, NewRuleSetId = a.RuleSetID }).ToDictionary(b => b.OriginalRuleSetId, b => b.NewRuleSetId);

                List<ScheduledJobRuleSetDTO> schduleJobRuleSets = _context.ScheduledJobRuleSets.Where(a => a.ScheduleJobID == scheduledJobId).ToList().Select(a => new ScheduledJobRuleSetDTO { OriginalRuleSetID = a.OriginalRuleSetID, RuleSetID = a.RuleSetID, ID = a.ID }).ToList();

                applicableRuleSets = (from ruleset in _context.RuleSets
                                      join appliedRuleset in _context.RuleSetsApplied on ruleset.ID equals appliedRuleset.RuleSetID
                                      where ruleset.LocationBrandID == LocationBrandID && !(ruleset.IsDeleted) && ((IsWideGap && ruleset.IsWideGapTemplate == IsWideGap) || (IsGov && ruleset.IsGov == IsGov)) && !(ruleset.IsCopiedAutomationRuleSet) && !(appliedRuleset.IsDeleted) && appliedRuleset.IsActive && appliedRuleset.EndDate > DateTime.Now
                                      select new
                                      {
                                          ruleSetID = ruleset.ID,
                                          endDate = appliedRuleset.EndDate

                                      }).ToList().Select(a => new ScheduledJobRuleSetDTO
                                      {
                                          RuleSetID = schduleJobRuleSets.Where(d => d.OriginalRuleSetID == a.ruleSetID).FirstOrDefault() != null ? schduleJobRuleSets.Where(d => d.OriginalRuleSetID == a.ruleSetID).FirstOrDefault().RuleSetID : a.ruleSetID, //dicScheduleJobRuleSets.ContainsKey(a.ruleSetID) ? dicScheduleJobRuleSets[a.ruleSetID] : a.ruleSetID,
                                          ID = schduleJobRuleSets.Where(d => d.OriginalRuleSetID == a.ruleSetID).FirstOrDefault() != null ? schduleJobRuleSets.Where(d => d.OriginalRuleSetID == a.ruleSetID).FirstOrDefault().ID : 0, //dicScheduleJobRuleSets.ContainsKey(a.ruleSetID) ? dicScheduleJobRuleSets[a.ruleSetID] : a.ruleSetID,
                                          CreatedDateTime = a.endDate

                                      }).ToList();
            }
            else
            {
                //if create mode

                applicableRuleSets = (from ruleset in _context.RuleSets
                                      join appliedRuleset in _context.RuleSetsApplied on ruleset.ID equals appliedRuleset.RuleSetID
                                      where ruleset.LocationBrandID == LocationBrandID && !(ruleset.IsDeleted) && ((IsWideGap && ruleset.IsWideGapTemplate == IsWideGap) || (IsGov && ruleset.IsGov == IsGov)) && !(ruleset.IsCopiedAutomationRuleSet) && !(appliedRuleset.IsDeleted) && appliedRuleset.IsActive && appliedRuleset.EndDate > DateTime.Now
                                      select new ScheduledJobRuleSetDTO
                                        {
                                            RuleSetID = ruleset.ID,
                                            CreatedDateTime = appliedRuleset.EndDate,
                                            ID = 0
                                        }).ToList();

            }

            if (applicableRuleSets != null && applicableRuleSets.Count > 0)
            {
                applicableRuleSetIds = applicableRuleSets.Select(a => a.RuleSetID).ToList();
                var getRuleSetDetails = (from ruleSet in _context.RuleSets
                                         join ruleSetCarClasses in _context.RuleSetCarClasses on ruleSet.ID equals ruleSetCarClasses.RuleSetID
                                         join ruleSetDaysOfWeeks in _context.RuleSetWeekDay on ruleSet.ID equals ruleSetDaysOfWeeks.RuleSetID
                                         join ruleSetRentalLengths in _context.RuleSetRentalLength on ruleSet.ID equals ruleSetRentalLengths.RuleSetID
                                         where ruleSet.LocationBrandID == LocationBrandID && !ruleSet.IsDeleted && applicableRuleSetIds.Contains(ruleSet.ID)
                                         select new
                                         {
                                             Id = ruleSet.ID,
                                             RuleSetName = ruleSet.Name,
                                             CarClassId = ruleSetCarClasses.CarClassID,
                                             WeekdayId = ruleSetDaysOfWeeks.WeekDayID,
                                             RentalLengthId = ruleSetRentalLengths.RentalLengthID,
                                             IsWideGap = ruleSet.IsWideGapTemplate,
                                             CompanyPositionAbvAvg = ruleSet.CompanyPositionAbvAvg,
                                             IsPositionOffset = ruleSet.IsPositionOffset,
                                             IsGov = ruleSet.IsGov,
                                             SelectedCompanyIDs = ruleSet.SelectedCompanyIDs,
                                             IsCopiedAutomationRuleSet = ruleSet.IsCopiedAutomationRuleSet
                                         }).ToList();

                ruleSetDetails = getRuleSetDetails.GroupBy(a => a.Id)
                .Select(a => new
                {
                    Id = a.Key,
                    RuleSetName = a.Select(b => b.RuleSetName).FirstOrDefault(),
                    CarClasses = a.Select(b => b.CarClassId).Distinct()
                    .OrderBy(c => c),
                    WeekDays = a.Select(b => b.WeekdayId).Distinct().OrderBy(c => c),
                    RentalLengths = a.Select(b => b.RentalLengthId).Distinct().OrderBy(c => c),
                    IsWideGap = a.Select(b => b.IsWideGap).FirstOrDefault(),
                    CompanyPositionAbvAvg = a.Select(b => b.CompanyPositionAbvAvg).FirstOrDefault(),
                    IsPositionOffset = a.Select(b => b.IsPositionOffset).FirstOrDefault(),
                    IsGov = a.Select(b => b.IsGov).FirstOrDefault(),
                    SelectedCompanyIDs = a.Select(b => b.SelectedCompanyIDs).FirstOrDefault(),
                    IsCopiedAutomationRuleSet = a.Select(b => b.IsCopiedAutomationRuleSet).FirstOrDefault()
                }).Select(a => new RuleSetDTO
                {
                    ruleSetID = a.Id,
                    ruleSetName = a.RuleSetName.ToString(),
                    isWideGapTemple = a.IsWideGap,
                    companyPositionAbvAvg = a.CompanyPositionAbvAvg,
                    isPositionOffset = a.IsPositionOffset,
                    isGov = a.IsGov,
                    selectedCompanyIDs = a.SelectedCompanyIDs,
                    isCopiedAutomationRuleSet = a.IsCopiedAutomationRuleSet,
                    weekDaysID = string.Join(",", a.WeekDays.Select(b => b)),
                    rentalLengthID = string.Join(",", a.RentalLengths.Select(b => b)),
                    carClassID = string.Join(",", a.CarClasses),
                    ScheduledJobRuleSetID = applicableRuleSets.Where(d => d.RuleSetID == a.Id).OrderByDescending(d => d.CreatedDateTime).FirstOrDefault().ID,
                    AppliedEndDate = applicableRuleSets.Where(d => d.RuleSetID == a.Id).OrderByDescending(d => d.CreatedDateTime).FirstOrDefault().CreatedDateTime.Value.ToString("MM/dd/yyyy"),
                    locationBrandID = LocationBrandID
                }).OrderBy(a => a.ruleSetID).ToList();

            }
            return ruleSetDetails;
        }

        public RuleSetTemplate GetRuleSetDefaultSetting()
        {
            RuleSetTemplate ruleSetTemplate = new RuleSetTemplate();
            ruleSetTemplate.lstRuleSetGapSettingDay = new List<RuleSetGapSettingCustomDTO>();
            ruleSetTemplate.lstRuleSetGapSettingMonth = new List<RuleSetGapSettingCustomDTO>();
            ruleSetTemplate.lstRuleSetGapSettingWeek = new List<RuleSetGapSettingCustomDTO>();

            List<RangeInterval> lstRangeInterval = new List<RangeInterval>();
            lstRangeInterval = _rangeIntervalsService.GetAll().ToList();

            List<RuleSetGapSettingCustomDTO> tempRuleSetGapSettting = new List<RuleSetGapSettingCustomDTO>();
            List<RuleSetDefaultSetting> ruleSetDefaultSetting = _context.RuleSetDefaultSettings.ToList();
            if (ruleSetDefaultSetting != null && ruleSetDefaultSetting.Count() > 0)
            {
                foreach (var ruleSetGapDefaultSetting in ruleSetDefaultSetting)
                {
                    RuleSetGapSettingCustomDTO ruleSetGapSettingCustomDTO = new Domain.DTOs.RuleSetGapSettingCustomDTO();
                    ruleSetGapSettingCustomDTO.ID = ruleSetGapDefaultSetting.ID;
                    ruleSetGapSettingCustomDTO.MaxAmount = Convert.ToString(ruleSetGapDefaultSetting.MaxAmount);
                    ruleSetGapSettingCustomDTO.MinAmount = Convert.ToString(ruleSetGapDefaultSetting.MinAmount);
                    ruleSetGapSettingCustomDTO.GapAmount = Convert.ToString(ruleSetGapDefaultSetting.GapAmount);

                    ruleSetGapSettingCustomDTO.BaseMaxAmount = Convert.ToString(ruleSetGapDefaultSetting.BaseMaxAmount);
                    ruleSetGapSettingCustomDTO.BaseMinAmount = Convert.ToString(ruleSetGapDefaultSetting.BaseMinAmount);
                    ruleSetGapSettingCustomDTO.BaseGapAmount = Convert.ToString(ruleSetGapDefaultSetting.BaseGapAmount);

                    RangeInterval rangeInterval = new Domain.Entities.RangeInterval();
                    rangeInterval = lstRangeInterval.Where(obj => obj.ID == ruleSetGapDefaultSetting.RangeIntervalID).FirstOrDefault();
                    ruleSetGapSettingCustomDTO.RangeIntervalID = rangeInterval.ID;
                    ruleSetGapSettingCustomDTO.RangeName = rangeInterval.Range;

                    tempRuleSetGapSettting.Add(ruleSetGapSettingCustomDTO);
                }
            }
            ruleSetTemplate.lstRuleSetGapSettingDay = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "D").ToList();
            ruleSetTemplate.lstRuleSetGapSettingWeek = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "W").ToList();
            ruleSetTemplate.lstRuleSetGapSettingMonth = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "M").ToList();

            return ruleSetTemplate;
        }
        public RuleSetTemplateDTO CreateRuleSet(List<RuleSetGroupDTO> lstRuleSetGroupDTO, RuleSetTemplateDTO RuleSetTemplate, string RuleSetGroupNameDB, decimal RuleSetGroupMaxValue)
        {
            // bool Flag = false;
            RuleSet ruleSet = new RuleSet();
            ruleSet.Name = RuleSetTemplate.RuleSetName;
            ruleSet.LocationBrandID = RuleSetTemplate.LocationBrandID;
            ruleSet.CompanyPositionAbvAvg = RuleSetTemplate.CompanyPositionAbvAvg;
            ruleSet.IsPositionOffset = RuleSetTemplate.IsPositionOffset;
            ruleSet.IsWideGapTemplate = RuleSetTemplate.IsWideGapTemplate;
            ruleSet.CreatedBy = RuleSetTemplate.LoggedInUserID;
            ruleSet.UpdatedBy = RuleSetTemplate.LoggedInUserID;
            ruleSet.UpdatedDateTime = DateTime.Now;
            ruleSet.CreatedDateTime = DateTime.Now;
            ruleSet.SelectedCompanyIDs = RuleSetTemplate.RuleSetCompanies;
            //GOV ruleset can be created only if isWideGap checkbox checked
            ruleSet.IsGov = RuleSetTemplate.IsGOV;

            if (RuleSetTemplate.IsAutomationRuleSet)
            {
                ruleSet.IsCopiedAutomationRuleSet = true;
            }

            _context.RuleSets.Add(ruleSet);
            _context.SaveChanges();

            _cacheManager.Remove(typeof(RuleSet).ToString());

            long RuleSetID = ruleSet.ID;


            var carclassID = RuleSetTemplate.RuleSetCarClasses.Split(',');
            foreach (var ID in RuleSetTemplate.RuleSetCarClasses.Split(','))
            {
                RuleSetCarClasses ruleSetCarClass = new RuleSetCarClasses();
                ruleSetCarClass.RuleSetID = RuleSetID;
                ruleSetCarClass.CarClassID = Convert.ToInt32(ID);
                _context.RuleSetCarClasses.Add(ruleSetCarClass);
            }
            foreach (var ID in RuleSetTemplate.RuleSetLengths.Split(','))
            {
                RuleSetRentalLength ruleSetRentalLength = new RuleSetRentalLength();
                ruleSetRentalLength.RuleSetID = RuleSetID;
                ruleSetRentalLength.RentalLengthID = Convert.ToInt32(ID);
                _context.RuleSetRentalLength.Add(ruleSetRentalLength);
            }

            foreach (var ID in RuleSetTemplate.RuleSetDayOfWeeks.Split(','))
            {
                RuleSetWeekDay ruleSetWeekDay = new RuleSetWeekDay();
                ruleSetWeekDay.RuleSetID = RuleSetID;
                ruleSetWeekDay.WeekDayID = Convert.ToInt32(ID);
                _context.RuleSetWeekDay.Add(ruleSetWeekDay);
            }
            _context.SaveChanges();
            _cacheManager.Remove(typeof(RuleSetCarClasses).ToString());
            _cacheManager.Remove(typeof(RuleSetRentalLength).ToString());
            _cacheManager.Remove(typeof(RuleSetWeekDay).ToString());

            int group = 1;
            if (lstRuleSetGroupDTO != null && lstRuleSetGroupDTO.Count() > 0)
            {
                foreach (var ruleSetGroupDTO in lstRuleSetGroupDTO)
                {
                    RuleSetGroup ruleSetGroup = new RuleSetGroup();
                    ruleSetGroup.GroupName = RuleSetGroupNameDB + " " + group;
                    ruleSetGroup.RuleSetID = RuleSetID;
                    _context.RuleSetGroup.Add(ruleSetGroup);
                    _context.SaveChanges();

                    long RuleSetGroupID = ruleSetGroup.ID;

                    foreach (var companyID in ruleSetGroupDTO.CompanyIDs.Split(','))
                    {
                        RuleSetGroupCompany ruleSetGroupCompany = new RuleSetGroupCompany();
                        ruleSetGroupCompany.RuleSetGroupID = RuleSetGroupID;
                        ruleSetGroupCompany.CompanyID = Convert.ToInt32(companyID);
                        _context.RuleSetGroupCompanies.Add(ruleSetGroupCompany);
                    }

                    int dayIndex = 1;
                    foreach (var ruleSetGapDay in ruleSetGroupDTO.lstRuleSetGapSettingDay)
                    {
                        RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                        ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                        ruleSetGapSetting.RangeIntervalID = ruleSetGapDay.RangeIntervalID;
                        ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapDay.MinAmount);
                        ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapDay.BaseMinAmount);
                        if (dayIndex == 5)
                        {
                            ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                        }
                        else
                        {
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapDay.MaxAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapDay.BaseMaxAmount);
                        }
                        ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapDay.GapAmount);
                        ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapDay.BaseGapAmount);
                        _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                        dayIndex++;
                    }
                    int weekIndex = 1;
                    foreach (var ruleSetGapWeek in ruleSetGroupDTO.lstRuleSetGapSettingWeek)
                    {
                        RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                        ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                        ruleSetGapSetting.RangeIntervalID = ruleSetGapWeek.RangeIntervalID;
                        ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapWeek.MinAmount);
                        ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapWeek.BaseMinAmount);
                        if (weekIndex == 5)
                        {
                            ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                        }
                        else
                        {
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapWeek.MaxAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapWeek.BaseMaxAmount);
                        }
                        ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapWeek.GapAmount);
                        ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapWeek.BaseGapAmount);
                        _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                        weekIndex++;
                    }

                    int monthIndex = 1;
                    foreach (var ruleSetGapMonth in ruleSetGroupDTO.lstRuleSetGapSettingMonth)
                    {
                        RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                        ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                        ruleSetGapSetting.RangeIntervalID = ruleSetGapMonth.RangeIntervalID;
                        ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapMonth.MinAmount);
                        ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapMonth.BaseMinAmount);
                        if (monthIndex == 5)
                        {
                            ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                        }
                        else
                        {
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapMonth.MaxAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapMonth.BaseMaxAmount);
                        }
                        ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapMonth.GapAmount);
                        ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapMonth.BaseGapAmount);
                        _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                        monthIndex++;
                    }

                    _context.SaveChanges();
                    group++;
                }
                _cacheManager.Remove(typeof(RuleSetGroup).ToString());
                _cacheManager.Remove(typeof(RuleSetGroupCompany).ToString());
                _cacheManager.Remove(typeof(RuleSetGapSetting).ToString());

            }
            // Flag = true;

            //Thid condition for automation ruleset 
            if (RuleSetTemplate.IsAutomationRuleSet)
            {
                ScheduledJobRuleSets scheduledJobRuleSets = new ScheduledJobRuleSets();
                scheduledJobRuleSets.OriginalRuleSetID = RuleSetTemplate.OriginalRuleSetID;
                scheduledJobRuleSets.RuleSetID = RuleSetID;
                scheduledJobRuleSets.CreatedDateTime = DateTime.Now.Date;

                if (RuleSetTemplate.ScheduledJobID != 0)
                {
                    scheduledJobRuleSets.ScheduleJobID = RuleSetTemplate.ScheduledJobID;
                }
                else
                {
                    scheduledJobRuleSets.ScheduleJobID = null;
                    if (RuleSetTemplate.IntermediateID == "" || RuleSetTemplate.IntermediateID == null)
                    {
                        RuleSetTemplate.IntermediateID = Guid.NewGuid().ToString();
                    }
                    scheduledJobRuleSets.IntermediateID = RuleSetTemplate.IntermediateID;
                }
                _context.ScheduledJobRuleSets.Add(scheduledJobRuleSets);
                _context.SaveChanges();
                RuleSetTemplate.ScheduledJobRulesetID = scheduledJobRuleSets.ID;
            }
            RuleSetTemplate.ID = RuleSetID;

            return RuleSetTemplate;
        }

        public RuleSetTemplateDTO UpdateRuleSet(List<RuleSetGroupDTO> lstRuleSetGroupDTO, RuleSetTemplateDTO RuleSetTemplate, string RuleSetGroupNameDB, decimal RuleSetGroupMaxValue)
        {
            //RuleSetDetails update
            RuleSet ruleSet = base.GetById(RuleSetTemplate.ID);
            ruleSet.Name = RuleSetTemplate.RuleSetName;
            if (RuleSetTemplate.LocationBrandID != 0)
            {
                ruleSet.LocationBrandID = RuleSetTemplate.AddLocationBrandID;
            }
            ruleSet.CompanyPositionAbvAvg = RuleSetTemplate.CompanyPositionAbvAvg;
            ruleSet.IsPositionOffset = RuleSetTemplate.IsPositionOffset;
            ruleSet.IsWideGapTemplate = RuleSetTemplate.IsWideGapTemplate;
            ruleSet.UpdatedBy = RuleSetTemplate.LoggedInUserID;
            ruleSet.UpdatedDateTime = DateTime.Now;
            ruleSet.SelectedCompanyIDs = RuleSetTemplate.RuleSetCompanies;

            ruleSet.IsGov = RuleSetTemplate.IsGOV;

            base.Update(ruleSet);
            //_context.Entry(ruleSet).State = System.Data.Entity.EntityState.Modified;
            //_context.SaveChanges();


            #region Add
            //Add Scenario for RentalLength
            if (RuleSetTemplate.AddRuleSetLengths != null)
            {
                foreach (var ID in RuleSetTemplate.AddRuleSetLengths.Split(','))
                {
                    RuleSetRentalLength ruleSetRentalLength = new RuleSetRentalLength();
                    ruleSetRentalLength.RuleSetID = RuleSetTemplate.ID;
                    ruleSetRentalLength.RentalLengthID = Convert.ToInt32(ID);
                    _context.RuleSetRentalLength.Add(ruleSetRentalLength);
                }
                _context.SaveChanges();
            }
            //Add Scenario for CarClass
            if (RuleSetTemplate.AddRuleSetCarClasses != null)
            {
                foreach (var ID in RuleSetTemplate.AddRuleSetCarClasses.Split(','))
                {
                    RuleSetCarClasses ruleSetCarClass = new RuleSetCarClasses();
                    ruleSetCarClass.RuleSetID = RuleSetTemplate.ID;
                    ruleSetCarClass.CarClassID = Convert.ToInt32(ID);
                    _context.RuleSetCarClasses.Add(ruleSetCarClass);
                }
                _context.SaveChanges();
            }
            //Add Scenario for Days Of Week
            if (RuleSetTemplate.AddRuleSetDayOfWeeks != null)
            {
                foreach (var ID in RuleSetTemplate.AddRuleSetDayOfWeeks.Split(','))
                {
                    RuleSetWeekDay ruleSetWeekDay = new RuleSetWeekDay();
                    ruleSetWeekDay.RuleSetID = RuleSetTemplate.ID;
                    ruleSetWeekDay.WeekDayID = Convert.ToInt32(ID);
                    _context.RuleSetWeekDay.Add(ruleSetWeekDay);
                }
                _context.SaveChanges();
            }
            #endregion

            #region Delete
            //Delete scenario for Rental Length if user unselect existing
            if (RuleSetTemplate.DeleteRuleSetLengths != null)
            {
                foreach (var ID in RuleSetTemplate.DeleteRuleSetLengths.Split(','))
                {
                    long RentalLengthID = long.Parse(ID);
                    RuleSetRentalLength ruleSetRentalLength = _context.RuleSetRentalLength.Where(obj => obj.RuleSetID == RuleSetTemplate.ID && obj.RentalLengthID == RentalLengthID).FirstOrDefault();
                    if (ruleSetRentalLength != null)
                    {
                        //_context.RuleSetRentalLength.Remove(ruleSetRentalLength);
                        _context.Entry(ruleSetRentalLength).State = System.Data.Entity.EntityState.Deleted;
                        //_context.SaveChanges();
                    }
                }
            }
            //Delete scenario for CarClass if user unselect existing
            if (RuleSetTemplate.DeleteRuleSetCarClasses != null)
            {
                foreach (var ID in RuleSetTemplate.DeleteRuleSetCarClasses.Split(','))
                {
                    long CarClassID = long.Parse(ID);
                    RuleSetCarClasses ruleSetCarClasses = _context.RuleSetCarClasses.Where(obj => obj.RuleSetID == RuleSetTemplate.ID && obj.CarClassID == CarClassID).FirstOrDefault();

                    if (ruleSetCarClasses != null)
                    {
                        _context.Entry(ruleSetCarClasses).State = System.Data.Entity.EntityState.Deleted;
                        //_context.RuleSetCarClasses.Remove(ruleSetCarClasses);
                        //_context.SaveChanges();
                    }
                }
            }
            //Delete scenario for Days of week if user unselect existing
            if (RuleSetTemplate.DeleteRuleSetDayOfWeeks != null)
            {
                foreach (var ID in RuleSetTemplate.DeleteRuleSetDayOfWeeks.Split(','))
                {
                    long WeekDaysID = long.Parse(ID);
                    RuleSetWeekDay ruleSetWeekDay = _context.RuleSetWeekDay.Where(obj => obj.RuleSetID == RuleSetTemplate.ID && obj.WeekDayID == WeekDaysID).FirstOrDefault();

                    if (ruleSetWeekDay != null)
                    {
                        _context.Entry(ruleSetWeekDay).State = System.Data.Entity.EntityState.Deleted;
                        //_context.RuleSetWeekDay.Remove(ruleSetWeekDay);
                        //_context.SaveChanges();
                    }
                }
            }
            _context.SaveChanges();

            #endregion

            _cacheManager.Remove(typeof(RuleSetCarClasses).ToString());
            _cacheManager.Remove(typeof(RuleSetRentalLength).ToString());
            _cacheManager.Remove(typeof(RuleSetWeekDay).ToString());
            #region UpdateRuleSetGroup
            int group = (_context.RuleSetGroup.Where(obj => obj.RuleSetID == RuleSetTemplate.ID).ToList().Count()) + 1;
            foreach (var ruleSetGroupDTO in lstRuleSetGroupDTO)
            {
                //if competitor item is null then all dependant related data for that group item should be deleted
                if (ruleSetGroupDTO.DeleteRuleSetGroupID != 0)
                {
                    List<RuleSetGroupCompany> lstRuleSetGroupCompany = _context.RuleSetGroupCompanies.Where(obj => obj.RuleSetGroupID == ruleSetGroupDTO.DeleteRuleSetGroupID).ToList();
                    if (lstRuleSetGroupCompany != null && lstRuleSetGroupCompany.Count() > 0)
                    {
                        foreach (var ruleSetGroupCompanies in lstRuleSetGroupCompany)
                        {
                            //_context.RuleSetGroupCompanies.Remove(ruleSetGroupCompanies);
                            _context.Entry(ruleSetGroupCompanies).State = System.Data.Entity.EntityState.Deleted;
                            //_context.SaveChanges();
                        }
                    }

                    List<RuleSetGapSetting> lstRuleSetGapSetting = _context.RuleSetGapSettings.Where(obj => obj.RuleSetGroupID == ruleSetGroupDTO.DeleteRuleSetGroupID).ToList();
                    if (lstRuleSetGapSetting != null && lstRuleSetGapSetting.Count() > 0)
                    {
                        foreach (var ruleSetGapSettings in lstRuleSetGapSetting)
                        {
                            //_context.RuleSetGapSettings.Remove(ruleSetGapSettings);
                            _context.Entry(ruleSetGapSettings).State = System.Data.Entity.EntityState.Deleted;
                            //_context.SaveChanges();
                        }
                    }

                    long RuleSetGroupID = ruleSetGroupDTO.DeleteRuleSetGroupID;
                    RuleSetGroup ruleSetGroup = _context.RuleSetGroup.Where(obj => obj.ID == ruleSetGroupDTO.DeleteRuleSetGroupID).FirstOrDefault();
                    // _context.Entry(ruleSetRentalLength).State = System.Data.Entity.EntityState.Deleted;
                    if (ruleSetGroup != null)
                    {
                        //_context.RuleSetGroup.Remove(ruleSetGroup);
                        _context.Entry(ruleSetGroup).State = System.Data.Entity.EntityState.Deleted;
                        //_context.SaveChanges();
                    }
                    _context.SaveChanges();
                }
                else
                {
                    //this case user add extra group than insert rule
                    if (ruleSetGroupDTO.AddlstRuleSetGapSettingDay != null && ruleSetGroupDTO.AddlstRuleSetGapSettingWeek != null && ruleSetGroupDTO.AddlstRuleSetGapSettingMonth != null)
                    {
                        RuleSetGroup ruleSetGroup = new RuleSetGroup();
                        ruleSetGroup.GroupName = RuleSetGroupNameDB + " " + group;
                        ruleSetGroup.RuleSetID = RuleSetTemplate.ID;//RuleSetID
                        _context.RuleSetGroup.Add(ruleSetGroup);
                        _context.SaveChanges();

                        long RuleSetGroupID = ruleSetGroup.ID;
                        //Insert RangeInterval Month m1-m5
                        foreach (var companyID in ruleSetGroupDTO.CompanyIDs.Split(','))
                        {
                            RuleSetGroupCompany ruleSetGroupCompany = new RuleSetGroupCompany();
                            ruleSetGroupCompany.RuleSetGroupID = RuleSetGroupID;
                            ruleSetGroupCompany.CompanyID = Convert.ToInt32(companyID);
                            _context.RuleSetGroupCompanies.Add(ruleSetGroupCompany);
                        }

                        //Insert RangeInterval Day d1-d5   
                        int dayIndex = 1;
                        foreach (var ruleSetGapDay in ruleSetGroupDTO.AddlstRuleSetGapSettingDay)
                        {
                            RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                            ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                            ruleSetGapSetting.RangeIntervalID = ruleSetGapDay.RangeIntervalID;
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapDay.MinAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapDay.BaseMinAmount);
                            if (dayIndex == 5)
                            {
                                ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                            }
                            else
                            {
                                ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapDay.MaxAmount);
                                ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapDay.BaseMaxAmount);
                            }
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapDay.GapAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapDay.BaseGapAmount);
                            _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                            dayIndex++;
                        }
                        //Insert RangeInterval Week w1-w5
                        int weekIndex = 1;
                        foreach (var ruleSetGapWeek in ruleSetGroupDTO.AddlstRuleSetGapSettingWeek)
                        {
                            RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                            ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                            ruleSetGapSetting.RangeIntervalID = ruleSetGapWeek.RangeIntervalID;
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapWeek.MinAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapWeek.BaseMinAmount);
                            if (weekIndex == 5)
                            {
                                ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                            }
                            else
                            {
                                ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapWeek.MaxAmount);
                                ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapWeek.BaseMaxAmount);
                            }
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapWeek.GapAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapWeek.BaseGapAmount);
                            _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                            weekIndex++;
                        }
                        //Insert RangeInterval Month m1-m5
                        int monthIndex = 1;
                        foreach (var ruleSetGapMonth in ruleSetGroupDTO.AddlstRuleSetGapSettingMonth)
                        {
                            RuleSetGapSetting ruleSetGapSetting = new RuleSetGapSetting();
                            ruleSetGapSetting.RuleSetGroupID = RuleSetGroupID;
                            ruleSetGapSetting.RangeIntervalID = ruleSetGapMonth.RangeIntervalID;
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(ruleSetGapMonth.MinAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(ruleSetGapMonth.BaseMinAmount);
                            if (monthIndex == 5)
                            {
                                ruleSetGapSetting.MaxAmount = RuleSetGroupMaxValue;
                            }
                            else
                            {
                                ruleSetGapSetting.MaxAmount = Convert.ToDecimal(ruleSetGapMonth.MaxAmount);
                                ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(ruleSetGapMonth.BaseMaxAmount);
                            }
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(ruleSetGapMonth.GapAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(ruleSetGapMonth.BaseGapAmount);
                            _context.RuleSetGapSettings.Add(ruleSetGapSetting);
                            monthIndex++;
                        }
                        _context.SaveChanges();
                        group++;
                    }

                    #region UpdateGroupCompanyADD
                    //Incase user can add extra company in competitor(select)
                    if (ruleSetGroupDTO.AddCompanyIDs != null)
                    {
                        foreach (var ID in ruleSetGroupDTO.AddCompanyIDs.Split(','))
                        {
                            RuleSetGroupCompany ruleSetGroupCompany = new RuleSetGroupCompany();
                            ruleSetGroupCompany.RuleSetGroupID = ruleSetGroupDTO.RuleSetGroupID;
                            ruleSetGroupCompany.CompanyID = Convert.ToInt32(ID);
                            _context.RuleSetGroupCompanies.Add(ruleSetGroupCompany);
                        }
                        _context.SaveChanges();
                    }
                    #endregion
                    #region UpdateGroupCompanyDelete
                    //Incase user can delete existing company in competitor(unselect)
                    if (ruleSetGroupDTO.DeleteCompanyIDs != null)
                    {
                        foreach (var ID in ruleSetGroupDTO.DeleteCompanyIDs.Split(','))
                        {
                            long CompanyID = long.Parse(ID);
                            RuleSetGroupCompany ruleSetGroupCompany = _context.RuleSetGroupCompanies.Where(obj => obj.RuleSetGroupID == ruleSetGroupDTO.RuleSetGroupID && obj.CompanyID == CompanyID).FirstOrDefault();
                            _context.Entry(ruleSetGroupCompany).State = System.Data.Entity.EntityState.Deleted;
                            //_context.RuleSetGroupCompanies.Remove(ruleSetGroupCompany);
                        }
                        _context.SaveChanges();
                    }
                    #endregion

                    //This case only update mode
                    //Update RangeInterval Day d1-d5
                    if (ruleSetGroupDTO.lstRuleSetGapSettingDay != null)
                    {
                        foreach (var RuleSetGapSettingDay in ruleSetGroupDTO.lstRuleSetGapSettingDay)
                        {
                            RuleSetGapSetting ruleSetGapSetting = ruleSetGapSettingService.GetById(RuleSetGapSettingDay.ID);
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(RuleSetGapSettingDay.MinAmount);
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(RuleSetGapSettingDay.MaxAmount);
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(RuleSetGapSettingDay.GapAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(RuleSetGapSettingDay.BaseMinAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(RuleSetGapSettingDay.BaseMaxAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(RuleSetGapSettingDay.BaseGapAmount);
                            _context.Entry(ruleSetGapSetting).State = System.Data.Entity.EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }
                    //Update RangeInterval Week w1-w5
                    if (ruleSetGroupDTO.lstRuleSetGapSettingWeek != null)
                    {
                        foreach (var RuleSetGapSettingWeek in ruleSetGroupDTO.lstRuleSetGapSettingWeek)
                        {
                            RuleSetGapSetting ruleSetGapSetting = ruleSetGapSettingService.GetById(RuleSetGapSettingWeek.ID);
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(RuleSetGapSettingWeek.MinAmount);
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(RuleSetGapSettingWeek.MaxAmount);
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(RuleSetGapSettingWeek.GapAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(RuleSetGapSettingWeek.BaseMinAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(RuleSetGapSettingWeek.BaseMaxAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(RuleSetGapSettingWeek.BaseGapAmount);
                            _context.Entry(ruleSetGapSetting).State = System.Data.Entity.EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }
                    //Update RangeInterval Month m1-m5
                    if (ruleSetGroupDTO.lstRuleSetGapSettingMonth != null)
                    {
                        foreach (var RuleSetGapSettingMonth in ruleSetGroupDTO.lstRuleSetGapSettingMonth)
                        {
                            RuleSetGapSetting ruleSetGapSetting = ruleSetGapSettingService.GetById(RuleSetGapSettingMonth.ID);
                            ruleSetGapSetting.MinAmount = Convert.ToDecimal(RuleSetGapSettingMonth.MinAmount);
                            ruleSetGapSetting.MaxAmount = Convert.ToDecimal(RuleSetGapSettingMonth.MaxAmount);
                            ruleSetGapSetting.GapAmount = Convert.ToDecimal(RuleSetGapSettingMonth.GapAmount);
                            ruleSetGapSetting.BaseMinAmount = Convert.ToDecimal(RuleSetGapSettingMonth.BaseMinAmount);
                            ruleSetGapSetting.BaseMaxAmount = Convert.ToDecimal(RuleSetGapSettingMonth.BaseMaxAmount);
                            ruleSetGapSetting.BaseGapAmount = Convert.ToDecimal(RuleSetGapSettingMonth.BaseGapAmount);
                            _context.Entry(ruleSetGapSetting).State = System.Data.Entity.EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }
                }
                _cacheManager.Remove(typeof(RuleSetGroup).ToString());
                _cacheManager.Remove(typeof(RuleSetGroupCompany).ToString());
                _cacheManager.Remove(typeof(RuleSetGapSetting).ToString());
            #endregion
            }//End else
            return RuleSetTemplate;
        }
        public UpdateRuleSet GetSelectedRuleSetData(long RuleSetID)
        {
            UpdateRuleSet updateRuleSet = new UpdateRuleSet();
            List<RuleSetGroupCustomDTO> lstRuleSetGroupCustom = new List<RuleSetGroupCustomDTO>();
            List<RuleSetGroup> lstRuleSetGroup = (_context.RuleSetGroup.Where(obj => obj.RuleSetID == RuleSetID)).ToList();
            List<RangeInterval> lstRangeInterval = new List<RangeInterval>();
            lstRangeInterval = _rangeIntervalsService.GetAll().ToList();

            //List<Company> lstcompany = (from company in _context.Companies
            //                            join rulesetgroupcompany in _context.RuleSetGroupCompanies on company.ID equals rulesetgroupcompany.CompanyID
            //                            join rulesetgroup in _context.RuleSetGroup on rulesetgroupcompany.RuleSetGroupID equals rulesetgroup.ID
            //                            where rulesetgroup.RuleSetID == RuleSetID
            //                            orderby company.ID
            //                            select company).ToList();

            //updateRuleSet.RuleSetCompanyIDs = string.Join(",", lstcompany.Select(obj => obj.ID).ToArray());
            updateRuleSet.RuleSetCompanyIDs = _context.RuleSets.Where(obj => obj.ID == RuleSetID).Select(obj => obj.SelectedCompanyIDs).FirstOrDefault();

            var queryablegroupcompany = (from ruleSetGroup in _context.RuleSetGroup
                                         join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                         join companies in _context.Companies on ruleSetGroupCompanies.CompanyID equals companies.ID
                                         where ruleSetGroup.RuleSetID == RuleSetID && !(companies.IsDeleted)
                                         select new
                                         {
                                             code = companies.Code,
                                             companyID = companies.ID,
                                             CompanyName = companies.Name,
                                             IsBrand = companies.IsBrand,
                                             ID = ruleSetGroup.ID
                                         }).ToList();
            if (lstRuleSetGroup != null && lstRuleSetGroup.Count() > 0)
            {
                foreach (var ruleSetGroup in lstRuleSetGroup)
                {
                    RuleSetGroupCustomDTO ruleSetGroupCustom = new RuleSetGroupCustomDTO();
                    ruleSetGroupCustom.ID = ruleSetGroup.ID;
                    ruleSetGroupCustom.GroupName = ruleSetGroup.GroupName;
                    //ruleSetGroupCustom.CompanyName = string.Join(", ", queryablegroupcompany.Where(obj => obj.ID == ruleSetGroup.ID).Select(obj => obj.code).ToArray());
                    ruleSetGroupCustom.lstGroupCompany = (from ruleSetGroupCompany in queryablegroupcompany
                                                          where ruleSetGroupCompany.ID == ruleSetGroup.ID
                                                          select new GroupCompany
                                                          {
                                                              CompanyID = ruleSetGroupCompany.companyID,
                                                              CompanyName = ruleSetGroupCompany.CompanyName,
                                                              IsBrand = ruleSetGroupCompany.IsBrand
                                                          }).ToList();
                    //queryablegroupcompany.Where(obj => obj.ID == ruleSetGroup.ID).Select(obj => obj.companyID).ToList();


                    List<RuleSetGapSetting> lstTempRuleSetGapSetting = new List<RuleSetGapSetting>();
                    IQueryable<RuleSetGapSetting> queryablerulesetgapsetting = (from rulesetgapsetting in _context.RuleSetGapSettings
                                                                                where rulesetgapsetting.RuleSetGroupID == ruleSetGroup.ID
                                                                                select rulesetgapsetting);
                    //_context.RuleSetGapSettings.Where(obj => obj.RuleSetGroupID == ruleSetGroup.ID);
                    lstTempRuleSetGapSetting = queryablerulesetgapsetting.ToList();

                    ruleSetGroupCustom.lstRuleSetGapSettingDay = new List<RuleSetGapSettingCustomDTO>();
                    ruleSetGroupCustom.lstRuleSetGapSettingWeek = new List<RuleSetGapSettingCustomDTO>();
                    ruleSetGroupCustom.lstRuleSetGapSettingMonth = new List<RuleSetGapSettingCustomDTO>();
                    List<RuleSetGapSettingCustomDTO> tempRuleSetGapSettting = new List<RuleSetGapSettingCustomDTO>();
                    foreach (var ruleSetGapSetting in lstTempRuleSetGapSetting)
                    {
                        RuleSetGapSettingCustomDTO ruleSetGapSettingCustom = new RuleSetGapSettingCustomDTO();
                        ruleSetGapSettingCustom.ID = ruleSetGapSetting.ID;
                        ruleSetGapSettingCustom.RuleSetGroupID = ruleSetGapSetting.RuleSetGroupID;
                        ruleSetGapSettingCustom.MaxAmount = Convert.ToString(ruleSetGapSetting.MaxAmount);
                        ruleSetGapSettingCustom.MinAmount = Convert.ToString(ruleSetGapSetting.MinAmount);
                        ruleSetGapSettingCustom.GapAmount = Convert.ToString(ruleSetGapSetting.GapAmount);
                        ruleSetGapSettingCustom.BaseMaxAmount = Convert.ToString(ruleSetGapSetting.BaseMaxAmount);
                        ruleSetGapSettingCustom.BaseMinAmount = Convert.ToString(ruleSetGapSetting.BaseMinAmount);
                        ruleSetGapSettingCustom.BaseGapAmount = Convert.ToString(ruleSetGapSetting.BaseGapAmount);

                        RangeInterval rangeInterval = new Domain.Entities.RangeInterval();
                        rangeInterval = lstRangeInterval.Where(obj => obj.ID == ruleSetGapSetting.RangeIntervalID).FirstOrDefault();
                        ruleSetGapSettingCustom.RangeIntervalID = rangeInterval.ID;
                        ruleSetGapSettingCustom.RangeName = rangeInterval.Range;

                        //Add rulesetGapSetting list
                        tempRuleSetGapSettting.Add(ruleSetGapSettingCustom);
                    }
                    ruleSetGroupCustom.lstRuleSetGapSettingDay = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "D").ToList();
                    ruleSetGroupCustom.lstRuleSetGapSettingWeek = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "W").ToList();
                    ruleSetGroupCustom.lstRuleSetGapSettingMonth = tempRuleSetGapSettting.Where(obj => obj.RangeName.Substring(0, 1) == "M").ToList();

                    //Add group wise RuleSetGapSetting list 
                    lstRuleSetGroupCustom.Add(ruleSetGroupCustom);

                }
            }
            updateRuleSet.lstRuleSetGroupCustomDTO = lstRuleSetGroupCustom;
            return updateRuleSet;
        }
        public bool DeletRuleSet(long RuleSetID, long LoggedInUserId)
        {
            bool flag = false;
            RuleSet ruleSet = _context.RuleSets.Where(a => a.ID == RuleSetID).FirstOrDefault();// ruleSetService.GetAll().Where(a => a.ID == RuleSetID).FirstOrDefault();
            if (ruleSet != null)
            {
                ruleSet.IsDeleted = true;
                ruleSet.UpdatedBy = LoggedInUserId;
                ruleSet.UpdatedDateTime = DateTime.Now;
                //ruleSetService.Update(ruleSet);
                _context.Entry(ruleSet).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                _cacheManager.Remove(typeof(RuleSet).ToString());
                flag = true;
            }
            return flag;
        }

        public void DeleteAutomationRuleSet(long ScheduledJobId)
        {
            List<ScheduledJobRuleSets> scheduledJobRuleSets = _context.ScheduledJobRuleSets.Where(obj => obj.ScheduleJobID == ScheduledJobId).ToList();
            //(from sjruleset in _context.ScheduledJobRuleSets 
            // join ruleset in _context.RuleSets on sjruleset.RuleSetID equals ruleset.ID
            // where sjruleset.ScheduleJobID==ScheduledJobId
            // select sjruleset).ToList();
            if (scheduledJobRuleSets != null && scheduledJobRuleSets.Count() > 0)
            {
                foreach (var rulesetitem in scheduledJobRuleSets)
                {
                    //********************Ruleset carclass,rental length and weekday **************
                    List<RuleSetCarClasses> rulesetCarclass = _context.RuleSetCarClasses.Where(obj => obj.RuleSetID == rulesetitem.RuleSetID).ToList();
                    foreach (var carclassitem in rulesetCarclass)
                    {
                        _context.Entry(carclassitem).State = System.Data.Entity.EntityState.Deleted;
                    }

                    List<RuleSetWeekDay> ruleSetWeekDay = _context.RuleSetWeekDay.Where(obj => obj.RuleSetID == rulesetitem.RuleSetID).ToList();
                    foreach (var ruleSetWeekDayItem in ruleSetWeekDay)
                    {
                        _context.Entry(ruleSetWeekDayItem).State = System.Data.Entity.EntityState.Deleted;
                    }

                    List<RuleSetRentalLength> ruleSetRentalLength = _context.RuleSetRentalLength.Where(obj => obj.RuleSetID == rulesetitem.RuleSetID).ToList();
                    foreach (var ruleSetRentalLengthitem in ruleSetRentalLength)
                    {
                        _context.Entry(ruleSetRentalLengthitem).State = System.Data.Entity.EntityState.Deleted;
                    }

                    //********************Ruleset group,groupcompany **************

                    List<RuleSetGroup> ruleSetGroup = _context.RuleSetGroup.Where(obj => obj.RuleSetID == rulesetitem.RuleSetID).ToList();
                    foreach (var groupitem in ruleSetGroup)
                    {
                        //Ruleset group company delete
                        List<RuleSetGroupCompany> ruleSetGroupCompany = _context.RuleSetGroupCompanies.Where(obj => obj.RuleSetGroupID == groupitem.ID).ToList();
                        foreach (var groupcompanyitem in ruleSetGroupCompany)
                        {
                            _context.Entry(groupcompanyitem).State = System.Data.Entity.EntityState.Deleted;
                        }

                        List<RuleSetGapSetting> ruleSetGapSetting = _context.RuleSetGapSettings.Where(obj => obj.RuleSetGroupID == groupitem.ID).ToList();
                        foreach (var rulesetgapitem in ruleSetGapSetting)
                        {
                            _context.Entry(rulesetgapitem).State = System.Data.Entity.EntityState.Deleted;
                        }


                        _context.Entry(groupitem).State = System.Data.Entity.EntityState.Deleted;
                    }

                    //final ruleset delete
                    RuleSet ruleset = _context.RuleSets.Where(obj => obj.ID == rulesetitem.RuleSetID).FirstOrDefault();
                    _context.Entry(ruleset).State = System.Data.Entity.EntityState.Deleted;


                    _context.Entry(rulesetitem).State = System.Data.Entity.EntityState.Deleted;
                    _context.SaveChanges();
                }
                _cacheManager.Remove(typeof(RuleSetCarClasses).ToString());
                _cacheManager.Remove(typeof(RuleSetRentalLength).ToString());
                _cacheManager.Remove(typeof(RuleSetWeekDay).ToString());
                _cacheManager.Remove(typeof(RuleSetGroup).ToString());
                _cacheManager.Remove(typeof(RuleSetGroupCompany).ToString());
                _cacheManager.Remove(typeof(RuleSetGapSetting).ToString());
                _cacheManager.Remove(typeof(RuleSet).ToString());
                _cacheManager.Remove(typeof(ScheduledJobRuleSets).ToString());
            }
        }

        //This method is used for update intermediate id again scheduledjobid
        public void updateAutomationRuleSet(long ScheduledJobId, string IntermediateId)
        {
            List<ScheduledJobRuleSets> scheduledJobRuleset = new List<ScheduledJobRuleSets>();
            if (string.IsNullOrEmpty(IntermediateId))
            {
                scheduledJobRuleset = _context.ScheduledJobRuleSets.Where(obj => obj.ScheduleJobID == ScheduledJobId).ToList();
            }
            else
            {
                scheduledJobRuleset = _context.ScheduledJobRuleSets.Where(obj => obj.IntermediateID == IntermediateId).ToList();
            }

            if (scheduledJobRuleset.Count > 0)
            {
                foreach (var automationRuleset in scheduledJobRuleset)
                {
                    automationRuleset.ScheduleJobID = ScheduledJobId;
                    automationRuleset.IntermediateID = null;
                    _context.Entry(automationRuleset).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();
            }
        }


        public void DeleteDirectCompetitorRuleSet(long locationBrandID, string DeletedCompanyID)
        {
            long[] DeletedCompanyIDArray = DeletedCompanyID.Split(',').Select(a => Convert.ToInt64(a)).ToArray();

            var FinalRuleSetGroupData = (from rs in _context.RuleSets
                                         join rg in _context.RuleSetGroup on rs.ID equals rg.RuleSetID
                                         join rgc in _context.RuleSetGroupCompanies on rg.ID equals rgc.RuleSetGroupID
                                         join delcomp in DeletedCompanyIDArray on rgc.CompanyID equals delcomp
                                         where rs.LocationBrandID == locationBrandID && !(rs.IsDeleted)
                                         select new
                                         {
                                             RuleSet = rs,
                                             RuleSetGroupCompanyID = rgc.ID,
                                             RuleSetGroupID = rg.ID,
                                             CompanyID = rgc.CompanyID
                                         }).ToList();

            List<RuleSet> FinalRuleSets = (from rs in FinalRuleSetGroupData
                                           select rs.RuleSet).Distinct().ToList();

            List<RuleSetGroupCompany> FinalRulesetGroupCompany = (from rgc in FinalRuleSetGroupData
                                                                  select new RuleSetGroupCompany
                                                                  {
                                                                      ID = rgc.RuleSetGroupCompanyID,
                                                                      CompanyID = rgc.CompanyID
                                                                  }).ToList();

            //Ruleset & RuleSetGroupCompany operation
            if (FinalRulesetGroupCompany.Count() > 0 && FinalRuleSets.Count() > 0)
            {
                //Used to delete RuleSetGroup Company
                foreach (var rulesetgroupcompany in FinalRulesetGroupCompany)
                {
                    _context.Entry(rulesetgroupcompany).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();

                //Final RuleSet selectedCompany update
                foreach (var ruleset in FinalRuleSets)
                {
                    string FinalSelectedCompanyID = "";
                    long[] SelectedRuleSetCompanyID = ruleset.SelectedCompanyIDs.Split(',').Select(a => Convert.ToInt64(a)).ToArray();

                    FinalSelectedCompanyID = string.Join(",", SelectedRuleSetCompanyID.Except(DeletedCompanyIDArray).OrderBy(obj => obj).Distinct().ToArray());

                    //FinalSelectedCompanyID = string.Join(",", (from rs in SelectedRuleSetCompanyID.OrderBy(obj => obj)
                    //                                           join companyArray in DeletedCompanyIDArray.OrderBy(obj => obj) on 1 equals 1
                    //                                           where rs != companyArray
                    //                                           select rs).OrderBy(obj => obj).Distinct().ToArray());

                    if (string.IsNullOrEmpty(FinalSelectedCompanyID))
                    {
                        FinalSelectedCompanyID = null;
                    }
                    ruleset.SelectedCompanyIDs = FinalSelectedCompanyID;
                    _context.Entry(ruleset).State = System.Data.Entity.EntityState.Modified;

                }
                _context.SaveChanges();
            }

            List<long> FinalRuleSetGroupIds = (from rsg in FinalRuleSetGroupData
                                               select rsg.RuleSetGroupID).Distinct().ToList();


            List<long> GetRuleSetGroupCompanyList = (from rsg in _context.RuleSetGroupCompanies
                                                     join rsgid in FinalRuleSetGroupIds on rsg.RuleSetGroupID equals rsgid
                                                     select rsgid).Distinct().ToList();
            // var finalRuleSetGroupList = FinalRuleSetGroupIds.Where(obj => !GetRuleSetGroupCompanyList.Contains(obj)).GroupBy(obj => obj).Select(g => new { g.Key, count = g.Count() });
            List<long> finalRuleSetGroupList = FinalRuleSetGroupIds.Where(obj => !GetRuleSetGroupCompanyList.Contains(obj)).Select(obj => obj).ToList();


            if (finalRuleSetGroupList.Count() > 0 && finalRuleSetGroupList != null)
            {
                //Rulesetgapdelete
                List<RuleSetGapSetting> lstRuleSetGroupGapSettings = (from rsgapsetting in _context.RuleSetGapSettings
                                                                      join rsgid in finalRuleSetGroupList on rsgapsetting.RuleSetGroupID equals rsgid
                                                                      select rsgapsetting).ToList();
                foreach (var rulesetGapSetting in lstRuleSetGroupGapSettings)
                {
                    _context.Entry(rulesetGapSetting).State = System.Data.Entity.EntityState.Deleted;
                }

                //Group delete
                List<RuleSetGroup> rulesetgroup = (from rsg in _context.RuleSetGroup
                                                   join rsgid in finalRuleSetGroupList on rsg.ID equals rsgid
                                                   select rsg).ToList();
                foreach (var rulesetGroupItem in rulesetgroup)
                {
                    _context.Entry(rulesetGroupItem).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }
            _cacheManager.Remove(typeof(RuleSetGroupCompany).ToString());
            _cacheManager.Remove(typeof(RuleSetGapSetting).ToString());
            _cacheManager.Remove(typeof(RuleSetGroup).ToString());
            _cacheManager.Remove(typeof(RuleSet).ToString());
        }
    }
}
