using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Util.Common
{
    public class EnumExtensions
    {
        public static TEnum ParseEnum<TEnum>(string value, bool ignoreCase = false) where TEnum : struct
        {
            TEnum tenumResult;
            Enum.TryParse<TEnum>(value, ignoreCase, out tenumResult);
            return tenumResult;
        }
    }
}
