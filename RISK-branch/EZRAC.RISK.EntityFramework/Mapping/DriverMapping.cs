using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class DriverMapping : EntityTypeConfiguration<RiskDriver>
    {

        public DriverMapping() {

            ToTable("RiskDriver");

            this.HasKey(p => p.Id);

            this.HasOptional(f => f.RiskDriverInsurance)
            .WithRequired(s => s.RiskDriver);



        
        }
    }
}
