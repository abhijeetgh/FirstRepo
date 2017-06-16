using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class CarClassMap : BaseEntityConfiguration<CarClass>
    {
        public CarClassMap()
        {
            // Properties
            //Moved to Car class Entity

            // Table & Column Mappings
            this.ToTable("CarClasses");            
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.CarClasses)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.CarClasses1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
