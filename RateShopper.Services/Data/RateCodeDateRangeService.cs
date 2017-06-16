using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class RateCodeDateRangeService : BaseService<RateCodeDateRange>, IRateCodeDateRangeService
    {
        public RateCodeDateRangeService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RateCodeDateRange>();
            _cacheManager = cacheManager;
        }

        public List<RateCodeDateRangeDTO> GetRateCodeDateRangeDetails(long ratecodeID)
        {
            return GetAll().Where(d => d.RateCodeID == ratecodeID && d.EndDate >= DateTime.Now.Date).Select(d => new RateCodeDateRangeDTO { ID = d.ID, StartDate = d.StartDate, EndDate = d.EndDate }).ToList();
        }

        public void SaveRateCodeDateRange(IEnumerable<RateCodeDateRangeDTO> DateRangeList, long ratecodeID)
        {
            foreach (var objDate in DateRangeList)
            {
                if (objDate.ID == 0)
                {
                    RateCodeDateRange newRateCodeDateRange = new RateCodeDateRange()
                    {
                        StartDate = objDate.StartDate,
                        EndDate = objDate.EndDate,
                        RateCodeID = ratecodeID
                    };
                    _context.RateCodeDateRanges.Add(newRateCodeDateRange);
                }
                else if (objDate.ID > 0 && objDate.isUpdated)
                {
                    RateCodeDateRange dateRangeToUpdate = GetById(objDate.ID, false);
                    if (dateRangeToUpdate != null)
                    {
                        dateRangeToUpdate.StartDate = objDate.StartDate;
                        dateRangeToUpdate.EndDate = objDate.EndDate;
                        _context.Entry(dateRangeToUpdate).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
            _context.SaveChanges();
            _cacheManager.Remove(typeof(RateCodeDateRange).ToString());
        }

        public void DeleteRateCodeDateRange(IEnumerable<long> deletedItemsId)
        {
            RateCodeDateRange objRateCodeDateRange = null;            
            foreach(var id in deletedItemsId)
            {
                objRateCodeDateRange = GetById(id, false);
                if (objRateCodeDateRange != null)
                {
                    _context.RateCodeDateRanges.Remove(objRateCodeDateRange);
                }
                objRateCodeDateRange = null;
            }
            _context.SaveChanges();
            _cacheManager.Remove(typeof(RateCodeDateRange).ToString());
        }
    }
}
