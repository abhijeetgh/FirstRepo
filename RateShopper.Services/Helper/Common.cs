using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Helper
{
    public class Common
    {
        public static IEnumerable<long> StringToLongList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                long num;
                if (long.TryParse(s, out num))
                    yield return num;
            }
        }
    }
}
