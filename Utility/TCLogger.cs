using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TCLogger
    {
        /// <summary>
        /// 
        /// </summary>
        public const string TCLOG = "TestCaseLog";
        //private StationHardware _stationInstance;

        //LoggingRule rule = null;

        //public bool removeRule()
        //{
        //    LoggingConfiguration config = LogManager.Configuration;

        //    if (rule != null)
        //    {
        //        config.LoggingRules.Remove(rule);
        //    }

        //    LogManager.Configuration = config;

        //    return true;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strTestCaseLogDir"></param>
        /// <returns></returns>
        public bool CreateNewLogFile(string strFileName, string strTestCaseLogDir)
        {
            try
            {

                var config = MyLogManager.Factory.Configuration;
                //_stationInstance = StationHardware.Instance();
                //var FwrAppConfig = (FwrTestAppConfig)_stationInstance.myConfig.myAppConfig;

                if (config == null)
                {
                    return false;
                }

                //string strTestCaseLogDir = FwrAppConfig.TestCaseLogDir;

                if (!Directory.Exists(strTestCaseLogDir))
                {
                    Directory.CreateDirectory(strTestCaseLogDir);
                }

                var path = strTestCaseLogDir +"\\" + strFileName + "_" + DateTime.Now.ToString("ddMMyyyyhhmmf") + ".txt";

                var target = config.FindTargetByName("TCLogFile");
                if (target == null)
                {
                    var logFile = new FileTarget();

                    logFile.Name = "RunningTC";
                    logFile.FileName = path;//string.Format("C:\\Temp\\SPA\\LogFile-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now); 
                    logFile.Layout = "${date} | ${message}";
                    logFile.CreateDirs = true;

                    config.AddTarget(logFile.Name, logFile);

                }
                else
                {
                    (target as FileTarget).FileName = path;
                }

                //if (rule != null)
                //{
                //    config.LoggingRules.Remove(rule);
                //}

                //rule = new LoggingRule("*", LogLevel.Info, target);
                ////config.LoggingRules.Clear();
                //config.LoggingRules.Add(rule);
                MyLogManager.Factory.Configuration = config;
            }
            catch (Exception)
            {

            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Info(string msg)
        {
            _TCLogger.Info(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Error(string msg)
        {
            _TCLogger.Error(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Debug(string msg)
        {
            _TCLogger.Debug(msg);
        }


        private Logger _TCLogger;
        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            _TCLogger =  MyLogManager.Factory.GetCurrentClassLogger();
        }

        private readonly static TCLogger _instance = new TCLogger();

        /// <summary>
        /// 
        /// </summary>
        public static TCLogger Instance { get { return _instance; } }

        static TCLogger()
        {

        }

        private TCLogger()
        {
            //Instantiate the individual 
            //TCLogger.Instanceger = LogManager.GetLogger(TCLOG);
        }
    }
}
