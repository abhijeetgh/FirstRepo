using System;
using System.Collections.Generic;
using System.Linq;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
using RateShopper.Domain.DTOs;
using RateShopper.Services.Helper;
using System.Globalization;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RateShopper.Core.Email_Helper;
using Newtonsoft.Json;

namespace RateShopper.Services.Data
{
    public class FTBScheduleJobService : BaseService<FTBScheduledJob>, IFTBScheduleJobService
    {
        private IScheduledJobFrequencyService _scheduledJobFrequencyService;
        private IWeekDayService _weekDayService;
        private IUserLocationBrandsService _userLocationBrandsService;
        private IFTBTargetService _ftbTargetService;
        private IFTBRatesService _ftbRatesService;
        private ISplitMonthDetailsService _splitMonthDetailsService;
        private IGlobalTetherSettingService _globalTetherSettingService;
        private IFormulaService _formulaService;
        private ITSDTransactionsService _tsdTransactionService;
        private IUserService _userService;
        private IFTBRatesSplitMonthsService _ftbRatesSplitMonthsService;
        private IRentalLengthService _rentalLengthService;
        private ILocationBrandRentalLengthService _locationBrandRentalLengthService;
        public FTBScheduleJobService(IEZRACRateShopperContext context, ICacheManager cacheManager, IScheduledJobFrequencyService scheduledJobFrequencyService, IWeekDayService weekDayService,
            IFTBTargetService ftbTargetService, IFTBRatesService ftbRatesService, ISplitMonthDetailsService splitMonthDetailsService, IGlobalTetherSettingService globalTetherSettingService,
            IFormulaService formulaService, ITSDTransactionsService tsdTransactionService, IUserService userService, IUserLocationBrandsService userLocationBrandsService, IFTBRatesSplitMonthsService ftbRatesSplitMonthsService,
            ILocationBrandRentalLengthService locationBrandRentalLengthService, IRentalLengthService rentalLengthService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<FTBScheduledJob>();
            _cacheManager = cacheManager;
            _scheduledJobFrequencyService = scheduledJobFrequencyService;
            _weekDayService = weekDayService;
            _ftbTargetService = ftbTargetService;
            _ftbRatesService = ftbRatesService;
            _splitMonthDetailsService = splitMonthDetailsService;
            _globalTetherSettingService = globalTetherSettingService;
            _formulaService = formulaService;
            _userService = userService;
            _tsdTransactionService = tsdTransactionService;
            _userLocationBrandsService = userLocationBrandsService;
            _ftbRatesSplitMonthsService = ftbRatesSplitMonthsService;
            _locationBrandRentalLengthService = locationBrandRentalLengthService;
            _rentalLengthService = rentalLengthService;
        }

        private string garsFee = string.Empty;


        public string SaveScheduledJob(FTBJobEditDTO ftbJobEditDTO)
        {
            if (ftbJobEditDTO == null || ftbJobEditDTO.StartMonth <= 0)
            {
                return "Failed";
            }

            FTBScheduledJob ftbScheduledJob = new FTBScheduledJob();

            #region Save Scheduled Job

            DateTime daysToStartDate = new DateTime(ftbJobEditDTO.StartYear, ftbJobEditDTO.StartMonth, 1);

            if (!ftbJobEditDTO.JobId.HasValue)
            {
                ftbScheduledJob.LocationBrandID = ftbJobEditDTO.LocationBrandID;
                ftbScheduledJob.StartDate = daysToStartDate;
                ftbScheduledJob.EndDate = daysToStartDate.AddMonths(1).AddDays(-1);
                ftbScheduledJob.UpdatedBy = ftbJobEditDTO.LoggedInUserId;
                ftbScheduledJob.UpdatedDateTime = DateTime.Now;
            }

            if (ftbJobEditDTO.JobId.HasValue)
            {
                //This is existing job and we need to modify it
                ftbScheduledJob = _context.FTBScheduledJobs.Where(obj => obj.ID == ftbJobEditDTO.JobId).FirstOrDefault();
                ftbScheduledJob.UpdatedBy = ftbJobEditDTO.LoggedInUserId;
                ftbScheduledJob.UpdatedDateTime = DateTime.Now;
                _context.Entry(ftbScheduledJob).State = System.Data.Entity.EntityState.Modified;
                _cacheManager.Remove(typeof(FTBScheduledJob).ToString() + "_" + ftbJobEditDTO.JobId);
            }
            else
            {

                ftbScheduledJob.CreatedBy = ftbJobEditDTO.LoggedInUserId;
                ftbScheduledJob.CreatedDateTime = DateTime.Now;
                _context.FTBScheduledJobs.Add(ftbScheduledJob);

            }
            if (ftbScheduledJob == null)
            {
                return "Failed";
            }



            //System.Data.Entity.DbFunctions.TruncateTime(obj.Date) == System.Data.Entity.DbFunctions.TruncateTime(SearchDate)
            if (((_context.FTBRates.Where(rates => rates.LocationBrandId == ftbScheduledJob.LocationBrandID) != null) &&
                (_context.FTBRates.Where(rates => rates.LocationBrandId == ftbScheduledJob.LocationBrandID && DbFunctions.TruncateTime(rates.Date) == DbFunctions.TruncateTime(ftbScheduledJob.StartDate)).Count()) == 0)
                || (_context.FTBTargets.Where(target => target.LocationBrandId == ftbScheduledJob.LocationBrandID && DbFunctions.TruncateTime(target.Date) == DbFunctions.TruncateTime(ftbScheduledJob.StartDate) && target.Target > 0).Count() == 0))
            {
                return "NoRatesTargets";
            }

            //Edit + New job created 
            ftbScheduledJob.ScheduledJobFrequencyID = ftbJobEditDTO.ScheduledJobFrequencyID;
            ftbScheduledJob.RunTime = ftbJobEditDTO.RunTime;
            ftbScheduledJob.DaysToRunStartDate = ftbJobEditDTO.StartDate;
            ftbScheduledJob.DaysToRunEndDate = daysToStartDate.AddSeconds(-1);
            ftbScheduledJob.IsActiveTethering = ftbJobEditDTO.IsActiveTethering;
            ftbScheduledJob.JobScheduleWeekDays = ftbJobEditDTO.JobScheduleWeekDays;

            if (ftbScheduledJob.DaysToRunEndDate < ftbScheduledJob.DaysToRunStartDate)
            {
                return "NoMatchingDate";
            }

            //only enable new jobs
            if (!ftbJobEditDTO.JobId.HasValue && ftbScheduledJob.DaysToRunEndDate.Date >= DateTime.Now.Date)
            {
                ftbScheduledJob.IsEnabled = true;
            }

            SetNextRunDateTime(ftbScheduledJob);

            if (ftbScheduledJob.NextRunDateTime != null)
            {
                _context.SaveChanges();
                _cacheManager.Remove(typeof(FTBScheduledJob).ToString());

                FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
                ftbAutomationScenarioDTO.StartDate = daysToStartDate;
                ftbAutomationScenarioDTO.IsFTBJob = true;
                ftbAutomationScenarioDTO.LocationBrandID = ftbJobEditDTO.LocationBrandID;
                ftbAutomationScenarioDTO.LoggedUserID = ftbJobEditDTO.LoggedInUserId;
                ftbAutomationScenarioDTO = CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);
                if (ftbAutomationScenarioDTO.IsReturnMsg)
                {
                    if (ftbAutomationScenarioDTO.ReturnMessage == "notconfiguredblackoutdates")
                    {
                        return "Successnotconfiguredblackoutdates";
                    }
                    else
                    {
                        return "Success" + ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString();
                    }
                }
            }
            else
            {
                return "NoMatchingDate";
            }

            #endregion

            return "Success";
        }

