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
    public class ScheduledJobMinRatesService : BaseService<ScheduledJobMinRates>, IScheduledJobMinRatesService
    {

        public ScheduledJobMinRatesService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScheduledJobMinRates>();
            _cacheManager = cacheManager;
        }

        public List<MinRatesDTO> preLoadMinRates(long[] carClassIds, string locationBrandId, long selectedJobId)
        {
            List<MinRatesDTO> minRatesDTO = new List<MinRatesDTO>();

            List<MinRatesDTO> minRatesResult = new List<MinRatesDTO>();
            long scheduledJobID;
            long[] locationBrands;
            if (carClassIds.Length > 0 && !string.IsNullOrEmpty(locationBrandId))
            {
                locationBrands = locationBrandId.Split(',').Select(a => Convert.ToInt64(a)).ToArray();


                var scheduledJobs = _context.ScheduledJobs.Where(job => !job.IsDeleted && job.ID != selectedJobId).Select(job => new
                {
                    locationBrandIds = job.LocationBrandIDs,
                    CreatedDateTime = job.CreatedDateTime,
                    ScheduledJobId = job.ID
                }).ToList().Select(job => new
                {
                    locationBrandIds = job.locationBrandIds.Split(',').Select(a => Convert.ToInt64(a)).ToArray(),
                    CreatedDateTime = job.CreatedDateTime,
                    ScheduledJobId = job.ScheduledJobId
                });

                var latestScheduledJob = scheduledJobs.Where(job => job.locationBrandIds.Intersect(locationBrands).Any())
                    .OrderByDescending(job => job.CreatedDateTime).FirstOrDefault();
                if (latestScheduledJob != null)
                {
                    scheduledJobID = latestScheduledJob.ScheduledJobId;
                    //get all min rates for scheduledJobID
                    minRatesDTO = _context.ScheduledJobMinRates.Where(rates => rates.ScheduleJobID == scheduledJobID)
                        .Select(rates => new MinRatesDTO
                        {
                            CarClassId = rates.CarClassID,
                            DayMinRate = rates.DayMin,
                            WeekMin = rates.WeekMin,
                            MonthMin = rates.MonthMin,
                            DayMax = rates.DayMax,
                            WeekMax = rates.WeekMax,
                            MonthMax = rates.MonthMax,
                            Day2Min = rates.Day2Min,
                            Day2Max = rates.Day2Max,
                            Week2Min = rates.Week2Min,
                            Week2Max = rates.Week2Max,
                            Month2Min = rates.Month2Min,
                            Month2Max = rates.Month2Max,
                            Days1 = rates.Days1,
                            Days2 = rates.Days2
                        }).ToList<MinRatesDTO>();

                    if (minRatesDTO.Count > 0)
                    {
                        minRatesResult = minRatesDTO.Join(carClassIds.Select(a => a), rate => rate.CarClassId, cars => cars, (rate, cars) => new MinRatesDTO
                        {
                            CarClassId = rate.CarClassId,
                            DayMinRate = rate.DayMinRate,
                            WeekMin = rate.WeekMin,
                            MonthMin = rate.MonthMin,
                            DayMax = rate.DayMax,
                            WeekMax = rate.WeekMax,
                            MonthMax = rate.MonthMax,
                            Day2Min = rate.Day2Min,
                            Day2Max = rate.Day2Max,
                            Week2Min = rate.Week2Min,
                            Week2Max = rate.Week2Max,
                            Month2Min = rate.Month2Min,
                            Month2Max = rate.Month2Max,
                            Days1 = rate.Days1,
                            Days2 = rate.Days2
                        }).ToList<MinRatesDTO>();
                    }
                    else
                        minRatesResult = null;
                }
                else
                    minRatesResult = null;
            }
            else
                minRatesResult = null;

            return minRatesResult;
        }
        public string SaveScheduledJobMinRates(List<ScheduledJobMinRates> lstScheduledJobMinRates, long ScheduleJobID)
        {
            //_context.ScheduledJobMinRates.RemoveRange(_context.ScheduledJobMinRates.Where(obj => obj.ScheduleJobID == ScheduleJobID));
            //List<ScheduledJobMinRates> lstDeleteScheduledJobMinRates = base.GetAll().Where(obj => obj.ScheduleJobID == ScheduleJobID).ToList();
            List<ScheduledJobMinRates> lstDeleteScheduledJobMinRates = _context.ScheduledJobMinRates.Where(obj => obj.ScheduleJobID == ScheduleJobID).ToList();
            if (lstDeleteScheduledJobMinRates.Count() != 0 && lstDeleteScheduledJobMinRates != null)
            {
                foreach (var scheduledJobMinRates in lstDeleteScheduledJobMinRates)
                {
                    _context.Entry(scheduledJobMinRates).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.SaveChanges();
            }

            if (lstScheduledJobMinRates != null)
            {


                foreach (var scheduledJobMinRates in lstScheduledJobMinRates)
                {
                    scheduledJobMinRates.ScheduleJobID = ScheduleJobID;
                    _context.ScheduledJobMinRates.Add(scheduledJobMinRates);
                }
                _context.SaveChanges();
            }
            return "success";
        }

        public List<ScheduledJobMinRates> GetSelectedMinRate(long ScheduleJobID)
        {
            List<ScheduledJobMinRates> ScheduledJobMinRates = new List<ScheduledJobMinRates>();
            if (ScheduleJobID != 0)
            {
                ScheduledJobMinRates = _context.ScheduledJobMinRates.Where(obj => obj.ScheduleJobID == ScheduleJobID).ToList();
            }
            return ScheduledJobMinRates;
        }
    }
}
