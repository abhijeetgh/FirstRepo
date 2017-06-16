using EZRAC.Core.CustomException;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.BaseModel
{
    [JsonObject]
    [Serializable]
    public class BaseResult<T>
    {
        public bool IsError
        {
            get
            {
                return this.ErrorMessage != null ? true : false;

            }
        }
        public CrmsException ErrorMessage { get; set; }

        public T Result { get; set; }

        public string OwsErrorCode { get; set; }

        public string CrmsErrorCode
        {
            get;
            set;
        }
    }
}
