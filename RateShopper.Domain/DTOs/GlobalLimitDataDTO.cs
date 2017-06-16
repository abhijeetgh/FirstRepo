using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
	public class GlobalLimitDataDTO
	{
		public GlobalLimit GlobalLimit { get; set; }
		public GlobalLimitDetail GlobalLimitDetails { get; set; }
	}
}
