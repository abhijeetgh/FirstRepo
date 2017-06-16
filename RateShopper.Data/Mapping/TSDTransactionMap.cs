using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class TSDTransactionMap : BaseEntityConfiguration<TSDTransaction>
    {
        public TSDTransactionMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            //this.Property(t => t.ResponseCode)
            //    .HasMaxLength(500);

            //this.Property(t => t.Message)
            //    .HasMaxLength(500);

            //this.Property(t => t.XMLRequest)
            //    .IsRequired();

            // Table & Column Mappings
            this.ToTable("TSDTransactions");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.LocationBrandID).HasColumnName("LocationBrandID");
            this.Property(t => t.SearchSummaryID).HasColumnName("SearchSummaryID");
            this.Property(t => t.ResponseCode).HasColumnName("ResponseCode");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.XMLRequest).HasColumnName("XMLRequest");
            this.Property(t => t.RequestStatus).HasColumnName("RequestStatus");
            this.Property(t => t.XMLResponse).HasColumnName("XMLResponse");
            this.Property(t => t.ErrorFound).HasColumnName("ErrorFound");
            this.Property(t => t.IsRezCentralUpdate).HasColumnName("IsRezCentralUpdate");
            this.Property(t => t.IsOpaqueUpdate).HasColumnName("IsOpaqueUpdate");
            //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime");
            //this.Property(t => t.UpdatedDateTime).HasColumnName("UpdatedDateTime");

            // Relationships
            this.HasRequired(t => t.LocationBrand)
                .WithMany(t => t.TSDTransactions)
                .HasForeignKey(d => d.LocationBrandID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.TSDTransactions)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.User1)
                .WithMany(t => t.TSDTransactions1)
                .HasForeignKey(d => d.UpdatedBy);

        }
    }
}
