using System;
using System.IO;
using System.Configuration;
using RateShopper.Services.Data;
using Microsoft.Practices.Unity;
using RateShopper.Data;
using RateShopper.Core.Cache;
using System.Threading.Tasks;

namespace RateShopper.Scheduler
{
	class Program
	{

		// Make Unity resolve the interface, providing an instance      
		static void Main(string[] args)
		{
			try
			{
				//WriteToLogFile("Scheduler Service Started ", GetLogFilePath());
				IUnityContainer unityContainer = new UnityContainer();
				unityContainer.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>();
				unityContainer.RegisterType<ICacheManager, InMemoryCacheManager>();
                unityContainer.RegisterType<IGlobalTetherSettingService, GlobalTetherSettingService>();
                unityContainer.RegisterType<IFormulaService, FormulaService>();
				unityContainer.RegisterType<IStatusesService, StatusesService>();
				unityContainer.RegisterType<ICompanyService, CompanyService>();
				unityContainer.RegisterType<ICarClassService, CarClassService>();
				unityContainer.RegisterType<IRentalLengthService, RentalLengthService>();
				unityContainer.RegisterType<ILocationService, LocationsService>();
				unityContainer.RegisterType<IScrapperSourceService, ScrapperSourceService>();
				unityContainer.RegisterType<IUserScrapperSourcesService, UserScrapperSourcesService>();
				unityContainer.RegisterType<IUserLocationBrandsService, UserLocationBrandsService>();
				unityContainer.RegisterType<ISearchResultSuggestedRatesService, SearchResultSuggestedRatesService>();
				unityContainer.RegisterType<IWeekDayService, WeekDayService>();
				unityContainer.RegisterType<IScheduledJobFrequencyService, ScheduledJobFrequencyService>();
				unityContainer.RegisterType<IScheduledJobMinRatesService, ScheduledJobMinRatesService>();

                unityContainer.RegisterType<IRuleSetService, RuleSetService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetRentalLengthService, RuleSetRentalLengthService>();
                unityContainer.RegisterType<IRuleSetWeekDayService, RuleSetWeekDayService>();

                unityContainer.RegisterType<IRuleSetsAppliedService, RuleSetsAppliedService>();
                unityContainer.RegisterType<IRuleSetCarClassesService, RuleSetCarClassesService>();
                unityContainer.RegisterType<IRuleSetGapSettingService, RuleSetGapSettingService>();
                unityContainer.RegisterType<IRuleSetGroupCompanyService, RuleSetGroupCompanyService>();
                unityContainer.RegisterType<IRuleSetGroupService, RuleSetGroupService>();
                
                
                
                
                unityContainer.RegisterType<IRangeIntervalsService, RangeIntervalsService>();
                
                unityContainer.RegisterType<ILocationBrandService, LocationBrandService>();
				unityContainer.RegisterType<IUserRolesService, UserRolesService>();
				unityContainer.RegisterType<IUserService, UserService>();
				unityContainer.RegisterType<ISearchSummaryService, SearchSummaryService>();
				unityContainer.RegisterType<IScheduledJobService, ScheduledJobService>();
                unityContainer.RegisterType<IScheduledJobTetheringsService, ScheduledJobTetheringsService>();
                unityContainer.RegisterType<IGlobalLimitService, GlobalLimitService>();
                unityContainer.RegisterType<IGlobalLimitDetailService, GlobalLimitDetailService>();
                unityContainer.RegisterType<IProvidersService, ProvidersService>();
                unityContainer.RegisterType<ICallbackResponseService, CallbackResponseService>();
                unityContainer.RegisterType<IScrappingServersService, ScrappingServersService>();
                unityContainer.RegisterType<ISearchResultProcessedDataService, SearchResultProcessedDataService>();
                unityContainer.RegisterType<ISplitMonthDetailsService, SplitMonthDetailsService>();
                unityContainer.RegisterType<IJobTypeMapperService, JobTypeMapperService>();
                unityContainer.RegisterType<ITSDTransactionsService, TSDTransactionService>();

                unityContainer.RegisterType<IFTBTargetService, FTBTargetService>();
                unityContainer.RegisterType<IFTBTargetsDetailService, FTBTargetsDetailService>();
                unityContainer.RegisterType<IFTBRatesService, FTBRatesService>();
                unityContainer.RegisterType<IFTBScheduleJobService, FTBScheduleJobService>();
                unityContainer.RegisterType<IFTBRatesSplitMonthsService, FTBRatesSplitMonthsService>();
                unityContainer.RegisterType<IScheduledJobOpaqueValuesService, ScheduledJobOpaqueValuesService>();
                unityContainer.RegisterType<IRateCodeService, RateCodeService>();
                unityContainer.RegisterType<IRateCodeDateRangeService, RateCodeDateRangeService>();

                unityContainer.RegisterType<ILocationBrandRentalLengthService, LocationBrandRentalLengthService>();
                

                var _cacheManager = unityContainer.Resolve<ICacheManager>();
				var searchSummaryService = unityContainer.Resolve<ISearchSummaryService>();
				var scheduledJobService = unityContainer.Resolve<IScheduledJobService>();

				//WriteToLogFile("Dependency Resolution done", GetLogFilePath());

                //Remove all cache object
                _cacheManager.RemoveAllCacheObjects();

				string runFailedShops = ConfigurationManager.AppSettings["RunFailedShops"];
				if (!string.IsNullOrEmpty(runFailedShops) && bool.Parse(runFailedShops))
				{
					searchSummaryService.SendAsync();
					WriteToLogFile("Request Sent to Search Summary Method", GetLogFilePath());
					WriteToLogFile("Finished Processing failed request", GetLogFilePath());
				}
				string runAutomationShops = ConfigurationManager.AppSettings["RunAutomationShops"];
				if (!string.IsNullOrEmpty(runAutomationShops) && bool.Parse(runAutomationShops))
				{
					WriteToLogFile("Started RunAutomationShops ", GetLogFilePath());
					scheduledJobService.RunAutomationShops();

					WriteToLogFile("End RunAutomationShops \n ", GetLogFilePath());
				}

			}
			catch (Exception ex)
			{
				//throw;
				//TO DO change the logging ELMAH
				WriteToLogFile("Exception Occured,Inner Exception: " + ex.InnerException + " exception Message " + ex.GetBaseException().ToString(), GetLogFilePath());
			}
		}

