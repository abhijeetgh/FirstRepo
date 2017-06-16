using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    class RiskNoteTypesMap : EntityTypeConfiguration<RiskNoteTypes>
    {
        public RiskNoteTypesMap() {
            this.ToTable("RiskNoteTypes");
        }
    }
}
