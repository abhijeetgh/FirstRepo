using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.CustomException
{
    [Serializable]
    public class CrmsException : System.Exception
    {
        public StatusCode Status { get; set; }
        public bool IsError { get; set; }
        public virtual string ErrorMessage { get; protected set; }
        public CrmsException()
        {

        }
        public CrmsException(string message)
            : base(message)
        {
        }
        public CrmsException(System.Exception innerException)
            : base("", innerException)
        {
        }
        public CrmsException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        public CrmsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                StatusCode code;

                if (Enum.TryParse(info.GetString("Status"), out code))
                    this.Status = code;
                else
                    this.Status = StatusCode.Fail;
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info != null)
            {
                info.AddValue("Status", (this.Status != null) ? this.Status.ToString() : StatusCode.Fail.ToString());
            }
        }
    }


    public enum StatusCode
    {
        Success,
        Fail,
        Warning, // added for handling informative messages. like no data, Invalid input.
        Partial,
    }
}
