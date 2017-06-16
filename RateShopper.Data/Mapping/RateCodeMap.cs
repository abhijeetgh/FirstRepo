using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RateCodeMap : BaseEntityConfiguration<RateCode>
    {
        public RateCodeMap()
        {
            // Table & Column Mappings
            this.ToTable("RateCode");

            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.SupportedBrandIDs).HasColumnName("SupportedBrandIDs");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.User)
                .WithMany(t => t.RateCodes)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.RateCodes1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
