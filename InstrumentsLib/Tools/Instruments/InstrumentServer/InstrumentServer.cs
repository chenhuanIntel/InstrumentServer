using InstrumentsLib.Tools.Core;
using System;
using System.Runtime.Serialization;

namespace InstrumentsLib.Tools.Instruments.InstrumentServer
{
    [DataContract]
    /// <summary>
    /// Interface of InstrumentServer
    /// </summary>
    public class InstrumentServer_Config : InstrumentXConfig
    {
        [DataMember]
        /// <summary>
        /// ServerAddress
        /// </summary>
        public string strServerAddress { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallInRuleName
        /// </summary>
        public string strFirewallInRuleName { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallInRuleDescription
        /// </summary>
        public string strFirewallInRuleDescription { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallInRuleDescription
        /// </summary>
        public string strFirewallInRuleLocalPorts { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallOutRuleName
        /// </summary>
        public string strFirewallOutRuleName { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallOutRuleDescription
        /// </summary>
        public string strFirewallOutRuleDescription { get; set; }
        [DataMember]
        /// <summary>
        /// FirewallOutRuleDescription
        /// </summary>
        public string strFirewallOutRuleLocalPorts { get; set; }

        public InstrumentServer_Config()
        {
            buildTargetClassInfo(typeof(InstrumentServer));
        }
    }

    public class InstrumentServer : InstrumentX
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public InstrumentServer(InstrumentServer_Config config)
            : base(config)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="protocol"></param>
        public InstrumentServer(InstrumentServer_Config config, ProtocolX protocol)
            : base(config, protocol)
        {
        }
    }
}
