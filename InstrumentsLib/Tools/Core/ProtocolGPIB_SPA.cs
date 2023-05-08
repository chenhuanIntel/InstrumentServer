using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.NI4882;

namespace InstrumentsLib.Tools.Core
{
    [Serializable]
    public class ProtocolGPIBSPAConfig : ProtocolXConfig
    {
        public byte PrimaryAddress { get; set; }
        public int BoardIndex { get; set; }
        public int BufferSize { get; set; }

        public ProtocolGPIBSPAConfig()
        {
            BufferSize = 1000000;
            MaxRetries = 4; //default
            buildTargetClassInfo(typeof(ProtocolGPIBSPA));
        }
    }

    public class ProtocolGPIBSPA: ProtocolX
    {
        // GPIB
        protected Device deviceGPIB = null;

        public override string query(string cmd)
        {
            if (_config.bSimulation)
            {
				if( null != _config.mySimData )
				{
					return _config.mySimData.StringData.getSimData();
				}
				else
				{
					return "";
				}
            }

            string strQueryResult = "";
            write(cmd);
            pauseBusy(50);
            strQueryResult = readString(100000);

            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + " : query(" + cmd + ") = " + strQueryResult);
            }

            return strQueryResult;
        }

		public override double queryDouble(string cmd)
		{
			ProtocolGPIBSPAConfig myConfig = (ProtocolGPIBSPAConfig)_config;
			double result = 0.0;

			if( myConfig.bSimulation )
			{
				return myConfig.mySimData.DoubleData.getSimData();
			}

			for( int nTry = 0; nTry < myConfig.MaxRetries; nTry++ )
			{
				try
				{
					result = Convert.ToDouble(query(cmd));
					return result;//This is a good result
				}
				catch( Exception ex )
				{
					//Log exception
					_protocolLog.ErrorException("Failed queryDouble", ex);

					if( nTry == (myConfig.MaxRetries - 1) )
						throw;
				}
			}

			return result;
		}

		public override int queryInt32(string cmd)
		{
			ProtocolGPIBSPAConfig myConfig = (ProtocolGPIBSPAConfig)_config;
			Int32 result = 0;

			if( myConfig.bSimulation )
			{
				return myConfig.mySimData.IntData.getSimData();
			}

			for( int nTry = 0; nTry < myConfig.MaxRetries; nTry++ )
			{
				try
				{
					result = Convert.ToInt32(query(cmd));
					return result;//This is a good result
				}
				catch( Exception ex )
				{
					//Log exception
					_protocolLog.ErrorException("Failed queryInt32", ex);
					if( nTry == (myConfig.MaxRetries - 1) )
						throw;
				}
			}

			return result;
		}

        public override bool write(string cmd)
        {
            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + ".write " + cmd);
            }

			if( _config.bSimulation )
			{
				return true;
			}

            StringBuilder sb = new StringBuilder();
            sb.Append(cmd);
            sb.Append('\n');
            deviceGPIB.Write(sb.ToString());
            return true;
        }

        public override byte[] readByte(int nCount)
        {
            //StringBuilder sb = new StringBuilder();
            //byte[] arBytes = new byte[nCount];
            //byte[] buffer = null;
            //for (int n = 0; n < nCount; n++)
            //{
            //    buffer = deviceGPIB.ReadByteArray(1);
            //    arBytes[n] = buffer[0];
            //}

            return deviceGPIB.ReadByteArray(nCount);
        }

        byte[] _arBytes = new byte[10000];

        public string readString(int nMaxBytes = 100)
        {
			if( _config.bSimulation )
				return "";

            byte[] buffer = null;
            int nSizeRead = 0;

            for (int n = 0; n < nMaxBytes; n++)
            {
                buffer = deviceGPIB.ReadByteArray(1);
                if (buffer[0] == 10)
                {
                    break;
                }
                nSizeRead++;
                _arBytes[n] = buffer[0];
            }

            byte[] arBytesToConvert = new byte[nSizeRead];
            Array.Copy(_arBytes, 0, arBytesToConvert, 0, arBytesToConvert.Length);

            ASCIIEncoding ascii = new ASCIIEncoding();
            String decoded = ascii.GetString(arBytesToConvert);

            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + ".read " + decoded);
            }

            return decoded;
        }

        public override string read()
        {
            if (_config.bSimulation)
                return "";

            string responseString = "";
            //responseString = this.readString(this._defaultBufferSize);
            responseString = deviceGPIB.ReadString();
            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + ".read " + responseString);
            }

            return responseString;
        }

        public override string read(int nBytes)
		{
			if( _config.bSimulation )
				return "";

			string responseString = "";
			responseString = deviceGPIB.ReadString(nBytes);

			if( _config.bVerbose )
			{
				log(this._config.strName + " : " + this.ToString() + ".read " + responseString);
			}

			return responseString;
		}


        public void Reset()
        {
            try
            {
                deviceGPIB.Reset();

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".Reset()");
                }
            }
            catch (Exception ex)
            {
            }
        }

		public void ClearBuffer()
		{
            try
            {
                GpibStatusFlags flag = deviceGPIB.GetCurrentStatus();

                deviceGPIB.Clear();

                if (_config.bVerbose)
                {
                    log(this._config.strName + " : " + this.ToString() + ".ClearBuffer()");
                }
            }
            catch (Exception ex)
            {
                deviceGPIB.Reset();
                pauseBusy(1000);
            }
		}

		public string readStatusByte()
		{
			if( _config.bSimulation )
				return "";

			string status;

            int nStatus = (int)deviceGPIB.SerialPoll();
            status = (nStatus).ToString();

            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + ".readStatusByte() = " + status.ToString());
            }

			return status;
		}

		public void waitForSRQ(double MaxWaitTime_secs = 30)
		{
            DateTime startTime = DateTime.Now;
            TimeSpan ts;
			GpibStatusFlags mask;
			mask = deviceGPIB.GetCurrentStatus();
			while( ((int)(mask) & (int)(GpibStatusFlags.DeviceServiceRequest)) < 1)
			{
                ts = DateTime.Now - startTime;
                if (ts.Seconds > MaxWaitTime_secs) return;
				mask = deviceGPIB.GetCurrentStatus();

                pauseBusy(2150);
			}
		}

        string _strEOS;

        public override bool initialize()
        {
            base.initialize();

            this._defaultBufferSize = 1000000;

            ProtocolGPIBSPAConfig myconfig = (ProtocolGPIBSPAConfig)_config;
            if (!myconfig.bSimulation)
            {
                deviceGPIB = new Device(myconfig.BoardIndex, myconfig.PrimaryAddress);
                deviceGPIB.SerialPollResponseTimeout = TimeoutValue.T30s;
                deviceGPIB.EndOfStringCharacter = 0xA;
                deviceGPIB.SetEndOnWrite = true;
                deviceGPIB.SetEndOnEndOfString = true;
                deviceGPIB.DefaultBufferSize = _defaultBufferSize;
                deviceGPIB.IOTimeout = TimeoutValue.T30s;
                pauseBusy(500);
            }



            byte[] arByte = new byte[1];
            arByte[0] = 0xA;
            _strEOS = System.Text.Encoding.ASCII.GetString(arByte);

            byte[] arDecodeBytes = System.Text.Encoding.ASCII.GetBytes(_strEOS);

            _arBytes = new byte[myconfig.BufferSize];

            return true;
        }

        public override void close()
        {
            if (null != deviceGPIB)
                deviceGPIB.Dispose();
        }

        public ProtocolGPIBSPA(ProtocolGPIBSPAConfig config)
        {
            _config = config;
        }
    }
}
