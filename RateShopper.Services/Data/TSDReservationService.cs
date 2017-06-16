using RateShopper.Data;
using RateShopper.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class TSDReservationService : ITSDReservationService, IDisposable
    {
        TSDContext _tsdContext = null;

        private static string _connectionString;
        const string getReserVationCount = "SELECT convert(date,date_out) as DayInMonth, CONVERT(bigint,COUNT(ISNULL(KNUM,0))) as ReservationCount FROM creser WHERE convert(date,date_out) between @startDate and @endDate AND CLOSED_FLAG = 0 AND LOC_OUT = @locationCode group by convert(date,date_out)";
        public TSDReservationService()
        {
            _tsdContext = new TSDContext();
            _connectionString = _tsdContext.Database.Connection.ConnectionString;
        }

        public async Task<IEnumerable<TSDReservationDTO>> GetLocationReservationCount(string locationCode, DateTime startDate, DateTime endDate)
        {
            IEnumerable<TSDReservationDTO> monthReservationList = null;

            if (!string.IsNullOrEmpty(locationCode) && startDate != null && endDate != null)
            {
                var queryParameters = new[] { new SqlParameter("@startDate", startDate.Date), new SqlParameter("@endDate", endDate.Date), new SqlParameter("@locationCode", locationCode) };
                monthReservationList = await _tsdContext.Database.SqlQuery<TSDReservationDTO>(getReserVationCount, queryParameters).ToListAsync();
                return monthReservationList;
            }

            return monthReservationList;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tsdContext.Dispose();
                _tsdContext = null;
            }
        }
    }
}
