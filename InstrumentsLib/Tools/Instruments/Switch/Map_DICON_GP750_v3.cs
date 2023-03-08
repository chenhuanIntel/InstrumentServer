using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;
using Utility;

namespace InstrumentsLib.Tools.Instruments.Switch
{
    /// <summary>
    /// 
    /// </summary>
    public class MapSwitch_DICON_GP750_v3_Config : MapSwitch_DICON_GP750_Config
    {
        //public int nSlotAddr { get; set; }
        //public Dictionary<string, int> channelMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MapSwitch_DICON_GP750_v3_Config()
        {
            //channelMap = new Dictionary<string, int>();
            buildTargetClassInfo(typeof(Map_DICON_GP750_v3));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Map_DICON_GP750_v3 : InstrumentX, ISwitch
    {
        private MapSwitch_DICON_GP750_v3_Config mySwitchConfig;
        /// <summary>
        /// 
        /// </summary>
        public int nMDeviceAddr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nTotalMChannel = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public Map_DICON_GP750_v3(MapSwitch_DICON_GP750_v3_Config config)
            : base(config)
        {
            mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public Map_DICON_GP750_v3(MapSwitch_DICON_GP750_v3_Config config, ProtocolX protocol)
            : base(config, protocol)
        {
            mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string query(string cmd)
        {
            return _ProtocolX.query(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override double queryDouble(string cmd)
        {
            return _ProtocolX.queryDouble(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override int queryInt32(string cmd)
        {
            return _ProtocolX.queryInt32(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool write(string cmd)
        {
            return _ProtocolX.write(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            lock (_ProtocolX)
            {
                this.query("*IDN?");
            }
            return base.initialize();
        }

        #region ISwitch

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            lock (_ProtocolX)
            {
                write("*RST");
            }
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
        public bool IsMultiChannelSwitchConnect(string strOutputChannel, string strInputChannel)
        {
            string sPort_Current = string.Empty;
            Log("Map_DICON_GP750_v3.IsMultiChannelSwitchConnect({0}, {1})", strOutputChannel, strInputChannel);
            try
            {
                if (this._config.bSimulation)
                    return true;

                if (!ValidateChannels(strInputChannel, strOutputChannel))
                {
                    return false;
                }

                int nLogicalInputChannel = mySwitchConfig.channelMap[strInputChannel] - 1;
                nMDeviceAddr = (nLogicalInputChannel / nTotalMChannel) + 1;
                int nMPort = (nLogicalInputChannel % nTotalMChannel) + 1;
                int nX1MatrixDeviceAddr = mySwitchConfig.channelMap[strOutputChannel];

                if (nMDeviceAddr <= 4)
                {
                    lock (_ProtocolX)
                    {
                        sPort_Current = this.query(string.Format("*CLS;SL{0}:M{1}?", mySwitchConfig.nSlotAddr, nMDeviceAddr));
                    }
                    string[] arResults = sPort_Current.Split(',');
                    int nPort_Current = 0;
                    nPort_Current = Convert.ToInt32(arResults[arResults.Length - 1]);
                    if (nPort_Current != nMPort)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                clog.Error(ex, $"Error Found: IsMultiChannelSwitchConnect return sPort_Current = {sPort_Current}, setting: nSlotAddr = {mySwitchConfig.nSlotAddr}, nX1MatrixDeviceAddr = {mySwitchConfig.channelMap[strOutputChannel]}");
                throw new HardwareErrorException(ex.Message, "Map_DICON_GP750_v3");
            }
        }

        /// <summary>
        /// This function checks if matrix switch X1 is in correct position for provided 
        /// input and output channels (aliases defined in station configuration file)
        /// </summary>
        /// <param name="strOutputChannel">alias for output channel from station configuration file</param>
        /// <param name="strInputChannel">alias for input channel from station configuration file</param>
        /// <returns></returns>
        public bool IsMatrixSwitchConnect(string strOutputChannel, string strInputChannel)
        {
            string sOutputChannel_Current = string.Empty;
            Log("Map_DICON_GP750_v3.IsMatrixSwitchConnect({0}, {1})", strOutputChannel, strInputChannel);
            try
            {
                if (this._config.bSimulation)
                    return true;

                if (!ValidateChannels(strInputChannel, strOutputChannel))
                {
                    return false;
                }

                int nLogicalInputChannel = mySwitchConfig.channelMap[strInputChannel] - 1;
                // Outputs #17 and #18 are special in a way that they are connected directly to X1 matrix and they do not have 1x4 M switch
                // Outut #17 fits the logic below for calculation of the nMDeviceAddr, but for #18 we need to replace "17" with "20" in order to keep calculations correct
                // please see schematic of the GP750 matrix switch
                if (nLogicalInputChannel == 17)
                {
                    nLogicalInputChannel = 20;
                }
                nMDeviceAddr = (nLogicalInputChannel / nTotalMChannel) + 1;
                int nMPort = (nLogicalInputChannel % nTotalMChannel) + 1;
                int nX1MatrixDeviceAddr = mySwitchConfig.channelMap[strOutputChannel];

                lock (_ProtocolX)
                {
                    sOutputChannel_Current = this.query(string.Format("*CLS;SL{0}:X1 {1}?", mySwitchConfig.nSlotAddr, nX1MatrixDeviceAddr));
                }
                string[] subString = sOutputChannel_Current.Split(',');
                sOutputChannel_Current = subString[subString.Length - 1];
                int nOutputChannel_Current = 0;
                nOutputChannel_Current = Convert.ToInt32(sOutputChannel_Current);
                if (nOutputChannel_Current != nMDeviceAddr)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                clog.Error(ex, $"Error Found: IsMatrixSwitchConnect return sOutputChannel_Current = {sOutputChannel_Current}, setting: nSlotAddr = {mySwitchConfig.nSlotAddr}, nX1MatrixDeviceAddr = {mySwitchConfig.channelMap[strOutputChannel]}");
                throw new HardwareErrorException(ex.Message, "Map_DICON_GP750_v3");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strOutputChannel"></param>
        /// <param name="strInputChannel"></param>
        /// <returns></returns>
        public bool IsConnect(string strOutputChannel, string strInputChannel)
        {
            Log("Map_DICON_GP750_v3.IsConnect({0}, {1})", strOutputChannel, strInputChannel);

            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nLogicalInputChannel = mySwitchConfig.channelMap[strInputChannel] - 1;
            nMDeviceAddr = (nLogicalInputChannel / nTotalMChannel) + 1;
            int nMPort = (nLogicalInputChannel % nTotalMChannel) + 1;
            int nX1MatrixDeviceAddr = mySwitchConfig.channelMap[strOutputChannel];

            string sOutputChannel_Current = string.Empty;
            try
            {
                lock (_ProtocolX)
                {
                    sOutputChannel_Current = this.query(string.Format("*CLS;SL{0}:X1 {1}?", mySwitchConfig.nSlotAddr, nX1MatrixDeviceAddr));
                }
                string[] subString = sOutputChannel_Current.Split(',');
                sOutputChannel_Current = subString[subString.Length - 1];
                int nOutputChannel_Current = 0;
                nOutputChannel_Current = Convert.ToInt32(sOutputChannel_Current);
                if (nOutputChannel_Current != nMDeviceAddr)
                    return false;
            }
            catch (Exception ex)
            {
                clog.Error(ex, $"Error Found: IsConnect return sOutputChannel_Current = {sOutputChannel_Current}, setting: nSlotAddr = {mySwitchConfig.nSlotAddr}, nX1MatrixDeviceAddr = {nX1MatrixDeviceAddr}");
                throw new HardwareErrorException(ex.Message, "Map_DICON_GP750_v3");
            }

            if (nMDeviceAddr <= 4)
            {
                string sPort_Current = string.Empty;
                try
                {
                    lock (_ProtocolX)
                    {
                        sPort_Current = this.query(string.Format("*CLS;SL{0}:M{1}?", mySwitchConfig.nSlotAddr, nMDeviceAddr));
                    }
                    string[] arResults = sPort_Current.Split(',');
                    int nPort_Current = 0;
                    nPort_Current = Convert.ToInt32(arResults[arResults.Length - 1]);
                    if (nPort_Current != nMPort)
                        return false;
                }
                catch (Exception ex)
                {
                    clog.Error(ex, $"Error Found: IsConnect return sPort_Current = {sPort_Current}, setting: nSlotAddr = {mySwitchConfig.nSlotAddr}, nMDeviceAddr = {nMDeviceAddr}");
                    throw new HardwareErrorException(ex.Message, "Map_DICON_GP750_v3");
                }
            }

            return true;
        }

        /// <summary>
        /// This function connects one input connector to one output connector on a Dicon switch according to provided 
        /// names for input and output channels (aliases defined in station configuration file)
        /// </summary>
        /// <param name="strOutputChannel">alias for output channel from station configuration file</param>
        /// <param name="strInputChannel">alias for input channel from station configuration file</param>
        /// <returns></returns>
        public bool Connect(string strOutputChannel, string strInputChannel)
        {
            Log("Map_DICON_GP750_v3.Connect({0}, {1})", strOutputChannel, strInputChannel);

            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nLogicalInputChannel = mySwitchConfig.channelMap[strInputChannel] - 1;
            // Outputs #17 and #18 are special in a way that they are connected directly to X1 matrix and they do not have 1x4 M switch
            // Outut #17 fits the logic below for calculation of the nMDeviceAddr, but for #18 we need to replace "17" with "20" in order to keep calculations correct
            // please see schematic of the GP750 matrix switch
            if (nLogicalInputChannel == 17)
            {
                nLogicalInputChannel = 20;
            }
            nMDeviceAddr = (nLogicalInputChannel / nTotalMChannel) + 1;
            int nMPort = (nLogicalInputChannel % nTotalMChannel) + 1;
            int nX1MatrixDeviceAddr = mySwitchConfig.channelMap[strOutputChannel];

            Debug.WriteLine("nLogicalInputChannel = " + nLogicalInputChannel.ToString());
            Debug.WriteLine("nMDeviceAddr = " + nMDeviceAddr.ToString());
            Debug.WriteLine("nMPort = " + nMPort.ToString());
            Debug.WriteLine("nX1MatrixDeviceAddr = " + nX1MatrixDeviceAddr.ToString());

            //Format: SL<slot index>:X<device index> CHannel <input> <output>
            //Parameters: <input>, The input channel
            //<output>, The new output channel
            //Return Value: None
            //Description: The X CHannel command establishes an optical connection between input channel input and output channel output of the selected matrix switch. Connecting an input channel to output channel 0 will switch that input to the off position.
            //Note: The X CHannel command is the default command for matrix switches. The CHannel command token may be omitted.
            //Example: The following command establishes an optical connection between input channel 5 and output channel 8 for the first matrix switch in slot 1.
            //SL1:X1 5 8
            string strResult;
            lock (_ProtocolX)
            {
                for (int nTry = 0; nTry < mySwitchConfig.nRetry; nTry++)
                {
                    strResult = this.query(string.Format("SL{0}:X1 {1},{2}", mySwitchConfig.nSlotAddr, nX1MatrixDeviceAddr, nMDeviceAddr));
                    if (IsMatrixSwitchConnect(strOutputChannel, strInputChannel))
                    {
                        break;
                    }
                    else if (nTry == mySwitchConfig.nRetry - 1)
                    {
                        Log($"Error: Map_DICON_GP750_v3.Connect({strOutputChannel}, {strInputChannel}) fails at Matrix Switch (X) after {nTry} tries");
                        throw new HardwareErrorException($"Error: Map_DICON_GP750_v3.Connect({strOutputChannel}, {strInputChannel}) fails at Matrix Switch (X) after {nTry} tries", this.Name);
                    }
                    Pause(1000);
                } // wait for the above Matric Switch Channels to connect

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

                if (nMDeviceAddr <= 4)
                {
                    for (int nTry = 0; nTry < mySwitchConfig.nRetry; nTry++)
                    {
                        strResult = this.query(string.Format("SL{0}:M{1} {2}", mySwitchConfig.nSlotAddr, nMDeviceAddr, nMPort));
                        if (IsMultiChannelSwitchConnect(strOutputChannel, strInputChannel))
                        {
                            break;
                        }
                        else if (nTry == mySwitchConfig.nRetry - 1)
                        {
                            Log($"Error: Map_DICON_GP750_v3.Connect({strOutputChannel}, {strInputChannel}) fails at Multi-Channel Switch (M) after {nTry} tries");
                            throw new HardwareErrorException($"Error: Map_DICON_GP750_v3.Connect({strOutputChannel}, {strInputChannel}) fails at Multi-Channel Switch (M) after {nTry} tries", this.Name);
                        }
                        Pause(1000);
                    }
                }
            }
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
            Log("Map_DICON_GP750_v3.Disconnect({0}, {1})", strOutputChannel, strInputChannel);

            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nLogicalInputChannel = mySwitchConfig.channelMap[strInputChannel] - 1;
            nMDeviceAddr = (nLogicalInputChannel / nTotalMChannel) + 1;
            int nX1MatrixDeviceAddr = mySwitchConfig.channelMap[strOutputChannel];
            int nPort = 0;
            //int nOutputChannel = 0;

            lock (_ProtocolX)
            {
                //write("CLS*");
                string strResult = this.query(string.Format("SL{0}:M{1} {2}", mySwitchConfig.nSlotAddr, nMDeviceAddr, nPort));
                strResult = this.query(string.Format("SL{0}:X1 {1},{2}", mySwitchConfig.nSlotAddr, nX1MatrixDeviceAddr, nMDeviceAddr));
            }

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
            string sPort_Current;
            lock (_ProtocolX)
            {
                sPort_Current = this.query(string.Format("*CLS;SL{0}:M{1}?", mySwitchConfig.nSlotAddr, Slot));
            }
            string[] subString = sPort_Current.Split(',');
            intInput = Convert.ToInt32(subString[2]);

            int MaxOut = intOutput;
            for (int i = 1; i <= MaxOut; i++)
            {
                string sOutputChannel_Current = string.Empty;
                try
                {
                    lock (_ProtocolX)
                    {
                        //this.write("CLS*");
                        sOutputChannel_Current = this.query(string.Format("*CLS;SL{0}:X1 {1}?", mySwitchConfig.nSlotAddr, i));
                    }
                    subString = sOutputChannel_Current.Split(',');
                    if (Convert.ToInt32(subString[2]) == Slot)
                    {
                        intOutput = Convert.ToInt32(subString[1]);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    clog.Error(ex, $"Error Found: GetSwitchSetting return sOutputChannel_Current = {sOutputChannel_Current}, setting: nSlotAddr = {mySwitchConfig.nSlotAddr}, i = {i}");
                    throw new HardwareErrorException(ex.Message, "Map_DICON_GP750_v3");
                }
                intOutput = 0;
            }
            return true;
        }

        #endregion

    }
}

