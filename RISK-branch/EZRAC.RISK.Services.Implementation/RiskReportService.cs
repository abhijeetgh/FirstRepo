using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.SqlClient;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using System.Data;
using System.Reflection;
using EZRAC.RISK.EntityFramework;

namespace EZRAC.RISK.Services.Implementation
{
    public class RiskReportService : IRiskReport, IDisposable
    {
        IGenericRepository<RiskReport> _riskReportRepository = null;
        IGenericRepository<RiskReportCategory> _riskReportCategoryRepository = null;
        IGenericRepository<RiskVehicle> _vehicleRepository = null;
        IGenericRepository<Claim> _claimRepository = null;
        IGenericRepository<User> _userRepository = null;
        IGenericRepository<UserRole> _userRoleRepository = null;
        ITSDService _tsdService = null;

        CRMSContext _crmsContext = null;
        private static string _connectionString;
        IEFHelper _iEFHelper = null;
        public RiskReportService(IGenericRepository<Claim> claimRepository,
                                 IGenericRepository<RiskReport> riskReportRepository,
                                 IGenericRepository<RiskReportCategory> riskReportCategoryRepository,
                                 IGenericRepository<RiskVehicle> vehicleRepository,
                                 IGenericRepository<User> userRepository,
                                 IGenericRepository<UserRole> userRoleRepository,
                                 IEFHelper iEFHelper,
                                 ITSDService tsdService)
        {
            _claimRepository = claimRepository;
            _riskReportRepository = riskReportRepository;
            _riskReportCategoryRepository = riskReportCategoryRepository;
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _iEFHelper = iEFHelper;
            _tsdService = tsdService;

            _crmsContext = new CRMSContext();
            _connectionString = _crmsContext.Database.Connection.ConnectionString;

        }
        public async Task<IEnumerable<RiskReportCategoryDto>> GetReportListAsync(long LoggedUsedId)
        {
            List<RiskReportCategoryDto> riskReportCategoryDto = new List<RiskReportCategoryDto>();
            IEnumerable<RiskReportDto> riskReportDto = null;
            //IEnumerable<RiskReport> riskreport = _riskReportRepository.AsQueryable.ToList();
            IEnumerable<RiskReportCategory> riskReportCategory = _riskReportCategoryRepository.AsQueryable.ToList();

            riskReportDto = _riskReportRepository.AsQueryable.IncludeMultiple(x => x.RiskReportCategory).ToList().Select(x => new RiskReportDto
            {
                RiskReportId = x.Id,
                RiskReportName = x.ReportName,
                RiskReportKey = Convert.ToString(x.ReportKey),
                ReportCategoryId = x.ReportCategoryId,
                ReportCategoryName = x.RiskReportCategory.ReportCategoryName// riskReportCategory.Where(obj => obj.Id == x.ReportCategoryId).Select(obj => obj.ReportCateogryName).SingleOrDefault()
            }).ToList<RiskReportDto>();

            User user = await _userRepository.AsQueryable.Where(x => x.Id == LoggedUsedId).SingleOrDefaultAsync();
            UserRole userRole = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(x => x.Id == user.UserRoleID).SingleOrDefault();


            riskReportDto = riskReportDto.Where(x => MatchReportKey(x.RiskReportKey, userRole.Permissions)).ToList();


            foreach (var reportItem in riskReportCategory)
            {
                RiskReportCategoryDto _riskReportCategory = new RiskReportCategoryDto();
                _riskReportCategory.RiskReports = riskReportDto.Where(obj => obj.ReportCategoryId == reportItem.Id);
                if (_riskReportCategory.RiskReports.Count() > 0)
                {
                    _riskReportCategory.ReportCategoryName = reportItem.ReportCategoryName;
                    _riskReportCategory.RiskReportCategoryId = reportItem.Id;
                    riskReportCategoryDto.Add(_riskReportCategory);
                }
            }
            return riskReportCategoryDto.ToList<RiskReportCategoryDto>();
        }

        private bool MatchReportKey(string key, IEnumerable<Permission> permission)
        {
            bool flag = permission.Where(x => x.Key.Contains(key)).Any();
            return flag;
        }


        public async Task<ReportAdminDto> GetAdminReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            ReportAdminDto reportAdminDto = new ReportAdminDto();

            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }

