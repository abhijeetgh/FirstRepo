using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DocumentTypeDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long Category { get; set; }

    }
}
