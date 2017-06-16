using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RentalLengthMap : BaseEntityConfiguration<RentalLength>
    {
        public RentalLengthMap()
        {
            // Table & Column Mappings
            this.ToTable("RentalLengths");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.MappedID).HasColumnName("MappedID");
            this.Property(t => t.AssociateMappedId).HasColumnName("AssociateMappedId");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.IsSearchEnable).HasColumnName("IsSearchEnable");
        }
    }
}
