using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class LookupService : ILookUpService
    {

        IGenericRepository<Location> _locationRepository = null;
        IGenericRepository<User> _userRepository = null;
        IGenericRepository<RiskLossType> _lossTypeRepository = null;
        IGenericRepository<RiskClaimStatus> _claimStatusesRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskPoliceAgency> _policeAgencyRepository = null;
        IGenericRepository<RiskNoteTypes> _notesTypeRepository = null;
        IGenericRepository<RiskDamageType> _riskDamageTypesRepository = null;
        IGenericRepository<RiskInsurance> _riskInsuranceRepository = null;
        IGenericRepository<RiskFileTypes> _riskFileTypeRepository = null;
        IGenericRepository<RiskBillingType> _riskBillingTypeRepository = null;
        IGenericRepository<Company> _riskCompanyRepository = null;
        IGenericRepository<RiskPaymentType> _riskPaymentTypeRepository = null;
        IGenericRepository<RiskWriteOffType> _riskWriteOffTypeRepository = null;

        public LookupService(
            IGenericRepository<Location> locationRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<RiskLossType> lossTypeRepository,
            IGenericRepository<RiskClaimStatus> claimStatusesRepository,
            IGenericRepository<RiskPoliceAgency> policeAgencyRepository,
            IGenericRepository<Claim> claimRepository,
            IGenericRepository<RiskNoteTypes> notesTypeRepository,
            IGenericRepository<RiskInsurance> riskInsuranceRepository,
            IGenericRepository<RiskDamageType> riskDamageTypesRepository,
            IGenericRepository<RiskFileTypes> riskFileTypesRepository,
            IGenericRepository<RiskBillingType> riskBillingTypeRepository,
            IGenericRepository<Company> riskCompanyRepository,
            IGenericRepository<RiskPaymentType> riskPaymentTypeRepository,
            IGenericRepository<RiskWriteOffType> riskWriteOffTypeRepository)
        {


            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _lossTypeRepository = lossTypeRepository;
            _claimStatusesRepository = claimStatusesRepository;
            _claimRepository = claimRepository;
            _policeAgencyRepository = policeAgencyRepository;
            _notesTypeRepository = notesTypeRepository;
            _riskDamageTypesRepository = riskDamageTypesRepository;
            _riskInsuranceRepository = riskInsuranceRepository;
            _riskFileTypeRepository = riskFileTypesRepository;
            _riskBillingTypeRepository = riskBillingTypeRepository;
            _riskCompanyRepository = riskCompanyRepository;
            _riskPaymentTypeRepository = riskPaymentTypeRepository;
            _riskWriteOffTypeRepository = riskWriteOffTypeRepository;
        }

        /// <summary>
        /// This methods returns all the Locations
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            IEnumerable<Location> locations = await _locationRepository.AsQueryable.AsNoTracking().Where(l => l.IsDeleted == false).ToListAsync().ConfigureAwait(false);

            List<LocationDto> locationsToReturn = new List<LocationDto>();

            LocationDto locationToAdd;

            foreach (var location in locations)
            {
                locationToAdd = new LocationDto();

                locationToAdd.Id = location.Id;
                locationToAdd.Name = location.Name;
                locationToAdd.Code = location.Code;
                locationToAdd.CompanyId = location.CompanyId;
                locationToAdd.CompanyAbbr = location.CompanyAbbr;
                locationToAdd.State = location.State;

                locationsToReturn.Add(locationToAdd);
            }

            return locationsToReturn;
        }

        /// <summary>
        /// This method is used to get All the users
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            IEnumerable<User> users = await _userRepository.AsQueryable.AsNoTracking().Where(x => x.IsActive && !(x.IsDeleted)).ToListAsync().ConfigureAwait(false);

            List<UserDto> usersToReturn = new List<UserDto>();

            UserDto userToAdd;

            foreach (var user in users)
            {
                userToAdd = new UserDto();

                userToAdd.Id = user.Id;
                userToAdd.FirstName = user.FirstName;
                userToAdd.LastName = user.LastName;

                usersToReturn.Add(userToAdd);
            }

            return usersToReturn;
        }

        /// <summary>
        /// This method is used to get User by UserId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserbyIdAsync(Int64 Id)
        {

            User user = await _userRepository.AsQueryable.AsNoTracking().Where(x => x.IsActive && x.Id == Id).FirstOrDefaultAsync();

            UserDto userDto = null;
            if (user != null)
            {
                userDto = new UserDto();

                userDto.Id = user.Id;
                userDto.FirstName = user.FirstName;
                userDto.LastName = user.LastName;
                userDto.Email = user.Email;
            }

            return userDto;
        }

        /// <summary>
        /// This method is used to get All the Loss Types
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LossTypesDto>> GetAllLossTypesAsync()
        {
            IEnumerable<RiskLossType> lossTypes = await _lossTypeRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            List<LossTypesDto> lossTypesToReturn = new List<LossTypesDto>();

            LossTypesDto lossTypeToAdd;
            foreach (var lossType in lossTypes)
            {
                lossTypeToAdd = new LossTypesDto();

                lossTypeToAdd.Id = lossType.Id;
                lossTypeToAdd.Type = lossType.Type;
                lossTypeToAdd.Description = lossType.Description;
                lossTypeToAdd.Status = lossType.Status;
                lossTypesToReturn.Add(lossTypeToAdd);
            }

            return lossTypesToReturn;
        }

        /// <summary>
        /// This method is used to get All the Police Agencies
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PoliceAgencyDto>> GetAllPoliceAgenciesAsync()
        {
            IEnumerable<RiskPoliceAgency> policeAgencies = await _policeAgencyRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            List<PoliceAgencyDto> policeAgenciesToReturn = new List<PoliceAgencyDto>();

            PoliceAgencyDto policeAgenciesToAdd;

            foreach (var policeAgency in policeAgencies)
            {
                policeAgenciesToAdd = new PoliceAgencyDto();

                policeAgenciesToAdd.Id = policeAgency.Id;
                policeAgenciesToAdd.Address = policeAgency.Address;
                policeAgenciesToAdd.AgencyName = policeAgency.AgencyName;
                policeAgenciesToAdd.Email = policeAgency.Email;
                policeAgenciesToReturn.Add(policeAgenciesToAdd);
            }

            return policeAgenciesToReturn;
        }

        /// <summary>
        /// This method is used to get All the Claim Statuses
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ClaimStatusDto>> GetAllClaimStatusesAsync()
        {
            IEnumerable<RiskClaimStatus> claimStatuses = await _claimStatusesRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            List<ClaimStatusDto> claimStatusesToReturn = new List<ClaimStatusDto>();

            ClaimStatusDto claimStatusToAdd;

            foreach (var claimStatus in claimStatuses)
            {
                claimStatusToAdd = new ClaimStatusDto();

                claimStatusToAdd.Id = claimStatus.Id;
                claimStatusToAdd.Description = claimStatus.Description;

                claimStatusesToReturn.Add(claimStatusToAdd);
            }

            return claimStatusesToReturn;
        }

        /// <summary>
        /// This method is used to get Claims by ContractNumber
        /// </summary>
        /// <param name="contractNumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ClaimDto>> GetClaimsByContractNumberOrUnitNumber(string contractNumber, string unitNumber)
        {

            List<ClaimDto> claimDtoList = null;

            List<Claim> claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.OpenLocation,
                                                                               x => x.RiskVehicle,
                                                                               x => x.RiskLossType,
                                                                               x => x.RiskContract)
                                                                               .Where(c => c.RiskContract.ContractNumber == contractNumber && c.ClaimStatusId != Constants.ClaimStatus.Closed && c.IsDeleted != true ||
                                                                                   c.RiskVehicle.UnitNumber == unitNumber &&
                                                                              c.ClaimStatusId != Constants.ClaimStatus.Closed && c.IsDeleted != true).ToListAsync();


            if (claimList != null && claimList.Any())
            {
                claimDtoList = claimList.Select(
                   x => new ClaimDto
                   {
                       Id = x.Id,
                       SelectedLossTypeDescription = x.RiskLossType.Description,
                       CompanyAbbr = x.OpenLocation != null ? x.OpenLocation.CompanyAbbr : String.Empty,
                       ContractNo = x.RiskContract != null ? x.RiskContract.ContractNumber : String.Empty
                   }).ToList();
            }


            return claimDtoList;
        }

        /// <summary>
        /// This method is used to get User by Role Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int Id)
        {
            IEnumerable<User> users = await _userRepository.AsQueryable.AsNoTracking().Where(u => u.UserRoleID == Id && u.IsActive && !u.IsDeleted).ToListAsync();

            List<UserDto> usersToReturn = new List<UserDto>();

            UserDto userToAdd;

            foreach (var user in users)
            {
                userToAdd = new UserDto();

                userToAdd.Id = user.Id;
                userToAdd.FirstName = user.FirstName;
                userToAdd.LastName = user.LastName;

                usersToReturn.Add(userToAdd);
            }

            return usersToReturn;
        }

        /// <summary>
        /// This method is used to get the list of AssignedTo users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserDto>> GetAssignToUsersAsync()
        {

            List<UserDto> usersToReturn = new List<UserDto>();

            IEnumerable<User> users = await _userRepository.AsQueryable.Where(u => (u.UserRoleID == EZRAC.RISK.Services.Implementation.Helper.Constants.Roles.RiskAgent ||
                                                                                    u.UserRoleID == EZRAC.RISK.Services.Implementation.Helper.Constants.Roles.RiskManager ||
                                                                                    u.UserRoleID == EZRAC.RISK.Services.Implementation.Helper.Constants.Roles.RiskSupervisor) && u.IsActive && !u.IsDeleted).ToListAsync().ConfigureAwait(false);


            UserDto userToAdd;

            foreach (var user in users)
            {
                userToAdd = new UserDto();

                userToAdd.Id = user.Id;
                userToAdd.FirstName = user.FirstName;
                userToAdd.LastName = user.LastName;

                usersToReturn.Add(userToAdd);
            }

            return usersToReturn;
        }

        /// <summary>
        /// This method is used to the Users by Role Ids
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserDto>> GetUsersByRolesAsync(List<Int64> roleIds)
        {

            List<UserDto> usersToReturn = new List<UserDto>();

            var predicate = PredicateBuilder.False<User>();

            foreach (var roleId in roleIds)
                predicate = predicate.Or(x => x.UserRoleID == roleId);


            IEnumerable<User> users = await _userRepository.AsQueryable.Where(x => x.IsActive && !x.IsDeleted).Where(predicate).ToListAsync();

            UserDto userToAdd;

            foreach (var user in users)
            {
                userToAdd = new UserDto();

                userToAdd.Id = user.Id;
                userToAdd.FirstName = user.FirstName;
                userToAdd.LastName = user.LastName;

                usersToReturn.Add(userToAdd);
            }

            return usersToReturn;


        }

        /// <summary>
        /// This method is used to Get AllNoteTypes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<NotesTypeDto>> GetAllNoteTypesAsync()
        {
            List<NotesTypeDto> notesTypeList = new List<NotesTypeDto>();

            IEnumerable<RiskNoteTypes> notes = await _notesTypeRepository.AsQueryable.AsNoTracking().Where(x => x.Id < 8).ToListAsync().ConfigureAwait(false);

            NotesTypeDto noteType = null;

            foreach (var item in notes)
            {
                noteType = new NotesTypeDto();
                noteType.Id = item.Id;
                noteType.Description = item.Description;

                notesTypeList.Add(noteType);
            }

            return notesTypeList;
        }

        /// <summary>
        /// This method is used to get all DamageTypes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DamageTypesDto>> GetAllDamageTypesAsync()
        {

            List<DamageTypesDto> damageTypeList = new List<DamageTypesDto>();
            IEnumerable<RiskDamageType> damageTypes = await _riskDamageTypesRepository.GetAllAsync().ConfigureAwait(false);
            damageTypes = damageTypes.Where(x => !x.IsDeleted);
            DamageTypesDto damageType = null;

            foreach (var item in damageTypes)
            {
                damageType = new DamageTypesDto();
                damageType.Id = item.Id;
                damageType.Section = item.Section;

                damageTypeList.Add(damageType);
            }

            return damageTypeList;


        }

        /// <summary>
        /// This method is used to get all InsuranceCompanies
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<InsuranceDto>> GetAllInsuranceCompaniesAsync()
        {
            List<InsuranceDto> insuranceDtoList = new List<InsuranceDto>();
            IEnumerable<RiskInsurance> insuranceCompanies = await _riskInsuranceRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            InsuranceDto insuranceDto = null;

            foreach (var item in insuranceCompanies)
            {
                insuranceDto = new InsuranceDto();
                insuranceDto.Id = item.Id;
                insuranceDto.CompanyName = item.CompanyName;
                insuranceDto.Address = item.Address;
                insuranceDto.City = item.City;
                insuranceDto.Contact = item.Contact;
                insuranceDto.Email = item.Email;
                insuranceDto.Fax = item.Fax;
                insuranceDto.Notes = item.Notes;
                insuranceDto.Phone = item.Phone;
                insuranceDto.State = item.State;
                insuranceDto.Zip = item.Zip;
                insuranceDtoList.Add(insuranceDto);
            }

            return insuranceDtoList;
        }

        /// <summary>
        /// This function is used to get all File Types of Categories 
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileCategoriesDto>> GetAllFileCategoriesAsync()
        {
            List<FileCategoriesDto> fileCategoryDtoList = null;
            var fileCategories = await _riskFileTypeRepository.GetAllAsync().ConfigureAwait(false);

            fileCategoryDtoList = fileCategories.Select(x => new FileCategoriesDto
            {
                Id = x.Id,
                Description = x.Type

            }).ToList();
            return fileCategoryDtoList;
        }

        /// <summary>
        /// This method is used to get all BillingTypes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BillingTypeDto>> GetBillingTypesAsync()
        {
            List<BillingTypeDto> billingTypeDtoList = null;

            var riskBillings = await _riskBillingTypeRepository.GetAllAsync().ConfigureAwait(false);

            billingTypeDtoList = riskBillings.Select(x => new BillingTypeDto
            {

                Id = x.Id,
                Description = x.Description,
                Type = x.Type

            }).ToList();

            return billingTypeDtoList;
        }

        /// <summary>
        /// This methods returns all the Payment Types
        /// </summary>
        /// <returns></returns>
        /// 

        public async Task<IEnumerable<PaymentTypesDto>> GetAllPaymentTypesAsync()
        {
            List<PaymentTypesDto> riskPaymentTypeDtoList = null;

            var riskPaymentType = await _riskPaymentTypeRepository.GetAllAsync().ConfigureAwait(false);

            riskPaymentTypeDtoList = riskPaymentType.Select(x => new PaymentTypesDto
            {

                Id = x.Id,
                PaymentType = x.PaymentFromType

            }).ToList();

            return riskPaymentTypeDtoList;
        }




        /// <summary>
        /// This method is used to get all user Emails 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAllUserEmailsAsync()
        {
            List<string> userEmail = await _userRepository.AsQueryable.AsNoTracking()
                            .Where(u => u.IsActive && !u.IsDeleted).Select(x => x.Email).OrderBy(x => x).
                                ToListAsync().ConfigureAwait(false);

            return userEmail;
        }


        /// <summary>
        /// This method is user to get Location using Location Code
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public async Task<LocationDto> GetLocationByCodeAsync(string locationCode)
        {
            Location location = await _locationRepository.AsQueryable.AsNoTracking().Where(l => l.IsDeleted == false && l.Code == locationCode).FirstOrDefaultAsync().ConfigureAwait(false);
            LocationDto locationDto = null;
            if (location != null)
            {
                locationDto = new LocationDto();

                locationDto.Id = location.Id;
                locationDto.Name = location.Name;
                locationDto.Code = location.Code;
                locationDto.CompanyAbbr = location.CompanyAbbr;
            }

            return locationDto;
        }


        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            List<CompanyDto> companyDtoList = null;

            var riskCompanies = await _riskCompanyRepository.AsQueryable.AsNoTracking().Where(x => x.IsDeleted != true).ToListAsync().ConfigureAwait(false);

            if (riskCompanies != null)
            {
                companyDtoList = riskCompanies.Select(x => new CompanyDto
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
                }).ToList();
            }
            return companyDtoList;
        }

        /// <summary>
        /// This method is used to get All the Write Off Types
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WriteOffTypeDTO>> GetAllWriteOffTypesAsync()
        {
            IEnumerable<RiskWriteOffType> writeOffTypes = await _riskWriteOffTypeRepository.AsQueryable.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync().ConfigureAwait(false);

            List<WriteOffTypeDTO> writeOffTypesToReturn = new List<WriteOffTypeDTO>();

            WriteOffTypeDTO writeOffTypeToAdd;
            foreach (var writeOffType in writeOffTypes)
            {
                writeOffTypeToAdd = new WriteOffTypeDTO();

                writeOffTypeToAdd.Id = writeOffType.Id;
                writeOffTypeToAdd.Type = writeOffType.Type;
                writeOffTypeToAdd.Description = writeOffType.Description;
                writeOffTypeToAdd.Status = writeOffType.Status;
                writeOffTypesToReturn.Add(writeOffTypeToAdd);
            }

            return writeOffTypesToReturn;
        }
        //Get generic totalDue 
        public double GetTotalDue(double? TotalBilling, double? TotalPayment)
        {
            double TotalDue = 0;
            TotalDue = (TotalBilling.HasValue ? TotalBilling.Value : default(double)) - (TotalPayment.HasValue ? TotalPayment.Value : default(double));
            return TotalDue;
        }
    }
}
