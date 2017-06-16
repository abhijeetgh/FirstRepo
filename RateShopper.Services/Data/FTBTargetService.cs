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
using RateShopper.Domain.DTOs;
using System.Globalization;
using System.Data.SqlClient;

namespace RateShopper.Services.Data
{
    public class FTBTargetService : BaseService<FTBTarget>, IFTBTargetService
    {
        private IWeekDayService _weekDayService;
        private IFTBTargetsDetailService _ftbTargetsDetailService;
        public FTBTargetService(IEZRACRateShopperContext context, ICacheManager cacheManager, IWeekDayService weekDayService, IFTBTargetsDetailService ftbTargetsDetailService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<FTBTarget>();
            _cacheManager = cacheManager;
            _weekDayService = weekDayService;
            _ftbTargetsDetailService = ftbTargetsDetailService;
        }

        public FTBTargetDTO GetFTBTargetBy()
        {
            return new FTBTargetDTO();
        }
        public bool AddTargetDetails(List<FTBTargetDTO> ftbTargetDTO)
        {
            bool flag = false;
            if (ftbTargetDTO != null && ftbTargetDTO.Count() > 0)
            {
                DateTime date;
                date = DateTime.ParseExact(ftbTargetDTO[0].Date, "yyyy-M-d", CultureInfo.InvariantCulture);

                long locationBrandId = ftbTargetDTO[0].LocationBrandId;
                var ftbtarget = (from tr in _context.FTBTargets
                                 where tr.Date == date && tr.LocationBrandId == locationBrandId
                                 select tr).ToList();
                if (ftbtarget.Count() > 0)
                {
                    DeleteTarget(ftbtarget);//if data is already exist and new data could come for save then previous target entry need to delete 
                }

                foreach (var ftbTargetItem in ftbTargetDTO)
                {
                    FTBTarget ftbTarget = new FTBTarget();

                    ftbTarget.Date = DateTime.ParseExact(ftbTargetItem.Date, "yyyy-M-d", CultureInfo.InvariantCulture);
                    ftbTarget.LocationBrandId = ftbTargetItem.LocationBrandId;
                    ftbTarget.DayOfWeekId = ftbTargetItem.DayOfWeekId;
                    ftbTarget.Target = (ftbTargetItem.Target.HasValue) ? ftbTargetItem.Target : null;
                    ftbTarget.CreatedBy = ftbTargetItem.LoggedUserId;
                    ftbTarget.UpdatedBy = ftbTargetItem.LoggedUserId;
                    ftbTarget.CreatedDateTime = DateTime.Now;
                    ftbTarget.UpdatedDateTime = DateTime.Now;
                    _context.FTBTargets.Add(ftbTarget);
                    _context.SaveChanges();
                    _cacheManager.Remove(typeof(FTBTarget).ToString());

                    long ftbTargetID = ftbTarget.ID;

                    foreach (var ftbTargetsDetailItem in ftbTargetItem.FTBTargetsDetailDTOs)
                    {
                        if ((ftbTargetsDetailItem.PercentTarget != Convert.ToDecimal(0)) && (ftbTargetsDetailItem.PercentRateIncrease != Convert.ToDecimal(0)))
                        {
                            FTBTargetsDetail ftbTargetsDetail = new FTBTargetsDetail();
                            ftbTargetsDetail.TargetId = ftbTargetID;
                            ftbTargetsDetail.PercentTarget = (ftbTargetsDetailItem.PercentTarget.HasValue) ? ftbTargetsDetailItem.PercentTarget : null;
                            ftbTargetsDetail.PercentRateIncrease = (ftbTargetsDetailItem.PercentRateIncrease.HasValue) ? ftbTargetsDetailItem.PercentRateIncrease : null;
                            ftbTargetsDetail.SlotOrder = ftbTargetsDetailItem.SlotOrder;
                            _context.FTBTargetsDetails.Add(ftbTargetsDetail);
                        }
                    }

                    _context.SaveChanges();
                    _cacheManager.Remove(typeof(FTBTargetsDetail).ToString());
                    flag = true;
                }
            }
            return flag;
        }
        public bool DeleteTarget(List<FTBTarget> ftbTarget)
        {
            bool flag = false;
            foreach (var targetitem in ftbTarget)
            {
                List<FTBTargetsDetail> FTBTargetsDetail = _context.FTBTargetsDetails.Where(x => x.TargetId == targetitem.ID).ToList();
                foreach (var detailitem in FTBTargetsDetail)
                {
                    _context.Entry(detailitem).State = System.Data.Entity.EntityState.Deleted;
                }
                _context.Entry(targetitem).State = System.Data.Entity.EntityState.Deleted;
            }
            _context.SaveChanges();
            return flag;
        }

