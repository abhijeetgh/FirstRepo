using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.DTOs;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.IO;
using System.Data.Entity;

namespace RateShopper.Services.Data
{
    public class FTBRatesService : BaseService<FTBRate>, IFTBRatesService
    {
        ISplitMonthDetailsService _iSplitMonthDetailsService;
        IFTBRatesSplitMonthsService _iFTBRatesSplitMonthsService;
        ILocationBrandRentalLengthService _locationBrandRentalLengthService;
        IRentalLengthService _rentalLengthService;

        public FTBRatesService(IEZRACRateShopperContext context, ICacheManager cacheManager, ISplitMonthDetailsService iSplitMonthDetailsService, IFTBRatesSplitMonthsService iFTBRatesSplitMonthsService, ILocationBrandRentalLengthService locationBrandRentalLengthService, IRentalLengthService rentalLengthService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<FTBRate>();
            _cacheManager = cacheManager;
            _iSplitMonthDetailsService = iSplitMonthDetailsService;
            _iFTBRatesSplitMonthsService = iFTBRatesSplitMonthsService;
            _locationBrandRentalLengthService = locationBrandRentalLengthService;
            _rentalLengthService = rentalLengthService;
        }

        public FTBRatesDTO GetFTBRates(long brandLocationId, long rentalLengthId, int year, int month, int selectedFTBRateId)
        {
            //getting location specific LORs
            var locationBrandRentalLengths = _locationBrandRentalLengthService.GetAll().Where(obj => obj.LocationBrandID == brandLocationId);
            IEnumerable<RentalLength> RentalLengths = _rentalLengthService.GetRentalLength();
            List<RentalLengthDTO> rentalLengths = (from tblLocationBrandRentalLengths in locationBrandRentalLengths
                                                   join tblRentalLength in RentalLengths on tblLocationBrandRentalLengths.RentalLengthID equals tblRentalLength.MappedID
                                                   orderby tblRentalLength.DisplayOrder
                                                   select new RentalLengthDTO { ID = tblRentalLength.ID, MappedID = tblRentalLength.MappedID, Code = tblRentalLength.Code }).ToList();

            DateTime selectedDate;
            if (year == 0)
            {
                selectedDate = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
            }
            else
            {
                selectedDate = new DateTime(year, month, 1);
            }

            List<FTBRateDetailsDTO> lstCarClassesWithEmptyFields = (from locationBrandCarClasses in _context.LocationBrandCarClass
                                                                    join carClasses in _context.CarClasses.Where(e => !e.IsDeleted) on locationBrandCarClasses.CarClassID equals carClasses.ID
                                                                    where locationBrandCarClasses.LocationBrandID == brandLocationId
                                                                    orderby carClasses.DisplayOrder
                                                                    select new FTBRateDetailsDTO
                                                                        {
                                                                            CarClass = carClasses.Code,
                                                                            CarClassId = carClasses.ID,
                                                                            FTBRateDetailsId = 0,
                                                                            IsSplitMonth = false
                                                                        }).ToList();


            FTBRateDetailsDTO firstRateDetailObject = (from ftbRate in _context.FTBRates
                                                       join ftbRateDetails in _context.FTBRatesDetails on ftbRate.ID equals ftbRateDetails.FTBRatesId
                                                       where (selectedFTBRateId == 0 || ftbRate.ID == selectedFTBRateId) && ftbRate.LocationBrandId == brandLocationId && ftbRate.Date == selectedDate
                                                       orderby ftbRateDetails.RentalLengthId
                                                       select new FTBRateDetailsDTO
                                                       {
                                                           FTBRatesId = ftbRateDetails.FTBRatesId,
                                                           RentalLengthId = ftbRateDetails.RentalLengthId,
                                                           HasBlackOutPeroid = ftbRate.HasBlackOutPeroid,
                                                           BlackOutStartDate = ftbRate.BlackOutStartDate,
                                                           BlackOutEndDate = ftbRate.BlackOutEndDate,
                                                           IsSplitMonth = ftbRate.IsSplitMonth.HasValue ? ftbRate.IsSplitMonth.Value : false
                                                       }).FirstOrDefault();


            //Get the split sections information
            List<SplitMonthDetailsDTO> lstSplitSection = new List<SplitMonthDetailsDTO>();
            if (firstRateDetailObject != null && firstRateDetailObject.IsSplitMonth.Value)
            {
                lstSplitSection = _iFTBRatesSplitMonthsService.GetAll().Where(d => d.FTBRatesId == firstRateDetailObject.FTBRatesId).Select(d => new SplitMonthDetailsDTO { SplitIndex = d.SplitIndex, Label = d.Label, StartDay = d.StartDay, EndDay = d.EndDay }).OrderBy(d => d.SplitIndex).ToList();
            }
            if (lstSplitSection.Count == 0)
            {
                lstSplitSection = _iSplitMonthDetailsService.GetAll().Select(d => new SplitMonthDetailsDTO { SplitIndex = d.SplitIndex, Label = d.Label, StartDay = d.StartDay, EndDay = d.EndDay }).ToList();
            }

            //Get list of dates which exists in system for copy functionality
            List<FTBCopyMonthsDTO> lstCopyMonths = _context.FTBRates.Where(d => d.LocationBrandId == brandLocationId && d.Date != selectedDate.Date).OrderBy(d => d.Date).ToList().Select(d => new FTBCopyMonthsDTO { Month = d.Date.Month, Year = d.Date.Year, MonthWithYear = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Date.Month) + "-" + d.Date.Year.ToString(), IsSplitMonth = d.IsSplitMonth.HasValue ? d.IsSplitMonth.Value : false, Labels = _iFTBRatesSplitMonthsService.GetAll().Where(e => e.FTBRatesId == d.ID).Select(e => e.Label).ToArray() }).ToList();