        public void SetNextRunDateTime(FTBScheduledJob ftbscheduledJob)
        {
            //get ScheduledJobFrequency for the job
            ScheduledJobFrequency scheduledJobFrequency = _scheduledJobFrequencyService.GetById(ftbscheduledJob.ScheduledJobFrequencyID, false);

            if (scheduledJobFrequency != null && scheduledJobFrequency.ID > 0)
            {
                DateTime nextRunDateTime = ftbscheduledJob.DaysToRunStartDate > DateTime.Now ? ftbscheduledJob.DaysToRunStartDate : DateTime.Now;//.AddMinutes(-2);
                //Current time without sec;				
                DateTime nextRunDateTimeWithoutSec = nextRunDateTime.Date.AddHours(nextRunDateTime.Hour).AddMinutes(nextRunDateTime.Minute);
                IEnumerable<WeekDay> weekDays = _weekDayService.GetAll();
                long[] jobScheduledWeekDayIds = Common.StringToLongList(ftbscheduledJob.JobScheduleWeekDays).ToArray();
                string[] jobScheduledWeekDays = new string[7];
                if (jobScheduledWeekDayIds != null && jobScheduledWeekDayIds.Count() > 0)
                {
                    Array.Sort(jobScheduledWeekDayIds);
                    jobScheduledWeekDays = weekDays.Where(obj => jobScheduledWeekDayIds.Contains(obj.ID)).Select(obj1 => obj1.Day).ToArray();
                }
                bool nextRunFound = false;
                bool endDayReached = false;
                if (scheduledJobFrequency.DayInterval.HasValue)
                {
                    #region Day Interval
                    //daily, weekly
                    if (!string.IsNullOrEmpty(ftbscheduledJob.JobScheduleWeekDays))
                    {
                        string runDate = nextRunDateTimeWithoutSec.ToString("yyyy-MM-dd") + " " + ftbscheduledJob.RunTime;
                        nextRunDateTime = DateTime.ParseExact(runDate, "yyyy-MM-dd h:mm tt", CultureInfo.CurrentCulture);
                        do
                        {
                            nextRunDateTime = GetNextRunUsingJobScheduleWeekDays(jobScheduledWeekDays, ftbscheduledJob, nextRunDateTime, out nextRunFound, out endDayReached);
                            //Also consider 2 min delay for saving the data and service trigger
                            if (nextRunDateTime != null && nextRunDateTime.AddMinutes(-2) < DateTime.Now)
                            {
                                //Run on next elapsed time
                                nextRunDateTime = nextRunDateTime.AddDays(scheduledJobFrequency.DayInterval.Value);
                                nextRunFound = false;
                            }
                        } while (!nextRunFound && !endDayReached);
                    }
                    ftbscheduledJob.NextRunDateTime = (ftbscheduledJob.DaysToRunEndDate >= nextRunDateTime) ? nextRunDateTime : (DateTime?)null;
                    #endregion
                }
            }
        }

        private DateTime GetNextRunUsingJobScheduleWeekDays(string[] jobScheduledWeekDays, FTBScheduledJob ftbScheduledJob, DateTime nextRunDateTime, out bool nextRunFound, out bool endDayReached)
        {
            nextRunFound = false;
            endDayReached = false;
            for (; nextRunDateTime <= ftbScheduledJob.DaysToRunEndDate; nextRunDateTime = nextRunDateTime.AddDays(1))
            {
                if (jobScheduledWeekDays.Contains(nextRunDateTime.DayOfWeek.ToString()))
                {
                    nextRunFound = true;
                    break;
                }
            }
            if (!nextRunFound && nextRunDateTime >= ftbScheduledJob.DaysToRunEndDate)
            {
                endDayReached = true;
            }
            return nextRunDateTime;
        }

        public async Task<List<FTBMonthsDTO>> GetLocationBrandJobMonths(long locationBrandId)
        {
            List<FTBMonthsDTO> ftbScheduledMonths = await (from ftb in _context.FTBScheduledJobs
                                                           where ftb.LocationBrandID == locationBrandId && !ftb.IsDeleted
                                                           select new FTBMonthsDTO { Month = ftb.StartDate.Month, Year = ftb.StartDate.Year, IsScheduledStopped = ftb.IsEnabled }).ToListAsync();
            return ftbScheduledMonths;
        }

