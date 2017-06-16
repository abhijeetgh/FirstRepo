using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class CompanyMap : BaseEntityConfiguration<Company>
    {
        public CompanyMap()
        {
            // Primary Key
            //Moved to Company Entity
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Code)
            //    .IsRequired()
            //    .HasMaxLength(50);

            //this.Property(t => t.Name)
            //    .IsRequired()
            //    .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Companies");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.IsBrand).HasColumnName("IsBrand");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.Companies)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.Companies1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
