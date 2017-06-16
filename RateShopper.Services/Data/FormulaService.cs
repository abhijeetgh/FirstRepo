using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class FormulaService : BaseService<Formula>, IFormulaService
    {
        ILocationBrandRentalLengthService _locationBrandRentalLengthService;
        IRentalLengthService _rentalLengthService;

        public FormulaService(IEZRACRateShopperContext context, ICacheManager cacheManager, ILocationBrandRentalLengthService locationBrandRentalLengthService, IRentalLengthService rentalLengthService)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<Formula>();
            _cacheManager = cacheManager;
            _locationBrandRentalLengthService = locationBrandRentalLengthService;
            _rentalLengthService = rentalLengthService;
        }
        public FormulaDTO GetFormulaByLocation(long locationID)
        {
            FormulaDTO formula = new FormulaDTO();

            formula = _context.Formulas.Where(obj => obj.LocationBrandID == locationID).Select(obj => new FormulaDTO
                {
                    LocationBrandID = obj.LocationBrandID,
                    FormulaID = obj.ID,
                    SalesTax = obj.SalesTax,
                    AirportFee = obj.AirportFee,
                    Arena = obj.Arena,
                    Surcharge = obj.Surcharge,
                    VLRF = obj.VLRF,
                    CFC = obj.CFC,
                    TotalCostToBase = obj.TotalCostToBase,
                    BaseToTotalCost = obj.BaseToTotalCost,
                    UpdatedBy = obj.UpdatedBy
                }).FirstOrDefault();

            formula.RentalLength = (from rentalLength in _context.LocationBrandRentalLength
                                    join LOR in _context.RentalLengths on rentalLength.RentalLengthID equals LOR.ID
                                    where rentalLength.LocationBrandID == locationID
                                    orderby LOR.ID
                                    select new RentalLengthDTO
                                    {
                                        Code = LOR.Code,
                                        AssociateMappedId = LOR.AssociateMappedId,
                                        MappedID = LOR.MappedID
                                    }).ToList();

            return formula;// _context.Formulas.Where(obj => obj.LocationBrandID == locationID).FirstOrDefault();
        }

        /// <summary>
        /// Get all companies/brands
        /// </summary>
        /// <returns></returns>
        public List<CompanyDTO> GetCompany()
        {
            List<CompanyDTO> companies = _context.Companies.Where(obj => obj.IsBrand && !obj.IsDeleted).Select(obj => new CompanyDTO { ID = obj.ID, Name = obj.Name, Code = obj.Code }).ToList();
            return companies;
        }

        /// <summary>
        /// Get list of all formulas for different locations exists in provided brand
        /// </summary>
        /// <param name="brandID"></param>
        /// <returns></returns>
        public List<FormulaDTO> GetAllFormulas(long brandID)
        {
            List<FormulaDTO> lstFormulas = (from locationBrand in _context.LocationBrands
                                            join formula in _context.Formulas on locationBrand.ID equals formula.LocationBrandID
                                            into fulltable
                                            from formula in fulltable.DefaultIfEmpty()
                                            where locationBrand.BrandID == brandID && locationBrand.IsDeleted == false
                                            orderby locationBrand.LocationBrandAlias
                                            select new FormulaDTO
                                            {
                                                FormulaID = formula.ID,
                                                LocationBrandID = locationBrand.ID,
                                                LocationName = locationBrand.LocationBrandAlias,
                                                AirportFee = formula.AirportFee,
                                                Arena = formula.Arena,
                                                CFC = formula.CFC,
                                                SalesTax = formula.SalesTax,
                                                Surcharge = formula.Surcharge,
                                                VLRF = formula.VLRF,
                                                TotalCostToBase = formula.TotalCostToBase,
                                                BaseToTotalCost = formula.BaseToTotalCost
                                            }).ToList();
            return lstFormulas;
        }

        public FormulaDTO GetTetherFormula(long locationID, long dependantBrandID)
        {
            var lid = _context.LocationBrands.Where(a => a.BrandID == dependantBrandID && a.LocationID == locationID && !(a.IsDeleted)).FirstOrDefault();
            FormulaDTO formula = new FormulaDTO();
            if (lid != null && lid.ID > 0)
            {
                formula = _context.Formulas.Where(obj => obj.LocationBrandID == lid.ID).Select(obj => new FormulaDTO
                {
                    LocationBrandID = obj.LocationBrandID,
                    FormulaID = obj.ID,
                    SalesTax = obj.SalesTax,
                    AirportFee = obj.AirportFee,
                    Arena = obj.Arena,
                    Surcharge = obj.Surcharge,
                    VLRF = obj.VLRF,
                    CFC = obj.CFC,
                    TotalCostToBase = obj.TotalCostToBase,
                    BaseToTotalCost = obj.BaseToTotalCost,
                    UpdatedBy = obj.UpdatedBy
                }).FirstOrDefault();
            }
            return formula;
        }

        /// <summary>
        /// Save the formula details for different brand locations
        /// </summary>
        /// <param name="formulaDetails"></param>
        public void SaveFormulas(FormulaDTODetails formulaDetails)
        {
            formulaDetails.LstFormulaDTO.ForEach(formula =>
            {
                if (formula.FormulaID.HasValue)
                {
                    Formula objFormulaEntity = _context.Formulas.Find(formula.FormulaID);
                    if (objFormulaEntity != null)
                    {
                        if (!formula.AirportFee.HasValue && !formula.Arena.HasValue && !formula.CFC.HasValue && !formula.SalesTax.HasValue &&
                        !formula.Surcharge.HasValue && !formula.VLRF.HasValue && string.IsNullOrEmpty(formula.TotalCostToBase) && string.IsNullOrEmpty(formula.BaseToTotalCost))
                        {
                            Delete(objFormulaEntity);
                        }
                        else
                        {
                            objFormulaEntity.AirportFee = formula.AirportFee;
                            objFormulaEntity.Arena = formula.Arena;
                            objFormulaEntity.CFC = formula.CFC;
                            objFormulaEntity.BaseToTotalCost = formula.BaseToTotalCost.Trim();
                            objFormulaEntity.SalesTax = formula.SalesTax;
                            objFormulaEntity.Surcharge = formula.Surcharge;
                            objFormulaEntity.TotalCostToBase = formula.TotalCostToBase.Trim();
                            objFormulaEntity.VLRF = formula.VLRF;
                            objFormulaEntity.UpdatedBy = formulaDetails.UserID;
                            objFormulaEntity.UpdatedDateTime = DateTime.Now;

                            Update(objFormulaEntity);
                        }
                    }
                }
                else
                {
                    if (formula.AirportFee.HasValue || formula.Arena.HasValue || formula.CFC.HasValue || formula.SalesTax.HasValue ||
                        formula.Surcharge.HasValue || formula.VLRF.HasValue || !string.IsNullOrEmpty(formula.TotalCostToBase) || !string.IsNullOrEmpty(formula.BaseToTotalCost))
                    {
                        Formula objFormulaEntity = new Formula();
                        objFormulaEntity.LocationBrandID = formula.LocationBrandID;
                        objFormulaEntity.AirportFee = formula.AirportFee;
                        objFormulaEntity.Arena = formula.Arena;
                        objFormulaEntity.CFC = formula.CFC;
                        objFormulaEntity.BaseToTotalCost = formula.BaseToTotalCost.Trim();
                        objFormulaEntity.SalesTax = formula.SalesTax;
                        objFormulaEntity.Surcharge = formula.Surcharge;
                        objFormulaEntity.TotalCostToBase = formula.TotalCostToBase.Trim();
                        objFormulaEntity.VLRF = formula.VLRF;
                        objFormulaEntity.UpdatedBy = formulaDetails.UserID;
                        objFormulaEntity.CreatedBy = formulaDetails.UserID;
                        objFormulaEntity.UpdatedDateTime = DateTime.Now;
                        objFormulaEntity.CreatedDateTime = DateTime.Now;

                        Add(objFormulaEntity);
                    }
                }
            });
        }
    }
}
