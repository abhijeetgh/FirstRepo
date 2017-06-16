using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateShopper.Services.Data
{
    public class RateCodeService : BaseService<RateCode>, IRateCodeService
    {

        IRateCodeDateRangeService _rateCodeDateRangeService;

        public RateCodeService(IEZRACRateShopperContext context, ICacheManager cacheManager, IRateCodeDateRangeService _rateCodeDateRangeService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RateCode>();
            _cacheManager = cacheManager;
            this._rateCodeDateRangeService = _rateCodeDateRangeService;
        }


        public List<RateCodeDTO> GetAllRateCodes()
        {
            List<RateCodeDTO> rateCodes = null;
            if (_context.RateCodes != null && _context.RateCodes.Count() > 0)
            {
                rateCodes = _context.RateCodes.Where(code => !code.IsDeleted).Select(code => new RateCodeDTO { ID = code.ID, Code = code.Code, Description = code.Description, Name = code.Name, CreatedBy = code.CreatedBy, SupportedBrandIDs = code.SupportedBrandIDs,IsActive = code.IsActive })
                    .ToList<RateCodeDTO>();
            }
            return rateCodes;
        }

        public RateCodeDTO GetRateCodeDetails(long rateCodeID)
        {
            //long RateCodeID = rateCodeID;
            List<RateCodeDateRangeDTO> DateRanges = _rateCodeDateRangeService.GetRateCodeDateRangeDetails(rateCodeID);

            RateCodeDTO rateCode = null;
            if (_context.RateCodes != null && _context.RateCodes.Count() > 0 && rateCodeID > 0)
            {
                rateCode = (from rate in _context.RateCodes where rate.ID == rateCodeID select new RateCodeDTO { ID = rate.ID, Code = rate.Code, Description = rate.Description, Name = rate.Name, CreatedBy = rate.CreatedBy, SupportedBrandIDs = rate.SupportedBrandIDs, IsActive = rate.IsActive}).FirstOrDefault();
                rateCode.DateRangeList = DateRanges;
            }
            return rateCode;
        }

        public long SaveRateCode(RateCodeDTO objRateCodeDTO)
        {
            if (objRateCodeDTO != null && objRateCodeDTO.ID == 0)
            {
                //New Rate Code Insertion                
                RateCode objExistingRateCode = base.GetAll() != null ? base.GetAll().Where(rate => rate.Code.Equals(objRateCodeDTO.Code) && !rate.IsDeleted).FirstOrDefault() : null;
                if (objExistingRateCode == null)
                {
                    RateCode newRateCode = new RateCode()
                    {
                        Code = objRateCodeDTO.Code,
                        Description = objRateCodeDTO.Description,
                        Name = objRateCodeDTO.Name,
                        CreatedBy = objRateCodeDTO.CreatedBy,
                        UpdatedBy = objRateCodeDTO.CreatedBy,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now,
                        SupportedBrandIDs = objRateCodeDTO.SupportedBrandIDs,
                        IsActive = objRateCodeDTO.IsActive
                    };
                    base.Add(newRateCode);

                    if (objRateCodeDTO.DeletedDateRangeIds != null && objRateCodeDTO.DeletedDateRangeIds.Count > 0)
                    {
                        _rateCodeDateRangeService.DeleteRateCodeDateRange(objRateCodeDTO.DeletedDateRangeIds);
                    }
                    if (objRateCodeDTO.DateRangeList !=null && objRateCodeDTO.DateRangeList.Count > 0)
                    {
                        _rateCodeDateRangeService.SaveRateCodeDateRange(objRateCodeDTO.DateRangeList, newRateCode.ID);
                    }
                    
                    return newRateCode.ID;
                }
                else
                {
                    return objExistingRateCode.ID;
                }
            }
            else
            {
                //check duplicate
                if (base.GetAll().Where(rate => rate.Code.Equals(objRateCodeDTO.Code) && !rate.IsDeleted && rate.ID != objRateCodeDTO.ID).FirstOrDefault() == null)
                {
                    //Update existing rate code
                    RateCode existingRateCode = base.GetAll().Where(rate => rate.ID == objRateCodeDTO.ID && !rate.IsDeleted).FirstOrDefault();
                    if (existingRateCode != null)
                    {
                        existingRateCode.Code = objRateCodeDTO.Code;
                        existingRateCode.Name = objRateCodeDTO.Name;
                        existingRateCode.Description = objRateCodeDTO.Description;
                        existingRateCode.UpdatedBy = objRateCodeDTO.CreatedBy;
                        existingRateCode.UpdatedDateTime = DateTime.Now;
                        existingRateCode.SupportedBrandIDs = objRateCodeDTO.SupportedBrandIDs;
                        existingRateCode.IsActive = objRateCodeDTO.IsActive;
                        base.Update(existingRateCode);

                        if (objRateCodeDTO.DeletedDateRangeIds != null && objRateCodeDTO.DeletedDateRangeIds.Count > 0)
                        {
                            _rateCodeDateRangeService.DeleteRateCodeDateRange(objRateCodeDTO.DeletedDateRangeIds);
                        }
                        if (objRateCodeDTO.DateRangeList != null && objRateCodeDTO.DateRangeList.Count > 0)
                        {
                            _rateCodeDateRangeService.SaveRateCodeDateRange(objRateCodeDTO.DateRangeList, existingRateCode.ID);
                        }
                        
                        return existingRateCode.ID;
                    }                    
                }
            }
            return 0;
        }


        public bool DeleteRateCode(long rateCodeID, long userID)
        {
            RateCode rateCode = GetById(rateCodeID);
            if (rateCode != null)
            {
                rateCode.IsDeleted = true;
                rateCode.UpdatedDateTime = DateTime.Now;
                rateCode.UpdatedBy = userID;
                base.Update(rateCode);
                return true;
            }
            return false;
        }

        public List<RateCodeDTO> GetRateCodesWithDateRanges()
        {            
            var query = (from rc in base.GetAll().Where(d => !d.IsDeleted && d.IsActive)
                         join rcdr in _rateCodeDateRangeService.GetAll() on rc.ID equals rcdr.RateCodeID
                         into a
                         from b in a.DefaultIfEmpty()
                         select new
                         {
                             Id = b != null ? b.ID : 0,
                             CodeId = rc.ID,
                             Code = rc.Code,
                             SupportedBrandIds = rc.SupportedBrandIDs,
                             StartDate = b != null ? b.StartDate : new DateTime(1900,01,01),
                             EndDate = b != null ? b.EndDate : new DateTime(1900,01,01)
                         });


            List<RateCodeDTO> lstRateCodes = query.GroupBy(d => new { d.Code, d.SupportedBrandIds, d.CodeId })
                .Select(d => new RateCodeDTO
                {
                    Code = d.Key.Code,
                    ID = d.Key.CodeId,
                    SupportedBrandIDs = d.Key.SupportedBrandIds,
                    DateRangeList = d.Select(e => e.Id > 0 ? new RateCodeDateRangeDTO { ID = e.Id, StartDate = e.StartDate, EndDate = e.EndDate, RateCodeID = e.CodeId } : null).ToList()
                }).ToList();

            lstRateCodes.ForEach(d => { if (d.DateRangeList[0] == null) { d.DateRangeList.RemoveAt(0); } });            
            return lstRateCodes;
        }

        public List<RateCodeDTO> GetApplicableRateCodesBetweenDateRange(DateTime startDate, DateTime endDate)
        {
            var query = (from rc in base.GetAll().Where(d => !d.IsDeleted && d.IsActive)
                       join rcdr in _rateCodeDateRangeService.GetAll() on rc.ID equals rcdr.RateCodeID
                         where (rcdr.StartDate <= startDate && rcdr.EndDate >= endDate) || (rcdr.StartDate >= startDate && rcdr.EndDate <= endDate) || (rcdr.StartDate <= startDate && rcdr.EndDate >= startDate) || (rcdr.StartDate <= endDate && rcdr.EndDate >= endDate)
                       select new
                       {
                           Id = rcdr.ID,
                           CodeId = rc.ID,
                           Code = rc.Code,
                           SupportedBrandIds = rc.SupportedBrandIDs,
                           StartDate = rcdr.StartDate,
                           EndDate = rcdr.EndDate
                       });

            List<RateCodeDTO> lstRateCodes = query.GroupBy(d => new { d.Code, d.SupportedBrandIds, d.CodeId })
                .Select(d => new RateCodeDTO
                {
                    Code = d.Key.Code,
                    ID = d.Key.CodeId,
                    SupportedBrandIDs = d.Key.SupportedBrandIds,
                    DateRangeList = d.Select(e => new RateCodeDateRangeDTO { ID = e.Id, StartDate = e.StartDate, EndDate = e.EndDate, RateCodeID = d.Key.CodeId }).ToList()
                }).ToList();

            foreach(var objRateCode in lstRateCodes)
            {
                List<RateCodeDateRangeDTO> lstDateRanges = objRateCode.DateRangeList.Where(e => e.StartDate <= startDate && e.EndDate >= endDate).ToList();
                if (lstDateRanges.Count > 0)
                {
                    objRateCode.DateRangeList = lstDateRanges.Take(1).ToList();
                    continue;
                }
                lstDateRanges = objRateCode.DateRangeList.Where(e => e.StartDate <= startDate && e.EndDate >= startDate).Take(1).ToList();
                var rangeBetweenStartAndEndDate = objRateCode.DateRangeList.Where(e => e.StartDate > startDate && e.EndDate < endDate);
                if (rangeBetweenStartAndEndDate.Count()>0)
                {
                    lstDateRanges.AddRange(rangeBetweenStartAndEndDate);
                }

                var endDateRange = objRateCode.DateRangeList.Where(e => e.StartDate <= endDate && e.EndDate >= endDate).FirstOrDefault();
                if (endDateRange != null)
                {
                    lstDateRanges.Add(endDateRange);
                }
                objRateCode.DateRangeList = lstDateRanges;
            }

            return lstRateCodes;
        }
    }
}

