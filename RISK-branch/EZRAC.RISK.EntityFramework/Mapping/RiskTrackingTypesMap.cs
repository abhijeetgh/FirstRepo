﻿using EZRAC.RISK.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.EntityFramework.Mapping
{
    class RiskTrackingTypesMap : EntityTypeConfiguration<RiskTrackingTypes>
    {

        public RiskTrackingTypesMap()
        {
            
            this.ToTable("RiskTrackingTypes");

            this.HasRequired(p => p.Permissions)
                .WithMany()
                .HasForeignKey(x => x.PermissionId);
        }
    }
}
