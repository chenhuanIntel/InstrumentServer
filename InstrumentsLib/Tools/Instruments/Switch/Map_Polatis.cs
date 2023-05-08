using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;
using Utility;
using System.Text.RegularExpressions;

namespace InstrumentsLib.Tools.Instruments.Switch
{
    /// <summary>
    /// 
    /// </summary>
    public class MapSwitch_Polatis_Config : MapSwitch_DICON_GP750_Config
    {
        //public int nSlotAddr { get; set; }
        //public Dictionary<string, int> channelMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MapSwitch_Polatis_Config()
        {
            //channelMap = new Dictionary<string, int>();
            buildTargetClassInfo(typeof(Map_Polatis));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Map_Polatis : InstrumentX, ISwitch
    {
        private MapSwitch_Polatis_Config mySwitchConfig;
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
        public Map_Polatis(MapSwitch_Polatis_Config config)
            : base(config)
        {
            mySwitchConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public Map_Polatis(MapSwitch_Polatis_Config config, ProtocolX protocol)
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
        /// IsConnect
        /// </summary>
        /// <param name="strOutputChannel">alias for output channel from station configuration file</param>
        /// <param name="strInputChannel">alias for input channel from station configuration file</param>
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

            int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            int nOutputChannel = mySwitchConfig.channelMap[strOutputChannel];
            string sOutputChannel_Current = string.Empty;
            // :OXC:SWITch:CONNect:PORT? <port>
            // example :oxc:swit:conn:port? 2
            // reply will be "18" when ports connected by :oxc:swit:conn:add (@1, 2, 3),(@17,18,19)
            try
            {
                lock (_ProtocolX)
                {
                    sOutputChannel_Current = this.query($"*CLS;:OXC:SWITch:CONNect:PORT? {nInputChannel};*OPC?");
                }
                // remove whitespaces and " 
                // checkStringForDoubleQuotes = @""""
                // e.g. "\"361\"\r\n", where the \" is actually "
                // result = "361"
                sOutputChannel_Current = Regex.Replace(sOutputChannel_Current, @"\s+", "");
                sOutputChannel_Current = Regex.Replace(sOutputChannel_Current, @"""", "");
                int nOutputChannel_Current = 0;
                nOutputChannel_Current = Convert.ToInt32(sOutputChannel_Current);
                if (nOutputChannel_Current != nOutputChannel)
                    return false;
            }
            catch (Exception ex)
            {
                clog.Error(ex, $"Error Found: IsConnect return sOutputChannel_Current = {sOutputChannel_Current}, expecting {strOutputChannel}");
                throw new HardwareErrorException(ex.Message, "Map_Polatis");
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
            Log("Map_Polatis.Connect({0}, {1})", strOutputChannel, strInputChannel);

            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            int nOutputChannel = mySwitchConfig.channelMap[strOutputChannel];
            // :OXC:SWITch:CONNect:ADD <ing-list>,<egr-list> [<mode>]
            // example :oxc:swit:conn:add (@1, 2, 3),(@17,18,19)
            string strResult;
            lock (_ProtocolX)
            {
                for (int nTry = 0; nTry < mySwitchConfig.nRetry; nTry++)
                {
                    strResult = this.query($":OXC:SWITch:CONNect:ADD (@{nInputChannel}),(@{nOutputChannel});*OPC?");
                    if (IsConnect(strOutputChannel, strInputChannel))
                    {
                        break;
                    }
                    else if (nTry == mySwitchConfig.nRetry - 1)
                    {
                        Log($"Error: Map_Polatis.Connect({strOutputChannel}, {strInputChannel}) fails at Matrix Switch (X) after {nTry} tries");
                        throw new HardwareErrorException($"Error: Map_Polatis.Connect({strOutputChannel}, {strInputChannel}) fails at Matrix Switch (X) after {nTry} tries", this.Name);
                    }
                    Pause(1000);
                } // wait for the above Matric Switch Channels to connect
            }
            return true;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <param name="strOutputChannel">alias for output channel from station configuration file</param>
        /// <param name="strInputChannel">alias for input channel from station configuration file</param>
        /// <returns></returns>
        public bool Disconnect(string strOutputChannel, string strInputChannel)
        {
            Log("Map_Polatis.Disconnect({0}, {1})", strOutputChannel, strInputChannel);

            if (this._config.bSimulation)
                return true;

            if (!ValidateChannels(strInputChannel, strOutputChannel))
            {
                return false;
            }

            int nInputChannel = mySwitchConfig.channelMap[strInputChannel];
            int nOutputChannel = mySwitchConfig.channelMap[strOutputChannel];
            // :OXC:SWITch:CONNect:SUB <ing-list> <egr-list> [<mode>]
            // example 1  :oxc:swit:conn:sub (@1),(@17)
            // example 2  :oxc:swit:conn:sub (@1,2,3),(@)
            lock (_ProtocolX)
            {
                string strResult = this.query($":OXC:SWITch:CONNect:SUB (@{nInputChannel}),(@{nOutputChannel});*OPC?");
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
        /// NotImplementedException
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

