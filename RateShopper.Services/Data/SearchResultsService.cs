using RateShopper.Data;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Data.Entity;
using RateShopper.Domain.Entities;
using RateShopper.Core.Cache;
using System.Web.Script.Serialization;
using RateShopper.Services.Helper;
using RateShopper.Domain.DTOs;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Transactions;
using System.Data.SqlClient;
using RateShopper.Core.Email_Helper;

namespace RateShopper.Services.Data
{
    public class SearchResultsService : BaseService<SearchResult>, ISearchResultsService
    {
        ISearchSummaryService _searchSummaryService;
        ILocationService _locationService;
        ICarClassService _carClassService;
        ICompanyService _companyService;
        ILocationBrandService _locationBrandService;
        ILocationCompanyService _locationCompanyService;
        ISearchResultProcessedDataService _searchResultProcessedData;
        IScrapperSourceService _scrapperSourceService;
        IRentalLengthService _rentalLengthService;

        IRuleSetService _ruleSetService;
        IRuleSetCarClassesService _ruleSetCarClassesService;
        IRuleSetGroupCompanyService _ruleSetGroupCompaniesService;
        IRuleSetGroupService _ruleSetGroupService;
        IRuleSetGapSettingService _ruleSetGapSettingService;
        IRuleSetRentalLengthService _ruleSetRentalLengthService;
        IRuleSetsAppliedService _ruleSetsAppliedService;
        IRuleSetWeekDayService _ruleSetWeekDayService;
        ISearchResultSuggestedRatesService _searchResultSuggestedRatesService;
        IStatusesService _statusService;
        IWeekDayService _weekDayService;
        IUserScrapperSourcesService _userScrapperSourcesService;
        IRangeIntervalsService _rangeIntervalsService;
        IUserLocationBrandsService _userLocationBrandsService;
        IGlobalLimitDetailService _globalLimitDetailService;
        IScheduledJobService _scheduledJobService;
        IScheduledJobMinRatesService _scheduledJobMinRatesService;
        IFormulaService _formulaService;
        ITSDTransactionsService _tSDTransactionService;
        IUserService _userService;
        IGlobalTetherSettingService _globalTetherService;
        IQuickViewResultsService _quickViewResultService;
        IPostJSONLogService _postJSONLogService;
        IScheduledJobOpaqueValuesService _scheduledJobOpaqueValuesService;
        IRateCodeService _rateCodeService;
        ILocationBrandRentalLengthService _locationBrandRentalLengthService;

        public SearchResultsService(IEZRACRateShopperContext context, ICacheManager cacheManager, ISearchSummaryService searchSummary,
            ILocationService location, ICarClassService carClass, ICompanyService company, ILocationBrandService locationBrand,
            ILocationCompanyService locationCompany, ISearchResultProcessedDataService searchResultProcessedData, ISearchResultSuggestedRatesService searchResultSuggestedRates,
            IRuleSetService ruleSet,
            IRuleSetCarClassesService ruleSetCarClasses, IRuleSetGroupCompanyService ruleSetGroupCompany, IRuleSetGroupService ruleSetGroup, IRuleSetDefaultSettingService ruleSetDefaultSetting,
            IRuleSetGapSettingService ruleSetGapSetting, IRuleSetRentalLengthService ruleSetRentalLength, IRuleSetsAppliedService ruleSetsApplied,
            IRuleSetWeekDayService ruleSetWeekDay,
             IRentalLengthService rentalLength,
             IScrapperSourceService scrapperSourceService, IRentalLengthService rentalLengthService, IStatusesService statusService,
            IWeekDayService weekDayService, IUserScrapperSourcesService userScrapperSourcesService, IRangeIntervalsService rangeIntervalsService,
            IUserLocationBrandsService userLocationBrandsService, IGlobalLimitDetailService globalLimitDetailService, IFormulaService formulaService,
            IScheduledJobService scheduledJobService, IScheduledJobMinRatesService scheduledJobMinRatesService, ITSDTransactionsService tSDTransactionService,
            IUserService userService, IGlobalTetherSettingService globalTetherService, IQuickViewResultsService quickViewResultService, IPostJSONLogService postJSONLogService, IScheduledJobOpaqueValuesService scheduledJobOpaqueValuesService,
            IRateCodeService rateCodeService, ILocationBrandRentalLengthService locationBrandRentalLengthService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<SearchResult>();
            _cacheManager = cacheManager;
            _searchSummaryService = searchSummary;
            _locationService = location;
            _carClassService = carClass;
            _companyService = company;
            _locationBrandService = locationBrand;
            _locationCompanyService = locationCompany;
            _searchResultProcessedData = searchResultProcessedData;
            _scrapperSourceService = scrapperSourceService;
            _rentalLengthService = rentalLengthService;

            _ruleSetService = ruleSet;
            _ruleSetCarClassesService = ruleSetCarClasses;
            _ruleSetGroupCompaniesService = ruleSetGroupCompany;

            _ruleSetGapSettingService = ruleSetGapSetting;
            _ruleSetRentalLengthService = ruleSetRentalLength;
            _ruleSetsAppliedService = ruleSetsApplied;
            _ruleSetWeekDayService = ruleSetWeekDay;
            _ruleSetGroupService = ruleSetGroup;
            _searchResultSuggestedRatesService = searchResultSuggestedRates;
            _statusService = statusService;
            _weekDayService = weekDayService;
            _userScrapperSourcesService = userScrapperSourcesService;
            _rangeIntervalsService = rangeIntervalsService;
            _userLocationBrandsService = userLocationBrandsService;
            _globalLimitDetailService = globalLimitDetailService;
            _formulaService = formulaService;
            _scheduledJobService = scheduledJobService;
            _scheduledJobMinRatesService = scheduledJobMinRatesService;
            _tSDTransactionService = tSDTransactionService;
            _userService = userService;
            _globalTetherService = globalTetherService;
            _quickViewResultService = quickViewResultService;
            _postJSONLogService = postJSONLogService;
            _scheduledJobOpaqueValuesService = scheduledJobOpaqueValuesService;
            _rateCodeService = rateCodeService;
            _locationBrandRentalLengthService = locationBrandRentalLengthService;
        }

