using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    public class RiskNoteMapping : EntityTypeConfiguration<RiskNote>
    {

        public RiskNoteMapping()
        {

            this.ToTable("RiskNotes");
        }
    }
}
