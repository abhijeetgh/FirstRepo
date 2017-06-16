using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public interface IFormulaService : IBaseService<Formula>
    {
        FormulaDTO GetFormulaByLocation(long locationID);
        FormulaDTO GetTetherFormula(long locationID, long dependantBrandID);
        List<CompanyDTO> GetCompany();
        List<FormulaDTO> GetAllFormulas(long brandID);
        void SaveFormulas(FormulaDTODetails formulaDetails);
    }
}