            //List<ReportClaimDto> reportClaimDto = new List<ReportClaimDto>();
            IEnumerable<ReportClaimDto> reportClaimDto = null;
            var ReportParam = new[]{
                new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@UserIds", reportCriteriaDto.UserIds.ToString()),
                new SqlParameter("@StatusIds", reportCriteriaDto.ClaimStatusIds.ToString())
            };
            switch (reportCriteriaDto.ReportTypeKey)
            {
                case "useractivity":
                    IEnumerable<UserActionLogDto> userActionLogDto = null;
                    var param = new[]{
                        new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                        new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                        new SqlParameter("@UserIds", reportCriteriaDto.UserIds.ToString()),
                         new SqlParameter("@StatusIds", reportCriteriaDto.ClaimStatusIds.ToString())
                    };
                    userActionLogDto = await ExecWithStoreProcedure<UserActionLogDto>(ReportConstants.USPNames.uspGetUserActivityAdminReport, param);
                    reportAdminDto.UserActionLogDtos = userActionLogDto;
                    break;

                case "calldates":
                    reportClaimDto = await ExecWithStoreProcedure<ReportClaimDto>(ReportConstants.USPNames.uspGetCallDateAdminReport, ReportParam);
                    reportAdminDto.ReportClaimDtos = reportClaimDto;
                    break;

                case "assignedcontracts":
                    reportClaimDto = await ExecWithStoreProcedure<ReportClaimDto>(ReportConstants.USPNames.uspGetAssignedContractsAdminReport, ReportParam);
                    reportAdminDto.ReportClaimDtos = reportClaimDto;
                    break;
                case "pendingamountdue":
                    IEnumerable<ReportPaymentDto> reportPaymentDto = null;
                    reportPaymentDto = await ExecWithStoreProcedure<ReportPaymentDto>(ReportConstants.USPNames.uspGetPendingPaymentDueAdminReport, ReportParam);
                    reportAdminDto.ReportPaymentDtos = reportPaymentDto;
                    break;

                case "customerservice":
                    //reportClaimDto = _iEFHelper.ExecuteSQLQuery<ReportClaimDto>("exec uspGetCustomerServiceAdminReport @FromDate=@FromDate,@ToDate=@ToDate,@UserIds=@UserIds,@StatusIds=@StatusIds", ReportParam).ToList();
                    reportClaimDto = await ExecWithStoreProcedure<ReportClaimDto>(ReportConstants.USPNames.uspGetCustomerServiceAdminReport, ReportParam);
                    reportAdminDto.ReportClaimDtos = reportClaimDto;
                    break;
            }

