using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.Helper
{
    public static class ConfigSettingsReader
    {
        public static string GetAppSettingValue(string key)
        {
            string appConfigValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appConfigValue))
            {
                //throw new Core.Exception.CRMSException(string.Concat("Value was not set for key: ", key));
            }
            return appConfigValue;
        }

    }
}