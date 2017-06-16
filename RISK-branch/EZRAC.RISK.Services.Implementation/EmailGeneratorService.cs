using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;


namespace EZRAC.RISK.Services.Implementation
{
    public class EmailGeneratorService : IEmailGeneratorService
    {

        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<RiskDriver> _driverRepository = null;
        IBillingService _billingService = null;
        IPaymentService _paymentService = null;
        IClaimService _claimService = null;
        IDriverAndIncService _driverAndIncService = null;


        public EmailGeneratorService(IGenericRepository<Claim> claimRepository,
                                      IGenericRepository<RiskDriver> driverRepository,
                                     IBillingService billingService,
                                     IPaymentService paymentService,
                                     IClaimService claimService,
                                     IDriverAndIncService driverService)
        {
            _claimRepository = claimRepository;
            _driverRepository = driverRepository;
            _billingService = billingService;
            _claimService = claimService;
            _driverAndIncService = driverService;
            _paymentService = paymentService;
        }
        /// <summary>
        /// To get Information To Send.
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<InformationToShowDto>> GetInformationToShowList(long claimId)
        {
            List<InformationToShowDto> informationToSendList = new List<InformationToShowDto>();

            var claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskDamages,
                                                                    x => x.RiskBillings,
                                                                    x => x.RiskPayments,
                                                                    x => x.RiskContract,
                                                                    x => x.RiskDrivers.Select(d => d.RiskDriverInsurance)).Where(x => x.Id == claimId).FirstOrDefaultAsync();

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.ContractInformation, ClaimsConstant.ContractInformation,claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.CreditCardInformation, ClaimsConstant.CreditCardInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.PrimaryDriverInformation, ClaimsConstant.PrimaryDriverInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.VehicleInformation, ClaimsConstant.VehicleInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.AdditionalDriverInformation, ClaimsConstant.AdditionalDriverInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.IncidentInformation, ClaimsConstant.IncidentInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInformation, ClaimsConstant.ThirdPartyDriverInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.DamageInformation, ClaimsConstant.DamageInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.PrimaryDriverInsuranceInformation, ClaimsConstant.PrimaryDriverInsuranceInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.ChargeInformation, ClaimsConstant.ChargeInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.AdditionalDriverInsuranceInformation, ClaimsConstant.AdditionalDriverInsuranceInformation, claim));

            informationToSendList.Add(GetInformationToShow(ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInsuranceInformation, ClaimsConstant.ThirdPartyDriverInsuranceInformation, claim));

            return informationToSendList;
        }
        /// <summary>
        /// Get Recipients based on conditions
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmailGeneratorReceipientDto>> GetRecipients(long claimId)
        {
            List<EmailGeneratorReceipientDto> recipients = new List<EmailGeneratorReceipientDto>();

            var claim = await _claimRepository.AsQueryable.Include(
                x => x.RiskDrivers.Select(d => d.RiskDriverInsurance)
                    .Select(i => i.RiskInsurance)).Where(c => c.Id == claimId).FirstOrDefaultAsync();

            if (claim != null && claim.RiskDrivers != null)
            {

                if (IsEmailExists(ClaimsConstant.DriverTypes.Primary, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.PrimaryDriver), Text = ClaimsConstant.PrimaryDriver });
                }

                if (IsEmailExists(ClaimsConstant.DriverTypes.Additional, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.AdditionalDriver), Text = ClaimsConstant.AdditionalDriver });
                }

                if (IsInsuranceEmailExists(ClaimsConstant.DriverTypes.Primary, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.PrimaryDriverInsurance), Text = ClaimsConstant.PrimaryDriverInsurance });
                }

                if (IsInsuranceEmailExists(ClaimsConstant.DriverTypes.Additional, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.AdditionalDriverInsurance), Text = ClaimsConstant.AdditionalDriverInsurance });
                }

                if (IsEmailExists(ClaimsConstant.DriverTypes.ThirdParty, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.ThirdPartyDriver), Text = ClaimsConstant.ThirdPartyDriver });
                }

                if (IsInsuranceEmailExists(ClaimsConstant.DriverTypes.ThirdParty, claim.RiskDrivers))
                {
                    recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.ThirdPartyDriverInsurance), Text = ClaimsConstant.ThirdPartyDriverInsurance });
                }

            }

            recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.OtherRiskUser), Text = ClaimsConstant.OtherRiskUser });
            recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.CustomEmail), Text = ClaimsConstant.CustomEmail });
            recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.EmailToEmpire), Text = ClaimsConstant.EmailToEmpire });
            recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.EmailToKnightManagement), Text = ClaimsConstant.EmailToKnightManagement });
            recipients.Add(new EmailGeneratorReceipientDto { Value = Convert.ToInt32(ClaimsConstant.EmailRecipients.EmailToNationalCasualty), Text = ClaimsConstant.EmailToNationalCasualty });

            return recipients;
        }

        private bool IsInsuranceEmailExists(ClaimsConstant.DriverTypes driverType, IList<RiskDriver> riskDrivers)
        {
            bool IsInsuranceEmailExists = false;

            foreach (var driver in riskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(driverType)).ToList())
            {
                if (driver != null && driver.RiskDriverInsurance != null && driver.RiskDriverInsurance.RiskInsurance != null &&
                !String.IsNullOrEmpty(driver.RiskDriverInsurance.RiskInsurance.Email))
                {
                    IsInsuranceEmailExists = true;
                }

            }

            return IsInsuranceEmailExists;
        }

        private bool IsEmailExists(ClaimsConstant.DriverTypes driverType, IList<RiskDriver> riskDrivers)
        {
            bool IsEmailExists = false;

            IsEmailExists = riskDrivers.Any(x => x.DriverTypeId == Convert.ToInt32(driverType) && !String.IsNullOrEmpty(x.Email));

          
            return IsEmailExists;

        }

        private InformationToShowDto GetInformationToShow(ClaimsConstant.EmailInfoToSend emailInfoToSendType, string InfoText, Claim claim)
        {
            
            InformationToShowDto InfoToSend = new InformationToShowDto();

            InfoToSend.Id = Convert.ToInt32(emailInfoToSendType);
            InfoToSend.Text = InfoText;

            switch (emailInfoToSendType)
            {
                case ClaimsConstant.EmailInfoToSend.ContractInformation:
                   
                    InfoToSend.IsAvailable = claim.ContractId != null;
                    break;

                case ClaimsConstant.EmailInfoToSend.VehicleInformation:
                    
                    InfoToSend.IsAvailable = claim.VehicleId != null;
                    break;
                case ClaimsConstant.EmailInfoToSend.IncidentInformation:
                    InfoToSend.IsAvailable = true;//Incident is always available for every claim
                    break;
                case ClaimsConstant.EmailInfoToSend.DamageInformation:
                    InfoToSend.IsAvailable = claim.RiskDamages != null && claim.RiskDamages.Any();
                    break;
                case ClaimsConstant.EmailInfoToSend.ChargeInformation:
                    InfoToSend.IsAvailable = (claim.RiskBillings != null && claim.RiskBillings.Any()) || (claim.RiskPayments != null && claim.RiskPayments.Any());
                    break;
                case ClaimsConstant.EmailInfoToSend.PrimaryDriverInformation:
                    InfoToSend.IsAvailable = claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Primary)).Any();
                    break;
                case ClaimsConstant.EmailInfoToSend.AdditionalDriverInformation:
                    InfoToSend.IsAvailable = claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Additional)).Any();
                    break;
                case ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInformation:
                    InfoToSend.IsAvailable = claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty)).Any();
                    break;
                case ClaimsConstant.EmailInfoToSend.PrimaryDriverInsuranceInformation:

                    if (claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Primary)).Any())
                    {
                        InfoToSend.IsAvailable = claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Primary)).FirstOrDefault().RiskDriverInsurance != null;
                    }
                    break;
                case ClaimsConstant.EmailInfoToSend.AdditionalDriverInsuranceInformation:

                    if (claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Additional)).Any())
                    {
                        InfoToSend.IsAvailable = claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Additional)).FirstOrDefault().RiskDriverInsurance != null;
                    }
                    break;

                case ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInsuranceInformation:

                    if (claim.RiskDrivers != null && claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty)).Any())
                    {
                        InfoToSend.IsAvailable = claim.RiskDrivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty)).FirstOrDefault().RiskDriverInsurance != null;
                    }
                    break;
                case ClaimsConstant.EmailInfoToSend.CreditCardInformation:


                    InfoToSend.IsAvailable = claim.RiskContract != null && (claim.RiskContract.CardNumber.HasValue || !String.IsNullOrEmpty(claim.RiskContract.CardExpDate) || !String.IsNullOrEmpty(claim.RiskContract.CardType));
                   
                    break;


                default:
                    break;
            }

            

            return InfoToSend;
        }


        /// <summary>
        /// To Get Dtos for selected information to send in email
        /// </summary>
        /// <param name="SelectedInfoToSend"></param>
        /// <returns></returns>
        public async Task<InformationToSendDto> GetInformationToSend(ClaimsConstant.EmailInfoToSend[] SelectedInfoToSend, long claimId)
        {

            InformationToSendDto informationToSendDto = null;
            List<DriverInfoDto> drivers = null;
            List<ClaimsConstant.DriverTypes> SelectedInsuranceInfo = null;
            List<ClaimsConstant.DriverTypes> SelectedDriverInfo = null;
            if (SelectedInfoToSend != null)
            {

                informationToSendDto = new InformationToSendDto();
                drivers = new List<DriverInfoDto>();

                SelectedInsuranceInfo = new List<ClaimsConstant.DriverTypes>();
                SelectedDriverInfo = new List<ClaimsConstant.DriverTypes>();

                Claim claim = await _claimRepository.GetByIdAsync(claimId);

                foreach (int Info in SelectedInfoToSend)
                {
                    switch ((ClaimsConstant.EmailInfoToSend)Info)
                    {
                        case ClaimsConstant.EmailInfoToSend.ContractInformation:

                            if (claim.ContractId.HasValue)
                            {
                                informationToSendDto.ContractInfo = await _claimService.GetContractInfoByIdWithoutCreditCardDetailsAsync(claim.ContractId.Value);
                            }
                            break;
                        case ClaimsConstant.EmailInfoToSend.CreditCardInformation:

                            if (claim.ContractId.HasValue)
                            {
                                informationToSendDto.ContractInfo = await _claimService.GetContractInfoByIdAsync(claim.ContractId.Value);
                            }
                            break;
                        case ClaimsConstant.EmailInfoToSend.VehicleInformation:

                            if (claim.VehicleId.HasValue)
                                informationToSendDto.VehicleInfo = await _claimService.GetVehicleInfoByIdAsync(claim.VehicleId.Value, null);

                            break;
                        case ClaimsConstant.EmailInfoToSend.IncidentInformation:
                            
                                informationToSendDto.IncidentInfo = await _claimService.GetIncidentInfoByIdAsync(claim.Id);
                            break;
                        case ClaimsConstant.EmailInfoToSend.DamageInformation:
                            informationToSendDto.Damages = await _claimService.GetDamagesInfoByClaimIdAsync(claimId);
                            break;
                        case ClaimsConstant.EmailInfoToSend.ChargeInformation:

                            informationToSendDto.Billings = await _billingService.GetBillingsByClaimIdAsync(claimId);

                            informationToSendDto.Payments = await _paymentService.GetAllPaymentsByClaimId(claimId);

                            break;
                        case ClaimsConstant.EmailInfoToSend.PrimaryDriverInformation:

                            drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.Primary, claimId));
                            SelectedDriverInfo.Add(ClaimsConstant.DriverTypes.Primary);
                            break;
                        case ClaimsConstant.EmailInfoToSend.AdditionalDriverInformation:

                            drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.Additional, claimId));
                            SelectedDriverInfo.Add(ClaimsConstant.DriverTypes.Additional);
                            break;
                        case ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInformation:

                            drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.ThirdParty, claimId));
                            SelectedDriverInfo.Add(ClaimsConstant.DriverTypes.ThirdParty);
                            break;
                        case ClaimsConstant.EmailInfoToSend.PrimaryDriverInsuranceInformation:

                            if (!drivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Primary)).Any())
                                drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.Primary, claimId));

                            SelectedInsuranceInfo.Add(ClaimsConstant.DriverTypes.Primary);

                            break;
                        case ClaimsConstant.EmailInfoToSend.AdditionalDriverInsuranceInformation:
                            if (!drivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.Additional)).Any())
                                drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.Additional, claimId));

                            SelectedInsuranceInfo.Add(ClaimsConstant.DriverTypes.Additional);

                            break;
                        case ClaimsConstant.EmailInfoToSend.ThirdPartyDriverInsuranceInformation:
                            if (!drivers.Where(x => x.DriverTypeId == Convert.ToInt32(ClaimsConstant.DriverTypes.ThirdParty)).Any())
                                drivers.AddRange(await _driverAndIncService.GetDriverByTypeIdOfClaimAsync(ClaimsConstant.DriverTypes.ThirdParty, claimId));

                            SelectedInsuranceInfo.Add(ClaimsConstant.DriverTypes.ThirdParty);

                            break;
                        default:
                            break;
                    }

                }

                informationToSendDto.DriverInfo = drivers != null && drivers.Any() ? drivers : null;
                informationToSendDto.SelectedInsuranceInfo = SelectedInsuranceInfo != null && SelectedInsuranceInfo.Any() ? SelectedInsuranceInfo : null;
                informationToSendDto.SelectedDriverInfo = SelectedDriverInfo != null && SelectedDriverInfo.Any() ? SelectedDriverInfo : null;
                
            }
            return informationToSendDto;

            
        }


        public async Task<IEnumerable<string>> GetEmailRecipients(EmailGeneratorDto emailGeneratorDto)
        {
            List<string> ToEmails = new List<string>();
            string email = String.Empty;
            RiskDriver driver = null;

            switch ((ClaimsConstant.EmailRecipients)emailGeneratorDto.SelectedReceipient)
            {
                case ClaimsConstant.EmailRecipients.PrimaryDriver:

                    email = await _driverRepository.AsQueryable.Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.Primary && d.ClaimId == emailGeneratorDto.ClaimId)
                                                                                                                        .Select(d=>d.Email).FirstOrDefaultAsync();
                    if (!String.IsNullOrEmpty(email))
                        ToEmails.Add(email);
                    
                    break;
                case ClaimsConstant.EmailRecipients.AdditionalDriver:
                    email = await _driverRepository.AsQueryable.Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.Additional && d.ClaimId == emailGeneratorDto.ClaimId)
                                                                                                                        .Select(d=>d.Email).FirstOrDefaultAsync();
                    if (!String.IsNullOrEmpty(email))
                        ToEmails.Add(email);
                    break;
                case ClaimsConstant.EmailRecipients.ThirdPartyDriver:
                    email = await _driverRepository.AsQueryable.Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.ThirdParty && d.ClaimId == emailGeneratorDto.ClaimId)
                                                                                                                        .Select(d=>d.Email).FirstOrDefaultAsync();
                    if (!String.IsNullOrEmpty(email))
                        ToEmails.Add(email);
                    break;
                case ClaimsConstant.EmailRecipients.AdditionalDriverInsurance:

                     driver = await _driverRepository.AsQueryable.IncludeMultiple(x => x.RiskDriverInsurance,
                                                                                x=>x.RiskDriverInsurance.RiskInsurance)
                                                                                .Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.Additional &&
                                                                                    d.ClaimId == emailGeneratorDto.ClaimId).FirstOrDefaultAsync();
                    if (driver != null && driver.RiskDriverInsurance!= null && driver.RiskDriverInsurance.RiskInsurance != null)
                        ToEmails.Add(driver.RiskDriverInsurance.RiskInsurance.Email);

                    break;
                case ClaimsConstant.EmailRecipients.PrimaryDriverInsurance:
                      driver = await _driverRepository.AsQueryable.IncludeMultiple(x => x.RiskDriverInsurance,
                                                                                x=>x.RiskDriverInsurance.RiskInsurance)
                                                                                .Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.Primary &&
                                                                                    d.ClaimId == emailGeneratorDto.ClaimId).FirstOrDefaultAsync();
                    if (driver != null && driver.RiskDriverInsurance!= null && driver.RiskDriverInsurance.RiskInsurance != null)
                        ToEmails.Add(driver.RiskDriverInsurance.RiskInsurance.Email);
                    break;
                case ClaimsConstant.EmailRecipients.ThirdPartyDriverInsurance:
                       driver = await _driverRepository.AsQueryable.IncludeMultiple(x => x.RiskDriverInsurance,
                                                                                x=>x.RiskDriverInsurance.RiskInsurance)
                                                                                .Where(d => d.DriverTypeId == (int)ClaimsConstant.DriverTypes.Primary &&
                                                                                    d.ClaimId == emailGeneratorDto.ClaimId).FirstOrDefaultAsync();
                    if (driver != null && driver.RiskDriverInsurance!= null && driver.RiskDriverInsurance.RiskInsurance != null)
                        ToEmails.Add(driver.RiskDriverInsurance.RiskInsurance.Email);
                    break;
                case ClaimsConstant.EmailRecipients.OtherRiskUser:
                    ToEmails.AddRange(emailGeneratorDto.SelectedUserEmails);
                    break;
                case ClaimsConstant.EmailRecipients.CustomEmail:

                    if (!String.IsNullOrEmpty(emailGeneratorDto.CustomEmailAddress))
                    {
                        foreach (var emailToAdd in emailGeneratorDto.CustomEmailAddress.Split(';'))
                        {
                            ToEmails.Add(emailToAdd);
                        }
                    }
                    break;

                case ClaimsConstant.EmailRecipients.EmailToEmpire:

                    ToEmails.Add(ConfigSettingsReader.GetAppSettingValue(AppSettings.SendEmailToEmpire));

                    break;

                case ClaimsConstant.EmailRecipients.EmailToNationalCasualty:

                    ToEmails.Add(ConfigSettingsReader.GetAppSettingValue(AppSettings.SendEmailToNationalCasualty));

                    break;
                case ClaimsConstant.EmailRecipients.EmailToKnightManagement:

                    ToEmails.Add(ConfigSettingsReader.GetAppSettingValue(AppSettings.SendEmailToKnightManagement));

                    break;
                default:
                    break;
            }
            return ToEmails;
        }

        public async Task<string> GetSubjectLine(long claimId, string loggedInUserName)
        {
            string subject = String.Empty;

            var claim = await _claimRepository.AsQueryable.IncludeMultiple(x => x.RiskContract,
                                                                     x => x.OpenLocation.Company).Where(x=>x.Id == claimId).FirstOrDefaultAsync();

            if (claim != null)
            {
                subject = String.Format("Claim Information - {0}-{1}, {2} from {3}",
                            claim.OpenLocation != null &&claim.OpenLocation.Company != null ? claim.OpenLocation.Company.Name : String.Empty, claim.Id,
                            claim.RiskContract != null ? claim.RiskContract.ContractNumber : String.Empty,
                            loggedInUserName);
            }

            return subject;
        }
    }
}
