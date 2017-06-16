using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskDriverInsuranceMap : EntityTypeConfiguration<RiskDriverInsurance>
    {
        public RiskDriverInsuranceMap() {

            this.ToTable("RiskDriverInsurance");

            this.HasRequired(x => x.RiskInsurance)
                .WithMany(x => x.RiskDriverInsurances)
                .HasForeignKey(x => x.InsuranceId);
        
        }

    }
}
