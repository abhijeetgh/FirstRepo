using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface ITSDReservationService
    {
        Task<IEnumerable<TSDReservationDTO>> GetLocationReservationCount(string locationCode, DateTime startDate, DateTime endDate);
    }
}
