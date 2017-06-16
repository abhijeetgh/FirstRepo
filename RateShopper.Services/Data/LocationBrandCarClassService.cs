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
    public class LocationBrandCarClassService : BaseService<LocationBrandCarClass>, ILocationBrandCarClassService
    {
        public LocationBrandCarClassService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<LocationBrandCarClass>();
            _cacheManager = cacheManager;
        }

        public void SaveLocationBrandCarClasses(List<long> carClasses, long locationBrandID)
        {
            //Delete existing mappings
            if (_context.LocationBrandCarClass.Select(d => d.LocationBrandID == locationBrandID).ToList().Count > 0)
            {
                _context.LocationBrandCarClass.Where(p => p.LocationBrandID == locationBrandID)
                    .ToList().ForEach(p => _context.LocationBrandCarClass.Remove(p));
                _context.SaveChanges();
            }

            LocationBrandCarClass objLocationBrandClasses = null;
            foreach (var carClass in carClasses)
            {
                objLocationBrandClasses = new LocationBrandCarClass()
                {
                    CarClassID = carClass,
                    LocationBrandID = locationBrandID
                };

                _context.LocationBrandCarClass.Add(objLocationBrandClasses);
            }
            _context.SaveChanges();
            _cacheManager.Remove(typeof(LocationBrandCarClass).ToString());
        }

        public List<long> GetLocationCarClasses(long locationBrandId)
        {
            return _context.LocationBrandCarClass.Where(a => a.LocationBrandID == locationBrandId).Select(a => a.CarClassID).ToList();
        }
        public string GetLocationCarClassesData(List<long> locationBrandId)
        {
            var LocationCarClass = (from cc in _context.CarClasses
                                    join lcc in _context.LocationBrandCarClass on cc.ID equals lcc.CarClassID
                                    join lcblist in locationBrandId on lcc.LocationBrandID equals lcblist
                                    //where lcc.LocationBrandID == locationBrandId
                                    select cc).OrderBy(x => x.DisplayOrder)
                                    .ToList();

            string carClassIds= string.Join(",",(LocationCarClass.Select(x => x.ID).Distinct()).ToArray());
            return carClassIds;
        }
    }
}
