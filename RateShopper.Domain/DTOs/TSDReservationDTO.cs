using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class TSDReservationDTO
    {
        public DateTime DayInMonth { get; set; }
        public long ReservationCount { get; set; }
    }
}
