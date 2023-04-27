using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;
using Utility;
using System.IO;
using NationalInstruments.VisaNS;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    [Serializable]
    // data contract must be applied to any class that is intended to be seralized, even in the all parent classes
    // data member must also be applied to any property of any class if the property is intended to be serialed
    [DataContract]
    /// <summary>
    /// Generic Scope Configuration Property Class
    /// </summary>
    public abstract class ScopeConfig : InstrumentXConfig
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double dMaxBusyTimeoutSecs { get; set; }
        
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double dMaxCRULockTimeoutSecs { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double dMaxLoadSetupTimeoutSecs { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double iMaxCRULockRetry { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string strDCA_ID { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public List<string> arChannels { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double dImageSaveWaitTime { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string sRFSwitchNameForTrigger { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bShrinkEyeFile { get; set; }

        [DataMember]
        /// <summary>
        /// Checks if pattern lock is completed using the much more slower OPC command.  
        /// Please set to false if we only want to check if the pattern is locked
        /// </summary>
        public bool bCheckBusyAfterPatternLock { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public long CompressLevel { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, DCASettings> mapDCASettings { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, MeasurementCmd> mapOverridenMeasCmds { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public double TDECQOffset { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public List<string> OERFactor { get; set; }

        [DataMember]
        /// <summary>
        /// TDECQ minimum offset limit
        /// </summary>
        public double OffsetTDECQMinLimit { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public List<DCA_A86100C_Flex400G_V3.ComputeInstrumentBase> arDCAMHost { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public List<DCA_A86100C_Flex400G_V3.ComputeInstrumentBase> arComputeHost { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bTakeScreenShot { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bAutoScaleScreenShot { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bDCAMWithDedicatedPC { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int nRetryAutoScale { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bAutoScaleScreenShotWhenException { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bRelaunchDCAWhenException { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int iRelaunchDCATimeOut_min { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bDebugRelaunchDCA { get; set; }

        [DataMember]
        /// <summary>
        /// Linar fit dictionary of X, Y points
        /// </summary>
        public Dictionary<double, double> mapTDECQPiecewiseLinearFit { get; set; }

        [DataMember]
        /// <summary>
        /// Flag to use AOP and OER to calculate to get OOMA
        /// </summary>
        public bool bUseOOMACalcMethod { get; set; }

        /// <summary>
        /// Default constructor for ScopeConfig
        /// </summary>
        public ScopeConfig()
        {

        }
    }

    /// <summary>
    /// Configuration Class of DCA_A86100CFlex400GConfig
    /// </summary>
    public class DCA_A86100CFlex400GConfig : ScopeConfig
    {

        /// <summary>
        /// Default constructor for DCA_A86100CFlex400GConfig
        /// </summary>
        public DCA_A86100CFlex400GConfig()
        {
            arChannels = new List<string>
            {
                "1Ax",
                "1B",
                "1Cx",
                "1D"
            };

            dImageSaveWaitTime = 0;

            sRFSwitchNameForTrigger = "";

            bShrinkEyeFile = true;

            CompressLevel = 8;

            mapDCASettings = new Dictionary<string, DCASettings>();

            TDECQOffset = 0.0;
            OffsetTDECQMinLimit = 1.0;

            buildTargetClassInfo(typeof(DCA_A86100C_Flex400G));
            bCheckBusyAfterPatternLock = false;
            dMaxBusyTimeoutSecs = 600;
            dMaxCRULockTimeoutSecs = 120;
            iMaxCRULockRetry = 2;
            dMaxCRULockTimeoutSecs = 30;
            iMaxCRULockRetry = 2;
            dMaxLoadSetupTimeoutSecs = 5;
            bTakeScreenShot = false;
            bAutoScaleScreenShot = false;
            bDCAMWithDedicatedPC = false;
            nRetryAutoScale = 3;
            bAutoScaleScreenShotWhenException = false;
            bRelaunchDCAWhenException = false;
            iRelaunchDCATimeOut_min = 5;
            bDebugRelaunchDCA = false;
            OERFactor = new List<string>();
            bUseOOMACalcMethod = false;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class DCA_A86100C_Flex400G : InstrumentsLib.Tools.Core.InstrumentX, IScope
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ON_OFF_Mode
        {
            /// <summary>
            /// 
            /// </summary>
			OFF = 0,
            /// <summary>
            /// 
            /// </summary>
			ON = 1
        }

        //public class Channel
        //{
        //    public string strChannel { get; set; }

        //    public Dictionary<string, DCAcmd> mapMeasCmd = new Dictionary<string, DCAcmd>();

        //    public Dictionary<string, double> mapMeasuredValues = new Dictionary<string, double>();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected bool initFunctor(DCAcmd cmd)
        {
            base.initFunctor(cmd);

            cmd.WriteAndCheckCmdStatus = this.WriteAndCheckCmdStatus;
            cmd.ExtractParam = this.ExtractParam;
            cmd._myConfig = this._myConfig;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public class ChannelInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, Eyecmd> mapEyeCmds;

            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, DCAcmd> mapPAM4Cmds;

            /// <summary>
            /// 
            /// </summary>
            public ChannelInfo()
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, ChannelInfo> _mapChannelInfo = new Dictionary<string, ChannelInfo>();

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, Eyecmd> mapEyeCmds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, DCAcmd> mapPAM4Cmds { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected int DEFAULT_POLLING_DELAY = 500;      // % Delay in miliseconds for ALERT query
        /// <summary>
        /// 
        /// </summary>
        protected DCA_A86100CFlex400GConfig _myConfig;
        /// <summary>
        /// 
        /// </summary>
        protected List<int> _channels;

        /// <summary>
        /// 
        /// </summary>
		public string sRun_err { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sPlengthLockState { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sBrateLockState { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sSignalTypeState { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sDivideRatioState { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sSystemMode { get; set; }  // %'EYE' 'JITTER'
        /// <summary>
        /// 
        /// </summary>
		public string sTriggerSrc { get; set; }  // 'FPAN'
        /// <summary>
        /// 
        /// </summary>
		public string sTriggerBW { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sPatternLock { get; set; } // %'ON' 'OFF'
        /// <summary>
        /// 
        /// </summary>
		public string sAutoFlag { get; set; } // %'ON' 'OFF'
        /// <summary>
        /// 
        /// </summary>
		public string sAcqEventState { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sTimeBaseRef { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public string sHistoAxis { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sHistoMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sHistoSRC { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string sWindowBorder { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public double dAvgCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public double dWindowX1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public double dWindowX2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public double dWindowY1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public double dWindowY2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public DateTime tTimeStamp { get; set; }

        // Histo Params
        /// <summary>
        /// 
        /// </summary>
        public string sPatLock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nDUTidx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strTestName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool bSeperateAcqTEQ { get; set; } // add this parameter for test time analysis

        /// <summary>
        /// Property of ScopeConfig for Read Only
        /// </summary>
        public ScopeConfig objScopeConfig => _myConfig;

        /// <summary>
        /// Return instrument name of RF switch for trigger signal of DCA
        /// </summary>
        public string sRFSwitchNameForTrigger => _myConfig.sRFSwitchNameForTrigger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public DCA_A86100C_Flex400G(DCA_A86100CFlex400GConfig config)
            : base(config)
        {
            _myConfig = config;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public DCA_A86100C_Flex400G(DCA_A86100CFlex400GConfig config, ProtocolX protocol)
            : base(config, protocol)
        {
            _myConfig = config;
        }

        private const int SNIDX = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string query(string cmd)
        {
            //cmd += "\n";
            return _ProtocolX.query(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string queryNoRetry(string cmd)
        {
            //cmd += "\n";
            return _ProtocolX.query(cmd, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool write(string cmd)
        {
            //cmd += "\n";
            bool bResult = _ProtocolX.write(cmd);
            string res;
            if (this._config.bVerbose)
            {
                if (CheckError(out res))
                {
                    Log($"CMD {cmd} => FAILED");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool WriteAndCheckCmdStatus(string cmd)
        {
            this._ProtocolX.write(cmd);

            string sRun_err;
            bool bHasError = DetectErrorAndClear(out sRun_err);

            return !bHasError;
        }

        /// <summary>
        /// Initialize the DCA
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            base.initialize();

            if (_myConfig.bSimulation)
            {
                return true;
            }

            //Query instruments
            string strResult = this.query("*IDN?");

            //Now parse the ID of the DCA frame
            string[] ar = strResult.Split(',');
            this._myConfig.strDCA_ID = ar[SNIDX];

            this.write("*RST");

            //setTimeout(600);
            this.SoftReset();

            // no need to load DCA set up during initialize()
            //this.LoadSetupFile(_myConfig.DCAConfigFile);

            // no need to apply any setting during initialize()
            //this.setDCAsettings(DCASettings.INIT_SETTING);

            buildDCAcommands();

            ChannelInfo myChannelInfoRef;
            //Build dictionary of channel info
            for (int i = 0; i < this._myConfig.arChannels.Count; i++)
            {
                myChannelInfoRef = new ChannelInfo();
                myChannelInfoRef.mapEyeCmds = CloneUtil.DeepCopy<Dictionary<string, Eyecmd>>(this.mapEyeCmds);
                myChannelInfoRef.mapPAM4Cmds = CloneUtil.DeepCopy<Dictionary<string, DCAcmd>>(this.mapPAM4Cmds);
                this._mapChannelInfo[this._myConfig.arChannels[i]] = myChannelInfoRef;

                this.initializeFunctorCol(myChannelInfoRef.mapEyeCmds);
                this.initializeFunctorCol(myChannelInfoRef.mapPAM4Cmds);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapCmd"></param>
        /// <returns></returns>
        protected bool initializeFunctorCol(Dictionary<string, Eyecmd> mapCmd)
        {
            foreach (Eyecmd cmd in mapCmd.Values)
            {
                this.initFunctor(cmd);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapCmd"></param>
        /// <returns></returns>
        protected bool initializeFunctorCol(Dictionary<string, DCAcmd> mapCmd)
        {
            foreach (DCAcmd cmd in mapCmd.Values)
            {
                this.initFunctor(cmd);
            }

            return true;
        }

        #region DCA specific functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetPatLock(string state)
        {
            if (state.Equals("AUTO"))
            {
                SetPatternLock("ON");
            }
            else
            {
                SetPatternLock("OFF");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern_lock"></param>
		public void SetPatternLock(string pattern_lock)
        {
            if (_config.bSimulation)
            {
                return;
            }

            write(":TRIG:PLOC " + pattern_lock); // 'ON' 'OFF'
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger_src"></param>
        protected void SetTriggerSRC(string trigger_src)
        {
            write(":TRIG:SOUR " + trigger_src); // FPAN
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger_bw"></param>
		protected void SetTriggerBW(string trigger_bw)
        {
            write(":TRIG:BWLimit " + trigger_bw);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points_per_waveform"></param>
		protected void SetPointsPerWaveform(string points_per_waveform)
        {
            if (points_per_waveform.Equals("AUTO"))
            {
                write(":ACQuire:RLENGTH:MODE AUT");
            }
            else
            {
                write(":ACQuire:RLENGTH:MODE " + points_per_waveform);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFileName"></param>
        protected void LoadMaskFile(string sFileName)
        {
            //write(@":MTESt1:LOAD C:\Program Files\Agilent\FlexDCA\Demo\Masks\Ethernet\" + sFileName);
            write(":MTESt1:LOAD:FNAMe " + sFileName);
            write(":MTESt1:LOAD");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sState"></param>
        protected void SetMaskMarginState(string sState)
        {
            write(":MTESt1:MARGIN:STAT " + sState);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sAmount"></param>
        protected void SetMaskMarginValue(int sAmount)
        {
            write(":MTESt1:MARG:PERC " + sAmount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bWait"></param>
        /// <param name="bSaveScreen"></param>
        public void Setautoscale(bool bWait = true, bool bSaveScreen = false)
        {
            try
            {
                clog.MarkStart(strTestName, clog.TimeKey.AutoScale, nDUTidx);
                string res = "";
                DetectErrorAndClear(out res);

                //for (int nTry = 1; nTry < 3; nTry++)
                //{
                write("SYSTEM:AUT");

                if (bWait)
                {
                    // save screen before busy in case exception occurs inside busy
                    if (bSaveScreen)
                    {
                        string strFileName = string.Format("AutoScale_beforeBusy_{0}_dut{1}_{2}.jpg", strTestName, nDUTidx, CDateTimeUtil.getDateTimeString(DateTime.Now));
                        saveAutosclaeScreen(strFileName);
                    }

                    Busy();
                    //write("*CLS");

                    // save screen after busy 
                    if (bSaveScreen)
                    {
                        string strFileName = string.Format("AutoScale_afterBusy_{0}_dut{1}_{2}.jpg", strTestName, nDUTidx, CDateTimeUtil.getDateTimeString(DateTime.Now));
                        saveAutosclaeScreen(strFileName);
                    }
                }
            }
            finally
            {
                clog.MarkEnd(strTestName, clog.TimeKey.AutoScale, nDUTidx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void turnOffAllChannelDisplay()
        {
            if (null != this._myConfig.arChannels)
            {
                for (int n = 0; n < this._myConfig.arChannels.Count; n++)
                {
                    write(":CHAN" + this._myConfig.arChannels[n] + ":DISPlay OFF");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string EyeParamMap(string param)
        {
            string res = "";

            if (param.Equals("Rise9010") ||
                 param.Equals("Rise8020") ||
                 param.Equals("Rise6040"))
                res = ":MEAS:EYE:RIS";

            if (param.Equals("Fall9010") ||
                 param.Equals("Fall8020") ||
                 param.Equals("Fall6040"))
                res = ":MEAS:EYE:FALL";

            if (param.Equals("EyeHeight"))
                res = ":MEAS:CGR:EHE";

            if (param.Equals("EEyeHeight"))
                res = ":MEAS:EYE:EHE";

            if (param.Equals("EEyeHeightRat"))
                res = ":MEAS:EYE:EHEight:FORMat RATio";

            if (param.Equals("EEyeHeightAmp"))
                res = ":MEAS:EYE:EHEight:FORMat AMPL";

            if (param.Equals("EyeAmp"))
                res = ":MEAS:EYE:AMPL";

            if (param.Equals("ZeroLevel"))
                res = ":MEAS:EYE:ZLEV";

            if (param.Equals("OneLevel"))
                res = ":MEAS:EYE:OLEV";

            if (param.Equals("Xing"))
                res = ":MEAS:EYE:CROS";

            if (param.Equals("JitterDcd"))
                res = ":MEAS:EYE:DCD";

            if (param.Equals("Jitter"))
                res = ":MEAS:EYE:JITT";

            if (param.Equals("JitterPp"))
                res = ":MEAS:EYE:JITTer";

            if (param.Equals("JitterRms"))
                res = ":MEAS:EYE:JITT";

            if (param.Equals("ExtRatioDb"))
                res = ":MEAS:EYE:ERAT";

            if (param.Equals("ExtRatio"))
                res = ":MEAS:EYE:ERAT";

            if (param.Equals("Crossing"))
                res = ":MEAS:EYE:CROS";

            if (param.Equals("AOP") || param.Equals("AOP_WATT"))
                res = ":MEASure:EYE:APOWer";

            if (param.Equals("DCDistortion"))
                res = ":MEASure:EYE:DCDistortion";

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
		public void Run()
        {
            ClearScreen();// clear screen
            write(":STOP");
            write("*CLS"); // clear errors queue
            write(":RUN");
            sRun_err = ReadError(); // #ok<*ST2NM>            
            Setautoscale();
        }

        /// <summary>
        /// 
        /// </summary>
		public void RunMaskTest()
        {
            ClearScreen();// clear screen
            write(":STOP");
            write("*CLS"); // clear errors queue
            write(":RUN");
            StartMaskTest();
            sRun_err = ReadError(); // #ok<*ST2NM>            
                                    //autoscale();
            Busy();
        }

        /// <summary>
        /// Clear Front Panel Screen
        /// </summary>
		public virtual void ClearScreen()
        {
            try
            {
                clog.MarkStart(strTestName, "ClearDCAScreen", nDUTidx);
                write(":ACQuire:CDISplay");
            }
            finally
            {
                clog.MarkEnd(strTestName, "ClearDCAScreen", nDUTidx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iChannel"></param>
        public void TapsReCalc(int iChannel)
        {
            try
            {
                clog.MarkStart(strTestName, $"TapsReCalc-{iChannel}", nDUTidx);
                write($":SPRocess{iChannel}:FFEQualizer:TAPS:RECalculate");
                Busy();
            }
            finally
            {
                clog.MarkEnd(strTestName, $"TapsReCalc-{iChannel}", nDUTidx);
            }
        }

        /// <summary>
        /// Check Error Message of Scope
        /// </summary>
        /// <param name="res">output error message</param>
        /// <returns>True: Error; False: No Error</returns>
        protected virtual bool CheckError(out string res)
        {
            bool bHasError = false;
            res = "";

            int error_cnt = 0;
            clog.Log("query(:SYST:ERR:COUNT?)", nDUTidx);
            int.TryParse(query(":SYST:ERR:COUNT?"), out error_cnt);
            if (error_cnt > 0)
            {
                bHasError = true;
                for (int i = 0; i < error_cnt; i++)
                {
                    clog.Log("query(:SYST:ERR:NEXT?)", nDUTidx);
                    res = query(":SYST:ERR:NEXT?");
                    clog.Log($"error message #{i + 1} out of {error_cnt} = {res}", nDUTidx);
                }
            }

            return bHasError;
        }

        /// <summary>
        /// Detects if there are errors in the command buffer and clears at the end of the function if has errors
        /// </summary>
        /// <param name="res">The name of the error response</param>
        /// <returns>true if has error(s); otherwise false</returns>
        protected bool DetectErrorAndClear(out string res)
        {
            bool bHasError = false;
            res = "";
            //res = query(":SYST:ERR?");

            if (CheckError(out res))
            {
                bHasError = true;
                clog.Log("*CLS", nDUTidx);
                write("*CLS");
            }

            clog.Log("_ProtocolX.clearBuffer()", nDUTidx);
            this._ProtocolX.clearBuffer();

            return bHasError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadError()
        {
            string res = query(":SYST:ERR?");
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string TrigPatLocked()
        {
            if (_config.bSimulation)
            {
                return "";
            }

            string res = "";
            if (sRun_err == "UNABLE_CHAR_EDGE_ERROR")
            {
                res = sRun_err;
                return res;
            }

            // Poll for completion
            int nValue = 0;
            TimeSpan ts;
            DateTime tmStart = DateTime.UtcNow;
            ts = DateTime.UtcNow - tmStart;
            bool bSucceded = false;
            try
            {
                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    while (false == bSucceded)
                    {
                        if (int.TryParse(query(":TRIGger:PLOCk?"), out nValue))
                        {
                            if (1 == nValue)
                            {

                                bSucceded = true;
                                break;

                            }
                            else
                            {
                                Pause(DEFAULT_POLLING_DELAY);
                            }
                        }
                        else
                        {
                            //todo log error
                            Pause(DEFAULT_POLLING_DELAY);
                        }

                        ts = DateTime.UtcNow - tmStart;
                        Pause(500); // wait for 0.5 sec before the next try
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex, string.Format("Visa Query throw Exception with {0}", ex.GetType().ToString()));
            }


            return res;
        }

        /// <summary>
        /// Ensure command has fully complete executed
        /// </summary>
        /// <param name="timeout">Duration Time that expecting won't exceed</param>
        public void Busy(double timeout = -9999)
        {
            if (_config.bSimulation)
            {
                return;
            }

            if (timeout == -9999)
            {
                timeout = _myConfig.dMaxBusyTimeoutSecs;
            }

            // Poll for completion
            DateTime tmStart = DateTime.Now;
            while ((DateTime.Now - tmStart).TotalSeconds < timeout)
            {
                try
                {
                    write("*OPC");
                    if (int.TryParse(query("*ESR?"), out int nValue))
                    {
                        if (1 == nValue)
                        {
                            return;
                        }
                    }
                    Pause(DEFAULT_POLLING_DELAY);
                }
                catch (Exception ex)
                {
                    if ((DateTime.Now - tmStart).TotalSeconds > timeout)
                    {
                        OSScreenShot("OPCTimeout");
                        Error(ex, string.Format("Visa Query throw Exception with {0}", ex.GetType().ToString()));
                        throw;
                    }
                }
            }

            throw new HardwareErrorException("Error: Busy() => Timeout!", this.Name);
        }

        /// <summary>
        /// 
        /// </summary>
		public void StartMaskTest()
        {
            write(":MTESt1:STARt");
        }


        //public void SaveScreen(string filename, bool bInvert)
        //{
        //    //write(":DISK:SIM:SINC CHAN" + _nChannel + "A");
        //    if (bInvert)
        //    {
        //        write(":DISK:SIMage:INVert ON");
        //    }
        //    else
        //    {
        //        write(":DISK:SIMage:INVert OFF");
        //    }

        //    write(":DISK:SIM:FNAME '" + filename + "'");
        //    write(":DISK:SIM:SAVE");
        //}

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="acq_limit_patterns"></param>
		protected void SetAcqLimitPatterns(decimal acq_limit_patterns)
        {
            write(":LTESt:ACQuire:CTYPe:PATTerns " + acq_limit_patterns);
            query(":LTESt:ACQuire:CTYPe:PATTerns?");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avg_count"></param>
        protected void SetAvgCount(int avg_count)
        {
            if (avg_count > 0)
            {
                write(":ACQ:SMO AVER");
                write(":ACQ:ECO " + avg_count);
            }
            else
            {
                write(":ACQ:SMO NONE");
                write(":ACQ:ECO " + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern_length"></param>
        protected void SetPatternLength(string pattern_length)
        {
            write(":TRIGger:PLENgth " + pattern_length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
		protected string ExtractJitterParam(string param)
        {
            string datum = "", strResult = "", strRootMeasCmd = "";
            for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
            {
                if (param.Equals("J2"))
                {
                    //:MEASure:JITTer:JN:SJN J2
                    //:MEASure:JITTer:JN?
                    strRootMeasCmd = ":MEASure:JITTer:JN";
                    write(":MEASure:JITTer:JN:SJN J2");
                    datum = query(":MEASure:JITTer:JN?").Trim();
                }
                else if (param.Equals("J9"))
                {
                    //:MEASure:JITTer:JN:SJN J9
                    //:MEASure:JITTer:JN?
                    strRootMeasCmd = ":MEASure:JITTer:JN";
                    write(":MEASure:JITTer:JN:SJN J9");
                    datum = query(":MEASure:JITTer:JN?").Trim();
                }
                else
                {
                    strRootMeasCmd = ":MEASure:JITTer:" + param;
                    datum = query(":MEAS:JITT:" + param + "?").Trim();
                }

                //Add some telemetry debug queries
                strResult = query(strRootMeasCmd + ":STATus?");
                if (!strResult.ToUpper().Contains("COR"))
                {
                    strResult = query(strRootMeasCmd + ":STATus:REASon?");
                }

                if (datum.Length > 0)
                {
                    return datum;
                }
            }

            //Debug.Assert(datum.Length > 0);

            return datum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        protected bool ExtractOERFactor(string strChannel, Dictionary<string, string> mapMeasuredValues)
        {
            string datum = query($":MEASure:ERATio:CHAN{strChannel}:OERFactor?").Trim();

            if (datum.Length > 0)
            {
                mapMeasuredValues[ScopeConst.OERFACTOR] = string.Format("{0:0.0}", Convert.ToDouble(datum));
            }

            return true;
        }

        /// <summary>
        /// Fetch Measurement from Scope
        /// </summary>
        /// <param name="channelConfig">Scope Channel Setting Object</param>
        /// <param name="arParams">Measurement Parameter to Fetch</param>
        /// <param name="mapMeasuredValues">output Measurement Values from Scope</param>
        /// <param name="mapMeasuredValuesStatus">output Measurment Param Status</param>
        /// <returns>Always true: Complete Fetch Measurement result</returns>
        protected virtual bool ExtractPAM4Param(CChannelSettings channelConfig, List<string> arParams,
                                        Dictionary<string, string> mapMeasuredValues,
                                        Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            if (channelConfig.strChannelName.Contains("CRU")) // a 3rd channel is used as trigger source
                return true;// do nothing

            ChannelInfo myChannelInfo = _mapChannelInfo[channelConfig.strChannelName];

            for (int i = 0; i < arParams.Count; i++)
            {
                myChannelInfo.mapPAM4Cmds[arParams[i]].MapParam(channelConfig.strChannelName, mapMeasuredValues, mapMeasuredValuesStatus);
                if (arParams[i] == ScopeConst.OER)
                {
                    ExtractOERFactor(channelConfig.strChannelName, mapMeasuredValues);
                }
            }

            OMACalculation(arParams, myChannelInfo.mapPAM4Cmds[ScopeConst.AOP].strParam, myChannelInfo.mapPAM4Cmds[ScopeConst.OER].strParam, myChannelInfo.mapPAM4Cmds[ScopeConst.OOMA].strParam, ref mapMeasuredValues);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subSystem"></param>
        /// <param name="param"></param>
        /// <param name="paramStatus"></param>
        /// <returns></returns>
        protected string ExtractParam(string subSystem, string param, out string paramStatus)
        {
            paramStatus = "";

            try
            {
                string strTransition = "", strDatum = "", strRootMeasCmd = "";
                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    // after measurement source changed, now to query the measurment value
                    if ((param == ScopeConst.TRANsitionRISing) || (param == ScopeConst.TRANsitionFALLing))
                    {
                        strDatum = query($":MEAS:{subSystem}:TTIMe?").Trim();
                        strTransition = query($":MEAS:{subSystem}:TTIMe:TRANsition?").Trim(); // RIS, FALL, SLOW
                        if (!param.Contains(strTransition))// check if we are not measuring the correct RIS or FALL transition time
                        {
                            return "-999";
                        }
                        strRootMeasCmd = $":MEASure:{subSystem}:TTIMe"; // :MEASure: EYE: TTIMe: STATus ?

                    }
                    else
                    {
                        strDatum = query($":MEAS:{subSystem}:{param}?").Trim();
                        strRootMeasCmd = $":MEASure:{subSystem}:{param}";
                    }

                    //Add some telemetry debug queries
                    paramStatus = query(strRootMeasCmd + ":STATus?");
                    Log("ExtractParam, strRootMeasCmd={0}, datum={1}, strResult={2}", strRootMeasCmd, strDatum, paramStatus);
                    if ((strDatum.Length > 0) &&
                        (!strDatum.Equals("9.91E+37")) &&
                        (paramStatus.ToUpper().Contains("CORR"))) // if datum == 9.91E+37, NaN,  it implies the measuremnet is not yet ready
                    {
                        return strDatum;
                    }

                    if (paramStatus.ToUpper().Contains("QUES")) // questionable measurement, then autoscale (decided not to, to save test time and allow DCAM to recover itself)
                    {
                        string strReason = query(strRootMeasCmd + ":STATus:REASon?");
                        Log("Questionable reason is " + strReason);
                        //if (nTry < _myConfig.nRetry - 1) // skip the autoscale for the nTry-th try because no need for the last autoscale
                        //{
                        //    Setautoscale(true); // true means must wait till OPC?
                        //}
                    }
                    else if (paramStatus.ToUpper().Contains("INV")) // invalid measurement
                    {
                        string strReason = query(strRootMeasCmd + ":STATus:REASon?");
                        Log("Invalid reason is " + strReason);
                    }
                }
                // still questionable measurement after nRetry
                if (paramStatus.ToUpper().Contains("QUES"))
                {
                    return strDatum;
                }

                return "-999"; // for invalid measurement

            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string ExtractAmplitudeParam(string param)
        {
            string datum = "", strResult = "", strRootMeasCmd = "";
            for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
            {
                strRootMeasCmd = ":MEASure:AMPLitude:" + param;
                datum = query(":MEASure:AMPLitude:" + param + "?").Trim();

                //Add some telemetry debug queries
                strResult = query(strRootMeasCmd + ":STATus?");
                if (!strResult.ToUpper().Contains("COR"))
                {
                    strResult = query(strRootMeasCmd + ":STATus:REASon?");
                }

                if (datum.Length > 0)
                {
                    return datum;
                }
            }

            //Debug.Assert(datum.Length > 0);
            return datum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string ExtractOscParam(string strChannel, string param)
        {
            string datum = "", strResult = "", strRootMeasCmd = "";
            for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
            {
                _ProtocolX.clearBuffer();
                strRootMeasCmd = ":MEASure:OSC:" + param;
                write(strRootMeasCmd + ":SOUR CHAN" + strChannel);
                datum = query(strRootMeasCmd + "?").Trim();

                //Add some telemetry debug queries
                strResult = query(strRootMeasCmd + ":STATus?");
                if (!strResult.ToUpper().Contains("COR"))
                {
                    strResult = query(strRootMeasCmd + ":STATus:REASon?");
                }

                if (datum.Length > 0)
                {
                    return datum;
                }
            }

            //Debug.Assert(datum.Length > 0);
            return datum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected string GetMaskHitCount(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return "0";
            }

            string res = query(string.Format(":MEAS:MTESt{0}:HITS?", nMask));
            res = res.TrimEnd();

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:HITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:HITS:STATus:REASon?", nMask));
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected string GetMarginHitCount(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return "0";
            }

            string res = query(string.Format(":MEAS:MTESt{0}:MHITS?", nMask));
            res = res.TrimEnd();

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus:REASon?", nMask));
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected double GetMaskHitRatio(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return 0.0;
            }

            double fMaskHitRatio = _ProtocolX.queryDouble(string.Format(":MEASure:MTESt{0}:HRatio?\n", nMask));

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus:REASon?", nMask));
            }

            return fMaskHitRatio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected double GetMaskMarginPct(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return 0.0;
            }

            double fMaskMarginPct = _ProtocolX.queryDouble(string.Format(":MEASure:MTESt{0}:MARGin?\n", nMask));

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MARGin:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MARGin:STATus:REASon?", nMask));
            }
            return fMaskMarginPct;
        }


        /// <summary>
        /// Do DCA initial load which preset in setx file
        /// If it is running in dynamic seq, file need to get from server which is preuploaded by HW team
        /// </summary>
        /// <param name="strSetupFile">DCA setx file for inital load</param>
        /// <returns></returns>
        public virtual bool LoadSetupFile(string strSetupFile)
        {
            try
            {
                clog.MarkStart(strTestName, clog.TimeKey.LoadSetupFile, nDUTidx);

                // turn off limit test before loading setup
                // need to also ensure acquire state is OFF inside setup file
                write(":LTESt:ACQuire:STATe OFF");

                //We need to call stop acquisition in case DCA still in acquisition progress.
                //Stops data acquisition. This command does not erase display memory
                write(":ACQUIRE:STOP");

                // Erases all data from display memory and resets the data acquisition to the starting data point. Initiating this command in stop/single mode clears the data.
                // we have found out that if there are residual data in display memory, when loading setx file, DCAM will not only relock but also carry out TDECQ calculation
                write(":ACQuire:CDISplay");

                // to clear any residual error from previous DCAM operations, in hope to avoid long *OPC? check            
                //Clear all the operation for prevent stopper operation.
                write("*CLS");
                this._ProtocolX.clearBuffer();

                if (_myConfig.bSimulation)
                {
                    return true;
                }

                if (!File.Exists(strSetupFile))
                {
                    Log("Error: file not found" + strSetupFile);
                    throw new Exception("Error: file not found" + strSetupFile);
                }

                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    try
                    {
                        clog.Log($"LoadSetupFile Trial ({nTry}/{_myConfig.nRetry}), FileName: {strSetupFile}", nDUTidx);
                        write(string.Format(":DISK:SETup:RECall:HCONfig OFF")); // Load setup file without reading in DCAM module configuration
                        write(string.Format(":DISK:SETup:RECall \"{0}\"", strSetupFile));
                        if (nTry == 0 && _myConfig.bDebugRelaunchDCA) throw new Exception("Debug Relaunch DCA");
                        // in case loading setx file incurs acquisition, issue a acquire:stop command before checking OPC
                        //write(":ACQUIRE:STOP");

                        Busy(_myConfig.dMaxLoadSetupTimeoutSecs);
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        if (nTry >= _myConfig.nRetry) throw;

                        Process[] KeysightProc = Process.GetProcessesByName("Keysight.N1000");
                        if (KeysightProc.Length == 0)
                        {
                            LaunchFlexDCA();
                        }

                        ReintializeDCA();
                    }
                    catch (Exception ex)
                    {
                        string sError;
                        bool bError = DetectErrorAndClear(out sError);
                        clog.Error(ex, $"Error Found at LoadSetupFile {strSetupFile}, Has Error Message = {bError}, Error Message = {sError}", nDUTidx);
                        if (nTry == (_myConfig.nRetry - 1)) throw;
                        if (nTry == (_myConfig.nRetry - 2))
                        {
                            try
                            {
                                if (_myConfig.bRelaunchDCAWhenException) RelaunchFlexDCA();
                            }
                            catch (Exception ex2)
                            {
                                clog.Error(ex2, "Fail to RelaunchFlexDCA", nDUTidx);
                                //Don't throw, let it proceed for next trial
                            }
                        }
                    }
                }
                return true;
            }
            finally
            {
                clog.MarkEnd(strTestName, clog.TimeKey.LoadSetupFile, nDUTidx);
            }
        }

        private void RelaunchFlexDCA()
        {
            _ProtocolX.close();
            CloseFlexDCA();
            LaunchFlexDCA();
            ReintializeDCA();
        }

        private void ReintializeDCA()
        {
            for (int i = 0; i < 60; i++)
            {
                Pause(1000);
                try
                {
                    _ProtocolX.initialize();
                    _ProtocolX.query("*IDN?");
                    break;
                }
                catch (Exception)
                {
                    if (i == 59) throw;
                    Process[] KeysightProc = Process.GetProcessesByName("Keysight.N1000");
                    //if not reach, then continue to retry
                    clog.Log($"Attempt#{i}: Trying to initialize Protocol, KeysightProc count = {KeysightProc.Length}", nDUTidx);
                }
            }
        }

        private void CloseFlexDCA()
        {
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMinutes < _myConfig.iRelaunchDCATimeOut_min)
            {
                Process[] MicrosoftProc = Process.GetProcessesByName("CompatTelRunner");
                if (MicrosoftProc != null)
                {
                    if (MicrosoftProc.Length == 0)
                    {
                        clog.Log($"Successfully kill CompatTelRunner, elapsed {(DateTime.Now - startTime).TotalMinutes}mins", nDUTidx);
                        break;
                    }
                    foreach (Process Proc in MicrosoftProc)
                    {
                        try
                        {
                            Proc.Kill();
                        }
                        catch
                        {
                            //ignore error, especially no access right trigger, just need to ensure it is closed.
                        }
                    }
                }
            }

            DateTime startTime2 = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMinutes < _myConfig.iRelaunchDCATimeOut_min)
            {
                Process[] KeysightProc = Process.GetProcessesByName("Keysight.N1000");
                if (KeysightProc != null)
                {
                    if (KeysightProc.Length == 0)
                    {
                        clog.Log($"Successfully kill Keysight.N1000, elapsed {(DateTime.Now - startTime2).TotalMinutes}mins", nDUTidx);
                        break;
                    }
                    foreach (Process keyProc in KeysightProc)
                    {
                        try
                        {
                            keyProc.Kill();
                        }
                        catch
                        {
                            //ignore error, especially no access right trigger, just need to ensure it is closed.
                        }
                    }
                }
            }
        }

        private void LaunchFlexDCA()
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.WorkingDirectory = @"C:\Program Files\Keysight\FlexDCA";
            procStartInfo.Arguments = "/ca";
            procStartInfo.FileName = "DcaConfigUtil.exe";
            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.RedirectStandardError = false;
            procStartInfo.UseShellExecute = true;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            DateTime startTime = DateTime.Now;
            Process[] KeysightProc = null;

            while ((DateTime.Now - startTime).TotalMinutes < _myConfig.iRelaunchDCATimeOut_min)
            {
                Pause(1000);
                KeysightProc = Process.GetProcessesByName("Keysight.N1000");
                if (KeysightProc != null)
                {
                    clog.Log($"KeysightProc count = {KeysightProc.Length}", nDUTidx);
                    if (KeysightProc.Length > 0)
                    {
                        clog.Log($"Successfully launch Keysight.N1000, elapsed {(DateTime.Now - startTime).TotalMinutes}mins", nDUTidx);
                        break;
                    }
                }
                else clog.Log("KeysightProc is null", nDUTidx);
            }
            if (KeysightProc is null) throw new HardwareErrorException("Fail to launch Flex DCA", _config.strName);
            if (KeysightProc.Length == 0) throw new HardwareErrorException("Fail to launch Flex DCA", _config.strName);

        }
        //protected bool SetupStatusEventPolling()
        //{
        //    //.Output "*RST" ' Reset multimeter
        //    //.Output "*CLS" ' Clear status registers
        //    //' Enable Operation Complete bit to 
        //    //.Output "*ESE 1"
        //    //' Enable standard event bit in status byte
        //    //.Output "*SRE 32"
        //    //.WriteString "*OPC?"     ' Assure synchronization
        //    //strTemp = .ReadString    ' Discard returned value




        //    return true;
        //}

        //protected bool PollSRQ()
        //{


        //    return true;
        //}

        /// <summary>
        /// 
        /// </summary>
        public void SoftReset()
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    this._ProtocolX.clearBuffer();
                    write("*CLS");
                    break;
                }
                catch (ObjectDisposedException)
                {
                    if (i == 1) throw;

                    Process[] KeysightProc = Process.GetProcessesByName("Keysight.N1000");
                    if (KeysightProc.Length == 0)
                    {
                        LaunchFlexDCA();
                    }

                    ReintializeDCA();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder_name"></param>
		protected void CreateFolder(string folder_name)
        {
            write(":DISK:MDIRectory '" + folder_name + "'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public string getID()
        {
            return this._myConfig.strDCA_ID;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="filter_state"></param>
		protected void SetFilterState(string strChannelName, string filter_state)
        {
            write(":CHAN" + strChannelName + ":FILT " + filter_state);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetFilterState(string strChannelName)
        {
            return query(":CHAN" + strChannelName + ":FILT?").Trim();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="filter_number"></param>
        protected void SetFilterType(string strChannelName, string filter_number)
        {
            write(":CHAN" + strChannelName + ":FSELect FILT" + filter_number);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetFilterType(string strChannelName)
        {
            try
            {
                string fil = query(":CHAN" + strChannelName + ":FSELect?");
                string[] filter = fil.Split('T');
                int nFilterIndex = Convert.ToInt32(filter[1]);

                return nFilterIndex.ToString();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }

            return "1";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="strWavelength"></param>
        protected void SetWavelength(string strChannelName, string strWavelength)// WAV1=850 WAV2=1310 WAV3=1550 USER->USER
        {
            try
            {
                write(":CHANnel" + strChannelName + ":WAV:VAL " + strWavelength);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="strWavelength"></param>
        protected void SetWavelengthValue(string strChannelName, string strWavelength)// Set the actual wavelength values...
        {
            try
            {
                write(":CHANnel" + strChannelName + ":WAV:VALue " + strWavelength);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetWavelength(string strChannelName)
        {
            if (_config.bSimulation)
            {
                return "1.31E-6";
            }

            string res = "";
            try
            {
                res = query(":CHANnel" + strChannelName + ":WAVelength:VAL?").ToUpper();

                if (res.Contains("WAV"))
                {
                    res = res.Substring(3);
                }
                else if (res.Contains("USER"))
                {
                    res = "1310";
                }
            }
            catch (Exception ex)
            {
                Error(ex, "DCA_A86100C_Flex.GetWavelength");
            }


            res = res.TrimEnd();
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetWavelengthValue(string strChannelName)
        {
            if (_config.bSimulation)
            {
                return "1.31E-6";
            }

            string res = "";
            try
            {
                res = query(":CHANnel" + strChannelName + ":WAVelength:VALue:VSET?").ToUpper();
                res = res.Trim('"');
                string[] arWavelength = res.Split(',');
                res = arWavelength[0];
            }
            catch (Exception ex)
            {
                Error(ex, "DCA_A86100C_Flex.GetWavelength");
            }


            res = res.TrimEnd();
            return res;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected bool SetupEyeParamMap(string strChannel, string param)
        {
            if (param.Equals("Rise9010") ||
                param.Equals("Rise8020") ||
                param.Equals("Rise6040"))
            {
                write(":MEAS:EYE:RIS:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:RIS");
            }

            //:MEASure:TBASe:METHod STANdard
            //:MEASure:THReshold:METHod UDEFined
            //:MEASure:THReshold:DISTal 6.00E+1
            //:MEASure:THReshold:PROXimal 4.00E+1
            //:MEASure:EBOundary:LEFT 4.90E+1
            //:MEASure:EBOundary:RIGHt 5.10E+1
            if (param.Equals("Rise6040"))
            {
                write(":MEASure:TBASe:METHod STANdard");
                write(":MEASure:THReshold:METHod UDEFined");
                write(":MEASure:THReshold:DISTal 6.00E+1");
                write(":MEASure:THReshold:PROXimal 4.00E+1");
                write(":MEASure:EBOundary:LEFT 4.90E+1");
                write(":MEASure:EBOundary:RIGHt 5.10E+1");
                return true;
            }

            if (param.Equals("Fall9010") ||
                param.Equals("Fall8020") ||
                param.Equals("Fall6040"))
            {
                write(":MEAS:EYE:FALL:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:FALL");
            }

            if (param.Equals("EyeHeight"))
            {
                write(":MEAS:EYE:EHE:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:EHE");
            }

            if (param.Equals("EyeAmp"))
            {
                write(":MEAS:EYE:AMPL:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:AMPL");
            }

            if (param.Equals("ZeroLevel"))
            {
                write(":MEAS:EYE:ZLEV:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:ZLEV");
            }

            if (param.Equals("OneLevel"))
            {
                write(":MEAS:EYE:OLEV:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:OLEV");
            }

            if (param.Equals("Xing"))
            {
                write(":MEAS:EYE:CROS:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:CROS");
            }

            if (param.Equals("JitterDcd"))
            {
                write(":MEAS:EYE:DCD:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:DCD");
            }

            if (param.Equals("JitterPp"))
            {
                write(":MEAS:EYE:JITTer:SOUR CHAN" + strChannel);
                write(":MEASure:EYE:JITTer:FORMat PP");
                return write(":MEAS:EYE:JITT");
            }

            if (param.Equals("JitterRms"))
            {
                write(":MEAS:EYE:JITTer:SOUR CHAN" + strChannel);
                write(":MEASure:EYE:JITTer:FORMat RMS");
                return write(":MEAS:EYE:JITT");
            }

            if (param.Equals("ExtRatioDb"))
            {
                write(":MEAS:EYE:ERAT:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:ERAT");
            }

            if (param.Equals("ExtRatio"))
            {
                write(":MEAS:EYE:ERAT:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:ERAT RAT");
            }

            if (param.Equals("Crossing"))
            {
                write(":MEAS:EYE:CROS:SOUR CHAN" + strChannel);
                return write(":MEAS:EYE:CROS");
            }

            if (param.Equals("AOP"))
            {
                write(":MEAS:EYE:APOWer:SOUR CHAN" + strChannel);
                write(":MEASure:EYE:APOWer");
                return write(":MEASure:EYE:APOWer:UNITs dBm");
            }

            if (param.Equals("AOP_WATT"))
            {
                write(":MEAS:EYE:APOWer:SOUR CHAN" + strChannel);
                write(":MEASure:EYE:APOWer");
                return write(":MEASure:EYE:APOWer:UNITs WATT");
            }

            if (param.Equals("DCDistortion"))
            {
                write(":MEAS:EYE:DCDistortion:SOUR CHAN" + strChannel);
                write(":MEASure:EYE:DCDistortion");
                return write(":MEASure:EYE:DCDistortion:FORMat PERCent");
            }

            if (param.Equals("TDEC"))
            {
                write(":MEAS:EYE:TDEC:SOUR CHAN" + strChannel);
                return write(":MEASure:EYE:TDEC");
            }


            return false;
        }


        /// <summary>
        /// 
        /// </summary>
		public const string Min = "Min";
        /// <summary>
        /// 
        /// </summary>
		public const string Max = "Max";
        /// <summary>
        /// 
        /// </summary>
		public const string Mean = "Mean";
        /// <summary>
        /// 
        /// </summary>
		public const string Std = "Std";

        /// <summary>
        /// 
        /// </summary>
		protected static Dictionary<string, string> s_mapSCPI_StatsMeasType;
        /// <summary>
        /// 
        /// </summary>
		public static List<string> s_arMeasurementStatisticalType;
        /// <summary>
        /// 
        /// </summary>
		public static void build_arMeasurementStatisticalType()
        {
            if (s_arMeasurementStatisticalType != null)
            {
                s_arMeasurementStatisticalType = new List<string>
                {
                    Min,
                    Max,
                    Mean,
                    Std
                };

                s_mapSCPI_StatsMeasType = new Dictionary<string, string>
                {
                    { Min, ":MIN?" },
                    { Max, ":MAX?" },
                    { Mean, ":MEAN?" },
                    { Std, ":SDEV?" }
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool buildDCAcommands()
        {
            this.mapPAM4Cmds = new Dictionary<string, DCAcmd>
            {
                [ScopeConst.AOP] = new AOPcmd(),
                [ScopeConst.TDEQ] = new TDEQcmd(),
                [ScopeConst.OOMA] = new OOMAcmd(),
                [ScopeConst.OMAX] = new OMAXcmd(),
                [ScopeConst.TTIME] = new TTIMEcmd(),
                [ScopeConst.CEQ] = new CEQcmd(),
                [ScopeConst.OER] = new OERcmd(),
                [ScopeConst.NMARGIN] = new NMARGINcmd(),
                [ScopeConst.PSER] = new PSERcmd(),
                [ScopeConst.PTDEQ] = new PTDEQcmd(),
                [ScopeConst.PNMARGIN] = new PNMARGINcmd(),
                [ScopeConst.TAPS] = new Tapscmd(),
                [ScopeConst.TAPS_FIXED_PRECURSER] = new Tapscmd(1),
                [ScopeConst.PAM4LIN] = new PAM4LINcmd(),
                [ScopeConst.PAM4LINSOURCE] = new PAM4LINSOURCEcmd(),
                [ScopeConst.EyeHeightPAM4] = new EyeHeightPAM4cmd(),
                [ScopeConst.PAM4LEVEL] = new PAM4Levelcmd(),
                [ScopeConst.PAM4LEVELSOURCE] = new PAM4LevelSOURCEcmd(),
                [ScopeConst.TRANsitionRISing] = new TRANsitionRISingcmd(),
                [ScopeConst.TRANsitionFALLing] = new TRANsitionFALLingcmd(),
                [ScopeConst.TRANsitionRISingMin] = new TRANsitionRISingcmd(),
                [ScopeConst.TRANsitionFALLingMin] = new TRANsitionFALLingcmd()
            };

            this.mapEyeCmds = new Dictionary<string, Eyecmd>
            {
                [ScopeConst.AOP_WATT] = new AOP_WATTcmd(),
                [ScopeConst.EyeAmp] = new EyeAmpcmd(),
                [ScopeConst.ZeroLevel] = new ZeroLevelcmd(),
                [ScopeConst.OneLevel] = new OneLevelcmd(),
                [ScopeConst.Xing] = new Xingcmd(),
                [ScopeConst.JitterDcd] = new JitterDcdcmd(),
                [ScopeConst.JitterPp] = new JitterPpcmd(),
                [ScopeConst.ExtRatioDb] = new ExtRatioDbcmd(),
                [ScopeConst.ExtRatio] = new ExtRatiocmd(),
                [ScopeConst.Crossing] = new Crossingcmd(),
                [ScopeConst.DCDistortion] = new DCDistortioncmd(),
                [ScopeConst.TDEC] = new TDECcmd()
            };

            // Now override the DCAM commands if applicable...
            if (null != this._myConfig.mapOverridenMeasCmds)
            {
                foreach (string cmdName in this._myConfig.mapOverridenMeasCmds.Keys)
                {
                    if (this._myConfig.mapOverridenMeasCmds[cmdName] is Eyecmd)
                    {
                        this.mapEyeCmds[cmdName] = (Eyecmd)this._myConfig.mapOverridenMeasCmds[cmdName];
                    }

                    if (this._myConfig.mapOverridenMeasCmds[cmdName] is DCAcmd)
                    {
                        this.mapPAM4Cmds[cmdName] = (DCAcmd)this._myConfig.mapOverridenMeasCmds[cmdName];
                    }
                }
            }

            // Set TDECQ Offset for TDECQ command
            if (this.mapPAM4Cmds.ContainsKey(ScopeConst.TDEQ))
            {
                TDEQcmd cmd = (TDEQcmd)this.mapPAM4Cmds[ScopeConst.TDEQ];
                cmd.TDECQOffset = _myConfig.TDECQOffset;
                cmd.OffsetTDECQMinLimit = _myConfig.OffsetTDECQMinLimit;

                if (_myConfig.mapTDECQPiecewiseLinearFit != null)
                {
                    if (_myConfig.mapTDECQPiecewiseLinearFit.Count >= 2)
                    {
                        cmd.mapTDECQPiecewiseLinearFit = _myConfig.mapTDECQPiecewiseLinearFit;
                    }
                    else
                    {
                        MessageBox.Show($"Error in DCA_A86100C_Flex400G {_myConfig.strName}. mapTDECQPiecewiseLinearFit has {_myConfig.mapTDECQPiecewiseLinearFit.Count}, map needs to have >= 2 entries ");
                    }
                }
            }

            // Set OER Offset for OER command
            if (this.mapPAM4Cmds.ContainsKey(ScopeConst.OER))
            {
                OERcmd cmd = (OERcmd)this.mapPAM4Cmds[ScopeConst.OER];
                cmd.OERFactor = _myConfig.OERFactor;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDCAFileName"></param>
        public void saveAutosclaeScreen(string strDCAFileName)
        {
            Log("saveAutosclaeScreen()");
            if (_config.bSimulation)
            {
                return;
            }

            int nSaveImageTrial = 0;
            bool bImageEx = false;

            StationHardware _stationInstance;
            _stationInstance = StationHardware.Instance();

             string noImageMes = "No image saved!";
            do
            {
                if (nSaveImageTrial > 5)
                {
                    Log(noImageMes);
                    throw new Exception(noImageMes);
                }

                write(":DISK:SIMage:INVert ON");
                //write(":DISK:SIM:FNAME '" + strLocalFileRawName + "'");
                write(":DISK:SIM:SAVE");
                Busy(); // wait till image is saved to avoid IO exception during JPEG shrinking or File.Copy(), or File.Exists

                //bImageEx = File.Exists(strLocalFileRawName);

                nSaveImageTrial++;
                if (!bImageEx)
                {
                    Pause(Convert.ToInt16(_myConfig.dImageSaveWaitTime) * nSaveImageTrial);
                    Log("Time delay {0}ms", _myConfig.dImageSaveWaitTime * nSaveImageTrial);
                }
            } while (bImageEx == false);
            Log("Image is saved in #" + nSaveImageTrial + " times.");
        }

        /// <summary>
        /// Scope front panel screen capture
        /// </summary>
        /// <param name="strDCAFileName">Image file Name to save screen capture</param>
        public virtual void saveScreen(string strDCAFileName)
        {
            Log("saveScreen()");
            if (_config.bSimulation)
            {
                return;
            }

            int nSaveImageTrial = 0;
            bool bImageEx = false;

            StationHardware _stationInstance;
            //ModuleLevelTestAppConfig _myAppConfig;
            _stationInstance = StationHardware.Instance();
            //_myAppConfig = (ModuleLevelTestAppConfig)_stationInstance.myConfig.myAppConfig;

            int nRemotePathLength = -1;
            //string strRawDCAFolderPath, strRawDCAFileFullName, strRawDCAFilePath, strlocalFilePath, strLocalFileRawDir, strLocalFileRawName = "";

            //if (_myAppConfig != null)
            //{
            //    //Create a new Raw folder (ON DCA)
            //    strRawDCAFolderPath = CAppendDir.appendDir(Path.GetDirectoryName(strDCAFileName), "Raw");
            //    strRawDCAFileFullName = Path.Combine(strRawDCAFolderPath, Path.GetFileName(strDCAFileName));
            //    strRawDCAFilePath = Path.GetDirectoryName(strRawDCAFileFullName);//Just a sanity check

            //    //Now build Local Directories and file anmes
            //    nRemotePathLength = _myAppConfig.DCA_RemoteImagePath.Length;
            //    strlocalFilePath = _myAppConfig.LocalHSOutputDir + strDCAFileName.Substring(nRemotePathLength);
            //    strLocalFileRawDir = CAppendDir.appendDir(Path.GetDirectoryName(strlocalFilePath), "Raw");
            //    strLocalFileRawName = Path.Combine(strLocalFileRawDir, Path.GetFileName(strlocalFilePath));

            //    if (!System.IO.Directory.Exists(strLocalFileRawDir))
            //    {
            //        System.IO.Directory.CreateDirectory(strLocalFileRawDir);
            //    }
            //}
            //else
            //{
            //    Exception ex = new Exception("DCA_A86100C_FlexV2.SaveScreen: _myAppConfig is null");
            //    Error(ex, "_myAppConfig is null");
            //    throw new Exception("DCA_A86100C_FlexV2.SaveScreen: _myAppConfig is null");
            //}


            string noImageMes = "No image saved!";
            if (nRemotePathLength > 0)
            {
                do
                {
                    if (nSaveImageTrial > 5)
                    {
                        Log(noImageMes);
                        throw new Exception(noImageMes);
                    }

                    write(":DISK:SIMage:INVert ON");
                    if (_myConfig.bDCAMWithDedicatedPC)
                    {
                        write(@":DISK:SIM:FNAME '%USER_DATA_DIR%\Screen Images\temp.jpg'");
                    }
                    else
                    {
                        //write(":DISK:SIM:FNAME '" + strRawDCAFileFullName + "'");
                    }
                    write(":DISK:SIM:SAVE");
                    Busy(); // wait till image is saved to avoid IO exception during JPEG shrinking or File.Copy(), or File.Exists
                    //if (_myConfig.bDCAMWithDedicatedPC) ReadFile(strRawDCAFileFullName);
                    //bImageEx = File.Exists(strLocalFileRawName);

                    nSaveImageTrial++;
                    if (!bImageEx)
                    {
                        Pause(Convert.ToInt16(_myConfig.dImageSaveWaitTime) * nSaveImageTrial);
                        Log("Time delay {0}ms", _myConfig.dImageSaveWaitTime * nSaveImageTrial);
                    }
                } while (bImageEx == false);
                Log("Image is saved in #" + nSaveImageTrial + " times.");
            }
            else
            {
                bImageEx = true;
                Log("No save image directory assigned and no verification");
            }


            //if (this._myConfig.bShrinkEyeFile)
            //{
            //    Utility.CImageConverter.ChangeJPGImageQuality(strLocalFileRawName, strlocalFilePath, _myConfig.CompressLevel);
            //}
            //else
            //{
            //    // Will overwrite if the destination file already exists.
            //    File.Copy(strLocalFileRawName, strlocalFilePath, true);
            //}

        }

        /// <summary>
        /// Function to save waveform data
        /// </summary>
        /// <param name="filename">Filename to save as</param>
        /// <param name="mapOptions">Dictionary of Options to save</param>
        /// <returns>true: success; false: fail</returns>
        public virtual bool SaveWaveformData(string filename, Dictionary<string, string> mapOptions)
        {
            this.write(":DISK:WAVeform:SAVE:SOURce " + mapOptions["Channel"]);
            this.write(":DISK:WAVeform:SAVE:FTYPe " + mapOptions["FTYPe"]);
            //this.write(":DISK:WAVeform:LINTerpolate ON
            //this.write(":DISK:WAVeform:LSDigits ON
            this.write(":DISK:WAVeform:FNAMe \"" + filename + "\"");
            this.write(":DISK:WAVeform:SAVE");

            return true;
        }

        /// <summary>
        /// Save Scope Screen capture of required Channel
        /// </summary>
        /// <param name="filename">File Name to save</param>
        /// <param name="strImageChannelName">Scope Channel to capture</param>
        /// <param name="bSelectMask">Whether want to show Mask file</param>
        public virtual void saveScreenMultiChannel(string filename, string strImageChannelName, bool bSelectMask = false)
        {
            Log("saveScreenMultiChanne()");
            
            write(":DISPLAY:WINDOW:TIME1:DMODE ZTILE");
            write(":DISPLAY:WINDOW:TIME1:ZSIGNAL " + strImageChannelName);
            
            // Select the mask if applicable
            if (bSelectMask)
            {
                int nMask = _mapChannelToMask[strImageChannelName];
                write(string.Format(":DISPlay:TMASk MASK{0}", nMask));
            }

            saveScreen(filename); // removed the delay inside (duplicate)
        }

        private bool ReadFile(string DCAImagePath)
        {
            string sFilePath = @"'%USER_DATA_DIR%\Screen Images\temp.jpg'";

            if (query($":DISK:FILE:EXISts? {sFilePath}").Trim() == "1")
            {
                write($":DISK:FILE:Read? {sFilePath}");
                Pause(1000);
                List<byte> binaryFile = _ProtocolX.readTillEnd();

                using (BinaryWriter writer = new BinaryWriter(File.Open(DCAImagePath, FileMode.Create)))
                {
                    writer.Write(binaryFile.ToArray());
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool captureEyeMeasureMaskMargin(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            if (_config.bSimulation)
            {
                return true;
            }
            //:CHAN3A:FILTer ON
            //:CHAN3A:WAVelength WAVelength2
            //:MEASure:THReshold:METHod P205080
            //:MEASure:EYE:LIST:REMove 1
            //:MEASure:EYE:ERATio
            //:MEASure:EYE:APOWer
            //:MEASure:EYE:RISetime
            //:MEASure:EYE:FALLtime
            //:MTESt1:LOAD
            //:MTESt1:MARGin:STATe ON
            //:MTESt1:MARGin:AUTo:METHod HRATio
            //:ACQuire:RUN
            //*OPC?
            //:SYSTem:AUToscale
            //*OPC?
            //:LTESt:ACQuire:STATe ON
            //*OPC?
            //:LTESt:ACQuire:CTYPe:WAVeforms 840
            //:ACQuire:CDISplay

            //Load the setup file if neccessary
            if (null != _DCAsettings)
            {
                if (null != _DCAsettings.sDCAConfigFile)
                {
                    if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
                    {
                        LoadSetupFile(_DCAsettings.sDCAConfigFile);
                    }
                }
            }

            write("*CLS");

            string strKey = "";
            mapMeasuredValues.Clear();

            write(":LTESt:ACQuire:STATe OFF");

            write(":MEAS:THR:METHOD P205080");

            write(":SYSTem:MODE EYE");
            SetTriggerSRC("FPAN");
            SetTriggerBW("DIV");

            SetPointsPerWaveform(_DCAsettings.EyePointsPerWaveform);
            SetMaskMarginValue(_DCAsettings.MaskMarginValue);

            SetPatLock("MAN");
            //Setautoscale();
            //query("*OPC?");
            //write("*CLS");

            write(":MEASure:EYE:LIST:CLEAR");

            //int i;
            //string strParam;
            strKey = SetupEyeChannelConfig(channelConfig, arParams, mapMeasuredValues);

            if (this._DCAsettings.bEnableMask)
            {
                //Define and Load Mask File
                write(string.Format(":MTESt1:LOAD:FNAMe \"{0}\"", _DCAsettings.sDCA_MaskFileNameWithPath));
                write(":MTESt1:LOAD");

                //setup mask margin
                write(":MTESt1:MARGin:STATe ON");
                write(":MTESt1:MARGin:METHod AUTO");
                write(":MTESt1:MARGin:AUTO:METHod HRATio");
                write(":MTESt1:MARGin:AUTO:HRATio 5e-5");
            }
            else
            {
                write(":MTESt1:MARGin:STATe OFF");
            }

            write(":ACQuire:REYE ON");

            write(":ACQuire:REYE:ALIGn ON");

            write(":TIMebase:BRATe 2.5781250E+10");

            Setautoscale();
            Busy(); //query("*OPC?");
            write("*CLS");

            //set acquisition limit
            write(":ACQuire:CDISplay");
            // the waveform limit is set and defined in DCA setup file and we do not want it to be overwritten by _DCAsettings.EyeAcqWavLimVal
            //write(string.Format(":LTESt:ACQuire:CTYPe  WAVeforms")); // to switch to use waveform for limit test
            //write(string.Format(":LTESt:ACQuire:CTYPe:WAVeforms {0}", _DCAsettings.EyeAcqWavLimVal));

            // enable limit test so that we can use OPC to check status upon limit is reached
            write(":LTESt:ACQuire:STATe ON");

            //Select Eye/Mask Mode and Autoscale
            write(":ACQ:RUN");

            // wait for limit to reach
            Busy();

            ExtractEyeMeasurements(channelConfig, arParams, mapMeasuredValues);

            // turn off limit test only after measurement
            write(":LTESt:ACQuire:STATe OFF");

            return true;
        }

        private string SetupEyeChannelConfig(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            string sResponse;
            //Display Channel
            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            //Apply ATTEN (i.e. station calibration)
            if (channelConfig.strAttenState.Contains("ON"))
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE ON");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("1"))
                {
                    Log("Error: :ATT:STATE ON failed");
                    return null;
                }
                write(":CHAN" + channelConfig.strChannelName + ":ATT:DEC " + channelConfig.strAttenVal);
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:DEC?");
                if (Convert.ToDouble(sResponse) != Convert.ToDouble(channelConfig.strAttenVal))
                {
                    Log("Error: ::ATT:DEC failed");
                    return null;
                }
            }
            else
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE OFF");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("0"))
                {
                    Log("Error: :ATT:STATE OFF failed");
                    return null;
                }
            }

            string strKey = "Wavelength";
            //Setup channels wavelength

            SetWavelength(channelConfig.strChannelName, channelConfig.strWavelength);
            string strWavelength = GetWavelength(channelConfig.strChannelName);
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strWavelength);


            //Setup filters
            //Turn ON filter if applicable
            SetFilterState(channelConfig.strChannelName, channelConfig.strFilterState);
            SetFilterType(channelConfig.strChannelName, channelConfig.strFilter);

            string strFilterState = GetFilterState(channelConfig.strChannelName).Trim();
            strKey = "FilterState";
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strFilterState);

            string strFilterType = GetFilterType(channelConfig.strChannelName).Trim();
            strKey = "FilterType";
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strFilterType);

            string strParam = "";
            for (int i = 0; i < arParams.Count; i++)
            {
                strParam = arParams[i];
                SetupEyeParamMap(channelConfig.strChannelName, strParam);
            }
            return strKey;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool SetupPAM4ChannelConfig(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            if (channelConfig.strChannelName.Contains("CRU")) // a 3rd channel is used as trigger source
                return true;// do nothing

            string sResponse;
            //Display Channel
            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            //Apply ATTEN (i.e. station calibration)
            if (channelConfig.strAttenState.Contains("ON"))
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE ON");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("1"))
                {
                    Log("Error: :ATT:STATE ON failed");
                    return false;
                }
                write(":CHAN" + channelConfig.strChannelName + ":ATT:DEC " + channelConfig.strAttenVal);
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:DEC?");
                if (Math.Abs(Convert.ToDouble(sResponse) - Convert.ToDouble(channelConfig.strAttenVal)) > 0.02)
                {
                    Log("Error: ::ATT:DEC failed");
                    return false;
                }
            }
            else
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE OFF");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("0"))
                {
                    Log("Error: :ATT:STATE OFF failed");
                    return false;
                }
            }

            ChannelInfo myChannelInfo = _mapChannelInfo[channelConfig.strChannelName];

            // if bSetupParam is true, then clear setting in .setx file and SetupParam by SW
            // otherwise use settings in *.setx file (default)
            if (_DCAsettings.bSetupParam)
            {
                for (int i = 0; i < arParams.Count; i++)
                {
                    myChannelInfo.mapPAM4Cmds[arParams[i]].SetupParam(channelConfig.strChannelName);
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskHits = "MaskHits";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskHitRatio = "MaskHitRatio";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MarginHits = "MarginHits";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskMarginPct = "MaskMarginPct";

        private Dictionary<string, int> _mapChannelToMask = new Dictionary<string, int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="arMapMaskValues"></param>
        /// <returns></returns>
        public bool captureEyeMeasureMaskMarginParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues, out List<Dictionary<string, string>> arMapMaskValues)
        {
            // lock CRU
            if (channelsGroupConfig.bLockCRU)
            {
                if (null != _DCAsettings.CRUsetup)
                {
                    _DCAsettings.CRUsetup.Setup();
                }
                //_DCAsettings.PAM4setup.setup();
            }

            arMapMeasuredValues = new List<Dictionary<string, string>>();
            arMapMaskValues = new List<Dictionary<string, string>>();

            CChannelSettings channelConfig;
            List<string> arParams;
            Dictionary<string, string> mapMeasuredValues;
            Dictionary<string, string> mapMaskValues;

            //:CHAN3A:FILTer ON
            //:CHAN3A:WAVelength WAVelength2
            //:MEASure:THReshold:METHod P205080
            //:MEASure:EYE:LIST:REMove 1
            //:MEASure:EYE:ERATio
            //:MEASure:EYE:APOWer
            //:MEASure:EYE:RISetime
            //:MEASure:EYE:FALLtime
            //:MTESt1:LOAD
            //:MTESt1:MARGin:STATe ON
            //:MTESt1:MARGin:AUTo:METHod HRATio
            //:ACQuire:RUN
            //*OPC?
            //:SYSTem:AUToscale
            //*OPC?
            //:LTESt:ACQuire:STATe ON
            //*OPC?
            //:LTESt:ACQuire:CTYPe:WAVeforms 840
            //:ACQuire:CDISplay

            // DCA setup file was loaded in HS_Test runHSBaseTest()
            ////Load the setup file if neccessary
            //if (null != _DCAsettings)
            //{
            //    if (null != _DCAsettings.sDCAConfigFile)
            //    {
            //        if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
            //        {
            //            LoadSetupFile(_DCAsettings.sDCAConfigFile);
            //        }
            //    }
            //}

            write("*CLS");

            write(":LTESt:ACQuire:STATe OFF");

            write(":MEAS:THR:METHOD P205080");

            write(":SYSTem:MODE EYE");
            write(":SLOT2:TRIGger:TRACking ON");
            SetTriggerSRC("FPAN");
            SetTriggerBW("DIV");

            turnOffAllChannelDisplay();

            SetPointsPerWaveform(_DCAsettings.EyePointsPerWaveform);
            SetMaskMarginValue(_DCAsettings.MaskMarginValue);

            SetPatLock("MAN");
            //Setautoscale();
            //query("*OPC?");
            //write("*CLS");

            int c;
            int nMask = 1;

            write(":MEASure:EYE:LIST:CLEAR");

            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = new Dictionary<string, string>();
                arMapMeasuredValues.Add(mapMeasuredValues);

                mapMaskValues = new Dictionary<string, string>();
                arMapMaskValues.Add(mapMaskValues);

                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;
                SetupEyeChannelConfig(channelConfig, arParams, mapMeasuredValues);
                nMask = c + 1;//channelsGroupConfig.arChannels[c].nChannel + 1;
                if (channelConfig.bEnableMask)
                {
                    _mapChannelToMask[$"CHAN{channelConfig.strChannelName}"] = nMask;
                    write(string.Format(":MTEST{0}:SOUR CHAN{1}", nMask, channelConfig.strChannelName));
                    write(string.Format(":MTESt{0}:ALIGnment:X AUTomatic", nMask));
                    query(string.Format(":MTESt{0}:ALIGnment:X?", nMask));

                    //Define and Load Mask File
                    write(string.Format(":MTESt{0}:LOAD:FNAMe \"{1}\"", nMask, channelConfig.MaskFileNameWithPath));
                    write(string.Format(":MTESt{0}:LOAD", nMask));

                    //setup mask margin
                    write(string.Format(":MTESt{0}:MARGin:STATe ON", nMask));
                    write(string.Format(":MTESt{0}:MARGin:METHod AUTO", nMask));
                    write(string.Format(":MTESt{0}:MARGin:AUTO:METHod HRATio", nMask));
                    write(string.Format(":MTESt{0}:MARGin:AUTO:HRATio 5e-5", nMask));
                }
                else
                {
                    write(string.Format(":MTESt{0}:MARGin:STATe OFF", nMask));
                }
            }

            //The 4 tiles split windows view
            write(":DISPLAY:WINDOW:TIME1:DMODE TILED");

            Pause(1000);

            write(":ACQuire:REYE ON");

            write(":ACQuire:REYE:ALIGn ON");

            write(":TIMebase:BRATe 2.5781250E+10");


            Setautoscale();

            //set acquisition limit
            write(":ACQuire:CDISplay"); // clear display
            // the following are now defined in DCA setup file and we do not want it to be overwritten by _DCAsettings.EyeAcqWavLimVal
            //// the DCA setting file defines limit using samples, here changed to waveforms
            //write(string.Format(":LTESt:ACQuire:CTYPe  WAVeforms")); // to switch to use waveform for limit test
            //write(string.Format(":LTESt:ACQuire:CTYPe:WAVeforms {0}", _DCAsettings.EyeAcqWavLimVal)); // to set up the number of waveforms in eye test
            // enable limit test so that we can use OPC to check status upon limit is reached
            write(":LTESt:ACQuire:STATe ON");

            //Select Eye/Mask Mode and Autoscale
            write(":ACQ:RUN");

            // wait for limit to reach
            Busy();

            //Retrieves the params
            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = arMapMeasuredValues[c];
                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;

                ExtractEyeMeasurements(channelConfig, arParams, mapMeasuredValues);

                nMask = c + 1;//channelsGroupConfig.arChannels[c].nChannel + 1;

                mapMaskValues = arMapMaskValues[c];
                //Get the Mask Values...
                try
                {
                    mapMaskValues[params_MaskHits] = "9999";
                    mapMaskValues[params_MarginHits] = "9999";
                    mapMaskValues[params_MaskHitRatio] = "9999";
                    mapMaskValues[params_MaskMarginPct] = "9999";

                    //Specify Channel Source
                    //write(string.Format("MTEST{0}:SOUR CHAN{1}", nMask, channelConfig.strChannelName));
                    write(string.Format(":DISPlay:TMASk MASK{0}", nMask));

                    if (channelConfig.bEnableMask)
                    {
                        mapMaskValues[params_MaskHits] = GetMaskHitCount(nMask);
                        mapMaskValues[params_MarginHits] = GetMarginHitCount(nMask);
                        mapMaskValues[params_MaskMarginPct] = GetMaskMarginPct(nMask).ToString();
                        mapMaskValues[params_MaskHitRatio] = GetMaskHitRatio(nMask).ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }

            // do not turn off so that limit will show in screen capture
            // turn off limit test only after measurement
            //write(":LTESt:ACQuire:STATe OFF");

            return true;
        }

        /// <inheritdoc/>
        public virtual bool measurePAM4Parallel(CChannelsGroupParallel channelsGroupConfig,
                                        out List<Dictionary<string, string>> arMapMeasuredValues,
                                        out List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus,
                                        bool bMeasureOnly = false, bool bTurnOffAllDispFirst = false)
        {
            clog.MarkStart(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            string res = "";
            arMapMeasuredValues = new List<Dictionary<string, string>>();
            arMapMeasuredValuesStatus = new List<Dictionary<string, DCAcmd.eMeasureDataStatus>>();

            CChannelSettings channelConfig;
            Dictionary<string, string> mapMeasuredValues;
            Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus;

            // Determine if CRU is locked...
            if (null != _DCAsettings.CRUsetup)
            {
                if (!_DCAsettings.CRUsetup.IsLocked())
                {
                    // Force the measurement to do relocking if there is no CRU Lock and CRU is enabled
                    bMeasureOnly = false;
                    channelsGroupConfig.bLockCRU = true;
                }
            }


            if (bMeasureOnly == false) // if bMeasureOnly==false, then do the following DCA setup steps
            {
                clog.MarkStart(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);
                // note that the DCA file was loaded in HS_Test runHSBaseTest()
                ////Load the setup file if neccessary
                //if (null != _DCAsettings)
                //{
                //    if (null != _DCAsettings.sDCAConfigFile)
                //    {
                //        if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
                //        {
                //            LoadSetupFile(_DCAsettings.sDCAConfigFile);
                //        }
                //    }
                //}

                if (channelsGroupConfig.bLockCRU)
                {
                    if (null != _DCAsettings.CRUsetup)
                    {
                        clog.MarkStart(strTestName, clog.TimeKey.CRUIsLocked, nDUTidx);
                        _DCAsettings.CRUsetup.Setup();

                        DateTime tmStart;
                        bool bLocked = false;

                        for (int i = 0; i < _myConfig.iMaxCRULockRetry; i++)
                        {
                            tmStart = DateTime.UtcNow;
                            if (i > 0) // because there is a CRU relock applied ealier, abput 10lines above, inside _DCAsettings.CRUsetup.Setup();
                                _DCAsettings.CRUsetup.ReLock();
                            do
                            {
                                Pause(1000); // delay 1 sec
                                bLocked = _DCAsettings.CRUsetup.IsLocked();
                                if (bLocked)
                                {
                                    break;// break the do while loop
                                }
                                else
                                {
                                    Log("Info: CRU not locked after CRUsetup.Setup(), waiting until the last relock command takes effect");
                                }
                            } while ((DateTime.UtcNow - tmStart).TotalSeconds < _myConfig.dMaxCRULockTimeoutSecs);
                            if (bLocked)
                            {
                                break; // break the for loop
                            }
                        }

                        if (!bLocked)
                        {
                            OSScreenShot("CRUNoLock");
                            Log("ERROR: CRU not locked after setup()");

                            //if (StationHardware.Instance().myConfig.myAppConfig.bEnablePauseAtException)
                            //{
                            //    frmMsgBox cMsgBox = new frmMsgBox();
                            //    cMsgBox.Text = "DUT#" + (nDUTidx + 1);
                            //    cMsgBox.sMessage = "CRUNoLock Exception is caught, need Engineer to take a look.";
                            //    cMsgBox.TopMost = true;
                            //    cMsgBox.ShowDialog();
                            //}

                            throw new CRURelockFailException("ERROR: CRU not locked after setup()", "DCA_A86100C_Flex_400G");
                        }
                        else
                            Log("Info: CRU is relocked now");

                        clog.MarkEnd(strTestName, clog.TimeKey.CRUIsLocked, nDUTidx);
                    }
                    //_DCAsettings.PAM4setup.setup();
                }


                DetectErrorAndClear(out res);

                write("*CLS");

                //write(":LTESt:ACQuire:STATe OFF"); // HY

                //write(":SYSTem:MODE EYE"); // HY

                //turnOffAllChannelDisplay();

                //The 4 tiles split windows view
                //write(":DISPLAY:WINDOW:TIME1:DMODE TILED"); // HY


                // set acquisition setup => smoothing 
                // default is none
                if (_DCAsettings.bACQSmoothAverage)
                {
                    WriteAndCheckCmdStatus(":ACQuire:SMOothing AVERage");
                    WriteAndCheckCmdStatus(":ACQuire: ECOunt " + _DCAsettings.ACQSmoothAveNumWaveform.ToString());
                }
                else
                {
                    WriteAndCheckCmdStatus(":ACQuire:SMOothing NONE");
                }

                if (_DCAsettings.bSetupParam)
                {
                    WriteAndCheckCmdStatus(":MEASure:EYE:LIST:CLEAR");
                }

                ////set acquisition limit

                //// the following TDEQ pattern is defined in the DCA setup file, we do not want to be overwritten by _DCAsettings.PAM4AcqPatLimVal
                //////acquisition limit for eye test is sample which is defined in DCA setting file
                ////// we changed to waveforms 
                //////TDEQ pattern limit for PAM4 test, it is already defined in DCA setting file
                ////// but we repeat to make sure it again
                ////// then apply the number of pattern limit,PAM4AcqPatLimVal, defined in test sequencce
                WriteAndCheckCmdStatus(string.Format(":LTESt:ACQuire:CTYPe PATTerns"));
                WriteAndCheckCmdStatus(string.Format(":LTESt:ACQuire:CTYPe:PATTerns {0}", _DCAsettings.PAM4AcqPatLimVal));

                // enable limit test so that we can use OPC to check status upon limit is reached
                WriteAndCheckCmdStatus(":LTESt:ACQuire:STATe ON");

                // trigger pattern lock
                clog.MarkStart(strTestName, clog.TimeKey.TrigPatLocked, nDUTidx);
                for (int nTry = 0; nTry < 2; nTry++)
                {
                    // write(":TRIGger:PLOCk OFF");

                    WriteAndCheckCmdStatus(":TRIGger:PLOCk ON");

                    //Needed to ensure that pattern is locked. 
                    if (this._myConfig.bCheckBusyAfterPatternLock)
                    {

                        if (DetectErrorAndClear(out res))
                        {
                            continue;
                        }

                        this.Busy();
                    }
                    // only to check if Triger pattern locked to skip the much longer OPC check which also waits for TDEQ iterative optimiztion
                    this.TrigPatLocked();

                    if (DetectErrorAndClear(out res))
                    {
                        continue;
                    }
                }
                clog.MarkEnd(strTestName, clog.TimeKey.TrigPatLocked, nDUTidx);

                //write(":ACQuire:CDISplay"); // Clears // HY

                //Setautoscale();

                //write(":MEASure:EYE:LIST:CLEAR"); // HY

                if (_DCAsettings.bSetupParam)
                {
                    write(":MEASure:EYE:LIST:CLEAR");
                }

                if (bTurnOffAllDispFirst) turnOffAllChannelDisplay();

                // the following steps (return array setup, autoscale, run, extract and map measurement) will be executed whether bMeasureOnly is true or flase
                // to do the following till autoscale to allow DCA to re-lock the number of patterns defined in test sequence
                for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                {
                    mapMeasuredValues = new Dictionary<string, string>();
                    arMapMeasuredValues.Add(mapMeasuredValues);

                    mapMeasuredValuesStatus = new Dictionary<string, DCAcmd.eMeasureDataStatus>();
                    arMapMeasuredValuesStatus.Add(mapMeasuredValuesStatus);

                    channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                    SetupPAM4ChannelConfig(channelConfig, channelsGroupConfig.arChannels[c].arParams, mapMeasuredValues); // HY
                }

                if (_DCAsettings.sDisplayMode != "") write($":DISPlay:WINDow:TIME1:DMODe {_DCAsettings.sDisplayMode}");

                // at this point we will encounter conflicts and errors during above a few channel setting FOR loops
                // let's clear them before autoscale
                //DetectErrorAndClear(out res); //it is already in "Setautoscale" function
                clog.MarkEnd(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);

                // this autoscale must be done otherwise PAM4 eye diagrams are shifted up after the above few channel setting FOR loops
                bool bSaveScreen = false;
                for (int nTry = 0; nTry < _myConfig.nRetryAutoScale; nTry++)
                {
                    try
                    {
                        Setautoscale(bSaveScreen: bSaveScreen); // this autoscale must be done otherwise PAM4 eye diagrams are shifted up after the above few channel setting FOR loops
                        break;
                    }
                    catch (Exception ex) // the Busy() inside the above Setautoscale() would throw exception when timeout is reached
                    {
                        try
                        {
                            clog.Log("Fail to complete AutoScale, Stopping Acquisition.", nDUTidx);
                            write(":ACQUIRE:STOP");
                            if (_myConfig.bAutoScaleScreenShotWhenException)
                            {
                                string strFileName = string.Format("AutoScaleFail_afterBusy_{0}_dut{1}_{2}.jpg", strTestName, nDUTidx, CDateTimeUtil.getDateTimeString(DateTime.Now));
                                saveAutosclaeScreen(strFileName);
                            }
                        }
                        catch
                        {
                            //ignore error thrown here, most important is to let it proceed to next trial autoscale
                        }
                        if (nTry == _myConfig.nRetryAutoScale - 1) throw new Exception($"AutoScale Error meet number of try = {_myConfig.nRetryAutoScale}", ex);
                    }
                    finally
                    {
                        DetectErrorAndClear(out res);

                        if (_myConfig.bAutoScaleScreenShot) bSaveScreen = true; // only to save screen when fail at 1st time
                    }
                }
            }// end of if (bMeasureOnly==false
            else
            {
                for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                {
                    mapMeasuredValues = new Dictionary<string, string>();
                    arMapMeasuredValues.Add(mapMeasuredValues);

                    mapMeasuredValuesStatus = new Dictionary<string, DCAcmd.eMeasureDataStatus>();
                    arMapMeasuredValuesStatus.Add(mapMeasuredValuesStatus);
                }
                ClearScreen();
            }

            if (_DCAsettings.PAM4AcqPatLimVal > 1 || bMeasureOnly)
            {
                try
                {
                    //run and acquire 
                    clog.MarkStart(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                    write(":ACQ:RUN");
                    // *OPC? in Busy() will ensure all measurements of :ACQ:RUN to complete before proceeding to next section of retrieval
                    // wait for limit to reach
                    Busy();
                }
                finally
                {
                    clog.MarkEnd(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                }
            }

            try
            {
                this._ProtocolX.clearBuffer();

                //Retrieves the params
                clog.MarkStart(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                {
                    mapMeasuredValues = arMapMeasuredValues[c];
                    mapMeasuredValuesStatus = arMapMeasuredValuesStatus[c];
                    channelConfig = channelsGroupConfig.arChannels[c].channelSetting;

                    ExtractPAM4Param(channelConfig, channelsGroupConfig.arChannels[c].arParams,
                                        mapMeasuredValues, mapMeasuredValuesStatus);
                }
                clog.MarkEnd(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
            }
            catch (Exception ex)
            {
                clog.Error(ex, "Error at ExtractMeasurment", nDUTidx);
                throw;
            }

            // do not turn off so that limit will show in screen capture
            // turn off limit test only after measurement
            //write(":LTESt:ACQuire:STATe OFF");

            clog.MarkEnd(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        protected void ExtractEyeMeasurements(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            string strKey, strResult, strRootMeasCmd, strCmd, strParam;
            for (int i = 0; i < arParams.Count; i++)
            {
                try
                {
                    strParam = arParams[i];
                    strRootMeasCmd = EyeParamMap(strParam);
                    write(strRootMeasCmd + ":SOUR CHAN" + channelConfig.strChannelName);

                    //Special case setup
                    //Set the particular Jitter Type if we are making Jitter Measurements...
                    if (strParam.Contains("JitterPp"))
                    {
                        write(":MEASure:EYE:JITTer:FORMat PP");
                    }
                    else if (strParam.Contains("JitterRms"))
                    {
                        write(":MEASure:EYE:JITTer:FORMat RMS");
                    }

                    //Add some telemetry debug queries
                    strResult = query(strRootMeasCmd + ":STATus?").Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");

                    if (!strResult.ToUpper().Contains("COR"))
                    {
                        strResult = query(strRootMeasCmd + ":STATus:REASon?").Trim();
                        //strResult = removeNewLine(strResult);
                        //strResult = Regex.Replace(strResult, @"\n", "");
                    }

                    if (channelConfig.bOnlySampleSelectedParams)
                    {
                        if (channelConfig.arSelectedParamsToOnlySample.Contains(strParam))
                        {
                            strCmd = EyeParamMap(strParam) + "?";
                            strResult = query(strCmd).Trim();
                            //strResult = removeNewLine(strResult);
                            //strResult = Regex.Replace(strResult, @"\n", "");
                            strKey = strParam + "Sample";
                            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Min;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Max;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Mean;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Std;
                            mapMeasuredValues.Add(strKey, "0");
                            continue;
                        }
                    }

                    //General statistical query for all params... 
                    strCmd = EyeParamMap(strParam) + ":MIN?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + Min;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":MAX?";
                    strResult = query(strCmd).Trim();
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    //strResult = removeNewLine(strResult);
                    strKey = strParam + Max;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":MEAN?";
                    strResult = query(strCmd).Trim();
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    //strResult = removeNewLine(strResult);
                    strKey = strParam + Mean;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":SDEV?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + Std;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + "?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + "Sample";
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());

                    strParam = arParams[i];

                    strKey = strParam + Min;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Max;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Mean;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Std;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + "Sample";
                    mapMeasuredValues.Add(strKey, "9999");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
		public bool captureMeasureJitter(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            //enter jitter mode
            write(":SYSTem:MODE JITTer");

            //Load the setup file if neccessary
            if (null != _DCAsettings)
            {
                if (null != _DCAsettings.sDCAConfigFile)
                {
                    if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
                    {
                        LoadSetupFile(_DCAsettings.sDCAConfigFile);
                    }
                }
            }

            write("*CLS");

            write(":LTESt:ACQuire:STATe OFF");

            //:MEASure:AMPLitude:DEFine:ANALysis OFF
            //:MEASure:AMPLitude:DEFine:UNITs UAMPlitude
            //:MEASure:AMPLitude:DEFine:ANALysis ON
            //:MEASure:AMPLitude:DEFine:UNITs WATT
            //:MEASure:AMPLitude:DEFine:RINoise:TYPe OMA
            //:MEASure:JITTer:LIST:DEFault
            //:MEASure:JITTer:JN
            //:MEASure:JITTer:JN:SJN J2
            //:MEASure:JITTer:JN
            //:MEASure:JITTer:DDPWs
            write(":MEASure:AMPLitude:DEFine:ANALysis OFF");
            write(":MEASure:AMPLitude:DEFine:ANALysis ON");
            write(":MEASure:AMPLitude:DEFine:RINoise:TYPe OMA");

            //Set the units for amplitude and jitter measurements...
            if (this._DCAsettings.bSetMeasUnitsType)
            {
                write(":MEASure:JITTer:DEFine:UNITs " + _DCAsettings.JitterMeasUnitType);
                write(":MEASure:AMPLitude:DEFine:UNITs " + _DCAsettings.AmplitudeMeasUnitType);
            }

            //write(":MEASure:JITTer:LIST:DEFault");
            //write(":MEASure:JITTer:JN");
            //write(":MEASure:JITTer:JN:SJN J2");
            //write(":MEASure:JITTer:JN");
            //write(":MEASure:JITTer:DDPWs");

            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            write(":LTESt:ACQuire:STATe OFF");

            write(":ACQuire:CDISplay");

            SetTriggerSRC("FPAN");

            //set acquisition limit
            //write(":LTESt:ACQuire:CTYPe:PATTerns 1");

            SetFilterState(channelConfig.strChannelName, channelConfig.strFilterState);
            string strFilterState = GetFilterState(channelConfig.strChannelName).Trim();

            SetFilterType(channelConfig.strChannelName, channelConfig.strFilter);
            string strFilterType = GetFilterType(channelConfig.strChannelName).Trim();

            //Sets the default Amplitude measurement list
            write(":MEASure:AMPLitude:LIST:DEFault");

            Pause(1000);

            SetAcqLimitPatterns(Convert.ToDecimal(_DCAsettings.JittAcqLimVal));

            for (int nTry = 1; nTry <= 2; nTry++)
            {
                try
                {
                    Setautoscale();
                    Busy();
                    write("*CLS");
                    break;
                }
                catch (Exception ex)
                {
                    clog.Log($"Error@captureMeasureJitter: {ex}");
                }
            }

            SetPatternLength(Convert.ToString(_DCAsettings.JittPatLen));
            write(":LTESt:ACQuire:STATe ON");

            Pause(1000);

            write(":ACQ:RUN");

            //write("*OPC?", true);
            //complete = myN1010A.ReadString();

            Pause(45000);

            Busy();

            int i = 0;
            string strParam = "";
            mapMeasuredValues.Clear();

            //Retrieves the jitter params
            for (i = 0; i < arJitterParams.Count; i++)
            {
                strParam = arJitterParams[i];
                Debug.Assert(!mapMeasuredValues.ContainsKey(strParam));
                mapMeasuredValues.Add(strParam, ExtractJitterParam(strParam));
            }

            for (i = 0; i < arAmplitudeParams.Count; i++)
            {
                strParam = arAmplitudeParams[i];
                Debug.Assert(!mapMeasuredValues.ContainsKey(strParam));
                mapMeasuredValues.Add(strParam, ExtractAmplitudeParam(strParam));
            }

            // turn off limit test only after measurement
            write(":LTESt:ACQuire:STATe OFF");

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <returns></returns>
		public bool setupRINmeas(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams)
        {
            write("*CLS");

            //enter jitter mode
            write(":SYSTem:MODE JITTer");
            //myN1010A.WriteString("*OPC?", true);
            //complete = myN1010A.ReadString();

            //Load the setup file if neccessary
            if (null != _DCAsettings)
            {
                if (null != _DCAsettings.sDCAConfigFile)
                {
                    if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
                    {
                        LoadSetupFile(_DCAsettings.sDCAConfigFile);
                    }
                }
            }

            SetFilterState(channelConfig.strChannelName, channelConfig.strFilterState);
            string strFilterState = GetFilterState(channelConfig.strChannelName).Trim();

            SetFilterType(channelConfig.strChannelName, channelConfig.strFilter);
            string strFilterType = GetFilterType(channelConfig.strChannelName).Trim();


            //:MEASure:AMPLitude:DEFine:ANALysis OFF
            //:MEASure:AMPLitude:DEFine:UNITs UAMPlitude
            //:MEASure:AMPLitude:DEFine:ANALysis ON
            //:MEASure:AMPLitude:DEFine:UNITs WATT
            //:MEASure:AMPLitude:DEFine:RINoise:TYPe OMA
            //:MEASure:JITTer:LIST:DEFault
            //:MEASure:JITTer:JN
            //:MEASure:JITTer:JN:SJN J2
            //:MEASure:JITTer:JN
            //:MEASure:JITTer:DDPWs
            //write(":MEASure:AMPLitude:DEFine:ANALysis OFF");
            write(":MEAS:AMPL:DEF:ANAL ON");
            write(":MEAS:AMPL:DEF:RIN:TYPe OLEVel");// + _DCAsettings.RINoiseType);
            write(":MEAS:AMPL:DEF:RIN:UNITs DEC");// + _DCAsettings.RINoiseUnits);
            write(":MEAS:AMPL:DEF:LEV AVER");

            //Set the units for amplitude and jitter measurements...
            if (this._DCAsettings.bSetMeasUnitsType)
            {
                write(":MEAS:JITT:DEF:UNITs " + _DCAsettings.JitterMeasUnitType);
                write(":MEAS:AMPL:DEF:UNITs " + _DCAsettings.AmplitudeMeasUnitType);
            }

            //write(":MEASure:JITTer:LIST:DEFault");
            //write(":MEASure:JITTer:JN");
            //write(":MEASure:JITTer:JN:SJN J2");
            //write(":MEASure:JITTer:JN");
            //write(":MEASure:JITTer:DDPWs");

            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            write(":LTESt:ACQuire:STATe OFF");

            write(":ACQ:CDISplay");

            SetTriggerSRC("FPAN");

            //set acquisition limit
            //write(":LTESt:ACQuire:CTYPe:PATTerns 1");

            //Sets the default Amplitude measurement list
            //if (null != arAmplitudeParams)
            //{
            //    write(":MEAS:AMPL:LIST:DEF");
            //}
            //else
            //{
            //    write(":MEAS:AMPL:LIST:CLEAR");
            //}

            write(":MEAS:JITTER:LIST:CLEAR");
            //if (null != arJitterParams)
            //{
            //    write(":MEAS:JITTER:LIST:DEF");
            //}
            //else
            //{
            //    write(":MEAS:JITTER:LIST:CLEAR");
            //}

            SetAcqLimitPatterns(Convert.ToDecimal(_DCAsettings.JittAcqLimVal));
            SetAvgCount(Convert.ToInt32(_DCAsettings.JittAvg));
            SetPatternLength(Convert.ToString(_DCAsettings.JittPatLen));

            write(":LTESt:ACQ:STAT OFF");

            write("*CLS");

            Setautoscale();

            Busy(); //query("*OPC?");

            string res = query(":SYST:ERR?");

            write("*CLS");

            write(":LTESt:ACQ:STAT ON");

            write("*CLS");

            write(":ACQuire:RUN");

            Busy();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="list"></param>
		protected void SetupScopeParam(string strChannelName, List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                write(":MEASure:OSC:" + list[i] + ":SOUR CHAN" + strChannelName);
                write(":MEASure:OSC:" + list[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool measureSCOPEmode(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            write("*CLS");

            //enter SCOPE mode
            write(":SYSTem:MODE OSC");
            //myN1010A.WriteString("*OPC?", true);
            //complete = myN1010A.ReadString();

            if (null != _DCAsettings.CRUsetup)
            {
                _DCAsettings.CRUsetup.Setup(false);
            }
            // LT:  Please make this SLOT2 configurable in the CChannelSettings
            write(":SLOT2:TRIGger:TRACking ON");

            write(":MEAS:OSC:LIST:CLEAR");

            string res = SetupScopeChannelConfig(channelConfig, arParams);

            //SetAcqLimitPatterns(Convert.ToDecimal(_DCAsettings.OscAcqLimType));
            //SetPatternLength(Convert.ToString(_DCAsettings.OscAcqLimVal));

            write(":TRIG:PLOCK ON");
            res = query(":SYST:ERR?");

            write(":LTESt:ACQ:STAT ON");
            res = query(":SYST:ERR?");

            Busy(); //res = query("*OPC?").ToUpper();

            write(":LTESt:ACQ:CTYPE:WAV " + _DCAsettings.OscAcqLimVal);
            res = query(":SYST:ERR?");

            string strMode = query(":SYSTem:MODE?").ToUpper();

            Setautoscale();

            write(":ACQuire:RUN");
            Busy();

            Pause(1000);

            ExtractScopeAcquiredParams(channelConfig, arParams, mapMeasuredValues);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        protected void ExtractScopeAcquiredParams(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            int i = 0;
            string strParam = "";
            mapMeasuredValues.Clear();

            if (null != arParams)
            {
                for (i = 0; i < arParams.Count; i++)
                {
                    strParam = arParams[i];
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strParam));
                    mapMeasuredValues.Add(strParam, ExtractOscParam(channelConfig.strChannelName, strParam));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <returns></returns>
        protected string SetupScopeChannelConfig(CChannelSettings channelConfig, List<string> arParams)
        {
            write(":CHAN" + channelConfig.strChannelName + ":DISPlay ON");

            //Apply ATTEN
            if (channelConfig.strAttenState.Contains("ON"))
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE ON");
                string res1 = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                write(":CHAN" + channelConfig.strChannelName + ":ATT:DEC " + channelConfig.strAttenVal);
                res1 = query(":CHAN" + channelConfig.strChannelName + ":ATT:DEC?");
            }
            else
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE OFF");
                query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
            }

            //Setup channels wavelength
            string strWavelength = "";
            SetWavelength(channelConfig.strChannelName, channelConfig.strWavelength);
            strWavelength = GetWavelength(channelConfig.strChannelName);

            string res = string.Empty;

            //Set the units for amplitude and jitter measurements...
            if (this._DCAsettings.bSetMeasUnitsType)
            {
                write(":MEASure:OSCilloscope:APOWer:UNITs DBM");
                res = query(":SYST:ERR?");
            }

            SetupScopeParam(channelConfig.strChannelName, arParams);
            return res;
        }

        /// <summary>
        /// Measure in Scope Mode
        /// </summary>
        /// <param name="channelsGroupConfig">Channel Setting Object</param>
        /// <param name="arMapMeasuredValues">Output Measruement Values</param>
        /// <returns>true: Success; false: Fail</returns>
        public bool measureSCOPEmodeParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues)
        {
            arMapMeasuredValues = new List<Dictionary<string, string>>();

            write("*CLS");
            write("*RST");

            //enter SCOPE mode
            write(":SYSTem:MODE OSC");
            //myN1010A.WriteString("*OPC?", true);
            //complete = myN1010A.ReadString();

            write(":MEASure:TBASe:METHod STANdard");

            write(":MEAS:OSC:LIST:CLEAR");

            turnOffAllChannelDisplay();

            if (null != _DCAsettings.CRUsetup)
            {
                _DCAsettings.CRUsetup.Setup(false);
            }

            int c;
            CChannelSettings channelConfig;
            List<string> arParams;
            Dictionary<string, string> mapMeasuredValues;

            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = new Dictionary<string, string>();
                arMapMeasuredValues.Add(mapMeasuredValues);
                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;
                SetupScopeChannelConfig(channelConfig, arParams);
            }

            //The 4 tiles split windows view
            write(":DISPLAY:WINDOW:TIME1:DMODE TILED");

            //SetAcqLimitPatterns(Convert.ToDecimal(_DCAsettings.OscAcqLimType));
            //SetPatternLength(Convert.ToString(_DCAsettings.OscAcqLimVal));

            write(":TRIG:PLOCK ON");
            string res = query(":SYST:ERR?");

            write(":LTESt:ACQ:STAT ON");
            res = query(":SYST:ERR?");

            Busy(); //res = query("*OPC?").ToUpper();

            if (_DCAsettings.OscAcqLimVal != null)
            {
                if (_DCAsettings.OscAcqLimVal != "")
                {
                    write(":LTESt:ACQ:CTYPE:WAV " + _DCAsettings.OscAcqLimVal);
                    res = query(":SYST:ERR?");
                }
            }

            string strMode = query(":SYSTem:MODE?").ToUpper();

            //Setautoscale();

            write(":ACQuire:RUN");
            Busy();

            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = arMapMeasuredValues[c];
                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;
                ExtractScopeAcquiredParams(channelConfig, arParams, mapMeasuredValues);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
		public bool measureRIN(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            write("*CLS");

            string strMode = query(":SYSTem:MODE?").ToUpper();
            if (!strMode.Contains("JITT"))
            {
                setupRINmeas(channelConfig, arJitterParams, arAmplitudeParams);
            }

            Setautoscale();
            write(":ACQuire:RUN");
            Busy();


            //write(":ACQuire:RUN");
            //write("*OPC?", true);
            //complete = myN1010A.ReadString();



            int i = 0;
            string strParam = "";
            mapMeasuredValues.Clear();

            if (null != arJitterParams)
            {
                //Retrieves the jitter params
                for (i = 0; i < arJitterParams.Count; i++)
                {
                    strParam = arJitterParams[i];
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strParam));
                    mapMeasuredValues.Add(strParam, ExtractJitterParam(strParam));
                }
            }

            if (null != arAmplitudeParams)
            {
                for (i = 0; i < arAmplitudeParams.Count; i++)
                {
                    strParam = arAmplitudeParams[i];
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strParam));
                    mapMeasuredValues.Add(strParam, ExtractAmplitudeParam(strParam));
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
		protected DCASettings _DCAsettings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAsettings"></param>
		public virtual void setDCAsettings(DCASettings DCAsettings)
        {
            _DCAsettings = DCAsettings;

            //Load the setup file if neccessary
            if (null != _DCAsettings)
            {
                if (null != _DCAsettings.sDCAConfigFile)
                {
                    if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
                    {
                        LoadSetupFile(_DCAsettings.sDCAConfigFile);
                    }
                }

                if (null != _DCAsettings.PAM4setup)
                {
                    this.initFunctor(_DCAsettings.PAM4setup);
                    _DCAsettings.PAM4setup.Setup();
                }

                if (null != _DCAsettings.CRUsetup)
                {
                    this.initFunctor(_DCAsettings.CRUsetup);
                    //_DCAsettings.CRUsetup.setup();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKeyDCAsettings"></param>
        public void setDCAsettings(string strKeyDCAsettings)
        {
            _DCAsettings = this._myConfig.mapDCASettings[strKeyDCAsettings];
            setDCAsettings(_DCAsettings);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Set4ChannelWindow()
        {
            write(":DISPlay:WINDOW:TIME1:DMODE TILED");
        }

        /// <summary>
        /// 
        /// </summary>
		public void SetupEyeConfig()
        {
            write(":MEASure:TBASe:METHod STANdard");
            write(":MEASure:THReshold:METHod P205080");
            write(":MEASure:THReshold:EREFerence OZERo");
            write(":MEASure:EBOundary:LEFT 4.90E+1");
            write(":MEASure:EBOundary:RIGHt 5.10E+1");
        }

        /// <summary>
        /// 
        /// </summary>
		public void SetupJitterConfig()
        {
            write(":SYSTem:MODE JITTer");
            //write(":MEASure:JITTer:DEFine:UNITs UINTerval");
            write(":MEASure:JITTer:DEFine:UNITs SEC");
            write(":MEASure:JITTer:DEFine:LEVel:PERCent 5.00E+1");
            write(":MEASure:JITTer:DEFine:SIGNal:AUTodetect ON");
            write(":MEASure:JITTer:DEFine:EDGe BEDGes");
            write(":MEASure:AMPLitude:DEFine:ANALysis ON");
            write(":MEASure:AMPLitude:DEFine:LOCation 5.00E+1");
            write(":MEASure:AMPLitude:DEFine:LEVel CIDigits");
            write(":MEASure:AMPLitude:DEFine:LEVel:CIDigits:LEADing 4");
            write(":MEASure:AMPLitude:DEFine:LEVel:CIDigits:LAGGing 4");
            write(":MEASure:AMPLitude:DEFine:UNITs VOLT");
            write(":MEASure:AMPLitude:DEFine:RINoise:TYPe OMA");
            write(":MEASure:AMPLitude:DEFine:RINoise:UNITs DECibel");

            //write(":DISPlay:Location:PRIM:AWIN JGR");            
            //write(":DISPlay:Location:LSEC:AWIN JITT");
            //write(":DISPlay:Location:RSEC:AWIN TIME1");

            write(":DISPlay:Location:LSEC:AWIN JITT");
            write(":DISPlay:Location:RSEC:AWIN JGR");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTimeoutSec"></param>
        /// <returns></returns>
        public override bool setTimeout(int nTimeoutSec)
        {
            return this._ProtocolX.setTimeout(nTimeoutSec);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string CheckVerticalCalCmd(string sCh)
        {
            return $":CALibrate:CHANnel{sCh}:STATus?";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public string EnableVerticalCalCmd(string sCh, bool bEnable)
        {
            string sEnabled = bEnable ? "ENABled" : "DISabled";

            return $":CALibrate:CHANnel{sCh}:ENABled {sEnabled}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSlot"></param>
        /// <returns></returns>
        public string StartVerticalCalCmd(string sSlot)
        {
            return $":CALibrate:SLOT{sSlot}:STARt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public bool VerticalCal(List<string> ChanList, bool bForce)
        {
            bool bEnable;
            string sRet = string.Empty;
            List<string> lstSlot = new List<string>();

            foreach (string Chan in ChanList)
            {
                bEnable = !(query(CheckVerticalCalCmd(Chan)).Trim().Trim('\"') == "CALIBRATED") || bForce;
                write(EnableVerticalCalCmd(Chan, bEnable));
                if (bEnable)
                {
                    if (!lstSlot.Contains(Chan.Substring(0, 1)))
                    {
                        lstSlot.Add(Chan.Substring(0, 1));
                    }
                }
            }

            if (lstSlot.Count > 0)
            {
                foreach (string sSlot in lstSlot)
                {
                    write(StartVerticalCalCmd(sSlot));
                    sRet = CalibrateBusy();
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    sRet = CalibrateBusy();
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    Busy();
                }
            }

            return true;
        }

        private string CalibrateBusy()
        {
            string sRet = string.Empty;
            DateTime tmStart = DateTime.Now;
            do
            {
                try
                {
                    sRet = queryNoRetry(":CALibrate:SDONe?");
                    return sRet;
                }
                catch (Exception)
                {
                    if ((DateTime.Now - tmStart).TotalSeconds >= _myConfig.dMaxBusyTimeoutSecs)
                    {
                        throw;
                    }
                }
            } while ((DateTime.Now - tmStart).TotalSeconds < _myConfig.dMaxBusyTimeoutSecs);
            return sRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string CheckDarkCalCmd(string sCh)
        {
            return $"CALibrate:DARK:CHANnel{sCh}:STATus?";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string StartDarkCalCmd(string sCh)
        {
            return $":CALibrate:DARK:CHANnel{sCh}:STARt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public bool DarkCal(List<string> ChanList, bool bForce)
        {
            bool bEnable;
            string sRet = string.Empty;

            foreach (string Chan in ChanList)
            {
                bEnable = !(query(CheckDarkCalCmd(Chan)).Trim().Trim('\"') == "CALIBRATED") || bForce;

                if (bEnable)
                {
                    write(StartDarkCalCmd(Chan));
                    sRet = CalibrateBusy();
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    sRet = CalibrateBusy();
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    Busy();
                }
            }

            return true;
        }

        /// <summary>
        /// Measure AOP
        /// </summary>
        /// <param name="DCAChan">DCA channel name</param>
        /// <param name="AcqDelay">delay setting acquisition</param>
        public virtual double MeasureAOP(string DCAChan, int AcqDelay = 0)
        {
            string cmd;
            string result;

            write("*CLS");
            write(":SYSTem:MODE EYE");


            write(":ACQ:RUN");
            Busy();
            Pause(AcqDelay);
            write(":MEASure:EYE:APOWer:UNITs DBM");
            cmd = ":MEASure:EYE:APOWer";
            write(cmd + ":SOUR CHAN" + DCAChan);
            result = query(cmd + "?");
            double dPowerLevel;
            double.TryParse(result, out dPowerLevel);
            write(":ACQ:STOP");
            Busy();
            return dPowerLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAChan"></param>
        /// <param name="MeasureParams"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="AcqDelay"></param>
        public virtual void SimpleMeasurement(string DCAChan, List<string> MeasureParams, out Dictionary<string, string> arMapMeasuredValues, int AcqDelay = 0)
        {
            string cmd;
            string result;
            bool bAcqLimit = false;
            int iAcqLimit = 0;
            arMapMeasuredValues = new Dictionary<string, string>();

            if (MeasureParams.Contains("OMA"))
            {
                bAcqLimit = query(":LTESt:ACQuire:STATe?").Trim() == "1" ? true : false;
                if (bAcqLimit) iAcqLimit = Convert.ToInt32(query(":LTESt:ACQuire:CTYPe:PATTerns?"));
                write(":CRECOVERY2:RELOCK");
                Busy();
                write(":SYSTEM:AUTOSCALE");
                Busy();
            }

            if (!bAcqLimit || (bAcqLimit && iAcqLimit > 1))
            {
                write(":ACQ:RUN");
                Busy();
            }
            if (!bAcqLimit) Pause(AcqDelay);

            foreach (string Param in MeasureParams)
            {
                switch (Param)
                {
                    case "AOP":
                        cmd = ":MEASure:EYE:APOWer";
                        break;
                    case "OMA":
                        cmd = ":MEASure:EYE:OOMA";
                        break;
                    default:
                        throw new Exception($"{Param} is unknown Measurement Parameter");
                }
                write(cmd + ":SOUR CHAN" + DCAChan);
                result = query(cmd + "?");
                arMapMeasuredValues.Add(Param, result);
            }

            write(":ACQ:STOP");
            Busy();
        }

        ///// <summary>
        ///// RF switch object
        ///// </summary>
        //protected IRFSwitch _RFSwitch;

        /// <summary>
        /// Station hardware object
        /// </summary>
        protected StationHardware _stationInstance;


        string waveformDir = @"C:/Users/lab_spsgtest/Documents/Keysight/FlexDCA/Waveforms";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="ConfiguredChannels"></param>
        public void SavePatternWaves(IReadOnlyList<string> fileNames, string[] ConfiguredChannels)
        {
            if (_myConfig.bSimulation)
            {
                return;
            }

            if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));

            write(":DISK:WAV:SAVE:FTYP WPIN");
            for (int i = 0; i < ConfiguredChannels.Length; i++)
            {
                write(string.Format(@":DISK:WAV:FNAM '{0}/{1}'", waveformDir, fileNames[i]));
                write(string.Format(":DISK:WAV:SAVE:SOUR {0}", ConfiguredChannels[i]));
                write(":DISK:WAV:SAVE");
            }
            Busy(); //query("*OPC?");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="ConfiguredChannels"></param>
        /// <returns></returns>
        public List<byte[]> GetPatternWaves(IReadOnlyList<string> fileNames, string[] ConfiguredChannels)
        {
            List<byte[]> waveforms = new List<byte[]>();
            if (_myConfig.bSimulation)
            {
                for (int i = 0; i < ConfiguredChannels.Length; i++)
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SimulatedWaveform.wfmx");
                    var bytes = File.ReadAllBytes(fileName);
                    waveforms.Add(bytes);
                    System.Threading.Thread.Sleep(100);
                }
            }
            else
            {
                if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));

                // if FlexDCA runs locally, it is faster to let FlexDCA store the waveform to a file and read the bytes from there!
                // was WINT but because pattern acquire is on, changed to WPIN write(":DISK:WAV:SAVE:FTYP WINT");
                // http://rfmw.em.keysight.com/DigitalPhotonics/flexdca/PG/Content/Topics/Commands/DISK/WAVeform_SAVE_FTYPe.htm 
                write(":DISK:WAV:SAVE:FTYP WPIN");
                for (int i = 0; i < ConfiguredChannels.Length; i++)
                {
                    write(string.Format(@":DISK:WAV:FNAM '{0}/{1}'", waveformDir, fileNames[i]));
                    write(string.Format(@":DISK:WAV:SAVE:SOUR {0}", ConfiguredChannels[i]));
                    write(":DISK:WAV:SAVE");
                }
                Busy(); //query("*OPC?"); 
                foreach (string strFN in fileNames)
                {
                    string strPath = string.Format(@"{0}/{1}", waveformDir, strFN);
                    var bytes = File.ReadAllBytes(strPath);
                    waveforms.Add(bytes);
                    File.Delete(strPath);
                }
            }

            return waveforms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="bMeasureOnly"></param>
        /// <returns></returns>
        public List<byte[]> acqWaveform(CChannelsGroupParallel channelsGroupConfig, bool bMeasureOnly = false)
        {
            throw new NotImplementedException();
        }

        Task<Tuple<List<string>, List<string>>> IScope.ExecuteOffloadComputeAsync(List<byte[]> patternWavesAllMeasureGroups, CChannelsGroupParallel channelsGroupConfig, List<Dictionary<string, string>> arMapMeasuredValues, List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus)
        {
            throw new NotImplementedException();
        }

        private void OSScreenShot(string strMethodName)
        {
            if (!_myConfig.bTakeScreenShot) return;

            //string strlocalFilePath = ((ModuleLevelTestAppConfig)StationHardware.Instance().myConfig.myAppConfig).LocalHSOutputDir;
            //strlocalFilePath = Utility.CFileOperation.CreateLocalOutputFolder(strlocalFilePath, "Raw");
            // commented - year & month already added in LocalHSOutputDir
            //DateTime dt = DateTime.Now;
            //strlocalFilePath = Path.Combine(strlocalFilePath, $"{dt:yyyyMM}//{dt:MM_dd}//{strMethodName}_{dt:yyyyMMddHHmmss}.jpg");

            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.WorkingDirectory = Environment.CurrentDirectory;
            Log($"WorkingDirectory = {procStartInfo.WorkingDirectory}");
            //procStartInfo.Arguments = $"Keysight.N1000 {strlocalFilePath}";
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
        /// Use AOP and ER to calculate OMA
        /// </summary>
        /// <param name="arParams">Measurement Parameters Collection</param>
        /// <param name="AOPparam">Parameter Name of AOP</param>
        /// <param name="OERparam">Parameter Name of OER</param>
        /// <param name="OOMAparam">Parameter Name of OOMA</param>
        /// <param name="mapMeasuredValues">Measurement Result Collection</param>
        protected void OMACalculation(List<string> arParams, string AOPparam, string OERparam, string OOMAparam, ref Dictionary<string, string> mapMeasuredValues)
        {
            if (_myConfig.bUseOOMACalcMethod && arParams.Contains(ScopeConst.OOMA) && mapMeasuredValues.ContainsKey(AOPparam) && mapMeasuredValues.ContainsKey(OERparam))
            {
                if (mapMeasuredValues[AOPparam] != "-999" && mapMeasuredValues[OERparam] != "-999")
                {
                    double dAOP_uW = Math.Pow(10, Convert.ToDouble(mapMeasuredValues[AOPparam]) / 10) * 1000;
                    clog.Log($"dAOP_uW = 1000 * 10^((AOP_dB:{mapMeasuredValues[AOPparam]})/10) = {dAOP_uW}", nDUTidx);
                    double dER_Ratio = Math.Pow(10, Convert.ToDouble(mapMeasuredValues[OERparam]) / 10);
                    clog.Log($"dER_Ratio = 10^((OER:{mapMeasuredValues[OERparam]})/ 10) = {dER_Ratio}", nDUTidx);

                    double dOMA_uW = 2 * dAOP_uW * (dER_Ratio - 1) / (dER_Ratio + 1);
                    clog.Log($"dOMA_uW = 2 * dAOP_uW * (dER_Ratio - 1) / (dER_Ratio + 1) = {dOMA_uW}", nDUTidx);
                    mapMeasuredValues[OOMAparam] = (10 * Math.Log10(dOMA_uW / 1000)).ToString();
                    clog.Log($"Calculated Measurement of {OOMAparam} = 10 * Log10(dOMA_uW / 1000) = {mapMeasuredValues[OOMAparam]}", nDUTidx);
                }
            }
        }
    }

}

