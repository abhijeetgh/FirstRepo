using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation.Helper
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWorkdays(this DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday //&&
                    //!newDate.IsHoliday()
                    )
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

    //    public static bool IsHoliday(this DateTime date)
    //    {
    //        // You'd load/cache from a DB or file somewhere rather than hardcode
    //        DateTime[] holidays =
    //        new DateTime[] { 
    //  //new DateTime(2010,12,27),
    //  //new DateTime(2010,12,28),
    //  //new DateTime(2011,01,03),
    //  //new DateTime(2011,01,12),
    //  //new DateTime(2011,01,13)
    //};

    //        return holidays.Contains(date.Date);
    //    }
    }
}