            //Create the dictionary as per number of split section
            Dictionary<string, List<FTBRateDetailsDTO>> dicFTBRates = new Dictionary<string, List<FTBRateDetailsDTO>>();
            List<FTBRateDetailsDTO> newList;
            lstSplitSection.ForEach(d =>
            {
                newList = new List<FTBRateDetailsDTO>(lstCarClassesWithEmptyFields.Count);

                lstCarClassesWithEmptyFields.ForEach(e =>
                {
                    newList.Add(new FTBRateDetailsDTO()
                    {
                        SplitIndex = d.SplitIndex,
                        CarClass = e.CarClass,
                        CarClassId = e.CarClassId,
                        IsSplitMonth = firstRateDetailObject != null ? firstRateDetailObject.IsSplitMonth : e.IsSplitMonth,
                        FTBRatesId = firstRateDetailObject != null ? firstRateDetailObject.FTBRatesId : 0
                    });
                });
                dicFTBRates.Add(d.SplitIndex.ToString(), newList);
            });

            if (firstRateDetailObject != null)
            {
                List<FTBRateDetailsDTO> ftbRatesDetail = new List<FTBRateDetailsDTO>();

                ftbRatesDetail = (from detail in _context.FTBRatesDetails.Where(d => d.FTBRatesId == firstRateDetailObject.FTBRatesId && d.RentalLengthId == (rentalLengthId > 0 ? rentalLengthId : firstRateDetailObject.RentalLengthId))
                                  select new FTBRateDetailsDTO
                                  {
                                      FTBRatesId = detail.FTBRatesId,
                                      FTBRateDetailsId = detail.ID,
                                      CarClassId = detail.CarClassId,
                                      RentalLengthId = detail.RentalLengthId,
                                      SplitIndex = detail.SplitPartId,
                                      Sunday = detail.Sunday,
                                      Monday = detail.Monday,
                                      Tuesday = detail.Tuesday,
                                      Wednesday = detail.Wednesday,
                                      Thursday = detail.Thursday,
                                      Friday = detail.Friday,
                                      Saturday = detail.Saturday,
                                      IsSplitMonth = firstRateDetailObject.IsSplitMonth
                                  }).ToList();

                if (ftbRatesDetail.Count > 0)
                {
                    //Get the FTB Rate Id
                    long ftbRateId = ftbRatesDetail[0].FTBRatesId;

                    FTBRatesDTO objFTBRatesDTO = new FTBRatesDTO();
                    objFTBRatesDTO.rentalLengths = rentalLengths;
                    objFTBRatesDTO.ID = firstRateDetailObject.FTBRatesId;
                    objFTBRatesDTO.LowestRentalLengthId = rentalLengthId > 0 ? rentalLengthId : firstRateDetailObject.RentalLengthId;
                    objFTBRatesDTO.Month = selectedDate.Month;
                    objFTBRatesDTO.Year = selectedDate.Year;
                    objFTBRatesDTO.SplitLabels = lstSplitSection;
                    objFTBRatesDTO.LstCopyMonths = lstCopyMonths;

                    objFTBRatesDTO.SelectedLORs = string.Join(",", _context.FTBRatesDetails.Where(d => d.FTBRatesId == firstRateDetailObject.FTBRatesId).OrderBy(d => d.RentalLengthId).Select(d => d.RentalLengthId).Distinct().ToArray());
                    objFTBRatesDTO.LocationBrandId = brandLocationId;
                    objFTBRatesDTO.HasBlackOutPeroid = firstRateDetailObject.HasBlackOutPeroid.HasValue ? firstRateDetailObject.HasBlackOutPeroid.Value : false;
                    objFTBRatesDTO.BlackOutStartDate = firstRateDetailObject.BlackOutStartDate;
                    objFTBRatesDTO.BlackOutEndDate = firstRateDetailObject.BlackOutEndDate;
                    objFTBRatesDTO.IsSplitMonth = firstRateDetailObject.IsSplitMonth.HasValue ? firstRateDetailObject.IsSplitMonth.Value : false;

                    objFTBRatesDTO.DicFTBRateDetails = dicFTBRates;
                    foreach (var item in objFTBRatesDTO.DicFTBRateDetails)
                    {
                        item.Value.ForEach(d =>
                        {
                            FTBRateDetailsDTO obj = ftbRatesDetail.Find(g => g.CarClassId == d.CarClassId && g.SplitIndex == Convert.ToInt32(item.Key));
                            if (obj != null)
                            {
                                d.Sunday = obj.Sunday;
                                d.Monday = obj.Monday;
                                d.Tuesday = obj.Tuesday;
                                d.Wednesday = obj.Wednesday;
                                d.Thursday = obj.Thursday;
                                d.Friday = obj.Friday;
                                d.Saturday = obj.Saturday;
                                d.FTBRateDetailsId = obj.FTBRateDetailsId;
                                d.SplitIndex = obj.SplitIndex;
                                d.RentalLengthId = obj.RentalLengthId;
                            }
                        });
                    }

                    return objFTBRatesDTO;

                }
                else
                {
                    FTBRatesDTO objFTBRateEmptyObject = new FTBRatesDTO();
                    objFTBRateEmptyObject.rentalLengths = rentalLengths;
                    objFTBRateEmptyObject.ID = firstRateDetailObject.FTBRatesId;
                    objFTBRateEmptyObject.LowestRentalLengthId = rentalLengthId == 0 ? (rentalLengths.Count() == 0 ? 1 : rentalLengths.First().MappedID) : rentalLengthId;
                    objFTBRateEmptyObject.LocationBrandId = brandLocationId;
                    objFTBRateEmptyObject.Month = selectedDate.Month;
                    objFTBRateEmptyObject.Year = selectedDate.Year;
                    objFTBRateEmptyObject.SplitLabels = lstSplitSection;
                    objFTBRateEmptyObject.LstCopyMonths = lstCopyMonths;
                    objFTBRateEmptyObject.IsSplitMonth = firstRateDetailObject.IsSplitMonth.HasValue ? firstRateDetailObject.IsSplitMonth.Value : false;
                    objFTBRateEmptyObject.HasBlackOutPeroid = firstRateDetailObject.HasBlackOutPeroid.HasValue ? firstRateDetailObject.HasBlackOutPeroid.Value : false;
                    objFTBRateEmptyObject.BlackOutStartDate = firstRateDetailObject.BlackOutStartDate;
                    objFTBRateEmptyObject.BlackOutEndDate = firstRateDetailObject.BlackOutEndDate;
                    objFTBRateEmptyObject.SelectedLORs = string.Join(",", _context.FTBRatesDetails.Where(d => d.FTBRatesId == firstRateDetailObject.FTBRatesId).OrderBy(d => d.RentalLengthId).Select(d => d.RentalLengthId).Distinct().ToArray());

                    objFTBRateEmptyObject.DicFTBRateDetails = dicFTBRates;

                    return objFTBRateEmptyObject;
                }
            }
            else
            {
                FTBRatesDTO objEmptyObject = new FTBRatesDTO();
                objEmptyObject.rentalLengths = rentalLengths;
                objEmptyObject.ID = 0;
                objEmptyObject.LowestRentalLengthId = rentalLengthId == 0 ? (rentalLengths.Count() == 0 ? 1 : rentalLengths.First().MappedID) : rentalLengthId;
                objEmptyObject.LocationBrandId = brandLocationId;
                objEmptyObject.Month = selectedDate.Month;
                objEmptyObject.Year = selectedDate.Year;
                objEmptyObject.SplitLabels = lstSplitSection;
                objEmptyObject.LstCopyMonths = lstCopyMonths;
                objEmptyObject.IsSplitMonth = false;
                objEmptyObject.HasBlackOutPeroid = false;
                objEmptyObject.DicFTBRateDetails = dicFTBRates;

                return objEmptyObject;
            }
        }

