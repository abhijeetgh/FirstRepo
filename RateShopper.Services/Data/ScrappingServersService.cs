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
    public enum EnumScrappingServers
    {
        NormalShop = 0,
        AutomationShop = 1,
        ReadOnlyShop = 2,
        SummaryShop = 3,
        QuickViewShop = 4
    }

    public class ScrappingServersService : BaseService<ScrappingServers>, IScrappingServersService
    {
        public ScrappingServersService(IEZRACRateShopperContext context, ICacheManager cacheManager):base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<ScrappingServers>();
            _cacheManager = cacheManager;
        }
    

        public string GetScrappingUrl(EnumScrappingServers enumScrappingServers)
        {            
            ScrappingServers scrappingServer = null;

            switch (enumScrappingServers)
            {
                case EnumScrappingServers.NormalShop:
                    scrappingServer = this.GetAll().Where(d => d.IsNormalShop.HasValue && d.IsNormalShop.Value).OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
                    break;
                case EnumScrappingServers.AutomationShop:
                    scrappingServer = this.GetAll().Where(d => d.IsAutomation.HasValue && d.IsAutomation.Value).OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
                    break;
                case EnumScrappingServers.QuickViewShop:
                    scrappingServer = this.GetAll().Where(d => d.IsQuickView.HasValue && d.IsQuickView.Value).OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
                    break;
                case EnumScrappingServers.ReadOnlyShop:
                    scrappingServer = this.GetAll().Where(d => d.IsReadOnlyShop.HasValue && d.IsReadOnlyShop.Value).OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
                    break;                
                case EnumScrappingServers.SummaryShop:
                    scrappingServer = this.GetAll().Where(d => d.IsSummaryShop.HasValue && d.IsSummaryShop.Value).OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
                    break;
            }
            if (scrappingServer == null)
            {
                scrappingServer = this.GetAll().OrderBy(d => d.LastUsedDateTime).FirstOrDefault();
            }

            scrappingServer.LastUsedDateTime = DateTime.Now;
            Update(scrappingServer);
            return scrappingServer.Url;
        }
    }
}
