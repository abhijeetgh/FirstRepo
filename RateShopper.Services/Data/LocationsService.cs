using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateShopper.Services.Data
{
    public class LocationsService : BaseService<Location>, ILocationService
    {
        public LocationsService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<Location>();
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all car classes
        /// </summary>
        /// <returns></returns>
        public List<CarClassDTO> GetCarClass()
        {
            List<CarClassDTO> carClasses = _context.CarClasses.Where(o => !o.IsDeleted).Select(obj => new CarClassDTO { ID = obj.ID, Code = obj.Code }
                    ).ToList<CarClassDTO>();
            return carClasses;
        }

        /// <summary>
        /// Get all rental lengths
        /// </summary>
        /// <returns></returns>
        public List<RentalLengthDTO> GetRentalLength()
        {
            List<RentalLengthDTO> rentalLengths = _context.RentalLengths.Select(obj => new RentalLengthDTO { MappedID = obj.MappedID, Code = obj.Code, ID = obj.ID }
                    ).ToList<RentalLengthDTO>();
            return rentalLengths;
        }

        /// <summary>
        /// Get all companies/brands
        /// </summary>
        /// <returns></returns>
        public List<CompanyDTO> GetCompany()
        {
            List<CompanyDTO> companies = _context.Companies.Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            return companies;
        }

        /// <summary>
        /// Add/update location details
        /// </summary>
        /// <param name="objLocationDTO"></param>
        /// <returns></returns>
        public long SaveLocation(LocationDTO objLocationDTO)
        {
            if (objLocationDTO.ID == 0)
            {
                Location objExistingLocation = GetAll().Where(obj => obj.Code == objLocationDTO.Code && !obj.IsDeleted).FirstOrDefault();

                if (objExistingLocation == null)
                {
                    Location objLocationEntity = new Location()
                    {
                        ID = objLocationDTO.ID,
                        Code = objLocationDTO.Code,
                        IsDeleted = false,
                        UpdatedBy = objLocationDTO.CreatedBy,
                        CreatedBy = objLocationDTO.CreatedBy,
                        UpdatedDateTime = DateTime.Now,
                        CreatedDateTime = DateTime.Now,
                    };

                    Add(objLocationEntity);
                    return objLocationEntity.ID;
                }
                else
                {
                    return objExistingLocation.ID;
                }
            }
            else
            {
                //Check for duplication of code
                if (GetAll().Where(obj => obj.Code == objLocationDTO.Code && !obj.IsDeleted && obj.ID != objLocationDTO.ID).FirstOrDefault() == null)
                {
                    Location objExistingLocationEntity = GetAll().Where(obj => obj.ID == objLocationDTO.ID && !obj.IsDeleted).FirstOrDefault();
                    if (objExistingLocationEntity != null)
                    {
                        objExistingLocationEntity.Code = objLocationDTO.Code;
                        objExistingLocationEntity.UpdatedBy = objLocationDTO.CreatedBy;
                        objExistingLocationEntity.UpdatedDateTime = DateTime.Now;
                        Update(objExistingLocationEntity);
                        return objExistingLocationEntity.ID;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Get all brand locations
        /// </summary>
        /// <returns></returns>
        public List<LocationDTO> GetLocations()
        {
            List<LocationDTO> locations = (from location in _context.Locations
                                           join locationBrand in _context.LocationBrands on location.ID equals locationBrand.LocationID
                                           orderby location.ID
                                           where !location.IsDeleted && !locationBrand.IsDeleted
                                           select new LocationDTO
                                           {
                                               ID = location.ID,
                                               LocationBrandID = locationBrand.ID,
                                               BrandID = locationBrand.BrandID,
                                               Code = location.Code,
                                               Description = locationBrand.Description,
                                               LocationBrandAlias = locationBrand.LocationBrandAlias
                                           }).ToList();
            return locations;
        }

        /// <summary>
        /// Check whether any brand exists for location and if not then delete it
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public bool DeleteLocation(long locationID, long updatedBy)
        {
            if (_context.LocationBrands.Where(obj => obj.LocationID == locationID && !obj.IsDeleted).FirstOrDefault() == null)
            {
                Location objLocationEntity = _context.Locations.Find(locationID);
                if (objLocationEntity != null)
                {
                    objLocationEntity.IsDeleted = true;
                    objLocationEntity.UpdatedBy = updatedBy;
                    objLocationEntity.UpdatedDateTime = DateTime.Now;

                    Update(objLocationEntity);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get specific brand location
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="locationBrandID"></param>
        /// <returns></returns>
        public LocationDTO GetLocation(long locationID, long locationBrandID)
        {
            bool ApplyDependantLocationBrand = (_context.LocationBrands.Where(x => x.LocationID == locationID && x.ID != locationBrandID && !(x.IsDeleted)).FirstOrDefault() != null) ? true : false;
            LocationDTO objLocationDTO = (from location in _context.Locations
                                          join locationBrand in _context.LocationBrands on location.ID equals locationBrand.LocationID
                                          
                                          where (locationBrand.ID == locationBrandID && !(location.IsDeleted))
                                          select new
                                          {
                                              ID = location.ID,
                                              LocationBrandID = locationBrand.ID,
                                              BrandID = locationBrand.BrandID,
                                              Code = location.Code,
                                              Description = locationBrand.Description,
                                              WeeklyExtraDenominator = locationBrand.WeeklyExtraDenom,
                                              DailyExtraDayFactor = locationBrand.DailyExtraDayFactor,
                                              TSDCustomerNumber = locationBrand.TSDCustomerNumber,
                                              TSDPassCode = locationBrand.TSDPassCode,
                                              UseLORRateCode = locationBrand.UseLORRateCode,
                                              BranchCode = locationBrand.BranchCode,
                                              CarClasses = _context.LocationBrandCarClass.Where(d => d.LocationBrandID == locationBrandID).Select(d => d.CarClassID),
                                              RentalLengths = _context.LocationBrandRentalLength.Where(d => d.LocationBrandID == locationBrandID).Select(d => d.RentalLengthID),
                                              CompetitorCompanyIds = locationBrand.CompetitorCompanyIDs,
                                              QuickViewCompetitors = locationBrand.QuickViewCompetitorCompanyIds
                                          }).ToList().Select(d => new LocationDTO
                                          {
                                              ID = d.ID,
                                              LocationBrandID = d.LocationBrandID,
                                              BrandID = d.BrandID,
                                              Code = d.Code,
                                              Description = d.Description,
                                              WeeklyExtraDenominator = d.WeeklyExtraDenominator,
                                              DailyExtraDayFactor = d.DailyExtraDayFactor,
                                              TSDCustomerNumber = d.TSDCustomerNumber,
                                              TSDPassCode = d.TSDPassCode,
                                              UseLORRateCode = d.UseLORRateCode,
                                              BranchCode = d.BranchCode,
                                              CarClasses = d.CarClasses.ToList(),
                                              RentalLengths = d.RentalLengths.ToList(),
                                              CompetitorCompanyIds = d.CompetitorCompanyIds,
                                              QuickViewCompetitors = d.QuickViewCompetitors,
                                              LocationPricingManagerName = GetLocationPricingManagerName(locationBrandID, locationID),
                                              ApplyDependantBrandLOR = ApplyDependantLocationBrand
                                          }).FirstOrDefault();

            return objLocationDTO;
        }

        public Dictionary<string, long> GetLocationDictionary()
        {
            return GetAll(false).Where(loc => !loc.IsDeleted).Select(obj => new { location = obj.Code, ID = obj.ID }).ToDictionary(obj => obj.location, obj => obj.ID);
        }

        public string GetLocationPricingManagerName(long locationBrandID, long locationID)
        {
            var locationBrands = (from loc in _context.LocationBrands
                                  where loc.LocationID == locationID && loc.IsFTBDominantBrand
                                  select loc.ID).FirstOrDefault();


            var PricingManagerName = (from locmgr in _context.LocationPricingManagers
                                      join user in _context.Users on locmgr.UserID equals user.ID
                                      where locmgr.LocationBrandID == locationBrands
                                      select user.FirstName + " " + user.LastName).SingleOrDefault();




            return string.IsNullOrEmpty(PricingManagerName) ? "Not Assgined" : PricingManagerName;
        }
    }
}
