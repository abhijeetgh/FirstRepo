using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSDCount
{
    public class TSDReservationDTO
    {
        public string DayInMonth { get; set; }
        public long ReservationCount { get; set; }

        protected internal TSDReservationDTO()
        {
        }
    }

    public class MyClassName_List : List<TSDReservationDTO>
    {
        public MyClassName_List() { }
    }
}

//public class MyClassName
//{
//    private string _item1 = string.Empty;
//    private string _item2 = string.Empty;

//    public string item1 = { get { return _item1; } set { _item1 = value; } }
//    public string item2 = { get { return _item2; } set { _item2 = value; } }

//    protected internal MyClassName() { } //add a protected internal constructor to remove the returned __type attribute in the JSON response
//}

