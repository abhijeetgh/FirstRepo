using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RangeIntervalMap : BaseEntityConfiguration<RangeInterval>
    {
        public RangeIntervalMap()
        {
            // Primary Key
            //Moved to RangeInterval Entity
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.Range)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RangeInterval");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Range).HasColumnName("Range");
        }
    }
}
