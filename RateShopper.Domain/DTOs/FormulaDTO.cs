using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Domain.DTOs
{
    public class FormulaDTO
    {
        public string LocationName { get; set; }
        public long LocationBrandID { get; set; }
        public Nullable<long> FormulaID { get; set; }
        public Nullable<decimal> SalesTax { get; set; }
        public Nullable<decimal> AirportFee { get; set; }
        public Nullable<decimal> Arena { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public Nullable<decimal> VLRF { get; set; }
        public Nullable<decimal> CFC { get; set; }
        public bool IsEdited { get; set; }
        public long UpdatedBy { get; set; }
        public string TotalCostToBase { get; set; }
        public string BaseToTotalCost { get; set; }
        public IEnumerable<RentalLengthDTO> RentalLength { get; set; }
    }

    public class FormulaDTODetails
    {
        public FormulaDTODetails()
        {
            LstFormulaDTO = new List<FormulaDTO>();
        }

        public long UserID { get; set; }
        public long BrandID { get; set; }
        public List<FormulaDTO> LstFormulaDTO { get; set; }
    }
}
