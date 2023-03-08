using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;

//Author: Kang He
//Function: Provide general features to download items from FTP.

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class FTPDownLoadConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string TransferSourceDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransferDestinationDir { get; set; }
        //public string TransferCompletedDir { get; set; }

        //public string StationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxFiles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FTP_CMName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FTP_RemoteFileNamePattern { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CFTPDownload
    {
        private FTPDownLoadConfig _config = null;

        string FTPconfigFile = "FTPDownloadConfig.xml";

        /// <summary>
        /// 
        /// </summary>
        public CFTPDownload()
        {
            //CLogger.Instance().Initialize(1);

            //_log = CLogger.Instance().getSysLogger();

            //_config = config;

            _config = GenericSerializer.DeserializeFromXML<FTPDownLoadConfig>(FTPconfigFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FTPDownLoadConfig getConfig()
        {
            return _config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool run()
        {
            bool bReturn = true;
            try
            {
                SessionOptions sessionOptions = null;

                if (_config.FTP_CMName.Contains("SST"))
                {
                    // Setup session options
                    sessionOptions = new SessionOptions
                    {
                        //SST FTP login in info.
                        Protocol = Protocol.Sftp,
                        HostName = @"192.198.165.62",
                        UserName = @"samwang",
                        Password = @"KTE_pass10",
                        SshHostKeyFingerprint = "ssh-rsa 1024 9d:b9:f4:f8:89:5e:ab:85:bd:e9:14:63:2a:a9:c0:11"
                    };
                }
                else
                {
                    sessionOptions = new SessionOptions
                    {
                        //FBN FTP login in info.
                        Protocol = Protocol.Sftp,
                        HostName = @"esft.intel.com",
                        UserName = @"SPOADMIN",
                        Password = @"PSM4_PRQed",
                        SshHostKeyFingerprint = "ssh-rsa 1024 9d:b9:f4:f8:89:5e:ab:85:bd:e9:14:63:2a:a9:c0:11"
                    };
                }


                using (Session session = new Session())
                {

                    TransferOperationResult transferResult = null;

                    try
                    {
                        // trasnfer CSV files
                        session.Open(sessionOptions);

                        // Upload files
                        TransferOptions transferOptions = new TransferOptions();
                        transferOptions.TransferMode = TransferMode.Binary;
                        transferOptions.FilePermissions = null;
                        transferOptions.PreserveTimestamp = false;

                        //List<RemoteFileInfo> fileInfos = session.EnumerateRemoteFiles(_config.TransferSourceDir, _config.FTP_RemoteFileNamePattern, EnumerationOptions.AllDirectories).Take(_config.MaxFiles).ToList();

                        List<RemoteFileInfo> fileInfos = getFilesByExtensions(session, _config.TransferSourceDir, _config.FTP_RemoteFileNamePattern).Take(_config.MaxFiles).ToList();

                        if (fileInfos.Count == 0)
                            return true;

                        foreach (RemoteFileInfo fileInfo in fileInfos)
                        {
                            string localFilePath = RemotePath.TranslateRemotePathToLocal(fileInfo.FullName, _config.TransferSourceDir, _config.TransferDestinationDir);

                            clog.Info(string.Format("Downloading file {0}...", fileInfo.FullName));
                            // Download file
                            string remoteFilePath = RemotePath.EscapeFileMask(fileInfo.FullName);
                            transferResult =
                                session.GetFiles(remoteFilePath, localFilePath);

                            // Did the download succeeded?
                            if (!transferResult.IsSuccess)
                            {
                                // Print error (but continue with other files)
                                Exception ex = new Exception("Download file Error");
                                clog.Error(ex, string.Format("Error downloading file {0}: {1}", fileInfo.FullName, transferResult.Failures[0].Message));
                                throw new Exception("Download file Error");
                            }

                            //Move to Transfer complete folder
                            //session.MoveFile(remoteFilePath, _config.TransferCompletedDir + RemotePath.EscapeFileMask(fileInfo.Name));
                            session.RemoveFiles(remoteFilePath);
                        }

                    }
                    catch (Exception ex)
                    {
                        clog.Error(ex, String.Format("CFTPDownload::run: Move file to {0} failed", _config.TransferSourceDir));
                    }
                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        clog.Info(String.Format("INFO: CFTPDownload::run: Download of {0} succeeded", transfer.FileName));
                    }
                }

                return bReturn;
            }
            catch (Exception e)
            {
                clog.Error(e, "CFTPDownload::run");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="SourceFolder"></param>
        /// <param name="Filter"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public List<RemoteFileInfo> getFilesByExtensions(Session session, string SourceFolder, string Filter, EnumerationOptions searchOption = EnumerationOptions.AllDirectories)
        {
            // ArrayList will hold all file names
            List<RemoteFileInfo> alFiles = new List<RemoteFileInfo>();

            // Create an array of filter string
            string[] MultipleFilters = Filter.Split('|');

            // for each filter find mathing file names
            foreach (string FileFilter in MultipleFilters)
            {
                // add found file names to array list
                alFiles.AddRange(session.EnumerateRemoteFiles(SourceFolder, FileFilter, searchOption));
            }

            // returns string array of relevant file names
            return alFiles;
        }
    }
}
