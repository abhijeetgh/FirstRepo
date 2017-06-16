using EZRAC.RISK.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;


namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("Users");

            Property(u => u.UserName)
                .IsUnicode()
                .IsRequired();

            Property(u => u.Email).HasColumnName("EmailAddress");
            Property(u => u.FirstName);
            Property(u => u.LastName);
            Property(u => u.PasswordHash);

            // Relationships
            this.HasRequired(p => p.Role).
               WithMany(x => x.Users)
                //.Map(x => x.MapKey("UserRoleID"));
            .HasForeignKey(x => x.UserRoleID);

            this.HasMany(x => x.Locations)
              .WithMany(x => x.Users)
              .Map(x =>
              {

                  x.MapLeftKey("UserId");
                  x.MapRightKey("LocationId");
                  x.ToTable("UserLocationMap");

              });


        }
    }
}