        private string garsFee = string.Empty;
        public async Task<string> GenerateSeachResultProcesssedData(long searchSummaryID)
        {
            bool isQuickView = false;
            try
            {
                #region Sample JSON to be Generated for Daily View
                /*
     var data = {  
                   finalData:{  
                      "HeaderInfo":[  
                         {  
                            "CompanyID":"123",
                            "CompanyName":"company1",
                            "Logo":"https://s3.amazonaws.com/ez-web-images/EZ.png",
                            "Inside":true
                         },
                         {  
                            "CompanyID":"123",
                            "CompanyName":"company2",
                            "Logo":"https://s3.amazonaws.com/ez-web-images/ZT.png",
                            "Inside":false
                         },
                         {  
                            "CompanyID":"123",
                            "CompanyName":"company3",
                            "Logo":"https://s3.amazonaws.com/ez-web-images/ZR.png",
                            "Inside":true
                         },
                         {  
                            "CompanyID":"123",
                            "CompanyName":"company4",
                            "Logo":"https://s3.amazonaws.com/ez-web-images/SX.png",
                            "Inside":true
                         }
                      ],
                      "RatesInfo":[  
                         {  
                            "CarClassID":"1",
                            "CarClass":"ECAR",
                            "CarClassLogo":"https://s3.amazonaws.com/ez-web-images/ECAR.png",
                            "CompanyDetails":[  
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company1",
                                  "TotalValue":22.22,
                                  "BaseValue":44.2,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":true
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company2",
                                  "TotalValue":33.22,
                                  "BaseValue":12.32,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company3",
                                  "TotalValue":12.12,
                                  "BaseValue":45.85,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company4",
                                  "TotalValue":56.22,
                                  "BaseValue":66.66,
                                  "Islowest":true,
                                  "IslowestAmongCompetitors":false
                               }
                            ]
                         },
                         {  
                            "CarClassID":"2",
                            "CarClass":"MCAR",
                            "CarClassLogo":"https://s3.amazonaws.com/ez-web-images/CCAR.png",
                            "CompanyDetails":[  
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company1",
                                  "TotalValue":22.22,
                                  "BaseValue":44.2,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":true
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company2",
                                  "TotalValue":33.22,
                                  "BaseValue":12.32,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company3",
                                  "TotalValue":12.12,
                                  "BaseValue":45.85,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company4",
                                  "TotalValue":56.22,
                                  "BaseValue":66.66,
                                  "Islowest":true,
                                  "IslowestAmongCompetitors":false
                               }
                            ]
                         },
                         {  
                            "CarClassID":"3",
                            "CarClass":"SCAR",
                            "CarClassLogo":"https://s3.amazonaws.com/ez-web-images/ICAR.png",
                            "CompanyDetails":[  
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company1",
                                  "TotalValue":22.22,
                                  "BaseValue":44.2,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":true
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company2",
                                  "TotalValue":33.22,
                                  "BaseValue":12.32,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company3",
                                  "TotalValue":12.12,
                                  "BaseValue":45.85,
                                  "Islowest":false,
                                  "IslowestAmongCompetitors":false
                               },
                               {  
                                  "CompanyID":"123",
                                  "CompanyName":"company4",
                                  "TotalValue":56.22,
                                  "BaseValue":66.66,
                                  "Islowest":true,
                                  "IslowestAmongCompetitors":false
                               }
                            ]
                         }
                      ]
                   },
                   suggestedRate:[  
                      {  
                         "CarClassID":"1",
                         "RuleSetID":"12",
                         "RuleSetName":"First Rule",
                         "BaseEdit":"12",
                         "TotalEdit":"12"
                      },
                      {  
                         "CarClassID":"2",
                         "RuleSetID":"23",
                         "RuleSetName":"Second Rule",
                         "BaseEdit":"23",
                         "TotalEdit":"23"
                      },
                      {  
                         "CarClassID":"3",
                         "RuleSetID":"13",
                         "RuleSetName":"Third Rule3",
                         "BaseEdit":"13",
                         "TotalEdit":"13"
                      }
                   ]
                }
          
             */
                #endregion

                #region Sample JSON to be Generated for Classic View
                /*
                var data = 
                      {
                        finalData: {
                            "BrandID": 1,
                            "BrandCode": "EZ",
                            "CarClassID": 12,
                            "RatesInfo":
                                [
                                    {
                                        "DateInfo": "08-15-15 Sunday",
                                        "CompanyDetails":
                                            [
                                                { "CompanyID": 22, "CompanyCode": "AD", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "AD", "BaseValue": 22, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "HZ", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                { "CompanyID": 1, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                            ]
                                    },
                                     {
                                         "DateInfo": "06-15-15 Sunday",
                                         "CompanyDetails":
                                             [
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 2, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                 { "CompanyID": 1, "CompanyCode": "EZ", "BaseValue": 3, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 3, "TotalValue": 52.23 },
                                                 { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                             ]
                                     },
                                      {
                                          "DateInfo": "06-15-15 Sunday",
                                          "CompanyDetails":
                                              [
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 1, "CompanyCode": "EZ", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 3, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 3, "TotalValue": 52.23 },
                                                  { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                              ]
                                      },
                                       {
                                           "DateInfo": "07-15-15 Sunday",
                                           "CompanyDetails":
                                               [
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 1, "CompanyCode": "EZ", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 33, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 3, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                   { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                               ]
                                       },
                                        {
                                            "DateInfo": "04-15-15 Sunday",
                                            "CompanyDetails":
                                                [
                                                    { "CompanyID": 1, "CompanyCode": "EZ", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 4, "CompanyCode": "SD", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                    { "CompanyID": 22, "CompanyCode": "FF", "BaseValue": 30, "TotalValue": 52.23 },
                                                ]
                                        },
                                ]
                        }
                    }
                */
                #endregion

                //get all records mathing seachsummaryID from Searhresults
                List<SearchResult> searchResultsforSeachSummaryID = GetBySeachSummaryID(searchSummaryID);
                //get carclassID for MCAR
                long carClassIDForMCar = _carClassService.GetCarClassIDByName(ConfigurationManager.AppSettings["DefaultCarClassForGridSorting"]);

                //Get data from seachSummary table for the search (scrapper sources, location, rental lengths, dates)
                SearchSummary searchSummary = _searchSummaryService.GetById(searchSummaryID, false);
                if (searchSummary == null || searchSummary.ID <= 0)
                {
                    return string.Empty;
                }
                //check gov shop/job
                bool isGov = searchSummary.IsGov.HasValue ? searchSummary.IsGov.Value : false;

                //check scheduled job/ automation job
                bool isScheduledJob = false;
                bool isApplyWideGapTemplate = false;
                bool isApplyGovTemplate = false;
                List<long> ignoreRuleSets = new List<long>();
                ScheduledJob scheduledJob = null;
                List<ScheduledJobRuleSets> scheduledJobRuleSets = new List<ScheduledJobRuleSets>();
                bool isSummaryShop = false;
                bool isFTBScheduledJob = (searchSummary.FTBScheduledJobID.HasValue && searchSummary.FTBScheduledJobID.Value > 0) ? true : false;
                bool isBaseRuleset = false;

                //create new dummy shop in case of Automation Console to generate summary report
                SearchSummary newSummaryForAutomation = null;

                isSummaryShop = (!string.IsNullOrEmpty(searchSummary.ShopType) && searchSummary.ShopType.Equals("SummaryShop", StringComparison.OrdinalIgnoreCase)) ? true : false;

                if (searchSummary.ScheduledJobID.HasValue && searchSummary.ScheduledJobID.Value > 0)
                {
                    scheduledJob = _scheduledJobService.GetById(searchSummary.ScheduledJobID.Value, false);

                    if (scheduledJob == null)
                    {
                        return string.Empty;
                    }


                    if (!isSummaryShop)
                    {
                        DateTime shopStartDate = DateTime.Parse(scheduledJob.StartDate.Value.ToString("yyyy-MM-dd") + " " + scheduledJob.PickUpTime);
                        DateTime shopEndDate = DateTime.Parse(scheduledJob.EndDate.Value.ToString("yyyy-MM-dd") + " " + scheduledJob.DropOffTime);
                        if (scheduledJob.StartDate.Value < DateTime.Now.Date)
                        {
                            DateTime todaysDate = DateTime.Now;
                            shopStartDate = DateTime.Parse(todaysDate.ToString("yyyy-MM-dd") + " " + scheduledJob.PickUpTime);
                            shopEndDate = DateTime.Parse(scheduledJob.EndDate.Value.ToString("yyyy-MM-dd") + " " + scheduledJob.DropOffTime);
                        }

                        newSummaryForAutomation = new SearchSummary()
                        {
                            ScrapperSourceIDs = searchSummary.ScrapperSourceIDs,
                            LocationBrandIDs = searchSummary.LocationBrandIDs,
                            CarClassesIDs = searchSummary.CarClassesIDs,
                            RentalLengthIDs = searchSummary.RentalLengthIDs,
                            StartDate = shopStartDate,
                            EndDate = shopEndDate,
                            StatusID = searchSummary.StatusID,
                            RetryCount = searchSummary.RetryCount,
                            CreatedBy = searchSummary.CreatedBy,
                            UpdatedBy = searchSummary.CreatedBy,
                            CreatedDateTime = DateTime.Now,
                            UpdatedDateTime = DateTime.Now,
                            RequestURL = searchSummary.RequestURL,
                            Response = searchSummary.Response,
                            ProviderId = searchSummary.ProviderId,
                            IsGov = Convert.ToBoolean(searchSummary.IsGov),
                            ShopType = ShopTypes.SummaryShop,
                            ScheduledJobID = searchSummary.ScheduledJobID > 0 ? searchSummary.ScheduledJobID : (long?)null,
                            FTBScheduledJobID = searchSummary.FTBScheduledJobID > 0 ? searchSummary.FTBScheduledJobID : (long?)null
                        };
                        _searchSummaryService.Add(newSummaryForAutomation);
                    }

                    isScheduledJob = true;
                    isApplyWideGapTemplate = scheduledJob.IsWideGapTemplate;
                    isApplyGovTemplate = scheduledJob.IsGovTemplate.HasValue ? scheduledJob.IsGovTemplate.Value : false;
                    isBaseRuleset = scheduledJob.CompeteOnBase;
                    //if this is an automation job and user have customized the ruleset for this job, then we hve to apply all the customized rulsets
                    //for this job and all unassigned wideGap templates
                    //if this is an GOV automation job and user have customized the ruleset for this job, then we have to apply all the customized rulsets
                    if (isApplyWideGapTemplate || isApplyGovTemplate)
                    {
                        ignoreRuleSets = GetIngnoredRulsets(scheduledJob.ID, out scheduledJobRuleSets);
                    }

                }
                //if it is scheduled job then create ignore list else do not but set isApplyGOV to true to apply only GOV ruleset to GOV shop
                else
                {
                    if (isGov)
                        isApplyGovTemplate = true;
                }
                //check if current search is quickview
                string quickViewCompetitors = string.Empty;
                string trackingCarClassIds = string.Empty;
                List<SearchResult> previousSearchResults = null;
                long[] quickViewCompetitorIds = null;
                long[] carClassToCompete = null;
                QuickView quickViewRow = null;
                bool monitorBase = false;
                IEnumerable<QuickViewGroupCompaniesDTO> quickViewGroupCompanies = null;
                IEnumerable<QuickViewGapDevSettingsDTO> quickViewGapDevSettings = null;
                IEnumerable<QuickViewCarClassGroups> quickViewCarClassGroups = null;
                SearchResult dominantBrandNewResult = null;
                SearchResult dominantBrandOldResult = null;
                decimal? domBrandNewTotalRate = null;
                decimal? domBrandNewBaseRate = null;
                decimal? domBrandOldTotalRate = null;
                decimal? domBrandOldBaseRate = null;
                string shopStartEndDate = string.Empty;
                List<CarClassCodeOrderDTO> carClassChanged = null;
                if (searchSummary.IsQuickView.HasValue && searchSummary.IsQuickView.Value)
                {
                    isQuickView = true;
                    quickViewRow = _context.QuickView.Where(quick => quick.IsEnabled && !quick.IsDeleted && quick.ChildSearchSummaryId == searchSummary.ID)
                        .Select(quick => quick).FirstOrDefault();
                    if (quickViewRow != null)
                    {
                        //fetch comma seperated competitors
                        quickViewCompetitors = !string.IsNullOrEmpty(quickViewRow.CompetitorCompanyIds) ? quickViewRow.CompetitorCompanyIds : string.Empty;
                        long parentSearchSummaryId = quickViewRow.ParentSearchSummaryId;
                        previousSearchResults = GetBySeachSummaryID(parentSearchSummaryId);
                        //fetch car classes which are used for tracking
                        trackingCarClassIds = !string.IsNullOrEmpty(quickViewRow.CarClassIds) ? quickViewRow.CarClassIds : string.Empty;

                        if (!string.IsNullOrEmpty(trackingCarClassIds))
                        {
                            carClassToCompete = Common.StringToLongList(trackingCarClassIds).ToArray();
                        }

                        if (_context.QuickviewCarClassGroup != null)
                        {
                            quickViewCarClassGroups = await (from quickViewCarGrp in _context.QuickviewCarClassGroup
                                                             join cartoCompete in carClassToCompete on quickViewCarGrp.CarClassId equals cartoCompete
                                                             select new QuickViewCarClassGroups
                                                             {
                                                                 GroupID = quickViewCarGrp.GroupId,
                                                                 CarClassID = quickViewCarGrp.CarClassId
                                                             }).ToListAsync();
                        }

                        monitorBase = quickViewRow.MonitorBase;

                        if (_context.QuickViewGroups != null && _context.QuickViewGroupCompanies != null && _context.QuickViewGapDevSettings != null)
                        {
                            quickViewGroupCompanies = await (from qvgrp in _context.QuickViewGroups
                                                             join qvcomp in _context.QuickViewGroupCompanies on qvgrp.ID equals qvcomp.GroupId
                                                             where qvgrp.QuickViewId == quickViewRow.ID
                                                             select new QuickViewGroupCompaniesDTO { GroupId = qvcomp.GroupId, CompanyId = qvcomp.CompanyId }).ToListAsync();

                            quickViewGapDevSettings = await (from qvgrp in _context.QuickViewGroups
                                                             join qvgapdev in _context.QuickViewGapDevSettings on qvgrp.ID equals qvgapdev.CompetitorGroupId
                                                             where qvgrp.QuickViewId == quickViewRow.ID
                                                             select new QuickViewGapDevSettingsDTO
                                                             {
                                                                 CompetitorGroupId = qvgapdev.CompetitorGroupId,
                                                                 CarClassGroupId = qvgapdev.CarClassGroupId,
                                                                 //GapValue = qvgapdev.GapValue,
                                                                 DeviationValue = qvgapdev.DeviationValue
                                                             }).ToListAsync();
                        }

                        if (quickViewGroupCompanies != null && quickViewGroupCompanies.Count() > 0)
                        {
                            quickViewCompetitorIds = quickViewGroupCompanies.Select(quick => quick.CompanyId).ToArray();
                        }

                        //step1: filter serarchresults based on quickViewCompetitorIds 
                        //if (previousSearchResults != null)
                        //{
                        //    previousSearchResults = previousSearchResults.Where(search => quickViewCompetitorIds.Any(competior => competior.Equals(search.CompanyID))).Select(search => search).ToList();
                        //}
                        shopStartEndDate = searchSummary.StartDate.ToString("MM-dd-yyyy") + " to " + searchSummary.EndDate.ToString("MM-dd-yyyy");
                        carClassChanged = new List<CarClassCodeOrderDTO>();
                    }
                }
                //end

                //get all companies            
                List<Company> allCompanies = _companyService.GetAll().Where(obj => !obj.IsDeleted).ToList();

                //get all scrapper sources
                long[] scrapperSourcesIDs = Common.StringToLongList(searchSummary.ScrapperSourceIDs).ToArray();

                //get all car classes
                long[] carClassIDs = Common.StringToLongList(searchSummary.CarClassesIDs).ToArray();

                //get all dates                
                DateTime[] arrivalDates = Enumerable.Range(0, 1 + searchSummary.EndDate.Date.Subtract(searchSummary.StartDate.Date).Days)
                    .Select(offset => searchSummary.StartDate.AddDays(offset)).ToArray();

                if (scrapperSourcesIDs == null || scrapperSourcesIDs.Length == 0 || arrivalDates == null || arrivalDates.Length <= 0)
                {
                    return string.Empty;
                }

                //get all rental lengths
                long[] rentalLengthIDs = Common.StringToLongList(searchSummary.RentalLengthIDs).ToArray();
                List<RentalLength> rentalLengths = _rentalLengthService.GetRentalLength().ToList();// _rentalLengthService.GetAll().ToList();

                //get all locationsBrands
                long[] locationBrandIDs = Common.StringToLongList(searchSummary.LocationBrandIDs).ToArray();

                List<LocationCompany> allLocationCompanies = _locationCompanyService.GetAll().ToList();
                List<WeekDay> allWeekDays = _weekDayService.GetAll().ToList();
                List<Formula> allFormula = _formulaService.GetAll().ToList();
                List<CarClass> allCarClasses = _carClassService.GetAll().Where(obj => !obj.IsDeleted).OrderBy(car => car.DisplayOrder).ToList();

                //if type of shop is not summary shop then create JSON data across date lor
                if (!isSummaryShop)
                {
                    //foreach LocationBrand ScrapperSource, Date, RentalLength, Create Sepatate JSON
                    foreach (long locationBrandID in locationBrandIDs)
                    {
                        LocationBrand locationBrand = _context.LocationBrands.FirstOrDefault(obj => obj.ID == locationBrandID);

                        //get all RuleSets for this locationBrand


                        var allRuleSetForLocationBrand = GetAllRuleSetsForLocationBrand(locationBrandID, searchSummary.StartDate.Date, searchSummary.EndDate.Date, isApplyWideGapTemplate, scheduledJobRuleSets, isApplyGovTemplate, isScheduledJob);

                        List<GlobalLimitDataDTO> globalLimitData = _globalLimitDetailService.GetGlobalLimitData(locationBrand.ID);

                        foreach (long scrapperSourceID in scrapperSourcesIDs)
                        {

                            foreach (long rentalLengthID in rentalLengthIDs)
                            {
                                List<SearchViewModel.RatesInfo> dailyViewRatesInfolist = new List<SearchViewModel.RatesInfo>();
                                foreach (DateTime arrivalDate in arrivalDates)
                                {
                                    #region processing
                                    //filter result based on filter criteria of daily view
                                    List<SearchResult> filteredSearchResults = searchResultsforSeachSummaryID.Where(obj => obj.ScrapperSourceID == scrapperSourceID && obj.ArrivalDate.Date == arrivalDate.Date && obj.RentalLengthID == rentalLengthID
                                    && obj.LocationID == locationBrand.LocationID).ToList();

                                    if (filteredSearchResults == null || filteredSearchResults.Count == 0)
                                    {
                                        if (isQuickView)
                                        {
                                            //if no data found for particular date and lor insert record for lor and date in quickviewResults Table
                                            _quickViewResultService.Add(new QuickViewResults
                                            {
                                                SearchSummaryId = searchSummary.ID,
                                                QuickViewId = quickViewRow.ID,
                                                ScrapperSourceId = scrapperSourceID,
                                                LocationBrandId = locationBrandID,
                                                RentalLengthId = rentalLengthID,
                                                Date = arrivalDate.Date,
                                                IsMovedUp = (bool?)null,
                                                IsPositionChange = false
                                            });

                                        }
                                        continue;
                                    }

                                    //get global limits for the location-brand and date
                                    List<GlobalLimitDetail> globalLimitDetails = _globalLimitDetailService.GetGlobalLimitDetails(globalLimitData, arrivalDate.Date);

                                    //if Automation job/ scheduled job then get automation global limit
                                    List<ScheduledJobMinRates> globalLimitDetailsScheduled = null;
                                    if (isScheduledJob)
                                    {
                                        globalLimitDetailsScheduled = _scheduledJobMinRatesService.GetSelectedMinRate(scheduledJob.ID);
                                    }
                                    //get companies IDs based on MCAR rates
                                    //Scenario 1: Only once car class is selected then order the grid based on selected car class.
                                    //Scenario 2: More than one car class is selected but MIDCAR is not selected then add midcar in criteria
                                    long[] companyIDsBasedOnMCarRates = null;
                                    if (filteredSearchResults.Exists(obj => obj.CarClassID == carClassIDForMCar))
                                    {
                                        List<long> companyIdsBasedOnMCar = filteredSearchResults.Where(obj => obj.CarClassID == carClassIDForMCar).OrderBy(obj => obj.TotalRate).Select(obj => obj.CompanyID).ToList();
                                        var remainingCompanies = filteredSearchResults.Where(obj => !companyIdsBasedOnMCar.Contains(obj.CompanyID));
                                        if (remainingCompanies.Count() > 0)
                                        {
                                            companyIdsBasedOnMCar.AddRange(remainingCompanies.Select(obj => obj.CompanyID).ToList());
                                        }

                                        companyIDsBasedOnMCarRates = companyIdsBasedOnMCar.Distinct().ToArray();
                                    }
                                    else
                                    {

                                        //long minCarClass = filteredSearchResults.Min(obj => obj.CarClassID);
                                        long minCarClass = allCarClasses.FirstOrDefault(car => filteredSearchResults.Exists(result => result.CarClassID == car.ID)).ID;
                                        //companyIDsBasedOnMCarRates = filteredSearchResults.Where(obj => obj.CarClassID == minCarClass).OrderBy(obj => obj.TotalRate).Select(obj => obj.CompanyID).ToArray();


                                        List<long> companyIdsBasedOnMinCarClass = filteredSearchResults.Where(obj => obj.CarClassID == minCarClass).OrderBy(obj => obj.TotalRate).Select(obj => obj.CompanyID).ToList();
                                        var remainingCompanies = filteredSearchResults.Where(obj => !companyIdsBasedOnMinCarClass.Contains(obj.CompanyID));
                                        if (remainingCompanies.Count() > 0)
                                        {
                                            companyIdsBasedOnMinCarClass.AddRange(remainingCompanies.Select(obj => obj.CompanyID).ToList());
                                        }

                                        companyIDsBasedOnMCarRates = companyIdsBasedOnMinCarClass.Distinct().ToArray();

                                    }
                                    //set dummy field for ordering the company based on MCAR rate
                                    filteredSearchResults.ForEach(obj => obj.CompanyRankBasedOnMCcarRate = Array.IndexOf(companyIDsBasedOnMCarRates, obj.CompanyID));

                                    //generate JSON
                                    #region generate headers
                                    SearchViewModel.FinalData finalData = new SearchViewModel.FinalData();
                                    finalData.HeaderInfo = new List<SearchViewModel.HeaderInfo>();
                                    finalData.RatesInfo = new List<SearchViewModel.RatesInfo>();

                                    foreach (long companyID in companyIDsBasedOnMCarRates)
                                    {
                                        Company company = allCompanies.Where(obj => obj.ID == companyID).FirstOrDefault();

                                        LocationCompany locationCompany = allLocationCompanies.FirstOrDefault(obj => obj.LocationID == locationBrand.LocationID && obj.CompanyID == companyID);

                                        //If company is not configured in company settings then remove all records for the company from
                                        //search results.
                                        if (company == null || company.ID <= 0)// || locationCompany == null || locationCompany.CompanyID <= 0)
                                        {
                                            filteredSearchResults.RemoveAll(obj => obj.CompanyID == companyID);
                                            companyIDsBasedOnMCarRates = companyIDsBasedOnMCarRates.Where(obj => obj != companyID).ToArray();
                                            continue;
                                        }

                                        //add headerinfo object to main list.
                                        finalData.HeaderInfo.Add(new SearchViewModel.HeaderInfo
                                        {
                                            CompanyID = company.ID.ToString(),
                                            CompanyName = company.Name,
                                            Inside = (locationCompany != null && locationCompany.CompanyID > 0) ? locationCompany.IsTerminalInside : (bool?)null,
                                            Logo = company.Logo
                                        });
                                    }
                                    #endregion
                                    #region generate rateInfo
                                    foreach (CarClass carClass in allCarClasses.Where(obj => carClassIDs.Contains(obj.ID)))
                                    {
                                        //Generate applied ruleset and suggested rates

                                        SearchViewModel.RatesInfo rateInformation = new SearchViewModel.RatesInfo();
                                        rateInformation.CarClassID = carClass.ID;
                                        rateInformation.CarClass = carClass.Code;
                                        rateInformation.CarClassLogo = carClass.Logo;

                                        rateInformation.CompanyDetails = new List<SearchViewModel.CompanyDetail>();

                                        //step2: create a list from step1 which is filter resultst from step1 for scrapper, LOR, date, carclass
                                        List<SearchResult> filteredPreviousResult = null;
                                        if (isQuickView)
                                        {
                                            filteredPreviousResult = previousSearchResults.Where(search => search.RentalLengthID == rentalLengthID &&
                                                  search.ArrivalDate.Date == arrivalDate.Date && search.ScrapperSourceID == scrapperSourceID && search.CarClassID == carClass.ID)
                                                  .Select(search => search).ToList();

                                            dominantBrandNewResult = searchResultsforSeachSummaryID.Where(company => company.CompanyID == locationBrand.BrandID
                                                && company.CarClassID == carClass.ID && company.RentalLengthID == rentalLengthID && company.ArrivalDate == arrivalDate.Date).FirstOrDefault();

                                            dominantBrandOldResult = previousSearchResults.Where(result => result.CompanyID == locationBrand.BrandID
                                                && result.CarClassID == carClass.ID && result.RentalLengthID == rentalLengthID && result.ArrivalDate == arrivalDate.Date).FirstOrDefault();

                                            if (dominantBrandNewResult != null)
                                            {
                                                domBrandNewBaseRate = dominantBrandNewResult.BaseRate;
                                                domBrandNewTotalRate = dominantBrandNewResult.TotalRate;
                                            }
                                            else
                                            {
                                                domBrandNewBaseRate = null; domBrandNewTotalRate = null;
                                            }

                                            if (dominantBrandOldResult != null)
                                            {
                                                domBrandOldBaseRate = dominantBrandOldResult.BaseRate;
                                                domBrandOldTotalRate = dominantBrandOldResult.TotalRate;
                                            }
                                            else
                                            {
                                                domBrandOldBaseRate = null; domBrandOldTotalRate = null;
                                            }
                                        }

                                        //TODO: Check significance of => .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate)             
                                        List<SearchViewModel.CompanyDetail> companyDetails = new List<SearchViewModel.CompanyDetail>();


                                        //In Quickview enhancements we removed Quickview feature for GOV shops changed If--else block based on monitor base flag

                                        if (!monitorBase)
                                        {
                                            companyDetails = filteredSearchResults.Where(obj => obj.CarClassID == carClass.ID)
                                               .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate)
                                                   .Select(obj => new SearchViewModel.CompanyDetail()
                                                   {
                                                       CompanyID = obj.CompanyID,
                                                       CompanyName = allCompanies.First(obj1 => obj1.ID == obj.CompanyID).Name,
                                                       BaseValue = obj.BaseRate.HasValue ? decimal.Round(obj.BaseRate.Value, MidpointRounding.AwayFromZero) : (decimal?)null,
                                                       TotalValue = obj.TotalRate.HasValue ? obj.TotalRate.Value : (decimal?)null,
                                                       //step3: set isMoveUp field based on company exist in step2 filtered for companyid. Compare with prev value with current scraped rates based on deviation value and set the flag
                                                       IsMovedUp = isQuickView ? ((quickViewCompetitorIds.Contains(obj.CompanyID) && carClassToCompete.Contains(obj.CarClassID))
                                                       ? IsMovedUp(obj.CompanyID, obj.CarClassID, obj.TotalRate, filteredPreviousResult, quickViewGroupCompanies, quickViewCarClassGroups, quickViewGapDevSettings,
                                                       domBrandOldTotalRate, domBrandNewTotalRate, monitorBase) : null) : null,
                                                       Islowest = false,
                                                       IslowestAmongCompetitors = false,
                                                       CompanyRankBasedOnMCcarRate = obj.CompanyRankBasedOnMCcarRate,
                                                       IsPositionChanged = isQuickView ? ((quickViewCompetitorIds.Contains(obj.CompanyID) && carClassToCompete.Contains(obj.CarClassID))
                                                       ? IsPositionChanged(obj.CompanyID, obj.CarClassID, obj.TotalRate, filteredPreviousResult, monitorBase, domBrandOldTotalRate, domBrandNewTotalRate) : false) : false
                                                   }).Distinct(new CompanyDetailsComparare(a => a.CompanyID)).ToList();
                                        }
                                        else
                                        {
                                            companyDetails = filteredSearchResults.Where(obj => obj.CarClassID == carClass.ID)
                                               .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate)
                                                   .Select(obj => new SearchViewModel.CompanyDetail()
                                                   {
                                                       CompanyID = obj.CompanyID,
                                                       CompanyName = allCompanies.First(obj1 => obj1.ID == obj.CompanyID).Name,
                                                       BaseValue = obj.BaseRate.HasValue ? decimal.Round(obj.BaseRate.Value, MidpointRounding.AwayFromZero) : (decimal?)null,
                                                       TotalValue = obj.TotalRate.HasValue ? obj.TotalRate.Value : (decimal?)null,
                                                       //step3: set isMoveUp field based on company exist in step2 filtered for companyid. Compare with prev value and set the flag
                                                       IsMovedUp = isQuickView ? ((quickViewCompetitorIds.Contains(obj.CompanyID) && carClassToCompete.Contains(obj.CarClassID))
                                                       ? IsMovedUp(obj.CompanyID, obj.CarClassID, obj.BaseRate, filteredPreviousResult, quickViewGroupCompanies, quickViewCarClassGroups, quickViewGapDevSettings,
                                                        domBrandOldBaseRate, domBrandNewBaseRate, monitorBase) : null) : null,
                                                       Islowest = false,
                                                       IslowestAmongCompetitors = false,
                                                       CompanyRankBasedOnMCcarRate = obj.CompanyRankBasedOnMCcarRate,
                                                       IsPositionChanged = isQuickView ? ((quickViewCompetitorIds.Contains(obj.CompanyID) && carClassToCompete.Contains(obj.CarClassID))
                                                       ? IsPositionChanged(obj.CompanyID, obj.CarClassID, obj.BaseRate, filteredPreviousResult, monitorBase, domBrandOldBaseRate, domBrandNewBaseRate) : false) : false
                                                   }).Distinct(new CompanyDetailsComparare(a => a.CompanyID)).ToList();
                                        }

                                        string rentalLengthCode = rentalLengths.First(obj => obj.ID == rentalLengthID).Code;
                                        string rangeInterval = string.Empty;
                                        rangeInterval = rentalLengthCode.Substring(0, 1);

                                        decimal minBaseRate = 0, maxBaseRate = 0;
                                        bool minMaxRateFound = false;

                                        if (isScheduledJob)
                                        {
                                            minMaxRateFound = SetMinMaxRatesAutomation(globalLimitDetailsScheduled, rangeInterval, carClass.ID, arrivalDate.Date, out minBaseRate, out maxBaseRate);
                                        }

                                        //Set min max rate; In Scheduled job, if min rate not found then apply min rate from global limit
                                        if (!minMaxRateFound)
                                        {
                                            minMaxRateFound = SetMinMaxRates(globalLimitDetails, rangeInterval, carClass.ID, out minBaseRate, out maxBaseRate);
                                        }

                                        //Set Islowest flag                  
                                        if (companyDetails.Count > 0)
                                        {
                                            //decimal minTotalValue = companyDetails.Where(obj2 => obj2.TotalValue.HasValue).Min(obj => obj.TotalValue.Value);
                                            //get minimum base value in case of GOV shop for all other shops(regular/Automation) get minimum total value.
                                            decimal minTotalValue = 0;
                                            if (!isGov && !isBaseRuleset)
                                            {
                                                minTotalValue = companyDetails.Where(obj2 => obj2.TotalValue.HasValue).Min(obj => obj.TotalValue.Value);
                                                companyDetails.Where(obj => obj.TotalValue == minTotalValue).All(obj2 => obj2.Islowest = true);
                                            }
                                            else
                                            {
                                                minTotalValue = companyDetails.Where(obj2 => obj2.BaseValue.HasValue).Min(obj => obj.BaseValue.Value);
                                                companyDetails.Where(obj => obj.BaseValue == minTotalValue).All(obj2 => obj2.Islowest = true);
                                            }

                                            #region Suggested Rate AND lowest amoung competitor rate

                                            //get rule set based on locationBrand, Date, rental length and car class
                                            RuleSet ruleSetApplied = GetAppliedRuleSet(allRuleSetForLocationBrand, allWeekDays, arrivalDate.Date, rentalLengthID, carClass.ID, ignoreRuleSets);

                                            //Check locationBrandCarClasses and appliedRuleset exist
                                            //bool isValidLocation = (locationBrandCarClassesForBrand != null
                                            //	&& locationBrandCarClassesForBrand.Exists(obj => obj.CarClassID == carClass.ID));


                                            decimal? myCompanyBaseRate;
                                            decimal? myCompanyTotalRate;
                                            SearchViewModel.CompanyDetail myBrandCompany = companyDetails.Where(company => company.CompanyID == locationBrand.BrandID).FirstOrDefault();
                                            if (myBrandCompany != null)
                                            {
                                                myCompanyBaseRate = myBrandCompany.BaseValue;
                                                myCompanyTotalRate = myBrandCompany.TotalValue;
                                            }
                                            else
                                            {
                                                myCompanyBaseRate = null;
                                                myCompanyTotalRate = null;
                                            }

                                            if (ruleSetApplied == null || ruleSetApplied.ID <= 0)
                                            {
                                                _searchResultSuggestedRatesService.Add(new SearchResultSuggestedRate
                                                {
                                                    SearchSummaryID = searchSummaryID,
                                                    LocationID = locationBrand.LocationID,
                                                    BrandID = locationBrand.BrandID,
                                                    RentalLengthID = rentalLengthID,

                                                    Date = arrivalDate.Date,
                                                    CarClassID = carClass.ID,
                                                    TotalRate = 0,
                                                    BaseRate = 0,
                                                    MinBaseRate = minMaxRateFound ? minBaseRate : (decimal?)null,
                                                    MaxBaseRate = minMaxRateFound && !isScheduledJob ? maxBaseRate : (decimal?)null,

                                                    RuleSetID = 0,
                                                    RuleSetName = "",
                                                    CompanyBaseRate = myCompanyBaseRate,
                                                    CompanyTotalRate = myCompanyTotalRate,

                                                });

                                                var remainingCompanies = companyIDsBasedOnMCarRates.Except(companyDetails.Select(obj => obj.CompanyID).Distinct());
                                                if (remainingCompanies.Count() > 0)
                                                {
                                                    foreach (long companyID in remainingCompanies)
                                                    {

                                                        companyDetails.Add(new SearchViewModel.CompanyDetail()
                                                        {
                                                            CompanyID = companyID,
                                                            CompanyName = allCompanies.First(obj1 => obj1.ID == companyID).Name,

                                                            BaseValue = (decimal?)null,
                                                            TotalValue = (decimal?)null,

                                                            Islowest = false,
                                                            IslowestAmongCompetitors = false,
                                                            CompanyRankBasedOnMCcarRate = filteredSearchResults.Where(obj => obj.CompanyID == companyID).First().CompanyRankBasedOnMCcarRate,
                                                            IsPositionChanged = false
                                                        });

                                                    }
                                                }
                                                //Rule set not found then add default data for everything
                                                //rateInformation.CompanyDetails.AddRange(companyDetails);
                                                rateInformation.CompanyDetails.AddRange(companyDetails.OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate));

                                                //set arrival date
                                                rateInformation.CompanyDetails.ForEach(obj => obj.ArrivalDate = arrivalDate);
                                                //add rateinfo object to main list.
                                                finalData.RatesInfo.Add(rateInformation);
                                                continue;
                                            }
                                            //get companies in the ruleset to get direct competitors
                                            //long[] directCompetitors = _ruleSetCompanies.GetAll().Where(obj => obj.RuleSetID == ruleSetApplied.ID).Select(obj => obj.CompanyID).ToArray();

                                            long[] directCompetitors = (from ruleSetGroup in _context.RuleSetGroup
                                                                        join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                                                        where ruleSetGroup.RuleSetID == ruleSetApplied.ID
                                                                        select ruleSetGroupCompanies.CompanyID).ToArray();



                                            //get direct competitor and rates
                                            Dictionary<long, decimal> directCompetitorsWithRate = new Dictionary<long, decimal>();

                                            //This list contain only competitor which has totalRate
                                            //For gov shop list has direct competitor with base rate in asc order.
                                            var dc = companyDetails.Where(obj => obj.TotalValue.HasValue && directCompetitors.Contains(obj.CompanyID));
                                            if (dc.Count() > 0)
                                            {
                                                //get distict company; sometimes it comes duplicate here
                                                //if not is gov and not BaseRuleset applied(scheduledjobs) then find competitor with lowest Total Cost 
                                                if (!isGov && !isBaseRuleset)
                                                {
                                                    directCompetitorsWithRate = dc.ToList().GroupBy(comp => comp.CompanyID).Select(grp => grp.First()).OrderBy(obj => obj.TotalValue.Value).ToDictionary(obj => obj.CompanyID, obj => obj.TotalValue.Value);
                                                }
                                                else
                                                {
                                                    directCompetitorsWithRate = dc.ToList().GroupBy(comp => comp.CompanyID).Select(grp => grp.First()).OrderBy(obj => obj.BaseValue.Value).ToDictionary(obj => obj.CompanyID, obj => obj.BaseValue.Value);
                                                }
                                            }

                                            decimal suggestedRate = 0;

                                            //set this variable for classic
                                            long companyIDForlowestRateAmoungDirectCompetitorsClassic = 0;
                                            decimal? rateMoreThanLowestRateDirectCompetitor = null;
                                            bool noCompitetorRateFound = false;
                                            if (directCompetitorsWithRate.Count > 0)
                                            {

                                                companyIDForlowestRateAmoungDirectCompetitorsClassic = directCompetitorsWithRate.First().Key;

                                                long companyIDForlowestRateAmoungDirectCompetitors = 0;
                                                //directCompetitorsWithRate = (from entry in directCompetitorsWithRate orderby entry.Value ascending select entry).ToDictionary(obj => obj.Key, obj => obj.Value);

                                                //Special Scenario1: If lowest rate competitor is direct competitor(from rule set) then we have to set the flag for 
                                                //'IslowestAmongCompetitors' to second lowest rate provider. 
                                                //Also the suggested rate should be calculated based on 'IsLowest' competitor but not from 'IslowestAmongCompetitors'                                                                                          
                                                if (companyDetails.FirstOrDefault(obj => obj.Islowest) != null && companyDetails.Where(obj => obj.Islowest).Select(obj1 => obj1.CompanyID).ToArray().Any(companyId => directCompetitorsWithRate.Keys.Contains(companyId)))
                                                {
                                                    if (directCompetitorsWithRate.Count > 1)
                                                    {
                                                        //direct competitor providing lowest rate amoung all competitors; go for second lowest competitor
                                                        companyIDForlowestRateAmoungDirectCompetitors = directCompetitorsWithRate.Skip(1).First().Key;

                                                        //Special Scenario2:If special scenario 1 satisfy then we will check for lowestRateAmoungDirectCompetitor
                                                        //if myCompany (brand) is providing lower/equal rate than lowestRateAmoungDirectCompetitor then myCompany should
                                                        //be highlighted in green. All calculation should be based on only lowestRateAmoungDirectCompetitor.
                                                        //Note: This scenario handled in front end

                                                        //Special scenario3: If two or more direct competitor is providing lowest rate then highlight such competitor in yellow
                                                        //Special scenario4: If scenario 3 satisfy, then highligh green should be shift to rate provider for more rate than lowest Rate providers
                                                        //
                                                        int lowestRateProviders = directCompetitorsWithRate.Values.Count(obj => obj == minTotalValue);
                                                        if (directCompetitorsWithRate.Count > lowestRateProviders)
                                                        {
                                                            rateMoreThanLowestRateDirectCompetitor = directCompetitorsWithRate.Skip(lowestRateProviders).First().Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //only one direct competitor found and competitor is itself lowest rate provider.
                                                        //Highligh my companay as lowest green
                                                        if (companyDetails.Exists(obj => obj.CompanyID == locationBrand.BrandID))
                                                        {
                                                            companyDetails.Where(obj => obj.CompanyID == locationBrand.BrandID).All(obj => obj.IslowestAmongCompetitors = true);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //get lowestRateAmoungDirectCompetitors from normal scenario
                                                    companyIDForlowestRateAmoungDirectCompetitors = directCompetitorsWithRate.First().Key;
                                                }

                                                //Set IslowestAmongCompetitors flag  
                                                if (rateMoreThanLowestRateDirectCompetitor.HasValue)
                                                {
                                                    //For gov shop compare rateMoreThanLowestRateDirectCompetitor value with Base Rate for all other shops compare it with total Rate .
                                                    if (!isGov && !isBaseRuleset)
                                                        companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.TotalValue == rateMoreThanLowestRateDirectCompetitor.Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                                    else
                                                        companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.BaseValue.Value == rateMoreThanLowestRateDirectCompetitor.Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                                }
                                                else
                                                {
                                                    if (!isGov)
                                                        if (!isBaseRuleset)
                                                            companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.TotalValue == directCompetitorsWithRate.First().Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                                        else
                                                            companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.BaseValue == directCompetitorsWithRate.First().Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                                    else
                                                        companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.BaseValue.Value == directCompetitorsWithRate.First().Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                                }

                                                companyDetails.Where(obj => obj.CompanyID == companyIDForlowestRateAmoungDirectCompetitorsClassic).ToList().First().IslowestAmongCompetitorsClassic = true;

                                                //Here we are setting lowest rate provider as 'companyIDForlowestRateAmoungDirectCompetitors'
                                                //to calculate suggested rate. This is because suggested rate should not depend on any special scenario
                                                companyIDForlowestRateAmoungDirectCompetitors = directCompetitorsWithRate.First().Key;
                                                //get lowest rate
                                                decimal lowestRateAmoungDirectCompetitor = directCompetitorsWithRate.First().Value;


                                                //Calculate suggested rates using 'companyIDForlowestRateAmoungDirectCompetitors' 

                                                //step 1: Decide that this Rental length is falls in dayRange/ WeekRange/ MonthRange based on start
                                                //charactor of rentalLength (e.g. D/ W/ M)

                                                decimal gapAmount = (from ruleSetGroup in _context.RuleSetGroup
                                                                     join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                                                     join ruleSetGapSettings in _context.RuleSetGapSettings on ruleSetGroup.ID equals ruleSetGapSettings.RuleSetGroupID
                                                                     join rangeIntervals in _context.RangeIntervals on ruleSetGapSettings.RangeIntervalID equals rangeIntervals.ID
                                                                     where ruleSetGroup.RuleSetID == ruleSetApplied.ID
                                                                     && ruleSetGroupCompanies.CompanyID == companyIDForlowestRateAmoungDirectCompetitors
                                                                     && ((isScheduledJob && isBaseRuleset) ? ruleSetGapSettings.BaseMinAmount < lowestRateAmoungDirectCompetitor
                                                                     && ruleSetGapSettings.BaseMaxAmount >= lowestRateAmoungDirectCompetitor :
                                                                     ruleSetGapSettings.MinAmount < lowestRateAmoungDirectCompetitor &&
                                                                     ruleSetGapSettings.MaxAmount >= lowestRateAmoungDirectCompetitor)
                                                                     && rangeIntervals.Range.IndexOf(rangeInterval) >= 0
                                                                     select ((isScheduledJob && isBaseRuleset) ? ruleSetGapSettings.BaseGapAmount : ruleSetGapSettings.GapAmount)).FirstOrDefault();


                                                if (isScheduledJob && (ruleSetApplied.IsWideGapTemplate || ruleSetApplied.IsGov))
                                                {
                                                    //For 'Wide gap templates'only: consider position or Avg component

                                                    //RuleSet => 'Position of Brand' OR 'Position above the average of the competitors'
                                                    //If  'IsPositionOffset' is set to true means we have to set position else 'above average of the competitor'
                                                    if (ruleSetApplied.IsPositionOffset)
                                                    {
                                                        if (ruleSetApplied.CompanyPositionAbvAvg > 1
                                                            && directCompetitorsWithRate.Count >= ruleSetApplied.CompanyPositionAbvAvg)
                                                        {
                                                            long compId = (directCompetitorsWithRate.OrderBy(obj => obj.Value)).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 1).Key;
                                                            decimal compRate = (directCompetitorsWithRate.OrderBy(obj => obj.Value)).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 1).Value;

                                                            gapAmount = (from ruleSetGroup in _context.RuleSetGroup
                                                                         join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                                                         join ruleSetGapSettings in _context.RuleSetGapSettings on ruleSetGroup.ID equals ruleSetGapSettings.RuleSetGroupID
                                                                         join rangeIntervals in _context.RangeIntervals on ruleSetGapSettings.RangeIntervalID equals rangeIntervals.ID
                                                                         where ruleSetGroup.RuleSetID == ruleSetApplied.ID
                                                                         && ruleSetGroupCompanies.CompanyID == compId
                                                                         && (isBaseRuleset ? ruleSetGapSettings.BaseMinAmount < compRate && ruleSetGapSettings.BaseMaxAmount >= compRate
                                                                         : ruleSetGapSettings.MinAmount < compRate && ruleSetGapSettings.MaxAmount >= compRate)
                                                                         && rangeIntervals.Range.IndexOf(rangeInterval) >= 0
                                                                         select (isBaseRuleset ? ruleSetGapSettings.BaseGapAmount : ruleSetGapSettings.GapAmount)).FirstOrDefault();

                                                            suggestedRate = (directCompetitorsWithRate.OrderBy(obj => obj.Value)).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 1).Value - gapAmount;

                                                            //If lowest rate calculated using [compmpetitor at the position - gapAmount) is less than next lowest competitor rate then
                                                            // take averate of the two competitor at the position and set as suggested rate
                                                            if (suggestedRate < (directCompetitorsWithRate.OrderBy(obj => obj.Value)).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 2).Value)
                                                            {
                                                                suggestedRate = (((directCompetitorsWithRate.OrderBy(obj => obj.Value).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 1).Value)
                                                                    + (directCompetitorsWithRate.OrderBy(obj => obj.Value).ElementAt(ruleSetApplied.CompanyPositionAbvAvg - 2).Value)) / 2);
                                                            }
                                                        }
                                                    }
                                                    else if (directCompetitorsWithRate.Count >= ruleSetApplied.CompanyPositionAbvAvg)
                                                    {
                                                        //Calculate rate using 'above average of the competitor'
                                                        suggestedRate = directCompetitorsWithRate.OrderBy(obj => obj.Value).Take(ruleSetApplied.CompanyPositionAbvAvg).Average(obj => obj.Value) + gapAmount;
                                                    }
                                                }
                                                //If does not fall in any condition then calculate using lowestRateAmoungDirectCompetitor
                                                if (suggestedRate == 0)
                                                {
                                                    suggestedRate = lowestRateAmoungDirectCompetitor - gapAmount;
                                                }
                                            }
                                            //Add logic here to insert rate as 0 whose rate was not present in companyDetails list

                                            //var remainingCompanies = companyDetails.Where(obj => !companyIDsBasedOnMCarRates.Contains(obj.CompanyID));

                                            var remainingCompanies1 = companyIDsBasedOnMCarRates.Except(companyDetails.Select(obj => obj.CompanyID).Distinct());
                                            if (remainingCompanies1.Count() > 0)
                                            {
                                                foreach (long companyID in remainingCompanies1)
                                                {

                                                    companyDetails.Add(new SearchViewModel.CompanyDetail()
                                                    {
                                                        CompanyID = companyID,
                                                        CompanyName = allCompanies.First(obj1 => obj1.ID == companyID).Name,

                                                        BaseValue = (decimal?)null,
                                                        TotalValue = (decimal?)null,

                                                        Islowest = false,
                                                        IslowestAmongCompetitors = false,
                                                        CompanyRankBasedOnMCcarRate = filteredSearchResults.Where(obj => obj.CompanyID == companyID).First().CompanyRankBasedOnMCcarRate,
                                                        IsPositionChanged = false
                                                    });

                                                }
                                            }
                                            //IsPositionChanged false added for quickview enhancements
                                            rateInformation.CompanyDetails.AddRange(companyDetails.OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate));

                                            rateInformation.CompanyDetails.ForEach(obj => obj.ArrivalDate = arrivalDate);

                                            //add rateinfo object to main list.
                                            finalData.RatesInfo.Add(rateInformation);

                                            decimal suggestedTotalRate = 0;
                                            //For GOV rate truncate decimal part in minimum rates present if any 
                                            if (isGov && minMaxRateFound)
                                            {
                                                minBaseRate = Math.Truncate(minBaseRate);
                                            }
                                            //decimal suggestedBaseRate = CalculateSuggestedBaseRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedRate, rentalLengthCode, rangeInterval, isGov);
                                            decimal suggestedBaseRate = 0;
                                            if (!isGov)
                                            {
                                                if (!isBaseRuleset)
                                                {
                                                    suggestedTotalRate = suggestedRate;
                                                    suggestedBaseRate = CalculateSuggestedBaseRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedTotalRate, rentalLengthCode, rangeInterval, isGov);
                                                }
                                                else
                                                {
                                                    suggestedBaseRate = suggestedRate;
                                                    suggestedTotalRate = CalculateSuggestedTotalRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedBaseRate, rentalLengthCode, rangeInterval, isGov);
                                                }
                                            }
                                            else
                                            {
                                                suggestedBaseRate = Math.Truncate(suggestedRate);
                                                suggestedTotalRate = CalculateSuggestedTotalRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedBaseRate, rentalLengthCode, rangeInterval, isGov);
                                            }
                                            //for scheduled job, calculate suggested rate based on min limit.
                                            if (isScheduledJob && minMaxRateFound && (suggestedBaseRate < minBaseRate || suggestedBaseRate > maxBaseRate))
                                            {
                                                if (suggestedBaseRate <= 0 && directCompetitorsWithRate.Count() == 0)
                                                {
                                                    noCompitetorRateFound = true;
                                                    //suggestedBaseRate = myCompanyBaseRate.HasValue ? myCompanyBaseRate.Value : minBaseRate;
                                                    //suggestedTotalRate = CalculateSuggestedTotalRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedBaseRate, rentalLengthCode, rangeInterval, isGov);
                                                    suggestedBaseRate = 0;
                                                    suggestedTotalRate = 0;
                                                }
                                                else
                                                {
                                                    suggestedBaseRate = suggestedBaseRate > maxBaseRate ? maxBaseRate : minBaseRate;
                                                    suggestedTotalRate = CalculateSuggestedTotalRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedBaseRate, rentalLengthCode, rangeInterval, isGov);
                                                }
                                            }
                                            else if (isScheduledJob && !minMaxRateFound)
                                            {
                                                if (suggestedBaseRate <= 0 && directCompetitorsWithRate.Count() == 0)
                                                {
                                                    noCompitetorRateFound = true;
                                                    suggestedBaseRate = 0;
                                                    suggestedTotalRate = 0;
                                                    //suggestedBaseRate = myCompanyBaseRate.HasValue ? myCompanyBaseRate.Value : minBaseRate;
                                                    //suggestedTotalRate = CalculateSuggestedTotalRate(locationBrandID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrandID), suggestedBaseRate, rentalLengthCode, rangeInterval, isGov);
                                                }
                                            }