        public bool UpdateTargetDetails(List<FTBTargetDTO> ftbTargetDTO)
        {
            bool flag = false;
            if (ftbTargetDTO != null && ftbTargetDTO.Count() > 0)
            {
                foreach (var ftbTargetItem in ftbTargetDTO)
                {
                    FTBTarget ftbTarget = new FTBTarget();
                    ftbTarget = base.GetById(ftbTargetItem.FTBTargetId);
                    if (ftbTarget != null)
                    {
                        //ftbTarget.Date = ftbTargetItem.Date;
                        ftbTarget.LocationBrandId = ftbTargetItem.LocationBrandId;
                        ftbTarget.DayOfWeekId = ftbTargetItem.DayOfWeekId;
                        ftbTarget.Target = (ftbTargetItem.Target.HasValue) ? ftbTargetItem.Target : null;
                        ftbTarget.CreatedBy = ftbTargetItem.LoggedUserId;
                        ftbTarget.UpdatedBy = ftbTargetItem.LoggedUserId;
                        ftbTarget.CreatedDateTime = DateTime.Now;
                        ftbTarget.UpdatedDateTime = DateTime.Now;
                        //ftbTarget.FTBTargetsDetail = (ftbTargetItem.FTBTargetsDetailDTOs.Select(x => new FTBTargetsDetail()
                        //{
                        //    ID = x.FTBTargetDetailId,
                        //    TargetId = x.FTBTargetId,
                        //    PercentTarget = ((x.PercentTarget.HasValue) ? x.PercentTarget : null),
                        //    PercentRateIncrease = ((x.PercentRateIncrease.HasValue) ? x.PercentRateIncrease : null),
                        //    SlotOrder = x.SlotOrder
                        //}).ToList());

                        _context.Entry(ftbTarget).State = System.Data.Entity.EntityState.Modified;



                        foreach (var ftbTargetsDetailItem in ftbTargetItem.FTBTargetsDetailDTOs)
                        {

                            FTBTargetsDetail ftbTargetsDetail = _context.FTBTargetsDetails.Where(x => x.ID == ftbTargetsDetailItem.FTBTargetDetailId).SingleOrDefault();
                            ftbTargetsDetail.ID = ftbTargetsDetailItem.FTBTargetDetailId;
                            ftbTargetsDetail.TargetId = ftbTargetsDetailItem.FTBTargetId;
                            ftbTargetsDetail.PercentTarget = ((ftbTargetsDetailItem.PercentTarget.HasValue) ? ftbTargetsDetailItem.PercentTarget : null);
                            ftbTargetsDetail.PercentRateIncrease = ((ftbTargetsDetailItem.PercentRateIncrease.HasValue) ? ftbTargetsDetailItem.PercentRateIncrease : null);
                            ftbTargetsDetail.SlotOrder = ftbTargetsDetailItem.SlotOrder;
                            _context.Entry(ftbTargetsDetail).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
            }
            _context.SaveChanges();
            flag = true;
            _cacheManager.Remove(typeof(FTBTarget).ToString());
            _cacheManager.Remove(typeof(FTBTargetsDetail).ToString());
            return flag;
        }

        public async Task<List<FTBTargetDTO>> GetTargetDetails(long locationBrandId, long year, long month, bool isCopyFrom)
        {
            List<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();

            var FTBTargets = await (from target in _context.FTBTargets
                                    where target.LocationBrandId == locationBrandId && target.Date.Year == year && target.Date.Month == month
                                    select target).ToListAsync();

            var ftbTargetDetails = (from target in FTBTargets.ToList()
                                    join targetdetails in _context.FTBTargetsDetails on target.ID equals targetdetails.TargetId
                                    select targetdetails).ToList();

            List<WeekDaysDTO> lstWeekDay = _weekDayService.GetAll().Select(wd => new WeekDaysDTO
            {
                ID = wd.ID,
                Day = wd.Day
            }).ToList();


            //Mapped data in DTO
            IEnumerable<FTBTargetDTO> ftbTargetDTO = FTBTargets.Select(x => new FTBTargetDTO()
            {
                FTBTargetId = x.ID,
                LocationBrandId = x.LocationBrandId,
                Date = Convert.ToDateTime(x.Date).ToString("yyyy-MM-dd"),
                DayOfWeekId = x.DayOfWeekId,
                Day = lstWeekDay.Where(y => y.ID == x.DayOfWeekId).Select(y => y.Day).SingleOrDefault(),
                Target = x.Target,
                LoggedUserId = x.CreatedBy,
                IsUpdate = (isCopyFrom) ? false : true,
                FTBTargetsDetailDTOs = ftbTargetDetails.Where(y => y.TargetId == x.ID).Select(y => new FTBTargetsDetailDTO()
                {
                    FTBTargetDetailId = y.ID,
                    FTBTargetId = x.ID,
                    PercentTarget = y.PercentTarget,
                    PercentRateIncrease = y.PercentRateIncrease,
                    SlotOrder = y.SlotOrder
                }).OrderBy(y => y.SlotOrder).ToList()
            }).OrderBy(x => x.DayOfWeekId).ToList();


            return ftbTargetDTO.ToList();
        }

        public List<FTBTargetDTO> FetchTargetDetails(long locationBrandId, long year, long month)
        {
            List<FTBTargetDTO> FTBTargetDTOs = new List<FTBTargetDTO>();
            var FTBTargets = base.GetAll().Where(target => target.LocationBrandId == locationBrandId && target.Date.Year == year && target.Date.Month == month).ToList();

            var ftbTargetDetails = (from target in FTBTargets.ToList()
                                    join targetdetails in _ftbTargetsDetailService.GetAll() on target.ID equals targetdetails.TargetId
                                    select targetdetails).ToList();

            List<WeekDaysDTO> lstWeekDay = _weekDayService.GetAll().Select(wd => new WeekDaysDTO
            {
                ID = wd.ID,
                Day = wd.Day
            }).ToList();

            //Mapped data in DTO
            IEnumerable<FTBTargetDTO> ftbTargetDTO = FTBTargets.Select(x => new FTBTargetDTO()
            {
                FTBTargetId = x.ID,
                LocationBrandId = x.LocationBrandId,
                Date = Convert.ToDateTime(x.Date).ToString("yyyy-MM-dd"),
                DayOfWeekId = x.DayOfWeekId,
                Day = lstWeekDay.Where(y => y.ID == x.DayOfWeekId).Select(y => y.Day).SingleOrDefault(),
                Target = x.Target,
                LoggedUserId = x.CreatedBy,
                FTBTargetsDetailDTOs = ftbTargetDetails.Where(y => y.TargetId == x.ID).Select(y => new FTBTargetsDetailDTO()
                {
                    FTBTargetDetailId = y.ID,
                    FTBTargetId = x.ID,
                    PercentTarget = y.PercentTarget,
                    PercentRateIncrease = y.PercentRateIncrease,
                    SlotOrder = y.SlotOrder
                }).OrderBy(y => y.SlotOrder).ToList()
            }).OrderBy(x => x.DayOfWeekId).ToList();

            return ftbTargetDTO.ToList();
        }

        public async Task<int> CopyFTBTarget(FTBCopyMonthsDTO objFTBCopyMonthsDTO)
        {
            DateTime copyFromDate = new DateTime(objFTBCopyMonthsDTO.SourceYear, objFTBCopyMonthsDTO.SourceMonth, 1);
            DateTime copyToDate = new DateTime(objFTBCopyMonthsDTO.Year, objFTBCopyMonthsDTO.Month, 1);

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("LocationBrandId", objFTBCopyMonthsDTO.LocationBrandId));
            sqlParameters.Add(new SqlParameter("CopyFromDate", copyFromDate.Date));
            sqlParameters.Add(new SqlParameter("CopyToDate", copyToDate.Date));
            sqlParameters.Add(new SqlParameter("CreatedDateTime", DateTime.Now));

            string queryCopyFTBTargets = "EXEC CopyFTBTarget ";
            queryCopyFTBTargets = string.Concat(queryCopyFTBTargets, string.Join(", ", sqlParameters.Select(d => "@" + d.ParameterName)));
            var results = await _context.ExecuteSQLQuery<int>(queryCopyFTBTargets, sqlParameters.ToArray());
            _cacheManager.Remove(typeof(FTBTarget).ToString());
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
    }
}