        public FTBCustomRateDTO SaveFTBRates(FTBRatesDTO objFTBRatesDTO)
        {
            FTBCustomRateDTO ftbCustomRateDTO = new FTBCustomRateDTO();
            List<FTBRateDetailsDTO> combinedListRateDetails = new List<FTBRateDetailsDTO>();
            DataTable dtFTBRateDetails;
            if (objFTBRatesDTO.DicFTBRateDetails != null)
            {
                foreach (var lst in objFTBRatesDTO.DicFTBRateDetails)
                {
                    combinedListRateDetails.AddRange(lst.Value);
                }
                dtFTBRateDetails = ToDataTable<FTBRateDetailsDTO>(combinedListRateDetails);
            }
            else
            {
                dtFTBRateDetails = ToDataTable<FTBRateDetailsDTO>(new List<FTBRateDetailsDTO>());
            }

            DataTable dtSplitMonthDetails = ToDataTable<SplitMonthDetailsDTO>(objFTBRatesDTO.SplitLabels);


            List<SqlParameter> queryParameters = new List<SqlParameter>();
            queryParameters.Add(new SqlParameter("ID", objFTBRatesDTO.ID));
            queryParameters.Add(new SqlParameter("FTBDate", new DateTime(objFTBRatesDTO.Year, objFTBRatesDTO.Month, 1).Date));
            queryParameters.Add(new SqlParameter("IsSplitMonth", objFTBRatesDTO.IsSplitMonth));
            queryParameters.Add(new SqlParameter("LocationBrandId", objFTBRatesDTO.LocationBrandId));
            queryParameters.Add(new SqlParameter("HasBlackOutPeroid", objFTBRatesDTO.HasBlackOutPeroid));
            queryParameters.Add(new SqlParameter("SelectedLORs", objFTBRatesDTO.SelectedLORs));
            queryParameters.Add(new SqlParameter("SelectedRentalLength", objFTBRatesDTO.LowestRentalLengthId));
            SqlParameter tableValueParameter = new SqlParameter("FTBRatesDetail", SqlDbType.Structured);
            tableValueParameter.Value = dtFTBRateDetails;
            tableValueParameter.TypeName = "dbo.FTBRateDetailsType";
            queryParameters.Add(tableValueParameter);
            SqlParameter splitMonthDetails = new SqlParameter("SplitMonthDetail", SqlDbType.Structured);
            splitMonthDetails.Value = dtSplitMonthDetails;
            splitMonthDetails.TypeName = "dbo.SplitMonthDetailsType";
            queryParameters.Add(splitMonthDetails);
            queryParameters.Add(new SqlParameter("CreatedBy", objFTBRatesDTO.CreatedBy));
            queryParameters.Add(new SqlParameter("UpdateBy", objFTBRatesDTO.UpdatedBy));
            queryParameters.Add(new SqlParameter("CreatedDateTime", DateTime.Now));
            queryParameters.Add(new SqlParameter("UpdatedDateTime", DateTime.Now));
            if (objFTBRatesDTO.HasBlackOutPeroid)
            {
                queryParameters.Add(new SqlParameter("BlackOutStartDate", objFTBRatesDTO.BlackOutStartDate.Value));
                queryParameters.Add(new SqlParameter("BlackOutEndDate", objFTBRatesDTO.BlackOutEndDate.Value));
            }

            string querySaveFTBRates = "EXEC SaveFTBRates ";
            querySaveFTBRates = string.Concat(querySaveFTBRates, string.Join(", ", queryParameters.Select(d => "@" + d.ParameterName)));
            var data = _context.ExecuteSqlCommand(querySaveFTBRates, queryParameters.ToArray());

            string Message = "Success";
            //Scenario if blackout date changed
            if (objFTBRatesDTO.OldHasBlackOutPeroid != objFTBRatesDTO.HasBlackOutPeroid)
            {
                FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
                ftbAutomationScenarioDTO.StartDate = (new DateTime(objFTBRatesDTO.Year, objFTBRatesDTO.Month, 1).Date);
                ftbAutomationScenarioDTO.IsFTBJob = true;
                ftbAutomationScenarioDTO.IsBlackoutDatesChange = true;
                ftbAutomationScenarioDTO.LocationBrandID = objFTBRatesDTO.LocationBrandId;
                ftbAutomationScenarioDTO.LoggedUserID = objFTBRatesDTO.UpdatedBy;
                ftbAutomationScenarioDTO = CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);

                if (ftbAutomationScenarioDTO.IsReturnMsg)
                {
                    if (ftbAutomationScenarioDTO.ReturnMessage == "notconfiguredblackoutdates")
                    {
                        Message = "Successnotconfiguredblackoutdates";
                    }
                    else
                    {
                        Message = "Success" + ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString();
                    }
                }

            }

