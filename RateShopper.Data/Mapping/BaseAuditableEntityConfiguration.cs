using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EZRAC.RateShopper.Data.Mapping
{
    public class BaseAuditableEntityConfiguration<T> : BaseEntityConfiguration<T> where T : BaseAuditableEntity
    {
        public BaseAuditableEntityConfiguration()
        {
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");
        }
    }
}