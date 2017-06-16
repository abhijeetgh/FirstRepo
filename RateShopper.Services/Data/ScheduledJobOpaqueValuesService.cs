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
    public class ScheduledJobOpaqueValuesService : BaseService<ScheduledJobOpaqueValues>, IScheduledJobOpaqueValuesService
    {
        public ScheduledJobOpaqueValuesService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScheduledJobOpaqueValues>();
            _cacheManager = cacheManager;
        }

        public void SaveOpaqueValues(List<ScheduledJobOpaqueValuesDTO> opaqueValues, long scheduledJobId)
        {
            List<ScheduledJobOpaqueValues> lstDeleteScheduledJobOpaqueValues = _context.ScheduledJobOpaqueValues.Where(obj => obj.ScheduledJobId == scheduledJobId).ToList();
            if (lstDeleteScheduledJobOpaqueValues.Count > 0)
            {
                foreach (var scheduledJobOpaqueValues in lstDeleteScheduledJobOpaqueValues)
                {
                    _context.Entry(scheduledJobOpaqueValues).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }

            ScheduledJobOpaqueValues scheduledJobOpaqueValuesEntity = null;
            if (opaqueValues != null && opaqueValues.Count > 0)
            {
                foreach (var opaqueRow in opaqueValues)
                {
                    scheduledJobOpaqueValuesEntity = new ScheduledJobOpaqueValues();
                    scheduledJobOpaqueValuesEntity.CarClassId = opaqueRow.CarClassId;
                    scheduledJobOpaqueValuesEntity.PercentValue = opaqueRow.PercenValue;
                    scheduledJobOpaqueValuesEntity.ScheduledJobId = scheduledJobId;

                    _context.ScheduledJobOpaqueValues.Add(scheduledJobOpaqueValuesEntity);
                }
                _context.SaveChanges();
            }

            _cacheManager.Remove(typeof(ScheduledJobOpaqueValues).ToString());
        }

        public void RemoveAllOpaqueValues(long scheduledJobId)
        {
            List<ScheduledJobOpaqueValues> lstDeleteScheduledJobOpaqueValues = _context.ScheduledJobOpaqueValues.Where(obj => obj.ScheduledJobId == scheduledJobId).ToList();
            if (lstDeleteScheduledJobOpaqueValues.Count >= 0)
            {
                foreach (var scheduledJobOpaqueValues in lstDeleteScheduledJobOpaqueValues)
                {
                    _context.Entry(scheduledJobOpaqueValues).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }
            _cacheManager.Remove(typeof(ScheduledJobOpaqueValues).ToString());
        }
    }
}
