using RateShopper.Data;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Data.Entity;
using RateShopper.Domain.Entities;
using RateShopper;
using RateShopper.Core.Cache;


namespace RateShopper.Services.Data
{
    public class RentalLengthService : BaseService<RentalLength>, IRentalLengthService
    {
        public RentalLengthService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<RentalLength>();
            _cacheManager = cacheManager;
        }

        public Dictionary<long, long> GetRentalLengthDictionary()
        {
            return GetAll(false).Select(obj => new { MappedId = obj.MappedID, ID = obj.ID }).ToDictionary(obj => obj.MappedId, obj => obj.ID);
        }

        public override IEnumerable<RentalLength> GetAll(bool isCacheble = true)
        {
            IEnumerable<RentalLength> rentalLengths = base.GetAll(isCacheble).Where(obj => obj.IsSearchEnable != null);
            return rentalLengths;
        }

        public IEnumerable<RentalLength> GetRentalLength(bool isCacheble = true)
        {
            IEnumerable<RentalLength> rentalLengths = base.GetAll(isCacheble);
            return rentalLengths;
        }
    }
}
