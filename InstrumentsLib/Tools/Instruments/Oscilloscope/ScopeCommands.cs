using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;
using Utility;
using System.IO;
using NationalInstruments.VisaNS;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    #region DCA COMMAND FUNCTORS

    /// <summary>
    /// 
    /// </summary>
    public class DCASetupcmd : MeasurementCmd
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> arCustomCmds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<string, bool> WriteAndCheckCmdStatus;

        /// <summary>
        /// 
        /// </summary>
        public DCASetupcmd()
        {
            arCustomCmds = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool executeCustomCmds()
        {
            if (null == arCustomCmds)
            {
                return false;
            }

            for (int n = 0; n < arCustomCmds.Count; n++)
            {
                Write(arCustomCmds[n]);
            }

            return true;
        }

        /// <summary>
        /// Check OPC status to ensure DCA operation completed
        /// </summary>
        /// <param name="dMaxBusyTimeoutSecs">maximum timeout to wait</param>
        /// <returns>true: Operation Completed; false: Operation Incompleted</returns>
        protected bool waitUnitOperationComplete(double dMaxBusyTimeoutSecs = 10.0)
        {
            //string res = "";
            //DateTime start = DateTime.Now;
            //TimeSpan ts;

            //ts = DateTime.Now - start;
            //bool bOperationComplete = false;
            //while (ts.TotalSeconds < maxWait_secs)
            //{
            //    res = Query("*OPC?").ToUpper();
            //    if (res.Contains("1"))
            //    {
            //        bOperationComplete = true;
            //        break;
            //    }
            //}

            //return bOperationComplete;


            // Poll for completion
            int nValue = 0;
            TimeSpan ts;
            DateTime tmStart = DateTime.UtcNow;
            ts = DateTime.UtcNow - tmStart;
            bool bSucceded = false;
            Write("*OPC");
            while (false == bSucceded && ts.TotalSeconds < dMaxBusyTimeoutSecs)
            {
                try
                {
                    if (int.TryParse(Query("*ESR?"), out nValue))
                    {
                        if (1 == (nValue & 1))
                        {
                            bSucceded = true;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(500); // wait for 0.5 sec
                        }
                    }
                    else
                    {
                        Thread.Sleep(500); // wait for 0.5 sec 
                    }

                    ts = DateTime.UtcNow - tmStart;
                }
                catch (Exception ex)
                {
                    ts = DateTime.UtcNow - tmStart;
                    if (ts.TotalSeconds > dMaxBusyTimeoutSecs)
                    {
                        clog.Error(ex, string.Format("Visa Query throw Exception with {0}", ex.GetType().ToString()));
                        throw;
                    }
                }
            }

            return bSucceded;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class PAMSetupcmd : DCASetupcmd
    {
        /// <summary>
        /// 
        /// </summary>
        public string EYE_ESTiming { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EYE_ELMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EYE_PPERcent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EYE_TIME_LTDefinition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EYE_TIME_UNITs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AMPL_UNITs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_PRESets { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_TSER { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_HWIDth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_OHTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_OHSeparation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_OHTHresholds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TDEQ_TALimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PATTernsLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReLock"></param>
        /// <returns></returns>
        public override bool Setup(bool bReLock = true)
        {
            //:MEASure:PAM:EYE:ESTiming CEYE
            //write(":MEASure:PAM:EYE:ESTiming " + EYE_ESTiming);

            ////:MEASure:PAM:EYE:ELMethod MEHeight
            //write(":MEASure:PAM:EYE:ELMethod " + EYE_ELMethod);

            ////:MEASure:PAM:EYE:PPERcent 9
            //write(":MEASure:PAM:EYE:PPERcent " + EYE_PPERcent);

            ////:MEASure:PAM:EYE:TIME:LTDefinition ECENter
            //write(":MEASure:PAM:EYE:TIME:LTDefinition "  + EYE_TIME_LTDefinition);

            ////:MEASure:PAM:EYE:TIME:UNITs UINTerval
            //write(":MEASure:PAM:EYE:TIME:UNITs " + EYE_TIME_UNITs);

            ////:MEASure:PAM:AMPLitude:UNITs PERCent
            //write(":MEASure:PAM:AMPLitude:UNITs " + AMPL_UNITs);

            ////:MEASure:TDEQ:PRESets "IEEE 802.3cd Draft 3.5"
            //write(":MEASure:TDEQ:PRESets " + TDEQ_PRESets);

            ////:MEASure:TDEQ:TSER 5.00E-4
            //write(":MEASure:TDEQ:TSER " + TDEQ_TSER);

            ////:MEASure:TDEQ:HWIDth 3E-2
            //write(":MEASure:TDEQ:HWIDth " + TDEQ_HWIDth);

            ////:MEASure:TDEQ:OHTime ON
            //write(":MEASure:TDEQ:OHTime " + TDEQ_OHTime);

            ////:MEASure:TDEQ:OHSeparation 1.1E-1
            //write(":MEASure:TDEQ:OHSeparation " + TDEQ_OHSeparation);

            ////:MEASure:TDEQ:OHTHresholds ON
            //write(":MEASure:TDEQ:OHTHresholds " + TDEQ_OHTHresholds);

            ////:MEASure:TDEQ:TALimit 2.0
            //write(":MEASure:TDEQ:TALimit " + TDEQ_TALimit);

            //write(":LTESt:ACQuire:STATe ON");

            //write(":LTESt:ACQuire:CTYPe:PATTerns " + PATTernsLimit);


            //:CHAN2A:SIGNal:TYPE:AUTO OFF
            //write(":CHAN2A:SIGNal:TYPE:AUTO OFF");

            //:CHAN2A:SIGNal:TYPE NRZ
            //write(":CHAN2A:SIGNal:TYPE NRZ");

            executeCustomCmds();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public PAMSetupcmd()
        {
            EYE_ESTiming = "CEYE";
            EYE_ELMethod = "MEHeight";
            EYE_PPERcent = "9";
            EYE_TIME_LTDefinition = "ECENter";
            EYE_TIME_UNITs = "UINTerval";
            AMPL_UNITs = "PERCent";
            TDEQ_PRESets = "IEEE 802.3cd Draft 3.5";
            TDEQ_TSER = "5.00E-4";
            TDEQ_HWIDth = "3E-2";
            TDEQ_OHTime = "ON";
            TDEQ_OHSeparation = "1.1E-1";
            TDEQ_OHTHresholds = "ON";
            TDEQ_TALimit = "2.0";
            PATTernsLimit = "15";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TEQualizerSetupcmd : DCASetupcmd
    {
        /// <summary>
        /// 
        /// </summary>
        public bool bOffloadCompute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool bSeperateAcqTEQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TapsPerUI { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumOfTaps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxPrecursors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AverageCount { get; set; }
        /// <summary>
        /// this var determines how many of available DCA channels are used. even though DCA has dual channel, we may only want to use 1 channel.
        /// </summary>
        public int nChannelUsed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumDCAOptions TDEQThresholdOptimization { get; set; }

        /// <summary>
        /// what channels used for DCA measurement; for example, DR4 2-ch DCAM { "CHAN1A", "CHAN1B" }; FR1 1-ch DCAM { "CHAN1A" }
        /// </summary>
        public List<string> DCAMeasureChannels  { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumTEQualizerPreset TdecqConfigurationPreset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumTEQualizerPreset TdecqEqualizerPreset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumDCAOptions IterativeOptimization { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumEyeLinearityDefinition EyeLinearityDefinition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumEyeWidthOpeningDefinition EyeWidthOpeningDefinition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EyeWidthOpeningProbability { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReLock"></param>
        /// <returns></returns>
        public override bool Setup(bool bReLock = true)
        {  
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public TEQualizerSetupcmd()
        {
            // setting for TEQualizer and all related parameters
            bOffloadCompute = false;
            bSeperateAcqTEQ = false;
            TapsPerUI = (int)EnumDCAOptions.Default;
            NumOfTaps = (int)EnumDCAOptions.Default;
            MaxPrecursors = (int)EnumDCAOptions.Default;
            AverageCount = 0; // i.e. no average
            nChannelUsed = 2;
            DCAMeasureChannels = new List<string> { "CHAN1A", "CHAN1B" };  //  for DR4, 2-ch DCAM
            //// for FR1, 1-ch DCAM
            //DCAMeasureChannels = new List<string> { "CHAN1A" };
            // the following for PAM4 SDK configuration
            TdecqConfigurationPreset = EnumTEQualizerPreset.CDFinal;
            TdecqEqualizerPreset = EnumTEQualizerPreset.CDFinal;
            IterativeOptimization = EnumDCAOptions.OFF;
            EyeLinearityDefinition = EnumEyeLinearityDefinition.RLMC94;
            EyeWidthOpeningDefinition = EnumEyeWidthOpeningDefinition.ZHITs;
            EyeWidthOpeningProbability = 1e-6;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CRUSetupcmd : DCASetupcmd
    {
        /// <summary>
        /// 
        /// </summary>
        public string SLOT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ODRatioAUTO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SOURCE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CRATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ODRatio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EHGain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Preset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double dMaxBusyTimeoutSecs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLocked()
        {
            string strResponse = this.Query($":CRECovery{SLOT}:Locked?");
            if (strResponse.Contains("1"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReLock"></param>
        /// <returns></returns>
        public override bool Setup(bool bReLock = true)
        {
            //:CRECovery1:SOURce OPTical
            //:CRECovery1:CRATe 5.3125000E+10
            //:CRECovery1:ODRatio SUB4
            //:CRECovery1:RELock
            // * OPC?

            // PER Standardization Agreement, Slot => "1" implies we are using an internal CRU
            if (SLOT.Equals("1"))
            {
                this.Write($":SLOT{SLOT}:TRiGger:SOURce CRECovery");
            }

            if (Preset != "")
            {
                Write($":CRECovery{SLOT}:PRESets \"{ Preset}\"");
            }
            this.Write($":CRECovery{SLOT}:SOUR " + this.SOURCE);
            this.Write($":CRECovery{SLOT}:CRATe " + this.CRATE);
            this.Write($":CRECovery{SLOT}:ODRatio:AUTO " + this.ODRatioAUTO);
            if (this.ODRatioAUTO.Contains("OFF"))
            {
                this.Write($":CRECovery{SLOT}:ODRatio " + this.ODRatio);
            }

            if (!SLOT.Equals("1")) this.Write($":CRECovery{SLOT}:EHGain " + this.EHGain);
            if (bReLock)
            {
                waitUnitOperationComplete(this.dMaxBusyTimeoutSecs);

                this.Write($":CRECovery{SLOT}:RELock ");

                waitUnitOperationComplete(this.dMaxBusyTimeoutSecs);
            }

            executeCustomCmds();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ReLock()
        {
            this.Write($":CRECovery{SLOT}:RELock ");

            waitUnitOperationComplete(this.dMaxBusyTimeoutSecs);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public CRUSetupcmd()
        {
            this.SOURCE = "OPTical";
            this.ODRatioAUTO = "OFF";
            this.CRATE = "5.3125000E+10";
            this.ODRatio = "SUB4";
            this.SLOT = "2";
            this.EHGain = "ON";
            Preset = "";
            this.dMaxBusyTimeoutSecs = 30;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DCAcmd : MeasurementCmd
    {
        /// <summary>
        /// 
        /// </summary>
        public const string EYE = "EYE";
        /// <summary>
        /// 
        /// </summary>
        public const string OSC = "OSC";
        /// <summary>
        /// 
        /// </summary>
        public const string JITTER = "JITTER";

        /// <summary>
        /// 
        /// </summary>
        public Func<string, bool> WriteAndCheckCmdStatus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public delegate string delegateExtractParam(string p1, string p2, out string p3);

        /// <summary>
        /// 
        /// </summary>
        public delegateExtractParam ExtractParam;

        /// <summary>
        /// 
        /// </summary>
        public string bSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DCA_A86100CFlex400GConfig _myConfig;

        /// <summary>
        /// Measurement Mode of DCA
        /// </summary>
        public string subSystem { get; set; }

        /// <summary>
        /// Measurement Parameter of DCA
        /// </summary>
        public string strParam { get; set; }

        /// <summary>
        /// Unit of Measurement Param
        /// </summary>
        public string strUnit { get; set; }

        /// <summary>
        /// Generic Method of Setting up Measurement Parameter
        /// </summary>
        /// <param name="strChannel">Channel Number of DCA</param>
        /// <returns>Success/Fail</returns>
        public virtual bool SetupParam(string strChannel)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");
            if (strUnit != null)
            {
                if (strUnit != "") Write($":MEASure:{subSystem}:{strParam}:UNITs {strUnit}");
            }
            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        public enum eMeasureDataStatus
        {
            /// <summary>
            /// 
            /// </summary>
            eCorrect,
            /// <summary>
            /// 
            /// </summary>
            eQues,
            /// <summary>
            /// 
            /// </summary>
            eError
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strParam"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        protected virtual bool DoMapParams( string strParam,
                                    Dictionary<string, string> mapMeasuredValues,
                                    Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {

            string strParamStatus;
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[strParam] = strResult;
            SetParamStatus(strParam, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strParam"></param>
        /// <param name="strParamStatus"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        protected bool SetParamStatus( string strParam, string strParamStatus, Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            if (strParamStatus.ToUpper().Contains("COR"))
            {
                mapMeasuredValuesStatus[strParam] = eMeasureDataStatus.eCorrect;
            }
            else if (strParamStatus.ToUpper().Contains("QUES"))
            {
                mapMeasuredValuesStatus[strParam] = eMeasureDataStatus.eQues;
            }
            else
            {
                mapMeasuredValuesStatus[strParam] = eMeasureDataStatus.eError;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public virtual bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");

            // 3rd is to send the query and retrieve the measurement value
            DoMapParams(strParam, mapMeasuredValues, mapMeasuredValuesStatus);
            
            return true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Eyecmd : DCAcmd
    {
        /// <summary>
        /// 
        /// </summary>
        public Eyecmd() : base()
        {
            subSystem = EYE;
        }
    }

    /// <summary>
    /// AOP command class
    /// </summary>
    public class AOPcmd : Eyecmd
    {
        /// <summary>
        /// DCA Measurement Command Name for AOP
        /// </summary>
        public const string AOPparam = "APOWer";

        /// <summary>
        /// Constructor of AOP Command Class
        /// </summary>
        public AOPcmd() : base()
        {
            strParam = AOPparam;
            strUnit = "DBM";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TDEQcmd : RemapChannelcmd
    {
        /// <summary>
        /// 
        /// </summary>
        public double TDECQOffset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double OffsetTDECQMinLimit { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //protected CLinearPieceWiseFit myFit;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<double, double> _mapTDECQLinearFit;
        
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<double, double> mapTDECQPiecewiseLinearFit
        {
            get
            {
                return _mapTDECQLinearFit;
            }
            set
            {
                _mapTDECQLinearFit = value;
                //myFit = null; //set null by default to prevent application using if _mapTDECQLinearFit is invalid
                if (_myConfig != null)
                {
                    if (_myConfig.mapTDECQPiecewiseLinearFit != null)
                    {
                        if (_myConfig.mapTDECQPiecewiseLinearFit.Count >= 2)
                        {
                            //myFit = new CLinearPieceWiseFit(_mapTDECQLinearFit);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            if (this.mapChannels != null)
            {
                //Write($":{this.mapChannels[strChannel]}:OPERAND CHAN{strChannel}");
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {this.mapChannels[strChannel]}");
            }
            else
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR CHAN{strChannel}");
            }

            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMyParam"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        protected override bool DoMapParams(string strMyParam,
            Dictionary<string, string> mapMeasuredValues,
            Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {

            string strParamStatus;
            string strParamRaw = strMyParam + "_RAW";
            string strResult = ExtractParam(subSystem, strMyParam, out strParamStatus);        



            //double myTDECQ = 0.0;
            //if (strParamStatus.ToUpper().Contains("COR"))
            //{
            //    mapMeasuredValues[strParamRaw] = strResult; // e.g. TDECQ_RAW
            //    // adding offset
            //    if (Double.TryParse(strResult, out myTDECQ))
            //    {
            //        if (this.myFit == null)
            //        {
            //            myTDECQ += this.TDECQOffset;
            //        }
            //        else
            //        {
            //            double rawTDECQ = myTDECQ;
            //            myTDECQ = myFit.Interpolate(rawTDECQ);
            //        }

            //        // This is to limit and avoid a very optimistic TDECQ after offset, that will cause our OOMA to be low.
            //        if ((myTDECQ < this.OffsetTDECQMinLimit) && 
            //            ((this.TDECQOffset != 0.0) || (myFit != null)))
            //        {
            //            myTDECQ = this.OffsetTDECQMinLimit;
            //        }

            //        mapMeasuredValues[strMyParam] = myTDECQ.ToString();// e.g. TDECQ
            //    }
            //}
            //else
            //{
            //    if (strParamStatus.ToUpper().Contains("QUES"))
            //    {
            //        mapMeasuredValues[strParamRaw] = "-99999";
            //        mapMeasuredValues[strMyParam] = "-99999";
            //    }
            //    else
            //    {
            //        mapMeasuredValues[strParamRaw] = "-999";
            //        mapMeasuredValues[strMyParam] = "-999";
            //    }
            //}


            SetParamStatus(strMyParam, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        //protected bool ApplyTDECQOffset(string myStrParam, Dictionary<string, string> mapMeasuredValues)
        //{
        //    double myTDECQ = 0.0;
        //    if (!mapMeasuredValues[myStrParam].Contains("-999"))
        //    {
        //        if (Double.TryParse(mapMeasuredValues[myStrParam], out myTDECQ))
        //        {
        //            myTDECQ += this.TDECQOffset;
        //            mapMeasuredValues[myStrParam] = myTDECQ.ToString();
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            // Refactored be more generic supporting any and all channels
            if (this.mapChannels != null)
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {this.mapChannels[strChannel]}");
            }
            else
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR CHAN{strChannel}");
            }

            // 3rd is to send the query and retrieve the measurement value
            DoMapParams(base.strParam, mapMeasuredValues, mapMeasuredValuesStatus);


            //General statistical MIN query  
            string myStrParam = base.strParam + ":MIN";
            DoMapParams(myStrParam, mapMeasuredValues, mapMeasuredValuesStatus);


            myStrParam = base.strParam + ":MAX";
            DoMapParams(myStrParam, mapMeasuredValues, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public TDEQcmd() : base()
        {
            TDECQOffset = 0.0;
            OffsetTDECQMinLimit = 1.0;
            strParam = ScopeConst.TDEQ;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OOMAcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public string UNITS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");
            Write($":MEASure:EYE:OOMA:UNITs " + UNITS);
            return Write($":MEASure:EYE:OOMA");
        }

        /// <summary>
        /// 
        /// </summary>
        public OOMAcmd() : base()
        {
            UNITS = "dBm";
            strParam = ScopeConst.OOMA;
            UNITS = "dBm";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OMAXcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public OMAXcmd() : base()
        {
            strParam = ScopeConst.OMAX;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TTIMEcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public TTIMEcmd() : base()
        {
            strParam = ScopeConst.TTIME;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RemapChannelcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> mapChannels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RemapChannelcmd() : base()
        {
            // By default we will use this mapping, but by design we can override this mapping in the station config by redefining the map channel configuration.
            mapChannels = new Dictionary<string, string>
            {
                ["1A"] = "FUNCTION1",
                ["1B"] = "FUNCTION2"
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CEQcmd : RemapChannelcmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            // apply DEQ function, not source eye waveform
            if (this.mapChannels != null)
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {this.mapChannels[strChannel]}");
            }
            else
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {strChannel}");
            }

            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            // TODO: Refactor be more generic supporting any and all channels
            if (this.mapChannels != null)
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {this.mapChannels[strChannel]}");
            }
            else
            {
                Write($":MEAS:{subSystem}:{base.strParam}:SOUR {strChannel}");
            }

            // 3rd is to send the query and retrieve the measurement value
            string strParamStatus;
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[strParam] = strResult;
            SetParamStatus(strParam, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public CEQcmd() : base()
        {
            strParam = ScopeConst.CEQ;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OERcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> OERFactor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");

            var match = OERFactor.FirstOrDefault(s => s.Contains(strChannel));
            if (match != null)
            {
                clog.Log($"OERFactor: {match}");
                string[] factor = Regex.Replace(match, @"\s", "").Split('='); // Removed all spaces if any
                Write($":MEASure:ERATio:CHAN{strChannel}:OERFactor {factor[1]}");
                Write($":MEASure:ERATio:CHAN{strChannel}:ACFactor ON");
            }
            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        public OERcmd() : base()
        {
            strParam = ScopeConst.OER;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class NMARGINcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public NMARGINcmd() : base()
        {
            strParam = ScopeConst.NMARGIN;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PSERcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public PSERcmd() : base()
        {
            strParam = ScopeConst.PSER;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PTDEQcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public PTDEQcmd() : base()
        {
            strParam = ScopeConst.PTDEQ;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PNMARGINcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public PNMARGINcmd() : base()
        {
            strParam = ScopeConst.PNMARGIN;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class TRANsitionRISingcmd : Eyecmd
    {
        // not measure on TDEQ, 
        // now measure on source eye

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            //  :MEASure: EYE: TTIMe: SOURce1 FUNCtion1
            //  :MEASure: EYE: TTIMe: TRANsition RISing
            //  :MEASure: EYE: TTIMe

            // set up source eye
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:SOUR CHAN{strChannel}");
            // set up for Transition Falling time measurement
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:TRAN RIS");
            return WriteAndCheckCmdStatus($":MEAS:EYE:TTIM");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            // set up source eye
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:SOUR CHAN{strChannel}");
            // set up for Transition Falling time measurement
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:TRAN RIS");

            // 3rd is to send the query and retrieve the measurement value
            string strParamStatus;
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.TRANsitionRISing] = strResult;
            SetParamStatus(ScopeConst.TRANsitionRISing, strParamStatus, mapMeasuredValuesStatus);

            //General statistical MIN query  
            string myStrParam = base.strParam + ":MIN";
            strResult = ExtractParam(subSystem, myStrParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.TRANsitionRISingMin] = strResult;
            SetParamStatus(ScopeConst.TRANsitionRISingMin, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public TRANsitionRISingcmd() : base()
        {
            strParam = "TTIME";
            subSystem = "EYE";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TRANsitionFALLingcmd : Eyecmd
    {
        // not measure on TDEQ, 
        // now measure on source eye

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            // set up source eye
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:SOUR CHAN{strChannel}");
            // set up for Transition Falling time measurement
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:TRAN FALL");
            return WriteAndCheckCmdStatus($":MEAS:EYE:TTIM");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            //:MEASure:EYE:TTIM:TRAN RIS

            // set up source eye
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:SOUR CHAN{strChannel}");
            // set up for Transition Falling time measurement
            WriteAndCheckCmdStatus($":MEAS:EYE:TTIM:TRAN FALL");

            // 3rd is to send the query and retrieve the measurement value
            string strParamStatus;
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.TRANsitionFALLing] = strResult;
            SetParamStatus(ScopeConst.TRANsitionFALLing, strParamStatus, mapMeasuredValuesStatus);

            //General statistical MIN query  
            string myStrParam = base.strParam + ":MIN";
            strResult = ExtractParam(subSystem, myStrParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.TRANsitionFALLingMin] = strResult;
            SetParamStatus(ScopeConst.TRANsitionFALLingMin, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public TRANsitionFALLingcmd() : base()
        {
            strParam = "TTIME";
            subSystem = "EYE";
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class PAM4LINcmd : Eyecmd
    {
        /// <summary>
        ///:MEASure:EYE:PAM:LINearity:DEFinition RLMC94
        ///:MEASure:EYE:PAM:LINearity:DEFinition RLMA120
        ///:MEASure:EYE:PAM:LINearity:DEFinition EYE
        /// </summary>
        public string LINearity_DEFinition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            //:MEASure:EYE:PAM:LINearity:DEFinition RLMC94
            //:MEASure:EYE:PAM:LINearity:DEFinition RLMA120
            //:MEASure:EYE:PAM:LINearity:DEFinition EYE

            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");
            Write($":MEASure:EYE:PAM:LINearity:DEFinition {LINearity_DEFinition}");
            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");
            string strResult = Query($":MEAS:{subSystem}:{strParam}" + ":STATus?");

            string strParamStatus;
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.PAM4LIN] = strResult;
            SetParamStatus(ScopeConst.PAM4LIN, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public PAM4LINcmd() : base()
        {
            LINearity_DEFinition = "RLMA120";
            strParam = "LIN";
            subSystem = "EYE:PAM";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PAM4LINSOURCEcmd : PAM4LINcmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            //:MEASure:EYE:PAM:LINearity:DEFinition RLMC94
            //:MEASure:EYE:PAM:LINearity:DEFinition RLMA120
            //:MEASure:EYE:PAM:LINearity:DEFinition EYE

            Write($":MEAS:{subSystem}:{strParam}:SOUR {mapSource[strChannel]}");
            Write($":MEASure:EYE:PAM:LINearity:DEFinition {LINearity_DEFinition}");
            return Write($":MEAS:{subSystem}:{strParam}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR {mapSource[strChannel]}");
            string strResult = Query($":MEAS:{subSystem}:{strParam}" + ":STATus?");

            string strParamStatus;
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.PAM4LIN] = strResult;
            SetParamStatus(ScopeConst.PAM4LIN, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> mapSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PAM4LINSOURCEcmd() : base()
        {
            LINearity_DEFinition = "RLMA120";
            strParam = "LIN";
            subSystem = "EYE:PAM";

            mapSource = new Dictionary<string, string>
            {
                ["1A"] = "FUNCTION1",
                ["1B"] = "FUNCTION2"
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Tapscmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public string AVERage_ECOunt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TSPacing_TPUI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TAPS_COUNt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MNPRecursors { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> mapProcess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            //:SPRocess5:AVERage:ECOunt 16
            //:SPRocess6:TEQualizer:TSPacing:TPUI 1
            //:SPRocess6:TEQualizer:TAPS:COUNt 5
            //:SPRocess6:TEQualizer:MNPRecursors 3

            //write($"{SPROCESS}:AVERage:ECOunt {AVERage_ECOunt}");
            //write($"{SPROCESS}:TEQualizer:TSPacing {TSPacing_TPUI}");
            //write($"{SPROCESS}:TEQualizer:TAPS:COUNT {TAPS_COUNt}");
            //write($"{SPROCESS}:TEQualizer:MNPRecursors {MNPRecursors}");

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            string strResult = Query($":{mapProcess[strChannel]}:TEQualizer:TAPS?");
            string[] arData = strResult.Split(',');
            
            //for (int i = 0; i < arData.Length; i++)
            //{
            //    mapMeasuredValues[FWConstants.CDRLineTap_PRE + i.ToString()] = arData[i];
            //}
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nFixedPrecursor"></param>
        public Tapscmd(int nFixedPrecursor) : base()
        {
            strParam = ScopeConst.TAPS;

            AVERage_ECOunt = "16";

            TSPacing_TPUI = "1";

            TAPS_COUNt = "5";

            MNPRecursors = nFixedPrecursor.ToString();

            mapProcess = new Dictionary<string, string>
            {
                ["1A"] = "SPROCESS1",
                ["1B"] = "SPROCESS2"
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Tapscmd() : base()
        {
            strParam = ScopeConst.TAPS;

            AVERage_ECOunt = "16";

            TSPacing_TPUI = "1";

            TAPS_COUNt = "10";

            MNPRecursors = "9";

            mapProcess = new Dictionary<string, string>
            {
                ["1A"] = "SPROCESS1",
                ["1B"] = "SPROCESS2"
            };
        }
    }

    #endregion


    #region DCA COMMANDS

    /// <summary>
    /// 
    /// </summary>
    public class AOP_WATTcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write(":MEAS:EYE:APOWer:SOUR CHAN" + strChannel);
            Write(":MEASure:EYE:APOWer");
            return Write(":MEASure:EYE:APOWer:UNITs WATT");
        }

        /// <summary>
        /// 
        /// </summary>
        public AOP_WATTcmd() : base()
        {
            strParam = ScopeConst.AOP_WATT;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class PAM4Levelcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        protected void checkError()
        {
            string strResult = Query($":MEASure:EYE:PAM:LEVel:STATus?");
            if (strResult.IndexOf("INV") >= 0)
            {
                strResult = Query($":MEASure:EYE:PAM:LEVel:STATus:REASon?");
                strResult = Query($":MEASure:EYE:PAM:LEVel:STATus:DETails?");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write("*CLS");
            Write($":MEASure:PAM:AMPLitude:UNITs PERCent");
            //write($":MEASure:EYE:PAM:LEVEL:SOUR CHAN{strChannel}");
            Write($":MEASure:EYE:PAM:LEVEL:SOUR CHAN{strChannel}");
            checkError();

            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel0");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel1");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            //write($":MEASure:EYE:PAM:LEVel");
            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel2");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            //write($":MEASure:EYE:PAM:LEVel");
            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel3");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");


            string strParamStatus;
            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel0");
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL0] = strResult;
            SetParamStatus(ScopeConst.LEVEL0, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel1");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL1] = strResult;
            SetParamStatus(ScopeConst.LEVEL1, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel2");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL2] = strResult;
            SetParamStatus(ScopeConst.LEVEL2, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel3");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL3] = strResult;
            SetParamStatus(ScopeConst.LEVEL3, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public PAM4Levelcmd() : base()
        {
            strParam = "LEVel";
            subSystem = "EYE:PAM";
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class PAM4LevelSOURCEcmd : PAM4Levelcmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write("*CLS");
            Write($":MEASure:PAM:AMPLitude:UNITs PERCent");
            //write($":MEASure:EYE:PAM:LEVEL:SOUR CHAN{strChannel}");
            Write($":MEASure:EYE:PAM:LEVEL:SOUR {mapSource[strChannel]}");
            checkError();

            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel0");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel1");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            //write($":MEASure:EYE:PAM:LEVel");
            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel2");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            //write($":MEASure:EYE:PAM:LEVel");
            Write($":MEASure:EYE:PAM:LEVel:LEVEL LEVel3");
            Write($":MEASure:EYE:PAM:LEVel");
            checkError();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR {mapSource[strChannel]}");


            string strParamStatus;
            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel0");
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL0] = strResult;
            SetParamStatus(ScopeConst.LEVEL0, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel1");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL1] = strResult;
            SetParamStatus(ScopeConst.LEVEL1, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel2");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL2] = strResult;
            SetParamStatus(ScopeConst.LEVEL2, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:LEVel LEVel3");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.LEVEL3] = strResult;
            SetParamStatus(ScopeConst.LEVEL3, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> mapSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PAM4LevelSOURCEcmd() : base()
        {
            strParam = "LEVel";
            subSystem = "EYE:PAM";

            mapSource = new Dictionary<string, string>
            {
                ["1A"] = "FUNCTION1",
                ["1B"] = "FUNCTION2"
            };
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class EyeHeightPAM4cmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public string PROBability { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected void checkError()
        {
            string strResult = Query($":MEASure:EYE:PAM:EHEight:STATus?");
            if (strResult.IndexOf("INV") >= 0)
            {
                strResult = Query($":MEASure:EYE:PAM:EHEight:STATus:REASon?");
                strResult = Query($":MEASure:EYE:PAM:EHEight:STATus:DETails?");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            //// Auto-generated SCPI script from FlexDCA version A.06.40.429
            //// 09/23/2019 16:50:36
            //:MEASure:EYE:PAM:EHEight:DEFine:EOPening PROBability
            //:MEASure:EYE:PAM:EHEight:DEFine:EOPening:PROBability 1.0E-3
            //:MEASure:EYE:PAM:EHEight:EYE EYE0
            //:MEASure:EYE:PAM:EHEight
            //:MEASure:EYE:PAM:EHEight:EYE EYE1
            //:MEASure:EYE:PAM:EHEight
            //:MEASure:EYE:PAM:EHEight:EYE EYE2
            //:MEASure:EYE:PAM:EHEight


            Write("*CLS");
            Write($":MEASure:EYE:PAM:EHEight:DEFine:EOPening PROBability");
            Write($":MEASure:EYE:PAM:EHEight:DEFine:EOPening:PROBability " + PROBability);
            checkError();

            Write($":MEASure:EYE:PAM:EHEight:EYE EYE0");
            Write($":MEASure:EYE:PAM:EHEight");
            checkError();

            Write($":MEASure:EYE:PAM:EHEight:EYE EYE1");
            Write($":MEASure:EYE:PAM:EHEight");
            checkError();

            Write($":MEASure:EYE:PAM:EHEight:EYE EYE2");
            Write($":MEASure:EYE:PAM:EHEight");
            checkError();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public override bool MapParam(string strChannel, Dictionary<string, string> mapMeasuredValues,
                                                         Dictionary<string, eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            Write($":MEAS:{subSystem}:{strParam}:SOUR CHAN{strChannel}");

            string strParamStatus;
            Write($":MEAS:{subSystem}:{strParam}:EYE EYE0");
            string strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.EYH0] = strResult;
            SetParamStatus(ScopeConst.EYH0, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:EYE EYE1");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.EYH1] = strResult;
            SetParamStatus(ScopeConst.EYH1, strParamStatus, mapMeasuredValuesStatus);

            Write($":MEAS:{subSystem}:{strParam}:EYE EYE2");
            strResult = ExtractParam(subSystem, strParam, out strParamStatus);
            mapMeasuredValues[ScopeConst.EYH2] = strResult;
            SetParamStatus(ScopeConst.EYH2, strParamStatus, mapMeasuredValuesStatus);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public EyeHeightPAM4cmd() : base()
        {
            this.PROBability = "1E-3";
            this.strParam = "EHEight";
            this.subSystem = "EYE:PAM";
        }
    }

    

    /// <summary>
    /// 
    /// </summary>
    public class EyeAmpcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public EyeAmpcmd() : base()
        {
            strParam = ScopeConst.EyeAmp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ZeroLevelcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public ZeroLevelcmd() : base()
        {
            strParam = "ZLEV";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OneLevelcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public OneLevelcmd() : base()
        {
            strParam = "OLEV";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Xingcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public Xingcmd() : base()
        {
            strParam = "CROS";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JitterDcdcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public JitterDcdcmd() : base()
        {
            strParam = "DCD";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JitterPpcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write(":MEAS:EYE:JITTer:SOUR CHAN" + strChannel);
            Write(":MEASure:EYE:JITTer:FORMat PP");
            return Write(":MEAS:EYE:JITT");
        }

        /// <summary>
        /// 
        /// </summary>
        public JitterPpcmd() : base()
        {
            strParam = "JITTer";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExtRatioDbcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public ExtRatioDbcmd() : base()
        {
            strParam = "ERAT";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExtRatiocmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public ExtRatiocmd() : base()
        {
            strParam = "ERAT";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Crossingcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public Crossingcmd() : base()
        {
            strParam = ScopeConst.Crossing;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DCDistortioncmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <returns></returns>
        public override bool SetupParam(string strChannel)
        {
            Write(":MEAS:EYE:DCDistortion:SOUR CHAN" + strChannel);
            Write(":MEASure:EYE:DCDistortion");
            return Write(":MEASure:EYE:DCDistortion:FORMat PERCent");
        }

        /// <summary>
        /// 
        /// </summary>
        public DCDistortioncmd() : base()
        {
            strParam = ScopeConst.DCDistortion;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TDECcmd : Eyecmd
    {
        /// <summary>
        /// 
        /// </summary>
        public TDECcmd() : base()
        {
            strParam = ScopeConst.TDEC;
        }
    }


    #endregion
}
