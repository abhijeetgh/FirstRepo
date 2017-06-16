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
    public class GlobalLimitService : BaseService<GlobalLimit>, IGlobalLimitService
    {
        public GlobalLimitService(IEZRACRateShopperContext context, ICacheManager cacheManager, IUserRolesService _userRolesService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<GlobalLimit>();
            _cacheManager = cacheManager;
        }

        public List<LocationBrandModel> GetLocationBrands()
        {
            IQueryable<LocationBrandModel> locationBrands = _context.LocationBrands
                .Join(_context.Locations, Brand => Brand.LocationID, Location => Location.ID, (Brand, Location)
                 => new { LocationID = Brand.LocationID, LocationBrandAlias = Brand.LocationBrandAlias, ID = Brand.ID, LocationCode = Location.Code, LocationIsDeleted = Location.IsDeleted, LocationBrandIsDeleted = Brand.IsDeleted }).Where(a => !a.LocationIsDeleted && !a.LocationBrandIsDeleted)
         .Select(obj => new LocationBrandModel { LocationID = obj.LocationID, LocationBrandAlias = obj.LocationBrandAlias, ID = obj.ID, LocationCode = obj.LocationCode }
                   ).OrderBy(a => a.LocationBrandAlias);

            return locationBrands.ToList<LocationBrandModel>();
        }

        public List<GlobalLimitDTO> GetGlobalLimitDetails(long brandLocationID)
        {
            List<GlobalLimitDetailsDTO> globalDetailResults = (from globalLimit in _context.GlobalLimits
                                                               join globalDetails in _context.GlobalLimitDetails on globalLimit.ID equals globalDetails.GlobalLimitID
                                                               into global
                                                               from detail in global.DefaultIfEmpty()
                                                               where globalLimit.LocationBrandID == brandLocationID
                                                               select new GlobalLimitDetailsDTO
                                                               {
                                                                   StartDate = globalLimit.StartDate,
                                                                   EndDate = globalLimit.EndDate,
                                                                   CarClassID = detail.CarClassID,
                                                                   DayMax = detail.DayMax,
                                                                   DayMin = detail.DayMin,
                                                                   WeekMax = detail.WeekMax,
                                                                   WeekMin = detail.WeekMin,
                                                                   MonthlyMax = detail.MonthMax,
                                                                   MonthlyMin = detail.MonthMin,
                                                                   BrandLocation = brandLocationID,
                                                                   GlobalDetailsID = detail.ID,
                                                                   GlobalLimitID = globalLimit.ID
                                                               }).ToList();

            List<GlobalLimitDetailsDTO> carClassResult = (from locationBrandCarClasses in _context.LocationBrandCarClass
                                                          join carClasses in _context.CarClasses.Where(d => !d.IsDeleted) on locationBrandCarClasses.CarClassID equals carClasses.ID
                                                          where locationBrandCarClasses.LocationBrandID == brandLocationID
                                                          orderby carClasses.DisplayOrder
                                                          select new GlobalLimitDetailsDTO
                                                          {
                                                              CarClass = carClasses.Code,
                                                              CarClassID = carClasses.ID,
                                                              GlobalDetailsID = 0
                                                          }).ToList();


            List<GlobalLimitDTO> lstGlobalLimits = new List<GlobalLimitDTO>();

            if (globalDetailResults.Count > 0)
            {
                List<long?> lstGlobalLimitIds = globalDetailResults.Where(d => d.GlobalLimitID > 0).Select(d => d.GlobalLimitID).Distinct().ToList();

                foreach (long? globalLimitId in lstGlobalLimitIds)
                {
                    GlobalLimitDTO objGlobalLimitDTO = new GlobalLimitDTO();
                    GlobalLimitDetailsDTO objGlobalLimitDetailsDTO = globalDetailResults.Find(d => d.GlobalLimitID == globalLimitId);
                    if (objGlobalLimitDetailsDTO != null)
                    {
                        objGlobalLimitDTO.StartDate = objGlobalLimitDetailsDTO.StartDate.HasValue ? objGlobalLimitDetailsDTO.StartDate.Value.ToString("MM/dd/yyyy") : string.Empty;
                        objGlobalLimitDTO.EndDate = objGlobalLimitDetailsDTO.EndDate.HasValue ? objGlobalLimitDetailsDTO.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty;
                        objGlobalLimitDTO.GlobalLimitID = objGlobalLimitDetailsDTO.GlobalLimitID;

                        objGlobalLimitDTO.LstGlobalLimitDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GlobalLimitDetailsDTO>>(Newtonsoft.Json.JsonConvert.SerializeObject(carClassResult));

                        objGlobalLimitDTO.LstGlobalLimitDetails.ForEach(d =>
                        {
                            GlobalLimitDetailsDTO obj = globalDetailResults.Find(g => g.CarClassID == d.CarClassID && g.GlobalLimitID == globalLimitId);
                            if (obj != null)
                            {
                                d.DayMin = obj.DayMin;
                                d.DayMax = obj.DayMax;
                                d.MonthlyMax = obj.MonthlyMax;
                                d.MonthlyMin = obj.MonthlyMin;
                                d.WeekMax = obj.WeekMax;
                                d.WeekMin = obj.WeekMin;
                                d.GlobalDetailsID = obj.GlobalDetailsID;
                            }
                        });

                        lstGlobalLimits.Add(objGlobalLimitDTO);
                    }
                }
            }
            GlobalLimitDTO objEmptyObject = new GlobalLimitDTO();
            objEmptyObject.GlobalLimitID = 0;
            objEmptyObject.LstGlobalLimitDetails = carClassResult;

            lstGlobalLimits.Add(objEmptyObject);

            return lstGlobalLimits;
        }

        public bool DeleteGlobalLimit(long globalLimitID)
        {
            GlobalLimit objGlobalLimitEntity = _context.GlobalLimits.Find(globalLimitID);
            if (objGlobalLimitEntity != null)
            {
                Delete(objGlobalLimitEntity);
                return true;
            }
            return false;
        }

        public long SaveGlobalLimit(GlobalLimitDTO objGlobalLimitDTO)
        {
            if (objGlobalLimitDTO.GlobalLimitID == 0)
            {
                DateTime StartDate = Convert.ToDateTime(objGlobalLimitDTO.StartDate);
                DateTime EndDate = Convert.ToDateTime(objGlobalLimitDTO.EndDate);

                if (!_context.GlobalLimits.Any(d => d.LocationBrandID == objGlobalLimitDTO.BrandLocation && d.StartDate == StartDate && d.EndDate == EndDate))
                {
                    GlobalLimit objGlobalLimitEntity = new GlobalLimit();
                    objGlobalLimitEntity.StartDate = StartDate;
                    objGlobalLimitEntity.EndDate = EndDate;
                    objGlobalLimitEntity.CreatedBy = objGlobalLimitDTO.CreatedBy;
                    objGlobalLimitEntity.CreatedDateTime = DateTime.Now;
                    objGlobalLimitEntity.UpdatedBy = objGlobalLimitDTO.CreatedBy;
                    objGlobalLimitEntity.UpdatedDateTime = DateTime.Now;
                    objGlobalLimitEntity.LocationBrandID = objGlobalLimitDTO.BrandLocation;

                    Add(objGlobalLimitEntity);
                    return objGlobalLimitEntity.ID;
                }
                return 0;
            }
            else
            {
                GlobalLimit objGlobalLimitEntity = _context.GlobalLimits.Find(objGlobalLimitDTO.GlobalLimitID);
                if (objGlobalLimitEntity != null)
                {
                    objGlobalLimitEntity.UpdatedBy = objGlobalLimitDTO.CreatedBy;
                    objGlobalLimitEntity.UpdatedDateTime = DateTime.Now;
                    Update(objGlobalLimitEntity);
                }

                return objGlobalLimitEntity.ID;
            }
        }
    }
}
