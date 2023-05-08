using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Core
{
	[Serializable]
	public class ProtocolIVIConfig : ProtocolXConfig
	{
		public string Resource { get; set; }

		public ProtocolIVIConfig()
        {
            buildTargetClassInfo(typeof(ProtocolModbus));
        }
	}

	public class ProtocolIVI : ProtocolX
	{
		protected ProtocolIVIConfig _myconfig;
        protected object _lock = new object();

		public ProtocolIVI(ProtocolIVIConfig config)
        {
            _myconfig = config;
        }
		public override bool initialize()
		{
			return true;
		}
	}
}
