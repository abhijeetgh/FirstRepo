using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using RateShopper.Core.Cache;
using System.Globalization;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public class QuickViewService : BaseService<QuickView>, IQuickViewService
    {
        ICompanyService _companyService;
        IScrapperSourceService _scrapperservice;
        IStatusesService _statusService;
        ISearchSummaryService _searchSummaryService;
        public QuickViewService(IEZRACRateShopperContext context, ICacheManager cacheManager, ICompanyService companyService,
            IScrapperSourceService scrapperservice, IStatusesService statusService, ISearchSummaryService searchSummaryService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<QuickView>();
            _cacheManager = cacheManager;
            _companyService = companyService;
            _scrapperservice = scrapperservice;
            _statusService = statusService;
            _searchSummaryService = searchSummaryService;
        }

        public DateTime GetESTDate(string utcDate)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.ParseExact(utcDate, "MM-dd-yyyy HH:mm", CultureInfo.InvariantCulture), easternZone);
        }

        public QuickViewDTO SaveQuickView(long searchSummaryId, string scheduleTime, string competitorsIds, long userId, string clientUTCDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData, long quickViewId = 0)
        {
            DateTime scheduleDate = new DateTime();
            DateTime clientDateAsPerEST = GetESTDate(clientUTCDate);
            bool UpdateStatus = false;
            //if schedule time is 0 
            if (scheduleTime != "0")
            {
                scheduleDate = GetESTDate(scheduleTime);
            }
            QuickView objQuickViewEntity = null;
            if (quickViewId > 0)
            {
                objQuickViewEntity = GetById(quickViewId, false);
            }
            else
            {
                objQuickViewEntity = _context.QuickView.Where(d => !d.IsDeleted && (d.ParentSearchSummaryId == searchSummaryId || d.ChildSearchSummaryId == searchSummaryId)).FirstOrDefault();
            }

            if (objQuickViewEntity != null)
            {
                UpdateStatus = true;
                if (!string.IsNullOrEmpty(competitorsIds))
                {
                    objQuickViewEntity.CompetitorCompanyIds = competitorsIds;
                }
                if (!string.IsNullOrEmpty(carClassIds))
                {
                    objQuickViewEntity.CarClassIds = carClassIds;
                }
                if (scheduleTime == "0")
                {
                    objQuickViewEntity.LastRunDate = clientDateAsPerEST;
                    objQuickViewEntity.IsExecutionInProgress = true;
                    objQuickViewEntity.StatusId = _statusService.GetStatusIDByName("Request Sent To Scrapper");
                    objQuickViewEntity.NextRunDate = null;
                }
                else
                {
                    objQuickViewEntity.NextRunDate = scheduleDate;
                    objQuickViewEntity.StatusId = _statusService.GetStatusIDByName("Pending");
                }
                if (scheduleControlId != null)
                {
                    objQuickViewEntity.UIControlId = scheduleControlId;
                }
                objQuickViewEntity.UpdatedDateTime = clientDateAsPerEST.Date;
                objQuickViewEntity.UpdatedBy = userId;
                //Quickview enhancement changes
                objQuickViewEntity.MonitorBase = (quickViewGroupData.IsTotal) ? false : true;
                objQuickViewEntity.NotifyEmail = quickViewGroupData.IsEmailNotification;
                objQuickViewEntity.PickupTime = quickViewGroupData.PickupTime;
                objQuickViewEntity.DropoffTime = quickViewGroupData.DropOffTime;
                Update(objQuickViewEntity);
            }
            else if (searchSummaryId > 0)
            {
                UpdateStatus = false;
                objQuickViewEntity = new QuickView();
                objQuickViewEntity.CompetitorCompanyIds = competitorsIds;
                objQuickViewEntity.CarClassIds = carClassIds;
                if (scheduleTime == "0")
                {
                    objQuickViewEntity.LastRunDate = clientDateAsPerEST;
                    objQuickViewEntity.IsExecutionInProgress = true;
                    objQuickViewEntity.StatusId = _statusService.GetStatusIDByName("Request Sent To Scrapper");
                }
                else
                {
                    objQuickViewEntity.NextRunDate = scheduleDate;
                    objQuickViewEntity.IsExecutionInProgress = false;
                    objQuickViewEntity.StatusId = _statusService.GetStatusIDByName("Pending");
                }
                objQuickViewEntity.UIControlId = scheduleControlId;
                objQuickViewEntity.ParentSearchSummaryId = searchSummaryId;
                objQuickViewEntity.IsDeleted = false;
                objQuickViewEntity.IsEnabled = true;
                objQuickViewEntity.CreatedBy = userId;
                objQuickViewEntity.UpdatedBy = userId;
                objQuickViewEntity.CreatedDateTime = clientDateAsPerEST.Date;
                objQuickViewEntity.UpdatedDateTime = clientDateAsPerEST.Date;
                //Quickview enhancement changes
                objQuickViewEntity.MonitorBase = (quickViewGroupData.IsTotal) ? false : true;
                objQuickViewEntity.NotifyEmail = quickViewGroupData.IsEmailNotification;
                objQuickViewEntity.PickupTime = quickViewGroupData.PickupTime;
                objQuickViewEntity.DropoffTime = quickViewGroupData.DropOffTime;
                Add(objQuickViewEntity);
            }
            if (objQuickViewEntity != null)
            {
                QuickViewDTO objQuickViewDTO = new QuickViewDTO();
                objQuickViewDTO.ID = objQuickViewEntity.ID;
                objQuickViewDTO.ParentSearchSummaryId = objQuickViewEntity.ParentSearchSummaryId;
                if (objQuickViewEntity.ChildSearchSummaryId.HasValue)
                {
                    objQuickViewDTO.ChildSearchSummaryId = objQuickViewEntity.ChildSearchSummaryId.Value;
                }
                objQuickViewDTO.CompetitorCompanyIds = objQuickViewEntity.CompetitorCompanyIds;
                objQuickViewDTO.IsExecutionInProgress = objQuickViewEntity.IsExecutionInProgress;
                objQuickViewDTO.StatusId = objQuickViewEntity.StatusId;
                if (objQuickViewEntity.LastRunDate.HasValue)
                {
                    objQuickViewDTO.LastRunDate = objQuickViewEntity.LastRunDate.Value;
                }
                if (objQuickViewEntity.NextRunDate.HasValue)
                {
                    objQuickViewDTO.NextRunDate = objQuickViewEntity.NextRunDate.Value;
                }
                //Quickview Enhancement operation
                if (!UpdateStatus)
                {
                    //Insert opreation of Quickview group operation
                    int groupindex = 1;
                    foreach (var groupItem in quickViewGroupData.GroupData)
                    {

                        AddQuickViewGroup(groupItem, groupindex, objQuickViewEntity.ID);
                        groupindex++;
                    }
                }
                else
                {
                    //update operation of Quickview group operation
                    int groupindex = 1;
                    foreach (var groupItem in quickViewGroupData.GroupData)
                    {
                        //If user will be add new group
                        if (groupItem.IsNewGroup)
                        {
                            AddQuickViewGroup(groupItem, groupindex, objQuickViewEntity.ID);
                        }
                        else
                        {
                            if (groupItem.DeleteGroupId != null)
                            {
                                long groupID = long.Parse(groupItem.DeleteGroupId);
                                DeleteQuickviewGroup(groupID);
                            }
                            else
                            {
                                #region add operation
                                if (groupItem.AddCompetitorIds != null)
                                {
                                    foreach (var companyItem in groupItem.AddCompetitorIds.Split(','))
                                    {
                                        QuickViewGroupCompanies quickViewGroupCompanies = new QuickViewGroupCompanies();
                                        quickViewGroupCompanies.GroupId = groupItem.groupId;
                                        quickViewGroupCompanies.CompanyId = Convert.ToInt64(companyItem);
                                        _context.QuickViewGroupCompanies.Add(quickViewGroupCompanies);
                                    }
                                    _context.SaveChanges();
                                }
                                #endregion

                                #region delete operation
                                if (groupItem.DeleteCompetitorIds != null)
                                {
                                    foreach (var companyItem in groupItem.DeleteCompetitorIds.Split(','))
                                    {
                                        long CompanyID = long.Parse(companyItem);
                                        QuickViewGroupCompanies quickViewGroupCompanies = new QuickViewGroupCompanies();
                                        quickViewGroupCompanies = _context.QuickViewGroupCompanies.Where(obj => obj.GroupId == groupItem.groupId && obj.CompanyId == CompanyID).FirstOrDefault();
                                        if (quickViewGroupCompanies != null)
                                        {
                                            _context.Entry(quickViewGroupCompanies).State = System.Data.Entity.EntityState.Deleted;
                                        }
                                    }
                                    _context.SaveChanges();
                                }
                                #endregion
                                if (groupItem.QuickViewCarClassGroups.Count() > 0)
                                {
                                    foreach (var carClassGroupItem in groupItem.QuickViewCarClassGroups)
                                    {
                                        QuickViewGapDevSettings quickViewGapDevSettings = new QuickViewGapDevSettings();
                                        quickViewGapDevSettings = _context.QuickViewGapDevSettings.Where(obj => obj.ID == carClassGroupItem.Id).FirstOrDefault();
                                        //quickViewGapDevSettings.GapValue = carClassGroupItem.GapValueGroup;
                                        quickViewGapDevSettings.DeviationValue = carClassGroupItem.DeviationValueGroup;
                                        if (quickViewGapDevSettings != null)
                                        {
                                            _context.Entry(quickViewGapDevSettings).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                    _context.SaveChanges();
                                }
                            }
                        }
                        groupindex++;
                    }
                }
                _cacheManager.Remove(typeof(QuickViewGroup).ToString());
                _cacheManager.Remove(typeof(QuickViewGroupCompanies).ToString());
                _cacheManager.Remove(typeof(QuickViewGapDevSettings).ToString());

                return objQuickViewDTO;
            }
            return null;
        }
        public void DeleteQuickviewGroup(long quickViewgroupId)
        {
            IEnumerable<QuickViewGapDevSettings> quickViewGapDevSettings = _context.QuickViewGapDevSettings.Where(obj => obj.CompetitorGroupId == quickViewgroupId).ToList();
            if (quickViewGapDevSettings.Count() > 0)
            {
                foreach (var gapsettinitem in quickViewGapDevSettings)
                {
                    _context.Entry(gapsettinitem).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }

            IEnumerable<QuickViewGroupCompanies> quickViewGroupCompanies = _context.QuickViewGroupCompanies.Where(obj => obj.GroupId == quickViewgroupId).ToList();
            if (quickViewGroupCompanies.Count() > 0)
            {
                foreach (var groupcompany in quickViewGroupCompanies)
                {
                    _context.Entry(groupcompany).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }

            QuickViewGroup quickViewGroup = _context.QuickViewGroups.Where(obj => obj.ID == quickViewgroupId).FirstOrDefault();
            if (quickViewGroup != null)
            {
                _context.Entry(quickViewGroup).State = System.Data.Entity.EntityState.Deleted;
            }
            _context.SaveChanges();
        }
        public void AddQuickViewGroup(QuickViewGroupItem groupItem, int groupIndex, long QuickviewID)
        {
            QuickViewGroup quickViewGroup = new Domain.Entities.QuickViewGroup();
            quickViewGroup.GroupName = "Group " + groupIndex;
            quickViewGroup.QuickViewId = QuickviewID;
            _context.QuickViewGroups.Add(quickViewGroup);
            _context.SaveChanges();

            //Save Quickview group company
            if (!string.IsNullOrEmpty(groupItem.CompetitorIds))
            {
                foreach (var companyItem in groupItem.CompetitorIds.Split(','))
                {
                    QuickViewGroupCompanies quickViewGroupCompanies = new QuickViewGroupCompanies();
                    quickViewGroupCompanies.GroupId = quickViewGroup.ID;
                    quickViewGroupCompanies.CompanyId = Convert.ToInt64(companyItem);
                    _context.QuickViewGroupCompanies.Add(quickViewGroupCompanies);
                }
                _context.SaveChanges();
            }

            //Save Quickview group data
            int CarClassId = 1;
            foreach (var carClassGroupItem in groupItem.QuickViewCarClassGroups)
            {
                QuickViewGapDevSettings quickViewGapDevSettings = new QuickViewGapDevSettings();
                quickViewGapDevSettings.CarClassGroupId = CarClassId;
                quickViewGapDevSettings.CompetitorGroupId = quickViewGroup.ID;
                //quickViewGapDevSettings.GapValue = carClassGroupItem.GapValueGroup;
                quickViewGapDevSettings.DeviationValue = carClassGroupItem.DeviationValueGroup;
                _context.QuickViewGapDevSettings.Add(quickViewGapDevSettings);
                CarClassId++;
            }
            _context.SaveChanges();
        }

        public QuickViewGridDTO UpdateQuickView(long quickViewId, string scheduleTime, string competitorsIds, long userId, string clientUTCDate, string scheduleControlId, string carClassIds, QuickViewGroupData quickViewGroupData)
        {
            QuickViewDTO objQuickViewDTO = SaveQuickView(0, scheduleTime, competitorsIds, userId, clientUTCDate, scheduleControlId, carClassIds, quickViewGroupData, quickViewId);
            if (objQuickViewDTO != null)
            {
                QuickViewGridDTO objQuickViewGridDTO = new QuickViewGridDTO();
                objQuickViewGridDTO.ID = objQuickViewDTO.ID;
                objQuickViewGridDTO.SearchSummaryId = objQuickViewDTO.ParentSearchSummaryId;
                if (objQuickViewDTO.ChildSearchSummaryId != 0 && objQuickViewDTO.ChildSearchSummaryId > 0)
                {
                    objQuickViewGridDTO.ChildSummaryId = objQuickViewDTO.ChildSearchSummaryId;
                }
                objQuickViewGridDTO.Status = GetStatusName(objQuickViewDTO.StatusId);
                return objQuickViewGridDTO;
            }
            return null;
        }

        public List<QuickViewGridDTO> GetQuickViews(long userId, string brandLocationIds, string clientCurrentUTCDate)
        {
            DateTime clientDateAsPerEST = GetESTDate(clientCurrentUTCDate).Date;
            Dictionary<long, string> companies = _companyService.GetAll().ToDictionary(a => a.ID, a => a.Code);
            Dictionary<long, string> sources = _scrapperservice.GetAll().ToDictionary(a => a.ID, a => a.Name);
            List<long> lstBrandLocation = !string.IsNullOrEmpty(brandLocationIds) ? brandLocationIds.Split(',').Select(a => Convert.ToInt64(a)).ToList() : new List<long>();

            //List<long> lstBrandLocation = (from LB in _context.LocationBrands.Where(d => (!d.IsDeleted && (d.ID == brandLocationId || brandLocationId == 0)))
            //                               join ULB in _context.UserLocationBrands.Where(d => (userId == 0 || d.UserID == userId)) on LB.ID equals ULB.LocationBrandID
            //                               select LB.ID).Distinct().ToList();

            var quickviewgrid = (from SS in _context.SearchSummaries
                                 //commented to show previous day scheduled jobs
                                 //join QV in _context.QuickView.Where(d => (d.UpdatedDateTime == clientDateAsPerEST && (userId == 0 || d.UpdatedBy == userId))) on SS.ID equals QV.ParentSearchSummaryId
                                 join QV in _context.QuickView.Where(d => !d.IsDeleted && (userId == 0 || d.CreatedBy == userId)) on SS.ID equals QV.ParentSearchSummaryId
                                 join US in _context.Users on QV.CreatedBy equals US.ID
                                 select new
                                 {
                                     ID = QV.ID,
                                     SearchSummaryId = SS.ID,
                                     CarClassesIDs = SS.CarClassesIDs,
                                     RentalLengthIDs = SS.RentalLengthIDs,
                                     CreatedDateTime = SS.UpdatedDateTime,
                                     IsEnabled = QV.IsEnabled,
                                     IsDeleted = QV.IsDeleted,
                                     StartDate = SS.StartDate,
                                     EndDate = SS.EndDate,
                                     ExecutionInProgress = QV.IsExecutionInProgress,
                                     CreatedByFirstName = US.FirstName,
                                     CreatedByLastName = US.LastName,
                                     LocationBrandIDs = SS.LocationBrandIDs,
                                     NextRunDateTime = QV.NextRunDate,
                                     CreatedByID = SS.CreatedBy,
                                     LastRunDateTime = QV.LastRunDate,
                                     CompetitorsId = QV.CompetitorCompanyIds,
                                     Sources = SS.ScrapperSourceIDs,
                                     ChildSummaryId = QV.ChildSearchSummaryId,
                                     StatusId = QV.StatusId,
                                     TrackingCarClassIds = QV.CarClassIds
                                 }).ToList();


            List<QuickViewGridDTO> quickviewGridData = quickviewgrid.OrderByDescending(c => c.ID).Select(d => new QuickViewGridDTO
            {
                ID = d.ID,
                SearchSummaryId = d.SearchSummaryId,
                CarClassesIDs = string.Join(",", d.CarClassesIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => b)),
                RentalLengthIDs = string.Join(",", d.RentalLengthIDs.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => b)),
                CreatedDateTime = d.CreatedDateTime,
                StartDate = d.StartDate.ToString("MM/dd/yyyy"),
                EndDate = d.EndDate.ToString("MM/dd/yyyy"),
                ExecutionInProgress = d.ExecutionInProgress,
                CreatedByUserName = d.CreatedByFirstName + " " + d.CreatedByLastName,
                LastRunDateTime = d.LastRunDateTime.HasValue ? TimeZoneInfo.ConvertTimeToUtc(d.LastRunDateTime.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) : d.LastRunDateTime,
                Competitors = string.Join(", ", d.CompetitorsId.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => companies[Convert.ToInt64(b)])),
                Status = GetStatusName(d.StatusId),
                LocationBrands = !string.IsNullOrEmpty(d.LocationBrandIDs) ? d.LocationBrandIDs.Split(',').Select(c => Convert.ToInt64(c)).ToList() : null,
                LocationsBrandIDs = d.LocationBrandIDs,
                NextRunDateTime = d.NextRunDateTime.HasValue ? TimeZoneInfo.ConvertTimeToUtc(d.NextRunDateTime.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) : d.NextRunDateTime,
                Sources = string.Join(",", d.Sources.Split(',').OrderBy(b => Convert.ToInt64(b)).Select(b => sources[Convert.ToInt64(b)])),
                ChildSummaryId = d.ChildSummaryId.HasValue ? d.ChildSummaryId.Value : 0,
                TrackingCarClassIds = !string.IsNullOrEmpty(d.TrackingCarClassIds) ? d.TrackingCarClassIds : string.Empty,
                //refactored - get all quickview results in list
                //IsReportReviewed =  _context.QuickViewResults.Where(quick => quick.QuickViewId == d.ID).Count() > 0 ? (_context.QuickViewResults.Where(quick => quick.QuickViewId == d.ID && quick.IsMovedUp.HasValue).Any(quick => !quick.IsReviewed.HasValue) ? false : true) : false
                //IsReportReviewed = quickViewResults.Where(quick => quick.QuickViewId == d.ID).Count() > 0 ? (quickViewResults.Where(quick => quick.QuickViewId == d.ID && quick.IsMovedUp.HasValue).Any(quick => !quick.IsReviewed.HasValue) ? false : true) : false
                IsReportReviewed = _context.QuickViewResults.Where(quick => quick.QuickViewId == d.ID).Count() > 0 ?
                                                         (_context.QuickViewResults.Where(quick => quick.QuickViewId == d.ID && (d.ChildSummaryId.HasValue ? quick.SearchSummaryId == d.ChildSummaryId.Value : quick.SearchSummaryId == d.SearchSummaryId) && (quick.IsMovedUp.HasValue || (quick.IsPositionChange.HasValue && quick.IsPositionChange.Value)))
                                                         .Any(quick => !quick.IsReviewed.HasValue) ? false : true) : false
            }).Where(e => e.LocationBrands.Intersect(lstBrandLocation).Any()).ToList();
            return quickviewGridData;

        }

        public bool DeleteQuickView(long quickViewId, long userId, string clientUTCDate)
        {
            QuickView objQuickViewEntity = GetById(quickViewId, false);
            if (objQuickViewEntity != null)
            {
                objQuickViewEntity.IsDeleted = true;
                objQuickViewEntity.UpdatedBy = userId;
                objQuickViewEntity.UpdatedDateTime = GetESTDate(clientUTCDate);

                Update(objQuickViewEntity);

                long childSearchSummaryId = objQuickViewEntity.ChildSearchSummaryId.HasValue ? objQuickViewEntity.ChildSearchSummaryId.Value : 0;
                long parentSearchSummaryId = objQuickViewEntity.ParentSearchSummaryId;
                if (childSearchSummaryId > 0)
                {
                    SearchSummary objChildSearchSummary = _searchSummaryService.GetById(childSearchSummaryId, false);
                    if (objChildSearchSummary != null)
                    {
                        objChildSearchSummary.StatusID = 6;
                        objChildSearchSummary.UpdatedDateTime = DateTime.Now;
                        objChildSearchSummary.UpdatedBy = userId;
                        _searchSummaryService.Update(objChildSearchSummary);
                    }
                }
                if (parentSearchSummaryId > 0)
                {
                    SearchSummary objParentSearch = _searchSummaryService.GetById(parentSearchSummaryId, false);
                    if (objParentSearch != null)
                    {
                        objParentSearch.HasQuickViewChild = false;
                        objParentSearch.UpdatedDateTime = DateTime.Now;
                        _searchSummaryService.Update(objParentSearch);
                    }
                }

                return true;
            }
            return false;
        }

        #region Quick View Automated shops

        public async Task<string> RunQuickViewShop()
        {
            Helper.LogHelper.WriteToLogFile("Called runquick view shop", Helper.LogHelper.GetLogFilePath());
            string response = string.Empty;
            //calculate current UTC time with 1 minute delay
            DateTime thisUTCTime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour).AddMinutes(DateTime.UtcNow.Minute);
            DateTime thisTimeWithNextMinUTC = thisUTCTime.AddMinutes(1);
            DateTime thisTimeInEST = GetESTDate(thisTimeWithNextMinUTC.ToString("MM-dd-yyyy HH:mm"));

            List<QuickView> quickViewsToRun = _context.QuickView.Where(quick => !quick.IsDeleted && quick.IsEnabled && quick.NextRunDate.HasValue
                && quick.NextRunDate.Value <= thisTimeInEST).ToList();

            //filter search summaries those have quick view childs
            //List<SearchSummary> searchSummaryShops = _context.SearchSummaries.Where(search => search.HasQuickViewChild.HasValue && search.HasQuickViewChild.Value
            //    && search.ScheduledJobID.Value == null).ToList();

            if (quickViewsToRun != null && quickViewsToRun.Count > 0)
            {
                foreach (var quickView in quickViewsToRun)
                {
                    response = await _searchSummaryService.InitiateQuickViewSearch(quickView.ParentSearchSummaryId, quickView.UpdatedBy, true);
                    if (response.Equals("ok", StringComparison.OrdinalIgnoreCase))
                    {
                        Helper.LogHelper.WriteToLogFile("Shop Saved and request sent to scrapper", Helper.LogHelper.GetLogFilePath());
                    }
                    else
                    {
                        Helper.LogHelper.WriteToLogFile("Exception occured " + response.Trim(), Helper.LogHelper.GetLogFilePath());
                    }
                }
            }
            else
            {
                Helper.LogHelper.WriteToLogFile("Scheduled Quick Views not found ", Helper.LogHelper.GetLogFilePath());
            }
            return response;
        }

        #endregion

        public void SetQuickViewReview(DateTime SearchDate, long rentalLengthId, long searchSummaryId)
        {
            if (rentalLengthId > 0 && searchSummaryId > 0)
            {
                QuickViewResults quickViewResult = _context.QuickViewResults.Where(obj => obj.RentalLengthId == rentalLengthId && obj.SearchSummaryId == searchSummaryId && System.Data.Entity.DbFunctions.TruncateTime(obj.Date) == System.Data.Entity.DbFunctions.TruncateTime(SearchDate)).FirstOrDefault();
                if (quickViewResult != null)
                {
                    quickViewResult.IsReviewed = true;
                    _context.Entry(quickViewResult).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
            }
        }

        public QuickViewDTO GetQuickViewDetails(long searchSummaryId)
        {
            if (searchSummaryId > 0)
            {
                QuickView objQuickViewEntity = _context.QuickView.Where(d => (!d.IsDeleted && (d.ParentSearchSummaryId == searchSummaryId || d.ChildSearchSummaryId == searchSummaryId))).SingleOrDefault();
                if (objQuickViewEntity != null)
                {
                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                    QuickViewDTO objQuickViewDTO = new QuickViewDTO();
                    objQuickViewDTO.ID = objQuickViewEntity.ID;
                    objQuickViewDTO.CompetitorCompanyIds = objQuickViewEntity.CompetitorCompanyIds;
                    objQuickViewDTO.UIControlId = objQuickViewEntity.UIControlId;
                    objQuickViewDTO.CarClassIds = objQuickViewEntity.CarClassIds;

                    objQuickViewDTO.IsTotal = (objQuickViewEntity.MonitorBase) ? false : true;//Save base on total flag
                    objQuickViewDTO.IsEmailNotification = (objQuickViewEntity.NotifyEmail != null) ? objQuickViewEntity.NotifyEmail.Value : false;
                    objQuickViewDTO.PickupTime = objQuickViewEntity.PickupTime;
                    objQuickViewDTO.DropOffTime = objQuickViewEntity.DropoffTime;

                    if (objQuickViewEntity.NextRunDate.HasValue)
                    {
                        objQuickViewDTO.NextRunDate = TimeZoneInfo.ConvertTimeToUtc(objQuickViewEntity.NextRunDate.Value, easternZone);
                    }
                    objQuickViewDTO.lstQuickViewGroupItem = new List<QuickViewGroupItem>();
                    objQuickViewDTO.lstQuickViewGroupItem = getQuickViewGroupDetails(objQuickViewEntity.ID);
                    return objQuickViewDTO;
                }
            }
            return null;
        }
        public List<QuickViewGroupItem> getQuickViewGroupDetails(long quickviewId)
        {
            IEnumerable<QuickViewGroupItem> lstQuickViewGroupItem = null;
            List<QuickViewGroup> quickViewGroup = new List<QuickViewGroup>();
            quickViewGroup = _context.QuickViewGroups.Where(obj => obj.QuickViewId == quickviewId).ToList();
            if (quickViewGroup.Count() > 0)
            {

                IEnumerable<QuickViewGroupCompany> quickViewGroupCompany = (from quickviewgroup in quickViewGroup
                                                                            join quickviewgroupcomp in _context.QuickViewGroupCompanies on quickviewgroup.ID equals quickviewgroupcomp.GroupId
                                                                            join companies in _context.Companies on quickviewgroupcomp.CompanyId equals companies.ID
                                                                            select new QuickViewGroupCompany
                                                                            {
                                                                                groupId = quickviewgroup.ID,
                                                                                CompanyID = companies.ID,
                                                                                CompanyName = companies.Name
                                                                            });

                IEnumerable<QuickViewCarClassGroup> quickViewCarClassGroup = (from quickviewgroup in quickViewGroup
                                                                              join quickviewcarclassgroup in _context.QuickViewGapDevSettings on quickviewgroup.ID equals quickviewcarclassgroup.CompetitorGroupId
                                                                              select new QuickViewCarClassGroup
                                                                              {
                                                                                  groupId = quickviewgroup.ID,
                                                                                  Id = quickviewcarclassgroup.ID,
                                                                                  //GapValueGroup = quickviewcarclassgroup.GapValue,
                                                                                  DeviationValueGroup = quickviewcarclassgroup.DeviationValue
                                                                              });
                lstQuickViewGroupItem = (from quickView in quickViewGroup
                                         select new QuickViewGroupItem
                                         {
                                             groupId = quickView.ID,
                                             CompetitorIds = string.Join(",", quickViewGroupCompany.Where(obj => obj.groupId == quickView.ID).Select(obj => obj.CompanyID).ToArray()),
                                             lstQuickViewGroupCompany = quickViewGroupCompany.Where(obj => obj.groupId == quickView.ID).ToList(),
                                             QuickViewCarClassGroups = quickViewCarClassGroup.Where(obj => obj.groupId == quickView.ID).ToList(),
                                         }).ToList();
                return lstQuickViewGroupItem.ToList();
            }
            return new List<QuickViewGroupItem>();
        }

        private string GetStatusName(long statusId)
        {
            string status = "SCHEDULED";
            switch (statusId)
            {
                case 1:
                    status = "SCHEDULED";
                    break;
                case 2:
                    status = "IN PROGRESS";
                    break;
                case 4:
                    status = "FINISHED";
                    break;
                case 5:
                    status = "FAILED";
                    break;
            }
            return status;
        }
    }
}
