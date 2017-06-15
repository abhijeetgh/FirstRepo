using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace TSDCount
{
    /// <summary>
    /// Summary description for wsRateShpr
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class wsRateShpr : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]        
        public List<TSDReservationDTO> getRateShopperRezCounts(string strStartDate, string strEndDate, string strBrandLocation)
        {
            return new DBLayer().GetReservationCount(strStartDate, strEndDate, strBrandLocation);
            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //ser.MaxJsonLength = int.MaxValue;
            //string json = ser.Serialize(new DBLayer().GetReservationCount(strStartDate, strEndDate, strBrandLocation));
            //return json;
            //return new DBLayer().GetReservationCount(strStartDate, strEndDate, strBrandLocation);
        }
    }
}
