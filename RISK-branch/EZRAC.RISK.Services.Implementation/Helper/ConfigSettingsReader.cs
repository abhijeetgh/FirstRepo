using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EZRAC.RISK.Services.Implementation.Helper
{

    public static class ConfigSettingsReader
    {
        public static string GetAppSettingValue(string key)
        {
            string appConfigValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appConfigValue))
            {
                
            }
            return appConfigValue;
        }

    }
}
