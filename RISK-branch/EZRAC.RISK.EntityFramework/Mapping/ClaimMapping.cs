using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class ClaimMapping : EntityTypeConfiguration<Claim>
    {

        public ClaimMapping()
        {
            this.ToTable("Claims");
          
           
            // Relationships
            this.HasRequired(p => p.RiskLossType)
                .WithMany(p => p.Claims)
                .HasForeignKey(p => p.LossTypeId);

            
            this.HasMany(p => p.RiskDrivers)
                .WithRequired(p => p.Claim)
                .HasForeignKey(p => p.ClaimId);
            

            this.HasOptional(p => p.RiskVehicle)
                .WithMany(p => p.Claims)
                .HasForeignKey(p => p.VehicleId);

            this.HasOptional(p => p.RiskContract)
               .WithMany(x => x.Claims)
               .HasForeignKey(x => x.ContractId);

            this.HasOptional(p => p.RiskNonContract)
              .WithMany(x => x.Claims)
              .HasForeignKey(x => x.NonContractId);

            this.HasOptional(p => p.OpenLocation)
             .WithMany(x => x.OpenLocationClaims)
           .HasForeignKey(x => x.OpenLocationId);
                     
            this.HasOptional(p => p.CloseLocation)
             .WithMany(x => x.CloseLocationClaims)
            .HasForeignKey(x => x.CloseLocationId);

            this.HasOptional(p => p.RiskIncident)
                .WithRequired(p => p.Claim);

            this.HasRequired(p => p.RiskClaimStatus)
                .WithMany(x=>x.Claims)
                .HasForeignKey(x => x.ClaimStatusId);

            this.HasRequired(p => p.AssignedUser)
               .WithMany(x => x.Claims)
               .HasForeignKey(x => x.AssignedTo);


            this.HasMany(s => s.RiskClaimApprovals)
                      .WithRequired(s => s.Claim)
                      .HasForeignKey(s => s.ClaimId);

            this.HasMany(s => s.RiskNotes)
                .WithRequired(s => s.Claim)
                .HasForeignKey(s => s.ClaimId);

            this.HasMany(s => s.RiskDamages)
                .WithRequired(s => s.Claim)
                .HasForeignKey(s => s.ClaimId);

            this.HasMany(c => c.RiskBillings)
                .WithRequired(c => c.Claim)
                .HasForeignKey(c => c.ClaimId);

            this.HasMany(s => s.RiskPayments)
            .WithRequired(s => s.Claim)
            .HasForeignKey(s => s.ClaimId);

            this.HasMany(s => s.RiskWriteOffs)
           .WithRequired(s => s.Claim)
           .HasForeignKey(s => s.ClaimId);
               
        }
    }
}
