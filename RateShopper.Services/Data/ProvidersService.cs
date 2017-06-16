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
    public class ProvidersService : BaseService<Providers>, IProvidersService
    {
        public ProvidersService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<Providers>();
            _cacheManager = cacheManager;
        }

        public List<ProvidersDTO> GetAllProviders()
        {
            return GetAll().Where(d => !d.IsDeleted && d.ID == 1).Select(d => new ProvidersDTO { ID = d.ID, Code = d.Code, Name = d.Name, Url = d.Url, IsGov = d.IsGov, IsOneWay = d.IsOneWay }).OrderBy(d => d.ID).ToList();
        }

        
    }
}
