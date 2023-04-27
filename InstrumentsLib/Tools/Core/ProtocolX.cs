using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Utility;
using System.IO;
using System.Runtime.Serialization;

namespace InstrumentsLib.Tools.Core
{
    // data contract must be applied to any class that is intended to be seralized, even in the all parent classes
    // data member must also be applied to any property of any class if the property is intended to be serialed
    [DataContract] // must declar DataContract to allow JSON to serialize it in WCF
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProtocolXConfig : CDynamicConfig
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bSimulation { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bVerbose { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bUseSimData { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public CSimData mySimData { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int MaxRetries { get; set; }

        [DataMember]
        /// <summary>
        /// Take Screen Shot while has exception
        /// </summary>
        public bool bTakeScreenShot { get; set; }

        /// <summary>
        /// Contructor of ProtocolXConfig
        /// </summary>
        public ProtocolXConfig()
        {
            buildTargetClassInfo(typeof(ProtocolX));
            MaxRetries = 6;
            bTakeScreenShot = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class ProtocolX
    {
        /// <summary>
        /// 
        /// </summary>
        public int _defaultBufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolXConfig _config;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return _config.strName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public ProtocolX(ProtocolXConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="myparams"></param>
        protected void log(string strFormat, params Object[] myparams)
        {
            //if (null == _protocolLog)
            //    return;

            if (this._config.bVerbose)
            {
                string strMsg = string.Format(strFormat, myparams);
                clog.Info(strMsg);
                Debug.WriteLine(strMsg);
            }
        }

        /// <summary>
        /// Hold process for sleep_milliseconds
        /// </summary>
        /// <param name="sleep_milliseconds">Time to delay</param>
        protected void Pause(int sleep_milliseconds)
        {
            log("Pause({0})", sleep_milliseconds);
            if (sleep_milliseconds >= 50)
            {
                Thread.Sleep(sleep_milliseconds);
            }
            else
            {
                DateTime dt = DateTime.Now;
                while ((DateTime.Now - dt).TotalMilliseconds < sleep_milliseconds) { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public virtual void clearBuffer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int ReadSRQ()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual List<byte> readTillEnd()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBytes"></param>
        /// <returns></returns>
        public virtual string read(int nBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public virtual byte[] readByte(int n)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ReadByteArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCount"></param>
        /// <returns></returns>
		public virtual byte[] ReadBytes(int nCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual string ReadDataString(string cmd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual bool write(string cmd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arBytes"></param>
        /// <returns></returns>
		public virtual bool writeBytes(byte[] arBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="bNoRetry"></param>
        /// <returns></returns>
        public virtual string query(string cmd, bool bNoRetry = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual int queryInt32(string cmd)
        {
            Int32 result = 0;

            if (_config.bSimulation)
            {
                if (null != _config.mySimData)
                {
                    return _config.mySimData.IntData.getSimData();
                }
                else
                {
                    return 0;
                }
            }

            for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
            {
                try
                {
                    result = Convert.ToInt32(query(cmd));
                    return result;//This is a good result
                }
                catch (Exception ex)
                {
                    //Log exception
                    clog.Error(ex, "queryInt32 Exception");
                    if (nTry == (_config.MaxRetries - 1))
                        return -999;
                    //throw;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual double queryDouble(string cmd)
        {
            double result = 0.0;

            if (_config.bSimulation)
            {
                if (null != _config.mySimData)
                {
                    return _config.mySimData.DoubleData.getSimData();
                }
                else
                {
                    return 0.0;
                }
            }

            for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
            {
                try
                {
                    result = Convert.ToDouble(query(cmd));
                    return result;//This is a good result
                }
                catch (Exception ex)
                {
                    //Log exception
                    clog.Error(ex, "queryDouble Exception");

                    if (nTry == (_config.MaxRetries - 1))
                        return -999;
                    //throw;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTimeoutSec"></param>
        /// <returns></returns>
        public virtual bool setTimeout(int nTimeoutSec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool initialize()
        {
            //_protocolLog = CLogger.Instance().getProtocolLogger();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMethodName"></param>
        protected void OSScreenShot(string strMethodName)
        {
            if (!this._config.bTakeScreenShot) return;

            string strlocalFilePath = Utility.CFileOperation.CreateLocalOutputFolder(".", "Raw");
            strlocalFilePath = Path.Combine(strlocalFilePath, $"{strMethodName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg");

            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.WorkingDirectory = Environment.CurrentDirectory;
            clog.Log($"WorkingDirectory = {procStartInfo.WorkingDirectory}");
            procStartInfo.Arguments = $"Keysight.N1000 {strlocalFilePath}";
            procStartInfo.FileName = "ScreenShot.exe";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void close()
        {
            throw new NotImplementedException();
        }
    }
}
