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
    public class LocationCompanyService : BaseService<LocationCompany>, ILocationCompanyService
    {
        public LocationCompanyService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<LocationCompany>();
            _cacheManager = cacheManager;
        }

        public List<LocationCompanyDTO> GetLocations()
        {
            List<LocationCompanyDTO> lstCompanyDTO = (from lo in _context.Locations
                                                      where !lo.IsDeleted
                                                      orderby lo.Code
                                                      select new LocationCompanyDTO { LocationID = lo.ID, LocationName = lo.Code }).ToList();
            return lstCompanyDTO;
        }

        public List<LocationCompanyDTO> GetCompanyLocations(long companyID)
        {
            List<LocationCompanyDTO> lstCompanyDTO = (from lo in _context.Locations
                                                      join lc in _context.LocationCompany on new { ID = lo.ID, companyid = companyID } equals new { ID = lc.LocationID, companyid = lc.CompanyID }                                                      
                                                      where !lo.IsDeleted                                                      
                                                      select new LocationCompanyDTO { LocationID = lo.ID, IsTerminalInside = lc.IsTerminalInside, ID = lc.ID }
                     ).ToList();
            return lstCompanyDTO;
        }

        public void SaveCompanyLocations(List<LocationCompanyDTO> lstCompanyLocations, long companyID)
        {
            //Delete existing mappings
            List<LocationCompanyDTO> lstExistingCompanyLocations = (from locationcompany in _context.LocationCompany
                                                                    where locationcompany.CompanyID == companyID
                                                                    select new LocationCompanyDTO { ID = locationcompany.ID, LocationID = locationcompany.LocationID, IsTerminalInside = locationcompany.IsTerminalInside }
                     ).ToList();

            if (lstExistingCompanyLocations.Count > 0)
            {   
                //remove items from list which are exists in latest input list
                lstExistingCompanyLocations.RemoveAll(x => lstCompanyLocations.Exists(y => y.LocationID == x.LocationID && y.IsTerminalInside == x.IsTerminalInside));
                
                //remove items from db which are not exists in latest input list
                lstExistingCompanyLocations.ForEach(d => _context.LocationCompany.Remove(_context.LocationCompany.Find(d.ID)));
                
                _context.SaveChanges();
            }

            //remove items from list which are already exists in db
            lstCompanyLocations.RemoveAll(d => _context.LocationCompany.Where(p => p.CompanyID == companyID).ToList().Exists(p => p.LocationID == d.LocationID));

            if (lstCompanyLocations.Count > 0)
            {
                LocationCompany objLocationCompany = null;
                foreach (var companyLocation in lstCompanyLocations)
                {
                    objLocationCompany = new LocationCompany()
                    {
                        LocationID = companyLocation.LocationID,
                        CompanyID = companyID,
                        IsTerminalInside = companyLocation.IsTerminalInside.HasValue ? companyLocation.IsTerminalInside.Value : true
                    };

                    _context.LocationCompany.Add(objLocationCompany);
                }
                _context.SaveChanges();
                _cacheManager.Remove(typeof(LocationCompany).ToString());
            }
        }
    }
}
