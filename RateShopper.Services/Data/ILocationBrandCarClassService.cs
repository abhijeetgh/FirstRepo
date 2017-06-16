using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
	public interface ILocationBrandCarClassService : IBaseService<LocationBrandCarClass>
	{
		void SaveLocationBrandCarClasses(List<long> carClasses, long locationBrandID);
		List<long> GetLocationCarClasses(long locationBrandId);
        string GetLocationCarClassesData(List<long> locationBrandId);
	}
}
