using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using NLog;
using NLog.Targets;
using NLog.Internal;
using NLog.Config;

namespace Utility
{
    /// <summary>
    /// logger singleton
    /// </summary>
    public class CLogger
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SYSLOG = "SiP_SysLog.syslog";

        /// <summary>
        /// 
        /// </summary>
        public const string PROTOCOL_LOG = "protocolLog";

        /// <summary>
        /// 
        /// </summary>
        public const string DUTLOG_PREFIX = "SiP_SysLog.DUTlog_";

        private int _maxDUTs = 4;
        /// <summary>
        /// 
        /// </summary>
        public int MAX_DUTS
        {
            get
            {
                return _maxDUTs;
            }
        }

        LoggingRule rule = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool removeRule()
        {
            LoggingConfiguration config = LogManager.Configuration;

            if (rule != null)
            {
                config.LoggingRules.Remove(rule);
            }

            LogManager.Configuration = config;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public bool spawnNewLogFile(string strFileName)
        {
            try
            {
                LoggingConfiguration config = LogManager.Configuration;

                var logFile = new FileTarget();

                logFile.Name = "MyCustom";
                logFile.FileName = strFileName;//string.Format("C:\\Temp\\SPA\\LogFile-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now); 
                logFile.Layout = "${date} | ${message}";
                logFile.CreateDirs = true;

                try
                {
                    if (config != null) config.RemoveTarget(logFile.Name);
                }
                finally
                {

                }
                config.AddTarget(logFile.Name, logFile);

                if (rule != null)
                {
                    config.LoggingRules.Remove(rule);
                }

                rule = new LoggingRule("*", LogLevel.Trace, logFile);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return true;
        }

        private Logger _protocolLogger;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Logger getProtocolLogger()
        {
            return _protocolLogger;
        }


        private Logger _syslogger;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Logger getSysLogger()
        {
            return _syslogger;
        }

        //private Logger logInstance;
        //public Logger getLogger()
        //{
        //    logInstance = LogManager.GetLogger("QnR_Logger");

        //    return logInstance;
        //}

        private List<ConcurrentQueue<CEventMessage>> _arDUTEventMessageQueue = new List<ConcurrentQueue<CEventMessage>>();
        /// <summary>
        /// 
        /// </summary>
		public List<ConcurrentQueue<CEventMessage>> arDUTEventMessagesQueue
        {
            get
            {
                return _arDUTEventMessageQueue;
            }
        }

        private ConcurrentQueue<CEventMessage> _sysEventMessageQueue;
        /// <summary>
        /// 
        /// </summary>
		public ConcurrentQueue<CEventMessage> sysMessageQueue
        {
            get
            {
                return _sysEventMessageQueue;
            }
        }

        private List<Logger> _arLogDUT = new List<Logger>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nDUT"></param>
        /// <returns></returns>
        public Logger getDUTLogger(int nDUT)
        {
            return _arLogDUT[nDUT];
        }

        private static CLogger _instance;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CLogger Instance()
        {
            if (null == _instance)
            {
                _instance = new CLogger();
            }

            return _instance;
        }

        /// <summary>
        /// Simple initialization for just sys log 
        /// </summary>
        public void SimpleInitialize()
        {
            //Instantiate all the loggers
            _syslogger = LogManager.GetLogger(SYSLOG);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMaxDuts"></param>
        public void Initialize(int nMaxDuts)
        {
            _maxDUTs = nMaxDuts;

            try
            {
                if (null == _syslogger)
                    _syslogger = LogManager.GetLogger(SYSLOG);

                _protocolLogger = LogManager.GetLogger(PROTOCOL_LOG);

                _sysEventMessageQueue = new ConcurrentQueue<CEventMessage>();

                _arLogDUT = new List<Logger>();
                Logger refLogger = null;
                for (int i = 0; i < nMaxDuts; i++)
                {
                    refLogger = LogManager.GetLogger(DUTLOG_PREFIX + i.ToString());
                    _arLogDUT.Add(refLogger);

                    _arDUTEventMessageQueue.Add(new ConcurrentQueue<CEventMessage>());
                }
            }
            catch (Exception)
            {
                // Silent exception for the cases where we do NOT have an NLog.config
            }

        }

        private const int MAX_MESSAGES = 200;

        /// <summary>
        /// The method to log messages to the event viewer
        /// </summary>
        /// <param name="myMessage">The message to log</param>
        /// <param name="nQueue">The queue specified to log the message</param>
        /// <param name="myLocationSource">The location source function</param>
        /// <param name="myThreadID">The thread ID where this method is called</param>
        /// <returns></returns>
        public bool logEvent(string myMessage, int nQueue = -1, string myLocationSource = "", int myThreadID = 0)
        {
            CEventMessage message = new CEventMessage(myMessage, nQueue, myLocationSource, myThreadID);

            if (nQueue == -1)
            {
                _sysEventMessageQueue.Enqueue(message);

                if (_sysEventMessageQueue.Count > MAX_MESSAGES)
                    _sysEventMessageQueue.TryDequeue(out message);
            }
            else
            {
                _arDUTEventMessageQueue[nQueue].Enqueue(message);

                if (_arDUTEventMessageQueue[nQueue].Count > MAX_MESSAGES)
                    _arDUTEventMessageQueue[nQueue].TryDequeue(out message);
            }

            return true;
        }

        private CLogger()
        {
            //Instantiate the individual 
        }
    }
}
