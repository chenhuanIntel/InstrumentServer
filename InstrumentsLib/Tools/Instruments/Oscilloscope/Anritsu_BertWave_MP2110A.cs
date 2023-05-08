using InstrumentsLib.Tools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// Config Class of Anritsu_BertWave_MP2110A
    /// </summary>
    public class Anritsu_BertWave_MP2110AConfig : Keysight_DCAM_N1092Config
    {
        /// <summary>
        /// Noise Coefficient Setting for TDECQ measurement
        /// </summary>
        public double dTDECQNoiseCoefficient { get; set; }

        /// <summary>
        /// Constructor of Anritsu_BertWave_MP2110A Config
        /// Declare all default value here
        /// </summary>
        public Anritsu_BertWave_MP2110AConfig()
        {
            dTDECQNoiseCoefficient = 0;
        }
    }

    /// <summary>
    /// Driver of Anritsu BertWave (MP2110A)
    /// </summary>
    public class Anritsu_BertWave_MP2110A : Keysight_DCAM_N1092
    {
        private Anritsu_BertWave_MP2110AConfig _MyConfig;
        //private CLinearPieceWiseFit _myFit;

        /// <summary>
        /// Constructor of Anritsu BertWave
        /// </summary>
        /// <param name="config">Config of Anritsu BertWave</param>
        public Anritsu_BertWave_MP2110A(Anritsu_BertWave_MP2110AConfig config) : base(config)
        {
            _MyConfig = config;
        }

        /// <summary>
        /// Constructor of Anritsu BertWave
        /// </summary>
        /// <param name="config">Config of Anritsu BertWave</param>
        /// <param name="protocol">Protocol Object for Communicate with Instrument</param>
        public Anritsu_BertWave_MP2110A(Anritsu_BertWave_MP2110AConfig config, ProtocolX protocol) : base(config, protocol)
        {
            _MyConfig = config;
        }

        /// <summary>
        /// If sDCAConfigFile of DCAsettings is .dcascppi file, just save DCAsettings content
        /// If it is running in dynamic seq, file need to get from server which is preuploaded by HW team
        /// </summary>
        /// <param name="DCAsettings">DCA setting declared in StationConfig file</param>
        public override void setDCAsettings(DCASettings DCAsettings)
        {
            _DCAsettings = DCAsettings;
            string[] StringSplit = new string[] { "::" };
            List<string> arFilePath = DCAsettings.sDCAConfigFile.Split(StringSplit, StringSplitOptions.None).ToList();
            string sFilePath = arFilePath[0];
            if (!sFilePath.ToLower().EndsWith(".dcascpi"))
            {
                throw new HardwareErrorException($"Error: FilePath = {_DCAsettings.sDCAConfigFile} is unknown file type!", _MyConfig.strName);
            }
        }

        /// <summary>
        /// Initialize of Anritsu BertWave Scope by checking info, reset and clear Scope
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            if (_MyConfig.bSimulation)
            {
                return true;
            }

            //Query instruments
            string strResult = query("*IDN?");

            //Now parse the ID of the DCA frame
            string[] ar = strResult.Split(',');
            _myConfig.strDCA_ID = ar[2];

            write("*CLS");
            write("*RST");
            _ProtocolX.clearBuffer();

            SetupTDECQ();

            return true;
        }

        /// <summary>
        /// Setup TDECQ Offset
        /// </summary>
        private void SetupTDECQ()
        {
            if (_MyConfig.mapTDECQPiecewiseLinearFit != null)
            {
                if (_MyConfig.mapTDECQPiecewiseLinearFit.Count >= 2)
                {
                    //_myFit = new CLinearPieceWiseFit(_MyConfig.mapTDECQPiecewiseLinearFit);
                }
            }
        }

        /// <summary>
        /// Measure PAM4 with scpi command in .dcascpi file to setup DCA
        /// </summary>
        /// <param name="channelsGroupConfig">Channel Group Setting Object</param>
        /// <param name="arMapMeasuredValues">Output measurement values</param>
        /// <param name="arMapMeasuredValuesStatus">Output measurement status</param>
        /// <param name="bMeasureOnly">Measure Eye without setup DCA</param>
        /// <param name="bTurnOffAllDispFirst">Flag to turn off all Display and only enable needed Scope Channel</param>
        /// <returns>true: success; false: fail</returns>
        public override bool measurePAM4Parallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues, out List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus, bool bMeasureOnly = false, bool bTurnOffAllDispFirst = false)
        {
            try
            {
                clog.MarkStart(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
                arMapMeasuredValues = new List<Dictionary<string, string>>();
                arMapMeasuredValuesStatus = new List<Dictionary<string, DCAcmd.eMeasureDataStatus>>();

                CChannelSettings channelConfig;
                Dictionary<string, string> mapMeasuredValues;
                Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus;

                if (!bMeasureOnly) // if bMeasureOnly is false, then do the following DCA setup steps
                {
                    try
                    {
                        clog.MarkStart(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);

                        //Setup Scope for prepare measurement
                        DCASetting();

                        if (bTurnOffAllDispFirst) write($":SCOPe:INPut:ALL OFF");

                        //Set factor to scope and add measurment params to scope
                        SetupChannelsConfig(channelsGroupConfig.arChannels);
                    }
                    finally
                    {
                        clog.MarkEnd(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);
                    }
                }
                else
                {
                    ClearScreen();
                }

                for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                {
                    mapMeasuredValues = new Dictionary<string, string>();
                    arMapMeasuredValues.Add(mapMeasuredValues);

                    mapMeasuredValuesStatus = new Dictionary<string, DCAcmd.eMeasureDataStatus>();
                    arMapMeasuredValuesStatus.Add(mapMeasuredValuesStatus);
                }

                // Autoscale -> Acquisition -> Calculate Taps
                AutoScale();

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
                }
                catch (Exception ex)
                {
                    clog.Error(ex, "Error at ExtractMeasurment", nDUTidx);
                    throw;
                }
                finally
                {
                    clog.MarkEnd(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                }
                return true;
            }
            finally
            {
                clog.MarkEnd(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            }
        }

        /// <summary>
        /// Fetch Measurement from Scope
        /// </summary>
        /// <param name="channelConfig">Scope Channel Setting Object</param>
        /// <param name="arParams">Measurement Parameter to Fetch</param>
        /// <param name="mapMeasuredValues">output Measurement Values from Scope</param>
        /// <param name="mapMeasuredValuesStatus">output Measurment Param Status</param>
        /// <returns>Always true: Complete Fetch Measurement result</returns>
        protected override bool ExtractPAM4Param(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues, Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            string sParam;

            for (int i = 0; i < arParams.Count; i++)
            {
                switch (arParams[i])
                {
                    case ScopeConst.TAPS:
                        string sResponse = QueryMeasurement(channelConfig.strChannelName, MeasurementParamMapping(arParams[i], true)).Trim().Trim('\"');
                        clog.Log($"sMeasurement of {ScopeConst.TAPS} = {sResponse}", nDUTidx);
                        string[] arData = sResponse.Split(',');

                        //for (int j = 0; j < arData.Length; j++)
                        //{
                        //    mapMeasuredValues[$"{FWConstants.CDRLineTap_PRE}{j}"] = arData[j];
                        //    clog.Log($"sMeasurement of {FWConstants.CDRLineTap_PRE}{j} = {arData[j]}", nDUTidx);
                        //}
                        break;

                    case ScopeConst.OERFACTOR:
                        mapMeasuredValues[ScopeConst.OERFACTOR] = QueryMeasurement(channelConfig.strChannelName, MeasurementParamMapping(arParams[i], true));
                        break;

                    case ScopeConst.PAM4LEVEL:
                    case ScopeConst.PAM4LEVELSOURCE:
                        for (int j = 0; j < 4; j++)
                        {
                            sParam = $"{ScopeConst.PAM4LEVEL}{j}";
                            FetchMeasurement(channelConfig, sParam, mapMeasuredValues, mapMeasuredValuesStatus);
                        }
                        double lvl0 = Convert.ToDouble(mapMeasuredValues[$"{ScopeConst.PAM4LEVEL}{0}"]);
                        double lvl3 = Convert.ToDouble(mapMeasuredValues[$"{ScopeConst.PAM4LEVEL}{3}"]);
                        for (int j = 0; j < 4; j++)
                        {
                            double lvlCurrent = Convert.ToDouble(mapMeasuredValues[$"{ScopeConst.PAM4LEVEL}{j}"]);
                            mapMeasuredValues[$"{ScopeConst.PAM4LEVEL}{j}"] = (((lvlCurrent - lvl0) / (lvl3 - lvl0)) * 100).ToString("0.##");
                            clog.Log($"sMeasurement of {ScopeConst.PAM4LEVEL}{j} = {mapMeasuredValues[$"{ScopeConst.PAM4LEVEL}{j}"]}", nDUTidx);
                        }
                        break;

                    case ScopeConst.EyeHeightPAM4:
                        for (int j = 0; j < 3; j++)
                        {
                            sParam = $"EyeHeight{j}";
                            FetchMeasurement(channelConfig, sParam, mapMeasuredValues, mapMeasuredValuesStatus);
                        }
                        break;

                    case ScopeConst.PTDEQ:
                    case ScopeConst.PNMARGIN:
                        for (int j = 0; j < 3; j++)
                        {
                            FetchMeasurement(channelConfig, arParams[i], mapMeasuredValues, mapMeasuredValuesStatus, j.ToString());
                        }
                        break;

                    default:
                        clog.Log($"Query sMeasurement of {arParams[i]}", nDUTidx);
                        FetchMeasurement(channelConfig, arParams[i], mapMeasuredValues, mapMeasuredValuesStatus);
                        if (arParams[i] == ScopeConst.OER)
                        {
                            clog.Log($"Query sMeasurement of {ScopeConst.OERFACTOR}", nDUTidx);
                            mapMeasuredValues[ScopeConst.OERFACTOR] = QueryMeasurement(channelConfig.strChannelName, MeasurementParamMapping(ScopeConst.OERFACTOR, true));
                        }
                        break;
                }
            }

            OMACalculation(arParams, MapParamName(ScopeConst.AOP), MapParamName(ScopeConst.OER), MapParamName(ScopeConst.OOMA), ref mapMeasuredValues);

            return true;
        }

        /// <summary>
        /// Send Fetch Measurement command to scope
        /// </summary>
        /// <param name="channelConfig">Channel Scope Setting Object</param>
        /// <param name="sParam">Measurement Parameter to fetch</param>
        /// <param name="mapMeasuredValues">Output Measurement Value</param>
        /// <param name="mapMeasuredValuesStatus">Ouput Measurement Status</param>
        /// <param name="sIdx">Index of measurement Parameter, optional</param>
        private void FetchMeasurement(CChannelSettings channelConfig, string sParam, Dictionary<string, string> mapMeasuredValues, Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus, string sIdx = "")
        {
            string sResponse;
            string sMeasure = QueryMeasurement(channelConfig.strChannelName, $"FETCh:{MeasurementParamMapping(sParam, true)}{sIdx}").Trim();
            clog.Log($"sMeasurement of {sParam} = {sMeasure}", nDUTidx);
            sResponse = QueryMeasurementStatus(channelConfig.strChannelName, $"FETCh:{MeasurementParamMapping(sParam, true)}{sIdx}:CURRent").Trim();
            if (sResponse == "\"\"")
            {
                mapMeasuredValues[MapParamName(sParam)] = ValueConversion(sParam, sMeasure.Split(',')[0]);
                mapMeasuredValues[MapParamName($"{sParam}:MIN")] = ValueConversion(sParam, sMeasure.Split(',')[3]);
                mapMeasuredValues[MapParamName($"{sParam}:MAX")] = ValueConversion(sParam, sMeasure.Split(',')[4]);
                mapMeasuredValuesStatus[MapParamName(sParam)] = DCAcmd.eMeasureDataStatus.eCorrect;
            }
            else if (!sMeasure.Contains("N/A"))
            {
                clog.Log($"Fail Query Reason of {sParam} is {sResponse}.", nDUTidx);
                mapMeasuredValues[MapParamName(sParam)] = ValueConversion(sParam, sMeasure.Split(',')[0]);
                mapMeasuredValues[MapParamName($"{sParam}:MIN")] = ValueConversion(sParam, sMeasure.Split(',')[3]);
                mapMeasuredValues[MapParamName($"{sParam}:MAX")] = ValueConversion(sParam, sMeasure.Split(',')[4]);
                mapMeasuredValuesStatus[MapParamName(sParam)] = DCAcmd.eMeasureDataStatus.eError;
            }
            else
            {
                clog.Log($"Fail Query Reason of {sParam} is {sResponse}.", nDUTidx);
                mapMeasuredValues[MapParamName(sParam)] = "-999";
                mapMeasuredValues[MapParamName($"{sParam}:MIN")] = "-999";
                mapMeasuredValues[MapParamName($"{sParam}:MAX")] = "-999";
                mapMeasuredValuesStatus[MapParamName(sParam)] = DCAcmd.eMeasureDataStatus.eError;
            }
        }

        /// <summary>
        /// Apply TDECQ Factor as offset to Measured TDECQ
        /// </summary>
        /// <param name="TDECQ">Measured TDECQ</param>
        /// <returns>TDECQ that applied offset</returns>
        private string ApplyTDECQFactor(string TDECQ)
        {
            clog.Log($"Measured TDECQ = {TDECQ}", nDUTidx);
            double dTDECQ = Convert.ToDouble(TDECQ);
            //if (_myFit is null)
            //{
            //    dTDECQ += _MyConfig.TDECQOffset;
            //}
            //else
            //{
            //    //dTDECQ = _myFit.Interpolate(dTDECQ);
            //}

            if ((dTDECQ < _MyConfig.OffsetTDECQMinLimit) && ((_MyConfig.TDECQOffset != 0)))
            {
                dTDECQ = _MyConfig.OffsetTDECQMinLimit;
            }

            clog.Log($"After applied Offset, TDECQ = {dTDECQ}", nDUTidx);
            return dTDECQ.ToString();
        }

        /// <summary>
        /// Save Scope Screen capture of required Channel
        /// </summary>
        /// <param name="filename">File Name to save</param>
        /// <param name="strImageChannelName">Scope Channel to capture</param>
        /// <param name="bSelectMask">Whether want to show Mask file</param>
        public override void saveScreenMultiChannel(string filename, string strImageChannelName, bool bSelectMask = false)
        {
            clog.MarkStart(strTestName, "SaveScreenMultiChannel", nDUTidx);
            try
            {
                if (bSelectMask)
                {
                    throw new NotImplementedException();
                }

                SaveScreen(filename, strImageChannelName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fail SaveScreenMultiChannel: filename = {filename}, strImageChannelName = {strImageChannelName}, bSelectMask = {bSelectMask}", ex);
            }
            finally
            {
                clog.MarkEnd(strTestName, "SaveScreenMultiChannel", nDUTidx);
            }
        }

        /// <summary>
        /// Scope front panel screen capture
        /// </summary>
        /// <param name="strDCAFileName">Image file Name to save screen capture</param>
        public override void saveScreen(string strDCAFileName)
        {
            SaveScreen(strDCAFileName);
        }

        /// <summary>
        /// Take Screen Capture of Scope
        /// </summary>
        /// <param name="strDCAFileName">Image File name to save</param>
        /// <param name="strChannel">Which Channel to Capture</param>
        private void SaveScreen(string strDCAFileName, string strChannel = "")
        {
            clog.MarkStart(strTestName, "SaveScreen", nDUTidx);
            try
            {
                if (_config.bSimulation)
                {
                    return;
                }

                string strRawDCAFolderPath, strRawDCAFileFullName, strRawDCAFilePath = "";

                strRawDCAFolderPath = CAppendDir.appendDir(Path.GetDirectoryName(strDCAFileName), "Raw");
                strRawDCAFileFullName = Path.Combine(strRawDCAFolderPath, Path.GetFileName(strDCAFileName));
                strRawDCAFilePath = Path.GetDirectoryName(strRawDCAFileFullName);

                //strlocalFilePath = Path.Combine(_myAppConfig.LocalHSOutputDir,
                //    strDCAFileName.Substring(_myAppConfig.DCA_RemoteImagePath.Length));

                if (!System.IO.Directory.Exists(strRawDCAFilePath))
                {
                    System.IO.Directory.CreateDirectory(strRawDCAFilePath);
                }

                if (strChannel != "") strChannel = $":CH{strChannel}";
                string strTEQualizer = _DCAsettings.bTEQualizer ? "ON" : "OFF";
                write($":SCOPe:CONFigure:MEASure:PAM:TEQualizer:DISPlay{strChannel} {strTEQualizer}");

                write(":SCOPe:PRINt:GRATicule OFF");

                write(":SCOPe:PRINt:INVerse ON");

                write($":SCOPe:EYEPulse:PRINt:COPY{strChannel}?");
                Pause(1000);
                List<byte> binaryFile = _ProtocolX.readTillEnd();

                using (BinaryWriter writer = new BinaryWriter(File.Open(strRawDCAFileFullName, FileMode.Create)))
                {
                    writer.Write(binaryFile.ToArray());
                }

                //if (this._myConfig.bShrinkEyeFile)
                //{
                //    Utility.CImageConverter.ChangeJPGImageQuality(strRawDCAFileFullName, strlocalFilePath, _myConfig.CompressLevel);
                //}
                //else
                //{
                //    // Will overwrite if the destination file already exists.
                //    File.Copy(strRawDCAFileFullName, strlocalFilePath, true);
                //}
            }
            finally
            {
                clog.MarkEnd(strTestName, "SaveScreen", nDUTidx);
            }
        }


        /// <summary>
        /// Function to save waveform data
        /// </summary>
        /// <param name="filename">Filename to save as</param>
        /// <param name="mapOptions">Dictionary of Options to save</param>
        /// <returns>true: success; false: fail</returns>
        public override bool SaveWaveformData(string filename, Dictionary<string, string> mapOptions)
        {
            throw new NotImplementedException();

            //return true;
        }

        /// <summary>
        /// Clear Front Panel Screen
        /// </summary>
        public override void ClearScreen()
        {
            try
            {
                clog.MarkStart(strTestName, "ClearDCAScreen", nDUTidx);
                write(":DISPlay:WINDow:GRAPhics:CLEar:ALL");
            }
            finally
            {
                clog.MarkEnd(strTestName, "ClearDCAScreen", nDUTidx);
            }
        }

        /// <summary>
        /// Send command to Instrument, Check Error if it is query command, which end with ?
        /// </summary>
        /// <param name="cmd">Command to send to instrument</param>
        /// <returns>True: Success; False: Fail with error</returns>
        public override bool write(string cmd)
        {
            bool bResult = _ProtocolX.write(cmd);
            clog.Log($"{this}:Write => {cmd}", nDUTidx);
            if (cmd.EndsWith("?")) return true;
            string res;
            if (CheckError(out res))
            {
                clog.Log($"Error@{this}:Write => {res}", nDUTidx);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check Error Message of Scope
        /// </summary>
        /// <param name="res">output error message</param>
        /// <returns>True: Error; False: No Error</returns>
        protected override bool CheckError(out string res)
        {
            string ret = query(":SYSTem:ERRor?").Trim();
            clog.Log($"query(:SYSTem:ERRor?) = {ret}", nDUTidx);
            if (ret.Split(',')[0].Trim() != "0")
            {
                write("*CLS");
                throw new HardwareErrorException($"{this}::Check Error: {ret}", _MyConfig.strName);
            }
            res = ret.Split(',')[1].Replace("\"", "").Trim();
            return false;
        }

        /// <summary>
        /// Check Error Message of Scope
        /// </summary>
        private void CheckError()
        {
            clog.Log($"{this}::Check Error()", nDUTidx);
            string ret = query(":SYSTem:ERRor?").Trim();
            clog.Log($"{this}::Check Error = {ret}", nDUTidx);
            if (ret.Split(',')[0].Trim() != "0")
            {
                throw new HardwareErrorException($"{this}::Check Error: {ret}", _MyConfig.strName);
            }
        }

        /// <summary>
        /// Do Autoscale, Acquisition and Calculate Tap
        /// </summary>
        private void AutoScale()
        {
            DateTime dtStart;
            string ret = "";
            string sCmd;
            try
            {
                clog.MarkStart(strTestName, clog.TimeKey.AutoScale, nDUTidx);
                sCmd = ":SCOPe:SAMPling:STATus:ALL RUN,AUTOscale";
                sCmd += _DCAsettings.bTEQualizer ? ",CALCulate" : "";
                write(sCmd);
                clog.Log($"AutoScale: Write => {sCmd}", nDUTidx);

                try
                {
                    // wait for acquisition limit to reach
                    dtStart = DateTime.Now;
                    clog.MarkStart(strTestName, "ScopeSampling", nDUTidx);
                    while ((DateTime.Now - dtStart).TotalSeconds < _myConfig.dMaxBusyTimeoutSecs)
                    {
                        ret = query(":SCOPe:SAMPling:STATus:ALL?").Trim();
                        if (ret == "HOLD") break;
                        else if (ret != "RUN") throw new HardwareErrorException($"{this}::AutoScale: Found unknown return from \":SCOPe:SAMPling:STATus:ALL?\" = {ret}", _MyConfig.strName);
                    }
                    if (ret != "HOLD") throw new HardwareErrorException($"{this}::AutoScale: Fail to complete in time ({_myConfig.dMaxBusyTimeoutSecs}secs)", _MyConfig.strName);
                }
                finally
                {
                    clog.MarkEnd(strTestName, "ScopeSampling", nDUTidx);
                }

                clog.Log("AutoScale::Check Error", nDUTidx);
                CheckError();                

                if (_DCAsettings.bTEQualizer)
                {
                    try
                    {
                        clog.MarkStart(strTestName, "TDEQCalculate", nDUTidx);

                        dtStart = DateTime.Now;
                        write(":SCOPe:CONFigure:MEASure:PAM:TEQualizer:CALCulate:RESult:ALL?");
                        while ((DateTime.Now - dtStart).TotalSeconds < 10)
                        {
                            try
                            {
                                ret = read().Trim();
                                clog.Log($":SCOPe:CONFigure:MEASure:PAM:TEQualizer:CALCulate:RESult:ALL? = {ret}", nDUTidx);
                                if (ret.Contains("Fail") || ret.Contains("Pass")) break;
                            }
                            catch { }
                        }
                        clog.Log($"ret = {ret}", nDUTidx);

                        if (ret.Contains("Fail") || !ret.Contains("Pass"))
                        {
                            throw new HardwareErrorException($"{this}::TEQualizer:CALCulate: Fail to Calculate.", _MyConfig.strName);
                        }
                    }
                    finally
                    {
                        clog.MarkEnd(strTestName, "TDEQCalculate", nDUTidx);
                    }
                }
            }
            finally
            {
                clog.MarkEnd(strTestName, clog.TimeKey.AutoScale, nDUTidx);
            }
        }

        /// <summary>
        /// Setup Channel Scope factor and add measurement param to scope
        /// </summary>
        /// <param name="arChannels">Channel Scope Setting object</param>
        private void SetupChannelsConfig(List<CChannelConfigParallel> arChannels)
        {
            string sResponse;

            //Clear all Measurement Parameters
            if (_DCAsettings.bSetupParam) write($":SCOPe:CONFigure:MEASure:DISPlay:ADELete:ALL");

            //Apply ATTEN (i.e. station calibration)
            foreach (var channel in arChannels)
            {
                write($":SCOPe:INPut:CH{channel.channelSetting.strChannelName} ON");
                if (!channel.channelSetting.strAttenState.Contains("ON")) channel.channelSetting.strAttenVal = "0";
                write($":INPut:ATTenuation:CH{channel.channelSetting.strChannelName} {channel.channelSetting.strAttenVal}");

                // Check whether ATTEN Successfully applied
                sResponse = query($":INPut:ATTenuation:CH{channel.channelSetting.strChannelName}?");
                if (Math.Abs(Convert.ToDouble(sResponse) - Convert.ToDouble(channel.channelSetting.strAttenVal)) > 0.02)
                {
                    throw new HardwareErrorException($"{this}::SetupChannelsConfig: Fail to set Attenuation", _MyConfig.strName);
                }

                // if bSetupParam is true, then clear setting in .setx file and SetupParam by SW
                // otherwise use settings in *.setx file (default)
                if (_DCAsettings.bSetupParam)
                {
                    for (int i = 0; i < arChannels[0].arParams.Count; i++)
                    {
                        if (arChannels[0].arParams[i] == ScopeConst.OER)
                        {
                            for (int j = 0; j < _MyConfig.OERFactor.Count; j++)
                            {
                                write($":SCOPe:CONFigure:EXRCorrection:CH{_MyConfig.arChannels[j]} 1");
                                write($":SCOPe:CONFigure:EXRCorrection:FACTor:CH{_MyConfig.arChannels[j]} {_MyConfig.OERFactor[j]}");
                            }
                        }
                        if (arChannels[0].arParams[i] != ScopeConst.TAPS)
                        {
                            AddMeasurementParam(channel.channelSetting.strChannelName, arChannels[0].arParams[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add Measurement Param to Scope
        /// </summary>
        /// <param name="sChan">Channel Number of Scope</param>
        /// <param name="sParamName">Parameter Name to add to display</param>
        private void AddMeasurementParam(string sChan, string sParamName)
        {
            if (sParamName == ScopeConst.TAPS) return;
            try
            {
                write($":SCOPe:CONFigure:MEASure:DISPlay:ADD CH{sChan.ToUpper()},{MeasurementParamMapping(sParamName, false, true)}");
            }
            catch { }
            switch (sParamName)
            {
                case ScopeConst.TDEQ:
                    string sCmd = $":SCOPe:CONFigure:MEASure:PAM:TDECQ:NCOefficient {_MyConfig.dTDECQNoiseCoefficient.ToString("0.00")}";
                    write(sCmd);
                    clog.Log($"Done Sent Command: {sCmd}", nDUTidx);
                    break;
            }
        }

        /// <summary>
        /// Fetch Measurment Value from Scope
        /// </summary>
        /// <param name="sChan">Channel Number to Query</param>
        /// <param name="sParamName">Measurement Parameter to query</param>
        /// <returns></returns>
        private string QueryMeasurement(string sChan, string sParamName)
        {
            clog.Log($"QueryMeasurement of :SCOPe:{sParamName}:CH{sChan.ToUpper()}?", nDUTidx);
            return query($":SCOPe:{sParamName}:CH{sChan.ToUpper()}?");
        }

        /// <summary>
        /// Fetch Measurment status from Scope
        /// </summary>
        /// <param name="sChan">Channel Number to Query</param>
        /// <param name="sParamName">Measurement Parameter to query</param>
        /// <returns></returns>
        private string QueryMeasurementStatus(string sChan, string sParamName)
        {
            return query($":SCOPe:{sParamName}:REASon:CH{sChan.ToUpper()}?");
        }

        /// <summary>
        /// Measurement Parameter to SCPI command conversion
        /// </summary>
        /// <param name="MeasParam">Measurement Parameter</param>
        /// <param name="bQuery">Flag to indicate is for Query value or for add Display</param>
        /// <param name="bDisplay">Flag whether command is for display</param>
        /// <returns>SCPI command</returns>
        private string MeasurementParamMapping(string MeasParam, bool bQuery, bool bDisplay = false)
        {
            string sSCPI = "";
            bool bTEQualizer = _DCAsettings.bTEQualizer && !bDisplay;
            switch (MeasParam)
            {
                case ScopeConst.AOP:
                case ScopeConst.APOWer:
                    if (bQuery) sSCPI += "AMPLitude:";
                    sSCPI += "AVEPower:DBM";
                    break;
                case ScopeConst.AOP_WATT:
                    if (bQuery) sSCPI += "AMPLitude:";
                    sSCPI += "AVEPower:MW";
                    break;
                case ScopeConst.ZeroLevel:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVel:ZERO";
                    break;
                case ScopeConst.OneLevel:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVel:ONE";
                    break;
                case ScopeConst.EyeAmp:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "EYEAmplitude";
                    break;
                case ScopeConst.Crossing:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "CROSsing";
                    break;
                case ScopeConst.ExtRatio:
                case ScopeConst.ExtRatioDb:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "EXTRatio";
                    break;
                case ScopeConst.OMA:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "OMA:MW";
                    break;
                case ScopeConst.OMA_dBm:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "OMA:DBM";
                    break;
                case ScopeConst.OMAX:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "OMAXp";
                    break;
                case ScopeConst.TDEC:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "TDEC";
                    break;
                case ScopeConst.CEQ:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "CEQ";
                    break;
                case ScopeConst.PAM4LEVEL:
                case ScopeConst.PAM4LEVELSOURCE:
                    if (bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support query");
                    sSCPI += "LEVEL";
                    break;
                case ScopeConst.LEVEL0:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVEL0";
                    break;
                case ScopeConst.LEVEL1:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVEL1";
                    break;
                case ScopeConst.LEVEL2:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVEL2";
                    break;
                case ScopeConst.LEVEL3:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LEVEL3";
                    break;
                case ScopeConst.EyeHeightPAM4:
                    if (bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support query");
                    sSCPI += "EYEHeight";
                    break;
                case ScopeConst.EYH0:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "EYE0:HEIGht";
                    break;
                case ScopeConst.EYH1:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "EYE1:HEIGht";
                    break;
                case ScopeConst.EYH2:
                    if (!bQuery) throw new NotSupportedException($"ERROR: {MeasParam} not support non query");
                    sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "EYE2:HEIGht";
                    break;
                case ScopeConst.TDEQ:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "TDECQ";
                    break;
                case ScopeConst.OOMA:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "OOMA:DBM";
                    break;
                case ScopeConst.OER:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "OER";
                    break;
                case ScopeConst.TAPS:
                    if (!bQuery) throw new NotSupportedException($"Error: {MeasParam} is not supported for Display.");
                    sSCPI += "CONFigure:MEASure:PAM:TEQualizer:TAPS";
                    break;
                case ScopeConst.PAM4LIN:
                case ScopeConst.PAM4LINSOURCE:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "LINearity";
                    break;
                case ScopeConst.NMARGIN:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "NMARgin";
                    break;
                case ScopeConst.PTDEQ:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "PTDeq";
                    if (bQuery) sSCPI += ":EYE";
                    break;
                case ScopeConst.PNMARGIN:
                    if (bQuery) sSCPI += "AMPLitude:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "PNMargin";
                    if (bQuery) sSCPI += ":EYE";
                    break;
                case ScopeConst.JitterPp:
                    if (bQuery) sSCPI += "TIME:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "JITTer:PPeak";
                    break;
                case ScopeConst.TRANsitionRISing:
                    if (bQuery)
                    {
                        sSCPI += "TIME:";
                        if (bTEQualizer) sSCPI += "TEQualizer:";
                        sSCPI += "TTIMe:RISE";
                    }
                    else sSCPI += "TTIMe:RISEFALL";
                    break;
                case ScopeConst.TRANsitionFALLing:
                    if (bQuery)
                    {
                        sSCPI += "TIME:";
                        if (bTEQualizer) sSCPI += "TEQualizer:";
                        sSCPI += "TTIMe:FALL";
                    }
                    else sSCPI += "TTIMe:RISEFALL";
                    break;
                case ScopeConst.Xing:
                    if (bQuery) sSCPI += "TIME:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "CROSsing";
                    break;
                case ScopeConst.DCDistortion:
                    if (bQuery) sSCPI += "TIME:";
                    if (bTEQualizer) sSCPI += "TEQualizer:";
                    sSCPI += "DCD";
                    break;
                case ScopeConst.JitterDcd:
                    if (bQuery) sSCPI += "TIME:AJITter:";
                    sSCPI += "DCD";
                    break;
                case ScopeConst.OERFACTOR:
                    if (bQuery) sSCPI += "CONFigure:EXRCorrection:FACTor";
                    break;
                case ScopeConst.Rise9010:
                case ScopeConst.Fall6040:
                case ScopeConst.Fall8020:
                case ScopeConst.Fall9010:
                case ScopeConst.Period:
                case ScopeConst.PSER:
                case ScopeConst.Rise6040:
                case ScopeConst.Rise8020:
                case ScopeConst.TAPS_FIXED_PRECURSER:
                case ScopeConst.TDEQ_RAW:
                case ScopeConst.TTIME:
                default:
                    throw new NotSupportedException($"ERROR: MeasParam = {MeasParam} is not Supported on Anritsu BertWave MP2110A.");
            }
            return sSCPI;
        }

        /// <summary>
        /// Key of Measurement Dictionary conversion
        /// </summary>
        /// <param name="sParam">Meaurement Parameter Name</param>
        /// <returns>Name to be add to Key of Measurement Dictionary</returns>
        private string MapParamName(string sParam)
        {
            switch (sParam)
            {
                case ScopeConst.AOP:
                    return ScopeConst.APOWer;
                case "TRANsitionFALLing:MIN":
                    return ScopeConst.TRANsitionFALLingMin;
                case "TRANsitionRISing:MIN":
                    return ScopeConst.TRANsitionRISingMin;
                case ScopeConst.PAM4LINSOURCE:
                    return ScopeConst.PAM4LIN;
                default:
                    return sParam;
            }
        }


        /// <summary>
        /// Measure AOP
        /// </summary>
        /// <param name="DCAChan">DCA channel name</param>
        /// <param name="AcqDelay">delay setting acquisition</param>
        public override double MeasureAOP(string DCAChan, int AcqDelay = 0)
        {
            string sMeasure;
            write($":SCOPe:CONFigure:MEASure:DISPlay:ADELete:ALL");
            clog.Log($"Remove all DCA measurement settings.", nDUTidx);
            AddMeasurementParam(DCAChan, "AOP");
            clog.Log($"Add AOP measurement.", nDUTidx);
            
            //Auto scale
            write(":SCOPe:DISPlay:WINDow:AUTOscale BOTH");
            string ret = "";
            DateTime dtStart = DateTime.Now;
            while ((DateTime.Now - dtStart).TotalSeconds < 15)
            {
                try
                {

                    ret = queryNoRetry("*OPC?").Trim();
                    clog.Log($"*OPC? = {ret}", nDUTidx);
                    if (ret == "1") break;
                }
                catch { }
            }
            clog.Log($"Auto scale done.", nDUTidx);
            write($":SCOPe:SAMPling:STATus:CH{DCAChan} RUN");
            Pause(AcqDelay);
            write($":SCOPe:SAMPling:STATus:CH{DCAChan} HOLD");
            clog.Log($"Acqusition done.", nDUTidx);
            sMeasure = QueryMeasurement(DCAChan, $"FETCh:{MeasurementParamMapping("AOP", true)}").Trim();
            clog.Log($"sMeasurement of AOP = {sMeasure}", nDUTidx);
            double dPowerLevel;
            double.TryParse( sMeasure.Split(',')[0], out dPowerLevel);
            return dPowerLevel;
        }

        /// <summary>
        /// simple measurement for AOP
        /// </summary>
        /// <param name="DCAChan">DCA channel</param>
        /// <param name="MeasureParams">paramemter list</param>
        /// <param name="arMapMeasuredValues">measurement result</param>
        /// <param name="AcqDelay">delay, not used</param>
        public override void SimpleMeasurement(string DCAChan, List<string> MeasureParams, out Dictionary<string, string> arMapMeasuredValues, int AcqDelay = 0)
        {
            arMapMeasuredValues = new Dictionary<string, string>();
            string sMeasure;
            write($":SCOPe:CONFigure:MEASure:DISPlay:ADELete:ALL");
            clog.Log($"Remove all DCA measurement settings.", nDUTidx);
            foreach (string Param in MeasureParams)
            {
                switch (Param)
                {
                    case ScopeConst.AOP:
                        AddMeasurementParam(DCAChan, Param);
                        clog.Log($"Add {Param} measurement.", nDUTidx);
                        break;
                    default:
                        throw new Exception($"{Param} is unknown Measurement Parameter");
                }
            }
            //Auto scale
            write(":SCOPe:DISPlay:WINDow:AUTOscale BOTH");
            string ret = "";
            DateTime dtStart = DateTime.Now;
            while ((DateTime.Now - dtStart).TotalSeconds < 15)
            {
                try
                {
                    
                    ret = queryNoRetry("*OPC?").Trim();
                    clog.Log($"*OPC? = {ret}", nDUTidx);
                    if (ret == "1") break;
                }
                catch { }
            }
            clog.Log($"Auto scale done.", nDUTidx);
            write($":SCOPe:SAMPling:STATus:CH{DCAChan} RUN");
            Pause(AcqDelay);
            write($":SCOPe:SAMPling:STATus:CH{DCAChan} HOLD");
            clog.Log($"Acqusition done.", nDUTidx);
            foreach (string Param in MeasureParams)
            {
                sMeasure = QueryMeasurement(DCAChan, $"FETCh:{MeasurementParamMapping(Param, true)}").Trim();
                clog.Log($"sMeasurement of {Param} = {sMeasure}", nDUTidx);
                arMapMeasuredValues[Param] = ValueConversion(Param, sMeasure.Split(',')[0]);
                arMapMeasuredValues[$"{Param}:MIN"] = ValueConversion(Param, sMeasure.Split(',')[3]);
                arMapMeasuredValues[$"{Param}:MAX"] = ValueConversion(Param, sMeasure.Split(',')[4]);
            }
        }

        /// <summary>
        /// Measurement Value Conversion
        /// </summary>
        /// <param name="sParam">Measurement parameter name</param>
        /// <param name="sValue">Measurement Value for Conversion</param>
        /// <returns>Converted Measurment Value</returns>
        private string ValueConversion(string sParam, string sValue)
        {
            switch (sParam)
            {
                case ScopeConst.TRANsitionFALLing:
                case ScopeConst.TRANsitionRISing:
                    return $"{sValue}E-12";
                case ScopeConst.TDEQ:
                    return ApplyTDECQFactor(sValue);
                default:
                    return sValue;
            }
        }

    }
}
