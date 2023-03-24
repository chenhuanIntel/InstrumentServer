using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// 
    /// </summary>
    public class CChannelConfigParallel
    {
        /// <summary>
        /// 
        /// </summary>
        public CChannelSettings channelSetting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> arParams { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool bApplySwitchConn { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int nChannel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public CChannelConfigParallel()
        {
            bApplySwitchConn = true;
            channelSetting = new CChannelSettings();
            arParams = new List<string>();
        }
    }
}
