using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
	public class AllRuleSetsForLocationBrandDTO
	{
		public RuleSet RuleSet { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime endDate { get; set; }
		public long WeekDayID { get; set; }
		public long RentalLengthID { get; set; }
		public long CarClassID { get; set; }
	}
}
