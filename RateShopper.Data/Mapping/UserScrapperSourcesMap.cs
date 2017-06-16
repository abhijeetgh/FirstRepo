using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class UserScrapperSourcesMap : BaseEntityConfiguration<UserScrapperSources>
    {
        public UserScrapperSourcesMap()
        {
            // Primary Key
            //this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("UserScrapperSources");
            //this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.ScrapperSourceID).HasColumnName("ScrapperSourceID");

            // Relationships
            this.HasRequired(t => t.ScrapperSource)
                .WithMany(t => t.UserScrapperSources)
                .HasForeignKey(d => d.ScrapperSourceID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserScrapperSources)
                .HasForeignKey(d => d.UserID);

        }
    }
}
