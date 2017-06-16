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
    public class LocationBrandRentalLengthService : BaseService<LocationBrandRentalLength>, ILocationBrandRentalLengthService
    {
        ILocationBrandService _locationBrandService;        
        public LocationBrandRentalLengthService(IEZRACRateShopperContext context, ICacheManager cacheManager, ILocationBrandService locationBrandService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<LocationBrandRentalLength>();
            _cacheManager = cacheManager;
            _locationBrandService = locationBrandService;            
        }

        public void SaveLocationBrandRentalLengths(LocationDTO objLocationDTO)
        {
            //Delete existing mappings
            long locationBrandId = objLocationDTO.LocationBrandID;
            UpdateSameLOR(objLocationDTO, locationBrandId);
            UpdateScheduledJobsLOR(locationBrandId, objLocationDTO.RentalLengths);
            //Replace dominent brand LOR to dependant brand LOR
            if (objLocationDTO.ApplyDependantBrandLOR)
            {
                LocationBrand DependantBrandLocationId = _context.LocationBrands.Where(x => x.LocationID == objLocationDTO.ID && x.ID != objLocationDTO.LocationBrandID && !(x.IsDeleted)).FirstOrDefault();
                if (DependantBrandLocationId != null)
                {
                    UpdateSameLOR(objLocationDTO, DependantBrandLocationId.ID);
                    UpdateScheduledJobsLOR(DependantBrandLocationId.ID, objLocationDTO.RentalLengths);
                }
            }

        }
        public void UpdateSameLOR(LocationDTO objLocationDTO, long locationBrandId)
        {

            if (_context.LocationBrandRentalLength.Select(d => d.LocationBrandID == locationBrandId).ToList().Count > 0)
            {
                _context.LocationBrandRentalLength.Where(p => p.LocationBrandID == locationBrandId)
                    .ToList().ForEach(p => _context.LocationBrandRentalLength.Remove(p));
                _context.SaveChanges();
            }

            LocationBrandRentalLength objLocationBrandRentalLengthEntity = null;
            foreach (var rentalLength in objLocationDTO.RentalLengths)
            {
                objLocationBrandRentalLengthEntity = new LocationBrandRentalLength()
                {
                    RentalLengthID = rentalLength,
                    LocationBrandID = locationBrandId,
                    CreatedBy = objLocationDTO.CreatedBy,
                    UpdatedBy = objLocationDTO.CreatedBy,
                    CreatedDateTime = DateTime.Now,
                    UpdatedDateTime = DateTime.Now
                };
                _context.LocationBrandRentalLength.Add(objLocationBrandRentalLengthEntity);
            }
            _context.SaveChanges();
            _cacheManager.Remove(typeof(LocationBrandRentalLength).ToString());
        }

        void UpdateScheduledJobsLOR(long locationBrandId, List<long> brandRentalLengths)
        {
            List<long> locationRentalLengths = new List<long>(brandRentalLengths);
            List<RentalLength> rentalLengths = _context.RentalLengths.ToList();

            locationRentalLengths.AddRange(rentalLengths.Join(locationRentalLengths, rl => rl.AssociateMappedId, lr => lr, (rentalLength, locationLOR) =>
                new { Id = rentalLength.MappedID, AssociateId = rentalLength.AssociateMappedId }).GroupBy(d => d.AssociateId).Select(d => d.Max(e => e.Id)));


            List<ScheduledJob> scheduledJobs = _context.ScheduledJobs.Where(d => !d.IsDeleted && d.LocationBrandIDs == locationBrandId.ToString()).ToList();
            scheduledJobs.ForEach(d =>
            {
                string commonLORs = string.Join(",", locationRentalLengths.Select(e => e.ToString()).Intersect(d.RentalLengthIDs.Split(',')));
                if (!string.IsNullOrEmpty(commonLORs))
                {
                    d.RentalLengthIDs = commonLORs;
                }
                //else if (d.RentalLengthIDs.IndexOf(',') == -1 && associateLORList.Any(e=>e.Id==Convert.ToInt64(d.RentalLengthIDs)))
                //{
                //assign D2 incase job is of D1 only
                //    d.RentalLengthIDs = associateLORList.Where(e => e.AssociateId == (associateLORList.Where(g => g.Id == Convert.ToInt64(d.RentalLengthIDs)).FirstOrDefault().AssociateId)).GroupBy(e => e.AssociateId).Select(e => e.Max(f => f.Id)).FirstOrDefault().ToString();
                //}
                else
                {
                    d.IsEnabled = false;
                    d.NextRunDateTime = null;
                }
                _context.Entry(d).State = System.Data.Entity.EntityState.Modified;
            });
            _context.SaveChanges();
            _cacheManager.Remove(typeof(ScheduledJob).ToString());
        }
    }
}
