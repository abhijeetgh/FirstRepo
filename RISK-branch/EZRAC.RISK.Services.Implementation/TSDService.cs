using EZRAC.RISK.EntityFramework;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Services.Contracts.ReportDtos;
using EZRAC.RISK.Services.Implementation.Helper;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class TSDService : ITSDService, IDisposable
    {
        TSDContext _tsdContext = null;

        private static string _connectionString;

        public TSDService()
        {
            _tsdContext = new TSDContext();
            _connectionString = _tsdContext.Database.Connection.ConnectionString;
        }

        public async Task<FetchedContractDetailsDto> GetContractInfoFromTSDAsync(string contractNumber)
        {
            FetchedContractDetailsDto fetchedContractInfo = null;

            try
            {
                if (!String.IsNullOrEmpty(contractNumber))
                {
                    fetchedContractInfo = (await _tsdContext.Database.SqlQuery<FetchedContractDetailsDto>(
                  String.Format(Constants.TSDSqlQueries.FetchContractInfoSql, contractNumber)).ToListAsync()).FirstOrDefault();
                    if (fetchedContractInfo != null)
                    {
                        fetchedContractInfo.LocationCode = MapLocationCodeAsPerRiskApplication(fetchedContractInfo.LocationCode);

                        fetchedContractInfo.SwappedVehicles = GetSwappedVehicles(contractNumber);
                    }
                    
                }

                return fetchedContractInfo;
            }
            catch (Exception)
            {
                //TODO: We can log the exception. Failed to connect to TSD database

                return null;
            }
        }

        public ContractInfoFromTsd GetFullContractInfoFromTSD(string contractNumber, string unitNumber)
        {
            ContractInfoFromTsd fullContractInfo = null;
            try
            {
                if (!String.IsNullOrEmpty(contractNumber))
                {

                    string sqlQuery = GetSqlStringForFullContractInfo(contractNumber);

                    
                    SqlParameter sqlParameter = new SqlParameter("@contractNumber", contractNumber);

                    DataSet fetchedFullContractDetails = GetResultDataSet(sqlQuery, sqlParameter);

                    fullContractInfo = GetContractInfoDTOFromDataSet(fetchedFullContractDetails);

                    if (fetchedFullContractDetails != null && fetchedFullContractDetails.Tables != null && fetchedFullContractDetails.Tables.Count > 0)
                    {
                        IEnumerable<string> listOfVehicles = MapSwappedVehicles(fetchedFullContractDetails.Tables[3], fetchedFullContractDetails.Tables[4]);
                        fullContractInfo.ContractInfo.SwapedVehicles = String.Join(",", listOfVehicles.Distinct().ToList());
                    }

                    if (fullContractInfo != null && (fullContractInfo.VehicleInfo == null || (!String.IsNullOrEmpty(unitNumber) && !fullContractInfo.VehicleInfo.UnitNumber.Equals(unitNumber))))
                    {
                        fullContractInfo.VehicleInfo = GetVehicleInfoFromTSD(unitNumber);
                    }



                }
                return fullContractInfo;
            }
            catch (Exception)
            {
                //TODO: We can log the exception. Failed to connect to TSD database
                return null;

            }

        }

        private static string GetSqlStringForFullContractInfo(string contractNumber)
        {
            string sqlQuery = String.Concat(Constants.TSDSqlQueries.FetchFullContractDetailsSql,//refer to Table[0] in DataSet
            Constants.TSDSqlQueries.FetchAdditionalDriverSql,//refer to Table[1] in DataSet
            String.Format(Constants.TSDSqlQueries.FetchCraExtra, contractNumber),//refer to Table[2] in DataSet
            Constants.TSDSqlQueries.SwapVehicles,//refer to Table[3] in DataSet
            Constants.TSDSqlQueries.AlternateSwapVehicles);//refer to Table[4] in DataSet

            return sqlQuery;
        }

        /// <summary>
        /// To get TSD informattion from DataSet to Dto
        /// </summary>
        /// <param name="fetchedFullContractDetails">Fetched data set</param>
        /// <returns>Contract Info From Tsd as DTO</returns>
        private ContractInfoFromTsd GetContractInfoDTOFromDataSet(DataSet fetchedFullContractDetails)
        {
            DataRow contractBasicInfoFromTsdDs = null;
            DataRow additionalDriverInfo = null;
            ContractInfoFromTsd contractInfoDto = null;

            if (fetchedFullContractDetails != null && fetchedFullContractDetails.Tables.Count > 0)
            {
                if (fetchedFullContractDetails.Tables[0].Rows.Count > 0)
                {
                    contractBasicInfoFromTsdDs = fetchedFullContractDetails.Tables[0].Rows[0];

                    if (fetchedFullContractDetails.Tables[1].Rows.Count>0)
                        additionalDriverInfo = fetchedFullContractDetails.Tables[1].Rows[0];


                    contractInfoDto = new ContractInfoFromTsd();

                    contractInfoDto.OpenLocationCode = MapLocationCodeAsPerRiskApplication((string)contractBasicInfoFromTsdDs["LOC_OUT"]);

                    contractInfoDto.CloseLocationCode = MapLocationCodeAsPerRiskApplication((string)contractBasicInfoFromTsdDs["LOC_IN"]);

                    //If close location is empty we assign open location to close location (logic as per existing system)
                    if (String.IsNullOrEmpty(contractInfoDto.CloseLocationCode))
                        contractInfoDto.CloseLocationCode = contractInfoDto.OpenLocationCode;
                    
                    contractInfoDto.ContractInfo = MapContractInfoDTO(fetchedFullContractDetails, contractBasicInfoFromTsdDs);

                    contractInfoDto.VehicleInfo = MapVehicleInfoDTO(contractBasicInfoFromTsdDs);

                    contractInfoDto.DriverAndInsuranceInfo = MapDriverIncInfoDTO(fetchedFullContractDetails, contractBasicInfoFromTsdDs, additionalDriverInfo);
                }
            }
      
            return contractInfoDto;
        }

        private static IEnumerable<DriverInfoDto> MapDriverIncInfoDTO(DataSet fetchedFullContractDetails, DataRow contractBasicInfo, DataRow additionalDriverInfo)
        {
           
            List<DriverInfoDto> listOfDriverAndIncInfoToReturn = new List<DriverInfoDto>();

            listOfDriverAndIncInfoToReturn.Add(MapEachDriverInformation(contractBasicInfo, ClaimsConstant.DriverTypes.Primary));

            if (additionalDriverInfo!=null)
                listOfDriverAndIncInfoToReturn.Add(MapEachDriverInformation(additionalDriverInfo, ClaimsConstant.DriverTypes.Additional));

            return listOfDriverAndIncInfoToReturn;
        }

        private static DriverInfoDto MapEachDriverInformation(DataRow contractBasicInfo, ClaimsConstant.DriverTypes driveType)
        {
            DriverInfoDto driverAndIncInfoToAdd = new DriverInfoDto();

            driverAndIncInfoToAdd.FirstName = contractBasicInfo["FNAME"] != null ? contractBasicInfo["FNAME"].ToString() : String.Empty;
            driverAndIncInfoToAdd.LastName = contractBasicInfo["LNAME"] != null ? contractBasicInfo["LNAME"].ToString() : String.Empty;
            driverAndIncInfoToAdd.Address1 = contractBasicInfo["STREET1"] != null ? contractBasicInfo["STREET1"].ToString() : String.Empty;
            driverAndIncInfoToAdd.Address2 = contractBasicInfo["STREET2"] != null ? contractBasicInfo["STREET2"].ToString() : String.Empty;
            driverAndIncInfoToAdd.City = contractBasicInfo["CITY"] != null ? contractBasicInfo["CITY"].ToString() : String.Empty;
            driverAndIncInfoToAdd.State = contractBasicInfo["STATE"] != null ? contractBasicInfo["STATE"].ToString() : String.Empty;
            driverAndIncInfoToAdd.Zip = contractBasicInfo["ZIP"] != null ? contractBasicInfo["ZIP"].ToString() : String.Empty;
            driverAndIncInfoToAdd.Phone1 = contractBasicInfo["HPHONE"] != null ? contractBasicInfo["HPHONE"].ToString() : String.Empty;
            driverAndIncInfoToAdd.Phone2 = contractBasicInfo["WPHONE"] != null ? contractBasicInfo["WPHONE"].ToString() : String.Empty;
            driverAndIncInfoToAdd.DOB = contractBasicInfo["DOB"] != null ? DateTime.Parse(contractBasicInfo["DOB"].ToString()) : (DateTime?)null;

            driverAndIncInfoToAdd.LicenceNumber = contractBasicInfo["LIC_NUM"] != null ? contractBasicInfo["LIC_NUM"].ToString() : String.Empty;
            driverAndIncInfoToAdd.LicenceState = contractBasicInfo["LIC_STATE"] != null ? contractBasicInfo["LIC_STATE"].ToString() : String.Empty;
            driverAndIncInfoToAdd.LicenceExpiry = contractBasicInfo["LIC_EXP_DATE"] != null ? DateTime.Parse(contractBasicInfo["LIC_EXP_DATE"].ToString()) : (DateTime?)null;

            if (driveType == ClaimsConstant.DriverTypes.Primary)
            {
                driverAndIncInfoToAdd.Email = contractBasicInfo["Aoth4"] != null ? contractBasicInfo["Aoth4"].ToString() : String.Empty;
            }
            else if (driveType == ClaimsConstant.DriverTypes.Additional)
            {

                driverAndIncInfoToAdd.Email = contractBasicInfo["EMAIL_ADDRESS"] != null ? contractBasicInfo["EMAIL_ADDRESS"].ToString() : String.Empty;
            }
            

            driverAndIncInfoToAdd.DriverTypeId = Convert.ToInt32(driveType);

            return driverAndIncInfoToAdd;
        }

       
        private static VehicleDto MapVehicleInfoDTO(DataRow contractBasicInfo)
        {
            //Vehicle Info
            VehicleDto vehicleInfo = new VehicleDto();

            vehicleInfo.UnitNumber = contractBasicInfo["UNUM"] != null ? contractBasicInfo["UNUM"].ToString() : string.Empty;
            vehicleInfo.TagNumber = contractBasicInfo["PLATE"] != null ? contractBasicInfo["PLATE"].ToString() : string.Empty;
            //vehicleInfo.Description = contractBasicInfo["Description"] != null ? contractBasicInfo["Description"].ToString() : string.Empty;
            vehicleInfo.Make = contractBasicInfo["Make"] != null ? contractBasicInfo["Make"].ToString() : string.Empty;
            vehicleInfo.Model = contractBasicInfo["Model"] != null ? contractBasicInfo["Model"].ToString() : string.Empty;
            vehicleInfo.Year = contractBasicInfo["Year"] != null ? contractBasicInfo["Year"].ToString() : string.Empty;
            vehicleInfo.TagExpires = !String.IsNullOrEmpty(contractBasicInfo["Plate_Expires"].ToString()) ? DateTime.Parse(contractBasicInfo["Plate_Expires"].ToString()) : (DateTime?)null;
            vehicleInfo.VIN = contractBasicInfo["Vin"] != null ? contractBasicInfo["Vin"].ToString() : string.Empty;
            vehicleInfo.Location = contractBasicInfo["Loc_Code"] != null ? contractBasicInfo["Loc_Code"].ToString() : string.Empty;
            vehicleInfo.Color = contractBasicInfo["Color"] != null ? contractBasicInfo["Color"].ToString() : string.Empty;
            vehicleInfo.Mileage = contractBasicInfo["Curr_Miles"] != null ? long.Parse(contractBasicInfo["Curr_Miles"].ToString()) : (long?)null;

            PurchaseType purchaseType;
            if (Enum.TryParse<PurchaseType>(contractBasicInfo["PurchaseType"] != null ? contractBasicInfo["PurchaseType"].ToString() : string.Empty, true, out purchaseType))
            {
                vehicleInfo.PurchaseType = purchaseType;

            }
            
            return vehicleInfo;
        }

        private IEnumerable<string> GetSwappedVehicles(string contractNumber) {

            List<string> swappedVehicles = null;
            try
            {
                string sqlQuery = String.Concat(Constants.TSDSqlQueries.SwapVehicles, Constants.TSDSqlQueries.AlternateSwapVehicles);

                SqlParameter sqlParameter = new SqlParameter("@contractNumber", contractNumber);

                DataSet swapVehiclesDataSet = GetResultDataSet(sqlQuery, sqlParameter);

                if (swapVehiclesDataSet != null && swapVehiclesDataSet.Tables != null && swapVehiclesDataSet.Tables.Count > 0)
                {
                    IEnumerable<string> listOfVehicles = MapSwappedVehicles(swapVehiclesDataSet.Tables[0], swapVehiclesDataSet.Tables[1]);

                    swappedVehicles = listOfVehicles.Distinct().ToList();
                }

                

                //TODO:Need to remove Added for testing

               // swappedVehicles.Add("FCEU343197");
              //  swappedVehicles.Add("FCE1179984");
              //  swappedVehicles.Add("LCDU212879");

                return swappedVehicles;

                //return swappedVehicles;
            }
            catch (Exception)
            {
                
               
                //TODO: We can log the exception. Failed to connect to TSD database
                return null;
            }
        }

        public bool IsContractNumberValid(string contractNumber) {

            bool IsContractValid = false;

            try
            {
               // string sqlQuery = String.Format(Constants.TSDSqlQueries.IsContractValid, contractNumber);

                SqlParameter sqlParameter = new SqlParameter("@contractNumber", contractNumber);

                DataSet isContractValidDataSet = GetResultDataSet(Constants.TSDSqlQueries.IsContractValid, sqlParameter);

                if (isContractValidDataSet != null && isContractValidDataSet.Tables.Count > 0 && isContractValidDataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in isContractValidDataSet.Tables[0].Rows)
                    {
                        IsContractValid = row["ISVALID"] != null ? row["ISVALID"].ToString().Equals("1") : false;
                    }

                }
                return IsContractValid;
            }
            catch (Exception)
            {
                //TODO: We can log the exception. Failed to connect to TSD database
                return false;
            }

            
        }

        public bool IsUnitNumberValid(string unitNumber)
        {

            bool IsUnitNumberValid = false;

            try
            {
                //string sqlQuery = String.Format(Constants.TSDSqlQueries.IsUnitNumberValid, unitNumber);

                SqlParameter sqlParameter = new SqlParameter("@unitNumber", unitNumber);

                DataSet isUnitNumberValidDataSet = GetResultDataSet(Constants.TSDSqlQueries.IsUnitNumberValid, sqlParameter);

                if (isUnitNumberValidDataSet != null && isUnitNumberValidDataSet.Tables.Count > 0 && isUnitNumberValidDataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in isUnitNumberValidDataSet.Tables[0].Rows)
                    {
                        IsUnitNumberValid = row["ISVALID"] != null ? row["ISVALID"].ToString().Equals("1") : false;
                    }

                }
                return IsUnitNumberValid;
            }
            catch (Exception)
            {
                //TODO: We can log the exception. Failed to connect to TSD database
                return false;
            }


        }

        public IEnumerable<TSDOpeningAgentName> GetAgentNameForChageLossReport(string ContractNos)
        {
            try
            {
                string sqlQuery = String.Format(Constants.TSDSqlQueries.FetchOpeningAgentNames, ContractNos);

                //String.Format(sqlQuery, ContractNos);
                //SqlParameter sqlParameter = new SqlParameter("@ContractNo", ContractNos);

                DataSet AgentNameForChageLossReport = GetResultDataSet(sqlQuery, null);
                List<TSDOpeningAgentName> lstTSDOpeningAgentName = new List<TSDOpeningAgentName>();
                if (AgentNameForChageLossReport != null && AgentNameForChageLossReport.Tables.Count > 0 && AgentNameForChageLossReport.Tables[0].Rows.Count > 0)
                {
                    
                    foreach (DataRow row in AgentNameForChageLossReport.Tables[0].Rows)
                    {
                        TSDOpeningAgentName tSDOpeningAgentName = new TSDOpeningAgentName();
                        tSDOpeningAgentName.ContractNo =row["ContractNo"] as string;
                        tSDOpeningAgentName.OpeningAgentName = Convert.ToString(row["FirstName"] as string + " " + row["LastName"] as string);
                        lstTSDOpeningAgentName.Add(tSDOpeningAgentName);
                    }

                }
                return lstTSDOpeningAgentName.ToList();
            }
            catch (Exception)
            {
                //TODO: We can log the exception. Failed to connect to TSD database
                return null;
            }


        }

        public VehicleDto GetVehicleInfoFromTSD(string unitNumber)
        {
            VehicleDto vehicleDto = null;
           
            SqlParameter sqlParameter = new SqlParameter("@unitNumber", unitNumber);

            DataSet vehicleDataSet = GetResultDataSet(Constants.TSDSqlQueries.FetchVehicle, sqlParameter);

            if (vehicleDataSet != null && vehicleDataSet.Tables.Count > 0 && vehicleDataSet.Tables[0].Rows.Count > 0)
            {
                vehicleDto = MapVehicleInfoDTO(vehicleDataSet.Tables[0].Rows[0]);
            }

            return vehicleDto;            
        }

        private static IEnumerable<string> MapSwappedVehicles(DataTable cra1,DataTable swap)
        {
            List<string> SwapedVehicles = new List<string>();
            DataTable vehicles = null;

            if (cra1 != null && swap != null)
            {

                vehicles = cra1;

                if (vehicles.Rows.Count < 2)
                    vehicles.Merge(swap);

            }


            if (vehicles != null)
            {
                foreach (DataRow swapVehicle in vehicles.Rows)
                {
                    if (!String.IsNullOrEmpty(swapVehicle["UNUM"].ToString()))
                    {
                        SwapedVehicles.Add(swapVehicle["UNUM"].ToString());
                    }

                }

            }

         

            return SwapedVehicles.AsEnumerable<string>();
        }


        private static ContractDto MapContractInfoDTO(DataSet fetchedFullContractDetails, DataRow contractBasicInfo)
        {
            //ContractInfo
            ContractDto contractInfoDto = new ContractDto();

            contractInfoDto.ContractNumber = contractBasicInfo["KNUM"] as string;

            contractInfoDto.CardType = contractBasicInfo["CardType"] as string;

            contractInfoDto.CardExpDate = contractBasicInfo["CardExpDate"] as string;

            if (contractBasicInfo["OrigDateOut"] as Nullable<DateTime> == null)
            {
                contractInfoDto.PickupDate = contractBasicInfo["DATE_OUT"] as Nullable<DateTime>;
            }
            else
            {
                contractInfoDto.PickupDate = contractBasicInfo["OrigDateOut"] as Nullable<DateTime>;
            }

            contractInfoDto.MilesIn = contractBasicInfo["MILES_IN"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_IN"]) : default(int);
            contractInfoDto.MilesOut = contractBasicInfo["MILES_OUT"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_OUT"]) : default(int);

            //if contract is still open, pull expected return date
            if (contractBasicInfo["DATE_IN"] as Nullable<DateTime> == null)
            {
                contractInfoDto.ReturnDate = contractBasicInfo["ExpectedReturnDate"] as Nullable<DateTime>;
                contractInfoDto.Miles = 0; //Miles deriven will be zero
            }

            else
            {
                contractInfoDto.ReturnDate = contractBasicInfo["DATE_IN"] as Nullable<DateTime>;
            
                var milesIn = contractBasicInfo["MILES_IN"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_IN"]) : default(int);
                var milesOut = contractBasicInfo["MILES_OUT"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_OUT"]) : default(int);

                contractInfoDto.Miles = milesIn-milesOut;
            }            
            ////if contract is still open, Miles deriven will be zero
            //if (contractBasicInfo["DATE_IN"] as Nullable<DateTime> == null)
            //{
            //    contractInfoDto.Miles = 0;
            //}
            //else
            //{
            //    contractInfoDto.Miles = (contractBasicInfo["MILES_IN"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_IN"]) : default(int))
            //                        - (contractBasicInfo["MILES_OUT"] as Nullable<int> != null ? Convert.ToInt32(contractBasicInfo["MILES_OUT"]) : default(int));
            //}
            // If Daily rate is equal to zero then take weekly rate as daily rate. 
            //This logic is taken from existing system  

           

            if (contractBasicInfo["DailyRate"] as Nullable<decimal> == 0)
            {
                contractInfoDto.DailyRate = Convert.ToDouble(contractBasicInfo["WeeklyRate"] as Nullable<decimal>);
                contractInfoDto.WeeklyRate = contractInfoDto.DailyRate * 5;
            }
            else
            {
                contractInfoDto.DailyRate =  Convert.ToDouble(contractBasicInfo["DailyRate"] as Nullable<decimal>);
                contractInfoDto.WeeklyRate = Convert.ToDouble(contractBasicInfo["WeeklyRate"] as Nullable<decimal>);
            }

            contractInfoDto.DaysOut = contractBasicInfo["RentalDays"] as Nullable<int>;

            //If weekly rate divided by daily rate less than 2 then we make weekly rate equal to 5 multiple of daily rate.
            if (contractInfoDto.WeeklyRate != 0 && contractInfoDto.DailyRate != 0)
            {
                if (contractInfoDto.WeeklyRate / contractInfoDto.DailyRate < 2)
                    contractInfoDto.WeeklyRate = contractInfoDto.DailyRate * 5;
            }

            //for extra day rates (they are saved as rate2 for extra day and then daily goes to rate1)
            if (contractInfoDto.DailyRate > contractInfoDto.WeeklyRate)
            {
                contractInfoDto.DailyRate = contractInfoDto.WeeklyRate;
                contractInfoDto.WeeklyRate = contractInfoDto.DailyRate * 5;
            }


            if (fetchedFullContractDetails.Tables[2].Rows.Count > 0)
            {
                DataRow craExtra = fetchedFullContractDetails.Tables[2].Rows[0];

                contractInfoDto.CDW = GetCraExtraByColumnName(craExtra, "CDW");

                contractInfoDto.SLI = GetCraExtraByColumnName(craExtra, "SLI");

                //If CDW_W is true then CDW is true else no change
                contractInfoDto.CDW = GetCraExtraByColumnName(craExtra, "CDW_W") ? true : contractInfoDto.CDW;

                //If SLI_W is true then SLI is true else no change
                contractInfoDto.SLI = GetCraExtraByColumnName(craExtra, "SLI_W") ? true : contractInfoDto.SLI;

                //If CDWSL or CDW_P or CSL_W is true then CDW and SLI are true else no change
                if (GetCraExtraByColumnName(craExtra, "CDWSL") || GetCraExtraByColumnName(craExtra, "CDW_P") || GetCraExtraByColumnName(craExtra, "CSL_W"))
                {
                    contractInfoDto.CDW = true;
                    contractInfoDto.SLI = true;
                }

                contractInfoDto.LPC = GetCraExtraByColumnName(craExtra, "LPC");

                contractInfoDto.LPC2 = GetCraExtraByColumnName(craExtra, "LPC_2");

                contractInfoDto.GARS = GetCraExtraByColumnName(craExtra, "GARS");

                contractInfoDto.LDW = GetCraExtraByColumnName(craExtra, "LDW");
                
            }

         

            return contractInfoDto;
        }

        private static bool GetCraExtraByColumnName(DataRow craExtra, string columnName)
        {

            if (((craExtra[columnName] as Nullable<int>).HasValue ? (craExtra[columnName] as Nullable<int>).Value : default(int)) == 1)
                return true;
            
            return false;
        }

      
        /// <summary>
        /// Location name are refered by TSD and Risk application in diffrent way. This logic is taken from existing system
        /// </summary>
        /// <param name="tsdLocationCode">Location name as per TSD</param>
        /// <returns>Loction Name as per Risk Application</returns>
        private static string MapLocationCodeAsPerRiskApplication(string tsdLocationCode)
        {
           
            switch (tsdLocationCode)
            {
                case "FTL":
                    return "FLL";
                case "LMAT":
                    return "AT";
                case "LMBT":
                    return "BT";
                case "EZBT":
                    return "MCO";
                default:
                    return tsdLocationCode;
            }

        }

        /// <summary>
        /// Get Data from TSD as dataSets
        /// </summary>
        /// <param name="sqlQueries">Sql Queries separated by semicolon (;) </param>
        /// <returns>Returns one dataset for each sql query</returns>
        private static DataSet GetResultDataSet(string sqlQueries, SqlParameter sqlParameter)
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

                if (sqlParameter != null)
                    sqlCmd.Parameters.Add(sqlParameter);

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

        protected virtual void Dispose(bool disposing) {

            if (disposing)
            {
                _tsdContext.Dispose();
                _tsdContext = null;
            }
        
        }

        public async Task<IEnumerable<SearchTagPlateDto>> GetSearchTagPlate(string tagNumber)
        {
            IEnumerable<SearchTagPlateDto> searchTagPlateDtoList = null;
            if (!String.IsNullOrEmpty(tagNumber))
            {
                searchTagPlateDtoList = (await _tsdContext.Database.SqlQuery<SearchTagPlateDto>(
                 Constants.TSDSqlQueries.SearchTag, new SqlParameter("@tagNumber", "%" + tagNumber + "%")).ToListAsync());
                return searchTagPlateDtoList;
            }
            return searchTagPlateDtoList;
        }
    }
}
