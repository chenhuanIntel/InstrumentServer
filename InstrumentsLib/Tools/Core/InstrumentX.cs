using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using NationalInstruments.VisaNS;
using System.Threading;
using InstrumentsLib.Tools.Instruments.Misc;
using Utility;

namespace InstrumentsLib.Tools.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MeasurementCmd
    {
        /// <summary>
        /// 
        /// </summary>
        public Func<string, bool> Write;
        /// <summary>
        /// 
        /// </summary>
        public Func<string, string> Query;
        /// <summary>
        /// 
        /// </summary>
        public Func<string, string> QueryNoRetry;
        /// <summary>
        /// 
        /// </summary>
        public Func<string, double> QueryDouble;
        /// <summary>
        /// 
        /// </summary>
        public Func<string, Int32> QueryInt32;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bReLock"></param>
        /// <returns></returns>
        public virtual bool Setup(bool bReLock = true)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool ReLock()
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public MeasurementCmd()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InstrumentXConfig : CDynamicConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public bool bSimulation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bVerbose { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bCreateProtocol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bIsPooled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolXConfig protocolCfg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProtocolObjectRefName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bTempCompensate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int nRetry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double dAttenOffset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DUTVariant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChanVariant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool bIsRef { get; set; }
        /// <summary>
        /// standardDeviation for nReTry power meter readings
        /// </summary>
        public double standardDeviation { get; set; }
        /// <summary>
        /// max number of additional reads in the sliding window method as timeout for power meter reading
        /// </summary>
        public int nAdditionalRead { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public InstrumentXConfig()
        {
            bTempCompensate = false;
            nRetry = 3;
            bVerbose = true;
            dAttenOffset = 0;
            MaxRetries = 60;
            standardDeviation = 0.5;
            nAdditionalRead = 3;

            buildTargetClassInfo(typeof(InstrumentX));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InstrumentX
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return _config.strName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected InstrumentXConfig _config;
        /// <summary>
        /// 
        /// </summary>
        public InstrumentXConfig Config
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ProtocolX _ProtocolX;

        /// <summary>
        /// 
        /// </summary>
        public InstrumentX()
        {
            //_syslog = CLogger.Instance().getSysLogger();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">The config object</param>
        public InstrumentX(InstrumentXConfig config)
        {
            _config = config;
            //_syslog = CLogger.Instance().getSysLogger();

            if (config.bCreateProtocol)
            {
                _ProtocolX = ActivatorBasedFactory.BuildObject<ProtocolX, ProtocolXConfig>(_config.protocolCfg.strTargetClassType, _config.protocolCfg);
            }
            else
            {
                _ProtocolX = StationHardware.Instance().MapProtocol[_config.ProtocolObjectRefName];
            }
        }

        /// <summary>
        /// Return instrument information
        /// </summary>
        /// <returns>instrument information</returns>
        public virtual string GetInfo()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual bool initFunctor(MeasurementCmd cmd)
        {
            cmd.Write = this.write;
            cmd.Query = this.query;
            cmd.QueryNoRetry = this.queryNoRetry;
            cmd.QueryDouble = this.queryDouble;
            cmd.QueryInt32 = this.queryInt32;

            return true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">The config object</param>
        /// <param name="protocol"></param>
        public InstrumentX(InstrumentXConfig config, ProtocolX protocol)
        {
            _ProtocolX = protocol;
            _config = config;
            //_syslog = CLogger.Instance().getSysLogger();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nSlot"></param>
        /// <returns></returns>
        public bool WaitForOpComplete(int nSlot)
        {
            int delay_ms = 1;

            for (int ii = 0; ii < _config.nRetry; ii++)
            {
                if (1 == this.queryInt32(string.Format(":SLOT{0}:OPC?", nSlot)))
                {
                    return true;
                }

                Pause(delay_ms);

                if (delay_ms < 1024)            // Keep delay times between *OPC? calls from getting excessively long
                {
                    delay_ms = delay_ms * 2;    // Essentially doing a binary search for when an operation of unknown length will be completed
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual string query(string cmd)
        {
            return _ProtocolX.query(cmd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual string queryNoRetry(string cmd)
        {
            return _ProtocolX.query(cmd, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual double queryDouble(string cmd)
        {
            return _ProtocolX.queryDouble(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual int queryInt32(string cmd)
        {
            return _ProtocolX.queryInt32(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual bool write(string cmd)
        {
            return _ProtocolX.write(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void reset()
        {
            //This will be device dependent
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string read()
        {
            return _ProtocolX.read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbyte"></param>
        /// <returns></returns>
        public virtual byte[] readByte(int nbyte)
        {
            return _ProtocolX.readByte(nbyte);
        }

        /// <summary>
        /// Hold process for sleep_milliseconds
        /// </summary>
        /// <param name="sleep_milliseconds">Time to delay</param>
        /// <param name="bLog">Whether want to Log message</param>
        public virtual void Pause(int sleep_milliseconds, bool bLog = true)
        {
            if (bLog)
            {
                Log("Pause({0})", sleep_milliseconds);
            }

            if (sleep_milliseconds >= 50)
            {
                Thread.Sleep(sleep_milliseconds);
            }
            else
            {
                DateTime dt = DateTime.Now;
                while ((DateTime.Now - dt).TotalMilliseconds < sleep_milliseconds) { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ShowGUI()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void HideGUI()
        {
            //throw new NotImplementedException();
        }

        //protected void Log(string strFormat, params Object[] myparams)
        //{
        //    string strMsg = string.Format(strFormat, myparams);
        //    Debug.WriteLine(strMsg);
        //    clog.Log(strMsg);
        //}

        //protected void Log(string strMsg)
        //{
        //    Debug.WriteLine(strMsg);
        //    //if (null == _syslog)
        //    //    return;

        //    clog.Log(strMsg);
        //}

        #region NEWLOGGER
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="myparams"></param>
        protected void Highlight(string strFormat, params Object[] myparams)
        {
            clog.Highlight(string.Format(strFormat, myparams));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        protected void MarkStart(string operation, clog.TimeKey key)
        {
            clog.MarkStart(operation, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="key"></param>
        protected void MarkEnd(string operation, clog.TimeKey key)
        {
            clog.MarkEnd(operation, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected void Error(System.Exception ex, string message)
        {
            Debug.WriteLine(ex.Message);
            clog.Error(ex, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        protected void Warn(string source, string message)
        {
            clog.Warn(source, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="myparams"></param>
        protected void Info(string strFormat, params Object[] myparams)
        {
            if (_config.bVerbose)
            {
                clog.Info(string.Format(strFormat, myparams));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="myparams"></param>
        protected void Log(string strFormat, params Object[] myparams)
        {
            if (_config.bVerbose)
            {
                Debug.WriteLine(string.Format(strFormat, myparams));
                clog.Log(string.Format(strFormat, myparams));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="myparams"></param>
        protected void Trace(string strFormat, params Object[] myparams)
        {
            if (_config.bVerbose)
            {
                clog.Trace(string.Format(strFormat, myparams));
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool initialize()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void close()
        {
            _ProtocolX.close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool getLock()
        {
            //if (null == _ProtocolX)
            //    return false;

            System.Threading.Monitor.Enter(this);
            Log("getLock() : " + this._config.strName);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool releaseLock()
        {
            //if (null == _ProtocolX)
            //    return false;

            System.Threading.Monitor.Exit(this);
            Log("releaseLock() : " + this._config.strName);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTimeoutSec"></param>
        /// <returns></returns>
        public virtual bool setTimeout(int nTimeoutSec)
        {
            return true;
        }

        /// <summary>
        /// Public handler for sending messages.
        /// </summary>
        public event System.EventHandler SendMessage;

        /// <summary>
        /// Raises the OnSendMessage event.
        /// </summary>
        /// <param name="message">Message Content</param>
        protected virtual void OnSendMessage(string message)
        {
            // Checks if the event is set.
            if (null != SendMessage)
            {
                // Create the arguments to be sent, in this case only the message content.
                MessageEventArgs e = new MessageEventArgs();
                e.msg = message;

                // Execute the delegate.
                SendMessage(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