        public async Task<string> RunFTBAutomationShops()
        {

            //Step1: Get all FTB jobs with NextRunTime == this time and isEnabled == true and IsDeleted == false
            DateTime thisTime = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            DateTime thisTimeWithNextMin = thisTime.AddMinutes(1);
            List<FTBScheduledJob> ftbJobsToRun = _context.FTBScheduledJobs.Where(obj => obj.IsEnabled && !obj.IsDeleted && obj.NextRunDateTime.HasValue
                && obj.NextRunDateTime <= thisTimeWithNextMin && obj.DaysToRunEndDate >= thisTime).ToList();

            IEnumerable<LocationBrandDetailsDTO> locationBrandDetails = null;
            List<FTBRatesTSDLogs> ftbRatesTSDUpdateslog = null;

            if (ftbJobsToRun != null && ftbJobsToRun.Count() > 0)
            {
                locationBrandDetails = await getAllLocationBrandDetails();

                #region Set LastRuntime, Calculate and set NextRunTime, set ExecutionInProgress

                DateTime DateWithTimeNoSeconds = thisTime;
                foreach (FTBScheduledJob job in ftbJobsToRun)
                {
                    // Parallel.ForEach(ftbJobsToRun, (job) =>
                    //{
                    //For scheduled Job, check location-Brands is not deleted, If yes then delete such job	
                    if (!locationBrandDetails.Any(location => location.ID == job.LocationBrandID))
                    {
                        _context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                        _cacheManager.Remove(typeof(FTBScheduledJob).ToString() + "_" + job.ID);
                        job.IsDeleted = true;
                    }
                    _context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    _cacheManager.Remove(typeof(FTBScheduledJob).ToString() + "_" + job.ID);
                    UpdateLastRunNextRunAndExecutionInProgress(job, DateWithTimeNoSeconds);
                    _context.SaveChanges();
                    _cacheManager.Remove(typeof(FTBScheduledJob).ToString());

                    //});
                }

                #endregion

                ftbRatesTSDUpdateslog = (from ftbJob in ftbJobsToRun join ratesTSD in _context.FTBRatesTSDLogs on ftbJob.ID equals ratesTSD.ScheduleJobId select ratesTSD).ToList();
            }
            else
            {
                return string.Empty;
            }

            #region Fetch Reservation count and update rates

            FTBRate ftbRates = null;
            List<FTBRatesSplitMonthDetails> ftbRatesSplitMonths = _ftbRatesSplitMonthsService.GetAll().ToList();
            List<GlobalTetherSetting> globalTetherSettings = _globalTetherSettingService.GetAll().ToList();
            List<Formula> formula = _formulaService.GetAll().ToList();
            List<User> rateShopperUsers = _userService.GetAll().ToList();
            List<RentalLength> masterRentalLengths = _rentalLengthService.GetRentalLength().ToList();
            bool isTetherActive = false;
            string userName = string.Empty;
            bool isSplitMonth = false;
            foreach (FTBScheduledJob ftbScheduledJob in ftbJobsToRun)
            {
                //Parallel.ForEach(ftbJobsToRun, (ftbScheduledJob) =>
                //{
                isTetherActive = ftbScheduledJob.IsActiveTethering;

                //create shop date array 
                DateTime[] shopDates = Enumerable.Range(0, 1 + ftbScheduledJob.EndDate.Date.Subtract(ftbScheduledJob.StartDate.Date).Days)
                      .Select(offset => ftbScheduledJob.StartDate.AddDays(offset)).ToArray();

                LocationBrandDetailsDTO locationBrand = locationBrandDetails.Where(location => location.ID == ftbScheduledJob.LocationBrandID).FirstOrDefault();
                List<LocationBrandRentalLength> locationRentalLengths = _locationBrandRentalLengthService.GetAll().Where(d => d.LocationBrandID == ftbScheduledJob.LocationBrandID).ToList();
                /*create request url to fetch reservation count from TSD end point*/

                ProviderRequest objRequest = CreateRequestToFetchReservation(ftbScheduledJob.StartDate.ToString("MM-dd-yyyy"), ftbScheduledJob.EndDate.ToString("MM-dd-yyyy"), locationBrand.Code);

                var task1 = fetchTargetDetails(locationBrand.ID, ftbScheduledJob.StartDate.Year, ftbScheduledJob.StartDate.Month);
                var task2 = TSDReservationHelper.fetchTSDReservationCount(objRequest);
                await Task.WhenAll(task1, task2);

                List<FTBTargetDTO> targetdetailsDTO = task1.Result;
                IEnumerable<TSDReservationDTO> reservationsPerDay = task2.Result;

                List<FTBRateDetailsDTO> ftbRatesDetails = null;
                DateTime[] blackOutDates = null;
                Dictionary<long, string> rentalLengths = null;
                Dictionary<long, string> carClasses = null;

                userName = rateShopperUsers.Where(user => user.ID == ftbScheduledJob.UpdatedBy).Select(user => user.UserName).FirstOrDefault();
                ftbRates = _ftbRatesService.GetAll().Where(ftbRate => ftbRate.LocationBrandId == ftbScheduledJob.LocationBrandID && ftbRate.Date.Date == ftbScheduledJob.StartDate.Date).FirstOrDefault();

                //if job has black out period then create blackout date array
                string blackOut = string.Empty;
                if (ftbRates != null && ftbRates.HasBlackOutPeroid.HasValue && ftbRates.HasBlackOutPeroid.Value)
                {
                    DateTime blackOutStartDate = ftbRates.BlackOutStartDate.Value;
                    DateTime blackOutEndDate = ftbRates.BlackOutEndDate.Value;
                    blackOutDates = Enumerable.Range(0, 1 + blackOutEndDate.Date.Subtract(blackOutStartDate.Date).Days)
                  .Select(offset => blackOutStartDate.AddDays(offset)).ToArray();
                    blackOut = blackOutStartDate.ToString("MM/dd/yyyy") + " - " + blackOutEndDate.ToString("MM/dd/yyyy");
                }

                //remove blackout dates from shop dates 
                if (blackOutDates != null && blackOutDates.Length > 0)
                {
                    shopDates = shopDates.Except(blackOutDates).ToArray();
                }
                if (ftbRates != null)
                {
                    isSplitMonth = ftbRates.IsSplitMonth.HasValue ? ftbRates.IsSplitMonth.Value : false;
                    ftbRatesDetails = await _ftbRatesService.fetchFTBRatesDetail(ftbRates.ID);
                    rentalLengths = (from ftb in ftbRatesDetails orderby ftb.RentalLengthId select new { ID = ftb.RentalLengthId, LOR = ftb.RentalLengthCode }).Distinct().ToDictionary(length => length.ID, length => length.LOR);
                    carClasses = (from ftb in ftbRatesDetails orderby ftb.CarClassId select new { ID = ftb.CarClassId, Code = ftb.CarClass }).Distinct().ToDictionary(car => car.ID, car => car.Code);
                }

                int splitIndex = 1;

                #region iterate over days configured of a job and compare % of target reached

                long reservationCount = 0;
                //mail request create 
                List<FTBJobShopDateDetails> ftbJobShopDateDetailsList = new List<FTBJobShopDateDetails>();
                FTBEmailCommonSettings emailCommonSettings = new FTBEmailCommonSettings();

                //emailCommonSettings.EmailRecipients.Add("anandkumarl@cybage.com");
                FTBTargetsDetailDTO targetSlabDTO = null;
                foreach (string dayOfWeek in targetdetailsDTO.Select(day => day.Day).ToList())
                {
                    foreach (DateTime shopDate in shopDates.Where(shop => shop.DayOfWeek.ToString().Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase)))
                    {
                        FTBJobShopDateDetails ftbJobShopDate = new FTBJobShopDateDetails();

                        DateTime shopTSDDate = shopDate;
                        FTBRatesTSDLogs ftbTSDLog = null;
                        if (isSplitMonth)
                        {
                            //splitIndex = splitMonths.Where(day => day.EndDay >= shopTSDDate.Day && day.StartDay <= shopTSDDate.Day).Select(a => a.SplitIndex).FirstOrDefault();
                            splitIndex = ftbRatesSplitMonths.Where(ratesSplit => ratesSplit.EndDay >= shopTSDDate.Day && ratesSplit.StartDay <= shopTSDDate.Day && ratesSplit.FTBRatesId == ftbRates.ID).Select(a => a.SplitIndex).FirstOrDefault();
                        }
                        FTBTargetDTO weekDayTarget = targetdetailsDTO.Where(target => target.Day.ToString().Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (weekDayTarget != null && weekDayTarget.Target.HasValue)
                        {
                            reservationCount = reservationsPerDay.Where(date => date.DayInMonth.Date == shopTSDDate.Date).Select(res => res.ReservationCount).FirstOrDefault();
                            int percentageOfTarget = 0;
                            if (reservationCount != 0)
                            {
                                percentageOfTarget = (int)((100 * reservationCount) / weekDayTarget.Target.Value);
                            }
                            targetSlabDTO = (from slotDetail in weekDayTarget.FTBTargetsDetailDTOs
                                             where slotDetail.FTBTargetId == weekDayTarget.FTBTargetId && slotDetail.PercentTarget.HasValue &&
                                             Convert.ToInt32(slotDetail.PercentTarget.Value) <= percentageOfTarget
                                             orderby slotDetail.SlotOrder descending
                                             select slotDetail).FirstOrDefault();

                            ftbTSDLog = (from tsdLog in ftbRatesTSDUpdateslog where tsdLog.ScheduleJobId == ftbScheduledJob.ID && tsdLog.ShopDate.Date == shopTSDDate.Date orderby tsdLog.CreatedDateTime descending select tsdLog).FirstOrDefault();

                            if (ftbTSDLog != null)
                            {
                                //current target should not be less than prev target achieved  
                                // if above is false then assign prev target value reached and calculate new rates based on same and push to TSD
                                if (percentageOfTarget < ftbTSDLog.Target)
                                {
                                    targetSlabDTO = (from slotDetail in weekDayTarget.FTBTargetsDetailDTOs
                                                     where slotDetail.FTBTargetDetailId == ftbTSDLog.TargetDetailsID
                                                     select slotDetail).FirstOrDefault();
                                }

                            }

                            //if any of target slab is achieved then 
                            if (targetSlabDTO != null)
                            {
                                logFTBRateUpdate(ftbScheduledJob.ID, shopTSDDate, reservationCount, targetSlabDTO.PercentTarget.Value, targetSlabDTO.FTBTargetDetailId);
                                await updateRatesToTSD(rentalLengths, carClasses, dayOfWeek, ftbRatesDetails, splitIndex, targetSlabDTO, locationBrandDetails, isTetherActive,
                                    shopTSDDate, formula, globalTetherSettings, ftbScheduledJob, userName, true, masterRentalLengths, locationRentalLengths);
                            }
                            else
                            {
                                await updateRatesToTSD(rentalLengths, carClasses, dayOfWeek, ftbRatesDetails, splitIndex, targetSlabDTO, locationBrandDetails, isTetherActive,
                                    shopTSDDate, formula, globalTetherSettings, ftbScheduledJob, userName, false, masterRentalLengths, locationRentalLengths);
                            }
                        }
                        ftbJobShopDate.ShopDate = shopTSDDate.ToString("MM/dd/yyyy");
                        ftbJobShopDate.TargetAchieved = targetSlabDTO != null ? targetSlabDTO.PercentTarget.Value : 0;
                        ftbJobShopDate.RateIncreaseAchieved = targetSlabDTO != null ? targetSlabDTO.PercentRateIncrease.Value : 0;
                        ftbJobShopDate.ReservationCount = reservationCount;
                        ftbJobShopDate.ShopDay = shopTSDDate.Date;
                        ftbJobShopDateDetailsList.Add(ftbJobShopDate);
                    }

                }
                #endregion

                #region send email with shop date details

                if (ftbJobShopDateDetailsList.Any(shop => shop.TargetAchieved > 0))
                {
                    emailCommonSettings.LocationBrand = locationBrand.BrandLocation;
                    emailCommonSettings.MonthYear = ftbScheduledJob.StartDate.ToString("MMM-yyyy");
                    emailCommonSettings.BlackoutPeriod = blackOut;
                    User pricingMgrUser = null;
                    if (locationBrand.PricingManager.HasValue)
                    {
                        pricingMgrUser = (from users in rateShopperUsers where users.ID == locationBrand.PricingManager.Value select users).FirstOrDefault();
                        if (pricingMgrUser != null)
                        {
                            emailCommonSettings.UserName = pricingMgrUser.FirstName;
                            emailCommonSettings.assignedToEmail = !string.IsNullOrEmpty(pricingMgrUser.EmailAddress) ? pricingMgrUser.EmailAddress : string.Empty;
                        }
                    }

                    await sendEmailNotification(ftbJobShopDateDetailsList, emailCommonSettings);
                }

                #endregion
                ftbScheduledJob.ExecutionInProgress = false;
                this.Update(ftbScheduledJob);
                //});
            }
            return string.Empty;
            #endregion
        }

