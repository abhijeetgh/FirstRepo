using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
	public interface IGlobalLimitDetailService : IBaseService<GlobalLimitDetail>
	{
		List<GlobalLimitDataDTO> GetGlobalLimitData(Int64 LocationBrandID);
		List<GlobalLimitDetail> GetGlobalLimitDetails(List<GlobalLimitDataDTO> GloalLimitData, DateTime date);
		void SaveGlobalLimitDetails(GlobalLimitDTO objGlobalLimitDTO);
		bool DeleteGlobalLimitDetails(long globalLimitID);
	}
}
