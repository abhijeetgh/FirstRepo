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
	public class GlobalLimitDetailService : BaseService<GlobalLimitDetail>, IGlobalLimitDetailService
	{
		IGlobalLimitService globalLimitService;
		public GlobalLimitDetailService(IEZRACRateShopperContext context, ICacheManager cacheManager, IUserRolesService _userRolesService, IGlobalLimitService globalLimitService)
			: base(context, cacheManager)
		{
			_context = context;
			_dbset = _context.Set<GlobalLimitDetail>();
			_cacheManager = cacheManager;

			this.globalLimitService = globalLimitService;
		}

		public List<GlobalLimitDataDTO> GetGlobalLimitData(Int64 LocationBrandID)
		{
			return (from globalLimit in _context.GlobalLimits
					join globalLimitDetail in _context.GlobalLimitDetails on globalLimit.ID equals globalLimitDetail.GlobalLimitID
					where globalLimit.LocationBrandID == LocationBrandID
					select new GlobalLimitDataDTO
					{
						GlobalLimit = globalLimit,
						GlobalLimitDetails = globalLimitDetail,
					}).ToList();
		}

		public List<GlobalLimitDetail> GetGlobalLimitDetails(List<GlobalLimitDataDTO> GlobalLimitData, DateTime date)
		{
			var x = (from record in GlobalLimitData
					 where record.GlobalLimit.StartDate <= date
					 && record.GlobalLimit.EndDate >= date
					 select record.GlobalLimitDetails).ToList();
			return x;
		}

		public void SaveGlobalLimitDetails(GlobalLimitDTO objGlobalLimitDTO)
		{
			foreach (var detail in objGlobalLimitDTO.LstGlobalLimitDetails)
			{
				GlobalLimitDetail objGlobalLimitDetailEntity = _context.GlobalLimitDetails.Where(d => d.GlobalLimitID == objGlobalLimitDTO.GlobalLimitID && d.CarClassID == detail.CarClassID).SingleOrDefault();
				if (objGlobalLimitDetailEntity == null)
				{
					if (!(!detail.DayMin.HasValue && !detail.DayMax.HasValue && !detail.WeekMin.HasValue && !detail.WeekMax.HasValue && !detail.MonthlyMin.HasValue && !detail.MonthlyMax.HasValue))
					{
						objGlobalLimitDetailEntity = new GlobalLimitDetail();
						objGlobalLimitDetailEntity.CarClassID = (long)detail.CarClassID;
						objGlobalLimitDetailEntity.GlobalLimitID = (long)objGlobalLimitDTO.GlobalLimitID;
						objGlobalLimitDetailEntity.DayMin = detail.DayMin;
						objGlobalLimitDetailEntity.DayMax = detail.DayMax;
						objGlobalLimitDetailEntity.WeekMin = detail.WeekMin;
						objGlobalLimitDetailEntity.WeekMax = detail.WeekMax;
						objGlobalLimitDetailEntity.MonthMin = detail.MonthlyMin;
						objGlobalLimitDetailEntity.MonthMax = detail.MonthlyMax;

						_context.GlobalLimitDetails.Add(objGlobalLimitDetailEntity);
					}
				}
				else
				{
					if (!detail.DayMin.HasValue && !detail.DayMax.HasValue && !detail.WeekMin.HasValue && !detail.WeekMax.HasValue && !detail.MonthlyMin.HasValue && !detail.MonthlyMax.HasValue)
					{
						_context.GlobalLimitDetails.Remove(objGlobalLimitDetailEntity);
					}
					else
					{
						objGlobalLimitDetailEntity.DayMin = detail.DayMin;
						objGlobalLimitDetailEntity.DayMax = detail.DayMax;
						objGlobalLimitDetailEntity.WeekMin = detail.WeekMin;
						objGlobalLimitDetailEntity.WeekMax = detail.WeekMax;
						objGlobalLimitDetailEntity.MonthMin = detail.MonthlyMin;
						objGlobalLimitDetailEntity.MonthMax = detail.MonthlyMax;

						_context.Entry(objGlobalLimitDetailEntity).State = System.Data.Entity.EntityState.Modified;
					}
				}
			}
			_context.SaveChanges();
			_cacheManager.Remove(typeof(GlobalLimitDetail).ToString());
		}

		public bool DeleteGlobalLimitDetails(long globalLimitID)
		{
			if (_context.GlobalLimitDetails.Select(d => d.GlobalLimitID == globalLimitID).ToList().Count > 0)
			{
				_context.GlobalLimitDetails.Where(p => p.GlobalLimitID == globalLimitID)
					.ToList().ForEach(p => _context.GlobalLimitDetails.Remove(p));
				_context.SaveChanges();
				_cacheManager.Remove(typeof(GlobalLimitDetail).ToString());

				return true;
			}
			return false;
		}
	}
}
