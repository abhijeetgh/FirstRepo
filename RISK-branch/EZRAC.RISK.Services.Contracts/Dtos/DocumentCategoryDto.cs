using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DocumentCategoryDto
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public IEnumerable<DocumentTypeDto> DocumentTypes { get; set; }
    }
}
