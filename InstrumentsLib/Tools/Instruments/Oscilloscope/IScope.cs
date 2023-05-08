using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// Interface of Scope
    /// </summary>
    public interface IScope
    {
        // add this parameter for test time analysis
        /// <summary>
        /// 
        /// </summary>
        bool bSeperateAcqTEQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int nDUTidx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string strTestName { get; set; }

        /// <summary>
        /// Property of ScopeConfig for Read Only
        /// </summary>
        ScopeConfig objScopeConfig { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string getID();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        void saveScreen(string filename);


        /// <summary>
        /// Function to save waveform data
        /// </summary>
        /// <param name="filename">Filename to save as</param>
        /// <param name="mapOptions">Dictionary of Options to save</param>
        /// <returns>true: success; false: fail</returns>
        bool SaveWaveformData(string filename, Dictionary<string, string> mapOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="strChannelName"></param>
        /// <param name="bSelectMask"></param>
        void saveScreenMultiChannel(string filename, string strChannelName, bool bSelectMask = false);

        /// <summary>
        /// Get lock with RF switch
        /// </summary>
        /// <returns>instrument name of RF switch to switch trigger signal of DCA</returns>
        string sRFSwitchNameForTrigger { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        bool captureMeasureJitter(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <returns></returns>
        bool setupRINmeas(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        bool measureRIN(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        bool captureEyeMeasureMaskMargin(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="arMapMaskValues"></param>
        /// <returns></returns>
        bool captureEyeMeasureMaskMarginParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues, out List<Dictionary<string, string>> arMapMaskValues);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        bool measureSCOPEmode(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <returns></returns>
        bool measureSCOPEmodeParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues);

        /// <summary>
        /// Trigger PaM4 eye measurement from Scope
        /// </summary>
        /// <param name="channelsGroupConfig">Channel Group Setting</param>
        /// <param name="arMapMeasuredValues">Measurment Result</param>
        /// <param name="arMapMeasuredValuesStatus">Status of Measurement Result</param>
        /// <param name="bMeasureOnly">Flag for only do measurement without Setup Scope</param>
        /// <param name="bTurnOffAllDispFirst">Flag to turn off all Display and only enable needed Scope Channel</param>
        /// <returns>True: Success; False: Fail</returns>
        bool measurePAM4Parallel(CChannelsGroupParallel channelsGroupConfig,
                                    out List<Dictionary<string, string>> arMapMeasuredValues,
                                    out List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus,
                                    bool bMeasureOnly = false, bool bTurnOffAllDispFirst = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="bMeasureOnly"></param>
        /// <returns></returns>
        List<byte[]>
            acqWaveform(CChannelsGroupParallel channelsGroupConfig, bool bMeasureOnly = false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternWavesAllMeasureGroups"></param>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="arMapMeasuredValuesStatus"></param>
        /// <returns></returns>
        Task<Tuple<List<string>, List<string>>> ExecuteOffloadComputeAsync(List<byte[]> patternWavesAllMeasureGroups,
                                                       CChannelsGroupParallel channelsGroupConfig,
                                                       List<Dictionary<string, string>> arMapMeasuredValues,
                                                       List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAsettings"></param>
        void setDCAsettings(DCASettings DCAsettings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKeyDCAsettings"></param>
        void setDCAsettings(string strKeyDCAsettings);

        /// <summary>
        /// 
        /// </summary>
        void SoftReset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool initialize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        bool VerticalCal(List<string> ChanList, bool bForce);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        bool DarkCal(List<string> ChanList, bool bForce);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSetupFile"></param>
        /// <returns></returns>
        bool LoadSetupFile(string strSetupFile);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAChan"></param>
        /// <param name="MeasureParams"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="AcqDelay"></param>
        void SimpleMeasurement(string DCAChan, List<string> MeasureParams, out Dictionary<string, string> arMapMeasuredValues, int AcqDelay = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iChannel"></param>
        void TapsReCalc(int iChannel);

        /// <summary>
        /// Measure AOP
        /// </summary>
        /// <param name="DCAChan">DCA channel name</param>
        /// <param name="AcqDelay">delay setting acquisition</param>
        double MeasureAOP(string DCAChan, int AcqDelay = 0);
    }
}