            //Scenario if blackout date has been changed then all automation job should  disable.
            if (objFTBRatesDTO.HasBlackOutPeroid && objFTBRatesDTO.OldBlackOutStartDate != null && (objFTBRatesDTO.OldBlackOutStartDate != objFTBRatesDTO.BlackOutStartDate || objFTBRatesDTO.OldBlackOutEndDate != objFTBRatesDTO.BlackOutEndDate))
            {
                FTBAutomationScenarioDTO ftbAutomationScenarioDTO = new FTBAutomationScenarioDTO();
                ftbAutomationScenarioDTO.StartDate = objFTBRatesDTO.OldBlackOutEndDate.Value;
                ftbAutomationScenarioDTO.IsFTBJob = true;
                ftbAutomationScenarioDTO.LocationBrandID = objFTBRatesDTO.LocationBrandId;
                ftbAutomationScenarioDTO.LoggedUserID = objFTBRatesDTO.UpdatedBy;
                ftbAutomationScenarioDTO = CommonFTBJobUpdateScenarios(ftbAutomationScenarioDTO);

                if (ftbAutomationScenarioDTO.IsReturnMsg)
                {
                    if (ftbAutomationScenarioDTO.ReturnMessage == "notconfiguredblackoutdates")
                    {
                        Message = "Successnotconfiguredblackoutdates";
                    }
                    else
                    {
                        Message = "Success" + ftbAutomationScenarioDTO.ReturnMessage + "-" + ftbAutomationScenarioDTO.BlackoutStartDate.Value.ToShortDateString() + "-" + ftbAutomationScenarioDTO.BlackoutEndDate.Value.ToShortDateString();
                    }
                }
            }
            _cacheManager.Remove(typeof(FTBRatesSplitMonthDetails).ToString());
            ftbCustomRateDTO.Message = Message;
            ftbCustomRateDTO.ftbRatesDTO = GetFTBRates(objFTBRatesDTO.LocationBrandId, objFTBRatesDTO.LowestRentalLengthId, objFTBRatesDTO.Year, objFTBRatesDTO.Month, 0);

