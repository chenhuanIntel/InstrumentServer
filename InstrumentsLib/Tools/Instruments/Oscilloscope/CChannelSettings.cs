using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// 
    /// </summary>
	public class CChannelSettings
    {
        /// <summary>
        /// 
        /// </summary>
		public string strChannelName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strImageChannelName { get; set; }

        // public bool bUseWaveVal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strWavelength { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public string strFilterState { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public string strFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strAttenState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strAttenVal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bOnlySampleSelectedParams { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> arSelectedParamsToOnlySample { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bEnableMask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaskFileNameWithPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public CChannelSettings()
        {
            strAttenState = "OFF";
            strAttenVal = "0.0";
            strWavelength = "1.31E-6";

            bOnlySampleSelectedParams = true;
            arSelectedParamsToOnlySample = new List<string>();
            arSelectedParamsToOnlySample.Add("AOP");
            arSelectedParamsToOnlySample.Add("AOP_WATT");

            bEnableMask = true;
            MaskFileNameWithPath = @"c:\Temp\25GbE_100GBASE-PSM4_TP2-NRZ.mskx";
        }
    }
}
