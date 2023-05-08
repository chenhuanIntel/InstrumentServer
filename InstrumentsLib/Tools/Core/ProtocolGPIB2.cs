using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.VisaNS;

namespace InstrumentsLib.Tools.Core
{
    [Serializable]
    public class ProtocolGPIB2Config : ProtocolXConfig
    {
        public string Resource { get; set; }
        public int nTimeout { get; set; }

        public ProtocolGPIB2Config()
        {
            MaxRetries = 4; //default
            buildTargetClassInfo(typeof(ProtocolGPIB2));
        }
    }

    public class ProtocolGPIB2: ProtocolX
    {
        // GPIB
        protected GpibSession deviceGPIB = null;

        public override string query(string cmd)
        {
            if (_config.bSimulation)
            {
                return _config.mySimData.StringData.getSimData();
            }

            string strQueryResult = "";
            write(cmd);
            strQueryResult = read();

            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + " : query(" + cmd + ") = " + strQueryResult);
            }

            return strQueryResult;
        }

		public override double queryDouble(string cmd)
		{
			ProtocolGPIBConfig myConfig = (ProtocolGPIBConfig)_config;
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
			ProtocolGPIBConfig myConfig = (ProtocolGPIBConfig)_config;
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

            deviceGPIB.Write(cmd);
            return true;
        }

        public override string read()
        {
			if( _config.bSimulation )
				return "";

            string responseString = "";
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


		public void ClearBuffer()
		{
			deviceGPIB.Clear();

            if (_config.bVerbose)
            {
                log(this._config.strName + " : " + this.ToString() + ".ClearBuffer()");
            }
		}

		public string readStatusByte()
		{
			if( _config.bSimulation )
				return "";

			string status;

			StatusByteFlags flag = deviceGPIB.ReadStatusByte();
            status = ((int)flag).ToString();

            //deviceGPIB.

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
			StatusByteFlags mask = deviceGPIB.ReadStatusByte();
			while( ((int)(mask) & (int)(StatusByteFlags.MessageAvailable)) < 1 )
			{
                ts = DateTime.Now - startTime;
                if (ts.Seconds > MaxWaitTime_secs) return;
				mask = deviceGPIB.ReadStatusByte();
			}
		}

		ProtocolGPIB2Config _myConfig;

        public override bool initialize()
        {
            base.initialize();

			if( !_myConfig.bSimulation )
            {
                deviceGPIB = new GpibSession(_myConfig.Resource);
                deviceGPIB.Timeout = _myConfig.nTimeout;
            }
            return true;
        }

        public override void close()
        {
            if (null != deviceGPIB)
                deviceGPIB.Dispose();
        }

        public ProtocolGPIB2(ProtocolGPIBConfig config)
        {
            _config = config;
        }
    }
}
