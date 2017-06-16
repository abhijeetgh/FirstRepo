using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RateShopper.Domain.Entities;

namespace RateShopper.Data.Mapping
{
    public class UserMap : BaseEntityConfiguration<User>
    {
        public UserMap()
        {
            
            // Properties
            //this.Property(t => t.FirstName)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //this.Property(t => t.LastName)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //this.Property(t => t.UserName)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //this.Property(t => t.EmailAddress)
            //    .IsRequired()
            //    .HasMaxLength(500);

            //this.Property(t => t.Password)
            //    .IsRequired()
            //    .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.UserRoleID).HasColumnName("UserRoleID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.IsAutomationUser).HasColumnName("IsAutomationUser");
            this.Property(t => t.IsTetheringAccess).HasColumnName("IsTetheringAccess");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.IsTSDUpdateAccess).HasColumnName("IsTSDUpdateAccess");

            // Relationships
            this.HasRequired(t => t.UserRole)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.UserRoleID);
        }
    }
}
