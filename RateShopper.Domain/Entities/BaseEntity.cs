using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.Entities
{
    public abstract class BaseEntity
    {
        public virtual long ID { get; set; }
    }
}
