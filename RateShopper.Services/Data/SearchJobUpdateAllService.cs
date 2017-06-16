using RateShopper.Domain.Entities;
using RateShopper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Core.Cache;
using RateShopper.Domain.DTOs;


namespace RateShopper.Services.Data
{
    public class SearchJobUpdateAllService : BaseService<SearchJobUpdateAll>, ISearchJobUpdateAllService
    {
        public SearchJobUpdateAllService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<SearchJobUpdateAll>();
            _cacheManager = cacheManager;
        }
        public List<SearchJobUpdateAll> GetPreloaBaseRate(long SearchSummaryId, bool isDailyPreLoad)
        {
            List<SearchJobUpdateAll> lstSearchJobUpdateAll = new List<SearchJobUpdateAll>();
            if (SearchSummaryId > 0)
            {
                lstSearchJobUpdateAll = _context.SearchJobUpdateAlls.Where(obj => obj.SearchSummaryId == SearchSummaryId && obj.IsDaily == isDailyPreLoad).ToList();
            }
            return lstSearchJobUpdateAll;
        }
        public string InsertUpdateTSDUpdateAll(long SearchSummaryId, List<SearchJobUpdateAllDTO> SearchJobUpdateAll)
        {
            if (SearchJobUpdateAll.Count > 0)
            {
                List<SearchJobUpdateAll> lstSearchJobUpdateAll = _context.SearchJobUpdateAlls.Where(obj => obj.SearchSummaryId == SearchSummaryId).ToList();
                if (lstSearchJobUpdateAll.Count > 0)
                {
                    foreach (var updateAllTSD in SearchJobUpdateAll)
                    {
                        SearchJobUpdateAll SearchJobUpdateAllobj = lstSearchJobUpdateAll.Where(obj => obj.CarClassId == updateAllTSD.CarClassId && obj.IsDaily == updateAllTSD.IsDaily).FirstOrDefault();
                        if (SearchJobUpdateAllobj != null)
                        {
                            if (updateAllTSD.BaseEdit != null)
                            {
                                SearchJobUpdateAllobj.BaseEdit = updateAllTSD.BaseEdit;
                            }
                            else
                            {
                                SearchJobUpdateAllobj.BaseEdit = SearchJobUpdateAllobj.BaseEdit;
                            }
                            if (updateAllTSD.BaseEditTwo != null)
                            {
                                SearchJobUpdateAllobj.BaseEditTwo = updateAllTSD.BaseEditTwo;
                            }
                            else
                            {
                                SearchJobUpdateAllobj.BaseEditTwo = SearchJobUpdateAllobj.BaseEditTwo;
                            }

                            _context.Entry(SearchJobUpdateAllobj).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            SearchJobUpdateAll objSearchJobUpdateAll = new SearchJobUpdateAll();
                            objSearchJobUpdateAll.SearchSummaryId = updateAllTSD.SearchSummaryId;
                            objSearchJobUpdateAll.CarClassId = updateAllTSD.CarClassId;
                            objSearchJobUpdateAll.BaseEdit = updateAllTSD.BaseEdit;
                            objSearchJobUpdateAll.BaseEditTwo = updateAllTSD.BaseEditTwo;
                            objSearchJobUpdateAll.IsDaily = updateAllTSD.IsDaily;
                            _context.SearchJobUpdateAlls.Add(objSearchJobUpdateAll);
                        }
                    }
                    _context.SaveChanges();
                }
                else
                {
                    foreach (var updateAllTSD in SearchJobUpdateAll)
                    {
                        SearchJobUpdateAll objSearchJobUpdateAll = new SearchJobUpdateAll();
                        objSearchJobUpdateAll.SearchSummaryId = updateAllTSD.SearchSummaryId;
                        objSearchJobUpdateAll.CarClassId = updateAllTSD.CarClassId;
                        objSearchJobUpdateAll.BaseEdit = updateAllTSD.BaseEdit;
                        objSearchJobUpdateAll.BaseEditTwo = updateAllTSD.BaseEditTwo;
                        objSearchJobUpdateAll.IsDaily = updateAllTSD.IsDaily;
                        _context.SearchJobUpdateAlls.Add(objSearchJobUpdateAll);
                    }
                    _context.SaveChanges();
                }
            }
            return "success";
        }
    }
}
