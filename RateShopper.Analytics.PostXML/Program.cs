using Microsoft.Practices.Unity;
using RateShopper.Core.Cache;
using RateShopper.Data;
using RateShopper.Services.Data;
using RateShopper.Services.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Analytics.PostXML
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IUnityContainer unityContainer = new UnityContainer();
                unityContainer.RegisterType<IEZRACRateShopperContext, EZRACRateShopperContext>();
                unityContainer.RegisterType<ICacheManager, InMemoryCacheManager>();

                unityContainer.RegisterType<IPostXMLLogsService, PostXMLLogsService>();
                unityContainer.RegisterType<IPostJSONLogService, PostJSONLogService>();

                var _cacheManager = unityContainer.Resolve<ICacheManager>();
                var postXMLLogsService = unityContainer.Resolve<IPostXMLLogsService>();
                var postJSONLogsService = unityContainer.Resolve<IPostJSONLogService>();
                string JSONflag = Convert.ToString(ConfigurationManager.AppSettings["PushJSONStartFlag"]);
                if (JSONflag == "true")
                {
                    string jsonResponse = postJSONLogsService.PostJSON().Result;
                    LogHelper.WriteToLogFile("Push JSON Ended with - ", LogHelper.GetLogFilePath("PushJSON"));
                }

                WriteToLogFile("Push XML Started ", GetLogFilePath());
                string response = postXMLLogsService.PostXML().Result;

                WriteToLogFile("Push XML Ended with - " + response, GetLogFilePath());
            }
            catch (Exception ex)
            {
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
