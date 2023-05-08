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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class clog
    {
        /// <summary>
        /// 
        /// </summary>
        public const string STATION_NAME = "STATION NAME";
        /// <summary>
        /// 
        /// </summary>
        public const string SW_VERSION = "SW VERSION";
        /// <summary>
        /// 
        /// </summary>
        public const int BECONLOGINDEX = -2;
        /// <summary>
        /// 
        /// </summary>
        public enum TimeKey
        {
            /// <summary>
            /// 
            /// </summary>
            SEQUENCE = 0,
            /// <summary>
            /// 
            /// </summary>
            INITIALIZEHW,
            /// <summary>
            /// 
            /// </summary>
            INITIALIZEUI,
            /// <summary>
            /// 
            /// </summary>
            TEST,
            /// <summary>
            /// 
            /// </summary>
            RUN,
            /// <summary>
            /// 
            /// </summary>
            PRETEST,
            /// <summary>
            /// 
            /// </summary>
            STOREDATA,
            /// <summary>
            /// 
            /// </summary>
            ADJUSTSWING,
            /// <summary>
            /// 
            /// </summary>
            ADJUSTPDRATIO,
            /// <summary>
            /// 
            /// </summary>
            MEASURELINEARITYLOOKUP,
            /// <summary>
            /// 
            /// </summary>
            MEASURELINEARITYGRADDESC,
            /// <summary>
            /// 
            /// </summary>
            APPLYTAPANDMEASURE,
            /// <summary>
            /// 
            /// </summary>
            ADJUSTPERCENTEQ,
            /// <summary>
            /// 
            /// </summary>
            POSTTAPADJUSTPDRATIO,
            /// <summary>
            /// 
            /// </summary>
            POSTTAPMEASURELINEARITYGRADDESC,
            /// <summary>
            /// 
            /// </summary>
            TEMPRAMP,
            /// <summary>
            /// 
            /// </summary>
            SETTEMP,
            /// <summary>
            /// 
            /// </summary>
            WAITTEMP,
            /// <summary>
            /// 
            /// </summary>
            SETUPSHUTDOWN,
            // the following added by Hsing-yu for HS           
            /// <summary>
            /// 
            /// </summary>
            InitModule,
            /// <summary>
            /// 
            /// </summary>
            SetupPPG,
            /// <summary>
            /// 
            /// </summary>
            DUTSetup_LineMode,
            /// <summary>
            /// 
            /// </summary>
            ApplyCMISDSP,
            /// <summary>
            /// 
            /// </summary>
            MeasurePAMEye,
            /// <summary>
            /// 
            /// </summary>
            AdjustTargetPowerPerPDratio,
            /// <summary>
            /// 
            /// </summary>
            AdjustTargetPowerPerPDratioSingle,
            /// <summary>
            /// 
            /// </summary>
            AdjustedTargetPower,
            /// <summary>
            /// 
            /// </summary>
            AuxTest,
            /// <summary>
            /// 
            /// </summary>
            GetTxDDMDelta,
            /// <summary>
            /// 
            /// </summary>
            SaveDeviceData,
            /// <summary>
            /// 
            /// </summary>
            ReadAllLineChannelSettings,
            /// <summary>
            /// 
            /// </summary>
            CalibrateChannel,
            /// <summary>
            /// 
            /// </summary>
            InitModuleChannel,
            /// <summary>
            /// 
            /// </summary>
            TxSwitchConnect,
            /// <summary>
            /// 
            /// </summary>
            MeasureEye,
            /// <summary>
            /// 
            /// </summary>
            MeasureScope,
            /// <summary>
            /// 
            /// </summary>
            ReadChannelData,
            /// <summary>
            /// 
            /// </summary>
            SetAndCacheDCAsettings,
            /// <summary>
            /// 
            /// </summary>
            WriteEyesValues,
            /// <summary>
            /// 
            /// </summary>
            DumpCMIS,
            /// <summary>
            /// 
            /// </summary>
            ReadEyeLIN,
            /// <summary>
            /// 
            /// </summary>
            ScanLinearityFunction,
            /// <summary>
            /// 
            /// </summary>
            ApplyInitTapsAndMeasure,
            /// <summary>
            /// 
            /// </summary>
            SaveChannelData,
            /// <summary>
            /// 
            /// </summary>
            AdjustSwing,
            /// <summary>
            /// 
            /// </summary>
            AdjustPDratio_Secant,
            /// <summary>
            /// 
            /// </summary>
            addChannelData,
            /// <summary>
            /// 
            /// </summary>
            AdjustLinearityLookupTable,
            /// <summary>
            /// 
            /// </summary>
            AdjustLinearityGradDesc,
            /// <summary>
            /// 
            /// </summary>
            AdjustLinearityStep,
            /// <summary>
            /// 
            /// </summary>
            MeasureTapFromScope,
            /// <summary>
            /// 
            /// </summary>
            ApplyTapAndMeasure,
            /// <summary>
            /// 
            /// </summary>
            AdjustPercentEQ,
            /// <summary>
            /// Adjust ER via Main Tap
            /// </summary>
            AdjustERViaMainTap,
            /// <summary>
            /// Adjust TDECQ Via Tap Offset
            /// </summary>
            AdjustTDECQViaTapOffset,
            /// <summary>
            /// 
            /// </summary>
            AdjustPDratio,
            /// <summary>
            /// 
            /// </summary>
            AdjustLinearityPostGradDesc,
            /// <summary>
            /// 
            /// </summary>
            CalibrateChannel_finally,
            /// <summary>
            /// 
            /// </summary>
            ReadAndSaveLaserAndQBmode,
            /// <summary>
            /// 
            /// </summary>
            MeasurePowerConsumption,
            /// <summary>
            /// 
            /// </summary>
            MeasureVTandILoad,
            /// <summary>
            /// 
            /// </summary>
            TemperatureDDMDelta,
            /// <summary>
            /// 
            /// </summary>
            ModEnableHP,
            /// <summary>
            /// 
            /// </summary>
            SetLaserAndQBMode,
            /// <summary>
            /// 
            /// </summary>
            ModuleFWWriteAndReboot,
            /// <summary>
            /// 
            /// </summary>
            RunMeasureCycle,
            /// <summary>
            /// 
            /// </summary>
            DoMeasure,
            /// <summary>
            /// 
            /// </summary>
            saveScreenMultiChannel,
            /// <summary>
            /// 
            /// </summary>
            SetupChannel,
            /// <summary>
            /// 
            /// </summary>
            MeasurePAM4,
            /// <summary>
            /// 
            /// </summary>
            RunHSBaseTest,
            // the following for DCA driver
            /// <summary>
            /// 
            /// </summary>
            measurePAM4Parallel,
            /// <summary>
            /// 
            /// </summary>
            CRUIsLocked,
            /// <summary>
            /// 
            /// </summary>
            SetupMeasurement,
            /// <summary>
            /// 
            /// </summary>
            TrigPatLocked,
            /// <summary>
            /// 
            /// </summary>
            AutoScale,
            /// <summary>
            /// 
            /// </summary>
            AcquireRun,
            /// <summary>
            /// 
            /// </summary>
            TEQualizer,
            /// <summary>
            /// 
            /// </summary>
            ExtractMea,
            /// <summary>
            /// 
            /// </summary>
            LoadSetupFile,
            /// <summary>
            /// 
            /// </summary>
            acqWaveform,
            /// <summary>
            /// 
            /// </summary>
            ExecuteOffloadComputeAsync,
            /// <summary>
            /// 
            /// </summary>
            RunOffloadTest,
            /// <summary>
            /// 
            /// </summary>
            switchChannel,
            /// <summary>
            /// 
            /// </summary>
            GetPatternWaves,
            // end of added by Hsing-yu for HS
            /// <summary>
            /// 
            /// </summary>
            OffloadJoin,
            /// <summary>
            /// 
            /// </summary>
            FWupgradeCDB,
            // for ML measurements by Camille
            /// <summary>
            /// 
            /// </summary>
            ApplyMLAndCheck,
            /// <summary>
            /// 
            /// </summary>
            GetMLInputData,
            /// <summary>
            /// 
            /// </summary>
            MLInputParamClone,
            /// <summary>
            /// 
            /// </summary>
            WriteMLInputParamToFile,
            /// <summary>
            /// 
            /// </summary>
            GenMLInputParamTempFile,
            /// <summary>
            /// 
            /// </summary>
            MLOutputParamList,
            /// <summary>
            /// 
            /// </summary>
            GetMLPred,
            /// <summary>
            /// 
            /// </summary>
            GetMLPred_ModelConsumption,
            /// <summary>
            /// 
            /// </summary>
            GetMLPred_ReadPredictedVals,
            /// <summary>
            /// 
            /// </summary>
            TDECQSettingComp,
            /// <summary>
            /// 
            /// </summary>
            ApplyMLTDECQSetting,
            /// <summary>
            /// 
            /// </summary>
            OptCheck,
            /// <summary>
            /// MeasureDSPVGA
            /// </summary>
            MeasureDSPVGA,
            /// <summary>
            /// ModuleSWReset
            /// </summary>
            ModuleSWReset
            // end of ML measurements
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Highlight(string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Highlight(message, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="dutIndex"></param>
        public static void HighlightTestName(string testName, int dutIndex = -1)
        {
            CNewLogger.Instance.HighlightTestName(testName, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dutIndex"></param>
        public static void DisableTestName(int dutIndex = -1)
        {
            CNewLogger.Instance.DisableTestName(dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkStart(string operation, TimeKey key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkStart(operation, key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkStart(string operation, string key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkStart(operation, key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkStart(string key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkStart(key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkEnd(string operation, TimeKey key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkEnd(operation, key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkEnd(string operation, string key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkEnd(operation, key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public static void MarkEnd(string key, int dutIndex = -1)
        {
            CNewLogger.Instance.MarkEnd(key, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Error(System.Exception ex, string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Error(message, ex, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Warn(string source, string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Warn(source, message, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Info(string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Info(message, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Log(string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Log(message, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="param"></param>
        public static void Log(string message, params object[] param)
        {
            CNewLogger.Instance.Log(message, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public static void Trace(string message, int dutIndex = -1)
        {
            CNewLogger.Instance.Trace(message, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="dutIndex"></param>
        public static void Heading(string stationName, int dutIndex = 1)
        {
            CNewLogger.Instance.Heading(stationName, dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CLogEventMediator LogEventMediator()
        {
            return CNewLogger.Instance.cEventMediator;
        }

        /// <summary>
        /// 
        /// </summary>
        public static CLogEventMediator cEventMediator
        {
            get
            {
                return CNewLogger.Instance.cEventMediator;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoggerDetail
    {
        private string _msID = string.Empty;
        // to ignore json self referencing loop on log factory
        // https://dotnetcoretutorials.com/2020/03/15/fixing-json-self-referencing-loop-exceptions/
        // https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
        [JsonIgnore]
        private Logger _logger;
        private List<string> _lstLog;

        /// <summary>
        /// 
        /// </summary>
        // to ignore json self referencing loop on log factory
        [JsonIgnore]
        public Logger Logger
        {
            get { return _logger; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MSID
        {
            get { return _msID; }
            set
            {
                _msID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public LoggerDetail(Logger logger)
        {
            _logger = logger;
            _lstLog = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> FullLog
        {
            get { return _lstLog; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string log
        {
            set { _lstLog.Add(value); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CNewLogger
    {
        private const string SYSLOG = "SysLog";
        private const string DUTLOG_PREFIX = "DUTLog";
        private const string DUTTARGET = "dutTarget";
        private const string BEACONLOG = "BeaconLog";

        private static CNewLogger _instance;
        /// <summary>
        /// 
        /// </summary>
        public CLogEventMediator cEventMediator;

        /// <summary>
        /// bDUTLoggersExists
        /// </summary>
        public bool bDUTLoggersExists = false;
        /// <summary>
        /// bBeaconLoggerExist
        /// </summary>
        public bool bBeaconLoggerExist = false;

        /// <summary>
        /// 
        /// </summary>
        public CNewLogger()
        {
            // add sysLogger
            _dctLoggerDetails.Add(0, new LoggerDetail(_sysLogger));  // index 0
            _dctLoggerDetails.Add(-1, new LoggerDetail(_beaconLogger));  // index -1
            cEventMediator = new CLogEventMediator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myInstance"></param>
        public static void SetLoggerInstance(CNewLogger myInstance)
        {
            _instance = myInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        public static CNewLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CNewLogger();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, LoggerDetail> LoggerDetails
        {
            get
            {
                return _dctLoggerDetails;
            }
        }

        private Logger _sysLogger = LogManager.GetLogger(SYSLOG);
        private Logger _beaconLogger = LogManager.GetLogger(BEACONLOG);
        /// <summary>
        /// _dctLoggerDetails
        /// </summary>
        public Dictionary<int, LoggerDetail> _dctLoggerDetails = new Dictionary<int, LoggerDetail>();
        private LoggingConfiguration _config = LogManager.Configuration;
        private string _sLogicalDrive;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMaxDUTs"></param>
        /// <param name="LogicalDrive"></param>
        public void Initialize(int nMaxDUTs, string LogicalDrive = "")
        {
            if (Regex.IsMatch(LogicalDrive, @"^[d-zD-Z]{1}$"))
            {
                _sLogicalDrive = LogicalDrive.ToUpper();
            }
            else
            {
                _sLogicalDrive = "";
            }

            //if (nMaxDUTs > _config.AllTargets.Count - 1)
            //{
            //    MessageBox.Show("Number of DUTS set in NLog.config file is only " + (_config.AllTargets.Count - 1).ToString());
            //}

            if (_dctLoggerDetails.Count > 0)
            {
                _dctLoggerDetails.Clear();
                // add sysLogger
                _dctLoggerDetails.Add(0, new LoggerDetail(_sysLogger));
                if (_sLogicalDrive != "")
                {
                    LogManager.Configuration.Variables["dir"] = _sLogicalDrive;
                }
            }

            var config = LogManager.Configuration;
            FileTarget dutTarget = (FileTarget)config.FindTargetByName(DUTTARGET);
            for (int i = 1; i <= nMaxDUTs; i++)
            {
                FileTarget newTarget = new FileTarget();
                newTarget.Layout = dutTarget.Layout;
                newTarget.KeepFileOpen = dutTarget.KeepFileOpen;
                newTarget.Encoding = dutTarget.Encoding;
                newTarget.FileName = dutTarget.FileName;

                config.AddTarget($"{DUTTARGET}{i}", newTarget);
                config.LoggingRules.Add(new LoggingRule($"{DUTLOG_PREFIX}{i}", LogLevel.Debug, config.FindTargetByName($"{DUTTARGET}{i}")));
            }
            LogManager.Configuration = config;

            for (int i = 1; i <= nMaxDUTs; i++)
            {
                Logger dutLogger = LogManager.GetLogger(DUTLOG_PREFIX + i.ToString());
                _dctLoggerDetails.Add(i, new LoggerDetail(dutLogger));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeaconLogInit()
        {
            // add beaconLogger
            _dctLoggerDetails[-1] = new LoggerDetail(_beaconLogger);
            bBeaconLoggerExist = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dutIndex"></param>
        /// <param name="MSID"></param>
        /// <param name="stationName"></param>
        public void UpdateDUTLogger(int dutIndex, string MSID, string stationName = "STATIONNAME")
        {
            bDUTLoggersExists = true;
            // 0-based
            dutIndex++;
            // set file name of target
            var config = LogManager.Configuration;
            string target = DUTTARGET + dutIndex.ToString();
            string logger = DUTLOG_PREFIX + dutIndex.ToString();
            DateTime dateNow = DateTime.Now;

            NLog.Targets.FileTarget dutTarget = (NLog.Targets.FileTarget)config.FindTargetByName(target);
            _dctLoggerDetails[dutIndex].FileName = string.Format("C:\\logs\\logs_{0}\\logs_{1}\\Log_{2}_{3}_{4}_{5}.txt", dateNow.ToString("yyyy_MM"), dateNow.ToString("MM_dd"), MSID, stationName, logger, dateNow.ToString("yyyy-MM-dd-HH-mm-ss"));
            if (_sLogicalDrive != "") _dctLoggerDetails[dutIndex].FileName = _dctLoggerDetails[dutIndex].FileName.Replace("C:", $"{_sLogicalDrive}:");
            dutTarget.FileName = _dctLoggerDetails[dutIndex].FileName;

            LogManager.Configuration = config;

            // set MSID of logger
            _dctLoggerDetails[dutIndex].MSID = MSID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="dutIndex"></param>
        public virtual void Heading(string stationName, int dutIndex = 1)
        {
            Highlight(string.Format("{0}: {1}  {2}: {3}", clog.STATION_NAME, stationName, clog.SW_VERSION, ApplicationUtil.getVersionInfo()), dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public void Highlight(string message, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            Logger objLogger = _dctLoggerDetails[dutIndex + 1].Logger;

            _dctLoggerDetails[dutIndex + 1].log = message;

            // add 1 space if odd + 4 spaces at both sides
            int totalLen = (message.Length % 2 == 0 ? message.Length : (message.Length + 1)) + 4 + 4;
            message = message.PadLeft(message.Length + 3, ' ');
            message = message.PadRight(message.Length % 2 == 0 ? message.Length + 4 : message.Length + 3, ' ');

            lock (objLogger)
            {
                objLogger.Info("".PadLeft(totalLen, '*'));
                cEventMediator.LOGGEREvent(this, dutIndex, "".PadLeft(totalLen, '*'));
                objLogger.Info("*" + message + "*");
                cEventMediator.LOGGEREvent(this, dutIndex, "*" + message + "*");
                objLogger.Info("".PadLeft(totalLen, '*'));
                cEventMediator.LOGGEREvent(this, dutIndex, "".PadLeft(totalLen, '*'));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="dutIndex"></param>
        public void HighlightTestName(string operation, int dutIndex = -1)
        {
            _dctLoggerDetails[dutIndex + 1].Operation = operation;
            Highlight($"TestName: {operation}", dutIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dutIndex"></param>
        public void DisableTestName(int dutIndex = -1)
        {
            _dctLoggerDetails[dutIndex + 1].Operation = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkStart(string operation, clog.TimeKey key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                if (logDetail.Operation is null)
                {
                    objLogger.Debug("{0}|{1}|TIME_START{2}", logDetail.MSID, operation, key);
                    _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_START{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), operation, key);
                    cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_START{2}", logDetail.MSID, operation, key);
                }
                else
                {
                    MarkStart(key.ToString(), dutIndex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkStart(string operation, string key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                if (logDetail.Operation is null)
                {
                    objLogger.Debug("{0}|{1}|TIME_START{2}", logDetail.MSID, operation, key);
                    _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_START{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), operation, key);
                    cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_START{2}", logDetail.MSID, operation, key);
                }
                else
                {
                    MarkStart(key, dutIndex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkStart(string key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Debug("{0}|{1}|TIME_START{2}", logDetail.MSID, logDetail.Operation, key);
                _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_START{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), logDetail.Operation, key);
                cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_START{2}", logDetail.MSID, logDetail.Operation, key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkEnd(string operation, clog.TimeKey key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                if (logDetail.Operation is null)
                {
                    objLogger.Debug("{0}|{1}|TIME_END{2}", logDetail.MSID, operation, key);
                    _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_END{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), operation, key);
                    cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_END{2}", logDetail.MSID, operation, key);
                }
                else
                {
                    MarkEnd(key.ToString(), dutIndex);
                }
            }

            //lock (objMarkEnd)
            //{
            //    if (!bDUTLoggersExists) dutIndex = -1;
            //    string msID = CNewLogger.Instance.LoggerDetails[dutIndex + 1].MSID;
            //    CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Debug("{0}|{1}|TIME_END{2}", msID, operation, key);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkEnd(string operation, string key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                if (logDetail.Operation is null)
                {
                    objLogger.Debug("{0}|{1}|TIME_END{2}", logDetail.MSID, operation, key);
                    _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_END{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), operation, key);
                    cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_END{2}", logDetail.MSID, operation, key);
                }
                else
                {
                    MarkEnd(key, dutIndex);
                }
            }

            //lock (objMarkEnd)
            //{
            //    if (!bDUTLoggersExists) dutIndex = -1;
            //    string msID = CNewLogger.Instance.LoggerDetails[dutIndex + 1].MSID;
            //    CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Debug("{0}|{1}|TIME_END{2}", msID, operation, key);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dutIndex"></param>
        public void MarkEnd(string key, int dutIndex = -1)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Debug("{0}|{1}|TIME_END{2}", logDetail.MSID, logDetail.Operation, key);
                _dctLoggerDetails[dutIndex + 1].log = string.Format("{0}|{1}|TIME_END{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), logDetail.Operation, key);
                cEventMediator.LOGGEREvent(this, dutIndex, "{0}|{1}|TIME_END{2}", logDetail.MSID, logDetail.Operation, key);
            }

            //lock (objMarkEnd)
            //{
            //    if (!bDUTLoggersExists) dutIndex = -1;
            //    string msID = CNewLogger.Instance.LoggerDetails[dutIndex + 1].MSID;
            //    CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Debug("{0}|{1}|TIME_END{2}", msID, operation, key);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="dutIndex"></param>
        public void Error(string message, System.Exception ex, int dutIndex)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Log(LogLevel.Debug, message);
                cEventMediator.LOGGEREvent(this, dutIndex, message);
                objLogger.Log(LogLevel.Debug, ex.ToString());
                cEventMediator.LOGGEREvent(this, dutIndex, ex.ToString());
            }

            //lock (objError)
            //{
            //    if (!bDUTLoggersExists) dutIndex = -1;
            //    CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Error(ex, message);
            //    CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Log(LogLevel.Debug, ex.ToString());
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public void Info(string message, int dutIndex)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Info(message);
                cEventMediator.LOGGEREvent(this, dutIndex, message);
            }

            //if (!bDUTLoggersExists) dutIndex = -1;
            //CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Info(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public void Log(string message, int dutIndex)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Debug(message);
                cEventMediator.LOGGEREvent(this, dutIndex, message);
            }

            //if (!bDUTLoggersExists) dutIndex = -1;
            //CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Debug(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public void Warn(string source, string message, int dutIndex)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Warn("WARNING - At {0}, {1}", source, message);
                cEventMediator.LOGGEREvent(this, dutIndex, "WARNING - At {0}, {1}", source, message);
            }

            //if (!bDUTLoggersExists) dutIndex = -1;
            //CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Warn("WARNING - At {0}, {1}", source, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dutIndex"></param>
        public void Trace(string message, int dutIndex)
        {
            if (dutIndex == clog.BECONLOGINDEX && !bBeaconLoggerExist) dutIndex = -1;
            if (dutIndex >= 0 && !bDUTLoggersExists) dutIndex = -1;
            LoggerDetail logDetail = _dctLoggerDetails[dutIndex + 1];
            Logger objLogger = logDetail.Logger;

            lock (objLogger)
            {
                objLogger.Trace(message);
                cEventMediator.LOGGEREvent(this, dutIndex, message);
            }

            //if (!bDUTLoggersExists) dutIndex = -1;
            //CNewLogger.Instance.LoggerDetails[dutIndex + 1].Logger.Trace(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Flush()
        {
            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        public void Log(string msg, params object[] param)
        {
            LoggerDetail logDetail = _dctLoggerDetails[0];
            Logger objLogger = logDetail.Logger;

            if (param == null)
            {
                objLogger.Log(LogLevel.Debug, msg);

            }
            else
            {
                objLogger.Log(LogLevel.Debug, string.Format(msg, param));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        public void Error(Exception ex, string msg, params object[] param)
        {
            LoggerDetail logDetail = _dctLoggerDetails[0];
            Logger objLogger = logDetail.Logger;

            objLogger.Error(ex, msg, param);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CLogEventMediator
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LOGGERMessage> LOGGER;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LOGERRORMessage> LOGERROR;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="dutIndex"></param>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        public void LOGGEREvent(Object caller, int dutIndex, string msg, params object[] param)
        {
            if (param != null) msg = string.Format(msg, param);

            if (null != LOGGER)
            {
                string sSlot = dutIndex >= 0 ? "DUT" + (dutIndex + 1) + "LOG" : "SYSTLOG";
                LOGGER(caller, new LOGGERMessage(sSlot + " " + msg));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="dutIndex"></param>
        /// <param name="msg"></param>
        public void LOGGEREvent(Object caller, int dutIndex, string msg)
        {
            if (null != LOGGER)
            {
                string sSlot = dutIndex >= 0 ? "DUT" + (dutIndex + 1) + "LOG" : "SYSTLOG";
                LOGGER(caller, new LOGGERMessage(sSlot + " " + msg));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        public void LOGGEREvent(Object caller, string msg, params object[] param)
        {
            if (param != null) msg = string.Format(msg, param);

            if (null != LOGGER)
            {
                LOGGER(caller, new LOGGERMessage(msg));
            }
            CNewLogger.Instance.Log(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="msg"></param>
        public void LOGGEREvent(Object caller, string msg)
        {
            if (null != LOGGER)
            {
                LOGGER(caller, new LOGGERMessage(msg));
            }
            CNewLogger.Instance.Log(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        public void LOGERROREvent(Object caller, Exception ex, string msg, params object[] param)
        {
            if (param != null) msg = string.Format(msg, param);

            if (null != LOGERROR)
            {
                LOGERROR(caller, new LOGERRORMessage(ex, msg));
            }
            CNewLogger.Instance.Error(ex, msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        public void LOGERROREvent(Object caller, Exception ex, string msg)
        {
            if (null != LOGERROR)
            {
                LOGERROR(caller, new LOGERRORMessage(ex, msg));
            }
            CNewLogger.Instance.Error(ex, msg);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LOGGERMessage : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_msg"></param>
        public LOGGERMessage(string _msg)
        {
            msg = _msg;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LOGERRORMessage : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Exception ex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ex"></param>
        /// <param name="_msg"></param>
        public LOGERRORMessage(Exception _ex, string _msg)
        {
            ex = _ex;
            msg = _msg;
        }
    }
}
