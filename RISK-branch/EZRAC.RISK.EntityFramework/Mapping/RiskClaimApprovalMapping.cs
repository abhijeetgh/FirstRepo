using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    class RiskClaimApprovalMapping : EntityTypeConfiguration<RiskClaimApproval>
    {

        public RiskClaimApprovalMapping(){

            this.ToTable("RiskClaimApproval");

            this.HasRequired(s => s.Claim)
                        .WithMany(s => s.RiskClaimApprovals)
                        .HasForeignKey(s => s.ClaimId);

            this.HasRequired(s => s.ClaimStatus)
                        .WithMany()
                        .HasForeignKey(s => s.ClaimStatusId);

            this.HasRequired(s => s.RequestedUser)
                        .WithMany()
                        .HasForeignKey(s => s.RequestedUserId);

            this.HasRequired(s => s.Approver)
                  .WithMany()
                  .HasForeignKey(s => s.ApproverId);

          

            
        }

        
    }
}
