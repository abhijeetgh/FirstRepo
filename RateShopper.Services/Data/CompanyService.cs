using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Domain.DTOs;
using RateShopper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Data
{
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        public CompanyService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<Company>();
            _cacheManager = cacheManager;
        }

        public List<CompanyDTO> GetAllCompanies()
        {
            return GetAll().Where(d => !d.IsDeleted).Select(d => new CompanyDTO { ID = d.ID, Code = d.Code, Name = d.Name }).OrderBy(d => d.ID).ToList();
        }

        public CompanyDTO GetCompanyDetails(long companyID)
        {
            Company companyEntity = GetById(companyID, false);
            if (companyEntity != null)
            {
                return new CompanyDTO()
                {
                    ID = companyEntity.ID,
                    Code = companyEntity.Code,
                    Name = companyEntity.Name,
                    Logo = companyEntity.Logo
                };
            }
            return new CompanyDTO();
        }

        public long SaveCompany(CompanyDTO objCompanyDTO)
        {
            if (objCompanyDTO != null)
            {
                if (objCompanyDTO.ID == 0)
                {
                    Company objExistingCompany = GetAll().Where(obj => obj.Code == objCompanyDTO.Code && !obj.IsDeleted).FirstOrDefault();
                    if (objExistingCompany == null)
                    {
                        Company objCompanyEntity = new Company()
                        {
                            Code = objCompanyDTO.Code,
                            Name = objCompanyDTO.Name,
                            IsDeleted = false,
                            Logo = objCompanyDTO.Logo,
                            UpdatedBy = objCompanyDTO.CreatedBy,
                            CreatedBy = objCompanyDTO.CreatedBy,
                            UpdatedDateTime = DateTime.Now,
                            CreatedDateTime = DateTime.Now
                        };

                        Add(objCompanyEntity);
                        return objCompanyEntity.ID;
                    }
                    return objCompanyDTO.ID;
                }
                else
                {
                    //Check for duplication of code
                    if (GetAll().Where(obj => obj.Code == objCompanyDTO.Code && !obj.IsDeleted && obj.ID != objCompanyDTO.ID).FirstOrDefault() == null)
                    {
                        Company objCompanyEntity = GetById(objCompanyDTO.ID, false);
                        if (objCompanyEntity != null)
                        {
                            objCompanyEntity.Code = objCompanyDTO.Code;
                            if (!string.IsNullOrEmpty(objCompanyDTO.Logo))
                            {
                                objCompanyEntity.Logo = objCompanyDTO.Logo;
                            }
                            objCompanyEntity.Name = objCompanyDTO.Name;
                            objCompanyEntity.UpdatedBy = objCompanyDTO.CreatedBy;
                            objCompanyEntity.UpdatedDateTime = DateTime.Now;

                            Update(objCompanyEntity);
                        }
                        return objCompanyEntity.ID;
                    }
                    return 0;
                }
            }
            return 0;
        }

        public void SaveCompanyLogo(long companyID, string logoPath)
        {
            if (companyID > 0 && !string.IsNullOrEmpty(logoPath))
            {
                Company objCompanyEntity = GetById(companyID, false);
                if (objCompanyEntity != null)
                {
                    objCompanyEntity.Logo = logoPath;
                    Update(objCompanyEntity);
                }
            }
        }

        public bool DeleteCompany(long companyID, long userID)
        {
            if (companyID > 0)
            {
                Company objCompanyEntity = GetById(companyID, false);
                if (objCompanyEntity != null)
                {
                    objCompanyEntity.IsDeleted = true;
                    objCompanyEntity.UpdatedBy = userID;
                    objCompanyEntity.UpdatedDateTime = DateTime.Now;
                    Update(objCompanyEntity);
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, long> GetCompaniesDictionary()
        {
            return GetAll(false).Where(company => !company.IsDeleted).Select(obj => new { Code = obj.Code, ID = obj.ID }).ToDictionary(obj => obj.Code, obj => obj.ID);
        }

        public List<CompanyDTO> GetCompanies(string companyIds)
        {
            List<CompanyDTO> filterCompanies = new List<CompanyDTO>();
            if (!string.IsNullOrEmpty(companyIds))
            {
                List<long> filterList = RateShopper.Services.Helper.Common.StringToLongList(companyIds).ToList();
                filterCompanies = (from all in GetAll()
                                   join filter in filterList on all.ID equals filter
                                   where !all.IsDeleted
                                   orderby all.Name
                                   select new CompanyDTO { Name = all.Name, ID = all.ID, Code = all.Code }
                                   ).ToList();                                    
            }
            return filterCompanies;
        }        
    }
}
