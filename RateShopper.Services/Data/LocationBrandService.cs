using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateShopper.Services.Data
{
    public class LocationBrandService : BaseService<LocationBrand>, ILocationBrandService
    {
        IRuleSetService _ruleSetService;
        public LocationBrandService(IEZRACRateShopperContext context, ICacheManager cacheManager, IRuleSetService ruleSetService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<LocationBrand>();
            _cacheManager = cacheManager;
            this._ruleSetService = ruleSetService;
        }

        /// <summary>
        /// Save location brand details
        /// </summary>
        /// <param name="objLocationDTO"></param>
        /// <returns></returns>
        public long SaveLocationBrand(LocationDTO objLocationDTO)
        {
            if (objLocationDTO.LocationBrandID == 0)
            {
                LocationBrand objLocationBrandEntity = new LocationBrand()
                {
                    ID = objLocationDTO.LocationBrandID,
                    LocationID = objLocationDTO.ID,
                    BrandID = objLocationDTO.BrandID,
                    WeeklyExtraDenom = objLocationDTO.WeeklyExtraDenominator,
                    DailyExtraDayFactor = objLocationDTO.DailyExtraDayFactor,
                    TSDCustomerNumber = objLocationDTO.TSDCustomerNumber,
                    TSDPassCode = objLocationDTO.TSDPassCode,
                    Description = objLocationDTO.Description,
                    CreatedBy = objLocationDTO.CreatedBy,
                    UpdatedBy = objLocationDTO.CreatedBy,
                    CreatedDateTime = DateTime.Now,
                    UpdatedDateTime = DateTime.Now,
                    LocationBrandAlias = objLocationDTO.LocationBrandAlias,
                    UseLORRateCode = objLocationDTO.UseLORRateCode,
                    BranchCode = objLocationDTO.BranchCode,
                    CompetitorCompanyIDs = objLocationDTO.CompetitorCompanyIds == null ? " " : objLocationDTO.CompetitorCompanyIds,
                    QuickViewCompetitorCompanyIds = objLocationDTO.QuickViewCompetitors == null ? " " : objLocationDTO.QuickViewCompetitors
                };
                if (GetAll().Where(obj => !obj.IsDeleted && obj.LocationBrandAlias == objLocationDTO.LocationBrandAlias).FirstOrDefault() == null)
                {
                    Add(objLocationBrandEntity);
                }
                return objLocationBrandEntity.ID;
            }
            else
            {
                if (GetAll().Where(obj => obj.LocationBrandAlias == objLocationDTO.LocationBrandAlias && !obj.IsDeleted && obj.ID != objLocationDTO.LocationBrandID).FirstOrDefault() == null)
                {
                    LocationBrand objLocationBrandEntity = _context.LocationBrands.Where(obj => obj.ID == objLocationDTO.LocationBrandID).FirstOrDefault();//GetById(objLocationDTO.LocationBrandID);
                    if (objLocationBrandEntity != null)
                    {
                        objLocationBrandEntity.WeeklyExtraDenom = objLocationDTO.WeeklyExtraDenominator;
                        objLocationBrandEntity.DailyExtraDayFactor = objLocationDTO.DailyExtraDayFactor;
                        objLocationBrandEntity.TSDCustomerNumber = objLocationDTO.TSDCustomerNumber;
                        objLocationBrandEntity.TSDPassCode = objLocationDTO.TSDPassCode;
                        objLocationBrandEntity.Description = objLocationDTO.Description;
                        objLocationBrandEntity.UpdatedBy = objLocationDTO.CreatedBy;
                        objLocationBrandEntity.UpdatedDateTime = DateTime.Now;
                        objLocationBrandEntity.UseLORRateCode = objLocationDTO.UseLORRateCode;
                        objLocationBrandEntity.BrandID = objLocationDTO.BrandID;
                        objLocationBrandEntity.BranchCode = objLocationDTO.BranchCode;

                        objLocationBrandEntity.CompetitorCompanyIDs = objLocationDTO.CompetitorCompanyIds == null ? " " : objLocationDTO.CompetitorCompanyIds;
                        objLocationBrandEntity.QuickViewCompetitorCompanyIds = objLocationDTO.QuickViewCompetitors == null ? " " : objLocationDTO.QuickViewCompetitors;

                        if (objLocationBrandEntity.LocationBrandAlias != objLocationDTO.LocationBrandAlias)
                        {
                            objLocationBrandEntity.LocationBrandAlias = objLocationDTO.LocationBrandAlias;
                            _context.Entry(objLocationBrandEntity).State = System.Data.Entity.EntityState.Modified;
                            

                            List<LocationBrand> lstBrandLocationsEntity = GetAll().Where(d => d.LocationID == objLocationDTO.ID && d.ID != objLocationDTO.LocationBrandID).ToList();
                            foreach (LocationBrand entity in lstBrandLocationsEntity)
                            {
                                string alias = entity.LocationBrandAlias;
                                string newBrandAlias = alias.Substring(alias.IndexOf('-'));
                                entity.LocationBrandAlias = objLocationDTO.Code + newBrandAlias;

                                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            }
                            _context.SaveChanges();
                            _cacheManager.Remove(typeof(LocationBrand).ToString());
                        }
                        else
                        {
                            objLocationBrandEntity.LocationBrandAlias = objLocationDTO.LocationBrandAlias;
                            Update(objLocationBrandEntity);
                        }                        
                        if (!string.IsNullOrEmpty(objLocationDTO.DeletedCompetitorCompanyIds))
                        {
                            _ruleSetService.DeleteDirectCompetitorRuleSet(objLocationDTO.LocationBrandID, objLocationDTO.DeletedCompetitorCompanyIds);
                        }
                        return objLocationBrandEntity.ID;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Delete location brand using location brand id
        /// </summary>
        /// <param name="locationBrandID"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public bool DeleteLocationBrand(long locationBrandID, long updatedBy)
        {
            LocationBrand objLocationBrandEntity = GetById(locationBrandID, false);
            if (objLocationBrandEntity != null)
            {
                objLocationBrandEntity.IsDeleted = true;
                objLocationBrandEntity.UpdatedBy = updatedBy;
                objLocationBrandEntity.UpdatedDateTime = DateTime.Now;

                Update(objLocationBrandEntity);
                return true;
            }
            return false;
        }

        public long GetLocationBrandId(long locationId, long BrandId, string rentalLength)
        {
            long locationBrandId = 0;
            if (locationId > 0 && BrandId > 0 && !string.IsNullOrEmpty(rentalLength))
            {

                var locationBrand = _context.LocationBrands.Where(obj => obj.BrandID == BrandId && obj.LocationID == locationId && !obj.IsDeleted).FirstOrDefault();
                if (locationBrand != null)
                {
                    locationBrandId = locationBrand.ID;
                }
            }
            return locationBrandId;
        }

        public decimal FetchExtraDailyRate(long LocationBrandId, string rentalLength)
        {
            decimal? extraDayRate = 0;
            if (LocationBrandId > 0 && !string.IsNullOrEmpty(rentalLength))
            {
                if (rentalLength.ToUpper().IndexOf('D') >= 0)
                {
                    extraDayRate = _context.LocationBrands.Where(obj => obj.ID == LocationBrandId).FirstOrDefault().DailyExtraDayFactor;
                }
                else if (rentalLength.ToUpper().IndexOf('W') >= 0)
                {
                    extraDayRate = _context.LocationBrands.Where(obj => obj.ID == LocationBrandId).FirstOrDefault().WeeklyExtraDenom;
                }
            }
            return Convert.ToDecimal(extraDayRate);
        }

        public string GetQuickViewCompetitorsIds(long locationBrandId)
        {
            LocationBrand objLocationBrand = GetById(locationBrandId, false);
            return objLocationBrand.QuickViewCompetitorCompanyIds;
        }

        public string GetCompetitors(long locationBrandId)
        {
            string competitors = string.Empty;
            var locationBrand = _context.LocationBrands.Where(loc => loc.ID == locationBrandId && !(loc.IsDeleted));
            if (locationBrand != null)
            {
                competitors = Convert.ToString(locationBrand.Select(loc => loc.CompetitorCompanyIDs).FirstOrDefault());
            }
            return competitors;
        }
    }
}
