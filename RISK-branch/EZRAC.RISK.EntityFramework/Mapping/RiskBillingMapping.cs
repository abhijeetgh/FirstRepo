using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskBillingMapping : EntityTypeConfiguration<RiskBilling>
    {

        public RiskBillingMapping() {

           // this.ToTable("RiskBilling");

            this.HasRequired(p => p.RiskBillingType)
                .WithMany(x => x.RiskBillings)
                .HasForeignKey(x => x.BillingTypeId);

           

        
        }
    }
}
