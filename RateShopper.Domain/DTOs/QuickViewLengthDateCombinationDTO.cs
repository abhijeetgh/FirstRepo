using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
	public class QuickViewLengthDateCombinationDTO
	{
		public List<LengthDateCombination> LengthDateCombinationChanged { get; set; }
		public List<LengthDateCombination> LengthDateCombinationUnChanged { get; set; }

	}
	public class LengthDateCombination
	{
		public long RentalLengthId { get; set; }
		public string FormattedDate { get; set; }
		public string DisplayText { get; set; }
        public bool Changed { get; set; }
	}
}
