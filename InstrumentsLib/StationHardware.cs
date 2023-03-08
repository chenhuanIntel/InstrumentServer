using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InstrumentsLib.Tools.Core;
using Utility;


namespace InstrumentsLib
{
    /// <summary>
    /// The station/instruments configuration class
    /// </summary>
    [Serializable]
    public class StationInstrumentConfig
    {
        /// <summary>
        /// Legacy Station Config File Name
        /// </summary>
		public const string STATIONCONFIGFILENAME = "StationInstrumentConfig.config";

        /// <summary>
        /// Protocol config array
        /// </summary>
        public List<ProtocolXConfig> arProtocolConfig { get; set; }

        /// <summary>
        /// IntrumentX array
        /// </summary>
        public List<InstrumentXConfig> arInstConfig { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public StationInstrumentConfig()
        {
            arProtocolConfig = new List<ProtocolXConfig>();
            arInstConfig = new List<InstrumentXConfig>();
        }
    }

    /// <summary>
    /// The purpose of this class is as a singleton factory pattern to manage and share instruments/resources
    /// </summary>
    public class StationHardware
    {
        /// <summary>
        /// The config file for this station
        /// </summary>
        protected StationInstrumentConfig _config;
        /// <summary>
        /// 
        /// </summary>
		public StationInstrumentConfig myConfig
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// List of concrete protocolX objects
        /// </summary>
        protected Dictionary<string, ProtocolX> _mapProtocol = new Dictionary<string, ProtocolX>();
        /// <summary>
        /// 
        /// </summary>
		public Dictionary<string, ProtocolX> MapProtocol
        {
            get
            {
                return _mapProtocol;
            }
        }

        /// <summary>
        /// Map of instrument name to its InstrumentX object
        /// </summary>
        protected Dictionary<string, InstrumentX> _mapInst = new Dictionary<string, InstrumentX>();
        /// <summary>
        /// 
        /// </summary>
		public Dictionary<string, InstrumentX> MapInst
        {
            get
            {
                return _mapInst;
            }
        }



        /// <summary>
        /// 1. Check what sequencer to be applied
        /// 2. Generate File Path of Station config file
        /// 3. Initialize Station config
        /// </summary>
        /// <returns>Manual Load Sequence Flag</returns>
        public bool Initialize()
        {
            string strStationConfig = Path.Combine(Application.StartupPath, StationInstrumentConfig.STATIONCONFIGFILENAME);
            return Initialize(strStationConfig); 
        }

        /// <summary>
        /// Initialize Station Instrument file
        /// Convert it to StationInstrumentConfig if it is SimplifyInstrumentConfig
        /// </summary>
        /// <param name="strStationFile">Station Instrument File Path</param>
        /// <param name="bSkipSetupInit">Flag whether to skip initialize setup</param>
        /// <param name="bFWUI">Flag whether is to initialize for FWUI Application</param>
        /// <returns>Success/Fail Initialize</returns>
        public bool Initialize(string strStationFile, bool bSkipSetupInit = false, bool bFWUI = false)
        {
            object obj = GenericSerializer.DeserializeFromXML(strStationFile);

            if (obj.GetType().Name == typeof(StationInstrumentConfig).Name)
            {
                _config = (StationInstrumentConfig)obj;
            }
            else
            {
                throw new Exception("Error: Unknown Instrument config type.");
            }
            return Initialize(_config, bSkipSetupInit, bFWUI);
        }

        /// <summary>
        /// Dispose/Disconnect all Protocols &amp; I2C connection before Initialize StationConfig Again
        /// </summary>
        /// <param name="myConfig"></param>
        public void ReInitialize(StationInstrumentConfig myConfig)
        {
            foreach (string key in MapProtocol.Keys)
            {
                MapProtocol[key].close();
            }
            Initialize(myConfig);
        }

