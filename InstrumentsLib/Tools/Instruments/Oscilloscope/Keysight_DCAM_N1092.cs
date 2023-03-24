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
    /// Config Class of Keysight_DCAM_N1092
    /// </summary>
    public class Keysight_DCAM_N1092Config : DCA_A86100CFlex400GConfig
    {
        /// <summary>
        /// Constructor of Keysight_DCAM_N1092 Config
        /// </summary>
        public Keysight_DCAM_N1092Config()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Keysight_DCAM_N1092 : DCA_A86100C_Flex400G
    {
        private Keysight_DCAM_N1092Config _MyConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public Keysight_DCAM_N1092(Keysight_DCAM_N1092Config config)
            : base(config)
        {
            _MyConfig = config;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public Keysight_DCAM_N1092(Keysight_DCAM_N1092Config config, ProtocolX protocol)
            : base(config, protocol)
        {
            _MyConfig = config;
        }

        /// <summary>
        /// If sDCAConfigFile of DCAsettings is .setx file, use legacy DCA_A86100C_Flex_400G
        /// If sDCAConfigFile of DCAsettings is .dcascppi file, just save DCAsettings content
        /// If it is running in dynamic seq, file need to get from server which is preuploaded by HW team
        /// </summary>
        /// <param name="DCAsettings">DCA setting declared in StationConfig file</param>
        public override void setDCAsettings(DCASettings DCAsettings)
        {
            if (DCAsettings.sDCAConfigFile.ToLower().EndsWith(".setx"))
            {
                base.setDCAsettings(DCAsettings);
                return;
            }

            _DCAsettings = DCAsettings;
            string[] StringSplit = new string[] { "::" };
            List<string> arFilePath = DCAsettings.sDCAConfigFile.Split(StringSplit, StringSplitOptions.None).ToList();
            string sFilePath = arFilePath[0];
            if (!sFilePath.ToLower().EndsWith(".dcascpi"))
            {
                throw new HardwareErrorException($"Error: FilePath = {_DCAsettings.sDCAConfigFile} is unknown file type!", _MyConfig.strName);
            }

            //if (StationHardware.Instance().myConfig.myAppConfig.bUseDynamicSeq)
            //{
            //    string rootPath = StationHardware.Instance().myConfig.myAppConfig.strHardwareFilesLocation;

            //    //Split to get file name
            //    string[] sFile = sFilePath.Split('\\');

            //    try
            //    {
            //        //Change File Path to standardize Path of Dynamic Seq
            //        sFilePath = Path.Combine(rootPath, sFile.Last());
            //        if (arFilePath.Count > 1) sFilePath += $"::{arFilePath[1]}";
            //        _DCAsettings.sDCAConfigFile = sFilePath;

            //        //Download file from Server
            //        StationHardware.Instance().TesterManager.DownloadSupportFile(rootPath, sFile.Last().Replace("." + sFile.Last().Split('.').Last(), ""));
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception($"sFile.Last = {sFile.Last()}; sFilePath = {sFilePath}", ex);
            //    }
            //}
        }

        /// <inheritdoc/>
        public override bool measurePAM4Parallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues, out List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus, bool bMeasureOnly = false, bool bTurnOffAllDispFirst = false)
        {
            bool bAcqLimit = false;
            int iAcqLimit = 0;
            if (_DCAsettings.sDCAConfigFile.ToLower().EndsWith(".setx"))
            {
                return base.measurePAM4Parallel(channelsGroupConfig, out arMapMeasuredValues, out arMapMeasuredValuesStatus, bMeasureOnly, bTurnOffAllDispFirst);
            }

            clog.MarkStart(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            string res = "";
            arMapMeasuredValues = new List<Dictionary<string, string>>();
            arMapMeasuredValuesStatus = new List<Dictionary<string, DCAcmd.eMeasureDataStatus>>();

            CChannelSettings channelConfig;
            Dictionary<string, string> mapMeasuredValues;
            Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus;

            if (bMeasureOnly == false) // if bMeasureOnly==false, then do the following DCA setup steps
            {
                clog.MarkStart(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);

                DCASetting();

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

                bAcqLimit = query(":LTESt:ACQuire:STATe?").Trim() == "1" ? true : false;
                if (bAcqLimit) iAcqLimit = Convert.ToInt32(query(":LTESt:ACQuire:CTYPe:PATTerns?"));

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
            }
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

            if (!bAcqLimit || (bAcqLimit && iAcqLimit > 1))
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

            clog.MarkEnd(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            return true;
        }

        private string _CurrentDCAConfigFile;
        private string _PreviousDCAConfigFile;
        private string _sSkipStep = "";
        private bool _bSwitchProfile;
        private Dictionary<string, Dictionary<string, List<string>>> _dictSCPI;

        /// <summary>
        /// Setup DCA with dcaspci file
        /// </summary>
        protected void DCASetting()
        {
            try
            {
                _CurrentDCAConfigFile = _DCAsettings.sDCAConfigFile;
                List<string> lstCMD = ReadDCASCPIFile(_CurrentDCAConfigFile);
                clog.Log($"Switched Profile = {_PreviousDCAConfigFile != _CurrentDCAConfigFile}", nDUTidx);
                if (_PreviousDCAConfigFile != _CurrentDCAConfigFile) SwitchedProfile();
                clog.Log($"Previous DCA Profile: {_PreviousDCAConfigFile} ... Current DCA Profile{_CurrentDCAConfigFile}", nDUTidx);
                foreach (string sCMD in lstCMD)
                {
                    if (_DCAsettings.CRUsetup != null)
                    {
                        ProcessCMD(sCMD, _DCAsettings.CRUsetup.SLOT);
                    }
                    else
                    {
                        ProcessCMD(sCMD);
                    }
                }
            }
            finally
            {
                _PreviousDCAConfigFile = _CurrentDCAConfigFile;
            }
        }

        private List<string> ReadDCASCPIFile(string FilePath)
        {
            string sProfile = "";
            string[] StringSplit = new string[] { "::" };
            string[] arPathInfo = FilePath.Split(StringSplit, StringSplitOptions.None);
            string sFilePath = arPathInfo[0].Trim();

            if (arPathInfo.Length > 1) sProfile = arPathInfo[1].Trim();

            if (_dictSCPI is null) _dictSCPI = new Dictionary<string, Dictionary<string, List<string>>>();
            if (!_dictSCPI.ContainsKey(sFilePath))
            {
                _dictSCPI.Add(sFilePath, ReadDCASCPIProfilesInFile(sFilePath));
            }

            Dictionary<string, List<string>> dictSCPI = _dictSCPI[sFilePath];
            if (!dictSCPI.ContainsKey(sProfile)) throw new HardwareErrorException($"Error: FilePath = {FilePath} is not Found!", _MyConfig.strName);
            return dictSCPI[sProfile];
        }

        /// <summary>
        /// Set flag to inform Profile is switched
        /// </summary>
        public void SwitchedProfile()
        {
            _bSwitchProfile = true;
        }

        private void CheckAddCMD(string sLine, ref List<string> lstCMD)
        {
            if (sLine.StartsWith("WRITE:"))
            {
                if (sLine.Contains("?"))
                {
                    throw new HardwareErrorException($"Error: Wrong command found in DCASCPI file, WRITE command should not has ?, Command Line = {sLine}", _MyConfig.strName);
                }
            }
            lstCMD.Add(sLine);
        }

        /// <summary>
        /// Read DCA SCPI CMD in .dcascpi File
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ReadDCASCPIProfilesInFile(string FilePath)
        {
            string sLine;
            string sFileProfile = "";
            Dictionary<string, List<string>> dictDCASCPI = new Dictionary<string, List<string>>();

            if (!File.Exists(FilePath)) throw new HardwareErrorException($"Error: FilePath = {FilePath} is not Found!", _MyConfig.strName);

            List<string> lstCMD = new List<string>();
            using (StreamReader Reader = new StreamReader(FilePath))
            {
                while (!Reader.EndOfStream)
                {
                    sLine = Reader.ReadLine().Trim();
                    if (!(sLine.StartsWith("//") || sLine == ""))
                    {
                        if (sLine.StartsWith("PROFILE:"))
                        {
                            if (sFileProfile != "")
                            {
                                dictDCASCPI.Add(sFileProfile, lstCMD.DeepCopy());
                                lstCMD.Clear();
                            }
                            sFileProfile = sLine.Split(':')[1].Trim();
                        }
                        else
                        {
                            CheckAddCMD(sLine, ref lstCMD);
                        }
                    }
                }

                dictDCASCPI.Add(sFileProfile, lstCMD.DeepCopy());
            }
            return dictDCASCPI;
        }

        /// <summary>
        /// Parse SCPI command with Predefined Keyword and send SCPI to DCA
        /// </summary>
        /// <param name="strCMD">SCPI command with Predefined Keyword</param>
        /// <param name="Slot">CRU Slot number</param>
        public void ProcessCMD(string strCMD, string Slot = "")
        {
            string sRet = "";
            string sExpectRet = "";
            string sSkipStep = "";
            double dTimeout_s = 0;
            string fullCMD = strCMD;
            string[] arCondition;
            string[] arCMD;

            arCondition = strCMD.Split('>');
            strCMD = arCondition[0];

            arCMD = strCMD.Split(':');
            if (strCMD.Contains(":"))
            {
                strCMD = strCMD.Replace(arCMD[0] + ":", "").Trim();
            }
            else
            {
                strCMD = strCMD.Replace(arCMD[0], "").Trim();
            }
            strCMD = string.Format(strCMD, Slot);

            if (_sSkipStep != "")
            {
                if (arCMD[0].ToUpper() == "STEP")
                {
                    if (arCMD[1].ToUpper() == _sSkipStep)
                    {
                        clog.Log($"Found Step ({_sSkipStep})", nDUTidx);
                        _sSkipStep = "";
                    }
                }
                return;
            }

            foreach (string sCMD in arCondition)
            {
                if (sCMD.ToUpper().Contains("IFCRU"))
                {
                    if (!CRUCheck(sCMD.Split(':')[1].Trim(), Slot)) return;
                }
            }
            switch (arCMD[0].ToUpper())
            {
                case "BUSY":
                    clog.Log($"Busy({strCMD})", nDUTidx);
                    if (arCMD.Length > 1) Busy(Convert.ToDouble(strCMD));
                    else Busy();
                    break;

                case "QUERY":
                    foreach (string sCMD in arCondition)
                    {
                        if (sCMD.ToUpper().Contains("TIMEOUT"))
                        {
                            dTimeout_s = Convert.ToDouble(sCMD.Split(':')[1].Trim());
                        }
                        else if (sCMD.ToUpper().Contains("EXPECT"))
                        {
                            sExpectRet = sCMD.Split(':')[1].Trim();
                        }
                        else if (sCMD.ToUpper().Contains("SAMEPROFILESKIP"))
                        {
                            if (_bSwitchProfile)
                            {
                                _bSwitchProfile = false;
                                return; //Don't check, just proceed to next command
                            }
                            sSkipStep = sCMD.Split(':')[1].Trim().ToUpper();
                        }
                    }

                    DateTime startTime = DateTime.Now;
                    do
                    {
                        clog.Log($"Query({strCMD})", nDUTidx);
                        sRet = query(strCMD).Trim();
                        clog.Log($"Query({strCMD}) = {sRet}", nDUTidx);

                        if (sExpectRet != "")
                        {
                            if (sExpectRet == sRet)
                            {
                                if (sSkipStep != "")
                                {
                                    _sSkipStep = sSkipStep;
                                    clog.Log($"Going to SKIP to Step ({sSkipStep})", nDUTidx);
                                }
                                break;
                            }
                        }
                        else break;

                        if (dTimeout_s > 0)
                        {
                            Pause(500, false);
                        }
                        else break;
                    } while ((DateTime.Now - startTime).TotalSeconds < dTimeout_s);

                    if (dTimeout_s > 0 && (DateTime.Now - startTime).TotalSeconds > dTimeout_s)
                    {
                        if (sSkipStep == "") throw new HardwareErrorException($"Error: Timeout to wait for Expected Feedback From Query({strCMD}) = {sRet} spend {(DateTime.Now - startTime).TotalSeconds}s, Timeout = {dTimeout_s}s", _MyConfig.strName);
                    }

                    if (sExpectRet != "" && sExpectRet != sRet)
                    {
                        if (sSkipStep == "") throw new HardwareErrorException($"Error: Fail to Get Expected Feedback From Query({strCMD}) = {sRet}, Expected = {sExpectRet}", _MyConfig.strName);
                    }
                    break;

                case "WRITE":
                    clog.Log($"Write({strCMD})", nDUTidx);
                    sRet = write(strCMD).ToString();
                    clog.Log($"Write({strCMD}) = {sRet}", nDUTidx);
                    break;

                case "SLEEP":
                    clog.Log($"SLEEP({strCMD})", nDUTidx);
                    Pause(Convert.ToInt32(Convert.ToDouble(strCMD) * 1000));
                    return;

                case "STEP":
                    //for state Step name to skip
                    return;

                case "SAMEPROFILESKIP":
                    if (_bSwitchProfile)
                    {
                        _bSwitchProfile = false;
                        return; //Don't check, just proceed to next command
                    }
                    clog.Log($"SAMEPROFILESKIP({strCMD})", nDUTidx);
                    _sSkipStep = strCMD;
                    return;

                default:
                    throw new HardwareErrorException($"Error: Unregconize CMD = {fullCMD}", _MyConfig.strName);
            }

            //if (CheckError(out sRet))
            //{

            //    if (StationHardware.Instance().myConfig.myAppConfig.bEnablePauseAtException)
            //    {
            //        frmMsgBox cMsgBox = new frmMsgBox();
            //        cMsgBox.Text = "DUT#" + (nDUTidx + 1);
            //        cMsgBox.sMessage = $"Error prompt on DCA: CMD = {fullCMD}, message = {sRet}";
            //        cMsgBox.TopMost = true;
            //        cMsgBox.ShowDialog();
            //    }

            //    throw new HardwareErrorException($"Error: Error prompt on DCA, CMD = {fullCMD}, message = {sRet}", _MyConfig.strName);
            //}
        }

        private bool CRUCheck(string sCMD, string CRUSlot)
        {
            return (sCMD == CRUSlot);
        }

    }
}
