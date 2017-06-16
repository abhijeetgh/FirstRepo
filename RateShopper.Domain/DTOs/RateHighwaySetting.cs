using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public static class RateHighwaySetting
    {
        public const string AdhocRequestURL = "http://rmwebservice.rate-monitor.com/RateMonitorService/rest/api/v1/RateMonitor/ShopAdhocRequest";
        public const string CallBackURL = "http://rmwebservice.rate-monitor.com/RateMonitorService/rest/api/v1/RateMonitor/ShopResultCallback";
        public const string AccessID = "DA1872A9-2DA4-4865-BDEF-98FC7DADC497";
    }
}
