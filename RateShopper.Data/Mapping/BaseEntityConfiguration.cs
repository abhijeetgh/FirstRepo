using RateShopper.Domain.Entities;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace RateShopper.Data.Mapping
{
    public abstract class BaseEntityConfiguration<T> : EntityTypeConfiguration<T> where T:BaseEntity
    {
        public BaseEntityConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Column Mappings
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
