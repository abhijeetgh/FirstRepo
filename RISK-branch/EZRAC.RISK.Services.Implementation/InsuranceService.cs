using EZRAC.RISK.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using System.Data.Entity;

namespace EZRAC.RISK.Services.Implementation
{
    public class InsuranceService : IInsuranceCompanyService
    {
        IGenericRepository<RiskInsurance> _riskInsuranceRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskDriverInsurance> _riskdriverInsuranceRepository = null;

        public InsuranceService(IGenericRepository<RiskInsurance> riskInsuranceRepository, IGenericRepository<Claim> claimRepository, IGenericRepository<RiskDriverInsurance> riskDriverInsuranceRepository)
        {
            _riskInsuranceRepository = riskInsuranceRepository;
            _claimRepository = claimRepository;
            _riskdriverInsuranceRepository = riskDriverInsuranceRepository;
        }


        public async Task<InsuranceDto> GetInsuranceCompanyDetailById(long id)
        {
            var riskInsurance = await _riskInsuranceRepository.GetByIdAsync(id);
            var insuranceDto = MapInsuranceDto(riskInsurance);
            return insuranceDto;
        }


        public async Task<bool> AddOrUpdateInsuranceCompany(InsuranceDto insuranceDto,long userId)
        {
            var success = false;

            var isDuplicate = IsInsuranceCompanyDuplicate(insuranceDto.Id, insuranceDto.CompanyName);
            if (isDuplicate)
            {
                return false;
            }

            if (insuranceDto.Id != 0)
            {
                var insuranceCompany = await _riskInsuranceRepository.GetByIdAsync(insuranceDto.Id);
                if (insuranceCompany != null)
                {
                    insuranceCompany.Id = insuranceDto.Id;
                    insuranceCompany.CompanyName = insuranceDto.CompanyName;
                    insuranceCompany.Address = insuranceDto.Address;
                    insuranceCompany.City = insuranceDto.City;
                    insuranceCompany.Contact = insuranceDto.Contact;
                    insuranceCompany.Email = insuranceDto.Email;
                    insuranceCompany.Fax = insuranceDto.Fax;
                    insuranceCompany.Notes = insuranceDto.Notes;
                    insuranceCompany.Phone = insuranceDto.Phone;
                    insuranceCompany.State = insuranceDto.State;
                    insuranceCompany.Zip = insuranceDto.Zip;
                    insuranceCompany.UpdatedBy = userId;
                    insuranceCompany.UpdatedDateTime = DateTime.Now;
                    await _riskInsuranceRepository.UpdateAsync(insuranceCompany);
                    success = true;
                }
            }
            else
            {
                 await _riskInsuranceRepository.InsertAsync(new RiskInsurance
                    {
                        Address = insuranceDto.Address,
                        City = insuranceDto.City,
                        CompanyName = insuranceDto.CompanyName,
                        Contact = insuranceDto.Contact,
                        Email = insuranceDto.Email,
                        Fax = insuranceDto.Fax,
                        Notes = insuranceDto.Notes,
                        Phone = insuranceDto.Phone,
                        State = insuranceDto.State,
                        Zip = insuranceDto.Zip,
                        UpdatedBy = userId,
                        UpdatedDateTime = DateTime.Now,
                        CreatedDateTime = DateTime.Now,
                    });
                    success = true;
            }
            return success;
        }

        public async Task<bool> DeleteInsuranceCompany(InsuranceDto insuranceDto, long userId)
        {
            var success = false;
            var isDuplicate = IsInsuranceCompanyAlreadyUsed(insuranceDto.Id);
            if (isDuplicate)
            {
                return false;
            }
            var companyToDelete = await _riskInsuranceRepository.GetByIdAsync(insuranceDto.Id);
            if (companyToDelete != null)
            {
                companyToDelete.IsDeleted = true;
                companyToDelete.UpdatedBy = userId;
                companyToDelete.UpdatedDateTime = DateTime.Now;
                await _riskInsuranceRepository.UpdateAsync(companyToDelete);
                success = true;
            }
            return success;
        }
        public bool IsInsuranceCompanyAlreadyUsed(long id)
        {
            //var isDuplicate = _riskInsuranceRepository.AsQueryable.Include(x => x.RiskDriverInsurances.Where(y => y.InsuranceId == x.Id)).Any();
            var isDuplicate = _riskdriverInsuranceRepository.AsQueryable.Where(x => x.InsuranceId == id).Any();
            return isDuplicate;
        }


        #region Private Methods
        private bool IsInsuranceCompanyDuplicate(long id, string name)
        {
            var exist = _riskInsuranceRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.CompanyName == name).Any();
            return exist;
        }

        private InsuranceDto MapInsuranceDto(RiskInsurance riskInsurance)
        {
            var insuranceDto = new InsuranceDto();
            insuranceDto.Id = riskInsurance.Id;
            insuranceDto.CompanyName = riskInsurance.CompanyName;
            insuranceDto.Address = riskInsurance.Address;
            insuranceDto.City = riskInsurance.City;
            insuranceDto.Contact = riskInsurance.Contact;
            insuranceDto.Email = riskInsurance.Email;
            insuranceDto.Fax = riskInsurance.Fax;
            insuranceDto.Notes = riskInsurance.Notes;
            insuranceDto.Phone = riskInsurance.Phone;
            insuranceDto.State = riskInsurance.State;
            insuranceDto.Zip = riskInsurance.Zip;
            return insuranceDto;
        } 
        #endregion
    }
}
