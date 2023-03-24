using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// 
    /// </summary>
	public class DCASettings
	{
        /// <summary>
        /// 
        /// </summary>
        public const string INIT_SETTING = "INIT_SETTING";
        /// <summary>
        /// 
        /// </summary>
        public const string PAM4_SETTING = "PAM4_SETTING";
        /// <summary>
        /// 
        /// </summary>
        public const string EYE_SETTING = "EYE_SETTING";
        /// <summary>
        /// 
        /// </summary>
        public const string OMA_SETTING = "OMA_SETTING";
        /// <summary>
        /// 
        /// </summary>
        public const string REFTX_SETTING = "REFTX_SETTING";

        /// <summary>
        /// 
        /// </summary>
        public int MaskMarginValue { set; get; }

        /// <summary>
        /// 
        /// </summary>
		public int MaskMarginHitsLimit { set; get; }

        /// <summary>
        /// 
        /// </summary>
		public string sDCA_MaskFileNameWithPath { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string sDCAConfigFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public int EyeAvg { set; get; }
        /// <summary>
        /// 
        /// </summary>
		public int EyePatLen { set; get; }
        /// <summary>
        /// 
        /// </summary>
		public int EyeAcqWavLimVal { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int PAM4AcqPatLimVal { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string EyePointsPerWaveform { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string RINoiseType { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string RINoiseUnits { set; get; }

        /// <summary>
        /// 
        /// </summary>
		public string JittPatType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string JittPatLen { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string JittAcqLimType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string JittAcqLimVal { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string JittAvg { get; set; }
        /// <summary>
        /// 
        /// </summary>

		public string OscAcqLimType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string OscAcqLimVal { get; set; }

        // we do not want to provide VISA timeout setting as a part of DCA setting; timeout is supposed to be set at VISA protocol
		//public int EyeModeDCA_TimeoutSec { get; set; }
		//public int JitterModeDCA_TimeoutSec { set; get; }
		//public int RinDCA_TimeoutSec { set; get; }

        /// <summary>
        /// 
        /// </summary>
		public double dCaptureEyePause_s { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public bool bEnableMask { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool bACQSmoothAverage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ACQSmoothAveNumWaveform { get; set; }

        /// <summary>
        /// Unit types
        /// </summary>
        public bool bSetMeasUnitsType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string JitterMeasUnitType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string AmplitudeMeasUnitType { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string PowerUnitType { get; set; }

        /// <summary>
        ///  for Tek DSA
        /// </summary>
        public string sDSASetupFile { get; set; } // must have this setup in the DSA PC, not the test PC

        /// <summary>
        /// 
        /// </summary>
        public PAMSetupcmd PAM4setup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CRUSetupcmd CRUsetup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bSetupParam { get; set; }

        /// <summary>
        ///  setting for TEQualizer and all related parameters
        /// </summary>
        public TEQualizerSetupcmd TEQsetup { get; set; }

        /// <summary>
        /// Flag to Measure Scope with TDECQ Equalizer
        /// </summary>
        public bool bTEQualizer { get; set; }

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
