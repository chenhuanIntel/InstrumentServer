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
    public class MapSwitch_DICON_GP750_Config : InstrumentXConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int nSlotAddr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, int> channelMap { get; set; }

        /// <summary>
        /// Resets the device when we initialize
        /// </summary>
        public bool bResetDeviceOnInit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MapSwitch_DICON_GP750_Config()
        {
            channelMap = new Dictionary<string, int>();
            buildTargetClassInfo(typeof(Map_DICON_GP750));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Map_DICON_GP750 : InstrumentX, ISwitch
    {
        private MapSwitch_DICON_GP750_Config _mySwitchConfig;
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
        public Map_DICON_GP750(MapSwitch_DICON_GP750_Config config)
            : base(config)
        {
            _mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public Map_DICON_GP750(MapSwitch_DICON_GP750_Config config, ProtocolX protocol)
            : base(config, protocol)
        {
            _mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            base.initialize();

            string strResult = this._ProtocolX.query("*IDN?");
            Log(this.ToString() + " : query(*IDN?) = " + strResult);


            if (_mySwitchConfig.bResetDeviceOnInit)
            {
                this._ProtocolX.write("*RST");
            }

            return true;
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
            if (!_mySwitchConfig.channelMap.ContainsKey(strChannel1))
                return false;

            if (!_mySwitchConfig.channelMap.ContainsKey(strChannel2))
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
            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            //int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            //nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
            //int nPort   = (nInputChannel % nTotalMChannel) + 1;            

            int nOutputChannel = _mySwitchConfig.channelMap[strOutputChannel];
            string sOutputChannel_Current = this.query(string.Format("SL{0}:X1 {1}?", _mySwitchConfig.nSlotAddr, nOutputChannel));
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
            if (!ValidateChannels(strInputChannel, strOutputChannel) ||
                true == IsConnect(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nInputChannel = _mySwitchConfig.channelMap[strInputChannel];
            nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
            int nPort = (nInputChannel % nTotalMChannel) + 1;
            int nOutputChannel = _mySwitchConfig.channelMap[strOutputChannel];

            string strResult = this.query(string.Format("SL{0}:M{1} {2}", _mySwitchConfig.nSlotAddr, nDeviceAddr, nPort));
            Pause(200);
            strResult = this.query(string.Format("SL{0}:X1 {1},{2}", _mySwitchConfig.nSlotAddr, nDeviceAddr, nOutputChannel));
            Pause(200);
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
            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nInputChannel = _mySwitchConfig.channelMap[strInputChannel];
            nDeviceAddr = (nInputChannel / nTotalMChannel) + 1;
            int nPort = 0;
            int nOutputChannel = 0;

            string strResult = this.query(string.Format("SL{0}:M{1} {2}", _mySwitchConfig.nSlotAddr, nDeviceAddr, nPort));
            strResult = this.query(string.Format("SL{0}:X1 {1},{2}", _mySwitchConfig.nSlotAddr, nDeviceAddr, nOutputChannel));

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

