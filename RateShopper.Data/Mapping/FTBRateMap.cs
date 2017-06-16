using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace RateShopper.Data.Mapping
{
    public class FTBRateMap : BaseEntityConfiguration<FTBRate>
    {
        public FTBRateMap()
        {
            this.ToTable("FTBRates");
            this.Property(t => t.LocationBrandId).HasColumnName("LocationBrandId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.IsSplitMonth).HasColumnName("IsSplitMonth");
            this.Property(t => t.HasBlackOutPeroid).HasColumnName("HasBlackOutPeroid");
            this.Property(t => t.BlackOutStartDate).HasColumnName("BlackOutStartDate");
            this.Property(t => t.BlackOutEndDate).HasColumnName("BlackOutEndDate");

            this.HasRequired(t => t.LocationBrands)
               .WithMany(t => t.FTBRate)
               .HasForeignKey(d => d.LocationBrandId);
            this.HasRequired(t => t.User)
              .WithMany(t => t.FTBRate)
              .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.FTBRate1)
                .HasForeignKey(d => d.UpdatedBy);
        }
    }
}