                                            //calculate suggested rate based on gapAmount and save in database
                                            _searchResultSuggestedRatesService.Add(new SearchResultSuggestedRate
                                            {
                                                SearchSummaryID = searchSummaryID,
                                                LocationID = locationBrand.LocationID,
                                                BrandID = locationBrand.BrandID,
                                                RentalLengthID = rentalLengthID,

                                                Date = arrivalDate.Date,
                                                CarClassID = carClass.ID,
                                                TotalRate = suggestedTotalRate,

                                                BaseRate = suggestedBaseRate,
                                                MinBaseRate = minMaxRateFound ? minBaseRate : (decimal?)null,
                                                MaxBaseRate = minMaxRateFound && !isScheduledJob ? maxBaseRate : (decimal?)null,

                                                RuleSetID = ruleSetApplied.ID,
                                                RuleSetName = ruleSetApplied.Name,
                                                CompanyBaseRate = myCompanyBaseRate,
                                                CompanyTotalRate = myCompanyTotalRate,
                                                NoCompitetorRateFound = noCompitetorRateFound
                                            });
                                            #endregion
                                        }
                                        else
                                        {
                                            #region add empty row as no one company providing rate for this car class
                                            _searchResultSuggestedRatesService.Add(new SearchResultSuggestedRate
                                            {
                                                SearchSummaryID = searchSummaryID,
                                                LocationID = locationBrand.LocationID,
                                                BrandID = locationBrand.BrandID,
                                                RentalLengthID = rentalLengthID,

                                                Date = arrivalDate.Date,
                                                CarClassID = carClass.ID,
                                                TotalRate = 0,
                                                BaseRate = 0,
                                                MinBaseRate = minMaxRateFound ? minBaseRate : (decimal?)null,
                                                MaxBaseRate = minMaxRateFound && !isScheduledJob ? maxBaseRate : (decimal?)null,

                                                RuleSetID = 0,
                                                RuleSetName = "",
                                                CompanyBaseRate = (decimal?)null,
                                                CompanyTotalRate = (decimal?)null,

                                            });

                                            var remainingCompanies = companyIDsBasedOnMCarRates.Distinct();
                                            if (remainingCompanies.Count() > 0)
                                            {
                                                foreach (long companyID in remainingCompanies)
                                                {

                                                    companyDetails.Add(new SearchViewModel.CompanyDetail()
                                                    {
                                                        CompanyID = companyID,
                                                        CompanyName = allCompanies.First(obj1 => obj1.ID == companyID).Name,

                                                        BaseValue = (decimal?)null,
                                                        TotalValue = (decimal?)null,

                                                        Islowest = false,
                                                        IslowestAmongCompetitors = false,
                                                        CompanyRankBasedOnMCcarRate = filteredSearchResults.Where(obj => obj.CompanyID == companyID).First().CompanyRankBasedOnMCcarRate,
                                                        IsPositionChanged = false
                                                    });

                                                }
                                            }
                                            //Rule set not found then add default data for everything
                                            //rateInformation.CompanyDetails.AddRange(companyDetails);
                                            rateInformation.CompanyDetails.AddRange(companyDetails.OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate));

                                            //set arrival date
                                            rateInformation.CompanyDetails.ForEach(obj => obj.ArrivalDate = arrivalDate);
                                            //add rateinfo object to main list.
                                            finalData.RatesInfo.Add(rateInformation);
                                            #endregion
                                        }
                                        if (isQuickView && finalData.RatesInfo.Count > 0)
                                        {
                                            if (finalData.RatesInfo.Exists(carClassRow => carClassToCompete.Contains(carClassRow.CarClassID) && carClassRow.CarClassID == carClass.ID &&
                                                                            carClassRow.CompanyDetails.Any(company => company.IsMovedUp.HasValue || (company.IsPositionChanged.HasValue && company.IsPositionChanged.Value))))
                                            {
                                                carClassChanged.Add(new CarClassCodeOrderDTO { CarClassCode = carClass.Code, DisplayOrder = carClass.DisplayOrder });
                                            }
                                        }
                                    }

                                    dailyViewRatesInfolist.AddRange(finalData.RatesInfo);

                                    //Save FinalData in database
                                    _searchResultProcessedData.Add(new SearchResultProcessedData
                                    {
                                        SearchSummaryID = searchSummaryID,
                                        ScrapperSourceID = scrapperSourceID,
                                        LocationID = locationBrand.LocationID,
                                        RentalLengthID = rentalLengthID,
                                        DateFilter = arrivalDate.Date,
                                        DailyViewJSONResult = Convert.ToString(new JavaScriptSerializer().Serialize(finalData)),
                                        ClassicViewJSONResult = "Daily View Record",
                                        CreatedDateTime = DateTime.Now,

                                    });
                                    #endregion

                                    #endregion

                                    if (isQuickView && finalData.RatesInfo.Count > 0)
                                    {
                                        bool? isMoveUp = null;
                                        bool isPositionChange = false;
                                        if (finalData.RatesInfo.Exists(carClassRow =>
                                        carClassRow.CompanyDetails.Any(company => company.IsMovedUp.HasValue && !company.IsMovedUp.Value)))
                                        {
                                            isMoveUp = false;
                                        }
                                        else if (finalData.RatesInfo.Exists(carClassRow =>
                                        carClassRow.CompanyDetails.Any(company => company.IsMovedUp.HasValue && company.IsMovedUp.Value)))
                                        {
                                            isMoveUp = true;
                                        }

                                        isPositionChange = finalData.RatesInfo.Exists(carClassRow => carClassRow.CompanyDetails.Any(company => company.IsPositionChanged.Value));
                                        //insert quick view comparison results
                                        //null means no movement
                                        _quickViewResultService.Add(new QuickViewResults
                                        {
                                            SearchSummaryId = searchSummary.ID,
                                            QuickViewId = quickViewRow.ID,
                                            ScrapperSourceId = scrapperSourceID,
                                            LocationBrandId = locationBrandID,
                                            RentalLengthId = rentalLengthID,
                                            Date = arrivalDate.Date,
                                            IsMovedUp = isMoveUp,
                                            IsPositionChange = isPositionChange
                                        });


                                    }
                                }

                                if (!isFTBScheduledJob)
                                {
                                    GenerateSeachResultProcesssedDataClassic(searchSummary, dailyViewRatesInfolist, rentalLengthID, locationBrand, scrapperSourceID, allCompanies);
                                }
                            }
                            //overwrite the rates for D12, D34 & W56
                            //OverWriteOneRateForAssociateLORGroup(searchSummaryID);
                        }
                        //update execution status of quick view
                        if (isQuickView && quickViewRow != null)
                        {
                            quickViewRow.IsExecutionInProgress = false;
                            if (quickViewRow.NextRunDate.HasValue)
                            {
                                quickViewRow.StatusId = _statusService.GetAll().Where(status => status.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).FirstOrDefault();
                                //set status to Scheduled
                            }
                            else
                            {
                                //set status to finished
                                quickViewRow.StatusId = _statusService.GetAll().Where(status => status.Status.Equals("Process Complete", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).FirstOrDefault();
                            }
                            _context.Entry(quickViewRow).State = EntityState.Modified;
                            _context.SaveChanges();

                            if (quickViewRow.NotifyEmail.HasValue && quickViewRow.NotifyEmail.Value)
                                await SendQuickViewEmail(quickViewRow.ID, searchSummaryID, shopStartEndDate, quickViewRow.CreatedBy, carClassChanged);
                        }
                    }

                    //check if shop is initiated from Automation Console then generate summary report for this shop too.
                    if (isScheduledJob)
                    {
                        CustomPushJSONDTO customPushJSONDTO = new CustomPushJSONDTO();
                        customPushJSONDTO.ShopStartDate = Convert.ToString(newSummaryForAutomation.StartDate.ToString("yyyy-MM-dd"));
                        customPushJSONDTO.ShopEndDate = Convert.ToString(newSummaryForAutomation.EndDate.ToString("yyyy-MM-dd"));
                        User user = _userService.GetById(newSummaryForAutomation.CreatedBy, false);
                        customPushJSONDTO.User = user.UserName;

                        GenerateSummaryReport(newSummaryForAutomation.ID, locationBrandIDs, scrapperSourcesIDs, rentalLengthIDs, carClassIDs, searchResultsforSeachSummaryID, carClassIDForMCar, allCarClasses, allCompanies, allLocationCompanies, customPushJSONDTO);
                    }
                }
                else
                {
                    CustomPushJSONDTO customPushJSONDTO = new CustomPushJSONDTO();
                    customPushJSONDTO.ShopStartDate = Convert.ToString(searchSummary.StartDate.ToString("yyyy-MM-dd"));
                    customPushJSONDTO.ShopEndDate = Convert.ToString(searchSummary.EndDate.ToString("yyyy-MM-dd"));
                    User user = _userService.GetById(searchSummary.CreatedBy, false);
                    customPushJSONDTO.User = user.UserName;

                    //generate summary report
                    GenerateSummaryReport(searchSummaryID, locationBrandIDs, scrapperSourcesIDs, rentalLengthIDs, carClassIDs, searchResultsforSeachSummaryID, carClassIDForMCar, allCarClasses, allCompanies, allLocationCompanies, customPushJSONDTO);
                }
                if (isScheduledJob && !isSummaryShop)
                {
                    User user = _userService.GetById(scheduledJob.UpdatedBy, false);
                    if (user != null && user.IsTSDUpdateAccess.HasValue && user.IsTSDUpdateAccess.Value)
                    {
                        UpdateRatesToTSD(searchSummary.ID, scheduledJob, allFormula, allCarClasses, rentalLengths, allWeekDays, isGov);
                    }
                }

                searchSummary = _searchSummaryService.GetById(searchSummaryID, false);
                searchSummary.UpdatedDateTime = DateTime.Now;
                if (searchSummary.StatusID != _statusService.GetStatusIDByName("Deleted"))
                {
                    searchSummary.StatusID = _statusService.GetStatusIDByName("Process Complete");

                    if (isScheduledJob && newSummaryForAutomation != null)
                    {
                        //update status of the new dummy shop which was added in search summary
                        newSummaryForAutomation.UpdatedDateTime = DateTime.Now;
                        newSummaryForAutomation.StatusID = _statusService.GetStatusIDByName("Process Complete");
                        _searchSummaryService.Update(newSummaryForAutomation);
                    }

                    if (isScheduledJob && !isSummaryShop)
                    {
                        searchSummary.IsReviewed = false;
                        UpdateScheduledJobStatus(scheduledJob);
                    }
                    _searchSummaryService.Update(searchSummary);
                }
            }
            catch (Exception ex)
            {
                SearchSummary searchSummary = _searchSummaryService.GetById(searchSummaryID, false);
                searchSummary.UpdatedDateTime = DateTime.Now;
                searchSummary.StatusID = _statusService.GetStatusIDByName("Failed");
                searchSummary.Response = "Data Processing Failed.";
                searchSummary.IsReviewed = false;
                _searchSummaryService.Update(searchSummary);

                if (searchSummary.ScheduledJobID.HasValue && searchSummary.ScheduledJobID.Value > 0)
                {
                    ScheduledJob scheduledJob = _scheduledJobService.GetById(searchSummary.ScheduledJobID.Value, false);
                    searchSummary.IsReviewed = false;
                    UpdateScheduledJobStatus(scheduledJob);
                }

                //update execution status of quick view
                if (isQuickView)
                {
                    QuickView quickViewRow = _context.QuickView.Where(quickView => quickView.ChildSearchSummaryId == searchSummaryID).FirstOrDefault();
                    if (quickViewRow != null)
                    {
                        quickViewRow.IsExecutionInProgress = false;
                        if (quickViewRow.NextRunDate.HasValue)
                        {
                            quickViewRow.StatusId = _statusService.GetAll().Where(status => status.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)).Select(status => status.ID).FirstOrDefault();
                            //set status to Scheduled
                        }
                        else
                        {
                            //set status to Failed
                            quickViewRow.StatusId = _statusService.GetStatusIDByName("Failed");
                        }
                        _context.Entry(quickViewRow).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    Logger.WriteToLogFile("Quick View Id: " + quickViewRow.ID + ",  Exception Occured,Inner Exception: " + ex.InnerException
                        + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, Logger.GetLogFilePath());
                }

                Logger.WriteToLogFile("SearchSummaryId: " + searchSummaryID + ",  Exception Occured,Inner Exception: " + ex.InnerException
                    + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, Logger.GetLogFilePath());
                throw;
            }
            return string.Empty;
        }


        private void OverWriteOneRateForAssociateLORGroup(ref List<SearchResultSuggestedRate> suggestedRates, List<long> finalLOR, List<RentalLength> allRentalLengths)
        {
            //List<SqlParameter> queryParameters = new List<SqlParameter>();
            //queryParameters.Add(new SqlParameter("SearchSummaryId", searchSummaryId));

            //string queryOverWriteOneRateForAssociateLORGroup = "EXEC OverWriteOneRateForAssociateLORGroup ";
            //queryOverWriteOneRateForAssociateLORGroup = string.Concat(queryOverWriteOneRateForAssociateLORGroup, string.Join(", ", queryParameters.Select(d => "@" + d.ParameterName)));
            //var data = _context.ExecuteSqlCommand(queryOverWriteOneRateForAssociateLORGroup, queryParameters.ToArray());


            //OverWriteOneRateForAssociateLORGroup(ref suggestedRates, finalLOR);
            foreach (long maxLor in finalLOR)
            {
                RentalLength rentalLength = allRentalLengths.Where(d => d.AssociateMappedId.HasValue && d.MappedID == maxLor).FirstOrDefault();
                if (rentalLength != null)
                {
                    List<RentalLength> groupRentalLengths = allRentalLengths.Where(e => e.AssociateMappedId.HasValue && e.AssociateMappedId == rentalLength.AssociateMappedId && e.MappedID != maxLor).ToList();
                    IEnumerable<SearchResultSuggestedRate> searchSuggestedRatesOfMaxLOR = suggestedRates.Where(d => d.RentalLengthID == maxLor);

                    foreach (RentalLength rental in groupRentalLengths)
                    {
                        if (suggestedRates.Any(d => d.RentalLengthID == rental.MappedID))
                        {
                            if (searchSuggestedRatesOfMaxLOR.Count() > 0)
                            {
                                suggestedRates.Where(d => d.RentalLengthID != maxLor).GroupBy(d => d.RentalLengthID);

                            }
                        }
                    }
                }

            }

        }

        /// <summary>
        /// If user have created ruleset for specific job then we have to apply all JobSpecific Rulesets + 
        /// (all unjobSpecific ruleset having isWideGap = 1)
        /// </summary>
        /// <param name="scheduledJobId"></param>
        /// <returns></returns>
        private List<long> GetIngnoredRulsets(long scheduledJobId, out List<ScheduledJobRuleSets> scheduledJobRuleSets)
        {
            List<long> ignoreRuleSets = new List<long>();
            scheduledJobRuleSets = _context.ScheduledJobRuleSets.ToList();
            if (scheduledJobRuleSets != null && scheduledJobRuleSets.Count > 0)
            {
                //Get other jobs 'Job specific ruleset (origional and copied)'
                var ingnoreEntries = scheduledJobRuleSets.Where(obj => obj.ScheduleJobID.HasValue && obj.ScheduleJobID.Value != scheduledJobId);

                //ignore own origional ruleset i.e. consider only copy once
                ignoreRuleSets.AddRange(scheduledJobRuleSets.Where(obj => obj.ScheduleJobID.HasValue && obj.ScheduleJobID.Value == scheduledJobId).Select(obj => obj.OriginalRuleSetID).ToList());
                if (ingnoreEntries != null && ingnoreEntries.Count() > 0)
                {
                    //If copy ruleset found for this job then ignore others copy rulesets
                    ignoreRuleSets.AddRange(ingnoreEntries.Select(obj => obj.RuleSetID).ToList());
                }
            }
            return ignoreRuleSets;
        }

        private void GenerateSeachResultProcesssedDataClassic(SearchSummary searchSummary, List<SearchViewModel.RatesInfo> dailyViewRatesInfolist,
            long rentalLengthID, LocationBrand locationBrand, long scrapperSourceID, List<Company> allCompanies)
        {
            if (dailyViewRatesInfolist == null || dailyViewRatesInfolist.Count <= 0)
            {
                return;
            }
            //get all locationCompanies and put it in cache
            IEnumerable<LocationCompany> locationCompanies = _context.LocationCompany.Where(obj => obj.LocationID == locationBrand.LocationID);

            Company myCompany = allCompanies.FirstOrDefault(obj => obj.ID == locationBrand.BrandID);
            //this data contain single ScrapperSource, location brand and rental length
            //Multiple date, car classes
            var carClassGroups = dailyViewRatesInfolist.GroupBy(obj => obj.CarClassID);
            if (carClassGroups == null)
            {
                return;
            }
            foreach (var carClassGroup in carClassGroups.OrderBy(obj => obj.Key))
            {
                //Get Car class id
                long carClassID = carClassGroup.Key;
                SearchViewModelClassic.FinalData finalData = new SearchViewModelClassic.FinalData();
                finalData.BrandID = locationBrand.BrandID;
                finalData.BrandCode = myCompany.Code;
                finalData.CarClassID = carClassGroup.Key;
                finalData.RatesInfo = new List<SearchViewModelClassic.RatesInfo>();

                foreach (SearchViewModel.RatesInfo ratesInfoDaily in carClassGroup)
                {
                    if (ratesInfoDaily == null || ratesInfoDaily.CompanyDetails.Count <= 0 || !ratesInfoDaily.CompanyDetails.Any(obj => obj.TotalValue.HasValue))
                    {
                        continue;
                    }
                    var dateGroups = ratesInfoDaily.CompanyDetails.GroupBy(obj => obj.ArrivalDate).OrderBy(obj => obj.Key);
                    foreach (var dateGroup in dateGroups)
                    {

                        SearchViewModelClassic.RatesInfo rateinfoClassic = new SearchViewModelClassic.RatesInfo();
                        rateinfoClassic.DateInfo = dateGroup.Key;
                        rateinfoClassic.CompanyDetails = new List<SearchViewModelClassic.CompanyDetail>();


                        LocationCompany locationCompany_myCompany = locationCompanies.FirstOrDefault(obj => obj.CompanyID == locationBrand.BrandID);
                        //if (locationCompany_myCompany == null || locationCompany_myCompany.ID <= 0)
                        //{
                        //    continue;
                        //}
                        //add my company details into data to show in first column
                        SearchViewModel.CompanyDetail myCompanydailyViewRate = dateGroup.FirstOrDefault(obj => obj.CompanyID == locationBrand.BrandID);
                        if (myCompanydailyViewRate != null && myCompanydailyViewRate.CompanyID > 0)
                        {
                            rateinfoClassic.CompanyDetails.Add(new SearchViewModelClassic.CompanyDetail()
                            {
                                CompanyID = myCompanydailyViewRate.CompanyID,
                                TotalValue = myCompanydailyViewRate.TotalValue.HasValue ? myCompanydailyViewRate.TotalValue.Value : (decimal?)null,
                                BaseValue = myCompanydailyViewRate.BaseValue.HasValue ? myCompanydailyViewRate.BaseValue.Value : (decimal?)null,
                                IsMovedUp = myCompanydailyViewRate.IsMovedUp.HasValue ? myCompanydailyViewRate.IsMovedUp.Value : (bool?)null,
                                CompanyCode = myCompany.Code,
                                Inside = (locationCompany_myCompany != null && locationCompany_myCompany.CompanyID > 0) ? locationCompany_myCompany.IsTerminalInside : (bool?)null,
                            });
                        }
                        else
                        {
                            rateinfoClassic.CompanyDetails.Add(new SearchViewModelClassic.CompanyDetail()
                            {
                                CompanyID = myCompany.ID,
                                TotalValue = (decimal?)null,
                                BaseValue = (decimal?)null,
                                IsMovedUp = (bool?)null,
                                CompanyCode = myCompany.Code,
                                Inside = (locationCompany_myCompany != null && locationCompany_myCompany.CompanyID > 0) ? locationCompany_myCompany.IsTerminalInside : (bool?)null,
                            });
                        }

                        //add remaining companies
                        foreach (SearchViewModel.CompanyDetail companyDetailDaily in dateGroup.Where(obj => obj.BaseValue.HasValue && obj.TotalValue.HasValue).OrderBy(obj1 => obj1.TotalValue))
                        {
                            Company thisCompany = allCompanies.FirstOrDefault(obj => obj.ID == companyDetailDaily.CompanyID);
                            LocationCompany locationCompany = locationCompanies.FirstOrDefault(obj => obj.CompanyID == companyDetailDaily.CompanyID);
                            if ((thisCompany != null && thisCompany.ID > 0))
                            {
                                rateinfoClassic.CompanyDetails.Add(new SearchViewModelClassic.CompanyDetail()
                                {
                                    CompanyID = companyDetailDaily.CompanyID,
                                    TotalValue = companyDetailDaily.TotalValue.HasValue ? companyDetailDaily.TotalValue.Value : (decimal?)null,
                                    BaseValue = companyDetailDaily.BaseValue.HasValue ? companyDetailDaily.BaseValue.Value : (decimal?)null,
                                    IslowestAmongCompetitors = companyDetailDaily.IslowestAmongCompetitorsClassic,
                                    IsMovedUp = companyDetailDaily.IsMovedUp.HasValue ? companyDetailDaily.IsMovedUp.Value : (bool?)null,
                                    CompanyCode = thisCompany.Code,
                                    Inside = (locationCompany != null && locationCompany.CompanyID > 0) ? locationCompany.IsTerminalInside : (bool?)null,
                                });
                            }
                        }
                        finalData.RatesInfo.Add(rateinfoClassic);
                    }
                }
                //Save FinalData in database
                if (finalData != null && finalData.RatesInfo != null && finalData.RatesInfo.Count > 0)
                {
                    _searchResultProcessedData.Add(new SearchResultProcessedData
                    {
                        SearchSummaryID = searchSummary.ID,
                        ScrapperSourceID = scrapperSourceID,
                        LocationID = locationBrand.LocationID,
                        RentalLengthID = rentalLengthID,
                        CarClassID = carClassID,
                        DailyViewJSONResult = "Classic View Record",
                        ClassicViewJSONResult = Convert.ToString(new JavaScriptSerializer().Serialize(finalData)),
                        CreatedDateTime = DateTime.Now,

                    });
                }
            }
        }


        private void UpdateScheduledJobStatus(ScheduledJob scheduledJob)
        {
            if (scheduledJob != null)
            {
                //get new so that latest update will be fethched
                scheduledJob = _scheduledJobService.GetById(scheduledJob.ID, false);
                scheduledJob.ExecutionInProgress = false;
                _scheduledJobService.Update(scheduledJob);
            }
        }

        private void UpdateRatesToTSD(long searchSummaryID, ScheduledJob scheduledJob, List<Formula> allFormula, List<CarClass> allCarClasses,
             List<RentalLength> allRentalLengths, List<WeekDay> allWeekDays, bool isGOV)
        {
            List<SearchResultSuggestedRate> suggestedRates = null;
            List<SearchResultSuggestedRate> tetheredSuggestedRates = new List<SearchResultSuggestedRate>();
            suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryID).OrderBy(obj => obj.CarClassID).ToList();
            if (suggestedRates != null && suggestedRates.Count() > 0)
            {
                List<Location> allLocations = _locationService.GetAll().ToList();
                List<LocationBrand> allLocationBrands = _locationBrandService.GetAll().ToList();


                long locationBrandId = Common.StringToLongList(scheduledJob.LocationBrandIDs).Min();
                List<LocationBrandRentalLength> locationBrandRentalLength = _context.LocationBrandRentalLength.Where(x => x.LocationBrandID == locationBrandId).ToList();
                List<long> rentalLengthIds = Common.StringToLongList(scheduledJob.RentalLengthIDs).ToList();

                //Implement Additional LOR D12,D34 and W56 implementation with scenario
                //var FinalRentalLength = from arl in allRentalLengths
                //                        join rl in rentalLengthIds on arl.MappedID equals rl
                //                        select arl;

                //var groupRentalLength = FinalRentalLength.GroupBy(obj => new { obj.AssociateMappedId });

                //List<long> FinalLOR = new List<long>();
                //foreach (var item in groupRentalLength)
                //{
                //    if ((item.Count() >= 2 || item.Count() <= 2) && item.Key.AssociateMappedId.HasValue)
                //    {
                //        long checklength = locationBrandRentalLength.Where(x => x.RentalLengthID == item.Key.AssociateMappedId.Value).Count();
                //        if (checklength != 0)
                //        {
                //            FinalLOR.Add(item.Select(x => x.MappedID).Max());
                //        }
                //        else
                //        {
                //            foreach (var rentalLength in item)
                //            {
                //                FinalLOR.Add(rentalLength.MappedID);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        foreach (var rentalLength in item)
                //        {
                //            FinalLOR.Add(rentalLength.MappedID);
                //        }
                //    }
                //}
                ////OverWriteOneRateForAssociateLORGroup(ref suggestedRates, FinalLOR, allRentalLengths);
                ////Skip if additional Rental configured and create only single XML file for combined LOR
                //suggestedRates = (from sr in suggestedRates
                //                  join rn in FinalLOR on sr.RentalLengthID equals rn
                //                  select sr).ToList();



                #region Save Tether brand Rates if 'IsActiveTethering'
                if (scheduledJob.IsActiveTethering)
                {
                    //tetheredSuggestedRates.AddRange(suggestedRates);

                    //We also need to check the global settings exist or not. Because if used delete the global settings then 
                    //tethering should be disabled for automation also.
                    List<GlobalTetherSetting> globalTetherSettings = _globalTetherService.GetAll().ToList();

                    List<ScheduledJobTetherings> scheduledJobTetherings = _context.ScheduledJobTetherings.Where(obj => obj.ScheduleJobID == scheduledJob.ID).ToList();

                    List<ScheduledJobMinRates> globalLimitDetailsScheduled = _scheduledJobMinRatesService.GetSelectedMinRate(scheduledJob.ID);

                    var suggestedLocationBrandIdGroups = suggestedRates.GroupBy(obj => new { obj.LocationID, obj.BrandID });

                    if (suggestedLocationBrandIdGroups == null || suggestedLocationBrandIdGroups.Count() <= 0)
                    {
                        return;
                    }
                    foreach (var suggestedRatesGroup in suggestedLocationBrandIdGroups)
                    {
                        ScheduledJobTetherings anyScheduledJobTethering = scheduledJobTetherings.Where(obj => obj.DominentBrandID == suggestedRatesGroup.Key.BrandID).FirstOrDefault();


                        if (anyScheduledJobTethering == null && anyScheduledJobTethering.ID <= 0)
                        {
                            continue;
                        }

                        LocationBrand locationBrand = allLocationBrands.Where(obj => obj.LocationID == suggestedRatesGroup.Key.LocationID && obj.BrandID == anyScheduledJobTethering.DependantBrandID).FirstOrDefault();

                        List<GlobalLimitDataDTO> globalLimitData = _globalLimitDetailService.GetGlobalLimitData(locationBrand.ID);

                        foreach (SearchResultSuggestedRate suggestedRate in suggestedRatesGroup)
                        {
                            SearchResultSuggestedRate tetherRate = new SearchResultSuggestedRate()
                            {
                                SearchSummaryID = suggestedRate.SearchSummaryID,
                                LocationID = suggestedRate.LocationID,
                                //BrandID = suggestedRate.BrandID,
                                RentalLengthID = suggestedRate.RentalLengthID,
                                Date = suggestedRate.Date,
                                CarClassID = suggestedRate.CarClassID,
                                //BaseRate = suggestedRate.BaseRate,
                                //MinBaseRate = suggestedRate.MinBaseRate,
                                //MaxBaseRate = suggestedRate.MaxBaseRate,
                                //TotalRate = suggestedRate.TotalRate,
                                //RuleSetID = suggestedRate.RuleSetID,
                                //RuleSetName = suggestedRate.RuleSetName,
                                //CompanyBaseRate = suggestedRate.CompanyBaseRate,
                                //CompanyTotalRate = suggestedRate.CompanyTotalRate,
                            };
                            ScheduledJobTetherings scheduledJobTethering = scheduledJobTetherings.Where(obj => obj.DominentBrandID == suggestedRate.BrandID
                                && obj.CarClassID == suggestedRate.CarClassID).FirstOrDefault();
                            if (scheduledJobTethering != null && scheduledJobTethering.ID > 0 && scheduledJobTethering.TetherValue.HasValue)
                            {
                                //also check the record exist in global settings, No need to check for carClass because it may not exist for all car class
                                bool isGlobalTetherSettingsExist = globalTetherSettings.Exists(obj => obj.DominentBrandID == suggestedRate.BrandID
                                & obj.LocationID == tetherRate.LocationID && obj.DependantBrandID == scheduledJobTethering.DependantBrandID);

                                if (!isGlobalTetherSettingsExist)
                                {
                                    continue;
                                }
                                //tether settings found now modify the suggested rate object for tsd update
                                tetherRate.BrandID = scheduledJobTethering.DependantBrandID;

                                decimal differenceAmount = 0;
                                decimal newTotalRate = 0;
                                decimal newBaseRate = 0;

                                tetherRate.BaseRate = newBaseRate;
                                tetherRate.TotalRate = newTotalRate;

                                //Calculate tether rates from origional rates
                                if (suggestedRate.TotalRate.HasValue)
                                {
                                    //for non-GOV shops calculate Tether rate from Dominant Brand Total Rate 
                                    if (!isGOV)
                                    {
                                        if (suggestedRate.TotalRate > 0 && suggestedRate.BaseRate > 0)
                                        {
                                            if (scheduledJobTethering.IsTetherValueinPercentage)
                                            {
                                                differenceAmount = suggestedRate.TotalRate.Value * scheduledJobTethering.TetherValue.Value / 100;
                                            }
                                            else
                                            {
                                                differenceAmount = scheduledJobTethering.TetherValue.Value;
                                            }
                                            newTotalRate = suggestedRate.TotalRate.Value + differenceAmount;

                                            //Calculate new base
                                            //LocationBrand locationBrand = allLocationBrands.Where(obj => obj.LocationID == tetherRate.LocationID && obj.BrandID == tetherRate.BrandID).FirstOrDefault();
                                            RentalLength rentalLength = allRentalLengths.FirstOrDefault(obj => obj.ID == tetherRate.RentalLengthID);
                                            string rentalLengthCode = rentalLength.Code;
                                            newBaseRate = CalculateSuggestedBaseRate(locationBrand.ID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrand.ID),
                                                newTotalRate, rentalLengthCode, rentalLengthCode.Substring(0, 1), isGOV);

                                            tetherRate.BaseRate = newBaseRate;
                                            tetherRate.TotalRate = newTotalRate;

                                            #region Apply 'Set Min Rate'
                                            //For tethered brand, we get min max rate from 'Set Min Rate' popup. If not found then apply from global limit.
                                            decimal minBaseRate = 0, maxBaseRate = 0;
                                            bool minMaxRateFound = false;


                                            minMaxRateFound = SetMinMaxRatesAutomation(globalLimitDetailsScheduled, rentalLengthCode.Substring(0, 1), tetherRate.CarClassID, tetherRate.Date, out minBaseRate, out maxBaseRate);
                                            if (!minMaxRateFound)
                                            {
                                                //Set min max rate; In Scheduled job, if min rate not found then apply min rate from global limit
                                                List<GlobalLimitDetail> globalLimitDetails = _globalLimitDetailService.GetGlobalLimitDetails(globalLimitData, tetherRate.Date);
                                                minMaxRateFound = SetMinMaxRates(globalLimitDetails, rentalLengthCode.Substring(0, 1), tetherRate.CarClassID, out minBaseRate, out maxBaseRate);
                                            }
                                            if (minMaxRateFound)
                                            {
                                                tetherRate.MinBaseRate = minBaseRate;
                                                tetherRate.MaxBaseRate = maxBaseRate;
                                                if (tetherRate.BaseRate < minBaseRate || tetherRate.BaseRate > maxBaseRate)
                                                {
                                                    tetherRate.BaseRate = tetherRate.BaseRate > maxBaseRate ? maxBaseRate : minBaseRate;
                                                    tetherRate.TotalRate = CalculateSuggestedTotalRate(locationBrand.ID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrand.ID), tetherRate.BaseRate, rentalLengthCode, rentalLengthCode.Substring(0, 1), isGOV);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    //for GOV shops calculate Tether rate from Dominant Brand Base Rate 
                                    else
                                    {
                                        bool noTetherGapForGov = Convert.ToBoolean(ConfigurationManager.AppSettings["GOVNoTetherGap"]);

                                        if (suggestedRate.TotalRate > 0 && suggestedRate.BaseRate > 0)
                                        {
                                            //if GOVNoTetherGap is true then Dominant Brand and Dependant brand rates will be same 
                                            //set noTetherGapForGov to false if tether gap setting to be used , tether gap setting will be applied on Base Rate for GOV shop
                                            if (!noTetherGapForGov)
                                            {
                                                if (scheduledJobTethering.IsTetherValueinPercentage)
                                                {
                                                    differenceAmount = suggestedRate.BaseRate * scheduledJobTethering.TetherValue.Value / 100;
                                                }
                                                else
                                                {
                                                    differenceAmount = scheduledJobTethering.TetherValue.Value;
                                                }
                                                newBaseRate = suggestedRate.BaseRate + differenceAmount;
                                            }
                                            else
                                            {
                                                newBaseRate = suggestedRate.BaseRate;
                                            }
                                            newBaseRate = Math.Truncate(newBaseRate);
                                            //Calculate new base
                                            //LocationBrand locationBrand = allLocationBrands.Where(obj => obj.LocationID == tetherRate.LocationID && obj.BrandID == tetherRate.BrandID).FirstOrDefault();
                                            RentalLength rentalLength = allRentalLengths.FirstOrDefault(obj => obj.ID == tetherRate.RentalLengthID);
                                            string rentalLengthCode = rentalLength.Code;
                                            newTotalRate = CalculateSuggestedTotalRate(locationBrand.ID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrand.ID),
                                                newBaseRate, rentalLengthCode, rentalLengthCode.Substring(0, 1), isGOV);

                                            tetherRate.BaseRate = newBaseRate;
                                            tetherRate.TotalRate = newTotalRate;

                                            #region Apply 'Set Min Rate'
                                            //For tethered brand, we get min max rate from 'Set Min Rate' popup. If not found then apply from global limit.
                                            decimal minBaseRate = 0, maxBaseRate = 0;
                                            bool minMaxRateFound = false;


                                            minMaxRateFound = SetMinMaxRatesAutomation(globalLimitDetailsScheduled, rentalLengthCode.Substring(0, 1), tetherRate.CarClassID, tetherRate.Date, out minBaseRate, out maxBaseRate);
                                            if (!minMaxRateFound)
                                            {
                                                //Set min max rate; In Scheduled job, if min rate not found then apply min rate from global limit
                                                List<GlobalLimitDetail> globalLimitDetails = _globalLimitDetailService.GetGlobalLimitDetails(globalLimitData, tetherRate.Date);
                                                minMaxRateFound = SetMinMaxRates(globalLimitDetails, rentalLengthCode.Substring(0, 1), tetherRate.CarClassID, out minBaseRate, out maxBaseRate);
                                            }
                                            if (minMaxRateFound)
                                            {
                                                tetherRate.MinBaseRate = minBaseRate;
                                                tetherRate.MaxBaseRate = maxBaseRate;
                                                //truncate decimal part present in base rate
                                                minBaseRate = Math.Truncate(minBaseRate);
                                                if (tetherRate.BaseRate < minBaseRate || tetherRate.BaseRate > maxBaseRate)
                                                {
                                                    tetherRate.BaseRate = tetherRate.BaseRate > maxBaseRate ? maxBaseRate : minBaseRate;
                                                    tetherRate.TotalRate = CalculateSuggestedTotalRate(locationBrand.ID, allFormula.FirstOrDefault(obj => obj.LocationBrandID == locationBrand.ID), tetherRate.BaseRate, rentalLengthCode, rentalLengthCode.Substring(0, 1), isGOV);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    //If no compitetor found but tether is enable then only dependant brand rate should save.
                                    if (suggestedRate.NoCompitetorRateFound)
                                    {
                                        tetherRate.BaseRate = 0;
                                        tetherRate.TotalRate = 0;
                                        tetherRate.RuleSetID = 0;
                                        tetherRate.NoCompitetorRateFound = true;
                                        _context.SearchResultSuggestedRates.Add(tetherRate);
                                    }
                                    else
                                    {
                                        //Save tether suggested Rates to SearchResultSuggestedRates
                                        _context.SearchResultSuggestedRates.Add(tetherRate);
                                        tetheredSuggestedRates.Add(tetherRate);
                                    }
                                }
                            }
                        }
                    }

                    //Save tether suggested Rates to SearchResultSuggestedRates
                    _context.SaveChanges();
                }
                #endregion

                long[] tSDUpdateWeekDayIds = Common.StringToLongList(scheduledJob.TSDUpdateWeekDay).ToArray();
                string[] jobScheduledWeekDays = new string[7];
                if (tSDUpdateWeekDayIds != null && tSDUpdateWeekDayIds.Count() > 0)
                {
                    Array.Sort(tSDUpdateWeekDayIds);
                    jobScheduledWeekDays = allWeekDays.Where(obj => tSDUpdateWeekDayIds.Contains(obj.ID)).Select(obj1 => obj1.Day).ToArray();
                    if (jobScheduledWeekDays.Count() > 0)
                    {
                        //suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryID).OrderBy(obj => obj.CarClassID).ToList();
                        if (suggestedRates != null && suggestedRates.Count() > 0)
                        {
                            suggestedRates = suggestedRates.Where(obj => jobScheduledWeekDays.Contains(obj.Date.DayOfWeek.ToString())).ToList();

                            string UserName = _userService.GetById(scheduledJob.UpdatedBy).UserName;
                            TSDUpdate(searchSummaryID, scheduledJob, suggestedRates, allCarClasses, allRentalLengths, allLocations,
                        allLocationBrands, UserName, false, allFormula);

                            #region Tether rate update
                            if (tetheredSuggestedRates != null && tetheredSuggestedRates.Count() > 0)
                            {
                                tetheredSuggestedRates = tetheredSuggestedRates.Where(obj => jobScheduledWeekDays.Contains(obj.Date.DayOfWeek.ToString())).ToList();
                                TSDUpdate(searchSummaryID, scheduledJob, tetheredSuggestedRates, allCarClasses, allRentalLengths, allLocations,
                            allLocationBrands, UserName, true, allFormula);
                            }
                            #endregion
                        }
                    }
                }
            }
        }

        private void TSDUpdate(long searchSummaryID, ScheduledJob scheduleJob, List<SearchResultSuggestedRate> suggestedRates, List<CarClass> allCarClasses,
            List<RentalLength> allRentalLengths, List<Location> allLocations, List<LocationBrand> allLocationBrands, string UserName, bool isTetheredValue,
            List<Formula> allFormula)
        {
            if (suggestedRates == null || suggestedRates.Count() <= 0)
            {
                return;
            }
            int SummaryIdCounter = 0;
            string brandCode = string.Empty;
            long locationBrandId = 0;
            string[] opaqueRentalLengths = ConfigurationManager.AppSettings["OpaqueLORs"].Split(',');

            string locationCode = allLocations.FirstOrDefault(obj => obj.ID == suggestedRates.FirstOrDefault().LocationID).Code;
            LocationBrand locationBrand = allLocationBrands.FirstOrDefault(obj => obj.LocationID == suggestedRates.FirstOrDefault().LocationID && obj.BrandID == suggestedRates.FirstOrDefault().BrandID);
            if (locationBrand == null || locationBrand.ID <= 0 || !locationBrand.LocationBrandAlias.Contains("-"))
            {
                return;
            }
            brandCode = locationBrand.LocationBrandAlias.Split('-')[1];
            locationBrandId = locationBrand.ID;

            List<GlobalLimitDataDTO> globalLimitData = _globalLimitDetailService.GetGlobalLimitData(locationBrandId);
            List<ScheduledJobOpaqueValuesDTO> lstJobOpaqueValues = new List<ScheduledJobOpaqueValuesDTO>();
            if (scheduleJob.ID > 0 && scheduleJob.IsOpaqueActive)
            {
                lstJobOpaqueValues = _scheduledJobOpaqueValuesService.GetAll(false).Where(d => d.ScheduledJobId == scheduleJob.ID).Select(d => new ScheduledJobOpaqueValuesDTO
                {
                    Id = d.ID,
                    PercenValue = d.PercentValue,
                    ScheduledJobId = d.ScheduledJobId,
                    CarClassId = d.CarClassId,
                    CarCode = allCarClasses.FirstOrDefault(obj => obj.ID == d.CarClassId).Code
                }).ToList();
            }

            #region Send RentalLength-wise TSD updates

            //Group by rental lengths
            var suggestedRateRentalLengthGroups = suggestedRates.Where(a => !(a.NoCompitetorRateFound)).GroupBy(obj => obj.RentalLengthID).OrderBy(obj1 => obj1.Key);

            if (suggestedRateRentalLengthGroups == null || suggestedRateRentalLengthGroups.Count() <= 0)
            {
                return;
            }

            List<RateCodeDTO> lstRateCodes = new List<RateCodeDTO>();
            lstRateCodes = _rateCodeService.GetRateCodesWithDateRanges();
            List<LocationBrandRentalLength> locationBrandRentalLength = _context.LocationBrandRentalLength.Where(x => x.LocationBrandID == locationBrandId).ToList();

            foreach (var suggestedRatesGroup in suggestedRateRentalLengthGroups)
            {
                RentalLength rentalLength = allRentalLengths.FirstOrDefault(obj => obj.ID == suggestedRatesGroup.Key);

                //Changed for implement additional LOR 
                var finalRentalLength = (rentalLength.AssociateMappedId.HasValue && locationBrandRentalLength.Where(x => x.RentalLengthID == rentalLength.AssociateMappedId.Value).Count() > 0) ? rentalLength.AssociateMappedId : rentalLength.MappedID;
                string rentalLengthCode = allRentalLengths.SingleOrDefault(x => x.MappedID == finalRentalLength).Code;

                //rentalLength.Code;

                List<TSDModel> tsdModels = new List<TSDModel>();
                foreach (SearchResultSuggestedRate suggestedRate in suggestedRatesGroup)
                {
                    if (suggestedRate.BaseRate > 0 && suggestedRate.TotalRate > 0)
                    {
                        SummaryIdCounter += 1;
                        TSDModel tsdModel = new TSDModel();
                        tsdModel.RemoteID = Convert.ToString(searchSummaryID) + '-' + SummaryIdCounter;
                        tsdModel.Branch = locationCode;
                        tsdModel.CarClass = allCarClasses.FirstOrDefault(obj => obj.ID == suggestedRate.CarClassID).Code;
                        tsdModel.RentalLength = rentalLengthCode;
                        tsdModel.RentalLengthIDs = Convert.ToString(suggestedRate.RentalLengthID);

                        tsdModel.StartDate = suggestedRate.Date.ToString("yyyyMMd");
                        tsdModel.DailyRate = suggestedRate.BaseRate;
                        tsdModel.TotalRate = suggestedRate.TotalRate.Value;
                        if (rentalLengthCode.ToUpper().IndexOf('D') >= 0 && locationBrand.DailyExtraDayFactor.HasValue)
                        {
                            tsdModel.ExtraDayRateFactor = locationBrand.DailyExtraDayFactor.Value;
                        }
                        else if (rentalLengthCode.ToUpper().IndexOf('W') >= 0 && locationBrand.WeeklyExtraDenom.HasValue)
                        {
                            tsdModel.ExtraDayRateFactor = locationBrand.WeeklyExtraDenom.Value;
                        }


                        tsdModel.RentalLengthCount = 1;
                        //save upated rate to database
                        tsdModel.SuggestedRateId = suggestedRate.ID;

                        tsdModels.Add(tsdModel);
                    }
                }

                //TODO bind Rate System Source to "RateSystemSource"
                _tSDTransactionService.ProcessRateSelection(tsdModels, UserName, "WebLink", locationBrandId, scheduleJob.UpdatedBy, searchSummaryID, isTetheredValue, brandCode);

                //Push opaque rates for D3 and W7 LOR
                if (scheduleJob.IsOpaqueActive && lstRateCodes.Count > 0 && opaqueRentalLengths.Contains(rentalLengthCode) && lstJobOpaqueValues.Count > 0)
                {
                    List<TSDModel> lstOpaqueTSD = new List<TSDModel>();
                    TSDModel opaqueTSD = null;

                    foreach (var tsd in tsdModels)
                    {
                        var objOpaque = lstJobOpaqueValues.Where(d => d.CarCode.Equals(tsd.CarClass)).FirstOrDefault();
                        if (objOpaque == null || objOpaque.PercenValue == 0)
                        {
                            continue;
                        }
                        opaqueTSD = new TSDModel();
                        opaqueTSD.IsOpaqueRates = true;
                        opaqueTSD.Branch = tsd.Branch;
                        opaqueTSD.CarClass = tsd.CarClass;
                        opaqueTSD.StartDate = tsd.StartDate;
                        opaqueTSD.RentalLength = _tSDTransactionService.GetApplicableRateCodes(lstRateCodes, locationBrand.BrandID, DateTime.ParseExact(tsd.StartDate, "yyyyMMd", CultureInfo.CurrentCulture));
                        if (string.IsNullOrEmpty(opaqueTSD.RentalLength))
                        {
                            string defaultRateCode = ConfigurationManager.AppSettings["DefaultRateCode"];
                            if (brandCode == "AD" && !string.IsNullOrEmpty(defaultRateCode))
                            {
                                opaqueTSD.RentalLength = defaultRateCode;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (rentalLengthCode.Length > 0 && rentalLengthCode.IndexOf("D") >= 0)
                        {
                            opaqueTSD.OpaqueTSDModel.IsDaily = true;
                        }
                        else if (rentalLengthCode.Length > 0 && rentalLengthCode.IndexOf("W") >= 0)
                        {
                            opaqueTSD.OpaqueTSDModel.IsWeekly = true;
                        }
                        opaqueTSD.DailyRate = Math.Round(tsd.DailyRate + (tsd.DailyRate * (objOpaque.PercenValue / 100)), 2);

                        lstOpaqueTSD.Add(opaqueTSD);
                        opaqueTSD = null;
                    }

                    _tSDTransactionService.ProcessRateSelection(lstOpaqueTSD, UserName, "WebLink", locationBrandId, scheduleJob.UpdatedBy, 0, false, brandCode, false, false);

                }
            }

            #endregion
        }

        public decimal CalculateSuggestedBaseRate(long locationBrandID, Formula objFormula, decimal totalRate, string rentalLengthCode, string rangeInterval, bool isGOV = false)
        {
            decimal baseRate = 0;
            //objFormula = _formulaService.GetFormulaByLocation(locationBrandID);
            if (objFormula != null && objFormula.ID > 0 && !string.IsNullOrEmpty(objFormula.TotalCostToBase))
            {
                //rentalLengthCode = GetAssociatedLORGroupMaxRentalLengthString(rentalLengthCode, locationBrandID);
                string baseRateFormula = objFormula.TotalCostToBase;
                string lengthForWeekAndMonth = "1";

                //FOR nd:numeric value exists in rental length code and for 
                //FOR len:if rental length is 'D' then len will be numeric value attached with 'D' else if rental length is 'W12-W14' then 2 else it will be 1
                if (rentalLengthCode.ToUpper() == "W12" || rentalLengthCode.ToUpper() == "W13" || rentalLengthCode.ToUpper() == "W14")
                {
                    lengthForWeekAndMonth = "2";
                }

                //For GOV Total Rate change suggested total deduct $5 per day
                if (isGOV)
                {
                    garsFee = ConfigurationManager.AppSettings["GARSFee"];
                    if (!string.IsNullOrEmpty(rentalLengthCode) && rentalLengthCode.Length > 0)
                    {
                        int noOfDays = Convert.ToInt32(rentalLengthCode.Substring(1));
                        totalRate = totalRate - (noOfDays * Convert.ToDecimal(garsFee));
                    }

                }
                baseRateFormula = baseRateFormula.Replace("total", totalRate.ToString()).Replace("sales", Convert.ToString(objFormula.SalesTax))
                    .Replace("surcharge", Convert.ToString(objFormula.Surcharge)).Replace("airport", Convert.ToString(objFormula.AirportFee))
                    .Replace("vlrf", Convert.ToString(objFormula.VLRF)).Replace("arena", Convert.ToString(objFormula.Arena))
                    .Replace("cfc", Convert.ToString(objFormula.CFC)).Replace("nd", rentalLengthCode.Substring(1))
                    .Replace("len", rangeInterval.ToUpper() == "D" ? rentalLengthCode.Substring(1) : lengthForWeekAndMonth);

                DataTable dataTable = new DataTable();
                baseRate = (decimal)dataTable.Compute(baseRateFormula, "");
                string lengthCode = rentalLengthCode.ToUpper();
                if (lengthCode == "W8" || lengthCode == "W9" || lengthCode == "W10" || lengthCode == "W11")
                {
                    baseRate = (baseRate * 7) / Convert.ToDecimal(rentalLengthCode.Substring(1));
                }
            }
            if (isGOV)
            {
                baseRate = Math.Truncate(baseRate);
                return baseRate;
            }
            return baseRate;

        }

        public decimal CalculateSuggestedTotalRate(long locationBrandID, Formula objFormula, decimal baseRate, string rentalLengthCode, string rangeInterval, bool isGOV = false)
        {
            decimal totalRate = 0;
            //Formula objFormula = _formulaService.GetFormulaByLocation(locationBrandID);
            if (objFormula != null && objFormula.ID > 0 && !string.IsNullOrEmpty(objFormula.BaseToTotalCost))
            {
                //rentalLengthCode = GetAssociatedLORGroupMaxRentalLengthString(rentalLengthCode, locationBrandID);
                string totalRateFormula = objFormula.BaseToTotalCost;
                string lengthForWeekAndMonth = "1";

                //FOR nd:numeric value exists in rental length code and for 
                //FOR len:if rental length is 'D' then len will be numeric value attached with 'D' else if rental length is 'W12-W14' then 2 else it will be 1
                if (rentalLengthCode.ToUpper() == "W12" || rentalLengthCode.ToUpper() == "W13" || rentalLengthCode.ToUpper() == "W14")
                {
                    lengthForWeekAndMonth = "2";
                }

                string lengthCode = rentalLengthCode.ToUpper();
                if (lengthCode == "W8" || lengthCode == "W9" || lengthCode == "W10" || lengthCode == "W11")
                {
                    baseRate = (baseRate * Convert.ToDecimal(rentalLengthCode.Substring(1)) / 7);
                }

                totalRateFormula = totalRateFormula.Replace("airport", Convert.ToString(objFormula.AirportFee))
                    .Replace("sales", Convert.ToString(objFormula.SalesTax))
                    .Replace("surcharge", Convert.ToString(objFormula.Surcharge)).Replace("rt", baseRate.ToString())
                    .Replace("vlrf", Convert.ToString(objFormula.VLRF)).Replace("arena", Convert.ToString(objFormula.Arena))
                    .Replace("cfc", Convert.ToString(objFormula.CFC)).Replace("nd", rentalLengthCode.Substring(1))
                    .Replace("len", rangeInterval.ToUpper() == "D" ? rentalLengthCode.Substring(1) : lengthForWeekAndMonth);

                DataTable dataTable = new DataTable();
                totalRate = (decimal)dataTable.Compute(totalRateFormula, "");

                if (isGOV)
                {
                    garsFee = ConfigurationManager.AppSettings["GARSFee"];
                    if (!string.IsNullOrEmpty(rentalLengthCode) && rentalLengthCode.Length > 0)
                    {
                        int noOfDays = Convert.ToInt32(rentalLengthCode.Substring(1));
                        totalRate = totalRate + (noOfDays * Convert.ToDecimal(garsFee));
                    }
                }
            }
            return totalRate;
        }

        private string GetAssociatedLORGroupMaxRentalLengthString(string rentalLength, long locationBrandID)
        {
            List<RentalLength> masterRentalLengths = _rentalLengthService.GetRentalLength().ToList();
            List<LocationBrandRentalLength> locationRentalLengths = _locationBrandRentalLengthService.GetAll().Where(d => d.LocationBrandID == locationBrandID).ToList();

            RentalLength entityRentalLength = masterRentalLengths.Where(d => d.Code.ToUpper() == rentalLength).FirstOrDefault();
            if (entityRentalLength.AssociateMappedId.HasValue && locationRentalLengths.Any(d => d.RentalLengthID == entityRentalLength.AssociateMappedId.Value))
            {
                long maxMappedId = masterRentalLengths.Where(d => d.AssociateMappedId == entityRentalLength.AssociateMappedId.Value).Max(d => d.MappedID);
                rentalLength = masterRentalLengths.Where(d => d.MappedID == maxMappedId).FirstOrDefault().Code;
            }
            else if (masterRentalLengths.Any(d => d.AssociateMappedId == entityRentalLength.MappedID) && locationRentalLengths.Any(d => d.RentalLengthID == entityRentalLength.MappedID))
            {
                long maxMappedId = masterRentalLengths.Where(d => d.AssociateMappedId == entityRentalLength.MappedID).Max(d => d.MappedID);
                rentalLength = masterRentalLengths.Where(d => d.MappedID == maxMappedId).FirstOrDefault().Code;
            }
            return rentalLength;
        }

        private bool SetMinMaxRates(List<GlobalLimitDetail> GlobalLimitDetails, string RentalRange, long CarClassID, out decimal minBaseRate, out decimal maxBaseRate)
        {
            minBaseRate = maxBaseRate = 0;
            if (GlobalLimitDetails != null)
            {
                GlobalLimitDetail globalLimitDetail = GlobalLimitDetails.FirstOrDefault(obj => obj.CarClassID == CarClassID);
                if (globalLimitDetail != null)
                {
                    switch (RentalRange)
                    {
                        case "D":
                            minBaseRate = globalLimitDetail.DayMin.HasValue ? globalLimitDetail.DayMin.Value : 0;
                            maxBaseRate = globalLimitDetail.DayMax.HasValue ? globalLimitDetail.DayMax.Value : 0;
                            break;

                        case "W":
                            minBaseRate = globalLimitDetail.WeekMin.HasValue ? globalLimitDetail.WeekMin.Value : 0;
                            maxBaseRate = globalLimitDetail.WeekMax.HasValue ? globalLimitDetail.WeekMax.Value : 0;
                            break;

                        case "M":
                            minBaseRate = globalLimitDetail.MonthMin.HasValue ? globalLimitDetail.MonthMin.Value : 0;
                            maxBaseRate = globalLimitDetail.MonthMax.HasValue ? globalLimitDetail.MonthMax.Value : 0;
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool SetMinMaxRatesAutomation(List<ScheduledJobMinRates> globalLimitDetailsScheduled, string RentalRange, long CarClassID, DateTime date, out decimal minBaseRate, out decimal maxBaseRate)
        {
            minBaseRate = maxBaseRate = 0;
            if (globalLimitDetailsScheduled != null)
            {
                ScheduledJobMinRates globalLimitDetail = globalLimitDetailsScheduled.FirstOrDefault(obj => obj.CarClassID == CarClassID);
                if (globalLimitDetail != null)
                {
                    //set 7 if hashcode is 0 as db contains 7 for sunday
                    int currentWeekDay = date.DayOfWeek.GetHashCode();
                    currentWeekDay = currentWeekDay == 0 ? 7 : currentWeekDay;
                    if (!string.IsNullOrEmpty(globalLimitDetail.Days1) && globalLimitDetail.Days1.Split(',').Any(a => Convert.ToInt32(a) == currentWeekDay))
                    {
                        switch (RentalRange)
                        {
                            case "D":
                                minBaseRate = globalLimitDetail.DayMin.HasValue ? globalLimitDetail.DayMin.Value : 0;
                                maxBaseRate = globalLimitDetail.DayMax.HasValue ? globalLimitDetail.DayMax.Value : 0;
                                break;

                            case "W":
                                minBaseRate = globalLimitDetail.WeekMin.HasValue ? globalLimitDetail.WeekMin.Value : 0;
                                maxBaseRate = globalLimitDetail.WeekMax.HasValue ? globalLimitDetail.WeekMax.Value : 0;
                                break;

                            case "M":
                                minBaseRate = globalLimitDetail.MonthMin.HasValue ? globalLimitDetail.MonthMin.Value : 0;
                                maxBaseRate = globalLimitDetail.MonthMax.HasValue ? globalLimitDetail.MonthMax.Value : 0;
                                break;
                        }
                        return true;
                    }
                    else if (!string.IsNullOrEmpty(globalLimitDetail.Days2) && globalLimitDetail.Days2.Split(',').Any(a => Convert.ToInt32(a) == currentWeekDay))
                    {
                        switch (RentalRange)
                        {
                            case "D":
                                minBaseRate = globalLimitDetail.Day2Min.HasValue ? globalLimitDetail.Day2Min.Value : 0;
                                maxBaseRate = globalLimitDetail.Day2Max.HasValue ? globalLimitDetail.Day2Max.Value : 0;
                                break;

                            case "W":
                                minBaseRate = globalLimitDetail.Week2Min.HasValue ? globalLimitDetail.Week2Min.Value : 0;
                                maxBaseRate = globalLimitDetail.Week2Max.HasValue ? globalLimitDetail.Week2Max.Value : 0;
                                break;

                            case "M":
                                minBaseRate = globalLimitDetail.Month2Min.HasValue ? globalLimitDetail.Month2Min.Value : 0;
                                maxBaseRate = globalLimitDetail.Month2Max.HasValue ? globalLimitDetail.Month2Max.Value : 0;
                                break;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerable<AllRuleSetsForLocationBrandDTO> GetAllRuleSetsForLocationBrand(long locationBrandID, DateTime startDate, DateTime endDate, bool isWideGapTemplate = false, List<ScheduledJobRuleSets> scheduledJobRuleSets = null, bool isGOVTemplate = false, bool isAutomationJob = false)
        {


            var allRuleSetsForLocation = (from ruleSet in _context.RuleSets
                                          join ruleSetsApplied in _context.RuleSetsApplied on ruleSet.ID equals ruleSetsApplied.RuleSetID
                                          join ruleSetRentalLength in _context.RuleSetRentalLength on ruleSet.ID equals ruleSetRentalLength.RuleSetID
                                          join ruleSetCarClasses in _context.RuleSetCarClasses on ruleSet.ID equals ruleSetCarClasses.RuleSetID
                                          join ruleSetWeekDay in _context.RuleSetWeekDay on ruleSet.ID equals ruleSetWeekDay.RuleSetID
                                          //join weekDays in _context.WeekDays on ruleSetWeekDay.WeekDayID equals weekDays.ID
                                          where
                                              //!ruleSet.IsDeleted && ruleSet.IsWideGapTemplate == isWideGapTemplate 
                                              //if ruleset is not WideGapTemplate then its standard template else it will be wideGapTemplate or wideGapTemplate + GOVTemplate
                                          !ruleSet.IsDeleted && (((isWideGapTemplate && ruleSet.IsWideGapTemplate) || (isGOVTemplate && ruleSet.IsGov)) || (!isWideGapTemplate && !ruleSet.IsWideGapTemplate &&
                                          !isGOVTemplate && !ruleSet.IsGov))
                                          && ruleSet.LocationBrandID == locationBrandID
                                          && !ruleSetsApplied.IsDeleted && ruleSetsApplied.IsActive
                                          && ruleSetsApplied.StartDate <= endDate && ruleSetsApplied.EndDate >= startDate
                                          select new AllRuleSetsForLocationBrandDTO
                                          {
                                              RuleSet = ruleSet,
                                              StartDate = ruleSetsApplied.StartDate,
                                              endDate = ruleSetsApplied.EndDate,
                                              WeekDayID = ruleSetWeekDay.WeekDayID,
                                              RentalLengthID = ruleSetRentalLength.RentalLengthID,
                                              CarClassID = ruleSetCarClasses.CarClassID
                                          }).ToList();

            if (isAutomationJob)
            {
                if (isWideGapTemplate || isGOVTemplate)
                {
                    var allRuleSetsForLocationForCopiedRulesets = (from ruleSet in _context.RuleSets
                                                                   join schduleJobRuleSet in _context.ScheduledJobRuleSets on ruleSet.ID equals schduleJobRuleSet.RuleSetID
                                                                   join ruleSetsApplied in _context.RuleSetsApplied on schduleJobRuleSet.OriginalRuleSetID equals ruleSetsApplied.RuleSetID
                                                                   join ruleSetRentalLength in _context.RuleSetRentalLength on ruleSet.ID equals ruleSetRentalLength.RuleSetID
                                                                   join ruleSetCarClasses in _context.RuleSetCarClasses on ruleSet.ID equals ruleSetCarClasses.RuleSetID
                                                                   join ruleSetWeekDay in _context.RuleSetWeekDay on ruleSet.ID equals ruleSetWeekDay.RuleSetID
                                                                   //join weekDays in _context.WeekDays on ruleSetWeekDay.WeekDayID equals weekDays.ID
                                                                   where
                                                                   !ruleSet.IsDeleted && ((isWideGapTemplate && ruleSet.IsWideGapTemplate == isWideGapTemplate) || (isGOVTemplate && ruleSet.IsGov == isGOVTemplate))
                                                                   && ruleSet.LocationBrandID == locationBrandID
                                                                   && !ruleSetsApplied.IsDeleted && ruleSetsApplied.IsActive
                                                                   && ruleSetsApplied.StartDate <= endDate && ruleSetsApplied.EndDate >= startDate
                                                                   select new AllRuleSetsForLocationBrandDTO
                                                                   {
                                                                       RuleSet = ruleSet,
                                                                       StartDate = ruleSetsApplied.StartDate,
                                                                       endDate = ruleSetsApplied.EndDate,
                                                                       WeekDayID = ruleSetWeekDay.WeekDayID,
                                                                       RentalLengthID = ruleSetRentalLength.RentalLengthID,
                                                                       CarClassID = ruleSetCarClasses.CarClassID
                                                                   }).ToList();
                    if (allRuleSetsForLocationForCopiedRulesets != null && allRuleSetsForLocationForCopiedRulesets.Count > 0)
                    {
                        if (allRuleSetsForLocation != null && allRuleSetsForLocation.Count > 0)
                        {
                            allRuleSetsForLocation.AddRange(allRuleSetsForLocationForCopiedRulesets);
                        }
                        else
                        {
                            allRuleSetsForLocation = allRuleSetsForLocationForCopiedRulesets;
                        }
                    }
                }
            }

            //if (allRuleSetsForLocation.Count > 0 && allRuleSetsForLocation.Count(obj => obj.RuleSet.IsCopiedAutomationRuleSet) > 0 && scheduledJobRuleSets != null && scheduledJobRuleSets.Count > 0)
            //{
            //    foreach (var copiedRuleset in allRuleSetsForLocation.Where(obj => obj.RuleSet.IsCopiedAutomationRuleSet))
            //    {
            //        //get origional rulesetId 
            //        long origionalRuleSetId = scheduledJobRuleSets.FirstOrDefault(obj => obj.RuleSetID == copiedRuleset.RuleSet.ID).OriginalRuleSetID;
            //        if (origionalRuleSetId > 0)
            //        {
            //            var origionalRuleSet = allRuleSetsForLocation.FirstOrDefault(rule => rule.RuleSet.ID == origionalRuleSetId);
            //            if (origionalRuleSet != null && origionalRuleSet.RuleSet.ID > 0)
            //            {
            //                copiedRuleset.StartDate = origionalRuleSet.StartDate;
            //                copiedRuleset.endDate = origionalRuleSet.endDate;
            //            }
            //        }
            //    }
            //}

            return allRuleSetsForLocation;
        }

        private RuleSet GetAppliedRuleSet(IEnumerable<AllRuleSetsForLocationBrandDTO> allRuleSetsForLocation, List<WeekDay> allWeekDays, DateTime date, long rentalLengthID, long carClassID, List<long> ignoreRuleSets)
        {

            if (allRuleSetsForLocation != null && allRuleSetsForLocation.Count() > 0)
            {
                //remove all ignoredRuleSets from main list
                if (ignoreRuleSets != null && ignoreRuleSets.Count > 0)
                {
                    allRuleSetsForLocation = allRuleSetsForLocation.Where(obj => !(ignoreRuleSets.Contains(obj.RuleSet.ID)));
                }
                if (allRuleSetsForLocation != null && allRuleSetsForLocation.Count() > 0)
                {
                    long weekDayID = allWeekDays.First(obj => obj.Day.Equals(Convert.ToString(date.DayOfWeek), StringComparison.OrdinalIgnoreCase)).ID;

                    var appliedRuleSet = allRuleSetsForLocation.Where(obj => obj.StartDate <= date && obj.endDate >= date
                                              && obj.WeekDayID == weekDayID
                                              && obj.RentalLengthID == rentalLengthID
                                              && obj.CarClassID == carClassID)
                                              .Select(obj1 => obj1.RuleSet).FirstOrDefault();

                    return appliedRuleSet;
                }
            }
            return null;
        }

        public List<SearchResult> GetBySeachSummaryID(long seachSummaryID)
        {
            return _context.SearchResults.Where(obj => obj.SearchSummaryID == seachSummaryID).ToList();
        }


        public List<ScrapperSourceDTO> GetScrapperSource(long UserId)
        {
            var ScrapperSources = _context.ScrapperSources.Join(_context.UserScrapperSources, Source => Source.ID, Users => Users.ScrapperSourceID,
                (Source, Users) => new { Code = Source.Code, ID = Source.ID, Name = Source.Name, UserId = Users.UserID, ProviderId = Source.ProviderId, IsGov = Source.IsGov }).Join(_context.Providers, scrapper => scrapper.ProviderId, providers => providers.ID,
                (scrapper, providers) => new { Code = scrapper.Code, ID = scrapper.ID, Name = scrapper.Name, UserId = scrapper.UserId, ProviderCode = providers.Code, ProviderId = providers.ID, IsGov = scrapper.IsGov.HasValue ? scrapper.IsGov.Value : false })
                .Where(Users => Users.UserId == UserId)
                .Select(obj => new { Code = obj.Code, ID = obj.ID, Name = obj.Name, ProviderCode = obj.ProviderCode, ProviderId = obj.ProviderId, IsGov = obj.IsGov }).ToList();
            return ScrapperSources.Select(obj => new ScrapperSourceDTO { Code = obj.Code, ID = obj.ID, Name = obj.Name, ProviderCode = obj.ProviderCode, ProviderId = obj.ProviderId, IsGov = obj.IsGov }).ToList();
        }

        public List<ScrapperSourceDTO> GetScrapperSource(long userId, long providerId)
        {
            var ScrapperSources = _context.ScrapperSources.Where(d => d.ProviderId == providerId).Join(_context.UserScrapperSources, Source => Source.ID, Users => Users.ScrapperSourceID,
                (Source, Users) => new { Code = Source.Code, ID = Source.ID, Name = Source.Name, UserId = Users.UserID })
                .Where(Users => Users.UserId == userId)
                .Select(obj => new { Code = obj.Code, ID = obj.ID, Name = obj.Name })
                .ToList();
            return ScrapperSources.Select(obj => new ScrapperSourceDTO { Code = obj.Code, ID = obj.ID, Name = obj.Name }).ToList();
        }

        public List<LocationBrandModel> GetBrandLocation(long userId)
        {
            var rentalLengths = _context.RentalLengths.ToList();
            var locationBrandRentalLengths = _context.LocationBrandRentalLength.ToList();

            //getting list of locations, specific to given user id
            List<LocationBrandModel> locationBrands = _context.LocationBrands
    .Join(_context.UserLocationBrands, LB => LB.ID, Users => Users.LocationBrandID, (LB, Users)
    => new { LocationID = LB.LocationID, LocationBrandAlias = LB.LocationBrandAlias, ID = LB.ID, userID = Users.UserID, IsDeleted = LB.IsDeleted })
    .Where(Users => Users.userID == userId && Users.IsDeleted == false)
    .Join(_context.Locations, Brand => Brand.LocationID, Location => Location.ID, (Brand, Location)
     => new LocationBrandModel { LocationID = Brand.LocationID, LocationBrandAlias = Brand.LocationBrandAlias, ID = Brand.ID, LocationCode = Location.Code })
     .Select(d => d).ToList();


            //List of lors which should get disabled only for locations present in locationBrands list(e.g Disable D1 in case of D12) 
            List<LocationBrandModel> locationWithDisableLors = locationBrands.Join(locationBrandRentalLengths, lb => lb.ID, lbr => lbr.LocationBrandID, (lb, lbr) => new { lbr.LocationBrandID, lbr.RentalLengthID })
                .Join(rentalLengths.Where(rl => rl.AssociateMappedId.HasValue), lbrl => lbrl.RentalLengthID, rl => rl.AssociateMappedId, (lbrl, rl) =>
                new { rl.MappedID, rl.AssociateMappedId, lbrl.LocationBrandID }).GroupBy(gr => new { gr.AssociateMappedId, gr.LocationBrandID }).Select(y =>
                    new { brandId = y.Key.LocationBrandID, lors = string.Join(",", y.Select(z => z.MappedID).OrderByDescending(d => d).Skip(1)) }).GroupBy(grp => grp.brandId)
                    .Select(d => new LocationBrandModel { ID = d.Key, LOR = string.Join(",", d.Select(e => e.lors)) }).ToList();


            //bind these list of lors to the respective location in locationBrands
            List<LocationBrandModel> LocationBrands = (from lb in locationBrands
                                            join lwd in locationWithDisableLors on lb.ID equals lwd.ID
                                            into table
                                            from lwdj in table.DefaultIfEmpty()
                                            orderby lb.LocationBrandAlias
                                            select new LocationBrandModel { ID = lb.ID, LocationBrandAlias = lb.LocationBrandAlias, LocationID = lb.LocationID, LocationCode = lb.LocationCode, LOR = lwdj != null ? lwdj.LOR : string.Empty }
                           ).ToList();

            return LocationBrands;
        }




        public List<long> GetLocationCarClasses(long locationBrandId)
        {
            return _context.LocationBrandCarClass.Where(a => a.LocationBrandID == locationBrandId).Select(a => a.CarClassID).ToList();
        }

        public List<CarClassDTO> GetCarClass()
        {
            List<CarClassDTO> carClasses = _context.CarClasses.Where(obj2 => !obj2.IsDeleted).Select(obj => new CarClassDTO { ID = obj.ID, Code = obj.Code, CarClassOrder = obj.DisplayOrder }
                    ).OrderBy(obj2 => obj2.CarClassOrder).ToList<CarClassDTO>();
            //_carClassService.GetAll()
            return carClasses;
        }

        public List<RentalLengthDTO> GetRentalLength()
        {
            List<RentalLengthDTO> rentalLengths = _context.RentalLengths.Select(obj => new RentalLengthDTO { MappedID = obj.MappedID, Code = obj.Code, AssociateMappedId = obj.AssociateMappedId, ID = obj.ID }
                    ).ToList<RentalLengthDTO>();
            return rentalLengths;
        }
        public List<Company> GetCompany()
        {
            List<Company> companies = _context.Companies.Where(obj2 => !obj2.IsDeleted).Select(obj => new Company { Code = obj.Code, ID = obj.ID }).ToList<Company>();
            return companies;
            // _companyService.GetAll()
        }

        public SearchViewAppliedRuleSetDTO SearchViewAppliedRuleSet(long RuleSetID)
        {
            SearchViewAppliedRuleSetDTO searchViewAppliedRuleSetDTO = new SearchViewAppliedRuleSetDTO();

            //Get carclasscode based on which ruleset applied

            List<CarClass> lstcarclass = (from rulesetcarclass in _context.RuleSetCarClasses
                                          join carclass in _context.CarClasses on rulesetcarclass.CarClassID equals carclass.ID
                                          orderby carclass.ID
                                          where !carclass.IsDeleted && rulesetcarclass.RuleSetID == RuleSetID
                                          select carclass).ToList();

            searchViewAppliedRuleSetDTO.CarClassID = string.Join(", ", lstcarclass.Select(obj => obj.ID).ToArray());
            searchViewAppliedRuleSetDTO.CarClassCode = string.Join(", ", lstcarclass.Select(obj => obj.Code).ToArray());

            //Get RentalLength based on which ruleset applied
            List<RentalLength> lstrental = (from rulsetrentallength in _context.RuleSetRentalLength
                                            join rentallength in _context.RentalLengths on rulsetrentallength.RentalLengthID equals rentallength.ID
                                            where rulsetrentallength.RuleSetID == RuleSetID
                                            orderby rentallength.ID ascending
                                            select rentallength).ToList();

            searchViewAppliedRuleSetDTO.RentalLengthID = string.Join(", ", (lstrental.Select(obj => obj.ID).ToArray()));
            searchViewAppliedRuleSetDTO.RentalLengthName = string.Join(", ", (lstrental.Select(obj => obj.Code).ToArray()));



            //Get dayofweek based on which ruleset applied
            List<WeekDay> lstweekday = (from rulesetweekday in _context.RuleSetWeekDay
                                        join weekday in _context.WeekDays on rulesetweekday.WeekDayID equals weekday.ID
                                        where rulesetweekday.RuleSetID == RuleSetID
                                        orderby weekday.ID
                                        select weekday).ToList();
            //lstweekday = queryableweekday.ToList();
            searchViewAppliedRuleSetDTO.DaysOfWeekID = string.Join(", ", lstweekday.Select(obj => obj.ID).ToArray());
            searchViewAppliedRuleSetDTO.DaysOfWeek = string.Join(", ", lstweekday.Select(obj => obj.Day).ToArray());

            //Get companyCode
            List<Company> lstcompany = (from company in _context.Companies
                                        join rulesetgroupcompany in _context.RuleSetGroupCompanies on company.ID equals rulesetgroupcompany.CompanyID
                                        join rulesetgroup in _context.RuleSetGroup on rulesetgroupcompany.RuleSetGroupID equals rulesetgroup.ID
                                        where !company.IsDeleted && rulesetgroup.RuleSetID == RuleSetID
                                        orderby company.ID
                                        select company).ToList();

            searchViewAppliedRuleSetDTO.lstCompany = lstcompany;
            searchViewAppliedRuleSetDTO.CompaniesID = string.Join(", ", lstcompany.Select(obj => obj.ID).ToArray());
            searchViewAppliedRuleSetDTO.CompanyCode = string.Join(", ", lstcompany.Select(obj => obj.Code).ToArray());

            List<RuleSetGroup> lstRuleSetGroup = (_context.RuleSetGroup.Where(obj => obj.RuleSetID == RuleSetID)).ToList();

            List<RangeInterval> lstRangeInterval = new List<RangeInterval>();
            lstRangeInterval = _rangeIntervalsService.GetAll().ToList();

            searchViewAppliedRuleSetDTO.lstRuleSetGroupCustom = new List<RuleSetGroupCustomDTO>();

            //Dictionary<long,string> test = _context.RuleSetGapSettings.GroupBy(a=>a.RuleSetGroupID).Select()

            var queryablegroupcompany = (from ruleSetGroup in _context.RuleSetGroup
                                         join ruleSetGroupCompanies in _context.RuleSetGroupCompanies on ruleSetGroup.ID equals ruleSetGroupCompanies.RuleSetGroupID
                                         join companies in _context.Companies on ruleSetGroupCompanies.CompanyID equals companies.ID
                                         where !companies.IsDeleted && ruleSetGroup.RuleSetID == RuleSetID
                                         select new
                                         {
                                             companyId = companies.ID,
                                             code = companies.Code,
                                             ID = ruleSetGroup.ID
                                         }).ToList();

            foreach (var ruleSetGroup in lstRuleSetGroup)
            {
                RuleSetGroupCustomDTO ruleSetGroupCustom = new RuleSetGroupCustomDTO();
                ruleSetGroupCustom.ID = ruleSetGroup.ID;
                ruleSetGroupCustom.GroupName = ruleSetGroup.GroupName;
                ruleSetGroupCustom.CompanyID = string.Join(",", queryablegroupcompany.Where(obj => obj.ID == ruleSetGroup.ID).Select(obj => obj.companyId).ToArray());
                ruleSetGroupCustom.CompanyName = string.Join(", ", queryablegroupcompany.Where(obj => obj.ID == ruleSetGroup.ID).Select(obj => obj.code).ToArray());

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
                searchViewAppliedRuleSetDTO.lstRuleSetGroupCustom.Add(ruleSetGroupCustom);

            }
            ////Add  RuleSetGroupList
            //searchViewAppliedRuleSetDTO.lstRuleSetGroupCustom = lstRuleSetGroupCustom;

            return searchViewAppliedRuleSetDTO;
        }

        public async Task<SearchResultDTO> GetSearchGridDailyViewDataDefault(long? LoggedInUserID, bool isAdmin)
        {
            if (!LoggedInUserID.HasValue)
            {
                return null;
            }
            //get recent searchSummary with 'success' status
            long completedStatusID = _statusService.GetStatusIDByName("Process Complete");

            var UserLocationBrandID = await _context.UserLocationBrands.Where(obj => obj.UserID == LoggedInUserID.Value).Select(obj => obj.LocationBrandID).ToListAsync();
            if (UserLocationBrandID != null && UserLocationBrandID.Count() > 0)
            {
                //get search summary latest record with completed status and user accessible location
                SearchSummary searchSummary = (from searchSummaries in _context.SearchSummaries
                                               join userLBId in UserLocationBrandID on searchSummaries.LocationBrandIDs equals userLBId.ToString()
                                               where (isAdmin || searchSummaries.CreatedBy == LoggedInUserID.Value) && searchSummaries.StatusID == completedStatusID
                                               && searchSummaries.ScheduledJobID == null && !searchSummaries.ShopType.Equals("SummaryShop", StringComparison.OrdinalIgnoreCase)
                                               orderby searchSummaries.CreatedDateTime descending
                                               select searchSummaries).FirstOrDefault();


                if (searchSummary != null && searchSummary.ID > 0 && DateTime.Now.Date.AddDays(int.Parse(ConfigurationManager.AppSettings["SearchGridDefaultDaysLimit"])) <= searchSummary.CreatedDateTime.Date)
                {
                    long searchSummaryID, scrapperSourceID, locationBrandID, locationID, rentallengthID;
                    DateTime arrivalDate;

                    searchSummaryID = searchSummary.ID;
                    scrapperSourceID = Common.StringToLongList(searchSummary.ScrapperSourceIDs).Min();
                    locationBrandID = Common.StringToLongList(searchSummary.LocationBrandIDs).Min();

                    LocationBrand locationBrand = _locationBrandService.GetById(locationBrandID);

                    if (locationBrand != null)
                    {
                        locationID = locationBrand.LocationID;
                        rentallengthID = Common.StringToLongList(searchSummary.RentalLengthIDs).Min();
                        arrivalDate = searchSummary.StartDate.Date;

                        SearchResultProcessedData searchResultProcessedData = _searchResultProcessedData.GetBySearchSummaryId(searchSummaryID).Where(obj => obj.ClassicViewJSONResult.Equals("Daily View Record", StringComparison.OrdinalIgnoreCase)
                            && obj.ScrapperSourceID == scrapperSourceID && obj.LocationID == locationID && obj.RentalLengthID == rentallengthID && obj.DateFilter == arrivalDate).FirstOrDefault();


                        if (searchResultProcessedData != null
                            && searchResultProcessedData.ID > 0
                            && !string.IsNullOrEmpty(searchResultProcessedData.DailyViewJSONResult))
                        {

                            List<SearchResultSuggestedRate> suggestedRates = await _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummary.ID && obj.LocationID == locationID
                                && obj.BrandID == locationBrand.BrandID && obj.RentalLengthID == rentallengthID && obj.Date == arrivalDate).ToListAsync();

                            List<SearchResultSuggestedRateDTO> SuggestedRatesDTO = suggestedRates.Select(a => new SearchResultSuggestedRateDTO { ID = a.ID, SearchSummaryID = a.SearchSummaryID, RuleSetName = a.RuleSetName, RuleSetID = a.RuleSetID, BaseRate = a.BaseRate, BrandID = a.BrandID, CarClassID = a.CarClassID, CompanyBaseRate = a.CompanyBaseRate, CompanyTotalRate = a.CompanyTotalRate, TotalRate = a.TotalRate, Date = a.Date, RentalLengthID = a.RentalLengthID, LocationID = a.LocationID, MaxBaseRate = a.MaxBaseRate, MinBaseRate = a.MinBaseRate }).ToList();

                            //get lastTSDUpdate details
                            string lastTSDUpdated = GetLastTSDUpdateDetails(suggestedRates);

                            return new SearchResultDTO
                            {
                                SearchSummaryId = searchSummaryID,
                                brandID = locationBrand.BrandID,
                                finalData = searchResultProcessedData.DailyViewJSONResult,
                                suggestedRate = SuggestedRatesDTO,
                                lastTSDUpdated = lastTSDUpdated
                            };
                        }
                    }
                }
                else
                    return null;
            }
            return null;

        }

        public async Task<SearchResultDTO> GetSearchGridDailyViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string arrivalDate)
        {
            long _searchSummaryID = 0, _scrapperSourceID = 0, _locationBrandID = 0, _rentallengthID = 0, _locationID = 0, _brandID = 0; DateTime _arrivalDate;

            if (!(long.TryParse(searchSummaryID, out _searchSummaryID) &&
            long.TryParse(scrapperSourceID, out _scrapperSourceID) &&
            long.TryParse(locationBrandID, out _locationBrandID) &&
            long.TryParse(locationID, out _locationID) &&
            long.TryParse(brandID, out _brandID) &&
            long.TryParse(rentallengthID, out _rentallengthID)))
            {
                return null;
            }
            //DateTime.TryParse(arrivalDate, out _arrivalDate);
            _arrivalDate = DateTime.ParseExact(arrivalDate, "yyyyMMd", CultureInfo.InvariantCulture);

            SearchResultProcessedData searchResultProcessedData = _searchResultProcessedData.GetBySearchSummaryId(_searchSummaryID).Where(obj => obj.ClassicViewJSONResult.Equals("Daily View Record", StringComparison.OrdinalIgnoreCase)
                && obj.ScrapperSourceID == _scrapperSourceID && obj.LocationID == _locationID && obj.RentalLengthID == _rentallengthID && obj.DateFilter == _arrivalDate).FirstOrDefault();

            if (searchResultProcessedData != null
                && searchResultProcessedData.ID > 0
                && !string.IsNullOrEmpty(searchResultProcessedData.DailyViewJSONResult))
            {
                List<SearchResultSuggestedRate> suggestedRates = await _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == _searchSummaryID
                    && obj.LocationID == _locationID && obj.BrandID == _brandID && obj.RentalLengthID == _rentallengthID && obj.Date == _arrivalDate).ToListAsync();
                List<SearchResultSuggestedRateDTO> SuggestedRatesDTO = suggestedRates.Select(a => new SearchResultSuggestedRateDTO { ID = a.ID, SearchSummaryID = a.SearchSummaryID, RuleSetName = a.RuleSetName, RuleSetID = a.RuleSetID, BaseRate = a.BaseRate, BrandID = a.BrandID, CarClassID = a.CarClassID, CompanyBaseRate = a.CompanyBaseRate, CompanyTotalRate = a.CompanyTotalRate, TotalRate = a.TotalRate, Date = a.Date, RentalLengthID = a.RentalLengthID, LocationID = a.LocationID, MaxBaseRate = a.MaxBaseRate, MinBaseRate = a.MinBaseRate }).ToList();

                //get lastTSDUpdate details
                string lastTSDUpdated = GetLastTSDUpdateDetails(suggestedRates);

                return new SearchResultDTO
                {
                    brandID = _brandID,
                    finalData = searchResultProcessedData.DailyViewJSONResult,
                    suggestedRate = SuggestedRatesDTO,
                    lastTSDUpdated = lastTSDUpdated
                };
            }

            return null;
        }

        public async Task<SearchResultDTO> GetSearchGridClassicViewData(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID, string carClassId)
        {
            long _searchSummaryID = 0, _scrapperSourceID = 0, _locationBrandID = 0, _rentallengthID = 0, _carClassId = 0, _locationID = 0, _brandID = 0;

            if (!(long.TryParse(searchSummaryID, out _searchSummaryID) &&
            long.TryParse(scrapperSourceID, out _scrapperSourceID) &&
            long.TryParse(locationBrandID, out _locationBrandID) &&
            long.TryParse(locationID, out _locationID) &&
            long.TryParse(brandID, out _brandID) &&
            long.TryParse(rentallengthID, out _rentallengthID) &&
            long.TryParse(carClassId, out _carClassId)))
            {
                return null;
            }

            //SearchSummary searchSummary = _searchSummaryService.GetAll().Where(obj => obj.ID == _searchSummaryID).FirstOrDefault();
            //LocationBrand locationBrand = _locationBrandService.GetById(_locationBrandID);


            SearchResultProcessedData searchResultProcessedData = _searchResultProcessedData.GetBySearchSummaryId(_searchSummaryID).Where(obj => obj.DailyViewJSONResult.Equals("Classic View Record", StringComparison.OrdinalIgnoreCase) &&
                obj.ScrapperSourceID == _scrapperSourceID && obj.LocationID == _locationID && obj.RentalLengthID == _rentallengthID && obj.CarClassID == _carClassId).FirstOrDefault();

            if (searchResultProcessedData != null
                && searchResultProcessedData.ID > 0
                && !string.IsNullOrEmpty(searchResultProcessedData.ClassicViewJSONResult))
            {
                List<SearchResultSuggestedRate> suggestedRates = await _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == _searchSummaryID
                    && obj.LocationID == _locationID && obj.BrandID == _brandID && obj.RentalLengthID == _rentallengthID && obj.CarClassID == _carClassId).ToListAsync();

                List<SearchResultSuggestedRateDTO> SuggestedRatesDTO = suggestedRates.Select(a => new SearchResultSuggestedRateDTO { ID = a.ID, SearchSummaryID = a.SearchSummaryID, RuleSetName = a.RuleSetName, RuleSetID = a.RuleSetID, BaseRate = a.BaseRate, BrandID = a.BrandID, CarClassID = a.CarClassID, CompanyBaseRate = a.CompanyBaseRate, CompanyTotalRate = a.CompanyTotalRate, TotalRate = a.TotalRate, Date = a.Date, RentalLengthID = a.RentalLengthID, LocationID = a.LocationID, MaxBaseRate = a.MaxBaseRate, MinBaseRate = a.MinBaseRate }).ToList();

                //get lastTSDUpdate details
                string lastTSDUpdated = GetLastTSDUpdateDetails(suggestedRates);

                return new SearchResultDTO
                {
                    brandID = _brandID,
                    finalData = searchResultProcessedData.ClassicViewJSONResult,
                    suggestedRate = SuggestedRatesDTO,
                    lastTSDUpdated = lastTSDUpdated
                };
            }

            return null;
        }

        public string GetLastUpdateOnTSD(long searchSummaryID, long scrapperSourceID, long locationBrandID, long locationID, long brandID, long rentallengthID, long carClassId, string arrivalDate, string view)
        {
            List<SearchResultSuggestedRate> suggestedRates;
            if (view.ToLower() == "daily")
            {
                DateTime _arrivalDate = DateTime.ParseExact(arrivalDate, "yyyyMMd", CultureInfo.InvariantCulture);

                suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryID && obj.LocationID == locationID
                                && obj.BrandID == brandID && obj.RentalLengthID == rentallengthID && obj.Date == _arrivalDate).ToList();

            }
            else
            {
                suggestedRates = _context.SearchResultSuggestedRates.Where(obj => obj.SearchSummaryID == searchSummaryID
                       && obj.LocationID == locationID && obj.BrandID == brandID && obj.RentalLengthID == rentallengthID && obj.CarClassID == carClassId).ToList();

            }
            return GetLastTSDUpdateDetails(suggestedRates);
        }

        public void BulkInsert(List<MapTableResult> searchResult)
        {
            EZRACRateShopperContext context = new EZRACRateShopperContext();

            //        //Convert list to dataTable for bulk copy 
            DataTable results = Helper.ListToDataTable.ConvertToDataTable(searchResult);
            if (context != null)
            {
                using (var tx = new TransactionScope())
                {
                    using (var bcp = new SqlBulkCopy(context.Database.Connection.ConnectionString))
                    {
                        bcp.BatchSize = searchResult.Count;
                        //TODO:make table name configurable 
                        bcp.DestinationTableName = "SearchResults";
                        bcp.WriteToServer(results);
                        tx.Complete();
                    }
                }
            }
        }

        private string GetLastTSDUpdateDetails(List<SearchResultSuggestedRate> suggestedRates)
        {
            if (suggestedRates != null && suggestedRates.Count(obj => obj.TSDUpdateDateTime.HasValue) > 0)
            {
                SearchResultSuggestedRate recentUpdatedRecord = suggestedRates.Where(obj => obj.TSDUpdateDateTime.HasValue).OrderByDescending(m => m.TSDUpdateDateTime.Value).FirstOrDefault();
                if (recentUpdatedRecord != null && recentUpdatedRecord.TSDUpdateDateTime.HasValue)
                {
                    return _userService.GetById(recentUpdatedRecord.TSDUpdatedBy.Value).UserName + "|" + recentUpdatedRecord.TSDUpdateDateTime.Value.ToString("MMMM dd, yyyy, hh:mm tt");
                }
            }
            return string.Empty;
        }

        class CompanyDetailsComparare : IEqualityComparer<RateShopper.Domain.Entities.SearchViewModel.CompanyDetail>
        {
            private Func<RateShopper.Domain.Entities.SearchViewModel.CompanyDetail, object> _funcDistinct;
            public CompanyDetailsComparare(Func<RateShopper.Domain.Entities.SearchViewModel.CompanyDetail, object> funcDistinct)
            {
                this._funcDistinct = funcDistinct;
            }

            public bool Equals(RateShopper.Domain.Entities.SearchViewModel.CompanyDetail x, RateShopper.Domain.Entities.SearchViewModel.CompanyDetail y)
            {
                return _funcDistinct(x).Equals(_funcDistinct(y));
            }

            public int GetHashCode(RateShopper.Domain.Entities.SearchViewModel.CompanyDetail obj)
            {
                return this._funcDistinct(obj).GetHashCode();
            }
        }

        public static class Logger
        {

            public static void WriteToLogFile(string strMessage, string outputFile)
            {

                try
                {
                    string line = DateTime.Now.ToString() + " | ";

                    line += strMessage;

                    FileStream fs = new FileStream(outputFile, FileMode.Append, FileAccess.Write, FileShare.None);

                    StreamWriter swFromFileStream = new StreamWriter(fs);

                    swFromFileStream.WriteLine(line);

                    swFromFileStream.Flush();

                    swFromFileStream.Close();

                }
                catch (Exception)
                {

                    throw;
                }
            }


            public static string GetLogFilePath()
            {
                try
                {
                    // get the base directory

                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    // search the file below the current directory
                    string retFilePath = baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\" + "LogFile.txt";

                    // Console.WriteLine("Relative file path " + ConfigurationManager.AppSettings["LogFilePath"]);
                    // if exists, return the path
                    if (File.Exists(retFilePath) == true)
                        return retFilePath;
                    //create a text file
                    else
                    {
                        if (CheckDirectory(baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\") == false)
                            return string.Empty;

                        FileStream fs = new FileStream(retFilePath,
                              FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs.Close();
                    }

                    return retFilePath;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            private static bool CheckDirectory(string strLogPath)
            {
                try
                {
                    int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                    string strDirectoryname =
                               strLogPath.Trim().Substring(0, nFindSlashPos);

                    if (false == Directory.Exists(strDirectoryname))
                        Directory.CreateDirectory(strDirectoryname);
                    return true;
                }
                catch (Exception)
                {
                    return false;

                }
            }

        }

        ///<QuickviewRateComparison>
        ///Quick View IsMoved Up flag check if competitor previous rate and current rate beat deviation value
        ///</QuickviewRateComparison> 
        public bool? IsMovedUp(long companyId, long carClassId, decimal? companyCurrentRate, List<SearchResult> prevSearchResults, IEnumerable<QuickViewGroupCompaniesDTO> quickViewGroupCompetitors,
            IEnumerable<QuickViewCarClassGroups> quickViewCarClassGroup, IEnumerable<QuickViewGapDevSettingsDTO> quickViewGapDeviations, decimal? domPrevRate, decimal? domCurrentRate, bool monitorBase)
        {
            decimal devValue = GetGapDeviationValue(GetCompetitorGroupId(companyId, quickViewGroupCompetitors), GetCarClassGroupId(carClassId, quickViewCarClassGroup), quickViewGapDeviations, true);
            //companyCurrentRate param is total value in case monitorbase flag = false
            //companyCurrentRate param is Base value in case of monitorbase flag = true
            bool? isMovedUp = null;
            decimal diffAmount = 0;
            if (companyId > 0 && prevSearchResults.Count > 0 && carClassId > 0 && devValue > 0)
            {
                SearchResult resultRow = prevSearchResults.Where(search => search.CompanyID == companyId && search.CarClassID == carClassId).FirstOrDefault();
                if (resultRow != null)
                {
                    if (!monitorBase)
                    {
                        if (companyCurrentRate.HasValue && resultRow.TotalRate.HasValue)
                        {
                            diffAmount = companyCurrentRate.Value - resultRow.TotalRate.Value;
                            if (diffAmount >= 0)
                            {
                                isMovedUp = diffAmount >= devValue ? true : (bool?)null;
                            }
                            else
                            {
                                diffAmount = Math.Abs(diffAmount);
                                isMovedUp = diffAmount >= devValue ? false : (bool?)null;
                            }
                        }
                    }
                    else
                    {
                        if (companyCurrentRate.HasValue && resultRow.BaseRate.HasValue)
                        {
                            diffAmount = companyCurrentRate.Value - resultRow.BaseRate.Value;
                            if (diffAmount >= 0)
                            {
                                isMovedUp = diffAmount >= devValue ? true : (bool?)null;
                            }
                            else
                            {
                                diffAmount = Math.Abs(diffAmount);
                                isMovedUp = diffAmount >= devValue ? false : (bool?)null;
                            }
                        }
                    }
                }
                else
                {
                    if (companyCurrentRate.HasValue)
                    {
                        isMovedUp = true;
                    }
                }
            }

            bool isPositionChange;
            isPositionChange = IsPositionChanged(companyId, carClassId, companyCurrentRate, prevSearchResults, monitorBase, domPrevRate, domCurrentRate);

            //check if position changed and isMovedUp is null then check if competitor rate is changed 
            if (isPositionChange && isMovedUp == null)
            {
                isMovedUp = IsRateChanged(companyId, carClassId, companyCurrentRate, prevSearchResults, monitorBase);
            }
            return isMovedUp;
        }


        public DataTable GetSearchShopDetailsForCSV(long searchSummaryID)
        {

            List<SearchShopCSVDTO> lstSearchShop = (from SS in _context.SearchSummaries
                                                    join SR in _context.SearchResults on SS.ID equals SR.SearchSummaryID
                                                    join RL in _context.RentalLengths on SR.RentalLengthID equals RL.MappedID
                                                    join CC in _context.CarClasses.Where(d => !d.IsDeleted) on SR.CarClassID equals CC.ID
                                                    join CO in _context.Companies.Where(d => !d.IsDeleted) on SR.CompanyID equals CO.ID
                                                    join SRSR in _context.SearchResultSuggestedRates
                                                        on new { searchsummaryid = SS.ID, brandid = CO.ID, rentallengthid = SR.RentalLengthID, carclassid = SR.CarClassID, date = SR.ArrivalDate } equals new { searchsummaryid = SRSR.SearchSummaryID, brandid = SRSR.BrandID, rentallengthid = SRSR.RentalLengthID, carclassid = SRSR.CarClassID, date = SRSR.Date }

                                                    into fulltable
                                                    from SRSR in fulltable.DefaultIfEmpty()
                                                    where SS.ID == searchSummaryID
                                                    orderby RL.MappedID, SR.ArrivalDate, CC.DisplayOrder, CO.ID
                                                    select new SearchShopCSVDTO { Length = RL.Code, Car_Class = CC.Code, Date = SR.ArrivalDate.ToString(), Vendor = CO.Code, Base_Rate = SR.BaseRate.HasValue ? SR.BaseRate.Value : 0, Total_Rate = SR.TotalRate, Suggested_Total_Rate = SRSR.TotalRate, Suggested_Base_Rate = SRSR.BaseRate, Suggested_Max_Base_Rate = SRSR.MaxBaseRate, Suggested_Min_Base_Rate = SRSR.MinBaseRate, LocationBrandId = SS.LocationBrandIDs, isGOV = SS.IsGov.HasValue ? SS.IsGov.Value : false }
                                                    ).ToList();

            long locationBrandId = Convert.ToInt64(lstSearchShop.FirstOrDefault().LocationBrandId);
            Formula locationFormula = _formulaService.GetAll().Where(form => form.LocationBrandID == locationBrandId).FirstOrDefault();

            if (lstSearchShop.Count > 0)
            {
                lstSearchShop.ForEach(d =>
                {
                    d.Date = Convert.ToDateTime(d.Date).ToString("dd-MMM-yyyy");
                    d.Base_Rate = Math.Round(d.Base_Rate, 2);
                    decimal suggestedBase = d.Suggested_Base_Rate.HasValue ? d.Suggested_Base_Rate.Value : 0;
                    d.Suggested_Base_Rate = (d.Suggested_Base_Rate <= d.Suggested_Min_Base_Rate) ? d.Suggested_Min_Base_Rate : ((d.Suggested_Base_Rate > d.Suggested_Max_Base_Rate) ? d.Suggested_Max_Base_Rate : d.Suggested_Base_Rate);
                    if (d.Suggested_Base_Rate.HasValue && suggestedBase != d.Suggested_Base_Rate.Value)
                    {
                        d.Suggested_Total_Rate = CalculateSuggestedTotalRate(locationBrandId, locationFormula, d.Suggested_Base_Rate.Value, d.Length, d.Length.Substring(0, 1), d.isGOV.Value);
                    }
                    d.Suggested_Base_Rate = d.Suggested_Base_Rate.HasValue ? Math.Round(d.Suggested_Base_Rate.Value, 2) : d.Suggested_Base_Rate;
                    d.Suggested_Total_Rate = d.Suggested_Total_Rate.HasValue ? Math.Round(d.Suggested_Total_Rate.Value, 2) : d.Suggested_Total_Rate;

                    string length = d.Length.ToUpper();
                    if (length == "W8" || length == "W9" || length == "W10" || length == "W11")
                    {
                        if (d.Suggested_Base_Rate.HasValue)
                        {
                            d.Additional_Base = Math.Round((decimal)d.Suggested_Base_Rate / 7, 2);
                        }
                    }
                });
            }
            return lstSearchShop.ConvertToDataTable();
        }

        //Get FTB operation methods
        public async Task<SearchResultDTO> GetFTBSummaryReportDefault(long? LoggedInUserID, bool isAdmin)
        {
            if (!LoggedInUserID.HasValue)
            {
                return null;
            }
            //get recent searchSummary with 'success' status
            long completedStatusID = _statusService.GetStatusIDByName("Process Complete");

            var UserLocationBrandID = await _context.UserLocationBrands.Where(obj => obj.UserID == LoggedInUserID.Value).Select(obj => obj.LocationBrandID).ToListAsync();
            if (UserLocationBrandID != null && UserLocationBrandID.Count() > 0)
            {
                //get search summary latest record with completed status and user accessible location
                SearchSummary searchSummary = (from searchSummaries in _context.SearchSummaries
                                               join userLBId in UserLocationBrandID on searchSummaries.LocationBrandIDs equals userLBId.ToString()
                                               where (isAdmin || searchSummaries.CreatedBy == LoggedInUserID.Value)
                                               && searchSummaries.StatusID == completedStatusID
                                               && !string.IsNullOrEmpty(searchSummaries.ShopType) && searchSummaries.ShopType.Equals("SummaryShop", StringComparison.OrdinalIgnoreCase)
                                               orderby searchSummaries.CreatedDateTime descending
                                               select searchSummaries).FirstOrDefault();


                if (searchSummary != null && searchSummary.ID > 0 && DateTime.Now.Date.AddDays(int.Parse(ConfigurationManager.AppSettings["SearchGridDefaultDaysLimit"])) <= searchSummary.CreatedDateTime.Date)
                {
                    long searchSummaryID, scrapperSourceID, locationBrandID, locationID, rentallengthID;
                    //DateTime arrivalDate;

                    searchSummaryID = searchSummary.ID;
                    scrapperSourceID = Common.StringToLongList(searchSummary.ScrapperSourceIDs).Min();
                    locationBrandID = Common.StringToLongList(searchSummary.LocationBrandIDs).Min();

                    LocationBrand locationBrand = _locationBrandService.GetById(locationBrandID);

                    if (locationBrand != null)
                    {
                        locationID = locationBrand.LocationID;
                        rentallengthID = Common.StringToLongList(searchSummary.RentalLengthIDs).Min();
                        //arrivalDate = searchSummary.StartDate.Date;

                        SearchResultProcessedData searchResultProcessedData = _searchResultProcessedData.GetBySearchSummaryId(searchSummaryID).Where(obj => obj.ScrapperSourceID == scrapperSourceID
                          && obj.LocationID == locationID && obj.RentalLengthID == rentallengthID && obj.ClassicViewJSONResult.Equals("Summary View Record", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


                        if (searchResultProcessedData != null
                            && searchResultProcessedData.ID > 0
                            && !string.IsNullOrEmpty(searchResultProcessedData.DailyViewJSONResult))
                        {
                            return new SearchResultDTO
                            {
                                SearchSummaryId = searchSummaryID,
                                brandID = locationBrand.BrandID,
                                finalData = searchResultProcessedData.DailyViewJSONResult,
                            };
                        }
                    }
                }
            }
            return null;
        }
        //End FTB operation methods

        public async Task<SearchResultDTO> GetFTBSummaryReport(string searchSummaryID, string scrapperSourceID, string locationBrandID, string locationID, string brandID, string rentallengthID)
        {
            long _searchSummaryID = 0, _scrapperSourceID = 0, _locationBrandID = 0, _rentallengthID = 0, _locationID = 0, _brandID = 0;

            if (!(long.TryParse(searchSummaryID, out _searchSummaryID) &&
            long.TryParse(scrapperSourceID, out _scrapperSourceID) &&
            long.TryParse(locationBrandID, out _locationBrandID) &&
            long.TryParse(locationID, out _locationID) &&
            long.TryParse(brandID, out _brandID) &&
            long.TryParse(rentallengthID, out _rentallengthID)))
            {
                return null;
            }
            SearchResultProcessedData searchResultProcessedData = _searchResultProcessedData.GetBySearchSummaryId(_searchSummaryID).Where(obj => obj.ScrapperSourceID == _scrapperSourceID
                && obj.LocationID == _locationID && obj.RentalLengthID == _rentallengthID && obj.ClassicViewJSONResult.Equals("Summary View Record", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (searchResultProcessedData != null
                && searchResultProcessedData.ID > 0
                && !string.IsNullOrEmpty(searchResultProcessedData.DailyViewJSONResult))
            {
                return new SearchResultDTO
                {
                    brandID = _brandID,
                    finalData = searchResultProcessedData.DailyViewJSONResult,
                };
            }

            return null;
        }

        private void GenerateSummaryReport(long searchSummaryID, long[] locationBrandIDs, long[] scrapperSourcesIDs, long[] rentalLengthIDs, long[] carClassIDs, List<SearchResult> searchResultsforSeachSummaryID,
            long carClassIDForMCar, List<CarClass> allCarClasses, List<Company> allCompanies, List<LocationCompany> allLocationCompanies, CustomPushJSONDTO customPushJSONDTO)
        {
            List<PushJSONRequestDTO> pushJSONRequestDTOs = new List<PushJSONRequestDTO>();
            //Push JSON operation
            List<ScrapperSource> AllScrapperSource = _scrapperSourceService.GetAll().ToList();
            List<Location> allLocations = _locationService.GetAll().ToList();

            foreach (long locationBrandID in locationBrandIDs)
            {
                LocationBrand locationBrand = _context.LocationBrands.FirstOrDefault(obj => obj.ID == locationBrandID);
                foreach (long scrapperSourceID in scrapperSourcesIDs)
                {
                    foreach (long rentalLengthID in rentalLengthIDs)
                    {
                        List<SearchViewModel.RatesInfo> dailyViewRatesInfolist = new List<SearchViewModel.RatesInfo>();
                        List<SearchResult> filteredSearchResults = searchResultsforSeachSummaryID.Where(obj => obj.ScrapperSourceID == scrapperSourceID && obj.RentalLengthID == rentalLengthID
                              && obj.LocationID == locationBrand.LocationID).ToList();

                        if (filteredSearchResults == null || filteredSearchResults.Count == 0)
                        {
                            continue;
                        }

                        long[] companyIDsBasedOnMCarRates = null;
                        if (filteredSearchResults.Exists(obj => obj.CarClassID == carClassIDForMCar))
                        {
                            //List<long> companyIdsBasedOnMCar = filteredSearchResults.Where(obj => obj.CarClassID == carClassIDForMCar).OrderBy(obj => obj.TotalRate).Select(obj => obj.CompanyID).ToList();
                            List<long> companyIdsBasedOnMCar = filteredSearchResults.Where(obj => obj.CarClassID == carClassIDForMCar).GroupBy(company => company.CompanyID)
                                            .Select(result => new { companyId = result.Key, avgRate = result.Average(x => x.TotalRate) }).OrderBy(a => a.avgRate).Select(a => a.companyId).ToList();

                            var remainingCompanies = filteredSearchResults.Where(obj => !companyIdsBasedOnMCar.Contains(obj.CompanyID));
                            if (remainingCompanies.Count() > 0)
                            {
                                companyIdsBasedOnMCar.AddRange(remainingCompanies.Select(obj => obj.CompanyID).ToList());
                            }

                            companyIDsBasedOnMCarRates = companyIdsBasedOnMCar.Distinct().ToArray();
                        }
                        else
                        {
                            long minCarClass = allCarClasses.FirstOrDefault(car => filteredSearchResults.Exists(result => result.CarClassID == car.ID)).ID;
                            // List<long> companyIdsBasedOnMinCarClass = filteredSearchResults.Where(obj => obj.CarClassID == minCarClass).OrderBy(obj => obj.TotalRate).Select(obj => obj.CompanyID).ToList();
                            List<long> companyIdsBasedOnMinCarClass = filteredSearchResults.Where(obj => obj.CarClassID == minCarClass).GroupBy(company => company.CompanyID)
                                            .Select(result => new { companyId = result.Key, avgRate = result.Average(x => x.TotalRate) }).OrderBy(a => a.avgRate).Select(a => a.companyId).ToList();

                            var remainingCompanies = filteredSearchResults.Where(obj => !companyIdsBasedOnMinCarClass.Contains(obj.CompanyID));
                            if (remainingCompanies.Count() > 0)
                            {
                                companyIdsBasedOnMinCarClass.AddRange(remainingCompanies.Select(obj => obj.CompanyID).ToList());
                            }
                            companyIDsBasedOnMCarRates = companyIdsBasedOnMinCarClass.Distinct().ToArray();
                        }

                        //set dummy field for ordering the company based on MCAR rate
                        filteredSearchResults.ForEach(obj => obj.CompanyRankBasedOnMCcarRate = Array.IndexOf(companyIDsBasedOnMCarRates, obj.CompanyID));

                        #region generate summary shop headers

                        SearchViewModel.FinalData finalData = new SearchViewModel.FinalData();
                        finalData.HeaderInfo = new List<SearchViewModel.HeaderInfo>();
                        finalData.RatesInfo = new List<SearchViewModel.RatesInfo>();

                        foreach (long companyID in companyIDsBasedOnMCarRates)
                        {
                            Company company = allCompanies.Where(obj => obj.ID == companyID).FirstOrDefault();

                            LocationCompany locationCompany = allLocationCompanies.FirstOrDefault(obj => obj.LocationID == locationBrand.LocationID && obj.CompanyID == companyID);

                            //If company is not configured in company settings then remove all records for the company from
                            //search results.
                            if (company == null || company.ID <= 0)// || locationCompany == null || locationCompany.CompanyID <= 0)
                            {
                                filteredSearchResults.RemoveAll(obj => obj.CompanyID == companyID);
                                companyIDsBasedOnMCarRates = companyIDsBasedOnMCarRates.Where(obj => obj != companyID).ToArray();
                                continue;
                            }

                            //add headerinfo object to main list.
                            finalData.HeaderInfo.Add(new SearchViewModel.HeaderInfo
                            {
                                CompanyID = company.ID.ToString(),
                                CompanyName = company.Name,
                                Inside = (locationCompany != null && locationCompany.CompanyID > 0) ? locationCompany.IsTerminalInside : (bool?)null,
                                Logo = company.Logo
                            });
                        }

                        #endregion

                        #region generate average rates for summary shop against each lor,carclass and company

                        foreach (CarClass carClass in allCarClasses.Where(obj => carClassIDs.Contains(obj.ID)))
                        {
                            SearchViewModel.RatesInfo rateInformation = new SearchViewModel.RatesInfo();
                            rateInformation.CarClassID = carClass.ID;
                            rateInformation.CarClass = carClass.Code;
                            rateInformation.CarClassLogo = carClass.Logo;
                            rateInformation.CompanyDetails = new List<SearchViewModel.CompanyDetail>();
                            List<SearchViewModel.CompanyDetail> companyDetails = new List<SearchViewModel.CompanyDetail>();

                            decimal? avgBaseRate = 0;
                            decimal? avgTotalRate = 0;

                            foreach (long CompanyIds in companyIDsBasedOnMCarRates)
                            {
                                SearchViewModel.CompanyDetail companyDetail = new SearchViewModel.CompanyDetail();
                                avgBaseRate = filteredSearchResults.Where(obj => obj.CarClassID == carClass.ID && obj.CompanyID == CompanyIds)
                                .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate).Average(rates => rates.BaseRate);

                                avgTotalRate = filteredSearchResults.Where(obj => obj.CarClassID == carClass.ID && obj.CompanyID == CompanyIds)
                                             .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate).Average(rates => rates.TotalRate);

                                companyDetail = filteredSearchResults.Where(obj => obj.CarClassID == carClass.ID && obj.CompanyID == CompanyIds)
                                           .OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate)
                                               .Select(obj => new SearchViewModel.CompanyDetail()
                                               {
                                                   CompanyID = obj.CompanyID,
                                                   CompanyName = allCompanies.First(obj1 => obj1.ID == obj.CompanyID).Name,
                                                   BaseValue = decimal.Round(avgBaseRate.Value, MidpointRounding.AwayFromZero),
                                                   TotalValue = avgTotalRate.Value,
                                                   IsMovedUp = null,
                                                   Islowest = false,
                                                   IslowestAmongCompetitors = false,
                                                   CompanyRankBasedOnMCcarRate = obj.CompanyRankBasedOnMCcarRate
                                               }).FirstOrDefault();

                                //add empty row for this company
                                if (companyDetail != null)
                                {
                                    PushJSONRequestDTO opushJSONRequestDTO = new PushJSONRequestDTO();
                                    opushJSONRequestDTO.CarClass = carClass.Code;
                                    opushJSONRequestDTO.SearchId = Convert.ToString(searchSummaryID);
                                    opushJSONRequestDTO.LOR = Convert.ToString(rentalLengthID);
                                    opushJSONRequestDTO.BaseRate = Convert.ToString(companyDetail.BaseValue);
                                    opushJSONRequestDTO.TotalRate = Convert.ToString(companyDetail.TotalValue);
                                    opushJSONRequestDTO.VendCd = allCompanies.First(obj1 => obj1.ID == CompanyIds).Code;
                                    opushJSONRequestDTO.CityCd = allLocations.FirstOrDefault(obj => obj.ID == locationBrand.LocationID).Code;
                                    opushJSONRequestDTO.DataSource = AllScrapperSource.FirstOrDefault(obj => obj.ID == scrapperSourceID).Code;
                                    opushJSONRequestDTO.ShopStartDate = customPushJSONDTO.ShopStartDate;
                                    opushJSONRequestDTO.ShopEndDate = customPushJSONDTO.ShopEndDate;
                                    opushJSONRequestDTO.User = customPushJSONDTO.User;

                                    pushJSONRequestDTOs.Add(opushJSONRequestDTO);

                                    companyDetails.Add(companyDetail);
                                }
                            }

                            #region highlighting logic

                            if (companyDetails.Count > 0)
                            {
                                companyDetails = companyDetails.Distinct(new CompanyDetailsComparare(a => a.CompanyID)).ToList();

                                decimal minTotalValue = 0;

                                minTotalValue = companyDetails.Where(obj2 => obj2.TotalValue.HasValue).Min(obj => obj.TotalValue.Value);
                                companyDetails.Where(obj => obj.TotalValue.Value == minTotalValue).All(obj2 => obj2.Islowest = true);


                                long[] directCompetitors = Common.StringToLongList(locationBrand.CompetitorCompanyIDs).ToArray();

                                Dictionary<long, decimal> directCompetitorsWithRate = new Dictionary<long, decimal>();

                                var dc = companyDetails.Where(obj => obj.TotalValue.HasValue && directCompetitors.Contains(obj.CompanyID));
                                if (dc.Count() > 0)
                                {
                                    directCompetitorsWithRate = dc.ToList().GroupBy(comp => comp.CompanyID).Select(grp => grp.First()).OrderBy(obj => obj.TotalValue.Value).ToDictionary(obj => obj.CompanyID, obj => obj.TotalValue.Value);
                                }
                                decimal? rateMoreThanLowestRateDirectCompetitor = null;
                                if (directCompetitorsWithRate.Count > 0)
                                {
                                    //Special Scenario1: If lowest rate competitor is direct competitor(from Location Brand Settings) then we have to set the flag for 
                                    //'IslowestAmongCompetitors' to second lowest rate provider. 
                                    if (companyDetails.FirstOrDefault(obj => obj.Islowest) != null && companyDetails.Where(obj => obj.Islowest).Select(obj1 => obj1.CompanyID).ToArray().Any(companyId => directCompetitorsWithRate.Keys.Contains(companyId)))
                                    {
                                        if (directCompetitorsWithRate.Count > 1)
                                        {
                                            //direct competitor providing lowest rate amoung all competitors; go for second lowest competitor
                                            //Special Scenario2:If special scenario 1 satisfy then we will check for lowestRateAmoungDirectCompetitor
                                            //if myCompany (brand) is providing lower/equal rate than lowestRateAmoungDirectCompetitor then myCompany should be highlighted in green. 
                                            //Note: This scenario handled in front end

                                            //Special scenario3: If two or more direct competitor is providing lowest rate then highlight such competitor in yellow
                                            //Special scenario4: If scenario 3 satisfy, then highligh green should be shift to rate provider for more rate than lowest Rate providers

                                            int lowestRateProviders = directCompetitorsWithRate.Values.Count(obj => obj == minTotalValue);
                                            if (directCompetitorsWithRate.Count > lowestRateProviders)
                                            {
                                                rateMoreThanLowestRateDirectCompetitor = directCompetitorsWithRate.Skip(lowestRateProviders).First().Value;
                                            }
                                        }
                                        else
                                        {
                                            //only one direct competitor found and competitor is itself lowest rate provider.
                                            //Highligh my companay as lowest green
                                            if (companyDetails.Exists(obj => obj.CompanyID == locationBrand.BrandID))
                                            {
                                                companyDetails.Where(obj => obj.CompanyID == locationBrand.BrandID).All(obj => obj.IslowestAmongCompetitors = true);
                                            }
                                        }
                                    }
                                    //Set IslowestAmongCompetitors flag  
                                    if (rateMoreThanLowestRateDirectCompetitor.HasValue)
                                    {
                                        companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.TotalValue == rateMoreThanLowestRateDirectCompetitor.Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                    }
                                    else
                                    {
                                        companyDetails.Where(obj => directCompetitors.Contains(obj.CompanyID) && obj.TotalValue == directCompetitorsWithRate.First().Value).ToList().ForEach(obj => obj.IslowestAmongCompetitors = true);
                                    }
                                }

                            }

                            #endregion

                            #region add empty row as no one company providing rate for this car class

                            var remainingCompanies = companyIDsBasedOnMCarRates.Except(companyDetails.Select(obj => obj.CompanyID).Distinct());
                            if (remainingCompanies.Count() > 0)
                            {
                                foreach (long companyID in remainingCompanies)
                                {

                                    companyDetails.Add(new SearchViewModel.CompanyDetail()
                                    {
                                        CompanyID = companyID,
                                        CompanyName = allCompanies.First(obj1 => obj1.ID == companyID).Name,

                                        BaseValue = (decimal?)null,
                                        TotalValue = (decimal?)null,

                                        Islowest = false,
                                        IslowestAmongCompetitors = false,
                                        CompanyRankBasedOnMCcarRate = filteredSearchResults.Where(obj => obj.CompanyID == companyID).First().CompanyRankBasedOnMCcarRate
                                    });

                                }
                            }

                            rateInformation.CompanyDetails.AddRange(companyDetails.OrderBy(obj1 => obj1.CompanyRankBasedOnMCcarRate));
                            finalData.RatesInfo.Add(rateInformation);

                            #endregion

                        }

                        dailyViewRatesInfolist.AddRange(finalData.RatesInfo);

                        _searchResultProcessedData.Add(new SearchResultProcessedData
                        {
                            SearchSummaryID = searchSummaryID,
                            ScrapperSourceID = scrapperSourceID,
                            LocationID = locationBrand.LocationID,
                            RentalLengthID = rentalLengthID,
                            DateFilter = null,
                            DailyViewJSONResult = Convert.ToString(new JavaScriptSerializer().Serialize(finalData)),
                            ClassicViewJSONResult = "Summary View Record",
                            CreatedDateTime = DateTime.Now,
                        });

                        #endregion
                    }
                }
            }
            //call for generate push json request JSON.
            string JSONFlag = Convert.ToString(ConfigurationManager.AppSettings["PushJSONStartFlag"]);
            if (pushJSONRequestDTOs.Count() > 0 && JSONFlag == "true")
            {
                _postJSONLogService.SavePushJSONSummary(pushJSONRequestDTOs, searchSummaryID);
            }
        }

        private async Task<int> SendQuickViewEmail(long quickViewId, long searchId, string shopDate, long shopCreatedBy, List<CarClassCodeOrderDTO> carClasses)
        {
            try
            {
                List<QuickViewResults> quickViewResults = _context.QuickViewResults.Where(d => d.SearchSummaryId == searchId && d.QuickViewId == quickViewId && (d.IsMovedUp != null || d.IsPositionChange.Value)).ToList();
                if (quickViewResults.Count > 0)
                {
                    LogHelper.WriteToLogFile(" car classes :" + carClasses.Count + "classes" + string.Join(", ", (carClasses.OrderBy(d => d.DisplayOrder).Select(d => d.CarClassCode).Distinct())),
                        LogHelper.GetLogFilePath("QuickViewEmail"));
                    LocationBrand locationBrand = _locationBrandService.GetById(quickViewResults.FirstOrDefault().LocationBrandId, false);
                    QuickViewEmailDTO objQuickViewEmailDTO = new QuickViewEmailDTO();
                    objQuickViewEmailDTO.BrandLocation = locationBrand.LocationBrandAlias;
                    objQuickViewEmailDTO.CarClasses = string.Join(", ", carClasses.OrderBy(d => d.DisplayOrder).Select(d => d.CarClassCode).Distinct());
                    List<long> rentalLengths = quickViewResults.Select(d => d.RentalLengthId).Distinct().ToList();
                    objQuickViewEmailDTO.LOR = string.Join(", ", _rentalLengthService.GetAll().OrderBy(f => f.MappedID).Where(d => rentalLengths.Contains(d.MappedID)).Select(e => e.Code));
                    objQuickViewEmailDTO.ShopDate = shopDate;
                    objQuickViewEmailDTO.ChangeDates = string.Join(", ", quickViewResults.OrderBy(d => d.Date).Select(d => d.Date.ToString("dd")).Distinct());
                    User objUser = _userService.GetById(shopCreatedBy, false);
                    objQuickViewEmailDTO.Email = objUser.EmailAddress;
                    objQuickViewEmailDTO.UserName = objUser.FirstName;

                    await EmailHelper.SendEmailAsync(EmailTemplateHelper.CreateQuickViewEmail(objQuickViewEmailDTO));
                    LogHelper.WriteToLogFile("Email Send To: " + objQuickViewEmailDTO.Email + " of quick view results for searchid:" + searchId, LogHelper.GetLogFilePath("QuickViewEmail"));

                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in send Quick View Email: " + ex.Message + "Inner Exception" + ex.InnerException
                      + ", Stack Trace: " + ex.StackTrace, LogHelper.GetLogFilePath("QuickViewEmail"));

                return 0;
            }
        }

        public bool IsPositionChanged(long companyId, long carClassId, decimal? companyCurrentRate, List<SearchResult> prevSearchResults, bool monitorBase, decimal? domPrevRate, decimal? domCurrentRate)
        {

            bool isPositionChange = false;
            //decimal diffAmount = 0;
            if (companyId > 0 && prevSearchResults.Count > 0 && carClassId > 0)
            {
                SearchResult resultRow = prevSearchResults.Where(search => search.CompanyID == companyId && search.CarClassID == carClassId).FirstOrDefault();
                if (resultRow != null)
                {
                    if (!monitorBase)
                    {
                        if (companyCurrentRate.HasValue && resultRow.TotalRate.HasValue)
                        {
                            isPositionChange = (domPrevRate.HasValue && domCurrentRate.HasValue) ? PositionChanged(companyCurrentRate.Value, resultRow.TotalRate.Value, domPrevRate.Value, domCurrentRate.Value) : false;
                        }
                    }
                    else
                    {
                        if (companyCurrentRate.HasValue && resultRow.BaseRate.HasValue)
                        {
                            isPositionChange = (domPrevRate.HasValue && domCurrentRate.HasValue) ? PositionChanged(companyCurrentRate.Value, resultRow.BaseRate.Value, domPrevRate.Value, domCurrentRate.Value) : false;
                        }
                    }
                }
            }
            return isPositionChange;
        }

        /// <summary>
        /// get competitor groupId from company Id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="quickViewGroupCompetitors"></param>
        /// <returns></returns>

        private long GetCompetitorGroupId(long companyId, IEnumerable<QuickViewGroupCompaniesDTO> quickViewGroupCompetitors)
        {
            long companyGroupId = 0;
            if (companyId > 0 && quickViewGroupCompetitors != null)
            {
                companyGroupId = quickViewGroupCompetitors.Where(group => group.CompanyId == companyId).FirstOrDefault().GroupId;
            }
            return companyGroupId;
        }

        /// <summary>
        /// get car class groupId from CarClassID
        /// </summary>
        /// <param name="carClassId"></param>
        /// <param name="quickViewCarClassGroup"></param>
        /// <returns></returns>

        private long GetCarClassGroupId(long carClassId, IEnumerable<QuickViewCarClassGroups> quickViewCarClassGroup)
        {
            long carClassGroupId = 0;
            if (carClassId > 0 && quickViewCarClassGroup != null)
            {
                if (quickViewCarClassGroup.Where(group => group.CarClassID == carClassId) != null && quickViewCarClassGroup.Where(group => group.CarClassID == carClassId).Count() > 0)
                    carClassGroupId = quickViewCarClassGroup.Where(group => group.CarClassID == carClassId).FirstOrDefault().GroupID;
            }
            return carClassGroupId;
        }

        private decimal GetGapDeviationValue(long companyGroupId, long carClassGroupId, IEnumerable<QuickViewGapDevSettingsDTO> quickViewGapDeviations, bool fetchDev)
        {
            decimal diffValue = 0;
            if (companyGroupId > 0 && carClassGroupId > 0 && quickViewGapDeviations != null)
            {
                if (fetchDev)
                {
                    diffValue = quickViewGapDeviations.Where(quick => quick.CarClassGroupId == carClassGroupId && quick.CompetitorGroupId == companyGroupId).FirstOrDefault().DeviationValue;
                }
                else
                {
                    diffValue = quickViewGapDeviations.Where(quick => quick.CarClassGroupId == carClassGroupId && quick.CompetitorGroupId == companyGroupId).FirstOrDefault().GapValue;
                }
            }
            return diffValue;
        }

        private bool PositionChanged(decimal companyCurrentRate, decimal companyPreviousRate, decimal domBrandPreviousRate, decimal domBrandCurrentRate)
        {
            bool isPositionChanged;
            if (companyPreviousRate < domBrandCurrentRate && companyCurrentRate < domBrandCurrentRate)
            {
                isPositionChanged = false;
            }
            else if (companyPreviousRate > domBrandCurrentRate && companyCurrentRate > domBrandCurrentRate)
            {
                isPositionChanged = false;
            }
            else if (companyPreviousRate == domBrandCurrentRate && companyCurrentRate == domBrandCurrentRate)
            {
                isPositionChanged = false;
            }
            else
            {
                isPositionChanged = true;
            }
            return isPositionChanged;
        }

        public bool? IsRateChanged(long companyId, long carClassId, decimal? companyCurrentRate, List<SearchResult> prevSearchResults, bool monitorBase)
        {
            //companyCurrentRate param is total value in case monitorbase flag = false
            //companyCurrentRate param is Base value in case of monitorbase flag = true
            bool? isRateChange = null;
            if (companyId > 0 && prevSearchResults.Count > 0 && carClassId > 0)
            {
                SearchResult resultRow = prevSearchResults.Where(search => search.CompanyID == companyId && search.CarClassID == carClassId).FirstOrDefault();
                if (resultRow != null && companyCurrentRate.HasValue)
                {
                    if (!monitorBase)
                    {
                        if (companyCurrentRate.HasValue && resultRow.TotalRate.HasValue)
                        {
                            isRateChange = (companyCurrentRate.Value > resultRow.TotalRate.Value) ? true : ((companyCurrentRate.Value < resultRow.TotalRate.Value) ? false : ((companyCurrentRate.Value == resultRow.TotalRate.Value) ? false : (bool?)null));
                        }
                    }
                    else
                    {
                        if (companyCurrentRate.HasValue && resultRow.BaseRate.HasValue)
                        {
                            isRateChange = (companyCurrentRate.Value > resultRow.BaseRate.Value) ? true : ((companyCurrentRate.Value < resultRow.BaseRate.Value) ? false : ((companyCurrentRate.Value == resultRow.BaseRate.Value) ? false : (bool?)null));
                        }
                    }
                }
                else
                {
                    if ((companyCurrentRate.HasValue && resultRow == null) || (!companyCurrentRate.HasValue && resultRow != null))
                    {
                        isRateChange = true;
                    }
                }
            }
            return isRateChange;
        }

    }
}



