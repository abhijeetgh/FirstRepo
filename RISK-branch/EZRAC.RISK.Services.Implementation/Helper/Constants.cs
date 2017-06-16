using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation.Helper
{
    internal class Constants
    {

        private Constants() 
        {

        }

        internal struct TSDSqlQueries
        {
            internal const string FetchFullContractDetailsSql = @"SELECT cra001.KNUM, cra001.UNUM, cra001.PLATE, cra001.LNAME, cra001.FNAME, cra001.STREET1, 
                                                                cra001.STREET2, cra001.CITY, cra001.STATE, cra001.ZIP, cra001.HPHONE, cra001.WPHONE, 
                                                                cra001.LIC_NUM, cra001.LIC_STATE, cra001.LIC_EXP_DATE, cra001.DOB, cra001.DATE_IN, 
                                                                cra001.MILES_OUT, cra001.DATE_OUT,cra001.EXP_DATE as ExpectedReturnDate, cra001.OrigDateOut, cra001.MILES_IN, cra001.LOC_OUT, 
                                                                cra001.LOC_IN, cra001.CHECKED_OUT_BY, cra001.CHECKED_IN_BY, cra001.RentalDays, cra001.RateID, 
                                                                Vin, Make, Model, Year, Color, Loc_Code, Swap_Number, Plate_Expires, Description, cemp01.LastName, 
                                                                cemp01.FirstName,Rate1 as WeeklyRate, Rate2 as DailyRate, Curr_Miles, cra001.CONFIRMATION, cDetail.Aoth4, CuFin.PurchaseType,
                                                                cra001.Card_Type as CardType, cra001.Card_Exp_Date as CardExpDate  
                                                                FROM cfleet01, cemp01, craRate, cDetail, CuFin, cra001 where cra001.KNUM = @contractNumber and cra001.UNUM = cfleet01.Num
                                                                and cra001.CHECKED_OUT_BY = cemp01.EmpID and cra001.RateID = craRate.ID and cDetail.Knum=cra001.KNUM and CuFin.UnitNum = cra001.UNUM;";

            internal const string FetchContractInfoSql = @"SELECT cra001.KNUM as ContractNo, cra001.DATE_IN as DateIn,cra001.DATE_OUT as DateOut, 
                                                           cra001.OrigDateOut as OriginalDate, cra001.LNAME as LastName, cra001.FNAME as FirstName,DOB,cra001.LIC_NUM as LicenseNumber, 
                                                           cra001.UNUM as UnitNo,cra001.LOC_IN as LocationCode, CuFin.PurchaseType FROM cra001, cfleet01, cemp01, craRate, CuFin where cra001.KNUM = '{0}' and
                                                           cra001.UNUM = cfleet01.Num and cra001.CHECKED_OUT_BY = cemp01.EmpID and cra001.RateID = craRate.ID and CuFin.UnitNum = cra001.UNUM;";

            internal const string FetchAdditionalDriverSql = "SELECT TOP 1 * FROM CraDriver WHERE KNUM = @contractNumber;";

            internal const string SwapVehicles = "SELECT UNUM FROM cra001 WHERE KNUM = @contractNumber;";

            internal const string AlternateSwapVehicles = "SELECT UNUM FROM Swap WHERE KNUM = @contractNumber;";

            internal const string IsContractValid = "SELECT COUNT(*) as ISVALID FROM cra001 WHERE KNUM = @contractNumber;";

            internal const string IsUnitNumberValid = "SELECT COUNT(*) as ISVALID FROM cfleet01 WHERE Num = @unitNumber;";

            internal const string SearchTag = "SELECT KNUM as UnitNumber, MSG as Messages, TRANS_DATE as TransDate FROM TRANSLOG WHERE BGN01 = '94' AND MSG LIKE @tagNumber";

            internal const string FetchOpeningAgentNames = "SELECT cemp01.FirstName as FirstName, cemp01.LastName as LastName,cra001.KNUM as ContractNo FROM cra001, cemp01 WHERE cra001.knum in ({0}) AND cra001.CHECKED_OUT_BY = cemp01.EmpID;";

            internal const string FetchCraExtra = @"SELECT distinct Knum,
                                                                CDW = 
                                                                count((CASE WHEN code = 'CDW' THEN 'true' END))
                                                                ,SLI= 
                                                                count((CASE WHEN code ='SLI' THEN 'true' END))
		                                                        ,CDW_W= 
                                                                count((CASE WHEN code ='CDW_W' THEN 'true' END))
		                                                        ,SLI_W= 
                                                                count((CASE WHEN code ='SLI_W' THEN 'true' END))
		                                                        ,CDWSL= 
                                                                count((CASE WHEN code ='CDWSL' THEN 'true' END))
		                                                        ,CDW_P= 
                                                                count((CASE WHEN code ='CDW_P' THEN 'true' END))
		                                                        ,CSL_W= 
                                                                count((CASE WHEN code ='CSL_W' THEN 'true' END))
		                                                        ,LPC= 
                                                                count((CASE WHEN code ='LPC' THEN 'true' END))
		                                                        ,LPC_2= 
                                                                count((CASE WHEN code ='LPC-2' THEN 'true' END))
		                                                        ,GARS= 
                                                                count((CASE WHEN code ='GARS' THEN 'true' END))
		                                                        ,LDW= 
                                                                count((CASE WHEN code ='LDW' THEN 'true' END))
                                                            FROM dbo.Craextra p  
                                                         group by Knum having
                                                         Knum = '{0}';";

            internal const string FetchVehicle = "SELECT DISTINCT Num as UNUM, Vin, cfleet01.PLATE, Make, Model, Year,Loc_Code, Color, Curr_Miles,Plate_Expires,CuFin.PurchaseType FROM cfleet01, CuFin WHERE Num = @unitNumber and CuFin.UnitNum = cfleet01.Num;";
        }

        internal struct Roles
        {

            
            internal const int Administrator = 1;
            internal const int RiskAgent = 2;
            internal const int RiskManager = 3;
            internal const int RiskSupervisor = 4;
            internal const int LocationManager = 5;
        
        }

        internal struct ClaimStatus
        {

            internal const int Open = 3;
            internal const int Closed = 46;

        
        }

        public struct AppSettings {

            internal const string FollowUpAddDays = "FollowUpAddDays";
            internal const string LPC2Deductible = "LPC2Deductible";
        
        }

        internal enum BillingTypes : int
        {
            AdminChange = 1,
            Estimate = 2,
            LossOfUse = 3,
            ActualCashValue = 13,
            Salvage = 14,
        }

        internal enum PaymentTypes : int
        {
            Cdw = 16,
            Ldw = 17,
            Lpc2 = 19,
            
        }
    }
}
