using RateShopper.Services.Data;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using System.Data.Entity;
using RateShopper.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class StatusesService : BaseService<Statuses>, IStatusesService
    {
        public StatusesService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<Statuses>();
            _cacheManager = cacheManager;
        }
        public long GetStatusIDByName(string status)
        {
            try
            {
                return base.GetAll(false).FirstOrDefault(obj => obj.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ID;
            }
            catch (Exception ex)
            {
                RateShopper.Services.Data.SearchResultsService.Logger.WriteToLogFile("Exception Occured in Status Service GetStatusIDByName(),Inner Exception: " + ex.InnerException
                   + ", Stack Trace: " + ex.StackTrace + " exception Message " + ex.Message, RateShopper.Services.Data.SearchResultsService.Logger.GetLogFilePath());

                Dictionary<string, long> statusList = new Dictionary<string, long> { 
                    {"pending",1},
                    {"request sent to scrapper",2},
                    {"data received from scrapper",3},
                    {"process complete",4},
                    {"failed",5},
                    {"deleted",6}
                };
                return statusList.ContainsKey(status.ToLower()) ? statusList[status] : 1;
            }
        }
    }
}
