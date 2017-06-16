using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class VehicleMapping : EntityTypeConfiguration<RiskVehicle>
    {
        public VehicleMapping()
        {
            ToTable("RiskVehicle");

            this.Ignore(x => x.PurchaseType);
            
            
        }
    }
}
