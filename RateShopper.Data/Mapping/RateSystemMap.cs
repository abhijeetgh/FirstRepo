using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RateSystemMap : BaseEntityConfiguration<RateSystem>
    {
        public RateSystemMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Name)
            //    .IsRequired()
            //    .HasMaxLength(50);

            //this.Property(t => t.Code)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RateSystem");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdateDateTime).HasColumnName("UpdateDateTime");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.RateSystems)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.RateSystems1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
