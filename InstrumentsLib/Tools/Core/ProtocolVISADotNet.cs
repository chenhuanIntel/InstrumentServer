using System;
using NationalInstruments.Visa;
using NationalInstruments.Visa.Internal;
using Ivi.Visa;
using Utility;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace InstrumentsLib.Tools.Core
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ProtocolVISADotNetConfig : ProtocolXConfig
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string Resource { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int nTimeout { get; set; }
        [DataMember]
        /// <summary>
        /// Flag to interpret nTerminationCharacter as end of received message
        /// </summary>
        public bool bRxTerminationCharacterEnabled { get; set; }
        [DataMember]
        /// <summary>
        /// Flag to interpret nTerminationCharacter as end of sent message
        /// </summary>
        public bool bTxTerminationCharacterEnabled { get; set; }
        [DataMember]
        /// <summary>
        /// Termination character used at the end of a received message.  Requires bTerminationCharacterEnabled = true
        /// </summary>
        public byte nTerminationCharacter { get; set; }
        [DataMember]
        /// <summary>
        /// Write buffer size in bytes
        /// </summary>
        public int nWriteBufferSize { get; set; }
        [DataMember]
        /// <summary>
        /// Read buffer size in bytes
        /// </summary>
        public int nReadBufferSize { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bAppendStringToCmd { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public string strToAppend { get; set; }
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public bool bClearSessionOnQuery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolVISADotNetConfig()
        {
            nTimeout = 600000;
            MaxRetries = 4;
            bRxTerminationCharacterEnabled = true;             // For legacy reasons, some devices read back in binary
            bTxTerminationCharacterEnabled = true;
            nTerminationCharacter = 0x0A;                       // line feed or /n by default
            nWriteBufferSize = 2000 * 1000;                      // ProtocolVisa.cs legacy number
            nReadBufferSize = 2000 * 1000;                       // ProtocolVisa.cs legacy number

            bAppendStringToCmd = false;
            strToAppend = "\n";

            bClearSessionOnQuery = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProtocolVISADotNet : ProtocolX
    {
        //static object _lock = new object();

        /// <summary>
        ///  VISA session object
        /// </summary>
        protected MessageBasedSession mbsession = null;
        //public MessageBasedSession Mbsession
        //{
        //    get
        //    {
        //        lock (mbsession)
        //        {
        //            return mbsession;
        //        }
        //    }
        //    set
        //    {
        //        lock (mbsession)
        //        {
        //            mbsession = value;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTimeoutSec"></param>
        /// <returns></returns>
        public override bool setTimeout(int nTimeoutSec)
        {
            if (_config.bSimulation)
            {
                return true;
            }

            lock (this)
            {
                try
                {
                    mbsession.LockResource();
                    mbsession.TimeoutMilliseconds = nTimeoutSec * 1000;
                    log("Timeout = " + mbsession.TimeoutMilliseconds.ToString());

                    return true;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
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
            if (_config.bSimulation)
            {
                return "";
            }

            bool bSkipUnlockResource = false;

            lock (this)
            {
                try
                {
                    mbsession.LockResource();

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


                    if (this._configVisaDotNet.bAppendStringToCmd)
                    {
                        cmd = cmd + _configVisaDotNet.strToAppend;
                    }

                    string strResponse = "";
                    for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
                    {
                        try
                        {
                            if (this._configVisaDotNet.bClearSessionOnQuery)
                            {
                                mbsession.Clear();
                            }

                            if (_config.bVerbose)
                            {
                                log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ")");
                            }

                            mbsession.FormattedIO.WriteLine(cmd);
                            //mbsession.FormattedIO.FlushWrite(((ProtocolVISADotNetConfig)_config).bTxTerminationCharacterEnabled);
                            strResponse = mbsession.FormattedIO.ReadLine();

                            if (_config.bVerbose)
                            {
                                log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ") = " + strResponse);
                            }
                            break;
                        }
                        catch (Ivi.Visa.NativeVisaException nvEx)
                        {
                            if (_config.bVerbose)
                            {
                                log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ") = EXCEPTION");
                            }

                            clog.Error(nvEx, "query Exception");

                            // Free up visa lock resource before we initialize.
                            this.mbsession.UnlockResource();
                            bSkipUnlockResource = true; // Don't free up visa resource at finally since freed already...

                            this.close();
                            this.initialize();

                            //Log exception
                            clog.Error(nvEx, "query Exception " + this._config.strName);

                            if (nTry == _config.MaxRetries)
                            {
                                //Log exception
                                log(this._config.strName + " : " + this.ToString() + ".query() = EXCEPTION; nTry=" + nTry + "  < _config.MaxRetries=" + _config.MaxRetries);

                                //ex.Data.Clear(); // do not propagate exception to upper levels
                                //strResponse = "";
                                // was not to throw exception as above, now throw
                                throw;
                            }
                        }
                        catch (Ivi.Visa.IOTimeoutException toEx)
                        {
                            if (_config.bVerbose)
                            {
                                log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ") = EXCEPTION");
                            }

                            if (bNoRetry) throw;

                            clog.Error(toEx, "query Exception");

                            if (this._configVisaDotNet.bClearSessionOnQuery)
                            {
                                mbsession.Clear();
                            }

                            //Log exception
                            clog.Error(toEx, "query Exception " + this._config.strName);

                            if (nTry == _config.MaxRetries)
                            {
                                //Log exception
                                log(this._config.strName + " : " + this.ToString() + ".query() = EXCEPTION; nTry=" + nTry + "  < _config.MaxRetries=" + _config.MaxRetries);

                                //ex.Data.Clear(); // do not propagate exception to upper levels
                                //strResponse = "";
                                // was not to throw exception as above, now throw
                                throw;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_config.bVerbose)
                            {
                                log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ") = EXCEPTION");
                            }

                            clog.Error(ex, "query Exception");

                            if (this._configVisaDotNet.bClearSessionOnQuery)
                            {
                                mbsession.Clear();
                            }

                            //Log exception
                            clog.Error(ex, "query Exception " + this._config.strName);

                            if (nTry == _config.MaxRetries)
                            {
                                //Log exception
                                log(this._config.strName + " : " + this.ToString() + ".query() = EXCEPTION; nTry=" + nTry + "  < _config.MaxRetries=" + _config.MaxRetries);

                                //ex.Data.Clear(); // do not propagate exception to upper levels
                                //strResponse = "";
                                // was not to throw exception as above, now throw
                                throw;
                            }
                        }
                    }

                    if (_config.bVerbose)
                    {
                        log(this._config.strName + " : " + this.ToString() + ".query(" + cmd + ") = " + strResponse);
                    }

                    return strResponse;
                }
                finally
                {
                    if (!bSkipUnlockResource)
                    {
                        mbsession.UnlockResource();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool write(string cmd)
        {
            if (_config.bSimulation)
            {
                return true;
            }

            lock (this)
            {
                try
                {
                    mbsession.LockResource();

                    if (_config.bVerbose)
                    {
                        log(this._config.strName + " : " + this.ToString() + ".write " + cmd);
                    }

                    if (this._configVisaDotNet.bAppendStringToCmd)
                    {
                        cmd = cmd + _configVisaDotNet.strToAppend;
                    }

                    for (int nTry = 0; nTry < _config.MaxRetries; nTry++)
                    {
                        try
                        {
                            lock (this)
                            {
                                mbsession.FormattedIO.WriteLine(cmd);
                                //mbsession.FormattedIO.FlushWrite(((ProtocolVISADotNetConfig)_config).bTxTerminationCharacterEnabled);
                            }

                            return true;
                        }
                        //catch(nativevisaexception VisaException)
                        catch (Exception ex)
                        {
                            //Log exception
                            clog.Error(ex, "write Exception");

                            Pause(1000);

                            if (this._configVisaDotNet.bClearSessionOnQuery)
                            {
                                mbsession.Clear();
                            }

                            if (nTry == (_config.MaxRetries - 1))
                                throw;
                        }
                    }

                    return false;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string read()
        {
            lock (this)
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

                try
                {
                    mbsession.LockResource();

                    // Read for VISA
                    string responseString = "";

                    lock (this)
                    {
                        responseString = mbsession.FormattedIO.ReadLine();
                    }

                    if (_config.bVerbose)
                    {
                        log(this._config.strName + " : " + this.ToString() + ".read = " + responseString);
                    }

                    return responseString;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<byte> readTillEnd()
        {
            List<byte> lstByte = new List<byte>();
            lock (this)
            {
                if (_config.bSimulation)
                {
                    return lstByte;
                }

                try
                {
                    mbsession.LockResource();

                    string responseString = "";
                    lock (this)
                    {
                        try
                        {
                            lstByte.AddRange(mbsession.RawIO.Read());
                            responseString = Encoding.UTF8.GetString(lstByte.ToArray());
                            if (responseString.Substring(0, 1) != "#") throw new HardwareErrorException("Wrong content reading", "ProtocolVISADotNet");
                            int DataLenStringLen = Convert.ToInt32(responseString.Substring(1, 1));
                            int DataLen = Convert.ToInt32(responseString.Substring(2, DataLenStringLen));
                            lstByte.RemoveRange(0, DataLenStringLen + 2);
                            while (true)
                            {
                                lstByte.AddRange(mbsession.RawIO.Read());
                                if (lstByte.Count >= DataLen) break;
                            }
                            if (lstByte.Count > DataLen)
                            {
                                lstByte.RemoveRange(DataLen, lstByte.Count - DataLen);
                            }
                        }
                        catch (Exception ex)
                        {
                            clog.Log($"{_config.strName} : {this}.readTillEnd = {ex}");
                            throw;
                        }
                    }

                    if (_config.bVerbose)
                    {
                        responseString = "";
                        foreach (byte byt in lstByte)
                        {
                            responseString += $"{byt}, ";
                        }
                        responseString = responseString.Trim().TrimEnd(',');
                        log(this._config.strName + " : " + this.ToString() + ".readTillEnd = " + responseString);
                    }

                    return lstByte;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBytes"></param>
        /// <returns></returns>
        public override string read(int nBytes)
        {
            lock (this)
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

                try
                {
                    mbsession.LockResource();

                    // Read for VISA
                    string responseString = "";
                    responseString = mbsession.FormattedIO.ReadString(nBytes);


                    if (_config.bVerbose)
                    {
                        log(this._config.strName + " : " + this.ToString() + ".read = " + responseString);
                    }

                    return responseString;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nByte"></param>
        /// <returns></returns>
        public override byte[] readByte(int nByte)
        {
            lock (this)
            {
                try
                {
                    mbsession.LockResource();

                    byte[] buffer = new byte[nByte];

                    for (int ii = 0; ii < nByte; ii++)
                    {
                        buffer[ii] = (byte)mbsession.FormattedIO.ReadChar();
                    }
                    //mbsession.FormattedIO.ReadBinaryBlockOfByte(buffer, 0, nByte);

                    return buffer;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override byte[] ReadByteArray()
        {
            lock (this)
            {
                try
                {
                    mbsession.LockResource();

                    byte[] buffer = mbsession.FormattedIO.ReadBinaryBlockOfByte();

                    return buffer;
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
        {
            lock (this)
            {
                base.initialize();

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".initialize()");
                }

                ProtocolVISADotNetConfig myconfig = (ProtocolVISADotNetConfig)_config;

                if (!myconfig.bSimulation)
                {
                    if (mbsession != null)
                    {
                        if (!mbsession.IsDisposed)
                        {
                            mbsession.DisposeIfNotNull();
                        }
                    }

                    using (var rmsession = new ResourceManager())
                    {
                        mbsession = (MessageBasedSession)rmsession.Open(myconfig.Resource);
                        mbsession.TimeoutMilliseconds = myconfig.nTimeout;
                        mbsession.FormattedIO.WriteBufferSize = myconfig.nWriteBufferSize;
                        mbsession.FormattedIO.ReadBufferSize = myconfig.nReadBufferSize;
                        // mbsession.FormattedIO.TypeFormatter = new
                        mbsession.TerminationCharacterEnabled = myconfig.bRxTerminationCharacterEnabled;
                        mbsession.TerminationCharacter = myconfig.nTerminationCharacter;

                        if (true == mbsession.TerminationCharacterEnabled)
                        {
                            //((INativeVisaSession)mbsession).SetAttributeBoolean(NativeVisaAttribute., true);
                            //((INativeVisaSession)mbsession).SetAttributeBoolean(NativeVisaAttribute.SerialEndOut, true);
                        }

                        //mbsession.FormattedIO.BinaryEncoding = BinaryEncoding.RawBigEndian; // Used on binary readbacks
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void close()
        {
            lock (this)
            {
                if (null != mbsession)
                {
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

        /// <summary>
        /// 
        /// </summary>
        public override void clearBuffer()
        {
            lock (this)
            {
                try
                {
                    mbsession.LockResource();
                    mbsession.Clear();
                }
                finally
                {
                    mbsession.UnlockResource();
                }
            }
        }

        ProtocolVISADotNetConfig _configVisaDotNet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public ProtocolVISADotNet(ProtocolVISADotNetConfig config) : base(config)
        {
            this._configVisaDotNet = config;
            this._config = config;
        }
    }
}
