using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class RuleSetRentalLengthMap : BaseEntityConfiguration<RuleSetRentalLength>
    {
        public RuleSetRentalLengthMap()
        {
            // Primary Key
           //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RuleSetRentalLength");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RuleSetID).HasColumnName("RuleSetID");
            this.Property(t => t.RentalLengthID).HasColumnName("RentalLengthID");

            // Relationships
            this.HasRequired(t => t.RentalLength)
                .WithMany(t => t.RuleSetRentalLength)
                .HasForeignKey(d => d.RentalLengthID);
            this.HasRequired(t => t.RuleSet)
                .WithMany(t => t.RuleSetRentalLengths)
                .HasForeignKey(d => d.RuleSetID);

        }
    }
}