        /// <summary>
        /// Initialization method
        /// </summary>
        /// <param name="myConfig">StationInstrumentConfig Object</param>
        /// <param name="bSkipSetupInit">Flag whether to skip initialize setup</param>
        /// <param name="bFWUI">Flag whether is to initialize for FWUI Application</param>
        /// <returns>Success/Fail Initialize</returns>
        public bool Initialize(StationInstrumentConfig myConfig, bool bSkipSetupInit = false, bool bFWUI = false)
        {
            clean();

            _config = myConfig;
            int nIdx;

            //Build all ProtocolX instances, map to their bins...
            ProtocolX ProtocolRef = null;
            for (nIdx = 0; nIdx < _config.arProtocolConfig.Count; nIdx++)
            {
                string str = String.Format("Initializing {0} ...", _config.arProtocolConfig.ToString());
                clog.Log(string.Format("StationHardware.Initialize: buildObject {0}", _config.arProtocolConfig[nIdx].strName));
                ProtocolRef = ActivatorBasedFactory.BuildObject<ProtocolX, ProtocolXConfig>(_config.arProtocolConfig[nIdx].strTargetClassType, _config.arProtocolConfig[nIdx]);
                this._mapProtocol.Add(_config.arProtocolConfig[nIdx].strName, ProtocolRef);
            }

            //Build all InstrumentX instruments, map the special intruments to their bins...
            InstrumentX InstRef = null;

            for (nIdx = 0; nIdx < _config.arInstConfig.Count; nIdx++)
            {
                string str = String.Format("Initializing {0} ...", _config.arInstConfig.ToString());
                clog.Log(string.Format("StationHardware.Initialize: buildObject {0}", _config.arInstConfig[nIdx].strName));
                InstRef = ActivatorBasedFactory.BuildObject<InstrumentX, InstrumentXConfig>(_config.arInstConfig[nIdx].strTargetClassType, _config.arInstConfig[nIdx]);
                Debug.Assert(!this._mapInst.ContainsKey(_config.arInstConfig[nIdx].strName));
                this._mapInst.Add(_config.arInstConfig[nIdx].strName, InstRef);
            }


            //For each Protocol, call initialize
            foreach (ProtocolX refProtocol in this._mapProtocol.Values)
            {
                string strProtocolName = refProtocol._config.strName;
                string str = String.Format("Initializing {0}, Protocol name = {1} ...", refProtocol.ToString(), strProtocolName);
                clog.Log(string.Format("StationHardware.Initialize: {0}, Protocol Name = {1}", refProtocol.ToString(), strProtocolName));
                try
                {
                    if (refProtocol.initialize() == false)
                    {
                        string msg = string.Format("StationHardware.Initialize  {0}, Protocol Name = {1} FAILED", refProtocol.ToString(), strProtocolName);
                        clog.Warn("StationHardware.Initialize", msg);
                        MessageBox.Show(msg);
                        throw new Exception(msg);
                        // here was to return a false' but the returned value never checked by GUI
                        // hence thro exception
                        //return false;
                    }
                }
                catch (Exception ex)
                {
                    string strErr = string.Format("StationHardware.Initialize: {0}\n\nProtocol Name = {1}\tFAILED!!!\n\n", refProtocol.ToString(), strProtocolName);
                    strErr = strErr + ex.ToString();
                    clog.Error(ex, strErr);
                    MessageBox.Show("Excception: " + strErr);
                }
            }

            //For each InstrumentX, call initialize
            foreach (InstrumentX instr in this._mapInst.Values)
            {
                string str = $"Initializing {instr}, strName = {instr.Config.strName}...";
                clog.Log($"StationHardware.Initialize: {str}");
                instr.initialize();
            }

            return true;
        }


        /// <summary>
        /// Method to clean station instruments singleton to re initialize...
        /// </summary>
		private void clean()
        {
            _mapProtocol = new Dictionary<string, ProtocolX>();
            _mapInst = new Dictionary<string, InstrumentX>();
        }

        /// <summary>
        /// Shutdown all equipment
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {


            //For each protocol, close and release handle
            foreach (ProtocolX protocol in this._mapProtocol.Values)
            {
                protocol.close();
            }

            //For each InstrumentX, call shutdown
            foreach (InstrumentX instr in this._mapInst.Values)
            {
                instr.close();
            }

            this._mapProtocol.Clear();
            this._mapInst.Clear();

            return true;
        }



        #region StationHardware Singleton Design Pattern

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static StationHardware _instance;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static StationHardware Instance()
        {
            if (null == _instance)
            {
                _instance = new StationHardware();
            }

            return _instance;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private StationHardware()
        {

        }

        #endregion

    }
}
