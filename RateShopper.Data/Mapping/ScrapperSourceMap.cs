using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class ScrapperSourceMap : BaseEntityConfiguration<ScrapperSource>
    {
        public ScrapperSourceMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //this.Property(t => t.Description)
            //    .HasMaxLength(200);

            //this.Property(t => t.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("ScrapperSources");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.ProviderId).HasColumnName("ProviderId");
            this.Property(t => t.IsGov).HasColumnName("IsGov");
        }
    }
}
