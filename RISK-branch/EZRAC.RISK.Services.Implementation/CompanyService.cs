using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class CompanyService : ICompanyService
    {
        IGenericRepository<Company> _companyRepository = null;
        IGenericRepository<Location> _locationRepository = null;
        public CompanyService(IGenericRepository<Company> companyRepository, IGenericRepository<Location> locationRepository)
        {
            _companyRepository = companyRepository;
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<CompanyDto>> GetCompanyListAsync()
        {

            var companyList = _companyRepository.AsQueryable.Where(x => x.IsDeleted == false).ToList();


            IEnumerable<CompanyDto> CompanyDtos = companyList.Select(x => new CompanyDto
            {
                Id = x.Id,
                Abbr = x.Abbr,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Zip = x.Zip,
                Phone = x.Phone,
                Fax = x.Fax,
                Website = x.Website,
                Zurich = x.Zurich
            }).ToList<CompanyDto>();

            return CompanyDtos;
        }
        public async Task<CompanyDto> GetCompanyById(long companyId)
        {
            var predicate = PredicateBuilder.True<Company>();
            predicate = predicate.And(x => x.IsDeleted == false && x.Id == companyId);
            CompanyDto companyDto = new CompanyDto();
            companyDto = await _companyRepository.AsQueryable.Where(predicate).Select(x => new CompanyDto
            {
                Id = x.Id,
                Abbr = x.Abbr,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Zip = x.Zip,
                Phone = x.Phone,
                Fax = x.Fax,
                Website = x.Website,
                Zurich = x.Zurich
            }).SingleOrDefaultAsync();

            return companyDto;
        }
        public async Task<bool> AddCompanyAsync(CompanyDto companyDto)
        {
            bool flag = false;
            Company company = new Company();
            if (companyDto != null)
            {
                company = GetCompanyDomain(companyDto, company, true);

                await _companyRepository.InsertAsync(company);

                flag = true;
            }
            return flag;
        }
        public async Task<bool> UpdateCompanyAsync(CompanyDto companyDto)
        {
            bool flag = false;
            Company company = new Company();
            if (companyDto != null)
            {
                company = await _companyRepository.GetByIdAsync(companyDto.Id);
                company = GetCompanyDomain(companyDto, company, false);

                await _companyRepository.UpdateAsync(company);

                var locationcompany = await _locationRepository.AsQueryable.Where(x => x.CompanyId == companyDto.Id).ToListAsync();
                if (locationcompany.Any())
                {
                    List<Location> locations = new List<Location>();
                    foreach (var item in locationcompany)
                    {
                        item.CompanyAbbr = companyDto.Abbr;
                        locations.Add(item);
                    }
                    await _locationRepository.BulkUpdateAsync(locations);
                }
                flag = true;
            }
            return flag;
        }
        public async Task<bool> DeleteCompanyAsync(long companyId)
        {
            bool flag = false;
            var isAlredyUsed = IsCompanyAlreadyUsed(companyId);
            if (isAlredyUsed)
            {
                return flag;
            }
            Company company = new Company();
            company = await _companyRepository.GetByIdAsync(companyId);
            if (company != null)
            {
                company.IsDeleted = true;
                await _companyRepository.UpdateAsync(company);
                flag = true;
            }

            return flag;
        }
        public async Task<bool> IsCompanyMapped(long companyId)
        {
            bool flag = false;
            var isAlredyUsed = IsCompanyAlreadyUsed(companyId);
            if (isAlredyUsed)
            {
                flag = true;
                return flag;
            }
            return flag;
        }
        #region private methods
        private Company GetCompanyDomain(CompanyDto companyDto, Company company, bool isCreate)
        {
            if (companyDto != null && company != null)
            {
                company.Id = companyDto.Id;
                company.Abbr = companyDto.Abbr.Trim();
                company.Name = companyDto.Name.Trim();
                company.Address = companyDto.Address;
                company.City = companyDto.City;
                company.State = companyDto.State;
                company.Zip = companyDto.Zip;
                company.Phone = companyDto.Phone;
                company.Fax = companyDto.Fax;
                company.Website = companyDto.Website;
                //company.Zurich = companyDto.Zurich;
                if (isCreate)
                {
                    company.CreatedBy = companyDto.CurrentUserId;
                    company.CreatedDateTime = DateTime.Now;
                    company.IsDeleted = false;
                }
                company.UpdatedBy = companyDto.CurrentUserId;
                company.UpdatedDateTime = DateTime.Now;
            }
            return company;
        }
        private bool IsCompanyAlreadyUsed(long companyId)
        {
            var IsCompanyExist = _locationRepository.AsQueryable.Where(x => x.CompanyId == companyId && !x.IsDeleted).Any();
            return IsCompanyExist;
        }
        #endregion
    }
}
