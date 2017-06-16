using System;

namespace RateShopper.Domain.Entities
{
    public abstract class BaseAuditableEntity:BaseEntity
    {
        public virtual long CreatedBy { get; set; }

        public virtual long UpdatedBy { get; set; }

        public virtual System.DateTime CreatedDateTime { get; set; }

        public virtual System.DateTime UpdatedDateTime { get; set; }
    }
}
