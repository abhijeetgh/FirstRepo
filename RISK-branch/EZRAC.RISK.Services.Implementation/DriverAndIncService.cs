using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class DriverService : IDriverAndIncService
    {

        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskDriver> _driverRepository = null;
        IGenericRepository<RiskInsurance> _insuranceRepository = null;

        public DriverService(IGenericRepository<Claim> claimRepository,
                                                    IGenericRepository<RiskDriver> driverRepository,
                                                    IGenericRepository<RiskInsurance> insuranceRepository)
        {
            _claimRepository = claimRepository;
            _driverRepository = driverRepository;
            _insuranceRepository = insuranceRepository;


        }


        #region Interface Implementation

        /// <summary>
        /// This method is used to get all the drivers information by ClaimId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DriverInfoDto>> GetDriverListByClaimIdAsync(long Id)
        {
            List<DriverInfoDto> driverAndIncInfoDtoList = null;

            Claim claim = await _claimRepository.AsQueryable.Include(x => x.RiskDrivers.Select(d => d.RiskDriverInsurance))
                                    .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (claim != null)
            {
                driverAndIncInfoDtoList = new List<DriverInfoDto>();
              
                driverAndIncInfoDtoList= claim.RiskDrivers.Select(driver=>
                                            MapDriverAndIncInfoDto(driver)).ToList();
            }           

            return driverAndIncInfoDtoList;
        }

        /// <summary>
        /// This method is to get single driver information by Driver Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<DriverInfoDto> GetDriverByIdAsync(long Id)
        {
            RiskDriver driver = await _driverRepository.AsQueryable.Include(d => d.RiskDriverInsurance).Where(d => d.Id == Id).FirstOrDefaultAsync();

            DriverInfoDto driverAndIncInfoDto = MapDriverAndIncInfoDto(driver);

            return driverAndIncInfoDto;
        }

        /// <summary>
        /// This method is to get single driver information by Driver Type Id
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DriverInfoDto>> GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes type,long claimId)
        {
            List<RiskDriver> riskDrivers = await _driverRepository.AsQueryable.Include(d => d.RiskDriverInsurance).Where(d => d.DriverTypeId == (int)type && d.ClaimId == claimId).ToListAsync();

            List<DriverInfoDto> driversDto = new List<DriverInfoDto>();

            foreach (var driver in riskDrivers)
            {
                driversDto.Add(MapDriverAndIncInfoDto(driver));
            }

            return driversDto;
        }

        /// <summary>
        /// This method is to update drivers information
        /// </summary>
        /// <param name="driverAndIncInfoDto"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public async Task<Nullable<Int64>> UpdateDriverAsync(DriverInfoDto driverAndIncInfoDto, long claimId)
        {
            Nullable<Int64> driverId = null;
            if (driverAndIncInfoDto != null && driverAndIncInfoDto.Id != 0)
            {

                RiskDriver driver = await _driverRepository.AsQueryable.Include(d => d.RiskDriverInsurance).Where(d => d.Id == driverAndIncInfoDto.Id).FirstOrDefaultAsync();

                driver = await MapRiskDriverAndIncInfo(driverAndIncInfoDto, driver);

                await _driverRepository.UpdateAsync(driver);

                driverId = driver.Id;

            }
            else {

                if(driverAndIncInfoDto!= null && claimId != 0) {

                    Claim claim = await _claimRepository.AsQueryable.Include(x => x.RiskDrivers).Where(x => x.Id == claimId).FirstOrDefaultAsync();

                    if (claim != null && claim.RiskDrivers != null)
                    {
                        //IList<RiskDriver> riskdrivers = new List<RiskDriver>();

                        RiskDriver driver = new RiskDriver();

                        claim.RiskDrivers.Add(await MapRiskDriverAndIncInfo(driverAndIncInfoDto, driver));

                        await _claimRepository.UpdateAsync(claim);

                        driverId = driver != null ? driver.Id : default(Int64);
                    }
                    
                }

            }

            return driverId;
        }

        #endregion
        #region Private Methods
        private DriverInfoDto MapDriverAndIncInfoDto(RiskDriver riskDriver)
        {

            DriverInfoDto driverAndIncInfoDto = null;

            if (riskDriver != null)
            {
                driverAndIncInfoDto = new DriverInfoDto();
                driverAndIncInfoDto.Id = riskDriver.Id;
                driverAndIncInfoDto.FirstName = riskDriver.FirstName;
                driverAndIncInfoDto.LastName = riskDriver.LastName;
                driverAndIncInfoDto.Address1 = riskDriver.Address;
                driverAndIncInfoDto.Address2 = riskDriver.Address2;
                driverAndIncInfoDto.City = riskDriver.City;
                driverAndIncInfoDto.State = riskDriver.State;
                driverAndIncInfoDto.Zip = riskDriver.Zip;
                driverAndIncInfoDto.Phone1 = riskDriver.Phone1;
                driverAndIncInfoDto.Phone2 = riskDriver.Phone2;
                driverAndIncInfoDto.Email = riskDriver.Email;
                driverAndIncInfoDto.Fax = riskDriver.Fax;
                driverAndIncInfoDto.OtherContact = riskDriver.OtherContact;

                driverAndIncInfoDto.DOB = riskDriver.DOB;
                driverAndIncInfoDto.LicenceExpiry = riskDriver.LicenceExpiryDate;
                driverAndIncInfoDto.LicenceNumber = riskDriver.LicenceNumber;
                driverAndIncInfoDto.LicenceState = riskDriver.LicenceState;
                driverAndIncInfoDto.IsAuthorizedDriver = riskDriver.IsAuthorized;


                driverAndIncInfoDto.DriverTypeId = riskDriver.DriverTypeId;

                if (riskDriver.RiskDriverInsurance != null)
                {

                    driverAndIncInfoDto.InsuranceId = riskDriver.RiskDriverInsurance.InsuranceId;
                    driverAndIncInfoDto.InsuranceCompanyName = riskDriver.RiskDriverInsurance.InsuranceCompanyName;
                    driverAndIncInfoDto.PolicyNumber = riskDriver.RiskDriverInsurance.PolicyNumber;
                    driverAndIncInfoDto.Deductible = riskDriver.RiskDriverInsurance.Deductible;
                    driverAndIncInfoDto.InsuranceClaimNumber = riskDriver.RiskDriverInsurance.InsuranceClaimNumber;
                    driverAndIncInfoDto.InsuranceExpiry = riskDriver.RiskDriverInsurance.InsuranceExpiry;
                    driverAndIncInfoDto.CreditCardCompany = riskDriver.RiskDriverInsurance.CreditCardCompany;
                    driverAndIncInfoDto.CreditCardPolicyNumber = riskDriver.RiskDriverInsurance.CreditCardPolicyNumber;
                    driverAndIncInfoDto.CreditCardCoverageAmount = riskDriver.RiskDriverInsurance.CreditCardCoverageAmt;

                }
            }

            return driverAndIncInfoDto;
        }

        private async Task<RiskDriver> MapRiskDriverAndIncInfo(DriverInfoDto driverAndIncInfoDto, RiskDriver riskDriver)
        {

            if (riskDriver != null & driverAndIncInfoDto != null)
            {

                riskDriver.FirstName = driverAndIncInfoDto.FirstName;
                riskDriver.LastName = driverAndIncInfoDto.LastName;
                riskDriver.Address = driverAndIncInfoDto.Address1;
                riskDriver.Address2 = driverAndIncInfoDto.Address2;
                riskDriver.City = driverAndIncInfoDto.City;
                riskDriver.State = driverAndIncInfoDto.State;
                riskDriver.Zip = driverAndIncInfoDto.Zip;
                riskDriver.Phone1 = driverAndIncInfoDto.Phone1;
                riskDriver.Phone2 = driverAndIncInfoDto.Phone2;
                riskDriver.Email = driverAndIncInfoDto.Email;
                riskDriver.Fax = driverAndIncInfoDto.Fax;
                riskDriver.OtherContact = driverAndIncInfoDto.OtherContact;
                riskDriver.IsAuthorized = driverAndIncInfoDto.IsAuthorizedDriver;
                riskDriver.DOB = driverAndIncInfoDto.DOB;
                riskDriver.LicenceExpiryDate = driverAndIncInfoDto.LicenceExpiry;
                riskDriver.LicenceNumber = driverAndIncInfoDto.LicenceNumber;
                riskDriver.LicenceState = driverAndIncInfoDto.LicenceState;


                riskDriver.DriverTypeId = driverAndIncInfoDto.DriverTypeId;

                if (driverAndIncInfoDto.InsuranceId.HasValue)
                {
                    riskDriver.RiskDriverInsurance = riskDriver.RiskDriverInsurance != null ? riskDriver.RiskDriverInsurance : new RiskDriverInsurance();


                    riskDriver.RiskDriverInsurance.InsuranceId = driverAndIncInfoDto.InsuranceId;

                    if (riskDriver.RiskDriverInsurance.InsuranceId.HasValue)
                    {
                        RiskInsurance riskInsurance = await _insuranceRepository.GetByIdAsync(riskDriver.RiskDriverInsurance.InsuranceId.Value);
                        riskDriver.RiskDriverInsurance.InsuranceCompanyName = riskInsurance != null ? riskInsurance.CompanyName : String.Empty;
                    }
                    else
                    {
                        riskDriver.RiskDriverInsurance.InsuranceCompanyName = String.Empty;
                    }


                    riskDriver.RiskDriverInsurance.PolicyNumber = driverAndIncInfoDto.PolicyNumber;
                    riskDriver.RiskDriverInsurance.Deductible = driverAndIncInfoDto.Deductible;
                    riskDriver.RiskDriverInsurance.InsuranceClaimNumber = driverAndIncInfoDto.InsuranceClaimNumber;
                    riskDriver.RiskDriverInsurance.InsuranceExpiry = driverAndIncInfoDto.InsuranceExpiry;
                    riskDriver.RiskDriverInsurance.CreditCardCompany = driverAndIncInfoDto.CreditCardCompany;
                    riskDriver.RiskDriverInsurance.CreditCardPolicyNumber = driverAndIncInfoDto.CreditCardPolicyNumber;
                    riskDriver.RiskDriverInsurance.CreditCardCoverageAmt = driverAndIncInfoDto.CreditCardCoverageAmount;
                }

            }

            return riskDriver;
        }
        #endregion


       
    }
}