            return reportAdminDto;
        }


        public async Task<IEnumerable<BasicReportDtos>> GetBasicReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            IEnumerable<BasicReportDtos> reportResult = null;
            var param = new[] { 
                new SqlParameter("@DateType",(!string.IsNullOrEmpty(reportCriteriaDto.DateTypeKey))?reportCriteriaDto.DateTypeKey:Convert.DBNull),
                new SqlParameter ("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))?reportCriteriaDto.FromDate:Convert.DBNull),
                new SqlParameter ("@ToDate",(!string.IsNullOrEmpty(reportCriteriaDto.ToDate))?reportCriteriaDto.ToDate:Convert.DBNull),
                new SqlParameter ("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))?reportCriteriaDto.LocationIds:Convert.DBNull),
                new SqlParameter ("@Status",(!string.IsNullOrEmpty(reportCriteriaDto.Status.ToString()))?reportCriteriaDto.Status.ToString():Convert.DBNull),
                new SqlParameter("@IncludeTicket",reportCriteriaDto.IncludeTicket)
            };
            reportResult = await ExecWithStoreProcedure<BasicReportDtos>(ReportConstants.USPNames.uspGetBasicReport, param);
            return reportResult;

        }


        public async Task<AdminFeeCommissionReportDto> GetAdminFeeCommissionReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            AdminFeeCommissionReportDto adminFeeCommissionReportDto = new Contracts.ReportDtos.AdminFeeCommissionReportDto();


            try
            {
                IEnumerable<ClaimWiseAdminFeeCommissionReportDto> claimWiseAdminFeeCommissionReportDto = null;
                IEnumerable<UserWiseAdminFeeCommissionReportDto> userWiseAdminFeeCommissionReportDto = null;


                var param = new[] {             
                new SqlParameter ("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))?reportCriteriaDto.FromDate:Convert.DBNull),
                new SqlParameter ("@ToDate",(!string.IsNullOrEmpty(reportCriteriaDto.ToDate))?reportCriteriaDto.ToDate:Convert.DBNull),
                new SqlParameter ("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))?reportCriteriaDto.LocationIds:Convert.DBNull),                
                new SqlParameter ("@Type",(!string.IsNullOrEmpty(reportCriteriaDto.TicketOrViolations))?reportCriteriaDto.TicketOrViolations:Convert.DBNull),
            
            };

                var param1 = new[] {             
                new SqlParameter ("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))?reportCriteriaDto.FromDate:Convert.DBNull),
                new SqlParameter ("@ToDate",(!string.IsNullOrEmpty(reportCriteriaDto.ToDate))?reportCriteriaDto.ToDate:Convert.DBNull),
                new SqlParameter ("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))?reportCriteriaDto.LocationIds:Convert.DBNull),
                new SqlParameter ("@Type",(!string.IsNullOrEmpty(reportCriteriaDto.LossTypeIds))?reportCriteriaDto.LossTypeIds:Convert.DBNull),
                
            };
                claimWiseAdminFeeCommissionReportDto = await ExecWithStoreProcedure<ClaimWiseAdminFeeCommissionReportDto>(ReportConstants.USPNames.uspAdminFeeCommissionReport, param);
                userWiseAdminFeeCommissionReportDto = await ExecWithStoreProcedure<UserWiseAdminFeeCommissionReportDto>(ReportConstants.USPNames.uspUserWiseAdminFeeCommissionReport, param1);

                //adminFeeCommissionReportDto.ClaimWiseAdminFeeCommissionReportDto = claimWiseAdminFeeCommissionReportDto.Select(x => new ClaimWiseAdminFeeCommissionReportDto()
                //{
                //    Claim = x.Claim,
                //    Contract = x.Contract,
                //    Location = x.Location,
                //    Payment = x.Payment,
                //});
                adminFeeCommissionReportDto.ClaimWiseAdminFeeCommissionReportDto = claimWiseAdminFeeCommissionReportDto.ToList<ClaimWiseAdminFeeCommissionReportDto>();
                adminFeeCommissionReportDto.UserWiseAdminFeeCommissionReportDto = userWiseAdminFeeCommissionReportDto.ToList<UserWiseAdminFeeCommissionReportDto>();
            }
            catch (Exception ex)
            {

                throw;
            }



            return adminFeeCommissionReportDto;
        }



        /// <summary>
        /// to get the Charge type report
        /// </summary>
        /// <param name="reportCriteriaDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ChargeTypeReportDto>> GetChargeTypeReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            //new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull)
            IEnumerable<ChargeTypeReportDto> chargeTypeReportDtoResult = null;
            string sChargeTypeCondition = string.Empty;
            if (reportCriteriaDto.GreaterThanLessThan == 0)
            {
                sChargeTypeCondition = ">" + reportCriteriaDto.GreaterThanLessThanValue;
            }
            else if (reportCriteriaDto.GreaterThanLessThan == 1)
            {
                sChargeTypeCondition = "<" + reportCriteriaDto.GreaterThanLessThanValue;
            }
            var param = new[] {                 
                new SqlParameter("@FromDate", (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? reportCriteriaDto.FromDate : Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? reportCriteriaDto.ToDate : Convert.DBNull),
                new SqlParameter("@Location", (!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds : Convert.DBNull),
                new SqlParameter("@ChargeType", (reportCriteriaDto.ChargeTypeId!=0)? reportCriteriaDto.ChargeTypeId : Convert.DBNull),
                new SqlParameter("@ChargeCondition", (!string.IsNullOrEmpty(sChargeTypeCondition))? sChargeTypeCondition : Convert.DBNull),               
            };
            chargeTypeReportDtoResult = await ExecWithStoreProcedure<ChargeTypeReportDto>(ReportConstants.USPNames.uspGetChargeTypeReport, param);
            return chargeTypeReportDtoResult;
        }


        public async Task<IEnumerable<StolenRecoveredReportDto>> GetStolenRecoveredReportsAsync(ReportCriteriaDto reportCriteriaDto)
        {
            IEnumerable<StolenRecoveredReportDto> stolenRecoveredReportResult = null;
            var param = new[] {                 
                new SqlParameter("@Location", (!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds : Convert.DBNull),
                new SqlParameter("@ClaimStatus", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds.ToString()))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull),
                  
            };

            stolenRecoveredReportResult = await ExecWithStoreProcedure<StolenRecoveredReportDto>(ReportConstants.USPNames.uspGetStolenRecoveredReport, param);
            return stolenRecoveredReportResult;
        }




        public async Task<IEnumerable<DepositDateReportDto>> GetDepositDateReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            IEnumerable<DepositDateReportDto> depositDateReportDto = null;
            var param = new[] { 
                new SqlParameter("@FromDate", (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))?reportCriteriaDto.FromDate:Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))?reportCriteriaDto.ToDate:Convert.DBNull),                              
                new SqlParameter("@PaidFrom", (!string.IsNullOrEmpty(reportCriteriaDto.PaymentTypeId))?reportCriteriaDto.PaymentTypeId:Convert.DBNull)
                
            };
            depositDateReportDto = await ExecWithStoreProcedure<DepositDateReportDto>(ReportConstants.USPNames.uspGetDepositDateReport, param);

            return depositDateReportDto;
        }
        public async Task<IEnumerable<VehicleToBeReleasedReportDto>> GetVahicleToBeReleasedReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            IEnumerable<VehicleToBeReleasedReportDto> vehicleToBeReleasedDto = null;

            var ReportParam = new[]{
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull),
                new SqlParameter("@LossType",(!string.IsNullOrEmpty(reportCriteriaDto.LossTypeIds))? reportCriteriaDto.LossTypeIds.ToString():Convert.DBNull),
            };
            vehicleToBeReleasedDto = await ExecWithStoreProcedure<VehicleToBeReleasedReportDto>(ReportConstants.USPNames.uspGetVehicalsToBeReleased, ReportParam);

            return vehicleToBeReleasedDto;
        }
        public async Task<IEnumerable<WriteOffCustomReportDto>> GetWriteOffReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull),
                new SqlParameter("@LossType",(!string.IsNullOrEmpty(reportCriteriaDto.LossTypeIds))? reportCriteriaDto.LossTypeIds.ToString():Convert.DBNull),
            };
            IEnumerable<WriteOffReportDto> writeOffReportDto = await ExecWithStoreProcedure<WriteOffReportDto>(ReportConstants.USPNames.uspGetWriteOffRerpot, ReportParam);
            IEnumerable<WriteOffCustomReportDto> WriteOffCustomReportDto = MappedWriteOffCustomReportDto(writeOffReportDto);
            return WriteOffCustomReportDto;
        }

        public async Task<IEnumerable<AccountReceivableCustomReportDto>> GetAccountReceivableReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull)
            };
            IEnumerable<AccountReceivableReportDto> accountReceivableReportDto = await ExecWithStoreProcedure<AccountReceivableReportDto>(ReportConstants.USPNames.uspGetAccountReceivableReport, ReportParam);
            IEnumerable<AccountReceivableCustomReportDto> accountReceivableCustomReportDto = MappedAccountReceivableCustomReportDto(accountReceivableReportDto);
            return accountReceivableCustomReportDto;
        }

        public async Task<IEnumerable<ARAgingCustomReportDto>> GetARAgingReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime asOfDate = DateTime.Now;
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(reportCriteriaDto.AsOfDate))
            {
                DateTime.TryParse(reportCriteriaDto.AsOfDate, out asOfDate);
                asOfDate = DateTime.Parse(asOfDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.AgingDays))
            {
                string[] range = reportCriteriaDto.AgingDays.Split('-');
                if (range.Length > 1)
                {
                    startDate = asOfDate.AddDays(-Convert.ToDouble(range[1]));
                    endDate = asOfDate.AddDays(-Convert.ToDouble(range[0]));
                }
                else if (reportCriteriaDto.AgingDays.IndexOf('+') > -1)
                {
                    asOfDate = asOfDate.AddDays(-Convert.ToDouble(reportCriteriaDto.AgingDays.Replace("+", "")));
                }
            }
            var ReportParam = new[]{
                new SqlParameter("@AsOfDate", asOfDate),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull),
                new SqlParameter("@LossType",(!string.IsNullOrEmpty(reportCriteriaDto.LossTypeIds))? reportCriteriaDto.LossTypeIds.ToString():Convert.DBNull),
                new SqlParameter("@StartDate",(startDate!=null)?startDate:Convert.DBNull),
                new SqlParameter("@EndDate",(endDate!=null)?endDate:Convert.DBNull)
            };
            IEnumerable<ARAgingReportDto> aRAgingReportDto = await ExecWithStoreProcedure<ARAgingReportDto>(ReportConstants.USPNames.uspGetAgingReport, ReportParam);
            IEnumerable<ARAgingCustomReportDto> aRAgingCustomReportDto = MappedARAgingCustomReportDto(aRAgingReportDto);
            return aRAgingCustomReportDto;
        }

        public async Task<IEnumerable<ChargeBackLossCustomReportDto>> GetChargeBackLossCustomReportAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull)
            };
            IEnumerable<ChargeBackLossReportDto> chargeBackLossReportDto = await ExecWithStoreProcedure<ChargeBackLossReportDto>(ReportConstants.USPNames.uspGetChargeBackLossReport, ReportParam);
            IEnumerable<ChargeBackLossCustomReportDto> chargeBackLossCustomReportDto = MappedChargeBackLossCustomReportDto(chargeBackLossReportDto);
            return chargeBackLossCustomReportDto;
        }

        public async Task<IEnumerable<VehicleDamageSectionCustomReportDto>> GetVehicleDamageSectionCustomReportDtoAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate", (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@Status", (!string.IsNullOrEmpty(reportCriteriaDto.ClaimStatusIds))? reportCriteriaDto.ClaimStatusIds : Convert.DBNull),
                new SqlParameter("@VehicleType", (!string.IsNullOrEmpty(reportCriteriaDto.VehicalDamageIds))? reportCriteriaDto.VehicalDamageIds: Convert.DBNull)
            };
            IEnumerable<VehicleDamageSectionReportDto> vehicleDamageSectionReportDto = await ExecWithStoreProcedure<VehicleDamageSectionReportDto>(ReportConstants.USPNames.uspGetVehicleDamageSectionReport, ReportParam);
            IEnumerable<VehicleDamageSectionCustomReportDto> vehicleDamageSectionCustomReportDto = MappedVehicleDamageSectionCustomReportDto(vehicleDamageSectionReportDto);
            return vehicleDamageSectionCustomReportDto;
        }

        public async Task<IEnumerable<CollectionCustomReportDto>> GetCollectionReportsAsync(ReportCriteriaDto reportCriteriaDto)
        {
            DateTime fromDate = DateTime.Now, toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reportCriteriaDto.FromDate))
            {
                DateTime.TryParse(reportCriteriaDto.FromDate, out fromDate);
                fromDate = DateTime.Parse(fromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(reportCriteriaDto.ToDate))
            {
                DateTime.TryParse(reportCriteriaDto.ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(reportCriteriaDto.FromDate))? fromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate",(!string.IsNullOrEmpty(reportCriteriaDto.ToDate))? toDate.ToShortDateString() : Convert.DBNull),
                new SqlParameter("@Location",(!string.IsNullOrEmpty(reportCriteriaDto.LocationIds))? reportCriteriaDto.LocationIds :  Convert.DBNull),
                new SqlParameter("@UserIds", (!string.IsNullOrEmpty(reportCriteriaDto.UserIds))? reportCriteriaDto.UserIds : Convert.DBNull),
                new SqlParameter("@IncludeCollection",reportCriteriaDto.IncludeCollection),
                new SqlParameter("@IncludeTicket",reportCriteriaDto.IncludeTicket)
            };
            IEnumerable<CollectionReportDto> collectionReportDto = await ExecWithStoreProcedure<CollectionReportDto>(ReportConstants.USPNames.uspGetCollectionReport, ReportParam);
            IEnumerable<CollectionCustomReportDto> collectionCustomReportDto = MappedCollectionCustomReportDto(collectionReportDto, reportCriteriaDto.ToDate);
            return collectionCustomReportDto;
        }

        public async Task<DataTable> GetAllClaimExport(string fromDate, string toDate, string exportType)
        {
            IEnumerable<DataRow> AllClaimsExport = null;
            DateTime FromDate = DateTime.Now, ToDate = DateTime.Now;
            if (!string.IsNullOrEmpty(fromDate))
            {                
                FromDate = DateTime.ParseExact(fromDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //FromDate = DateTime.Parse(FromDate.ToShortDateString());
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                ToDate = DateTime.ParseExact(toDate,"MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //ToDate = DateTime.Parse(ToDate.ToShortDateString());
            }
            int ExportType = Convert.ToInt32(exportType);
            var ReportParam = new[]{
                 new SqlParameter("@FromDate",(!string.IsNullOrEmpty(fromDate))? FromDate.ToShortDateString() :  Convert.DBNull),
                new SqlParameter("@ToDate",(!string.IsNullOrEmpty(toDate))? ToDate.ToShortDateString() : Convert.DBNull),                
                new SqlParameter("@IsEvenClaims",ExportType)
            };
            //AllClaimsExport = await ExecWithStoreProcedure<DataRow>(ReportConstants.USPNames.uspGetClaimsToExport, ReportParam);
            DataSet ds = GetResultDataSet(ReportConstants.USPNames.uspGetClaimsToExport, ReportParam);
            DataTable dt = ds.Tables[0];

            return dt;
        }
        public static DataTable GetDataTableFromObjects(IEnumerable<object> objects)
        {
            if (objects != null && objects.Count() > 0)
            {
                Type t = objects.ElementAt(0).GetType();
                DataTable dt = new DataTable(t.Name);
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }
                foreach (var o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }

        /// <summary>
        /// Mapper private functions
        /// </summary>
        /// <param name="WriteOffReportDto"></param>
        /// <returns></returns>
        /// 
        /// Write off report
        private IEnumerable<WriteOffCustomReportDto> MappedWriteOffCustomReportDto(IEnumerable<WriteOffReportDto> WriteOffReportDto)
        {
            IEnumerable<WriteOffCustomReportDto> lstWriteOffCustomReportDto = null;
            if (WriteOffReportDto.Any())
            {
                lstWriteOffCustomReportDto = WriteOffReportDto.GroupBy(x => x.LocationId).Select(item => new WriteOffCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    writeOffReportDto = item.ToList(),
                    LocationTotal = item.Count(),
                    FinalRenterTotal = item.Count(),
                    FinalTotalCollected = Math.Round(item.Select(x => x.TotalCollected).Sum(), 2),
                    FinalTotalDue = Math.Round(item.Select(x => x.TotalDue).Sum(), 2),
                    FinalTotalClaims = Convert.ToDouble(item.Select(x => x.Claim).Distinct().Count())
                }).ToList<WriteOffCustomReportDto>();
            }
            return lstWriteOffCustomReportDto;
        }
        //Account receiable report
        private IEnumerable<AccountReceivableCustomReportDto> MappedAccountReceivableCustomReportDto(IEnumerable<AccountReceivableReportDto> accountReceivableReportDto)
        {
            IEnumerable<AccountReceivableCustomReportDto> lstAccountReceivableCustomReportDto = null;
            if (accountReceivableReportDto.Any())
            {
                lstAccountReceivableCustomReportDto = accountReceivableReportDto.GroupBy(x => x.LocationId).Select(item => new AccountReceivableCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    accountReceivableReportDto = item.ToList(),
                    LocationTotal = item.Count(),
                    FinalRenterTotal = item.Count(),
                    FinalTotalCollected = Math.Round(item.Select(x => x.TotalCollected).Sum(), 2),
                    FinalTotalDue = Math.Round(item.Select(x => x.TotalDue).Sum(), 2),
                    FinalTotalClaims = item.Select(x => x.Claim).Distinct().Count(),
                }).ToList<AccountReceivableCustomReportDto>();
            }
            return lstAccountReceivableCustomReportDto;
        }

        //A/R Aging report
        private IEnumerable<ARAgingCustomReportDto> MappedARAgingCustomReportDto(IEnumerable<ARAgingReportDto> aRAgingReportDto)
        {
            IEnumerable<ARAgingCustomReportDto> lstARAgingCustomReportDto = null;
            if (aRAgingReportDto.Any())
            {
                lstARAgingCustomReportDto = aRAgingReportDto.GroupBy(x => x.LocationId).Select(item => new ARAgingCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    aRAgingReportDto = item.ToList(),
                    TotalClaims = item.Count(),
                    TotalReceivables = item.Sum(d => d.TotalDue)
                }).ToList<ARAgingCustomReportDto>();
            }
            return lstARAgingCustomReportDto;
        }

        //Charge Back Loss Report
        private IEnumerable<ChargeBackLossCustomReportDto> MappedChargeBackLossCustomReportDto(IEnumerable<ChargeBackLossReportDto> chargeBackLossReportDto)
        {
            IEnumerable<ChargeBackLossCustomReportDto> lstChargeBackLossCustomReportDto = null;
            if (chargeBackLossReportDto.Any())
            {
                string ContractNos = string.Join(",", chargeBackLossReportDto.Select(x => "'" + x.ContractNo + "'").Distinct().ToArray());
                IEnumerable<TSDOpeningAgentName> TSDOpeningAgentName = _tsdService.GetAgentNameForChageLossReport(ContractNos);

                IEnumerable<ChargeBackLossReportDto> lstChargeBackLossReportDto = chargeBackLossReportDto = chargeBackLossReportDto.Select(x => new ChargeBackLossReportDto()
                {
                    Claim = x.Claim,
                    ContractNo = x.ContractNo,
                    LocationId = x.LocationId,
                    LocationName = x.LocationName,
                    Code = x.Code,
                    ClosedDate = x.ClosedDate,
                    TotalCollected = x.TotalCollected,
                    TotalDue = x.TotalDue,
                    ClosedReason = x.ClosedReason,
                    Renter = x.Renter,
                    AgentName = x.AgentName,
                    OpeningAgent = ((!(TSDOpeningAgentName.Where(y => y.ContractNo.Equals(x.ContractNo)).Any())) ? string.Empty : (TSDOpeningAgentName.Where(y => y.ContractNo.Equals(x.ContractNo)).Select(y => y.OpeningAgentName).SingleOrDefault())),
                    CompanyAbbr = x.CompanyAbbr
                });

                lstChargeBackLossCustomReportDto = lstChargeBackLossReportDto.GroupBy(x => x.LocationId).Select(item => new ChargeBackLossCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    chargeBackLossReportDto = item.ToList(),
                    LocationTotal = item.Count(),
                    FinalRenterTotal = item.Count(),
                    FinalTotalCollected = Convert.ToString(Math.Round(item.Select(x => x.TotalCollected).Sum(), 2)),
                    FinalTotalDue = Convert.ToString(Convert.ToString(Math.Round(item.Select(x => x.TotalDue).Sum(), 2))),
                    FinalTotalClaims = Convert.ToString(item.Select(x => x.Claim).Distinct().Count())
                }).ToList<ChargeBackLossCustomReportDto>();
            }
            return lstChargeBackLossCustomReportDto;
        }

        //Vehicle Damage seciton report
        private IEnumerable<VehicleDamageSectionCustomReportDto> MappedVehicleDamageSectionCustomReportDto(IEnumerable<VehicleDamageSectionReportDto> vehicleDamageSectionReportDto)
        {
            IEnumerable<VehicleDamageSectionCustomReportDto> lstVehicleDamageSectionCustomReportDto = null;
            if (vehicleDamageSectionReportDto.Any())
            {
                lstVehicleDamageSectionCustomReportDto = vehicleDamageSectionReportDto.GroupBy(x => x.LocationId).Select(item => new VehicleDamageSectionCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    vehicleDamageSectionReportDto = item.ToList(),
                    LocationTotal = item.Count(),
                    FinalRenterTotal = item.Count(),
                    FinalTotalCollected = Convert.ToString(Math.Round(item.Select(x => x.TotalCollected).Sum(), 2)),
                    FinalTotalDue = Convert.ToString(Convert.ToString(Math.Round(item.Select(x => x.TotalDue).Sum(), 2)))
                }).ToList<VehicleDamageSectionCustomReportDto>();
            }
            return lstVehicleDamageSectionCustomReportDto;
        }

        //Collection Report Section
        private IEnumerable<CollectionCustomReportDto> MappedCollectionCustomReportDto(IEnumerable<CollectionReportDto> collectionReportDto, string ToDate)
        {
            IEnumerable<CollectionCustomReportDto> lstCollectionCustomReportDto = null;
            if (collectionReportDto.Any())
            {

                IEnumerable<CollectionReportDto> innerCollectionReportDto = collectionReportDto.Select(x => new CollectionReportDto()
                {
                    PayDate = x.PayDate,
                    CloseDate = x.CloseDate,
                    ContractNo = x.ContractNo,
                    Renter = x.Renter,
                    Collected = x.Collected,
                    TotalPayment = x.TotalPayment,
                    ClaimStatusId = x.ClaimStatusId,
                    PayFrom = x.PayFrom,
                    Damage = x.Damage,
                    LossOfUse = x.LossOfUse,
                    AdminFee = x.AdminFee,
                    DiminFee = x.DiminFee,
                    OtherFee = x.OtherFee,
                    SubroTotal = (Convert.ToDecimal(x.Damage) + Convert.ToDecimal(x.LossOfUse) + Convert.ToDecimal(x.AdminFee) + Convert.ToDecimal(x.DiminFee) + Convert.ToDecimal(x.OtherFee)),
                    Status = getClaimStatusForCollectionReport(x.CloseDate, ToDate, x.ClaimStatusId),
                    Agent = x.Agent,
                    LocationId = x.LocationId,
                    CompanyAbbr = x.CompanyAbbr,
                    LocationName = x.LocationName,
                    Code = x.Code,
                    ClaimId = x.ClaimId,
                    WriteOffId = x.WriteOffId,
                    WriteOffTotal = x.WriteOffTotal
                }).ToList();

                IEnumerable<CollectionReportDto> distinctWriteOffId = innerCollectionReportDto.GroupBy(g => new { g.WriteOffId, g.WriteOffTotal }).Select(g => g.First()).ToList();

                var CollectionTotal = innerCollectionReportDto.GroupBy(x => x.ClaimId).Select(x => new
                {
                    claimId = x.Key,
                    ClaimCount = x.Count(),
                    TotalPayment = (x.Select(w => w.TotalPayment).FirstOrDefault()),
                    TotalDamage = x.Select(w => w.Damage).FirstOrDefault(),
                    TotalLossOfUser = x.Select(w => w.LossOfUse).FirstOrDefault(),
                    TotalDiminFee = x.Select(w => w.DiminFee).FirstOrDefault(),
                    TotalAdminFee = x.Select(w => w.AdminFee).FirstOrDefault(),
                    TotalOtherFee = x.Select(w => w.OtherFee).FirstOrDefault(),
                    TotalLocationId = x.Select(w => w.LocationId).FirstOrDefault(),
                    TotalStatus = x.Select(w => w.Status).FirstOrDefault(),
                    TotalWriteOff = distinctWriteOffId.Where(y => y.ClaimId == x.Key).Select(y => y.WriteOffTotal).Sum(),
                    TotalWriteOffId = x.Select(w => w.WriteOffId)
                    //TotalSubro = x.Select(w => w.Damage).SingleOrDefault() + x.Select(w => w.LossOfUser).SingleOrDefault() + x.Select(w => w.DiminFee).SingleOrDefault() + x.Select(w => w.AdminFee).SingleOrDefault() + x.Select(w => w.OtherFee).SingleOrDefault()
                });

                lstCollectionCustomReportDto = innerCollectionReportDto.GroupBy(x => x.LocationId).Select(item => new CollectionCustomReportDto()
                {
                    LocationId = item.Key,
                    Location = item.ToList().Select(x => (x.LocationName + "-" + x.Code)).FirstOrDefault(),
                    collectionReportDto = item.ToList(),
                    LocationTotal = item.Count(),
                    FinalRenterTotal = item.Count(),
                    FinalTotalCollected = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalPayment).Sum()),
                    FinalTotalDamage = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalDamage).Sum()),
                    FinalTotalLossOfUse = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalLossOfUser).Sum()),
                    FinalTotalAdminFee = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalAdminFee).Sum()),
                    FinalTotalDiminFee = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalDiminFee).Sum()),
                    FinalTotalOtherFee = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalOtherFee).Sum()),
                    FinalTotalPending = Convert.ToInt32(CollectionTotal.Where(y => y.TotalLocationId == item.Key && y.TotalStatus == "pending").ToList().Select(x => x.TotalStatus).Count()),
                    FinalTotalClosed = Convert.ToInt32(CollectionTotal.Where(y => y.TotalLocationId == item.Key && y.TotalStatus == "closed").ToList().Select(x => x.TotalOtherFee).Count()),
                    FinalTotalWriteOffTotal = Convert.ToDouble(CollectionTotal.Where(y => y.TotalLocationId == item.Key).ToList().Select(x => x.TotalWriteOff).Sum()),

                    //FinalTotalSubroTotal = Convert.ToDouble((temp.ToList().Select(x => x.TotalSubro).Sum())
                }).ToList<CollectionCustomReportDto>();

            }
            return lstCollectionCustomReportDto;
        }
        private string getClaimStatusForCollectionReport(Nullable<DateTime> CloseDate, string ToDate, int ClaimStatusId)
        {
            string Status = string.Empty;
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(ToDate))
            {
                DateTime.TryParse(ToDate, out toDate);
                toDate = DateTime.Parse(toDate.ToShortDateString());
            }

            if (CloseDate == null)
            {
                Status = "pending";
            }
            else
            {
                Status = "closed";
            }

            if (!string.IsNullOrEmpty(ToDate) && toDate < CloseDate)
            {
                Status = "pending";
            }
            else if (ClaimStatusId == 2 || ClaimStatusId == 47)
            {
                Status = "closed";
            }
            else
            {
                Status = "pending";
            }
            return Status;
        }

        private async Task<IEnumerable<T>> ExecWithStoreProcedure<T>(string query, params object[] parameters) where T : class
        {
            return await _iEFHelper.ExecuteSQLQueryAsync<T>("EXEC " + query, parameters);
        }


        public bool IsAuthorizedForReports(long LoggedUsedId)
        {
            IEnumerable<string> allReports = _riskReportRepository.AsQueryable.Select(x => x.ReportKey);

            User user = _userRepository.AsQueryable.Where(x => x.Id == LoggedUsedId).SingleOrDefault();
            UserRole userRole = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(x => x.Id == user.UserRoleID).SingleOrDefault();


            IEnumerable<string> allowedReports = allReports.Where(x => MatchReportKey(x, userRole.Permissions));

            return allowedReports.Any();
        }

        /// <summary>
        /// Get Data from TSD as dataSets
        /// </summary>
        /// <param name="sqlQueries">Sql Queries separated by semicolon (;) </param>
        /// <returns>Returns one dataset for each sql query</returns>
        private static DataSet GetResultDataSet(string sqlQueries, SqlParameter[] sqlParameter)
        {

            DataSet resultDataSet = new DataSet();
            SqlCommand sqlCmd;
            SqlConnection sqlConnection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

            //GetConnection
            using (sqlConnection = new SqlConnection(_connectionString))
            {
                //OpenConnection
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                //Create Sql connection
                sqlCmd = new SqlCommand(sqlQueries, sqlConnection);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                if (sqlParameter != null)
                {
                    sqlCmd.Parameters.Add(sqlParameter[0]);
                    sqlCmd.Parameters.Add(sqlParameter[1]);
                    sqlCmd.Parameters.Add(sqlParameter[2]);
                }

                //provides communicaton betwen dataset and SQL database
                sqlDataAdapter.SelectCommand = sqlCmd;

                sqlDataAdapter.Fill(resultDataSet);
            }
            return resultDataSet;

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                _crmsContext.Dispose();
                _crmsContext = null;
            }

        }
    }
}
