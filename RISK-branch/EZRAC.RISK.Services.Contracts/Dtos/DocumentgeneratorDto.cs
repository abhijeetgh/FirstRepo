﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class DocumentgeneratorDto
    {
        public IEnumerable<DocumentCategoryDto> CategoryList { get; set; }
    }
}
