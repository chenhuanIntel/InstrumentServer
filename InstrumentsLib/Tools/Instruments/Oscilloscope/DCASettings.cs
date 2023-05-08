using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    // data contract must be applied to any class that is intended to be seralized, even in the all parent classes
    // data member must also be applied to any property of any class if the property is intended to be serialed
    [DataContract]
    /// <summary>
    /// 
    /// </summary>
	public class DCASettings
	{
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public const string INIT_SETTING = "INIT_SETTING";
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public const string PAM4_SETTING = "PAM4_SETTING";
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public const string EYE_SETTING = "EYE_SETTING";
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public const string OMA_SETTING = "OMA_SETTING";
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public const string REFTX_SETTING = "REFTX_SETTING";
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int MaskMarginValue { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public int MaskMarginHitsLimit { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string sDCA_MaskFileNameWithPath { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string sDCAConfigFile { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public int EyeAvg { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public int EyePatLen { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public int EyeAcqWavLimVal { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int PAM4AcqPatLimVal { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string EyePointsPerWaveform { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string RINoiseType { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string RINoiseUnits { set; get; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JittPatType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JittPatLen { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JittAcqLimType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JittAcqLimVal { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JittAvg { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary
		public string OscAcqLimType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string OscAcqLimVal { get; set; }

        // we do not want to provide VISA timeout setting as a part of DCA setting; timeout is supposed to be set at VISA protocol
        //public int EyeModeDCA_TimeoutSec { get; set; }
        //public int JitterModeDCA_TimeoutSec { set; get; }
        //public int RinDCA_TimeoutSec { set; get; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public double dCaptureEyePause_s { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public bool bEnableMask { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bACQSmoothAverage { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int ACQSmoothAveNumWaveform { get; set; }
        [DataMember]
        /// <summary>
        /// Unit types
        /// </summary>
        public bool bSetMeasUnitsType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string JitterMeasUnitType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string AmplitudeMeasUnitType { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
		public string PowerUnitType { get; set; }
        [DataMember]
        /// <summary>
        ///  for Tek DSA
        /// </summary>
        public string sDSASetupFile { get; set; } // must have this setup in the DSA PC, not the test PC
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public PAMSetupcmd PAM4setup { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public CRUSetupcmd CRUsetup { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bSetupParam { get; set; }
        [DataMember]
        /// <summary>
        ///  setting for TEQualizer and all related parameters
        /// </summary>
        public TEQualizerSetupcmd TEQsetup { get; set; }
        [DataMember]
        /// <summary>
        /// Flag to Measure Scope with TDECQ Equalizer
        /// </summary>
        public bool bTEQualizer { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// Diplay Mode on Scope
        /// </summary>
        public string sDisplayMode { get; set; }

        /// <summary>
        /// Default Setting for DCASetings
        /// </summary>
        public DCASettings()
		{
			dCaptureEyePause_s = 45.0;
			bEnableMask = true;
			bSetMeasUnitsType = true;
			JitterMeasUnitType = "UINT";
			AmplitudeMeasUnitType = "UAMPlitude";
			PowerUnitType = "DBM";
            RINoiseType = "OMA";
            RINoiseUnits = "DEC";
            EyePointsPerWaveform = "AUTO";
            EyeAcqWavLimVal = 300; 
            PAM4AcqPatLimVal = 3;
            bACQSmoothAverage = false;
            ACQSmoothAveNumWaveform = 16;

            sDCA_MaskFileNameWithPath = @"";
            sDCAConfigFile = "";

            sDSASetupFile = @"C:\Users\lab_spsgtest.AMR\Documents\Keysight\FlexDCA\Setups\Setup_2019 - 03 - 27_POR_WW15.5.setx";

            //PAM4setup = new PAMSetupcmd();

            //CRUsetup = new CRUSetupcmd();

            bSetupParam = true;
            bTEQualizer = true;

            sDisplayMode = "";
        }
	}
}
