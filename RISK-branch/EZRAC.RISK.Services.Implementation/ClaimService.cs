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
using System.Linq.Expressions;
using EZRAC.RISK.Util.Common;


namespace EZRAC.RISK.Services.Implementation
{
    public class ClaimService : IClaimService
    {
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<Location> _locationRepository = null;
        IGenericRepository<User> _userRepository = null;
        IGenericRepository<RiskContract> _contractRepository = null;
        IGenericRepository<RiskNonContract> _nonContractRepository = null;
        IGenericRepository<RiskVehicle> _vehicleRepository = null;
        IGenericRepository<RiskClaimApproval> _claimApprovalRepository = null;
        IGenericRepository<RiskIncident> _incidentRepository = null;
        IGenericRepository<RiskPoliceAgency> _policeAgencyRepository = null;
        IGenericRepository<RiskDamage> _damageRepository = null;
        IGenericRepository<RiskDriver> _driverRepository = null;
        IGenericRepository<RiskDriverInsurance> _driverInsuranceRepository = null;
        IBillingService _billingService = null;
        ITSDService _tsdService = null;
        IPaymentService _paymentService = null;

        public ClaimService(IGenericRepository<Claim> claimRepository,
                            IGenericRepository<Location> locationRepository,
                            IGenericRepository<User> userRepository,
                            IGenericRepository<RiskContract> contractRepository,
                            IGenericRepository<RiskVehicle> vehicleRepository,
                            IGenericRepository<RiskClaimApproval> claimApproval,
                            IGenericRepository<RiskIncident> incidentRepository,
                            IGenericRepository<RiskDamage> damageRepository,
                            IGenericRepository<RiskPoliceAgency> policeAgencyRepository,
                            IGenericRepository<RiskDriver> driverRepository,
             IGenericRepository<RiskDriverInsurance> driverInsuranceRepository,
            IGenericRepository<RiskNonContract> nonContractRepository,
                            IBillingService billingService,
                            ITSDService tsdService,
                             IPaymentService paymentService)
        {
            _claimRepository = claimRepository;
            _tsdService = tsdService;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _contractRepository = contractRepository;
            _vehicleRepository = vehicleRepository;
            _claimApprovalRepository = claimApproval;
            _incidentRepository = incidentRepository;
            _policeAgencyRepository = policeAgencyRepository;
            _damageRepository = damageRepository;
            _driverRepository = driverRepository;
            _driverInsuranceRepository = driverInsuranceRepository;
            _billingService = billingService;
            _paymentService = paymentService;
            _nonContractRepository = nonContractRepository;
        }

        public async Task<IEnumerable<ClaimDto>> GetClaimsByCriteria(SearchClaimsCriteria claimsCriteria)
        {

            IList<ClaimDto> claimDtoList = new List<ClaimDto>();

            IList<Claim> claimList = null;

            if (claimsCriteria.ClaimType.Equals(ClaimType.FollowupClaim))
            {
                var predicate = PredicateBuilder.True<Claim>();

                predicate = predicate.And(x => x.IsDeleted == false);

                if (claimsCriteria.ClaimType == ClaimType.FollowupClaim)
                {
                    predicate = predicate.And(x => x.FollowUpDate != null);
                }
                if (!IsLimitedAccess(claimsCriteria.UserId))
                {
                    predicate = predicate.And(x => x.AssignedTo.Equals(claimsCriteria.UserId));
                }

                claimList = await GetClaims(claimsCriteria, predicate);
                if (!claimList.Any() && claimsCriteria.PageCount > 1)
                {
                    claimsCriteria.PageCount = claimsCriteria.PageCount - 1;
                    claimList = await GetClaims(claimsCriteria, predicate);
                }
            }
            claimDtoList = MapClaimList(claimList);

            return claimDtoList;

        }

        private async Task<IList<Claim>> GetClaims(SearchClaimsCriteria claimsCriteria, Expression<Func<Claim, bool>> predicate)
        {
            IList<Claim> claimList = null;
            if (claimsCriteria.SortType.Equals("RiskDriver.FirstName"))
            {
                var driverType = Convert.ToInt32(ClaimsConstant.DriverTypes.Primary);
                claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                x => x.RiskLossType,
                                                                                x => x.RiskContract,
                                                                                x => x.RiskClaimStatus,
                                                                                x => x.RiskDrivers,
                                                                                x => x.OpenLocation).Where(predicate).
                                                                               OrderByWithDirection(x => x.RiskDrivers
                                                                                   .Where(y => y.DriverTypeId == driverType)
                                                                                   .FirstOrDefault().FirstName, claimsCriteria.SortOrder)
                                                                                   .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                                .Take(claimsCriteria.PageSize)
                                                                                .ToListAsync();
            }
            else
            {

                claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                    x => x.RiskLossType,
                                                                                    x => x.RiskContract,
                                                                                    x => x.RiskClaimStatus,
                                                                                    x => x.RiskDrivers,
                                                                                    x => x.OpenLocation).Where(predicate)
                                                                                .OrderBy(claimsCriteria.SortType, claimsCriteria.SortOrder)
                                                                                .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                                .Take(claimsCriteria.PageSize).
                                                                                ToListAsync();
            }

