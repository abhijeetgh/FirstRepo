using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public class RuleSetsAppliedService : BaseService<RuleSetsApplied>, IRuleSetsAppliedService
    {
        private IWeekDayService _weekdayService;
        private ICarClassService _carClassService;
        private IRentalLengthService _rentalLengthService;
        private ICompanyService _companyService;

        public RuleSetsAppliedService(IEZRACRateShopperContext context, ICacheManager cacheManager, IWeekDayService weekdayService, ICarClassService carClassService, IRentalLengthService rentalLengthService, ICompanyService companyService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RuleSetsApplied>();
            _cacheManager = cacheManager;
            _weekdayService = weekdayService;
            _carClassService = carClassService;
            _rentalLengthService = rentalLengthService;
            _companyService = companyService;
        }


        public List<LocationBrandModel> GetLocationBrands()
        {
            IQueryable<LocationBrandModel> locationBrands = _context.LocationBrands
                .Join(_context.Locations, Brand => Brand.LocationID, Location => Location.ID, (Brand, Location)
                 => new { LocationID = Brand.LocationID, LocationBrandAlias = Brand.LocationBrandAlias, ID = Brand.ID, LocationCode = Location.Code, LocationIsDeleted = Location.IsDeleted, LocationBrandIsDeleted = Brand.IsDeleted }).Where(a => !a.LocationIsDeleted && !a.LocationBrandIsDeleted)
         .Select(obj => new LocationBrandModel { LocationID = obj.LocationID, LocationBrandAlias = obj.LocationBrandAlias, ID = obj.ID, LocationCode = obj.LocationCode }
                   ).OrderBy(a => a.LocationBrandAlias);

            return locationBrands.ToList<LocationBrandModel>();
        }

        public List<AppliedRuleSetsDTO> GetAppliedRuleSets(long locationBrandId)
        {
            IQueryable<AppliedRuleSetsDTO> appliedRuleSets = _context.RuleSets
                .Join(_context.RuleSetsApplied, RS => RS.ID, RSA => RSA.RuleSetID, (RS, RSA)
                 => new { Id = RSA.ID, RuleSetId = RS.ID, Name = RS.Name, Status = RSA.IsActive, StartDate = RSA.StartDate, EndDate = RSA.EndDate, LocationBrandId = RS.LocationBrandID, RSAIsDeleted = RSA.IsDeleted, RSIsDeleted = RS.IsDeleted, IsWideGap = RS.IsWideGapTemplate, IsGov = RS.IsGov }).Where(a => a.LocationBrandId == locationBrandId && DateTime.Now <= a.EndDate && !a.RSAIsDeleted && !a.RSIsDeleted)
         .Select(obj => new AppliedRuleSetsDTO { Id = obj.Id, RuleSetId = obj.RuleSetId, Name = obj.Name, Status = obj.Status, StartDate = obj.StartDate, EndDate = obj.EndDate, IsWideGap = obj.IsWideGap, IsGOV = obj.IsGov }
                   );

            return appliedRuleSets.OrderBy(a => a.RuleSetId).ThenBy(a => a.StartDate).ToList<AppliedRuleSetsDTO>();
        }

        public void ApplyRuleSets(long rulesetId, DateTime startDate, DateTime endDate, long userId)
        {
            RuleSetsApplied ruleSetApplied = new RuleSetsApplied();
            ruleSetApplied.RuleSetID = rulesetId;
            ruleSetApplied.StartDate = startDate;
            ruleSetApplied.EndDate = endDate;
            ruleSetApplied.CreatedBy = userId;
            ruleSetApplied.CreatedDateTime = DateTime.Now;
            ruleSetApplied.UpdatedBy = userId;
            ruleSetApplied.UpdatedDateTime = DateTime.Now;
            ruleSetApplied.IsActive = true;
            ruleSetApplied.IsDeleted = false;
            Add(ruleSetApplied);
        }

        public List<AppliedRuleSetDetailsDTO> GetAppliedRuleSetDetails(long locationBrandId)
        {
            Dictionary<long, string> weekDays = _weekdayService.GetAll().ToDictionary(a => a.ID, a => a.Day);
            Dictionary<long, string> carClasses = _carClassService.GetAll().Where(a => !a.IsDeleted).ToDictionary(a => a.ID, a => a.Code);
            Dictionary<long, string> rentalLengths = _rentalLengthService.GetAll().ToDictionary(a => a.ID, a => a.Code);
            Dictionary<long, string> companies = _companyService.GetAll().Where(a => !a.IsDeleted).ToDictionary(a => a.ID, a => a.Code);

            var getRuleSetDetailForSelectedBrand = (from ruleSet in _context.RuleSets
                                                    join ruleSetCarClasses in _context.RuleSetCarClasses on ruleSet.ID equals ruleSetCarClasses.RuleSetID
                                                    join ruleSetDaysOfWeeks in _context.RuleSetWeekDay on ruleSet.ID equals ruleSetDaysOfWeeks.RuleSetID
                                                    join ruleSetRentalLengths in _context.RuleSetRentalLength on ruleSet.ID equals ruleSetRentalLengths.RuleSetID
                                                    join ruleSetGroup in _context.RuleSetGroup on ruleSet.ID equals ruleSetGroup.RuleSetID
                                                    join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                                    where ruleSet.LocationBrandID == locationBrandId && !ruleSet.IsDeleted && !(ruleSet.IsCopiedAutomationRuleSet)
                                                    select new { Id = ruleSet.ID, RuleSetName = ruleSet.Name, CarClassId = ruleSetCarClasses.CarClassID, WeekdayId = ruleSetDaysOfWeeks.WeekDayID, RentalLengthId = ruleSetRentalLengths.RentalLengthID, CompanyId = ruleSetGroupCompanies.CompanyID, IsWideGap = ruleSet.IsWideGapTemplate, IsGov = ruleSet.IsGov }).ToList();

            List<AppliedRuleSetDetailsDTO> appliedRuleSetDetails = getRuleSetDetailForSelectedBrand.GroupBy(a => a.Id)
                .Select(a => new
                {
                    Id = a.Key,
                    RuleSetName = a.Select(b => b.RuleSetName).FirstOrDefault(),
                    CarClasses = a.Select(b => b.CarClassId).Distinct()
                    .OrderBy(c => c),
                    WeekDays = a.Select(b => b.WeekdayId).Distinct().OrderBy(c => c),
                    RentalLengths = a.Select(b => b.RentalLengthId).Distinct().OrderBy(c => c),
                    Companies = a.Select(b => b.CompanyId).Distinct().OrderBy(c => c),
                    IsWideGap = a.Select(b => b.IsWideGap).FirstOrDefault(),
                    IsGOV = a.Select(b => b.IsGov).FirstOrDefault()
                }).Select(a => new AppliedRuleSetDetailsDTO
                {
                    Id = a.Id,
                    Name = a.RuleSetName.ToString(),
                    IsWideGap = a.IsWideGap,
                    IsGOV = a.IsGOV,
                    Companies = string.Join(", ", a.Companies.Where(b => companies.ContainsKey(b)).Select(b => companies[b])),
                    DaysOfWeek = string.Join(", ", a.WeekDays.Select(b => weekDays[b])),
                    RentalLengths = string.Join(", ", a.RentalLengths.Select(b => rentalLengths[b])),
                    CarClasses = string.Join(",", a.CarClasses)
                }).OrderBy(a => a.Id).ToList();

            return appliedRuleSetDetails.ToList<AppliedRuleSetDetailsDTO>();
        }
    }
}
