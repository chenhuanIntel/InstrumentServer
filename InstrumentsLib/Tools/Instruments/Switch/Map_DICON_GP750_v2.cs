using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;

namespace InstrumentsLib.Tools.Instruments.Switch
{
    /// <summary>
    /// 
    /// </summary>
    public class MapSwitch_DICON_GP750_v2_Config : MapSwitch_DICON_GP750_Config
    {
        //public int nSlotAddr { get; set; }
        //public Dictionary<string, int> channelMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MapSwitch_DICON_GP750_v2_Config()
        {
            //channelMap = new Dictionary<string, int>();
            buildTargetClassInfo(typeof(Map_DICON_GP750_v2));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Map_DICON_GP750_v2 : InstrumentX, ISwitch
    {
        private MapSwitch_DICON_GP750_v2_Config mySwitchConfig;
        /// <summary>
        /// 
        /// </summary>
        public int nDeviceAddr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nTotalMChannel = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public Map_DICON_GP750_v2(MapSwitch_DICON_GP750_v2_Config config)
            : base(config)
        {
            mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public Map_DICON_GP750_v2(MapSwitch_DICON_GP750_v2_Config config, ProtocolX protocol)
            : base(config, protocol)
        {
            mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            return base.initialize();
        }

        #region ISwitch

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            this._ProtocolX.write("*RST");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        public bool ValidateChannels(string strChannel1, string strChannel2)
        {
            if (!mySwitchConfig.channelMap.ContainsKey(strChannel1))
                return false;

            if (!mySwitchConfig.channelMap.ContainsKey(strChannel2))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strOutputChannel"></param>
        /// <param name="strInputChannel"></param>
        /// <returns></returns>
        public bool IsConnect(string strOutputChannel, string strInputChannel)
        {
            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            //int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            //nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
            //int nPort   = (nInputChannel % nTotalMChannel) + 1;            

            int nOutputChannel = mySwitchConfig.channelMap[strOutputChannel];
            string sOutputChannel_Current = this.query(string.Format("SL{0}:X1 {1}?", mySwitchConfig.nSlotAddr, nOutputChannel));
            sOutputChannel_Current = this.read();
            string[] subString = sOutputChannel_Current.Split(',');
            sOutputChannel_Current = subString[subString.Length - 1];
            int nOutputChannel_Current = 0;
            nOutputChannel_Current = Convert.ToInt32(sOutputChannel_Current);
            if (0 == nOutputChannel_Current || nOutputChannel_Current == nOutputChannel)
                return false;

            //string sPort_Current = this.query(string.Format("SL{0}:M{1}?", mySwitchConfig.nSlotAddr, nDeviceAddr));
            //sPort_Current = sPort_Current.TrimEnd(',');
            //int nPort_Current = 0;
            //nPort_Current = Convert.ToInt32(sPort_Current);
            //if (nPort != nPort_Current)
            //    return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strOutputChannel"></param>
        /// <param name="strInputChannel"></param>
        /// <returns></returns>
        public bool Connect(string strOutputChannel, string strInputChannel)
        {
            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            //if (true == IsConnect(strInputChannel, strOutputChannel))
            //{
            //    return true;
            //}

            int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            int nPort;
            if (nInputChannel <= 16)
            {
                nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
                nPort = (nInputChannel % nTotalMChannel) + 1;
            }
            else if (nInputChannel == 17)
            {
                nDeviceAddr = 6;
                nPort = 1;
            }
            else
                return false;
            int nOutputChannel = mySwitchConfig.channelMap[strOutputChannel];

            //Format: SL<slot index>:M<device index> CHannel {input} <output>
            //Parameters: {input}, The input channel. If the device has only one input channel, this parameter is optional.
            //<output>, The new output channel
            //Return Value: None
            //Description: The M CHannel command establishes an optical connection between input channel input and output channel output of the selected multi-channel switch. Connecting an input channel to output channel 0 will switch that input to the off position.
            //Note: The M CHannel command is the default command for multi-channel switches. The CHannel command token may be omitted.
            //Example: The following command sets the output channel of the first multi-channel switch in slot 1 to channel 31.
            //SL1:M1 31
            //The following command sets input channel 3 of the first multi-channel switch in slot 1 to output channel 16.
            //SL1:M1 3, 16

            string strResult = this.query(string.Format("SL{0}:M{1} {2}", mySwitchConfig.nSlotAddr, nDeviceAddr, nPort));
            Pause(200);

            // If we have two output channels, we need to re-visit the code 
            //To select the device dynamically.

            //Format: SL<slot index>:X<device index> CHannel <input> <output>
            //Parameters: <input>, The input channel
            //<output>, The new output channel
            //Return Value: None
            //Description: The X CHannel command establishes an optical connection between input channel input and output channel output of the selected matrix switch. Connecting an input channel to output channel 0 will switch that input to the off position.
            //Note: The X CHannel command is the default command for matrix switches. The CHannel command token may be omitted.
            //Example: The following command establishes an optical connection between input channel 5 and output channel 8 for the first matrix switch in slot 1.
            //SL1:X1 5 8

            strResult = this.query(string.Format("SL{0}:X1 {1},{2}", mySwitchConfig.nSlotAddr, nOutputChannel, nDeviceAddr));
            Pause(200);

            //IsConnect(strOutputChannel, strInputChannel);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strOutputChannel"></param>
        /// <param name="strInputChannel"></param>
        /// <returns></returns>
        public bool Disconnect(string strOutputChannel, string strInputChannel)
        {
            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
            int nPort = 0;
            int nOutputChannel = 0;

            string strResult = this.query(string.Format("SL{0}:M{1} {2}", mySwitchConfig.nSlotAddr, nDeviceAddr, nPort));
            strResult = this.query(string.Format("SL{0}:X1 {1},{2}", mySwitchConfig.nSlotAddr, nDeviceAddr, nOutputChannel));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        public bool ObtainLock(string strChannel1, string strChannel2)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        public bool ReleaseLock(string strChannel1, string strChannel2)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Slot"></param>
        /// <param name="intInput"></param>
        /// <param name="intOutput"></param>
        /// <returns></returns>
        public bool GetSwitchSetting(int Slot, ref int intInput, ref int intOutput)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

