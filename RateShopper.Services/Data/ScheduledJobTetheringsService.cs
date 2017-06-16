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
    public class ScheduledJobTetheringsService : BaseService<ScheduledJobTetherings>, IScheduledJobTetheringsService
    {
        public ScheduledJobTetheringsService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScheduledJobTetherings>();
            _cacheManager = cacheManager;
        }
        public string SaveScheduledJobTethering(ScheduledJobTetheringsDTO scheduledJobTetherings, long scheduledJobID)
        {
            string Result = "";
            bool flag = false;
            if (scheduledJobTetherings != null)
            {
                // ScheduledJobTetherings ScheduledJobTether = scheduledJobTetherings.GroupBy(obj => obj.ScheduleJobID).Select(grp => grp.OrderBy(item => item.ScheduleJobID).FirstOrDefault()).FirstOrDefault();

                //Update Automation console tether setting
                if (scheduledJobTetherings.ScheduleJobID > 0)
                {

                    List<ScheduledJobTetherings> CheckSJTethering = _context.ScheduledJobTetherings.Where(obj => obj.ScheduleJobID == scheduledJobID).ToList<ScheduledJobTetherings>();
                    if (CheckSJTethering.Count() > 0)
                    {
                        ScheduledJobTetherings GetLocationBrandID = CheckSJTethering.GroupBy(obj => obj.ScheduleJobID).Select(grp => grp.OrderBy(item => item.ScheduleJobID).FirstOrDefault()).FirstOrDefault<ScheduledJobTetherings>();

                        if (scheduledJobTetherings.LocationBrandID == GetLocationBrandID.LocationBrandID && scheduledJobTetherings.lstScheduledJobTetherCarClass.Count() >= CheckSJTethering.Count())
                        {
                            foreach (var SJTetherData in scheduledJobTetherings.lstScheduledJobTetherCarClass)
                            {
                                bool IsPercentage = false;
                                if (SJTetherData.IsTetherValueinPercentage)
                                {
                                    IsPercentage = true;
                                }

                                ScheduledJobTetherings CheckCarClass = new ScheduledJobTetherings();
                                CheckCarClass = CheckSJTethering.Where(obj => obj.CarClassID == SJTetherData.CarClassID && obj.ScheduleJobID == scheduledJobID).FirstOrDefault();
                                if (CheckCarClass != null)
                                {
                                    //for update all  the data
                                    CheckCarClass.TetherValue = SJTetherData.TetherValue;

                                    CheckCarClass.IsTetherValueinPercentage = IsPercentage;

                                    _context.Entry(CheckCarClass).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    ScheduledJobTetherings sjTether = new ScheduledJobTetherings();
                                    sjTether.ScheduleJobID = scheduledJobID;
                                    sjTether.LocationBrandID = scheduledJobTetherings.LocationBrandID;
                                    sjTether.DependantBrandID = scheduledJobTetherings.DependantBrandID;
                                    sjTether.DominentBrandID = scheduledJobTetherings.DominentBrandID;
                                    sjTether.CarClassID = SJTetherData.CarClassID;
                                    sjTether.TetherValue = SJTetherData.TetherValue;
                                    sjTether.IsTetherValueinPercentage = IsPercentage;
                                    _context.ScheduledJobTetherings.Add(sjTether);
                                }
                            }
                            //Final database save and update operation
                            _context.SaveChanges();
                            flag = true;
                            _cacheManager.Remove(typeof(ScheduledJobTetherings).ToString());
                        }
                        else
                        {
                            //If location can be changed
                            if (CheckSJTethering != null)
                            {
                                foreach (var SJTetherData in CheckSJTethering)
                                {
                                    _context.Entry(SJTetherData).State = System.Data.Entity.EntityState.Deleted;
                                }
                                _context.SaveChanges();
                                //Remove cache if incase used same cache on this table.
                                _cacheManager.Remove(typeof(ScheduledJobTetherings).ToString());

                                //Created new tether data if location will be changed.
                                flag = InsertScheduleJobTetheringData(scheduledJobTetherings, scheduledJobID);
                            }
                        }
                    }
                    else
                    {
                        //Note: If not set job agains tether settings
                        flag = InsertScheduleJobTetheringData(scheduledJobTetherings, scheduledJobID);
                    }
                }
                else
                {
                    //New 
                    flag = InsertScheduleJobTetheringData(scheduledJobTetherings, scheduledJobID);
                }
            }
            if (flag)
                Result = "Success";
            else
                Result = "Fail";

            return Result;
        }
        public ScheduledJobTetheringsDTO GetSelectedJobTetheringData(long ScheduledJobID)
        {
            ScheduledJobTetheringsDTO scheduledJobTetherings = new ScheduledJobTetheringsDTO();
            if (ScheduledJobID > 0)
            {
                List<ScheduledJobTetherings> SJTethers = _context.ScheduledJobTetherings.Where(obj => obj.ScheduleJobID == ScheduledJobID).ToList();
                scheduledJobTetherings.lstScheduledJobTetherCarClass = new List<ScheduledJobTetherCarClassDTO>();
                foreach (var SJTetherSetting in SJTethers)
                {
                    ScheduledJobTetherCarClassDTO scheduledJobTetherCarClassDTO = new ScheduledJobTetherCarClassDTO();

                    scheduledJobTetherings.LocationBrandID = SJTetherSetting.LocationBrandID;
                    scheduledJobTetherings.DominentBrandID = SJTetherSetting.DominentBrandID;
                    scheduledJobTetherings.DependantBrandID = SJTetherSetting.DependantBrandID;

                    scheduledJobTetherCarClassDTO.ID = SJTetherSetting.ID;
                    scheduledJobTetherCarClassDTO.CarClassID = SJTetherSetting.CarClassID;
                    scheduledJobTetherCarClassDTO.TetherValue = SJTetherSetting.TetherValue;
                    scheduledJobTetherCarClassDTO.IsTetherValueinPercentage = SJTetherSetting.IsTetherValueinPercentage;

                    scheduledJobTetherings.lstScheduledJobTetherCarClass.Add(scheduledJobTetherCarClassDTO);
                }
            }
            return scheduledJobTetherings;
        }
        public bool InsertScheduleJobTetheringData(ScheduledJobTetheringsDTO scheduledJobTetherings, long scheduledJobID)
        {
            bool flag = false;
            if (scheduledJobTetherings.lstScheduledJobTetherCarClass.Count() > 0)
            {
                foreach (var scheduledTether in scheduledJobTetherings.lstScheduledJobTetherCarClass)
                {
                    ScheduledJobTetherings sjTether = new ScheduledJobTetherings();
                    sjTether.ScheduleJobID = scheduledJobID;
                    sjTether.LocationBrandID = scheduledJobTetherings.LocationBrandID;
                    sjTether.DominentBrandID = scheduledJobTetherings.DominentBrandID;
                    sjTether.DependantBrandID = scheduledJobTetherings.DependantBrandID;
                    sjTether.CarClassID = scheduledTether.CarClassID;
                    sjTether.TetherValue = scheduledTether.TetherValue;
                    if (scheduledTether.IsTetherValueinPercentage)
                    {
                        sjTether.IsTetherValueinPercentage = true;
                    }
                    else
                    {
                        sjTether.IsTetherValueinPercentage = false;
                    }
                    _context.ScheduledJobTetherings.Add(sjTether);
                }
                _context.SaveChanges();
                flag = true;
                _cacheManager.Remove(typeof(ScheduledJobTetherings).ToString());
            }
            return flag;
        }
        public bool DeleteScheduleJobTetheringData(long scheduledJobID)
        {
            bool flag = false;
            List<ScheduledJobTetherings> SJTethering = _context.ScheduledJobTetherings.Where(obj => obj.ScheduleJobID == scheduledJobID).ToList<ScheduledJobTetherings>();
            foreach (var SJTetherData in SJTethering)
            {
                _context.Entry(SJTetherData).State = System.Data.Entity.EntityState.Deleted;
            }
            _context.SaveChanges();
            flag = true;
            //Remove cache if incase used same cache on this table.
            _cacheManager.Remove(typeof(ScheduledJobTetherings).ToString());

            return flag;
        }
    }
}