		//Create Log File

		static void WriteToLogFile(string strMessage, string outputFile)
		{

			try
			{
				string line = DateTime.Now.ToString() + " | ";

				line += strMessage;

				FileStream fs = new FileStream(outputFile, FileMode.Append, FileAccess.Write, FileShare.None);

				StreamWriter swFromFileStream = new StreamWriter(fs);

				swFromFileStream.WriteLine(line);

				swFromFileStream.Flush();

				swFromFileStream.Close();

			}
			catch (Exception ex)
			{

				throw;
			}
		}


		private static string GetLogFilePath()
		{
			try
			{
				// get the base directory

				string baseDir = AppDomain.CurrentDomain.BaseDirectory;

				// search the file below the current directory
				string retFilePath = baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\" + "LogFile-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

				// Console.WriteLine("Relative file path " + ConfigurationManager.AppSettings["LogFilePath"]);
				// if exists, return the path
				if (File.Exists(retFilePath) == true)
					return retFilePath;
				//create a text file
				else
				{
					if (CheckDirectory(baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\") == false)
						return string.Empty;

					FileStream fs = new FileStream(retFilePath,
						  FileMode.OpenOrCreate, FileAccess.ReadWrite);
					fs.Close();
				}

				return retFilePath;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		private static bool CheckDirectory(string strLogPath)
		{
			try
			{
				int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
				string strDirectoryname =
						   strLogPath.Trim().Substring(0, nFindSlashPos);

				if (false == Directory.Exists(strDirectoryname))
					Directory.CreateDirectory(strDirectoryname);
				return true;
			}
			catch (Exception)
			{
				return false;

			}
		}        
	}
}
