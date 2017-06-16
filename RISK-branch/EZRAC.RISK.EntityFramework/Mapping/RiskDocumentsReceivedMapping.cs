using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskDocumentsReceivedMapping : EntityTypeConfiguration<RiskDocumentsReceived>
    {
        public RiskDocumentsReceivedMapping() {


            this.ToTable("RiskDocumentsReceived");

            this.HasKey(x => x.ClaimId);

            this.HasRequired(x => x.Claim)
                .WithOptional(x => x.RiskDocumentsReceived);
        
        }
    }
}