            return ftbCustomRateDTO;
        }

        public async Task<int> CopyFTBRates(FTBCopyMonthsDTO objFTBCopyMonthsDTO, bool isRequestFromFTBSchedular)
        {
            DateTime copyFromDate = new DateTime(objFTBCopyMonthsDTO.SourceYear, objFTBCopyMonthsDTO.SourceMonth, 1);
            DateTime copyToDate = new DateTime(objFTBCopyMonthsDTO.Year, objFTBCopyMonthsDTO.Month, 1);
            DataTable dtSplitMonthDetails = ToDataTable<SplitMonthDetailsDTO>(objFTBCopyMonthsDTO.SplitLabels);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("locationBrandId", objFTBCopyMonthsDTO.LocationBrandId));
            sqlParameters.Add(new SqlParameter("CopyFromDate", copyFromDate.Date));
            sqlParameters.Add(new SqlParameter("CopyToDate", copyToDate.Date));
            sqlParameters.Add(new SqlParameter("CreatedDateTime", DateTime.Now));
            sqlParameters.Add(new SqlParameter("IsRequestFromFTBSchedular", isRequestFromFTBSchedular));
            SqlParameter splitMonthDetails = new SqlParameter("SplitMonthDetail", SqlDbType.Structured);
            splitMonthDetails.Value = dtSplitMonthDetails;
            splitMonthDetails.TypeName = "dbo.SplitMonthDetailsType";
            sqlParameters.Add(splitMonthDetails);
            if (!isRequestFromFTBSchedular)
            {
                sqlParameters.Add(new SqlParameter("IsSplitMonth", objFTBCopyMonthsDTO.IsSplitMonth));
                sqlParameters.Add(new SqlParameter("CopyFromSplitSection", objFTBCopyMonthsDTO.SourceSplitIndex));
                sqlParameters.Add(new SqlParameter("CopyToSplitSection", objFTBCopyMonthsDTO.DestinationSplitIndex));
                sqlParameters.Add(new SqlParameter("HasBlackOutPeroid", objFTBCopyMonthsDTO.HasBlackOutPeroid));
                sqlParameters.Add(new SqlParameter("CreatedBy", objFTBCopyMonthsDTO.CreatedBy));
                if (objFTBCopyMonthsDTO.HasBlackOutPeroid)
                {
                    sqlParameters.Add(new SqlParameter("BlackOutStartDate", objFTBCopyMonthsDTO.BlackOutStartDate.Value));
                    sqlParameters.Add(new SqlParameter("BlackOutEndDate", objFTBCopyMonthsDTO.BlackOutEndDate.Value));
                }
            }


            string queryCopyFTBRates = "EXEC CopyFTBRates ";
            queryCopyFTBRates = string.Concat(queryCopyFTBRates, string.Join(", ", sqlParameters.Select(d => "@" + d.ParameterName)));
            var results = await _context.ExecuteSQLQuery<int>(queryCopyFTBRates, sqlParameters.ToArray());

            _cacheManager.Remove(typeof(FTBRatesSplitMonthDetails).ToString());
            _cacheManager.Remove(typeof(FTBRate).ToString());
            _cacheManager.Remove(typeof(FTBRatesDetail).ToString());

            if (results != null)
            {
                var lstResult = results.ToList();

                if (lstResult.Count > 0)
                {
                    return lstResult.FirstOrDefault();
                }
            }
            return 0;
        }

        public List<int> GetYears()
        {
            List<int> years = new List<int>();
            years.Add(DateTime.Now.Year - 1);
            years.Add(DateTime.Now.Year);
            years.Add(DateTime.Now.Year + 1);

            return years;
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            string[] ValidColumns = { "FTBRatesId", "FTBRateDetailsId", "RentalLengthId", "CarClassId", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "SplitIndex", "Label", "StartDay", "EndDay" };

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(d => ValidColumns.Contains(d.Name)).ToArray();
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public async Task<List<FTBRateDetailsDTO>> fetchFTBRatesDetail(long ftbRatesId)
        {
            const string ratesDetailsQuery = "Select FBRD.RentalLengthId,FBRD.CarClassId,CRS.Code as CarClass,FBRD.Sunday,FBRD.Monday,FBRD.Tuesday,FBRD.Wednesday,FBRD.Thursday,FBRD.Friday,FBRD.Saturday,FBRD.SplitPartId as SplitIndex,RS.Code as RentalLengthCode from FTBRatesDetails FBRD join CarClasses CRS on CRS.ID= FBRD.CarClassId join RentalLengths RS on RS.MappedID=FBRD.RentalLengthId Where FBRD.FTBRatesId = @ftbRatesID";
            var queryParameters = new[] { new SqlParameter("@ftbRatesID", ftbRatesId) };
            var ftbRatesDetails = await _context.ExecuteSQLQuery<FTBRateDetailsDTO>(ratesDetailsQuery, queryParameters);
            return ftbRatesDetails.ToList<FTBRateDetailsDTO>();
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


            //if (ftbAutomationScenarioDTO.IsBlackoutDatesChange)
            //{
            //    FTBScheduledJob ftbScheduledJobsingle = null;
            //    ftbScheduledJobsingle = (from ftbJob in _context.FTBScheduledJobs
            //                             where !ftbJob.IsDeleted &&
            //                             ftbJob.LocationBrandID == ftbAutomationScenarioDTO.LocationBrandID &&
            //                             ftbJob.StartDate.Year == ftbAutomationScenarioDTO.StartDate.Year &&
            //                             ftbJob.StartDate.Month == ftbAutomationScenarioDTO.StartDate.Month
            //                             select ftbJob).FirstOrDefault();

            //    //Scenario if FTB job is scheduled but it is stop. and user want tog go for initiate new normal automation job at that time user able to initiate a automation job.
            //    if (ftbScheduledJobsingle != null && ftbScheduledJobsingle.NextRunDateTime != null && !(ftbScheduledJobsingle.IsEnabled))
            //    {
            //        ftbAutomationScenarioDTO.IsReturnMsg = false;
            //        return ftbAutomationScenarioDTO;
            //    }
            //    else if (ftbRate.HasBlackOutPeroid.HasValue && !ftbRate.HasBlackOutPeroid.Value)//If blackout period is not assign than all job should be disabled
            //    {
            //        FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);
            //    }
            //    else if (scheduledJobs.Any() && blackoutMatchScheduledJobs.Any())
            //    {
            //        IEnumerable<ScheduledJob> scheduledDateMatchUpdatejob = (from sjob in scheduledJobs
            //                                                                 join bsjob in blackoutMatchScheduledJobs on 1 equals 1
            //                                                                 where sjob.ID != bsjob.ID
            //                                                                 select sjob).ToList();
            //        if (scheduledDateMatchUpdatejob.Any())
            //        {
            //            FTPDisabledAutomationJobs(scheduledDateMatchUpdatejob, ftbAutomationScenarioDTO.LoggedUserID);
            //            ftbAutomationScenarioDTO.ReturnMessage = "AutomationJobDisabled";
            //            ftbAutomationScenarioDTO.IsReturnMsg = true;
            //            ftbAutomationScenarioDTO.BlackoutStartDate = ftbRate.BlackOutStartDate;
            //            ftbAutomationScenarioDTO.BlackoutEndDate = ftbRate.BlackOutEndDate;
            //        }
            //    }

            //    FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);
            //    return ftbAutomationScenarioDTO;
            //}
            //Scenario

            //This condition is use when normal automation job shcduled, it checks weather this month has ftbjob is exists or not 
            if (ftbAutomationScenarioDTO.IsFTBJob)
            {
                bool ReturnFlag = false;
                //Scheduling FTB job 
                FTBScheduledJob ftbScheduledJobsingle = null;
                ftbScheduledJobsingle = (from ftbJob in _context.FTBScheduledJobs
                                         where !ftbJob.IsDeleted &&
                                         ftbJob.LocationBrandID == ftbAutomationScenarioDTO.LocationBrandID &&
                                         ftbJob.StartDate.Year == ftbAutomationScenarioDTO.StartDate.Year &&
                                         ftbJob.StartDate.Month == ftbAutomationScenarioDTO.StartDate.Month
                                         select ftbJob).FirstOrDefault();

                if (ftbScheduledJobsingle != null && ftbScheduledJobsingle.NextRunDateTime != null && !(ftbScheduledJobsingle.IsEnabled))
                {
                    ftbAutomationScenarioDTO.IsReturnMsg = false;
                    return ftbAutomationScenarioDTO;
                }
                else if (scheduledJobs.Any() && ftbScheduledJobsingle == null && !blackoutMatchScheduledJobs.Any())
                {
                    ftbAutomationScenarioDTO.IsReturnMsg = false;
                    return ftbAutomationScenarioDTO;
                }
                if (scheduledJobs != null && blackoutMatchScheduledJobs != null && scheduledJobs.Any() && blackoutMatchScheduledJobs.Any())
                {
                    //get scehduled job which not match blackout dates
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
                    //this else used for no blackout period assigned than all given automation job should disable
                    ReturnFlag = FTPDisabledAutomationJobs(scheduledJobs, ftbAutomationScenarioDTO.LoggedUserID);

                    if (ftbRate.HasBlackOutPeroid.HasValue && !ftbRate.HasBlackOutPeroid.Value)
                    {
                        ftbAutomationScenarioDTO.IsReturnMsg = true;
                        ftbAutomationScenarioDTO.ReturnMessage = "notconfiguredblackoutdates";
                        return ftbAutomationScenarioDTO;
                    }
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
    }
}
