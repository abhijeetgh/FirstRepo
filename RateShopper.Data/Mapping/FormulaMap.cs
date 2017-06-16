using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class FormulaMap : BaseEntityConfiguration<Formula>
    {
        public FormulaMap()
        {
            // Primary Key
           // Moved to Formula Entity
            //this.HasKey(t => t.ID);

            //// Properties
            //this.Property(t => t.TotalCostToBase)
            //    .IsRequired();

            // Table & Column Mappings
            this.ToTable("Formulas");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.SalesTax).HasColumnName("SalesTax").HasPrecision(20, 4);
            this.Property(t => t.AirportFee).HasColumnName("AirportFee").HasPrecision(20, 4);
            this.Property(t => t.Arena).HasColumnName("Arena").HasPrecision(20, 4);
            this.Property(t => t.Surcharge).HasColumnName("Surcharge").HasPrecision(20, 4);
            this.Property(t => t.VLRF).HasColumnName("VLRF").HasPrecision(20, 4);
            this.Property(t => t.CFC).HasColumnName("CFC").HasPrecision(20, 4);
            this.Property(t => t.TotalCostToBase).HasColumnName("TotalCostToBase");
            this.Property(t => t.BaseToTotalCost).HasColumnName("BaseToTotalCost");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.Formulas)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Formulas)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.Formulas1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
