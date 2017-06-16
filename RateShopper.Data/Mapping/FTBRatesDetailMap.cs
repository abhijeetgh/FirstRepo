using RateShopper.Data.Mapping;
using RateShopper.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RateShopper.Data.Mapping
{
    public class FTBRatesDetailMap : BaseEntityConfiguration<FTBRatesDetail>
    {
        public FTBRatesDetailMap()
        {
            this.ToTable("FTBRatesDetails");
            this.Property(t => t.FTBRatesId).HasColumnName("FTBRatesId");
            this.Property(t => t.RentalLengthId).HasColumnName("RentalLengthId");
            this.Property(t => t.CarClassId).HasColumnName("CarClassId");
            this.Property(t => t.Sunday).HasColumnName("Sunday");
            this.Property(t => t.Monday).HasColumnName("Monday");
            this.Property(t => t.Tuesday).HasColumnName("Tuesday");
            this.Property(t => t.Wednesday).HasColumnName("Wednesday");
            this.Property(t => t.Thursday).HasColumnName("Thursday");
            this.Property(t => t.Friday).HasColumnName("Friday");
            this.Property(t => t.Saturday).HasColumnName("Saturday");
            this.Property(t => t.SplitPartId).HasColumnName("SplitPartId");

            this.HasRequired(t => t.FTBRates)
            .WithMany(t => t.FTBRatesDetail)
            .HasForeignKey(d => d.FTBRatesId);

            this.HasRequired(t => t.CarClasses)
             .WithMany(t => t.FTBRatesDetail)
             .HasForeignKey(d => d.CarClassId);
        }
    }
}
