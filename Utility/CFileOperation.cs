using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CFileOpConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string TransferSourceDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransferDestinationDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransferCompletedDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ToSpyglassDBDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CompletedDBDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FailedDBDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DropboxDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProcessedJsonEyeImageDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FailedJsonEyeImageDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool uploadOnly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CLR4Database { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PSM4Database { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CLR4SSTDatabase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool updateRec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nJSN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nJPG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CompletedFileKeepDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FailedFileKeepDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] EmailAddresses { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxZipFiles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CFileOperation
    {
        CFileOpConfig _config = null;
        int nChannel = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public CFileOperation(CFileOpConfig config)
        {
            //_log = CLogger.Instance().getSysLogger();
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="targetFolder"></param>
        /// <param name="bMoveZip"></param>
        /// <param name="bCleanCSV"></param>
        /// <returns></returns>
        public bool fileLogisticsExecution(string sourceFolder, string targetFolder, bool bMoveZip = true, bool bCleanCSV = false)
        {
            bool bReturn = true;
            int n;
            List<string> CSVfiles = null;

            var fileNames = Directory.EnumerateFiles(sourceFolder, "*.jsn", SearchOption.TopDirectoryOnly)
                .Where(x => new FileInfo(x).LastWriteTime < DateTime.Now.AddMinutes(-2))
                .Where(x => new FileInfo(x).Length > 0)
                .Select(Path.GetFileName)
                .Take(_config.nJSN);
            CSVfiles = fileNames.ToList<string>();
            int iNumFiles = CSVfiles.Count;
            clog.Info("INFO: run: found total of {0} jsn files to be moved", iNumFiles);
            for (n = 0; n < iNumFiles; n++)
            {
                string csvFile = Path.Combine(sourceFolder, CSVfiles[n]);
                int iPos = csvFile.LastIndexOf(@"\");
                string shortFilename = csvFile.Substring(iPos + 1);
                try
                {
                    if (File.Exists(Path.Combine(targetFolder, shortFilename)) == true)
                    {
                        File.Delete(Path.Combine(targetFolder, shortFilename));
                        clog.Info(String.Format("INFO: CopyMoveFiles::run: file {0} already in {1}, delete it first in {2}",
                            shortFilename, targetFolder, targetFolder));
                    }
                    File.Move(csvFile, Path.Combine(targetFolder, shortFilename));
                    clog.Log(String.Format("INFO: CopyMoveFiles::run: Move file {0} to {1} succeeded",
                        csvFile, targetFolder));
                }
                catch (Exception ex)
                {
                    clog.Log(ex.ToString());
                    clog.Log(String.Format("ERROR: CopyMoveFiles::run: Move file {0} to {1} failed",
                        csvFile, targetFolder));
                    bReturn = false;
                }
            }

            CSVfiles = GetCompleteFourImageFiles(sourceFolder);

            iNumFiles = CSVfiles.Count;
            clog.Log("INFO: run: found total of {0} jpg files to be moved", iNumFiles);
            for (n = 0; n < iNumFiles; n++)
            {
                string csvFile = Path.Combine(sourceFolder, CSVfiles[n]);
                int iPos = csvFile.LastIndexOf(@"\");
                string shortFilename = csvFile.Substring(iPos + 1);
                try
                {
                    if (File.Exists(Path.Combine(targetFolder, shortFilename)) == true)
                    {
                        File.Delete(Path.Combine(targetFolder, shortFilename));
                        clog.Info(String.Format("INFO: CopyMoveFiles::run: file {0} already in {1}, delete it first in {2}",
                        shortFilename, targetFolder, targetFolder));
                    }
                    File.Move(csvFile, Path.Combine(targetFolder, shortFilename));
                    clog.Info(String.Format("INFO: CopyMoveFiles::run: Move file {0} to {1} succeeded",
                        csvFile, targetFolder));
                }
                catch (Exception ex)
                {
                    clog.Error(ex, string.Format("CopyMoveFiles::run: Move file {0} to {1} failed", csvFile, targetFolder));
                    bReturn = false;
                }
            }

            if (bMoveZip)
            {
                fileNames = Directory.EnumerateFiles(sourceFolder, "*.zip", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName);
                CSVfiles = fileNames.ToList<string>();
                iNumFiles = CSVfiles.Count;
                clog.Info("INFO: run: found total of {0} zip files to be moved", iNumFiles);
                for (n = 0; n < iNumFiles; n++)
                {
                    string csvFile = Path.Combine(sourceFolder, CSVfiles[n]);
                    int iPos = csvFile.LastIndexOf(@"\");
                    string shortFilename = csvFile.Substring(iPos + 1);
                    try
                    {
                        if (File.Exists(Path.Combine(targetFolder, shortFilename)) == true)
                        {
                            File.Delete(Path.Combine(targetFolder, shortFilename));
                            clog.Info(String.Format("INFO: CopyMoveFiles::run: file {0} already in {1}, delete it first in {2}",
                            shortFilename, targetFolder, targetFolder));
                        }
                        File.Move(csvFile, Path.Combine(targetFolder, shortFilename));
                        clog.Info(String.Format("INFO: CopyMoveFiles::run: Move file {0} to {1} succeeded",
                            csvFile, targetFolder));
                    }
                    catch (Exception ex)
                    {
                        clog.Error(ex, string.Format("CopyMoveFiles::run: Move file {0} to {1} failed", csvFile, targetFolder));
                        bReturn = false;
                    }
                }
            }
            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <returns></returns>
        public List<string> GetCompleteFourImageFiles(string sourceFolder)
        {
            var fileNames = Directory.EnumerateFiles(sourceFolder, "*ch1*.jpg", SearchOption.TopDirectoryOnly)
                                .Where(x => new FileInfo(x).LastWriteTime < DateTime.Now.AddMinutes(-2))
                                .Where(x => new FileInfo(x).Length > 0)
                                .Select(Path.GetFileName)
                                .Take(_config.nJPG);

            List<string> jsnFiles = fileNames.ToList<string>();

            List<string> jpgCompleteFiles = new List<string>();

            for (int n = 0; n < jsnFiles.Count; n++)
            {
                string strFilename = jsnFiles[n];

                string MSID = ""; string catergory = "";
                string strTimeStamp = "";
                int channel = 0;
                parseImageFileName(strFilename, ref MSID, ref catergory,
                        ref strTimeStamp, ref channel);

                string partFilename = MSID + "*" + strTimeStamp + ".jpg";

                fileNames = Directory.EnumerateFiles(sourceFolder, partFilename, SearchOption.TopDirectoryOnly)
                 .Select(Path.GetFileName);
                if (fileNames.Count() == nChannel)
                    jpgCompleteFiles.AddRange(fileNames.ToList<string>());
            }

            return jpgCompleteFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFilename"></param>
        /// <param name="MSID"></param>
        /// <param name="catergory"></param>
        /// <param name="strTimeStamp"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected bool parseImageFileName(string strFilename, ref string MSID, ref string catergory,
           ref string strTimeStamp, ref int channel)
        {
            int iPos = strFilename.IndexOf("_");
            MSID = strFilename.Substring(0, iPos);

            if (strFilename.ToLower().Contains("eye"))
                catergory = "HS_EyeTest";
            else if (strFilename.ToLower().Contains("smsr"))
                catergory = "SMSR";

            iPos = strFilename.ToLower().IndexOf("_ch");
            string str = strFilename.Substring(iPos + 3, 2);
            str = str.Remove(str.IndexOf("_"));
            channel = Convert.ToInt16(str);

            int strIdx = strFilename.LastIndexOf("_");
            int stopIdx = strFilename.LastIndexOf(".jpg");

            strTimeStamp = strFilename.Substring(strIdx + 1, stopIdx - strIdx - 1);


            return true;
        }

        /// <summary>
        /// Calculate checksum of given file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string getCheckSumMD5(string filename)
        {
            string sHex = "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    string enCode = Encoding.UTF8.GetString(md5.ComputeHash(stream));
                    foreach (char str in enCode)
                    {
                        sHex = sHex + Convert.ToInt32(str).ToString("X");
                    }
                }
            }
            return sHex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFilename"></param>
        /// <param name="DestinationDir"></param>
        /// <returns></returns>
        public static bool FileMove(string srcFilename, string DestinationDir)
        {
            bool bReturn = true;
            int iPos = srcFilename.LastIndexOf(@"\");
            string fileName = srcFilename.Substring(iPos + 1);
            string dest = Path.Combine(DestinationDir, fileName);
            try
            {


                FileInfo file = new FileInfo(dest);

                if (!file.Directory.Exists)
                    file.Directory.Create();
                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }
                File.Move(srcFilename, dest);
                bReturn = true;
            }
            catch (Exception ex)
            {
                clog.Error(ex, "MoveFileToFailedDir: Failed in MoveFileToProcessedDir: exception");
                bReturn = false;
            }

            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SourceFolder"></param>
        /// <param name="Filter"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> getFilesByExtensions(string SourceFolder, string Filter, System.IO.SearchOption searchOption)
        {
            // ArrayList will hold all file names
            List<string> alFiles = new List<string>();

            // Create an array of filter string
            string[] MultipleFilters = Filter.Split('|');

            // for each filter find mathing file names
            foreach (string FileFilter in MultipleFilters)
            {
                // add found file names to array list
                alFiles.AddRange(Directory.EnumerateFiles(SourceFolder, FileFilter, searchOption).Select(Path.GetFileName));
            }

            // returns string array of relevant file names
            return alFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="bAppend"></param>
        /// <returns></returns>
        public static bool writeContentToFile(string filePath, string content, bool bAppend = false)
        {
            using (StreamWriter writer = new StreamWriter(filePath, bAppend))
            {

                if (bAppend)
                {
                    writer.Write("\r\n" + content);
                }
                else
                    writer.Write(content);
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool WritListStringToFile(string filePath, List<string> content)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < content.Count; i++)
                {
                    if (i == 0)
                        writer.Write(content.ElementAt(i));
                    else
                        writer.Write("\r\n" + content.ElementAt(i));
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localOutputFile"></param>
        /// <returns></returns>
        public static string GetFolderPathByWW(string localOutputFile)
        {
            string res = "";

            DateTime dateNow = DateTime.Now;

            string YYYY = dateNow.Year.ToString();
            string ww = GetIso8601WeekOfYear(dateNow).ToString();

            res = Path.Combine(localOutputFile, YYYY, "WW" + ww);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localOutputFile"></param>
        /// <param name="folders"></param>
        /// <returns></returns>
        public static string CreateLocalOutputFolder(string localOutputFile, params string[] folders)
        {
            string ret = string.Empty;
            DateTime dateNow = DateTime.Now;

            string yyyy_MM = dateNow.ToString("yyyy_MM");
            string MM_dd = dateNow.ToString("MM_dd");
            ret = Path.Combine(localOutputFile, yyyy_MM, MM_dd);

            for (int i = 0; i < folders.Length; i++)
            {
                ret = Path.Combine(ret, folders[i]);
            }

            try
            {
                if (!Directory.Exists(ret))
                    Directory.CreateDirectory(ret);
            }
            catch (Exception ex)
            {
                clog.Error(ex, "CreateLocalOutputFolder: Failed in create local output folder");
                throw;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            Calendar cal = CultureInfo.InvariantCulture.Calendar;
            // Seriously cheat. If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = cal.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return cal.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MSID"></param>
        /// <param name="Type"></param>
        /// <param name="CollName"></param>
        /// <param name="Timestamp"></param>
        /// <param name="DBName"></param>
        /// <param name="bUpdate"></param>
        /// <returns></returns>
        public static string generateFileName(string MSID, string Type, string CollName, string Timestamp, string DBName, bool bUpdate = true)
        {
            string tmp = MSID + "_" + Type + "_" + CollName + "_" + Timestamp + "_" + DBName;

            if (bUpdate)
                return tmp + "_Update";
            else
                return tmp + "_Insert";
        }
    }
}