        private void UpdateLastRunNextRunAndExecutionInProgress(FTBScheduledJob job, DateTime DateWithTimeNoSeconds)
        {
            job.LastRunDateTime = DateWithTimeNoSeconds;
            SetNextRunDateTime(job);
            if (job.NextRunDateTime.HasValue)
            {
                job.ExecutionInProgress = true;
            }
            else
            {
                job.ExecutionInProgress = false;
                job.IsEnabled = false;
                Task.Run(() => CheckAndCopyTwelthMonthData(job.LocationBrandID)).Wait();
            }

        }

        private async Task<List<FTBTargetDTO>> fetchTargetDetails(long locationBrandId, long year, long month)
        {
            //return _ftbTargetService.fetchTargetDetails(locationBrandId, year, month);
            return await _ftbTargetService.GetTargetDetails(locationBrandId, year, month, false);
        }

        private async Task<IEnumerable<LocationBrandDetailsDTO>> getAllLocationBrandDetails()
        {
            const string getLocationBrand = "EXEC GetLocationBrandPricingMgr";
            IEnumerable<LocationBrandDetailsDTO> locationBrandDetails = await _context.ExecuteSQLQuery<LocationBrandDetailsDTO>(getLocationBrand);
            return locationBrandDetails;
        }

        private decimal CalculateSuggestedBaseRate(long locationBrandID, Formula objFormula, decimal totalRate, string rentalLengthCode, string rangeInterval, List<RentalLength> masterRentalLengths, List<LocationBrandRentalLength> locationRentalLengths, bool isGOV = false)
        {
            decimal baseRate = 0;
            //objFormula = _formulaService.GetFormulaByLocation(locationBrandID);
            if (objFormula != null && objFormula.ID > 0 && !string.IsNullOrEmpty(objFormula.TotalCostToBase))
            {
                rentalLengthCode = GetAssociatedLORGroupMaxRentalLengthString(rentalLengthCode, masterRentalLengths, locationRentalLengths);
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

        private decimal CalculateSuggestedTotalRate(long locationBrandID, Formula objFormula, decimal baseRate, string rentalLengthCode, string rangeInterval,  List<RentalLength> masterRentalLengths, List<LocationBrandRentalLength> locationRentalLengths, bool isGOV = false)
        {
            decimal totalRate = 0;
            //Formula objFormula = _formulaService.GetFormulaByLocation(locationBrandID);
            if (objFormula != null && objFormula.ID > 0 && !string.IsNullOrEmpty(objFormula.BaseToTotalCost))
            {
                rentalLengthCode = GetAssociatedLORGroupMaxRentalLengthString(rentalLengthCode, masterRentalLengths, locationRentalLengths);
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

        private string GetAssociatedLORGroupMaxRentalLengthString(string rentalLength, List<RentalLength> masterRentalLengths, List<LocationBrandRentalLength> locationRentalLengths)
        {
            RentalLength entityRentalLength = masterRentalLengths.Where(d => d.Code.ToUpper() == rentalLength).FirstOrDefault();
            //if (entityRentalLength.AssociateMappedId.HasValue && locationRentalLengths.Any(d => d.RentalLengthID == entityRentalLength.AssociateMappedId.Value))
            //{
            //    long maxMappedId = masterRentalLengths.Where(d => d.AssociateMappedId == entityRentalLength.AssociateMappedId.Value).Max(d => d.MappedID);
            //    rentalLength = masterRentalLengths.Where(d => d.MappedID == maxMappedId).FirstOrDefault().Code;
            //}
            //else 
            if (masterRentalLengths.Any(d => d.AssociateMappedId == entityRentalLength.MappedID))
            {
                long maxMappedId = masterRentalLengths.Where(d => d.AssociateMappedId == entityRentalLength.MappedID).Max(d => d.MappedID);
                rentalLength = masterRentalLengths.Where(d => d.MappedID == maxMappedId).FirstOrDefault().Code;
            }
            return rentalLength;
        }

        public void logFTBRateUpdate(long scheduledJobID, DateTime shopDate, long reservationCount, decimal target, long targetDetailsId)
        {
            try
            {
                FTBRatesTSDLogs ftbRatesTSDLog = new FTBRatesTSDLogs();
                ftbRatesTSDLog.ScheduleJobId = scheduledJobID;
                ftbRatesTSDLog.ShopDate = shopDate;
                ftbRatesTSDLog.ReservationCount = reservationCount;
                ftbRatesTSDLog.Target = target;
                ftbRatesTSDLog.TargetDetailsID = targetDetailsId;
                ftbRatesTSDLog.CreatedDateTime = DateTime.Now;
                _context.FTBRatesTSDLogs.Add(ftbRatesTSDLog);
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in logFTBRateUpdate: " + ex.Message + "Inner Exception" + ex.InnerException
                      + ", Stack Trace: " + ex.StackTrace, LogHelper.GetLogFilePath());
                LogHelper.WriteToLogFile("Log writing in FTBRatesUpdate Table Failed", LogHelper.GetLogFilePath());
            }
        }

        public decimal getBaseConfiguredRate(string dayOfWeek, List<FTBRateDetailsDTO> ftbRatesDetails, long rentalLengthId, long carClassId, int splitIndex)
        {
            decimal dailyRate = 0;

            try
            {
                switch (dayOfWeek.ToLower().ToString())
                {
                    case "sunday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Sunday.Value).FirstOrDefault();
                        break;
                    case "monday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Monday.Value).FirstOrDefault();
                        break;
                    case "tuesday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Tuesday.Value).FirstOrDefault();
                        break;
                    case "wednesday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Wednesday.Value).FirstOrDefault();
                        break;
                    case "thursday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Thursday.Value).FirstOrDefault();
                        break;
                    case "friday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Friday.Value).FirstOrDefault();
                        break;
                    case "saturday":
                        dailyRate = ftbRatesDetails.Where(rate => rate.RentalLengthId == rentalLengthId && rate.CarClassId == carClassId && rate.SplitIndex == splitIndex).Select(a => a.Saturday.Value).FirstOrDefault();
                        break;
                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteToLogFile("Error Occured in getBaseConfiguredRate: " + ex.Message + "Inner Exception" + ex.InnerException
                      + ", Stack Trace: " + ex.StackTrace, LogHelper.GetLogFilePath());
                LogHelper.WriteToLogFile("Rate Fetch from table ftbRatesDetails Failed", LogHelper.GetLogFilePath());
            }

            return dailyRate;
        }

        public async Task<string> updateRatesToTSD(Dictionary<long, string> rentalLengths, Dictionary<long, string> carClasses, string dayOfWeek, List<FTBRateDetailsDTO> ftbRatesDetails,
            int splitIndex, FTBTargetsDetailDTO targetSlabDTO, IEnumerable<LocationBrandDetailsDTO> locationBrandDetails, bool isTetherActive, DateTime shopTSDDate, List<Formula> formula, List<GlobalTetherSetting> globalTetherSettings,
            FTBScheduledJob ftbScheduledJob, string userName, bool targetMet, List<RentalLength> masterRentalLengths, List<LocationBrandRentalLength> locationRentalLengths)
        {
            string response = string.Empty;
            try
            {
                LocationBrandDetailsDTO dominentBrand = locationBrandDetails.Where(location => location.ID == ftbScheduledJob.LocationBrandID).FirstOrDefault();
                LocationBrandDetailsDTO dependentBrand = null;
                List<TSDModel> tsdModelList = new List<TSDModel>();
                List<TSDModel> tetherBrandtList = new List<TSDModel>();
                GlobalTetherSetting globalTether = null;
                Formula dependentBrandFormula = null;
                Formula dominentBrandFormula = (from obj in formula where obj.LocationBrandID == dominentBrand.ID select obj).FirstOrDefault();


                if (rentalLengths != null && carClasses != null)
                {
                    foreach (var rentalLength in rentalLengths)
                    {
                        string rangeInterval = rentalLength.Value.Substring(0, 1);

                        foreach (var car in carClasses)
                        {
                            decimal dailyRate = 0;

                            #region calculate Dominent Brand Rate

                            dailyRate = getBaseConfiguredRate(dayOfWeek, ftbRatesDetails, rentalLength.Key, car.Key, splitIndex);

                            decimal incrementFactor = 0;
                            if (targetMet)
                            {
                                incrementFactor = (targetSlabDTO.PercentRateIncrease.Value * dailyRate) / 100;
                                dailyRate = dailyRate + incrementFactor;
                            }

                            TSDModel tsdModel = new TSDModel();
                            tsdModel.Branch = dominentBrand.Code; ;
                            tsdModel.CarClass = car.Value;
                            tsdModel.RentalLength = rentalLength.Value;
                            tsdModel.RentalLengthIDs = Convert.ToString(rentalLength.Key);
                            tsdModel.StartDate = shopTSDDate.ToString("yyyyMMd");
                            tsdModel.DailyRate = dailyRate;
                            tsdModel.ExtraDayRateFactor = rentalLength.Value.IndexOf("D") > -1 ? dominentBrand.DailyExtraDayFactor.Value : dominentBrand.WeeklyExtraDenominator.Value;

                            if (tsdModel.DailyRate > 0)
                            {
                                tsdModelList.Add(tsdModel);
                            }

                            #endregion

                            #region calculate tether brand rates

                            if (isTetherActive && dailyRate > 0)
                            {
                                globalTether = (from tether in globalTetherSettings
                                                where tether.DominentBrandID == dominentBrand.BrandId && tether.LocationID == dominentBrand.LocationID && tether.CarClassID == car.Key
                                                select tether).FirstOrDefault();
                                if (globalTether != null)
                                {
                                    decimal dominentBrandTotalRate = 0;

                                    if (dominentBrandFormula != null)
                                    {
                                        dominentBrandTotalRate = CalculateSuggestedTotalRate(dominentBrand.ID, dominentBrandFormula, dailyRate, rentalLength.Value, rangeInterval, masterRentalLengths, locationRentalLengths);
                                    }
                                    dependentBrand = (from location in locationBrandDetails where location.BrandId == globalTether.DependantBrandID && location.LocationID == globalTether.LocationID select location).FirstOrDefault();

                                    if (dominentBrandTotalRate > 0)
                                    {
                                        dependentBrandFormula = (from obj in formula where obj.LocationBrandID == dependentBrand.ID select obj).FirstOrDefault();

                                        decimal differenceAmount = 0;
                                        decimal newTotalRate = 0;
                                        decimal newBaseRate = 0;

                                        if (globalTether.IsTeatherValueinPercentage)
                                        {
                                            differenceAmount = dominentBrandTotalRate * globalTether.TetherValue / 100;
                                        }
                                        else
                                        {
                                            differenceAmount = globalTether.TetherValue;
                                        }

                                        newTotalRate = dominentBrandTotalRate + differenceAmount;

                                        newBaseRate = CalculateSuggestedBaseRate(dependentBrand.ID, dependentBrandFormula, newTotalRate, rentalLength.Value, rangeInterval, masterRentalLengths, locationRentalLengths);
                                        TSDModel tetherModel = new TSDModel();
                                        tetherModel.Branch = dependentBrand.Code; ;
                                        tetherModel.CarClass = car.Value;
                                        tetherModel.RentalLength = rentalLength.Value;
                                        tetherModel.RentalLengthIDs = Convert.ToString(rentalLength.Key);
                                        tetherModel.StartDate = shopTSDDate.ToString("yyyyMMd");
                                        tetherModel.DailyRate = newBaseRate;
                                        tetherModel.ExtraDayRateFactor = rentalLength.Value.IndexOf("D") > -1 ? dependentBrand.DailyExtraDayFactor.Value : dependentBrand.WeeklyExtraDenominator.Value;
                                        if (tetherModel.DailyRate > 0)
                                        {
                                            tetherBrandtList.Add(tetherModel);
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    if (tsdModelList.Count > 0)
                    {
                        response = await _tsdTransactionService.ProcessRateSelection(tsdModelList, userName, "Weblink", dominentBrand.ID, ftbScheduledJob.UpdatedBy, 0, false, dominentBrand.BrandLocation.Split('-')[1], false, true);

                    }
                    if (tetherBrandtList.Count > 0)
                    {
                        await _tsdTransactionService.ProcessRateSelection(tetherBrandtList, userName, "WSPAN", dependentBrand.ID, ftbScheduledJob.UpdatedBy, 0, true, dependentBrand.BrandLocation.Split('-')[1], false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in updateRatesToTSD: " + ex.Message + "Inner Exception" + ex.InnerException
                    + ", Stack Trace: " + ex.StackTrace, LogHelper.GetLogFilePath());
                LogHelper.WriteToLogFile("Rate Update Failed", LogHelper.GetLogFilePath());
            }
            return response;
        }

        public async Task<IEnumerable<FTBAutomationJobsDTO>> GetFTBAutomationJobList(long loggedUserId, long locationBrandId, bool isAdminUser)
        {
            IEnumerable<FTBAutomationJobsDTO> FTBAutomationJobsDTOs = null;

            const string getFTBScheduledJobList = "EXEC GetFTBScheduledJobList @locationBrandId = @locationBrandId,@loggedUserId=@loggedUserId,@isAdminUser=@isAdminUser";

            var queryParameters = new[] { new SqlParameter("@locationBrandId", locationBrandId),
            new SqlParameter("@loggedUserId",loggedUserId),new SqlParameter("@isAdminUser",isAdminUser)};

            var getAllFTBAutomationJob = await _context.ExecuteSQLQuery<FTBAutomationJobsDTO>(getFTBScheduledJobList, queryParameters);
            FTBAutomationJobsDTOs = getAllFTBAutomationJob.OrderBy(a => a.ID).Select(a => new FTBAutomationJobsDTO
            {
                ID = a.ID,
                //  CarClasses = string.Join(", ", a.CarClassesIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Where(b => carClasses.ContainsKey(Convert.ToInt64(b))).Select(b => carClasses[Convert.ToInt64(b)])),
                CarClasses = GetSplitItems(a.CarClasses, false),
                CarClassIds = GetSplitItems(a.CarClasses, true),
                RentalLengths = GetSplitItems(a.RentalLengths, false), //string.Join(", ", a.RentalLengthIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => rentalLengths[Convert.ToInt64(b)])),
                RentalLengthIds = GetSplitItems(a.RentalLengths, true),
                CreatedDateTime = a.CreatedDateTime,
                CreatedDateAsString = a.CreatedDateTime.ToString("MM/dd/yyyy"),
                RunDates = a.DaysToRunStartDate.ToString("MM/dd/yyyy") + "..." + a.DaysToRunEndDate.ToString("MM/dd/yyyy"),
                IsEnabled = a.IsEnabled,
                IsDeleted = a.IsDeleted,
                Month_Year = a.StartDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + a.StartDate.Year,
                ExecutionInProgress = a.ExecutionInProgress,
                CreatedBy = a.CreatedBy,
                CreatedByID = a.CreatedByID,
                IsActiveTethering = a.IsActiveTethering,
                IsPresentNextRunDateTime = (a.NextRunDateTime != null) ? true : false,
                NextRunDateTime = (a.NextRunDateTime != null) ? a.NextRunDateTime : null,
                NextRunDateAsString = (a.NextRunDateTime != null) ? a.NextRunDateTime.Value.ToString("MM/dd/yyyy HH:mm") : string.Empty,
                LocationBrandID = a.LocationBrandID,
                LocationBrandAlias = a.LocationBrandAlias,
                Status = a.ExecutionInProgress ? "IN PROGRESS" : (a.NextRunDateTime != null ? (a.IsEnabled ? "SCHEDULED" : "STOPPED") : "FINISHED"),
                IsSplitMonth = a.IsSplitMonth,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                ShopStartDate = a.StartDate.ToString("MM/dd/yyyy"),
                ShopEndDate = a.EndDate.ToString("MM/dd/yyyy"),
                SplitAndSearchDetails = a.SplitAndSearchDetails
            }).ToList();


            return FTBAutomationJobsDTOs;
        }

        public async Task<string> GetSearchDetails(long ftbJobId)
        {
            string query = "SELECT dbo.GetSearchDetails(" + ftbJobId.ToString() + ")";
            var splitAndSearchDetails = await _context.ExecuteSQLQuery<string>(query);
            return splitAndSearchDetails.FirstOrDefault();
        }

        //Get ids and their code of rental length and carclass format 'ECAR-12,CCAR-11'
        public string GetSplitItems(string item, bool isId)
        {
            string returnItem = "";
            if (!string.IsNullOrEmpty(item))
            {
                var SplitItem = item.Split(',');

                if (SplitItem.Length > 0)
                {
                    if (isId)
                    {
                        returnItem = string.Join(",", (item.Split(',').Select(x => x.Split('-')[1]).ToArray()));
                    }
                    else
                    {
                        returnItem = string.Join(",", (item.Split(',').Select(x => x.Split('-')[0]).ToArray()));
                    }
                }
            }
            return returnItem;
        }

        public async Task<string> sendEmailNotification(List<FTBJobShopDateDetails> ftbJobShopDateDetails, FTBEmailCommonSettings emailCommon)
        {
            try
            {
                if (ftbJobShopDateDetails != null)
                {
                    ftbJobShopDateDetails = ftbJobShopDateDetails.OrderBy(shop => shop.ShopDay.Date).ToList();
                    EmailRequest shopDateRequest = EmailTemplateHelper.CreateEmailForShopDateDetail(ftbJobShopDateDetails, emailCommon);

                    if (shopDateRequest != null)
                    {
                        await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(shopDateRequest));
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("Error Occured in sendEmailNotification: " + ex.Message + "Inner Exception" + ex.InnerException
                      + ", Stack Trace: " + ex.StackTrace, LogHelper.GetLogFilePath());
            }
            return string.Empty;
        }

        async Task<string> CheckAndCopyTwelthMonthData(long locationBrandId)
        {
            DateTime twelthMonth = new DateTime(DateTime.Now.AddMonths(12).Year, DateTime.Now.AddMonths(12).Month, 1);
            DateTime nextMonth = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            try
            {
                DateTime dateTime = DateTime.Now.AddMonths(-1);
                List<FTBScheduledJob> lstFTBScheduledJobs = _context.FTBScheduledJobs.Where(d => d.LocationBrandID == locationBrandId && !d.IsDeleted && d.StartDate > DbFunctions.TruncateTime(dateTime) && d.StartDate <= DbFunctions.TruncateTime(twelthMonth)).OrderBy(d => d.StartDate).ToList();

                //Check whether next 11th month jobs are scheduled and twelth month job is not exists
                if (lstFTBScheduledJobs.Where(d => d.StartDate == twelthMonth).Count() == 0 && lstFTBScheduledJobs.Where(d => d.StartDate != twelthMonth && d.StartDate != currentMonth).Count() == 11)
                {
                    //Fetch the current month for the settings of twelth month job.
                    FTBScheduledJob settingForTwelthMonthJob = lstFTBScheduledJobs.Where(d => d.StartDate == currentMonth).FirstOrDefault();
                    if (settingForTwelthMonthJob != null)
                    {
                        FTBCopyMonthsDTO copyMonthDTO = new FTBCopyMonthsDTO();
                        copyMonthDTO.LocationBrandId = locationBrandId;
                        copyMonthDTO.SourceMonth = DateTime.Now.Month;
                        copyMonthDTO.SourceYear = DateTime.Now.Year;
                        copyMonthDTO.Month = twelthMonth.Month;
                        copyMonthDTO.Year = twelthMonth.Year;
                        var copyRates = _ftbRatesService.CopyFTBRates(copyMonthDTO, true);
                        var copyTargets = _ftbTargetService.CopyFTBTarget(copyMonthDTO);

                        await Task.WhenAll(copyRates, copyTargets);

                        if (copyRates.Result > 0 && copyTargets.Result > 0)
                        {
                            FTBJobEditDTO twelthMonthJob = new FTBJobEditDTO();
                            twelthMonthJob.LoggedInUserId = settingForTwelthMonthJob.CreatedBy;
                            twelthMonthJob.LocationBrandID = locationBrandId;
                            twelthMonthJob.JobScheduleWeekDays = settingForTwelthMonthJob.JobScheduleWeekDays;
                            twelthMonthJob.RunTime = settingForTwelthMonthJob.RunTime;
                            twelthMonthJob.ScheduledJobFrequencyID = settingForTwelthMonthJob.ScheduledJobFrequencyID;
                            twelthMonthJob.StartMonth = twelthMonth.Month;
                            twelthMonthJob.StartYear = twelthMonth.Year;
                            twelthMonthJob.StartDate = DateTime.Now;
                            twelthMonthJob.ExecutionInProgress = false;
                            twelthMonthJob.IsActiveTethering = settingForTwelthMonthJob.IsActiveTethering;

                            string result = SaveScheduledJob(twelthMonthJob);
                            if (result.ToUpper() == "SUCCESS")
                            {
                                LogHelper.WriteToLogFile("FTB Job created for the date: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                            }
                            else
                            {
                                LogHelper.WriteToLogFile("FTB Job is NOT created for the date: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString() + " message: " + result, LogHelper.GetLogFilePath());
                                FTBEmailCommonSettings oFTBEmailCommonSettings = GetEmailCommonSettings(locationBrandId, twelthMonth);
                                oFTBEmailCommonSettings.Subject = "Schedule FTB job for " + oFTBEmailCommonSettings.MonthYear;
                                //send mail to schedule twelth month job manually
                                string message = string.Concat(ConfigurationManager.AppSettings["ScheduleFTBMsg"].ToString(), nextMonth.ToString("dd-MMM-yyyy"));
                                await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(EmailTemplateHelper.CreateGeneralEmail(message, oFTBEmailCommonSettings)));
                            }
                            return result;
                        }
                        else
                        {
                            if (copyRates.Result == 0 && copyTargets.Result == 0)
                            {
                                LogHelper.WriteToLogFile("*******Copy operation failed for Rates and Targets: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                            }
                            else if (copyRates.Result == 0)
                            {
                                LogHelper.WriteToLogFile("*******Copy operation failed for Rates: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                            }
                            else if (copyTargets.Result == 0)
                            {
                                LogHelper.WriteToLogFile("*******Copy operation failed for Targets: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                            }
                            FTBEmailCommonSettings oFTBEmailCommonSettings = GetEmailCommonSettings(locationBrandId, twelthMonth);
                            oFTBEmailCommonSettings.Subject = "Configure FTB rate and target settings for " + oFTBEmailCommonSettings.MonthYear;
                            string message = string.Concat(ConfigurationManager.AppSettings["RatesNotConfigureMsg"].ToString(), nextMonth.ToString("dd-MMM-yyyy"), ".");
                            await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(EmailTemplateHelper.CreateGeneralEmail(message, oFTBEmailCommonSettings)));

                            return "Copy Operation Failed";
                        }
                    }
                    else
                    {
                        LogHelper.WriteToLogFile("---------Schedule settings not found for the twelth month: " + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                        FTBEmailCommonSettings oFTBEmailCommonSettings = GetEmailCommonSettings(locationBrandId, twelthMonth);
                        oFTBEmailCommonSettings.Subject = "Schedule FTB job for " + oFTBEmailCommonSettings.MonthYear;
                        string message = string.Concat(ConfigurationManager.AppSettings["ScheduleFTBMsg"].ToString(), nextMonth.ToString("dd-MMM-yyyy"), ".");
                        await EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(EmailTemplateHelper.CreateGeneralEmail(message, oFTBEmailCommonSettings)));
                    }
                }
                return "No Jobs Found";
            }
            catch (Exception ex)
            {
                LogHelper.WriteToLogFile("------------------------Error---------------", LogHelper.GetLogFilePath());
                LogHelper.WriteToLogFile("Error occured while creating Twelth month job:" + twelthMonth.ToShortDateString() + " for locationbrandid: " + locationBrandId.ToString(), LogHelper.GetLogFilePath());
                LogHelper.WriteToLogFile("Exception:" + ex.GetBaseException().ToString(), LogHelper.GetLogFilePath());
                FTBEmailCommonSettings oFTBEmailCommonSettings = GetEmailCommonSettings(locationBrandId, twelthMonth);
                oFTBEmailCommonSettings.Subject = "Schedule FTB job for " + oFTBEmailCommonSettings.MonthYear;
                string message = string.Concat(ConfigurationManager.AppSettings["ScheduleFTBMsg"].ToString(), nextMonth.ToString("dd-MMM-yyyy"), ".");
                EmailHelper.SendEmailAsync(EmailHelper.CreateEmailMessage(EmailTemplateHelper.CreateGeneralEmail(message, oFTBEmailCommonSettings)));
                return "Failed";
            }

        }

        FTBEmailCommonSettings GetEmailCommonSettings(long locationBrandId, DateTime date)
        {
            FTBEmailCommonSettings oFTBEmailCommonSettings = new FTBEmailCommonSettings();
            var settings = (from LB in _context.LocationBrands.Where(d => d.ID == locationBrandId)
                            join PMS in _context.LocationPricingManagers on LB.ID equals PMS.LocationBrandID into BPM
                            from PM in BPM.DefaultIfEmpty()
                            join USRS in _context.Users on PM.UserID equals USRS.ID into USR
                            from USER in USR.DefaultIfEmpty()
                            select new
                            {
                                LB.LocationBrandAlias,
                                USER.FirstName,
                                USER.EmailAddress

                            }).FirstOrDefault();
            if (!string.IsNullOrEmpty(settings.EmailAddress))
            {
                oFTBEmailCommonSettings.assignedToEmail = settings.EmailAddress;
                oFTBEmailCommonSettings.UserName = settings.FirstName;
            }
            oFTBEmailCommonSettings.LocationBrand = settings.LocationBrandAlias;
            oFTBEmailCommonSettings.MonthYear = date.ToString("MMM-yyyy");
            return oFTBEmailCommonSettings;
        }

        public FTBAutomationScenarioDTO CommonFTBJobUpdateScenarios(FTBAutomationScenarioDTO ftbAutomationScenarioDTO)
        {

            string locationbrandids = Convert.ToString(ftbAutomationScenarioDTO.LocationBrandID);
            long locationID = (from location in _context.LocationBrands
                               where location.ID == ftbAutomationScenarioDTO.LocationBrandID
                               select location.LocationID).SingleOrDefault();

            List<string> LocationDependantDominent = (from loc in _context.LocationBrands
                                                      where loc.LocationID == locationID
                                                      select loc.ID.ToString()).ToList<string>();

            //Get FTBrate setting of given month and year
            FTBRate ftbRate = (from ftbRates in _context.FTBRates
                               where LocationDependantDominent.Contains(ftbRates.LocationBrandId.ToString()) && ftbRates.Date.Month == ftbAutomationScenarioDTO.StartDate.Month && ftbRates.Date.Year == ftbAutomationScenarioDTO.StartDate.Year
                               select ftbRates).SingleOrDefault();

            if (ftbRate == null)
            {
                ftbAutomationScenarioDTO.IsReturnMsg = false;
                return ftbAutomationScenarioDTO;
            }


            IEnumerable<ScheduledJob> scheduledJobs = null;
            scheduledJobs = (from sjobs in _context.ScheduledJobs
                             where !(sjobs.IsDeleted) && LocationDependantDominent.Contains(sjobs.LocationBrandIDs) && sjobs.StartDate.Value.Year == ftbAutomationScenarioDTO.StartDate.Year && sjobs.StartDate.Value.Month == ftbAutomationScenarioDTO.StartDate.Month && sjobs.IsStandardShop
                             select sjobs).ToList();
            //Scenario
            //Check if jobs date range matches with blackoutdate range and check.If fall than check more than one than no change dates or got one or more than check which job dates fall in blackout dates
            IEnumerable<ScheduledJob> blackoutMatchScheduledJobs = null;
            if (ftbRate.HasBlackOutPeroid.HasValue && ftbRate.HasBlackOutPeroid.Value)///Get all matches scheduled jobs
            {
                blackoutMatchScheduledJobs = (from sjob in scheduledJobs
                                              where sjob.StartDate.Value.Date >= ftbRate.BlackOutStartDate.Value.Date && sjob.EndDate.Value.Date <= ftbRate.BlackOutEndDate.Value.Date
                                              select sjob).ToList();
            }

            //Scenario
            //if (ftbAutomationScenarioDTO.IsBlackoutDatesChange)
            //{
            //    FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);
            //}
            //This condition is use when normal automation job shcduled, it checks weather this month has ftbjob is exists or not 
            FTBScheduledJob ftbScheduledJob = null;
            if (!ftbAutomationScenarioDTO.IsFTBJob)//This if condition fall when this function call form normal automation job operation and pass this flag as 'false'
            {
                ftbScheduledJob = (from ftbJob in _context.FTBScheduledJobs
                                   where !ftbJob.IsDeleted &&
                                   LocationDependantDominent.Contains(ftbJob.LocationBrandID.ToString()) &&
                                   ftbJob.StartDate.Year == ftbAutomationScenarioDTO.StartDate.Year &&
                                   ftbJob.StartDate.Month == ftbAutomationScenarioDTO.StartDate.Month
                                   select ftbJob).FirstOrDefault();

                //Scenario if FTB job is scheduled but it is stop. and user want tog go for initiate new normal automation job at that time user able to initiate a automation job.
                if (ftbScheduledJob != null && ftbScheduledJob.NextRunDateTime != null && !(ftbScheduledJob.IsEnabled))
                {
                    ftbAutomationScenarioDTO.IsReturnMsg = false;
                    return ftbAutomationScenarioDTO;
                }
                //If blackout period is not set and no ftb job scheduled for current month than this scenario will execute.
                if (ftbRate.HasBlackOutPeroid.HasValue && !ftbRate.HasBlackOutPeroid.Value)
                {
                    if (ftbScheduledJob == null)
                    {
                        ftbAutomationScenarioDTO.IsReturnMsg = false;
                        return ftbAutomationScenarioDTO;
                    }
                    else
                    {
                        ftbAutomationScenarioDTO.IsReturnMsg = true;
                        ftbAutomationScenarioDTO.ReturnMessage = "notconfiguredblackoutdates";
                        return ftbAutomationScenarioDTO;
                    }
                }

                //Check blackout date range
                if (ftbRate.HasBlackOutPeroid.HasValue && ftbRate.HasBlackOutPeroid.Value && ftbAutomationScenarioDTO.StartDate.Date >= ftbRate.BlackOutStartDate.Value.Date && ftbAutomationScenarioDTO.EndDate.Date <= ftbRate.BlackOutEndDate.Value.Date)
                {
                    ftbAutomationScenarioDTO.IsReturnMsg = false;
                    return ftbAutomationScenarioDTO;
                }

                if (ftbScheduledJob != null && ftbRate.HasBlackOutPeroid.HasValue && ftbRate.HasBlackOutPeroid.Value)
                {
                    ftbAutomationScenarioDTO.ReturnMessage = "FTBJobScheduled";
                    ftbAutomationScenarioDTO.BlackoutStartDate = ftbRate.BlackOutStartDate;
                    ftbAutomationScenarioDTO.BlackoutEndDate = ftbRate.BlackOutEndDate;
                    ftbAutomationScenarioDTO.IsReturnMsg = true;
                    return ftbAutomationScenarioDTO;
                }
            }
            else//Thie else part all operation from  FTB
            {
                //Scheduling FTB job 
                bool ReturnFlag = false;
                if (scheduledJobs.Any() && blackoutMatchScheduledJobs != null && blackoutMatchScheduledJobs.Any())
                {
                    //get scehduled job which not match blackout dates
                    //IEnumerable<ScheduledJob> scheduledDateMatchUpdatejob = scheduledJobs.Except(blackoutMatchScheduledJobs).ToList();

                    IEnumerable<ScheduledJob> scheduledDateMatchUpdatejob = scheduledJobs.Where(m => !blackoutMatchScheduledJobs.Select(x => x.ID).Contains(m.ID)).ToList();

                    if (scheduledDateMatchUpdatejob.Any())
                    {
                        ReturnFlag = FTPDisabledAutomationJobs(scheduledDateMatchUpdatejob, ftbAutomationScenarioDTO.LoggedUserID);
                        ftbAutomationScenarioDTO.ReturnMessage = "AutomationJobDisabled";
                        ftbAutomationScenarioDTO.IsReturnMsg = (ReturnFlag) ? true : false;
                        ftbAutomationScenarioDTO.BlackoutStartDate = ftbRate.BlackOutStartDate;
                        ftbAutomationScenarioDTO.BlackoutEndDate = ftbRate.BlackOutEndDate;
                    }
                }
                else if (scheduledJobs.Any() && ftbRate.HasBlackOutPeroid.HasValue && ftbRate.HasBlackOutPeroid.Value)
                {
                    //If isBlackout period assigned for given locationbrand, month and year.
                    ReturnFlag = FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);
                    ftbAutomationScenarioDTO.ReturnMessage = "AutomationJobDisabled";
                    ftbAutomationScenarioDTO.IsReturnMsg = (ReturnFlag) ? true : false;
                    ftbAutomationScenarioDTO.BlackoutStartDate = ftbRate.BlackOutStartDate;
                    ftbAutomationScenarioDTO.BlackoutEndDate = ftbRate.BlackOutEndDate;
                }
                else if (scheduledJobs.Any())
                {
                    ReturnFlag = FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);
                    if (ftbRate.HasBlackOutPeroid.HasValue && !ftbRate.HasBlackOutPeroid.Value)
                    {
                        ftbAutomationScenarioDTO.IsReturnMsg = (ReturnFlag) ? true : false;
                        ftbAutomationScenarioDTO.ReturnMessage = "notconfiguredblackoutdates";
                        return ftbAutomationScenarioDTO;
                    }
                    //this else used for no blackout period assigned than all given automation job should disable
                    ftbAutomationScenarioDTO.ReturnMessage = "AutomationJobDisabled";
                    ftbAutomationScenarioDTO.IsReturnMsg = (ReturnFlag) ? true : false;
                    ftbAutomationScenarioDTO.BlackoutStartDate = ftbRate.BlackOutStartDate;
                    ftbAutomationScenarioDTO.BlackoutEndDate = ftbRate.BlackOutEndDate;
                }
            }
            return ftbAutomationScenarioDTO;
        }
        //Group job disabled using in FTB scenario
        public bool FTPDisabledAutomationJobs(IEnumerable<ScheduledJob> scheduledJobs, long LoggedUserID)
        {
            bool returnFlag = false;
            foreach (var sitem in scheduledJobs)
            {
                if (sitem.NextRunDateTime != null)
                {
                    if (sitem.IsEnabled)
                    {
                        returnFlag = true;
                        ScheduledJob oScheduledJob = new ScheduledJob();
                        oScheduledJob = sitem;
                        oScheduledJob.IsEnabled = false;

                        oScheduledJob.IsStopByFTB = true;//done by automatic stop automation job scenario in highlight logic

                        //                oScheduledJob.CreatedBy = LoggedUserID;
                        oScheduledJob.UpdatedBy = LoggedUserID;
                        oScheduledJob.UpdatedDateTime = DateTime.Now;
                        _context.Entry(oScheduledJob).State = EntityState.Modified;
                    }
                }
            }
            _context.SaveChanges();
            return returnFlag;
        }

        public ProviderRequest CreateRequestToFetchReservation(string startDate, string endDate, string brandLocation)
        {
            ProviderRequest objRequest = new ProviderRequest();
            objRequest.URL = ConfigurationManager.AppSettings["TSDReservationEndPoint"];
            objRequest.HttpMethod = Enumerations.HttpMethod.Post;
            //objRequest.HeaderItems.Add("content-type", "application/json");
            TSDParams tsdParametrs = new TSDParams();
            tsdParametrs.StartDate = startDate;
            tsdParametrs.EndDate = endDate;
            tsdParametrs.BrandLocation = brandLocation;

            objRequest.ProviderRequestData = JsonConvert.SerializeObject(tsdParametrs);
            return objRequest;
        }
    }
}
