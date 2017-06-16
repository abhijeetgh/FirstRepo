using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskPaymentMapping : EntityTypeConfiguration<RiskPayment>
    {
        public RiskPaymentMapping()
        {
            this.ToTable("RiskPayments");

            this.HasRequired(s => s.RiskPaymentType)
                .WithMany()
              .HasForeignKey(s => s.PaymentTypeId);

            this.HasOptional(s=>s.RiskPaymentReason)
                .WithMany()
              .HasForeignKey(s => s.ReasonId);

        }
    }
}
