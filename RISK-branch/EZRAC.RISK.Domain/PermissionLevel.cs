using System;
using System.Collections.Generic;

namespace EZRAC.RISK.Domain
{
    public class PermissionLevel : BaseEntity
    {
        public string LevelName { get; set; }

        public Permission Permission { get; set; }
    }
}
