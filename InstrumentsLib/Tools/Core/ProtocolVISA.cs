using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NationalInstruments.VisaNS;
using Utility;

namespace InstrumentsLib.Tools.Core
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProtocolVISAConfig : ProtocolXConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool TerminationCharacterEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte TerminationCharacter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool bAppendStringToCmd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strToAppend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int nBaudRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StopBitType StopBits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short DataBits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FlowControlTypes FlowControl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nInBufferSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nOutBufferSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool bClearSessionOnQuery { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ProtocolVISAConfig()
        {
            nTimeout = 5000;
            MaxRetries = 4;
            TerminationCharacterEnabled = true;
            TerminationCharacter = 0xA; //newline\n

            bAppendStringToCmd = false;
            strToAppend = "\n";
            bTakeScreenShot = false;
            bClearSessionOnQuery = true;
            nBaudRate = 9600;
            Parity = Parity.None;
            StopBits = StopBitType.One;
            DataBits = 8;
            FlowControl = FlowControlTypes.None;
            nInBufferSize = 2048;
            nOutBufferSize = 2048;

            buildTargetClassInfo(typeof(ProtocolVISA));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProtocolVISA : ProtocolX
    {
        //public int _defaultBufferSize { get; set; }

        static object _lock = new object();
        /// <summary>
        ///  VISA
        /// </summary>
        protected MessageBasedSession mbsession = null;
        /// <summary>
        /// 
        /// </summary>
        public MessageBasedSession Mbsession
        {
            get
            {
                lock (_lock)
                {
                    return mbsession;
                }
            }
            set
            {
                lock (_lock)
                {
                    mbsession = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void clearBuffer()
        {
            lock (_lock)
            {
                mbsession.Clear();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTimeoutSec"></param>
        /// <returns></returns>
        public override bool setTimeout(int nTimeoutSec)
        {
            lock (_lock)
            {
                if (_config.bSimulation)
                {
                    return true;
                }

                mbsession.Timeout = nTimeoutSec * 1000;
                log("Timeout = " + mbsession.Timeout.ToString());

                return true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="bNoRetry"></param>
        /// <returns></returns>
        public override string query(string cmd, bool bNoRetry = false)
        {
            lock (_lock)
            {
                if (_config.bSimulation)
                {
                    if (_config.bUseSimData)
                    {
                        return _config.mySimData.StringData.getSimData();
                    }
                    else
                    {
                        return "";
                    }
                }

                if (this._myProtocolVISAConfig.bAppendStringToCmd)
                {
                    cmd = cmd + _myProtocolVISAConfig.strToAppend;
                }

                //log(this._config.strName + " : " + this.ToString() + ".query() command = " + cmd);

                string strResponse = "";
                for (int nTry = 1; nTry <= _config.MaxRetries; nTry++)
                {
                    try
                    {
                        if (nTry == _config.MaxRetries)
                        {
                            OSScreenShot("VISAQuery");
                            log(this._config.strName + " : " + this.ToString() + ".query() re-initialising");
                            initialize(); // re-initialize this VISA protocol to see if it can be recovered near the last retry
                            Pause(3000);// wait for a little longer after almost reaching MaxRetries to query the last time                            
                        }
                        if (_myProtocolVISAConfig.bClearSessionOnQuery) mbsession.Clear();
                        strResponse = mbsession.Query(cmd);
                        log(this._config.strName + " : " + this.ToString() + ".query() command = " + cmd + "  strResponse = " + strResponse);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (bNoRetry) throw;
                        if (_config.bVerbose)
                        {
                            log(this._config.strName + " : " + this.ToString() + ".query() = EXCEPTION; nTry=" + nTry + "  < _config.MaxRetries=" + _config.MaxRetries);
                            log(ex.ToString());
                        }

                        Pause(500);

                        mbsession.Clear();

                        if (nTry == _config.MaxRetries)
                        {
                            //Log exception
                            OSScreenShot("VISAQuery");
                            log(this._config.strName + " : " + this.ToString() + ".query() = EXCEPTION; nTry=" + nTry + "  < _config.MaxRetries=" + _config.MaxRetries);
                            log(ex.ToString());

                            //ex.Data.Clear(); // do not propagate exception to upper levels
                            //strResponse = "";
                            // was not to throw exception as above, now throw
                            throw;

                        }
                    }
                }

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".query() Response = " + strResponse);
                }

                return strResponse;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool write(string cmd)
        {
            lock (_lock)
            {
                if (_config.bSimulation)
                {
                    return true;
                }

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".write " + cmd);
                }

                if (this._myProtocolVISAConfig.bAppendStringToCmd)
                {
                    cmd = cmd + _myProtocolVISAConfig.strToAppend;
                }

                //mbsession.Timeout = 10000;

                for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
                {
                    try
                    {
                        //if (cmd.Equals(":CHANnel3:WAV WAV2"))
                        //{
                        //    mbsession.Timeout = 5000;
                        //}

                        lock (this)
                        {
                            mbsession.Write(cmd);
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Log exception
                        clog.Error(ex, "write Exception");
                        log(ex.ToString());

                        Pause(1000);

                        mbsession.Clear();

                        if (nTry == (_config.MaxRetries - 1))
                            throw;
                    }
                }

                return false;//Max tries did not work
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string read()
        {
            lock (_lock)
            {
                if (_config.bSimulation)
                {
                    if (_config.bUseSimData)
                    {
                        return _config.mySimData.StringData.getSimData();
                    }
                    else
                    {
                        return "";
                    }
                }

                // Read for VISA
                string responseString = "";

                lock (this)
                {
                    responseString = mbsession.ReadString();
                }

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".read = " + responseString);
                }

                return responseString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ReadSRQ()
        {
            StatusByteFlags result = this.mbsession.ReadStatusByte();

            return (int)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBytes"></param>
        /// <returns></returns>

        public override string read(int nBytes)
        {
            lock (_lock)
            {
                if (_config.bSimulation)
                {
                    if (_config.bUseSimData)
                    {
                        return _config.mySimData.StringData.getSimData();
                    }
                    else
                    {
                        return "";
                    }
                }

                // Read for VISA
                string responseString = "";

                lock (this)
                {
                    responseString = mbsession.ReadString(nBytes);
                }

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".read = " + responseString);
                }

                return responseString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nByte"></param>
        /// <returns></returns>
        public override byte[] readByte(int nByte)
        {
            lock (_lock)
            {
                // Read for VISA
                string responseString = "";
                responseString = mbsession.ReadString();

                byte[] buffer = mbsession.ReadByteArray(nByte);

                return buffer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override byte[] ReadByteArray()
        {
            lock (_lock)
            {
                byte[] buffer = mbsession.ReadByteArray();

                return buffer;
            }
        }


        //public virtual double queryDouble(string cmd)
        //{
        //    double result = 0.0;

        //    if (_config.bSimulation)
        //    {
        //        return _config.mySimData.DoubleData.getSimData();
        //    }

        //    for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
        //    {
        //        try
        //        {
        //            result = Convert.ToDouble(query(cmd));
        //            return result;//This is a good result
        //        }
        //        catch (Exception ex)
        //        {
        //            //Log exception
        //            this._protocolLog.ErrorException("queryDouble Exception", ex);

        //            if (nTry == (_config.MaxRetries - 1))
        //                throw;
        //        }
        //    }

        //    return result;
        //}

        //public virtual int queryInt32(string cmd)
        //{
        //    Int32 result = 0;

        //    if (_config.bSimulation)
        //    {
        //        return _config.mySimData.IntData.getSimData();
        //    }

        //    for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
        //    {
        //        try
        //        {
        //            result = Convert.ToInt32(query(cmd));
        //            return result;//This is a good result
        //        }
        //        catch (Exception ex)
        //        {
        //            //Log exception
        //            this._protocolLog.ErrorException("queryInt32 Exception", ex);
        //            if (nTry == (_config.MaxRetries - 1))
        //                throw;
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            ProtocolVISAConfig myconfig = (ProtocolVISAConfig)_config;

            if (_config.bSimulation)
            {
                return true;
            }

            lock (_lock)
            {
                base.initialize();

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".initialize()");
                }

                mbsession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(myconfig.Resource);

                //mbsession = new MessageBasedSession(myconfig.Resource);

                mbsession.Timeout = myconfig.nTimeout;
                mbsession.DefaultBufferSize = 2000 * 1000;
                mbsession.TerminationCharacterEnabled = myconfig.TerminationCharacterEnabled;
                mbsession.TerminationCharacter = myconfig.TerminationCharacter;


                // cast message based session to a serial session
                SerialSession ss = mbsession as SerialSession;
                if (ss != null)// if the mesage based session is a serial session
                {
                    ss.BaudRate = myconfig.nBaudRate;
                    ss.Parity = myconfig.Parity;
                    ss.StopBits = myconfig.StopBits;
                    ss.DataBits = myconfig.DataBits;
                    ss.FlowControl = myconfig.FlowControl;
                    //ss.TerminationCharacterEnabled = true; 
                    //ss.TerminationCharacter = 13;                      
                    // Sets the size of the low-level I/O communication buffer.
                    // without setting these low level I/O buffer ssize, we have seen error in serial communication (fec.c and my_wedge100_qsfp_eeprom), even using NIMax tool
                    // reference: SPAN project NationalInstruments.VisaNS.SerialSession.cs
                    ss.SetBufferSize(BufferTypes.InBuffer, myconfig.nInBufferSize); // low level receive buffer
                    ss.SetBufferSize(BufferTypes.OutBuffer, myconfig.nOutBufferSize); // low level transmit buffer

                }

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void close()
        {
            if (_config.bSimulation)
            {
                return;
            }

            lock (_lock)
            {
                if (null != mbsession)
                {
                    //mbsession.Clear();
                    lock (this)
                    {
                        mbsession.Dispose();
                    }

                    if (_config.bVerbose)
                    {
                        log(this._config.strName + " : " + this.ToString() + ".close()");
                    }
                }
            }
        }

        ProtocolVISAConfig _myProtocolVISAConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public ProtocolVISA(ProtocolVISAConfig config) : base(config)
        {
            _myProtocolVISAConfig = config;
            this._config = config;
        }
    }
}
