using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateShopper.Services.Helper
{
    public class LogHelper
    {

        public static void WriteToLogFile(string strMessage, string outputFile)
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
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetLogFilePath()
        {
            //  "LogFile-"
            return GetCommonLogFilePath(string.Empty);
        }
        public static string GetLogFilePath(string FileName)
        {
            return GetCommonLogFilePath(FileName);
        }
        public static string GetCommonLogFilePath(string FileName)
        {
            try
            {
                string InnerFileName = string.Empty;
                InnerFileName = ((!string.IsNullOrEmpty(FileName)) ? FileName : "LogFile");
                // get the base directory

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // search the file below the current directory
                string retFilePath = baseDir + ConfigurationManager.AppSettings["LogFilePath"] + "\\" + InnerFileName + "-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

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

        public static bool CheckDirectory(string strLogPath)
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
