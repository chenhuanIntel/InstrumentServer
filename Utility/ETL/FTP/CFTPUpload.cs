using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;
using System.IO;

namespace Utility.ETL.FTP
{
    public class FTPUploadConfig
    {
        public string TransferSourceDir_ToUpload { get; set; }

        public string TransferDestinationDir { get; set; }
        public string TransferCompletedDir { get; set; }
        

        //public string StationName { get; set; }
        public int MaxFiles { get; set; }

        public string FTP_CMName { get; set; }
        public string FTP_RemoteFileNamePattern { get; set; }
    }

    public class CFTPUpload
    {
        NLog.Logger _log = null;

        private FTPUploadConfig _config = null;

        string FTPconfigFile = "FTPUploadConfig.xml";

        public CFTPUpload()
        {
            CLogger.Instance().Initialize(1);
            
            _log = CLogger.Instance().getSysLogger();

            //_config = config;

            _config = GenericSerializer.DeserializeFromXML<FTPUploadConfig>(FTPconfigFile);
        }

        public FTPUploadConfig getConfig()
        {
            return _config;
        }

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
                    //session.DisableVersionCheck = true;
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    transferOptions.FilePermissions = null;
                    transferOptions.PreserveTimestamp = false;
                    //transferOptions.ResumeSupport =  TransferResumeSupport.Equals;

                    //select zip files maximum.
                    //var fileNames = Directory.EnumerateFiles(_config.TransferSourceDir_ToUpload, _config.FTP_RemoteFileNamePattern, SearchOption.TopDirectoryOnly)
                    //.Select(Path.GetFileName)
                    //.Take(_config.MaxFiles);

                    //List<string> files = fileNames.ToList<string>();

                    List<string> files = Utility.CFileOperation.getFilesByExtensions(_config.TransferSourceDir_ToUpload, _config.FTP_RemoteFileNamePattern, SearchOption.TopDirectoryOnly).Take(_config.MaxFiles).ToList(); 
                    int iNumFiles = files.Count;
                    _log.Debug("INFO: CFTPUpload::run: found total of {0} zip files to be moved", iNumFiles);

                    TransferOperationResult transferFileResult;
                    for (int n = 0; n < iNumFiles; n++)
                    {

                        // trasnfer files

                        transferFileResult = session.PutFiles(_config.TransferSourceDir_ToUpload + files[n], _config.TransferDestinationDir, false, transferOptions);

                        // Throw on any error
                        transferFileResult.Check();

                        // Print results
                        foreach (TransferEventArgs transfer in transferFileResult.Transfers)
                        {
                            _log.Debug(String.Format("INFO: CFTPUpload::run: Upload of {0} succeeded", transfer.FileName));
                        }

                        //move file if uploaded successfully
                        string filePath = Path.Combine(_config.TransferSourceDir_ToUpload, files[n]);
                        int iPos = filePath.LastIndexOf(@"\");
                        string shortFilename = filePath.Substring(iPos + 1);
                        try
                        {
                            if (File.Exists(Path.Combine(_config.TransferCompletedDir, shortFilename)) == true)
                            {
                                File.Delete(Path.Combine(_config.TransferCompletedDir, shortFilename));
                                _log.Debug(String.Format("INFO: CFTPUpload::run: file {0} already in {1}, delete it first in {2}",
                                shortFilename, _config.TransferCompletedDir, _config.TransferCompletedDir));
                            }
                            File.Move(filePath, Path.Combine(_config.TransferCompletedDir, shortFilename));
                            _log.Debug(String.Format("INFO: CFTPUpload::run: Move file {0} to {1} succeeded",
                                filePath, _config.TransferCompletedDir));
                        }
                        catch (Exception ex)
                        {
                            _log.Debug(String.Format("ERROR: CFTPUpload::run: Move file {0} to {1} failed",
                                filePath, _config.TransferCompletedDir));
                            bReturn = false;
                        }
                    }
                }

                return bReturn;
            }

            catch (Exception e)
            {
                _log.Debug(String.Format("ERROR: CFTPUpload::run: {0}", e));
                return false;
            }
        }


    }  
    
}