            return claimList;
        }

        public async Task<IEnumerable<ClaimDto>> GetPendingApprovedClaimsByCriteria(SearchClaimsCriteria claimsCriteria)
        {
            IList<Claim> claimList = null;
            IList<ClaimDto> claimDtoList = new List<ClaimDto>();

            var predicate = PredicateBuilder.True<Claim>();

            predicate = predicate.And(x => x.IsDeleted == false);

            if (claimsCriteria.ClaimType.Equals(ClaimType.PendingApproval))
                predicate = predicate.And(x => x.RiskClaimApprovals.Any(y => y.ApprovalStatus == null && y.ApproverId == claimsCriteria.UserId));
            else
                predicate = predicate.And(x => x.RiskClaimApprovals.Any(y => y.ApprovalStatus != null && y.ApproverId == claimsCriteria.UserId));

            claimList = await GetPendingClaims(claimsCriteria, predicate);

            if (!claimList.Any() && claimsCriteria.PageCount > 1)
            {
                claimsCriteria.PageCount = claimsCriteria.PageCount - 1;
                claimList = await GetPendingClaims(claimsCriteria, predicate);
            }

            claimDtoList = MapClaimList(claimList);
            return claimDtoList;
        }

        private async Task<IList<Claim>> GetPendingClaims(SearchClaimsCriteria claimsCriteria, Expression<Func<Claim, bool>> predicate)
        {
            IList<Claim> claimList = null;

            if (claimsCriteria.SortType.Equals("RiskDriver.FirstName"))
            {
                var driverType = Convert.ToInt32(ClaimsConstant.DriverTypes.Primary);
                claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                               x => x.RiskLossType,
                                                                               x => x.RiskContract,
                                                                               x => x.RiskClaimStatus,
                                                                               x => x.RiskDrivers,
                                                                               x => x.RiskClaimApprovals,
                                                                               x => x.OpenLocation).Where(predicate)
                                                                              .OrderByWithDirection(x => x.RiskDrivers
                                                                                  .Where(y => y.DriverTypeId == driverType)
                                                                                  .FirstOrDefault().FirstName, claimsCriteria.SortOrder)
                                                                              .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                              .Take(claimsCriteria.PageSize)
                                                                              .ToListAsync();

            }
            else
            {
                claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                    x => x.RiskLossType,
                                                                                    x => x.RiskContract,
                                                                                    x => x.RiskClaimStatus,
                                                                                    x => x.RiskDrivers,
                                                                                    x => x.RiskClaimApprovals,
                                                                                    x => x.OpenLocation).Where(predicate)
                                                                                .OrderBy(claimsCriteria.SortType, claimsCriteria.SortOrder)
                                                                                .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                                .Take(claimsCriteria.PageSize)
                                                                                .ToListAsync();
            }
            return claimList;
        }

        private IList<ClaimDto> MapClaimList(IList<Claim> claimList)
        {
            var driverType = Convert.ToInt32(ClaimsConstant.DriverTypes.Primary);
            var claimDtoList = claimList.Select(
                 x => new ClaimDto
                 {
                     Id = x.Id,
                     FollowUpdate = x.FollowUpDate,
                     VehicleId = x.VehicleId,
                     SelectedStatusName = x.RiskClaimStatus != null ? x.RiskClaimStatus.Description : string.Empty,
                     SelectedAssignedUserName = x.AssignedToName,
                     SelectedAssignedUserId = x.AssignedTo,
                     ContractNo = x.RiskContract != null ? x.RiskContract.ContractNumber : string.Empty,
                     DriverName = (x.RiskDrivers != null && x.RiskDrivers.Count > 0 && x.RiskDrivers.Where(y =>
                          y.DriverTypeId == driverType).SingleOrDefault() != null) ?
                          x.RiskDrivers.Where(y => y.DriverTypeId == driverType).SingleOrDefault().FirstName : string.Empty,
                     UnitNumber = x.RiskVehicle != null ? x.RiskVehicle.UnitNumber : string.Empty,
                     VehicleName = x.RiskVehicle != null ? x.RiskVehicle.Model : string.Empty,
                     ApprovalStatus = x.RiskClaimApprovals != null && x.RiskClaimApprovals.Any() ? x.RiskClaimApprovals.FirstOrDefault().ApprovalStatus.ToString() : string.Empty,
                     IsComplete = x.FollowUpDate != null ? true : false,
                     CompanyAbbr = x.OpenLocation != null ? x.OpenLocation.CompanyAbbr : string.Empty

                 });
            return claimDtoList.ToList();
        }

        public async Task<Nullable<Int64>> CreateClaimAsync(ClaimDto createClaimRequest, long createdBy)
        {

            ContractInfoFromTsd fetchedDetailsDTO = _tsdService.GetFullContractInfoFromTSD(createClaimRequest.ContractNo, createClaimRequest.UnitNumber);

            Claim claim = await GetClaimBasicInfoToSaveAsync(fetchedDetailsDTO, createClaimRequest, createdBy);


            claim = await _claimRepository.InsertAsync(claim);

            if (claim != null)
            {
                return claim.Id;
            }
            else
            {
                return null;
            }

        }

        private async Task<Claim> GetClaimBasicInfoToSaveAsync(ContractInfoFromTsd contractInfoFromTsd, ClaimDto createClaimRequest, long createdBy)
        {
            Claim claim = null;
            if (createClaimRequest != null)
            {

                claim = await GetNewClaim(contractInfoFromTsd, createClaimRequest, createdBy);

                if (contractInfoFromTsd != null)
                {
                    claim.RiskContract = await GetNewOrUpdatedRiskContract(contractInfoFromTsd.ContractInfo);

                    claim.RiskVehicle = await GetNewOrUpdateRiskVehicle(contractInfoFromTsd.VehicleInfo);

                    claim.RiskDrivers = await GetNewOrUpdateRiskDrivers(contractInfoFromTsd.DriverAndInsuranceInfo, null);

                }
                claim.RiskIncident = new RiskIncident() { LossDate = createClaimRequest.DateOfLoss };

            }
            return claim;
        }

        private async Task<Claim> GetNewClaim(ContractInfoFromTsd contractInfoFromTsd, ClaimDto createClaimRequest, long createdBy)
        {
            Claim claimToReturn = null;


            claimToReturn = new Claim();
            User assignedUser = await _userRepository.GetByIdAsync(createClaimRequest.SelectedAssignedUserId);

            string userFullName = String.Format("{0} {1}", assignedUser.FirstName, assignedUser.LastName);

            long? closeLocationId = null;
            if (contractInfoFromTsd != null)
            {
                closeLocationId = await _locationRepository.AsQueryable.Where(l => l.Code == contractInfoFromTsd.CloseLocationCode).Select(p => p.Id).FirstOrDefaultAsync();
            }


            long? openLocationId = null;
            if (contractInfoFromTsd != null)
            {
                openLocationId = await _locationRepository.AsQueryable.Where(l => l.Code == contractInfoFromTsd.OpenLocationCode).Select(p => p.Id).FirstOrDefaultAsync();
            }


            claimToReturn = MapRiskClaim(createClaimRequest, openLocationId, closeLocationId, userFullName);

            claimToReturn.HasContract = true;

            claimToReturn.FillAuditableEntity(createdBy, true);



            return claimToReturn;
        }

        private async Task<IList<RiskDriver>> GetNewOrUpdateRiskDrivers(IEnumerable<DriverInfoDto> driverAndIncInfo, Nullable<Int64> claimId)
        {
            List<RiskDriver> riskDrivers = new List<RiskDriver>();



            if (driverAndIncInfo != null & driverAndIncInfo.Any())
            {



                foreach (var driver in driverAndIncInfo)
                {
                    RiskDriver riskDriverToAdd = null;
                    if (claimId.HasValue)
                    {
                        riskDriverToAdd = await _driverRepository.AsQueryable.Where(d => d.ClaimId == claimId.Value && d.DriverTypeId == driver.DriverTypeId).FirstOrDefaultAsync();
                    }

                    if (riskDriverToAdd == null)
                    {
                        riskDriverToAdd = new RiskDriver();
                    }

                    riskDriverToAdd = MapRiskDrivers(driver, riskDriverToAdd);

                    riskDrivers.Add(riskDriverToAdd);
                }
            }
            return riskDrivers;
        }

        private async Task<RiskContract> GetNewOrUpdatedRiskContract(ContractDto contractDto)
        {
            RiskContract riskContract = null;
            if (contractDto != null)
            {
                RiskContract contractInfoToUpdate = null;

                contractInfoToUpdate = await _contractRepository.AsQueryable.Where(c => c.ContractNumber == contractDto.ContractNumber).FirstOrDefaultAsync();

                //If contract information alresdy exists update existing contract else create new

                if (contractInfoToUpdate == null)
                    contractInfoToUpdate = new RiskContract();

                riskContract = MapRiskContract(contractDto, contractInfoToUpdate);
            }
            return riskContract;
        }

        private async Task<RiskVehicle> GetNewOrUpdateRiskVehicle(VehicleDto vehicleDto)
        {
            RiskVehicle riskVehicle = null;
            if (vehicleDto != null)
            {
                //If vehicle information alresdy exists update existing contract else create new
                RiskVehicle vehicleInfoToUpdate = await _vehicleRepository.AsQueryable.Where(v => v.UnitNumber == vehicleDto.UnitNumber).FirstOrDefaultAsync();

                if (vehicleInfoToUpdate == null)
                    vehicleInfoToUpdate = new RiskVehicle();


                riskVehicle = MapRiskVehicle(vehicleDto, vehicleInfoToUpdate);
            }
            return riskVehicle;
        }


        public async Task<ClaimBasicInfoDto> GetClaimBasicInfoByClaimIdAsync(long claimId)
        {

            ClaimBasicInfoDto claimBasicInfo;
            Claim claim = null;

            claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,

                                                                           x => x.RiskContract,
                                                                           x => x.RiskNonContract,
                                                                           x => x.OpenLocation,
                                                                           x => x.RiskLossType,
                                                                           x => x.RiskIncident,
                                                                           x => x.RiskClaimApprovals,
                                                                           x => x.RiskClaimStatus).Where(p => p.Id == claimId).SingleOrDefaultAsync();

            claimBasicInfo = MapClaimBasicInfoDTO(claim);

            return claimBasicInfo;
        }

        public async Task<ClaimDto> GetClaimInfoByClaimIdAsync(long claimId)
        {
            ClaimDto claimInfo;


            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.OpenLocation,
                                                                             x => x.CloseLocation,
                                                                             x => x.RiskLossType,
                                                                             x=>x.RiskContract).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            claimInfo = MapClaimInfoDto(claim);

            return claimInfo;
        }

        public async Task<ClaimDto> GetClaimInfoForHeaderAsync(long claimId)
        {
            ClaimDto claimInfo;

            Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskClaimApprovals,
                                                                               x => x.RiskContract,
                                                                               x => x.OpenLocation).Where(x => x.Id == claimId && x.IsDeleted == false).FirstOrDefaultAsync();


            claimInfo = claim != null ? MapClaimInfoDto(claim) : null;

            //Populated to show in the header section
            if (claimInfo != null)
            {
                claimInfo.ContractNo = claim != null && claim.RiskContract != null ? claim.RiskContract.ContractNumber : String.Empty;

            }

            return claimInfo;
        }


        public async Task<ContractDto> GetContractInfoByIdAsync(Int64 id)
        {

            ContractDto contractDto = null;


            RiskContract contract = await _contractRepository.GetByIdAsync(id);

            contractDto = MapContractInfoDTO(contract);

            return contractDto;
        }


        public async Task<ContractDto> GetContractInfoByIdWithoutCreditCardDetailsAsync(Int64 id)
        {

            ContractDto contractDto = null;

            RiskContract contract = await _contractRepository.GetByIdAsync(id);

            contractDto = MapContractInfoDTOWithoutCreditCardDetails(contract);

            return contractDto;
        }

        public async Task<VehicleDto> GetVehicleInfoByIdAsync(Int64 id, string contractNumber)
        {
            VehicleDto vehicleDto = null;


            RiskVehicle vehicle = await _vehicleRepository.GetByIdAsync(id);

            RiskContract contract = null;

            if (contractNumber != null)
                contract = await _contractRepository.AsQueryable.Where(x => x.ContractNumber == contractNumber).FirstOrDefaultAsync();

            vehicleDto = MapVehicleInfoDTO(vehicle, contract);

            return vehicleDto;
        }

        public async Task<IncidentDto> GetIncidentInfoByIdAsync(Int64 incidentId)
        {
            IncidentDto incidentDto = null;

            RiskIncident incident = await _incidentRepository.GetByIdAsync(incidentId);

            incidentDto = MapIncidentInfoDTO(incident);

            return incidentDto;
        }

        public async Task<Nullable<Int64>> AddOrUpdateVehicleInfoAsync(VehicleDto vehicleDto, Int64 claimId)
        {
            Nullable<Int64> vehicleId = null;

            if (vehicleDto != null)
            {
                RiskVehicle vehicle = await _vehicleRepository.AsQueryable.Where(x => x.Id == vehicleDto.Id & x.UnitNumber == vehicleDto.UnitNumber).FirstOrDefaultAsync();


                if (vehicle != null)
                {
                    vehicle = vehicle != null ? vehicle : new RiskVehicle();

                    vehicle = MapRiskVehicle(vehicleDto, vehicle);

                    await _vehicleRepository.UpdateAsync(vehicle);

                    vehicleId = vehicle.Id;

                }//if vehicle not found create new vehicle
                else
                {

                    vehicleId = await CreateNewVehicleInClaim(vehicleDto, claimId);
                }

            }


            return vehicleId;
        }

        private async Task<long?> CreateNewVehicleInClaim(VehicleDto vehicleDto, Int64 claimId)
        {
            Nullable<Int64> vehicleId = null;
            Claim claim = await _claimRepository.GetByIdAsync(claimId);

            if (claim != null)
            {
                claim.RiskVehicle = MapRiskVehicle(vehicleDto, new RiskVehicle());
            }

            await _claimRepository.UpdateAsync(claim);

            claim = await _claimRepository.AsQueryable.Include(x => x.RiskVehicle).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim != null & claim.RiskVehicle != null)
                vehicleId = claim.RiskVehicle.Id;
            return vehicleId;
        }



        public async Task<Nullable<Int64>> UpdateClaimInfoAsync(ClaimDto claimDto, long updatedBy)
        {
            Nullable<Int64> claimId = null;
            if (claimDto != null)
            {
                Claim claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskContract,
                                                                                 x => x.RiskBillings).Where(x => x.Id == claimDto.Id).FirstOrDefaultAsync();
                if (claim != null)
                {
                    claim = MapClaimInfoDomain(claimDto, claim);

                    claim.FillAuditableEntity(updatedBy, false);

                    await _claimRepository.UpdateAsync(claim);

                    claimId = claim.Id;
                }

            }

            return claimId;


        }

        private RiskBillingDto GetLossOfUseBilling(Claim claim)
        {
            RiskBillingDto riskBilling = null;

            if (claim != null && claim.RiskContract != null && claim.LabourHour.HasValue && claim.RiskContract.DailyRate.HasValue)
            {

                riskBilling = new RiskBillingDto();

                riskBilling.Amount = CalculateLossOfUse(claim.LabourHour.Value, claim.RiskContract.DailyRate.Value);

                riskBilling.BillingTypeId = (int)Constants.BillingTypes.LossOfUse;

                riskBilling.ClaimId = claim.Id;
            }

            return riskBilling;

        }


        private double CalculateLossOfUse(double labourHours, double dailyRate)
        {
            //Logic taken from existing system to caluclate loss of use

            double factor = default(double);

            double labourHoursByFour = labourHours / 4;

            if (labourHoursByFour < 6)
            {
                factor = 0;
            }
            else if (labourHoursByFour >= 6 && labourHoursByFour < 11)
            {
                factor = 2;
            }
            else if (labourHoursByFour >= 11 && labourHoursByFour < 16)
            {
                factor = 4;
            }
            else if (labourHoursByFour >= 16 && labourHoursByFour < 21)
            {
                factor = 6;
            }
            else if (labourHoursByFour >= 21 && labourHoursByFour < 26)
            {
                factor = 8;
            }
            else if (labourHoursByFour >= 26 && labourHoursByFour < 31)
            {
                factor = 10;
            }
            else if (labourHoursByFour >= 31 && labourHoursByFour < 36)
            {
                factor = 12;
            }
            else if (labourHoursByFour >= 36 && labourHoursByFour < 41)
            {
                factor = 14;
            }
            else if (labourHoursByFour >= 41 && labourHoursByFour < 46)
            {
                factor = 16;
            }
            else if (labourHoursByFour >= 46)
            {
                factor = 18;
            }
            return (labourHoursByFour + factor) * dailyRate; //(From contract table) 

        }

        public async Task<Nullable<Int64>> UpdateContractInfoAsync(ContractDto contractDto, Int64 claimId)
        {
            Nullable<Int64> contractId = null;
            if (contractDto != null)
            {

                RiskContract contract = await _contractRepository.AsQueryable.Where(x => x.Id == contractDto.Id &
                                                                    x.ContractNumber == contractDto.ContractNumber).FirstOrDefaultAsync();

                if (contract != null)
                {
                    contract = MapRiskContractOnEdit(contractDto, contract);

                    await _contractRepository.UpdateAsync(contract);

                    contractId = contractDto.Id;
                }
                else
                {
                    ////contract dosenot exist for this claim create new contract
                    contractId = await CreateNewContractInClaim(contractDto, claimId);
                }

                await _paymentService.UpdateCdwLdwAndLpc2(claimId);
            }

            return contractId;
        }

        private RiskContract MapRiskContractOnEdit(ContractDto contractInfoDto, RiskContract riskContractInfo)
        {
            if (contractInfoDto != null && riskContractInfo != null)
            {

                riskContractInfo.CDWVoid = contractInfoDto.CDWVoided;
                riskContractInfo.LDWVoid = contractInfoDto.LDWVoided;
                riskContractInfo.CardNumber = contractInfoDto.CardNumber;

            }

            return riskContractInfo;
        }

        private async Task<Nullable<Int64>> CreateNewContractInClaim(ContractDto contractDto, Int64 claimId)
        {
            Nullable<Int64> contractId = null;
            Claim claim = await _claimRepository.GetByIdAsync(claimId);

            if (claim != null)
                claim.RiskContract = MapRiskContract(contractDto, new RiskContract());

            await _claimRepository.UpdateAsync(claim);

            claim = await _claimRepository.AsQueryable.Include(x => x.RiskContract).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim != null & claim.RiskContract != null)
                contractId = claim.RiskContract.Id;

            return contractId;
        }

        public async Task<Nullable<Int64>> UpdateIncidentInfoAsync(IncidentDto incidentDto)
        {
            Nullable<Int64> incidentId = null;
            if (incidentDto != null)
            {

                var id = incidentDto.Id;
                RiskIncident incident = await _incidentRepository.AsQueryable.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (incident != null)
                {

                    Location selectedLocation = null;
                    RiskPoliceAgency selectedPoliceAgency = null;

                    if (incidentDto.SelectedLocationId.HasValue)
                        selectedLocation = await _locationRepository.GetByIdAsync(incidentDto.SelectedLocationId.Value);


                    if (incidentDto.SelectedPoliceAgencyId.HasValue)
                        selectedPoliceAgency = await _policeAgencyRepository.GetByIdAsync(incidentDto.SelectedPoliceAgencyId.Value);



                    incident = MapRiskIncident(incidentDto, incident, selectedLocation, selectedPoliceAgency);

                    await _incidentRepository.UpdateAsync(incident);

                    incidentId = incident.Id;

                }

            }

            return incidentId;

        }

        private RiskIncident MapRiskIncident(IncidentDto incidentDto, RiskIncident incident, Location selectedLocation, RiskPoliceAgency selectedPoliceAgency)
        {

            if (incidentDto != null && incident != null)
            {

                incident.LossDate = incidentDto.LossDate.HasValue ? incidentDto.LossDate.Value : incident.LossDate;
                incident.LocationId = incidentDto.SelectedLocationId;

                incident.LocationName = selectedLocation != null ? selectedLocation.Name : String.Empty;


                incident.CaseNumber = incidentDto.CaseNumber;
                incident.PoliceAgencyId = incidentDto.SelectedPoliceAgencyId;


                incident.PoliceAgencyName = selectedPoliceAgency != null ? selectedPoliceAgency.AgencyName : String.Empty;

                incident.RenterFault = incidentDto.RenterFault;
                incident.ThirdPartyFault = incidentDto.ThirdPartyFault;

                incident.ReportedDate = incidentDto.ReportedDate;
            }
            return incident;
        }





        private Claim MapClaimInfoDomain(ClaimDto claimDto, Claim claim)
        {
            if (claimDto != null)
            {
                claim.CloseDate = claimDto.CloseDate;
                claim.OpenLocationId = claimDto.SelectedOpenLocationId;
                claim.CloseLocationId = claimDto.SelectedCloseLocationId;
                claim.EstReturnDate = claimDto.EstReturnDate;
                // claim.LabourHour = claimDto.LabourHour;
                claim.LossTypeId = claimDto.SelectedLossTypeId;
                claim.IsCollectable = claimDto.IsCollectable;
                claim.HasContract = claimDto.HasContract;
            }
            return claim;
        }



        public async Task<int> GetFollowUpCountList(long userId)
        {

            var predicate = PredicateBuilder.True<Claim>();
            predicate = predicate.And(x => x.IsDeleted == false);
            predicate = predicate.And(x => x.FollowUpDate != null);

            if (!IsLimitedAccess(userId))
            {
                predicate = predicate.And(x => x.AssignedTo.Equals(userId));
            }

            var claimsCount = await _claimRepository.AsQueryable.
                                Where(predicate).CountAsync();

            return claimsCount;
        }


        public async Task<int> GetPendingApprovedCountList(long userId, ClaimType type)
        {
            var predicate = PredicateBuilder.True<Claim>();

            predicate = predicate.And(x => x.IsDeleted == false);

            if (type.Equals(ClaimType.PendingApproval))
            {
                predicate = predicate.And(x => x.RiskClaimApprovals.Any(y => y.ApprovalStatus == null && y.ApproverId == userId));
            }
            else
            {
                predicate = predicate.And(x => x.RiskClaimApprovals.Any(y => y.ApprovalStatus != null && y.ApproverId == userId));
            }
            var claimsCount = await _claimRepository.AsQueryable.Where(predicate).CountAsync();

            return claimsCount;
        }


        public Task<List<DriverInfoDto>> GetDriverAndIncInfoByClaimIdAsync(int claimNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesInfoByClaimIdAsync(long claimNumber)
        {
            var claim = await _claimRepository.AsQueryable
                                        .Include(x => x.RiskDamages.Select(y => y.RiskDamageType))
                                        .Where(x => x.Id == claimNumber)
                                        .FirstOrDefaultAsync().ConfigureAwait(false);

            IEnumerable<DamageDto> dmgList = MapDamageInfo(claim);

            return dmgList;

        }

        public async Task<bool> AddDamage(DamageDto damageDto)
        {
            bool success = false;

            Claim claim = await _claimRepository.GetByIdAsync(damageDto.ClaimId);

            if (damageDto != null)
            {
                await _damageRepository.InsertAsync(
                            new RiskDamage
                            {
                                ClaimId = damageDto.ClaimId,
                                DamageTypeId = damageDto.SectionId,
                                Details = damageDto.Details,
                                VehicleId = (long)claim.VehicleId
                            });
                success = true;
            }
            return success;
        }

        public async Task<bool> DeleteDamage(DamageDto damageDto)
        {
            bool success = false;

            if (damageDto != null)
            {
                RiskDamage recordToDelete = await _damageRepository.GetByIdAsync(damageDto.DamageId);

                if (recordToDelete != null)
                {
                    await _damageRepository.DeleteAsync(recordToDelete);
                }
                success = true;
            }
            return success;
        }

        public Task<List<PicturesAndFilesDto>> GetFilesInfoByClaimIdAsync(int claimNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<BillingsDto>> GetBillingsInfoByClaimIdAsync(int claimNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentDto>> GetPaymentsInfoByClaimIdAsync(int claimNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<SalvageDto> GetSalvageInfoByClaimIdAsync(int claimNumber)
        {
            var claim = await _claimRepository.GetByIdAsync(claimNumber);

            var salvageDto = MapSalvageInfoDto(claim);

            return salvageDto;
        }




        public async Task<bool> SetFollowUpdateByClaimIdAsync(long claimId, DateTime followupDate, long updatedBy)
        {
            bool success = false;
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim != null)
            {
                claim.FollowUpDate = followupDate;
                claim.FillAuditableEntity(updatedBy, false);
                await _claimRepository.UpdateAsync(claim);
                success = true;
            }
            return success;
        }

        public async Task<bool> SetAssignedTo(long claimId, long userId, long updatedBy)
        {
            bool success = false;
            var claim = await _claimRepository.GetByIdAsync(claimId);

            if (claim != null && claim.AssignedTo != userId)
            {
                claim.AssignedTo = userId;
                var user = await GetUserUserById(userId);
                claim.AssignedToName = String.Format("{0} {1}", user.FirstName, user.LastName); ;
                claim.AssignedTo = user.Id;
                claim.FollowUpDate = claim.FollowUpDate < DateTime.Now.Date ?
                    DateTime.Now.Date.AddWorkdays(Convert.ToInt32(ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.FollowUpAddDays))) :
                    claim.FollowUpDate.Value.AddWorkdays(Convert.ToInt32(ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.FollowUpAddDays)));

                claim.FillAuditableEntity(updatedBy, false);

                await _claimRepository.UpdateAsync(claim);
                success = true;
            }
            return success;
        }


        public async Task<bool> SetFollowUpCompletedAsync(long claimId, bool isCompleted)
        {
            bool success = false;
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim != null && isCompleted)
            {
                claim.FollowUpDate = null;
                await _claimRepository.UpdateAsync(claim);
                success = true;
            }
            return success;
        }

        public async Task<bool> DeleteClaims(List<long> claimsToDelete)
        {
            bool success = false;

            foreach (var claimId in claimsToDelete)
            {
                Claim claim = await _claimRepository.GetByIdAsync(claimId);

                claim.IsDeleted = true;

                await _claimRepository.UpdateAsync(claim);

                success = true;

            }

            return success;
        }

        public async Task<bool> SaveClaimHeaderInfo(ClaimDto claimResponse, long updatedBy)
        {
            var claim = await _claimRepository.AsQueryable.Include(c => c.RiskClaimApprovals).Where(c => c.Id == claimResponse.Id).FirstOrDefaultAsync();
            if (claim != null)
            {
                claim.ClaimStatusId = claimResponse.SelectedStatusId;

                if (!claimResponse.SelectedAssignedUserId.Equals(claim.AssignedTo))
                {
                    string daysToAdd = ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.FollowUpAddDays);

                    claim.FollowUpDate = !String.IsNullOrEmpty(daysToAdd) ? DateTime.Now.AddWorkdays(int.Parse(daysToAdd)) : DateTime.Now.AddDays(0);

                    claim.AssignedTo = claimResponse.SelectedAssignedUserId;

                    User user = await _userRepository.GetByIdAsync(claimResponse.SelectedAssignedUserId);

                    claim.AssignedToName = String.Format("{0} {1}", user.FirstName, user.LastName);
                }
                else
                {

                    claim.FollowUpDate = claimResponse.FollowUpdate;
                }


                if (claimResponse.ApproverId != null)
                {
                    claim.RiskClaimApprovals = GetNewOrUpdateClaimApproval(claimResponse, claim.RiskClaimApprovals.ToList());
                }
                else
                {
                    claim.RiskClaimApprovals = await RemoveOpenApprovels(claimResponse, claim.RiskClaimApprovals);
                }

                claim.FillAuditableEntity(updatedBy, false);

                await _claimRepository.UpdateAsync(claim);

                return true;
            }
            else
                return false;
        }

        public async Task<bool> ApproveOrRejectClaims(long claimId, bool status)
        {
            var success = false;

            var claimApproval = await _claimApprovalRepository.AsQueryable.Where(x => x.ClaimId == claimId && x.ApprovalStatus == null).SingleOrDefaultAsync();

            if (claimApproval != null)
            {
                claimApproval.ApprovalStatus = status;
                await _claimApprovalRepository.UpdateAsync(claimApproval);
                success = true;
            }

            return success;
        }

        public async Task DownloadContractInfo(Int64 claimId, string contractNumber)
        {
            ContractInfoFromTsd fetchedDetailsDTO = _tsdService.GetFullContractInfoFromTSD(contractNumber, string.Empty);

            if (fetchedDetailsDTO != null)
            {
                Claim claim = await GetClaimBasicInfoToUpdateAsync(fetchedDetailsDTO, claimId);

                await _claimRepository.UpdateAsync(claim);

                await _paymentService.UpdateCdwLdwAndLpc2(claimId);
            }

        }

        public async Task<Nullable<Int64>> DownloadVehicleInfo(long claimId, string unitNumber)
        {
            Nullable<Int64> vehicleId = null;

            VehicleDto vehicleDto = _tsdService.GetVehicleInfoFromTSD(unitNumber);

            Claim claim = await _claimRepository.GetByIdAsync(claimId);

            if (claim != null && vehicleDto != null)
            {
                claim.RiskVehicle = await GetNewOrUpdateRiskVehicle(vehicleDto);

                await _claimRepository.UpdateAsync(claim);

                //claim = await _claimRepository.GetByIdAsync(claim.Id);

                vehicleId = claim != null ? claim.VehicleId : null;
            }

            return vehicleId;
        }

        private async Task<Claim> GetClaimBasicInfoToUpdateAsync(ContractInfoFromTsd fetchedDetailsDto, Int64 claimId)
        {
            Claim claim = null;

            if (fetchedDetailsDto != null)
            {
                claim = await _claimRepository.AsQueryable.Include(c => c.RiskDrivers.Select(d => d.RiskDriverInsurance)).Where(c => c.Id == claimId).FirstOrDefaultAsync();
                if (claim != null)
                {


                    long? openLocationId = null;
                    if (fetchedDetailsDto != null)
                        openLocationId = await _locationRepository.AsQueryable.Where(l => l.Code == fetchedDetailsDto.OpenLocationCode).Select(p => p.Id).FirstOrDefaultAsync();

                    claim.OpenLocationId = openLocationId.HasValue && openLocationId.Value != 0 ? openLocationId : null;


                    long? closeLocationId = null;
                    if (fetchedDetailsDto != null)
                        closeLocationId = await _locationRepository.AsQueryable.Where(l => l.Code == fetchedDetailsDto.OpenLocationCode).Select(p => p.Id).FirstOrDefaultAsync();

                    claim.CloseLocationId = closeLocationId.HasValue && closeLocationId.Value != 0 ? closeLocationId : null;


                    claim.RiskContract = await GetNewOrUpdatedRiskContract(fetchedDetailsDto.ContractInfo);

                    claim.RiskVehicle = await GetNewOrUpdateRiskVehicle(fetchedDetailsDto.VehicleInfo);

                    claim = await RemoveExistingDrivers(claim);

                    claim.RiskDrivers = await GetNewOrUpdateRiskDrivers(fetchedDetailsDto.DriverAndInsuranceInfo, claimId);
                }

            }
            return claim;
        }

        private async Task<Claim> RemoveExistingDrivers(Claim claim)
        {

            List<RiskDriver> riskDrivers = claim.RiskDrivers.ToList();

            foreach (var driver in riskDrivers)
            {
                if (driver.RiskDriverInsurance != null)
                {
                    await _driverInsuranceRepository.DeleteAsync(driver.RiskDriverInsurance);
                }

                await _driverRepository.DeleteAsync(driver);
            }


            return claim;
        }

        private async Task<IList<RiskClaimApproval>> RemoveOpenApprovels(ClaimDto claimResponse, IList<RiskClaimApproval> claimApprovals)
        {
            List<RiskClaimApproval> claimApprovalsToReturn = null;



            RiskClaimApproval openApproval = null;

            claimApprovalsToReturn = claimApprovals != null ? claimApprovalsToReturn = claimApprovals.ToList() : new List<RiskClaimApproval>();

            openApproval = claimApprovalsToReturn.Count > 0 ? claimApprovalsToReturn.Where(p => p.ApprovalStatus == null).FirstOrDefault() : null;

            if (openApproval != null)
            {
                await _claimApprovalRepository.DeleteAsync(openApproval);
                claimApprovalsToReturn.Remove(openApproval);
            }

            return claimApprovalsToReturn;
        }

        private static IList<RiskClaimApproval> GetNewOrUpdateClaimApproval(ClaimDto claimResponse, List<RiskClaimApproval> claimApprovals)
        {
            claimApprovals = claimApprovals != null ? claimApprovals : new List<RiskClaimApproval>();

            RiskClaimApproval claimApproval = new RiskClaimApproval();


            claimApproval = claimApprovals.Any() ? claimApprovals.FirstOrDefault() : new RiskClaimApproval();


            claimApproval.ApproverId = (Int64)claimResponse.ApproverId;
            claimApproval.ClaimId = claimResponse.Id;
            claimApproval.ClaimStatusId = claimResponse.SelectedStatusId;
            claimApproval.RequestedUserId = claimResponse.SelectedAssignedUserId;
            claimApproval.ApprovalStatus = null;

            if (!claimApprovals.Any())
            {
                claimApprovals.Add(claimApproval);
            }

            return claimApprovals;

        }

        private async Task<User> GetUserUserById(long userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }


        private bool IsLimitedAccess(long userId)
        {

            bool exist = false;

            IEnumerable<User> users = _userRepository.AsQueryable.Where(u => (u.UserRoleID == EZRAC.RISK.Services.Implementation.Helper.Constants.Roles.RiskManager ||
                                                                                 u.UserRoleID == EZRAC.RISK.Services.Implementation.Helper.Constants.Roles.RiskSupervisor) && u.Id == userId).ToList();
            if (users.Any())
            {
                exist = true;
            }

            return exist;
        }

        #region Mappers


        //Mappers that convert Dto to Domain object
        private static Claim MapRiskClaim(ClaimDto objClaim, Nullable<Int64> openLocationId, Nullable<long> closeLocationId, string userFullName)
        {

            Claim claim = new Claim();

            //if (openLocationId != null && openLocationId != 0)
            //    claim.OpenLocationId = openLocationId;
            //else
            //    claim.OpenLocationId = objClaim.SelectedLocationId;


            claim.OpenLocationId = openLocationId.HasValue && openLocationId.Value != 0 ? openLocationId.Value : objClaim.SelectedOpenLocationId;


            claim.CloseLocationId = closeLocationId.HasValue && closeLocationId.Value != 0 ? closeLocationId : claim.CloseLocationId;

            claim.OpenDate = DateTime.Now;
            
            claim.LossTypeId = objClaim.SelectedLossTypeId;
            if (!openLocationId.HasValue || openLocationId == 0)
            {
                claim.OpenLocationId = objClaim.SelectedOpenLocationId;
            }
            claim.AssignedTo = objClaim.SelectedAssignedUserId;
            claim.AssignedToName = userFullName;
            claim.ClaimStatusId = Constants.ClaimStatus.Open;
            claim.IsCollectable = true;

            //If required we can set followup date as current day +2

            string daysToAdd = ConfigSettingsReader.GetAppSettingValue(Constants.AppSettings.FollowUpAddDays);

            claim.FollowUpDate = DateTime.Now.AddWorkdays(int.Parse(daysToAdd));
            claim.IsDeleted = false;

            return claim;
        }

        private static RiskDriver MapRiskDrivers(DriverInfoDto driverAndIncInfo, RiskDriver riskDrivers)
        {
            if (driverAndIncInfo != null && riskDrivers != null)
            {

                riskDrivers.FirstName = driverAndIncInfo.FirstName;
                riskDrivers.LastName = driverAndIncInfo.LastName;
                riskDrivers.Address = driverAndIncInfo.Address1;
                riskDrivers.Address2 = driverAndIncInfo.Address2;
                riskDrivers.City = driverAndIncInfo.City;
                riskDrivers.State = driverAndIncInfo.State;
                riskDrivers.Zip = driverAndIncInfo.Zip;
                riskDrivers.Phone1 = driverAndIncInfo.Phone1;
                riskDrivers.Phone2 = driverAndIncInfo.Phone2;
                riskDrivers.Email = driverAndIncInfo.Email;
                riskDrivers.Fax = driverAndIncInfo.Fax;
                riskDrivers.OtherContact = driverAndIncInfo.OtherContact;
                riskDrivers.DOB = driverAndIncInfo.DOB;
                riskDrivers.LicenceExpiryDate = driverAndIncInfo.LicenceExpiry;
                riskDrivers.IsAuthorized = driverAndIncInfo.IsAuthorizedDriver;
                riskDrivers.LicenceNumber = driverAndIncInfo.LicenceNumber;
                riskDrivers.LicenceState = driverAndIncInfo.LicenceState;


                riskDrivers.DriverTypeId = driverAndIncInfo.DriverTypeId;

            }
            return riskDrivers;
        }

        private static RiskVehicle MapRiskVehicle(VehicleDto vehicleInfo, RiskVehicle riskVehicle)
        {

            if (vehicleInfo != null && riskVehicle != null)
            {
                riskVehicle.UnitNumber = !String.IsNullOrEmpty(vehicleInfo.UnitNumber) ? vehicleInfo.UnitNumber : riskVehicle.UnitNumber;
                riskVehicle.TagNumber = vehicleInfo.TagNumber;
                riskVehicle.TagExpires = vehicleInfo.TagExpires;
                riskVehicle.Description = vehicleInfo.Description;
                riskVehicle.Make = vehicleInfo.Make;
                riskVehicle.Model = vehicleInfo.Model;
                riskVehicle.Year = vehicleInfo.Year;
                riskVehicle.VIN = vehicleInfo.VIN;
                riskVehicle.Location = vehicleInfo.Location;
                riskVehicle.Color = vehicleInfo.Color;
                riskVehicle.Status = vehicleInfo.Status;
                riskVehicle.Mileage = vehicleInfo.Mileage;
                riskVehicle.PurchaseType = vehicleInfo.PurchaseType;
            }

            return riskVehicle;
        }

        private static RiskContract MapRiskContract(ContractDto contractInfoDto, RiskContract riskContractInfo)
        {


            if (contractInfoDto != null && riskContractInfo != null)
            {
                riskContractInfo.ContractNumber = !String.IsNullOrEmpty(contractInfoDto.ContractNumber) ? contractInfoDto.ContractNumber : riskContractInfo.ContractNumber;

                riskContractInfo.PickUpDate = contractInfoDto.PickupDate;
                riskContractInfo.ReturnDate = contractInfoDto.ReturnDate;
                riskContractInfo.DailyRate = contractInfoDto.DailyRate;
                riskContractInfo.WeeklyRate = contractInfoDto.WeeklyRate;
                riskContractInfo.SLI = contractInfoDto.SLI;
                riskContractInfo.CDW = contractInfoDto.CDW;
                riskContractInfo.CDWVoid = contractInfoDto.CDWVoided;
                riskContractInfo.LDWVoid = contractInfoDto.LDWVoided;
                riskContractInfo.DaysOut = contractInfoDto.DaysOut;
                riskContractInfo.Miles = (int)contractInfoDto.Miles;
                riskContractInfo.MilesIn = contractInfoDto.MilesIn;
                riskContractInfo.MilesOut = contractInfoDto.MilesOut;

                //contract.CDWVoid = contractInfo.CDWVoided;
                riskContractInfo.SLI = contractInfoDto.SLI;
                riskContractInfo.LPC = contractInfoDto.LPC;
                riskContractInfo.LPC2 = contractInfoDto.LPC2;
                riskContractInfo.GARS = contractInfoDto.GARS;
                riskContractInfo.LDW = contractInfoDto.LDW;
                riskContractInfo.CardNumber = contractInfoDto.CardNumber;
                riskContractInfo.CardType = contractInfoDto.CardType;
                riskContractInfo.CardExpDate = contractInfoDto.CardExpDate;
                riskContractInfo.SwapedVehicles = contractInfoDto.SwapedVehicles;
            }

            return riskContractInfo;
        }


        //Mappers that convert Domain To Dto
        private ClaimBasicInfoDto MapClaimBasicInfoDTO(Claim claim)
        //IEnumerable<string> swappedVehicles)
        {
            ClaimBasicInfoDto claimBasicInfo = null;
            if (claim != null)
            {
                claimBasicInfo = new ClaimBasicInfoDto();

                claimBasicInfo.ClaimInfo = MapClaimInfoDto(claim);

                claimBasicInfo.ContractInfo = MapContractInfoDTO(claim.RiskContract);

                claimBasicInfo.NonContractInfo = MapNonContarctDto(claim.RiskNonContract);

                claimBasicInfo.VehicleInfo = MapVehicleInfoDTO(claim.RiskVehicle, claim.RiskContract);

                claimBasicInfo.IncidentInfo = MapIncidentInfoDTO(claim.RiskIncident);
            }

            return claimBasicInfo;
        }

        private NonContractDto MapNonContarctDto(RiskNonContract riskNonContract)
        {
            NonContractDto nonContractDto = null;
            if (riskNonContract!=null)
            {
                nonContractDto = new NonContractDto();
                nonContractDto.Id = riskNonContract.Id;
                nonContractDto.NonContractNumber = riskNonContract.NonContractNumber;
                
                
            }
            return nonContractDto;
        }

        private ClaimDto MapClaimInfoDto(Claim riskClaim)
        {
            ClaimDto claimInfo = null;

            if (riskClaim != null)
            {
                claimInfo = new ClaimDto();
                claimInfo.Id = riskClaim.Id;
                claimInfo.OpenDate = riskClaim.OpenDate;
                claimInfo.CloseDate = riskClaim.CloseDate;
                claimInfo.LabourHour = riskClaim.LabourHour;
                claimInfo.SelectedOpenLocationId = riskClaim.OpenLocationId;
                claimInfo.SelectedCloseLocationId = riskClaim.CloseLocationId;
                claimInfo.VehicleId = riskClaim.VehicleId;
                if (riskClaim.OpenLocation != null)
                {
                    claimInfo.SelectedOpenLocationName = riskClaim.OpenLocation.Name;
                    claimInfo.CompanyAbbr = riskClaim.OpenLocation.CompanyAbbr;
                }
                claimInfo.SelectedLossTypeId = riskClaim.LossTypeId;

                if (riskClaim.RiskLossType != null)
                {
                    claimInfo.SelectedLossTypeDescription = riskClaim.RiskLossType.Description;
                }

                claimInfo.SelectedStatusId = riskClaim.ClaimStatusId;

                RiskClaimApproval claimApproval = null;
                if (riskClaim.RiskClaimApprovals != null)
                    claimApproval = riskClaim.RiskClaimApprovals.Where(p => p.ApprovalStatus == null).FirstOrDefault();

                if (claimApproval != null)
                {
                    claimInfo.ApproverId = claimApproval.ApproverId;
                }

                claimInfo.SelectedAssignedUserId = riskClaim.AssignedTo;

                claimInfo.FollowUpdate = riskClaim.FollowUpDate;

                claimInfo.EstReturnDate = riskClaim.EstReturnDate;

                var due = (riskClaim.TotalBilling.HasValue ? riskClaim.TotalBilling.Value : default(double))
                                    - (riskClaim.TotalPayment.HasValue ? riskClaim.TotalPayment.Value : default(double));
                claimInfo.TotalDue = Math.Round(due, 2);

                claimInfo.IsCollectable = riskClaim.IsCollectable;
                claimInfo.HasContract = riskClaim.HasContract;
                claimInfo.DisableHasContract = riskClaim.RiskContract != null && !String.IsNullOrEmpty(riskClaim.RiskContract.ContractNumber);

            }
            return claimInfo;
        }


        private SalvageDto MapSalvageInfoDto(Claim claim)
        {
            SalvageDto salvageDto = new SalvageDto();

            if (claim != null)
            {
                salvageDto.Amount = claim.SalvageAmount;
                salvageDto.Date = claim.SalvageReceiptDate;
                salvageDto.ClaimId = claim.Id;
            }
            return salvageDto;
        }

        private ContractDto MapContractInfoDTO(RiskContract riskContract)
        {
            ContractDto contractInfo = null;
            if (riskContract != null)
            {
                contractInfo = new ContractDto();

                contractInfo.Id = riskContract.Id;
                contractInfo.ContractNumber = riskContract.ContractNumber;
                contractInfo.PickupDate = riskContract.PickUpDate;
                contractInfo.ReturnDate = riskContract.ReturnDate;
                contractInfo.DaysOut = riskContract.DaysOut;
                contractInfo.Miles = riskContract.Miles;
                contractInfo.DailyRate = riskContract.DailyRate;
                contractInfo.WeeklyRate = riskContract.WeeklyRate;
                contractInfo.CDW = riskContract.CDW;
                contractInfo.CDWVoided = riskContract.CDWVoid;
                contractInfo.LDWVoided = riskContract.LDWVoid;
                contractInfo.LDW = riskContract.LDW;
                contractInfo.SLI = riskContract.SLI;
                contractInfo.LPC = riskContract.LPC;
                contractInfo.LPC2 = riskContract.LPC2;
                contractInfo.TotalRate = riskContract.TotalRate;
                contractInfo.GARS = riskContract.GARS;
                contractInfo.CardNumber = riskContract.CardNumber;
                contractInfo.CardType = riskContract.CardType;
                contractInfo.CardExpDate = riskContract.CardExpDate;
               
            }
            return contractInfo;
        }

        private ContractDto MapContractInfoDTOWithoutCreditCardDetails(RiskContract riskContract)
        {
            ContractDto contractInfo = null;
            if (riskContract != null)
            {
                contractInfo = new ContractDto();

                contractInfo.Id = riskContract.Id;
                contractInfo.ContractNumber = riskContract.ContractNumber;
                contractInfo.PickupDate = riskContract.PickUpDate;
                contractInfo.ReturnDate = riskContract.ReturnDate;
                contractInfo.DaysOut = riskContract.DaysOut;
                contractInfo.Miles = riskContract.Miles;
                contractInfo.DailyRate = riskContract.DailyRate;
                contractInfo.WeeklyRate = riskContract.WeeklyRate;
                contractInfo.CDW = riskContract.CDW;
                contractInfo.CDWVoided = riskContract.CDWVoid;
                contractInfo.LDWVoided = riskContract.LDWVoid;
                contractInfo.LDW = riskContract.LDW;
                contractInfo.SLI = riskContract.SLI;
                contractInfo.LPC = riskContract.LPC;
                contractInfo.LPC2 = riskContract.LPC2;
                contractInfo.TotalRate = riskContract.TotalRate;
                contractInfo.GARS = riskContract.GARS;
               

            }
            return contractInfo;
        }

        private VehicleDto MapVehicleInfoDTO(RiskVehicle riskVehicle, RiskContract contract)
        //IEnumerable<string> swappedVehicles)
        {
            VehicleDto vehicleInfo = null;
            if (riskVehicle != null)
            {
                vehicleInfo = new VehicleDto();

                vehicleInfo.Id = riskVehicle.Id;
                vehicleInfo.UnitNumber = riskVehicle.UnitNumber;
                vehicleInfo.TagNumber = riskVehicle.TagNumber;
                vehicleInfo.TagExpires = riskVehicle.TagExpires;
                vehicleInfo.VIN = riskVehicle.VIN;
                vehicleInfo.Model = riskVehicle.Model;
                vehicleInfo.Make = riskVehicle.Make;
                vehicleInfo.Year = riskVehicle.Year;
                vehicleInfo.Color = riskVehicle.Color;
                vehicleInfo.Mileage = riskVehicle.Mileage;
                vehicleInfo.Location = riskVehicle.Location;
                vehicleInfo.Status = riskVehicle.Status;
                vehicleInfo.Description = riskVehicle.Description;

                vehicleInfo.PurchaseType = riskVehicle.PurchaseType;

                vehicleInfo.SwappedVehicles = contract != null ? contract.SwapedVehicles : String.Empty;
            }
            return vehicleInfo;
        }

        private IncidentDto MapIncidentInfoDTO(RiskIncident riskIncident)
        {
            IncidentDto incidentInfo = null;

            if (riskIncident != null)
            {
                incidentInfo = new IncidentDto();

                incidentInfo.Id = riskIncident.Id;
                incidentInfo.LossDate = riskIncident.LossDate;
                incidentInfo.SelectedLocationName = riskIncident.LocationName;
                incidentInfo.SelectedPoliceAgencyId = riskIncident.PoliceAgencyId;
                incidentInfo.SelectedPoliceAgencyName = riskIncident.PoliceAgencyName;
                incidentInfo.SelectedLocationId = riskIncident.LocationId;
                incidentInfo.CaseNumber = riskIncident.CaseNumber;

                incidentInfo.RenterFault = riskIncident.RenterFault;
                incidentInfo.ThirdPartyFault = riskIncident.ThirdPartyFault;

                incidentInfo.ReportedDate = riskIncident.ReportedDate;
            }
            return incidentInfo;
        }


        private IEnumerable<DamageDto> MapDamageInfo(Claim claim)
        {
            var damageDtoList = claim.RiskDamages.Select(
                   x => new DamageDto
                   {
                       DamageId = x.Id,
                       Section = x.RiskDamageType.Section,
                       SectionId = x.DamageTypeId,
                       Details = x.Details

                   });
            return damageDtoList.ToList();
        }

        #endregion



        public async Task<IList<ClaimDto>> SearchClaimsByContractNumberAsync(string contractNumber)
        {
            IList<ClaimDto> claimDtoList = new List<ClaimDto>();

            if (!string.IsNullOrEmpty(contractNumber))
            {
                var predicate = PredicateBuilder.True<Claim>();

                predicate = predicate.And(x => x.IsDeleted == false);

                predicate = predicate.And(x => x.RiskContract != null && x.RiskContract.ContractNumber.Equals(contractNumber));

                var claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                         x => x.RiskLossType,
                                                                                         x => x.RiskContract,
                                                                                         x => x.RiskClaimStatus,
                                                                                         x => x.RiskDrivers,
                                                                                         x => x.OpenLocation).Where(predicate)
                                                                                        .ToListAsync();

                if (claimList.Any())
                {
                    claimDtoList = MapClaimList(claimList);
                }
            }
            return claimDtoList;
        }

        public async Task<bool> SearchClaimByClaimIdAsync(long claimId)
        {
            Claim claim = null;

            var predicate = PredicateBuilder.True<Claim>();
            predicate = predicate.And(x => x.IsDeleted == false);
            predicate = predicate.And(x => x.Id == claimId);

            if (claimId != null && claimId > 0)
            {
                claim = await _claimRepository.AsQueryable.Where(predicate).SingleOrDefaultAsync();
            }
            if (claim != null)
                return true;

            return false;
        }


        public async Task<bool> SaveSalvageInfo(long claimNumber, decimal? amount, DateTime? dateOfReceipt)
        {
            var success = false;
            var claim = await _claimRepository.AsQueryable.Where(x => x.Id == claimNumber).FirstOrDefaultAsync();

            if (claim != null)
            {
                claim.SalvageAmount = amount;
                claim.SalvageReceiptDate = dateOfReceipt;
                await _claimRepository.UpdateAsync(claim);
                success = true;
            }
            return success;
        }

        public async Task<IList<ClaimDto>> GetAdvancedSearchRecords(ClaimSearchDto claimSearchDto, SearchClaimsCriteria claimsCriteria)
        {
            IList<ClaimDto> claimDtoList = new List<ClaimDto>();
            if (claimSearchDto != null)
            {
                var predicate = PredicateBuilder.True<Claim>();
                //predicate = predicate.And(x => x.IsDeleted == false);

                predicate = AdvancedSearchExpression(claimSearchDto);

                IList<Claim> claimList = null;
                if (claimsCriteria.SortType.Equals("RiskDriver.FirstName"))
                {
                    int driverTypeId = Convert.ToInt32(ClaimsConstant.DriverTypes.Primary);
                    claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                           x => x.RiskLossType,
                                                                                           x => x.RiskContract,
                                                                                           x => x.RiskClaimStatus,
                                                                                           x => x.RiskNotes,
                                                                                           x => x.RiskIncident,
                                                                                           x => x.RiskDrivers,
                                                                                           x => x.OpenLocation).Where(predicate)
                                                                                          .OrderByWithDirection(x => x.RiskDrivers
                                                                                 .Where(y => y.DriverTypeId == driverTypeId)
                                                                                 .FirstOrDefault().FirstName, claimsCriteria.SortOrder)
                                                                                          .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                                          .Take(claimsCriteria.PageSize)
                                                                                          .ToListAsync();
                }
                else
                {
                    claimList = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                        x => x.RiskLossType,
                                                                                        x => x.RiskContract,
                                                                                        x => x.RiskClaimStatus,
                                                                                        x => x.RiskNotes,
                                                                                        x => x.RiskIncident,
                                                                                        x => x.RiskDrivers,
                                                                                        x => x.OpenLocation).Where(predicate)
                                                                                       .OrderBy(claimsCriteria.SortType, claimsCriteria.SortOrder)
                                                                                       .Skip(claimsCriteria.PageSize * (claimsCriteria.PageCount - 1))
                                                                                       .Take(claimsCriteria.PageSize)
                                                                                       .ToListAsync();
                }
                if (claimList.Any())
                {
                    claimDtoList = MapClaimList(claimList);
                }
            }
            return claimDtoList;
        }

        public async Task<int> GetAdvancedSearchRecordCount(ClaimSearchDto claimSearchDto)
        {
            
            if (claimSearchDto != null)
            {
                var predicate = PredicateBuilder.True<Claim>();
                //predicate = predicate.And(x => x.IsDeleted == false);

                predicate = AdvancedSearchExpression(claimSearchDto);

                int claimListCount = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskVehicle,
                                                                                        x => x.RiskLossType,
                                                                                        x => x.RiskContract,
                                                                                        x => x.RiskClaimStatus,
                                                                                        x => x.RiskNotes,
                                                                                        x => x.RiskIncident,
                                                                                        x => x.RiskDrivers,
                                                                                        x => x.OpenLocation).Where(predicate)
                                                                                       .CountAsync();
                return claimListCount;
            }
            return 0;
        }

        public Expression<Func<Claim, bool>> AdvancedSearchExpression(ClaimSearchDto claimSearchDto)
        {
            var predicate = PredicateBuilder.True<Claim>();
            predicate = predicate.And(x => x.IsDeleted == false);

            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.primarydrivername)//1.done.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.Primary;
                predicate = predicate.And(x => x.RiskDrivers.Any(a => a.FirstName.Equals(claimSearchDto.SearchItem) || a.LastName.Equals(claimSearchDto.SearchItem) && a.DriverTypeId == driverTypeId));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.additionaldrivername)//2.done.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.Additional;
                predicate = predicate.And(x => x.RiskDrivers.Any(a => a.FirstName.Equals(claimSearchDto.SearchItem) || a.LastName.Equals(claimSearchDto.SearchItem) && a.DriverTypeId == driverTypeId));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.thirdpartyname)//3.done.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.ThirdParty;
                predicate = predicate.And(x => x.RiskDrivers.Any(a => a.FirstName.Equals(claimSearchDto.SearchItem) || a.LastName.Equals(claimSearchDto.SearchItem) && a.DriverTypeId == driverTypeId));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.tagno)//4.done.
            {
                predicate = predicate.And(x => x.VehicleId != null && x.RiskVehicle.TagNumber.Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.unitno)//5.done.
            {
                predicate = predicate.And(x => x.VehicleId != null && x.RiskVehicle.UnitNumber.Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.last8ofvin)//6.done.
            {
                predicate = predicate.And(x => x.VehicleId != null && x.RiskVehicle.VIN.Contains(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.driverlicense)//7.done
            {
                predicate = predicate.And(x => x.RiskDrivers.Any(a => a.LicenceNumber.Equals(claimSearchDto.SearchItem)));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.contract)//8.done.
            {
                predicate = predicate.And(x => x.ContractId != null && x.RiskContract.ContractNumber.Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.notes)//9.done.
            {
                predicate = predicate.And(x => x.RiskNotes.Any(y => y.Description.Contains(claimSearchDto.SearchItem)));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.policecaseno)//10.done
            {
                predicate = predicate.And(x => x.RiskIncident.CaseNumber.Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.dateofloss)//11.done.
            {
                DateTime dateOfLossFrom, dateOfLossTo;
                DateTime.TryParse(claimSearchDto.DateFrom, out dateOfLossFrom);
                DateTime.TryParse(claimSearchDto.DateTo, out dateOfLossTo);
                dateOfLossFrom = DateTime.Parse(dateOfLossFrom.ToShortDateString());
                dateOfLossTo = DateTime.Parse(dateOfLossTo.ToShortDateString());
                if (string.IsNullOrEmpty(claimSearchDto.DateFrom))
                {
                    predicate = predicate.And(x => x.RiskIncident.LossDate != null && (x.RiskIncident.LossDate <= dateOfLossTo));
                }
                else if (string.IsNullOrEmpty(claimSearchDto.DateTo))
                {
                    predicate = predicate.And(x => x.RiskIncident.LossDate != null && (x.RiskIncident.LossDate >= dateOfLossFrom));
                }
                else
                {
                    predicate = predicate.And(x => x.RiskIncident.LossDate != null && (x.RiskIncident.LossDate >= dateOfLossFrom) && (x.RiskIncident.LossDate <= dateOfLossTo));
                }


                //DateTime dateOfLoss;
                //DateTime.TryParse(claimSearchDto.DateOfLoss, out dateOfLoss);
                //predicate = predicate.And(x => x.DateofLoss != null && x.DateofLoss.Value.Equals(dateOfLoss.Date));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.inspolicyno)//12.done
            {
                predicate = predicate.And(x => x.RiskDrivers.Any(y => y.RiskDriverInsurance.PolicyNumber.Equals(claimSearchDto.SearchItem)));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.primarydriverins)//13.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.Primary;
                predicate = predicate.And(x => x.RiskDrivers.Any(y => y.RiskDriverInsurance.InsuranceClaimNumber.Equals(claimSearchDto.SearchItem) && y.DriverTypeId == driverTypeId));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.additionaldriverins)//14.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.Additional;
                predicate = predicate.And(x => x.RiskDrivers.Any(y => y.RiskDriverInsurance.InsuranceClaimNumber.Equals(claimSearchDto.SearchItem) && y.DriverTypeId == driverTypeId));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.thirdpartyins)//15.
            {
                var driverTypeId = (int)ClaimsConstant.DriverTypes.ThirdParty;
                predicate = predicate.And(x => x.RiskDrivers.Any(y => y.RiskDriverInsurance.InsuranceClaimNumber.Equals(claimSearchDto.SearchItem) && y.DriverTypeId == driverTypeId));
            }
            //if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.zurichclaimno)//16.
            //{
            //    predicate = predicate.And(x => x.RiskDrivers.Any(y => y.RiskDriverInsurance.InsuranceClaimNumber.Equals(claimSearchDto.SearchItem) && y.RiskDriverInsurance.RiskInsurance.CompanyName.Equals("ZURICH")));
            //}
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.phone)//17.
            {
                predicate = predicate.And(x => x.RiskDrivers.Any(y => y.Phone1.Equals(claimSearchDto.SearchItem) || y.Phone2.Equals(claimSearchDto.SearchItem)));
            }

            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.followdaterange)//18.done.
            {
                DateTime FollowUpDateFrom, FollowUpDateTo;
                DateTime.TryParse(claimSearchDto.DateFrom, out FollowUpDateFrom);
                DateTime.TryParse(claimSearchDto.DateTo, out FollowUpDateTo);
                FollowUpDateFrom = DateTime.Parse(FollowUpDateFrom.ToShortDateString());
                FollowUpDateTo = DateTime.Parse(FollowUpDateTo.ToShortDateString());
                if (string.IsNullOrEmpty(claimSearchDto.DateFrom))
                {
                    predicate = predicate.And(x => (x.FollowUpDate <= FollowUpDateTo));
                }
                else if (string.IsNullOrEmpty(claimSearchDto.DateTo))
                {
                    predicate = predicate.And(x => (x.FollowUpDate >= FollowUpDateFrom));
                }
                else
                {
                    predicate = predicate.And(x => (x.FollowUpDate >= FollowUpDateFrom) && (x.FollowUpDate <= FollowUpDateTo));
                }
            }

            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.CreditCardNumber)
            {
                predicate = predicate.And(x => x.RiskContract != null && x.RiskContract.CardNumber.HasValue && x.RiskContract.CardNumber.Value.ToString().Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.CreditCardType)
            {
                predicate = predicate.And(x => x.RiskContract != null && x.RiskContract.CardType.Equals(claimSearchDto.SearchItem));
            }
            if (claimSearchDto.SearchType == ClaimsConstant.AdvanceSearchCriteriaKey.CreditCardExpiration)
            {
                predicate = predicate.And(x => x.RiskContract != null && x.RiskContract.CardExpDate.Equals(claimSearchDto.SearchItem));
            }

            return predicate;
        }
        public async Task<bool> UpdateLabourHour(Nullable<double> labourHour, long claimId)
        {
            bool isSuccess = false;

            labourHour = labourHour.HasValue ? labourHour.Value : default(double);

            Claim claim = await _claimRepository.AsQueryable.Include(x => x.RiskContract).Where(x => x.Id == claimId).FirstOrDefaultAsync();
            if (claim != null)
            {
                claim.LabourHour = labourHour;

                await _claimRepository.UpdateAsync(claim);
                isSuccess = true;
            }


            if (claim != null && claim.LabourHour.HasValue && claim.RiskContract != null && claim.RiskContract.DailyRate.HasValue)
            {
                RiskBillingDto billingDto = GetLossOfUseBilling(claim);

                await _billingService.AddOrUpdateBillingToClaimAsync(billingDto);
            }

            return isSuccess;
        }


        public async Task<Nullable<double>> GetLabourHoursByClaimIdAsync(long id)
        {
            return await _claimRepository.AsQueryable.Where(x => x.Id == id).Select(x => x.LabourHour).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteClaimFromViewClaim(long id,long user)
        {
            var success = false;
            var claimToDelete = await _claimRepository.AsQueryable.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (claimToDelete != null)
            {
                claimToDelete.IsDeleted = true;
                claimToDelete.UpdatedBy = user;
                claimToDelete.UpdatedDateTime = DateTime.Now;
                await _claimRepository.UpdateAsync(claimToDelete);
                success = true;
            }
            return success;    
        }



        public async Task<CompanyDto> GetCompanyByClaimIdAsync(long id)
        {
            CompanyDto compantDto = null;
            Claim claim = await _claimRepository.AsQueryable.Include(x => x.OpenLocation.Company).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (claim != null && claim.OpenLocation!= null && claim.OpenLocation.Company != null)
            {
                compantDto = MapCompanyDto(claim.OpenLocation.Company);
                
            }

            return compantDto;
        }

        private CompanyDto MapCompanyDto(Company company)
        {
            CompanyDto companyDto = null;
            if (company !=null)
            {
                companyDto = new CompanyDto();
                companyDto.Abbr = company.Abbr;
                companyDto.Address = company.Address;
                companyDto.City = company.City;
                companyDto.Fax = company.Fax;
                companyDto.State = company.State;
                companyDto.Zip = company.Zip;
                companyDto.Phone = company.Phone;
                companyDto.Name = company.Name;
                companyDto.Website = company.Website;
            }
            return companyDto;
        }


        public async Task<ContractDto> GetContractByClaimIdAsync(long id)
        {
            ContractDto contractDto = null;
            RiskContract contract = await _claimRepository.AsQueryable.Where(x => x.Id == id).Select(x => x.RiskContract).FirstOrDefaultAsync();

            if (contract != null)
            {
                contractDto = MapContractInfoDTO(contract);
            }

            return contractDto;
        }

        public bool IsClaimCreator(long userId, long claimId)
        {
            bool IsClaimCreator = false;

            if (claimId != 0 && userId != 0)
            {
                Claim claim = _claimRepository.AsQueryable.Where(x => x.Id == claimId).FirstOrDefault();
                IsClaimCreator = userId == claim.CreatedBy;
            }
       
            return IsClaimCreator;
        }


        public async Task<NonContractDto> GetNonContractInfoByIdAsync(long nonContractId)
        {
            NonContractDto nonContractDto = null;
            RiskNonContract nonContract = await _nonContractRepository.GetByIdAsync(nonContractId);

            if (nonContract != null)
            {
                nonContractDto = new NonContractDto();

                nonContractDto.Id = nonContract.Id;
                nonContractDto.NonContractNumber = nonContract.NonContractNumber;
            }
            return nonContractDto;
        }


        public async Task<Nullable<long>> UpdateNonContractInfoAsync(NonContractDto nonContractDto, long claimId)
        {
            Nullable<long> nonContractId = null;
            if (nonContractDto != null && nonContractDto.Id != 0)
            {
                RiskNonContract riskNonContract = await _nonContractRepository.GetByIdAsync(nonContractDto.Id);
                if (riskNonContract != null)
                {
                    riskNonContract.NonContractNumber = nonContractDto.NonContractNumber;
                }

                await _nonContractRepository.UpdateAsync(riskNonContract);

                nonContractId = nonContractDto.Id;
            }
            else if (nonContractDto != null && nonContractDto.Id == 0)
            {
                Claim claim = await _claimRepository.GetByIdAsync(claimId);

                claim.RiskNonContract = new RiskNonContract();

                claim.RiskNonContract.NonContractNumber = nonContractDto.NonContractNumber;

               await  _claimRepository.UpdateAsync(claim);

               nonContractId = claim.RiskNonContract.Id;
            
            }
            return nonContractId;
        }


        public async Task<string> GenerateNonContractNumber(long claimId)
        {
            string nonContractNumber = string.Empty;
            var claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskLossType,
                                                                            x=>x.OpenLocation).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            if (claim!=null && claim.OpenLocation!=null && claim.RiskLossType !=null)
            {
                nonContractNumber = String.Format("{0}-{1}-{2}", String.IsNullOrEmpty(claim.OpenLocation.Code) ? string.Empty : claim.OpenLocation.Code.ToUpper().Trim(),
                                                                 String.IsNullOrEmpty(claim.RiskLossType.Description) ? string.Empty : claim.RiskLossType.Description.Substring(0, 3).ToUpper(),
                                                                 DateTime.Now.ToString(CommonConstant.MMDDYYYY));
            }

            return nonContractNumber;

        }


    }


}
