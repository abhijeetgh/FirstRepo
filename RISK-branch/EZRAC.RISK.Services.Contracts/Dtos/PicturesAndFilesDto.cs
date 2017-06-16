using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class PicturesAndFilesDto
    {
        public string ClaimId { get; set; }
        public string CategoryName { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath{ get; set; }        
        public RiskFilesCategory RiskFilesCategory { get; set; }
        public int CategoryId { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum RiskFilesCategory
    {
        Pictures =1,
        Legal =2,
        Claims =3,
        Salvage=4,
        Impound=5,
        Police=6
    }  

    public class FileCategoriesDto
    {
        public int Id { get; set; }
        public string Description { get; set; }        
    }
}

