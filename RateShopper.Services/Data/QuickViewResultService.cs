using RateShopper.Data;
using System.Collections.Generic;
using System.Linq;
using RateShopper.Core.Cache;
using RateShopper.Domain.Entities;
using RateShopper.Domain.DTOs;
using System;
using RateShopper.Services.Helper;
using System.Configuration;

namespace RateShopper.Services.Data
{
    public class QuickViewResultService : BaseService<QuickViewResults>, IQuickViewResultsService
    {
        IRentalLengthService _rentalLengthService;

        public QuickViewResultService(IEZRACRateShopperContext context, ICacheManager cacheManager, IRentalLengthService rentalLengthService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<QuickViewResults>();
            _cacheManager = cacheManager;
            _rentalLengthService = rentalLengthService;
        }
        public QuickViewReportDTO GetQuickViewResult(long QuickViewId, long SearchSummaryId)
        {
            QuickViewReportDTO quickViewReportDTO = null;
            long[] rentalLength = null;
            List<DateRows> quickViewResultDates = new List<DateRows>();
            Dictionary<long, string> rentalLengths = new Dictionary<long, string>();
            QuickViewRow quickViewResultRow = new QuickViewRow();
            SearchSummary searchSummary = null;
            DateTime searchStartDate, searchEndDate;
            DateTime[] arrivalDates;
            List<QuickViewResults> quickViewResults = null;
            List<RentalLenghts> lorList = new List<RentalLenghts>();
            DateRows dateRowResult = new DateRows();
            int statusComplete = Convert.ToInt32(ConfigurationManager.AppSettings["StatusComplete"]);
            if (QuickViewId > 0 && SearchSummaryId > 0)
            {
                rentalLengths = _rentalLengthService.GetAll().Select(obj => new { MappedId = obj.MappedID, LOR = obj.Code })
                                                                    .ToDictionary(obj => obj.MappedId, obj => obj.LOR);

                string searchRentalLength = string.Empty;
                searchSummary = _context.SearchSummaries.Where(search => search.ID == SearchSummaryId && search.IsQuickView.Value && search.StatusID == statusComplete).FirstOrDefault();

                if (searchSummary != null)
                {
                    quickViewReportDTO = new QuickViewReportDTO();
                    searchRentalLength = searchSummary.RentalLengthIDs;
                    if (!string.IsNullOrEmpty(searchRentalLength))
                    {
                        rentalLength = Common.StringToLongList(searchSummary.RentalLengthIDs).OrderBy(length => length).ToArray();
                    }

                    lorList = rentalLength
                            .Join(rentalLengths, length => length, lors => lors.Key, (length, lors) => new RentalLenghts { RentalLengthId = length, RentalLength = lors.Value }).Distinct(new RentalLengthComparer(lor => lor.RentalLengthId))
                               .OrderBy(quick => quick.RentalLengthId).ToList();

                    searchStartDate = searchSummary.StartDate.Date;
                    searchEndDate = searchSummary.EndDate.Date;
                    arrivalDates = Enumerable.Range(0, 1 + searchEndDate.Date.Subtract(searchStartDate.Date).Days).Select(offset => searchStartDate.AddDays(offset)).ToArray();

                    quickViewResults = _context.QuickViewResults.Where(quick => quick.QuickViewId == QuickViewId && quick.SearchSummaryId == SearchSummaryId).ToList();

                    if (quickViewResults.Count > 0)
                    {
                        foreach (DateTime day in arrivalDates)
                        {
                            List<QuickViewRow> quickViewRows = new List<QuickViewRow>();
                            foreach (long lorid in rentalLength)
                            {
                                QuickViewRow row = new QuickViewRow();
                                if (quickViewResults.Where(quick => quick.Date.Date == day && quick.RentalLengthId == lorid).Count() > 0)
                                {
                                    row = quickViewResults.Where(quick => quick.Date.Date == day && quick.RentalLengthId == lorid).Select(quick =>
                                     new QuickViewRow
                                     {
                                         ID = quick.ID,
                                         SearchSummaryId = quick.SearchSummaryId,
                                         QuickViewId = quick.QuickViewId,
                                         RentalLengthId = quick.RentalLengthId,
                                         IsMovedUp = quick.IsMovedUp.HasValue ? quick.IsMovedUp.Value : (bool?)null,
                                         IsReviewed = quick.IsReviewed.HasValue ? quick.IsReviewed.Value : (bool?)null,
                                         FormattedDate = day.Date.ToString("yyyyMMd"),
                                         IsPositionChange = quick.IsPositionChange.HasValue ? quick.IsPositionChange.Value : false,
                                     }).FirstOrDefault();
                                }
                                else
                                {
                                    row = new QuickViewRow
                                    {
                                        SearchSummaryId = SearchSummaryId,
                                        QuickViewId = QuickViewId,
                                        RentalLengthId = lorid,
                                        IsMovedUp = (bool?)null,
                                    };
                                }
                                quickViewRows.Add(row);
                            }
                            dateRowResult = new DateRows
                            {
                                Date = day.Date.ToString("MM/dd/yyyy"),
                                QuickViewResult = quickViewRows
                            };
                            quickViewResultDates.Add(dateRowResult);
                        }
                        quickViewReportDTO.Dates = quickViewResultDates;
                        quickViewReportDTO.LORs = lorList;
                    }
                    return quickViewReportDTO;
                }
            }

            return quickViewReportDTO;
        }
        public QuickViewLengthDateCombinationDTO GetlengthDateCombination(long SearchSummaryID)
        {
            QuickViewLengthDateCombinationDTO quickViewLengthDateCombinationDTO = new QuickViewLengthDateCombinationDTO();

            List<LengthDateCombination> result = (from quickViewResults in _context.QuickViewResults.Where(obj => obj.SearchSummaryId == SearchSummaryID)
                                                  join rentalLength in _context.RentalLengths on quickViewResults.RentalLengthId equals rentalLength.ID
                                                  orderby quickViewResults.Date, rentalLength.ID
                                                  select new
                                                  {
                                                      Changed = quickViewResults.IsMovedUp.HasValue,
                                                      Date = quickViewResults.Date,
                                                      RentalLengthId = rentalLength.ID,
                                                      RentalLength = rentalLength.Code,
                                                  }).Distinct().OrderBy(a => a.RentalLengthId).ThenBy(a => a.Date).ToList().Select(a => new LengthDateCombination
                          {
                              FormattedDate = a.Date.ToString("yyyyMMd"),
                              RentalLengthId = a.RentalLengthId,
                              DisplayText = a.RentalLength + ", " + a.Date.ToString("ddd-MMM dd"),
                              Changed = a.Changed
                          }).ToList();

            if (result != null)
            {
                quickViewLengthDateCombinationDTO.LengthDateCombinationChanged = result.Where(a => a.Changed).Distinct().ToList<LengthDateCombination>();
                quickViewLengthDateCombinationDTO.LengthDateCombinationUnChanged = result.Where(a => !a.Changed).Distinct().ToList<LengthDateCombination>();
            }
            return quickViewLengthDateCombinationDTO;
        }

        class RentalLengthComparer : IEqualityComparer<RateShopper.Domain.DTOs.RentalLenghts>
        {
            private Func<RateShopper.Domain.DTOs.RentalLenghts, object> _funcDistinct;
            public RentalLengthComparer(Func<RateShopper.Domain.DTOs.RentalLenghts, object> funcDistinct)
            {
                this._funcDistinct = funcDistinct;
            }

            public bool Equals(RateShopper.Domain.DTOs.RentalLenghts x, RateShopper.Domain.DTOs.RentalLenghts y)
            {
                return _funcDistinct(x).Equals(_funcDistinct(y));
            }

            public int GetHashCode(RateShopper.Domain.DTOs.RentalLenghts obj)
            {
                return this._funcDistinct(obj).GetHashCode();
            }
        }
    }
}
