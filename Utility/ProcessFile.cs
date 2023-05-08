using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;

using WinSCP;
using NLog;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessFileConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string sSourceFolder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sDestinationFolder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sFileType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sTargetFilePrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessFileConfig()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProcessFile
    {
        ProcessFileConfig _config;
        static string sConfigFile = "FileProcess.config";

        /// <summary>
        /// 
        /// </summary>
        public ProcessFile()
        {
            _config = GenericSerializer.DeserializeFromXML<ProcessFileConfig>(sConfigFile);
            //_log = CLogger.Instance().getSysLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnzipFile()
        {
            string sSourceDir = @_config.sSourceFolder;
            string sDestDir = @_config.sDestinationFolder;
            string sFileType = _config.sFileType;

            try
            {
                if (!Directory.Exists(sSourceDir))
                {
                    throw new DirectoryNotFoundException();
                }

                var tempFileNames = Directory.EnumerateFiles(sSourceDir, "*.zip", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(Path.GetFileName)
                    .Select(Path.GetFileName);

                if (!Directory.Exists(sDestDir))
                {
                    Directory.CreateDirectory(sDestDir);
                }

                ProcessStartInfo proc = new ProcessStartInfo();
                proc.FileName = @"C:\Program Files\7-zip\7z.exe";

                List<string> fileNames = tempFileNames.ToList<string>();

                string zippedNewDir = sSourceDir + @"\Arch\";
                string newPath = Path.Combine(sDestDir);
                string cmdStr = Path.Combine(@"C:\Program Files\7-Zip", "7z.exe");

                foreach (string file in fileNames)
                {
                    bool bValid = ZipTest(sSourceDir, file);
                    if (!bValid)
                    {
                        clog.Log("Broken file: " + file);
                        continue;
                    }

                    string zippedPath = Path.Combine(sSourceDir, file);
                    string zippedNewPath = Path.Combine(zippedNewDir, file);
                    string argStr = "e " + zippedPath + " -o" + newPath + " " + sFileType + " -r -y";

                    using (Process exeProc = Process.Start(cmdStr, argStr))
                    {
                        exeProc.WaitForExit();
                        if (exeProc.HasExited)
                        {
                            if (exeProc.ExitCode != 0)
                            {
                                clog.Log(string.Format("7z.exe exited with exitcode of {0}", exeProc.ExitCode));
                            }
                        }
                    }


                    if (File.Exists(zippedPath))
                    {
                        File.Delete(zippedPath);
                    }

                    //if (!Directory.Exists(zippedNewDir))
                    //{
                    //    Directory.CreateDirectory(zippedNewDir);
                    //}
                    //if (File.Exists(zippedNewPath))
                    //{
                    //    File.Delete(zippedNewPath);
                    //}
                    //File.Move(zippedPath, zippedNewPath);
                }
            }
            catch (System.Exception Ex)
            {
                clog.Error(Ex, "UnzipFile::run");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ZipFile()
        {
            string TargetFilePrefix = _config.sTargetFilePrefix;
            string sSourceDir = @_config.sSourceFolder;
            string sDestDir = @_config.sDestinationFolder;
            string sFileType = @_config.sFileType;

            int nLoop = 0;
            int _nMaxTrial = 3;

            var tempFileNames = Directory.EnumerateFiles(sSourceDir, sFileType, SearchOption.TopDirectoryOnly)
                .OrderByDescending(Path.GetFileName)
                .Select(Path.GetFileName);
            List<string> fileNames = tempFileNames.ToList<string>();

            while (nLoop < _nMaxTrial)
            {
                string ZipFilename = TargetFilePrefix + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".zip";
                string cmdStr = Path.Combine(@"C:\Program Files\7-Zip", "7z.exe");
                string argStr = "a -tzip " + Path.Combine(sDestDir, ZipFilename) + " " + sSourceDir + sFileType;

                using (Process exeProc = Process.Start(cmdStr, argStr))
                {
                    exeProc.WaitForExit();
                    if (exeProc.HasExited)
                    {
                        bool bValid = ZipTest(sDestDir, ZipFilename);
                        if (exeProc.ExitCode == 0 && bValid)
                        {
                            clog.Log(string.Format("7z.exe zip exited with exitcode of {0}", exeProc.ExitCode));
                            break;
                        }
                    }
                }
                nLoop++;
            }

            if (nLoop == _nMaxTrial)
            {
                clog.Log("7z.exe zip trial exceeded.");
            }

            string RawArchDir = sSourceDir + @"\RawArch\";

            foreach (string file in fileNames)
            {
                string RawFile = Path.Combine(sSourceDir, file);
                string RawArchFile = Path.Combine(RawArchDir, file);

                if (!Directory.Exists(RawArchDir))
                {
                    Directory.CreateDirectory(RawArchDir);
                }

                if (File.Exists(RawArchFile))
                {
                    File.Delete(RawArchFile);
                }
                File.Move(RawFile, RawArchFile);
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="ZipPath"></param>
        /// <returns></returns>
        public bool ZipTest(string FolderPath, string ZipPath)
        {
            string cmdStr = Path.Combine(@"C:\Program Files\7-Zip", "7z.exe");
            string argStr = "t " + Path.Combine(FolderPath, ZipPath) + " -r";

            using (Process exeProc = Process.Start(cmdStr, argStr))
            {
                exeProc.WaitForExit();
                if (exeProc.HasExited)
                {
                    if (exeProc.ExitCode != 0)
                    {
                        clog.Log(string.Format("7z.exe exited with exitcode of {0}", exeProc.ExitCode));
                        return false;
                    }
                }
            }
            return true;
        }

        //Need further implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <returns></returns>
        public bool ZipContentValidation(string FullFileName)
        {
            string cmdStr = Path.Combine(@"C:\Program Files\7-Zip", "7z.exe");
            string argStr = "l " + FullFileName;

            Process exeProc = new Process();
            exeProc.StartInfo.UseShellExecute = false;
            exeProc.StartInfo.RedirectStandardOutput = true;

            using (exeProc = Process.Start(cmdStr, argStr))
            {
                while (!exeProc.StandardOutput.EndOfStream)
                {
                    string line = exeProc.StandardOutput.ReadLine();
                }
                exeProc.WaitForExit();
                if (exeProc.HasExited)
                {
                    if (exeProc.ExitCode != 0)
                    {
                        clog.Log(string.Format("7z.exe exited with exitcode of {0}", exeProc.ExitCode));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
