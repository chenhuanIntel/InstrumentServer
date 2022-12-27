using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib; // Located in FirewallAPI.dll
using System.ServiceModel;

namespace InstrumentLockServiceShared
{
    public class InstrumentLockServiceShared: IInstrumentLockServiceShared
    {
        // https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        public void setFirewall()
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            //// add application
            //INetFwRule firewallAppRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            //firewallAppRule.Name = "WCF_Tester_Client";
            //firewallAppRule.Description = "Allow WCF Tester Client thru firewall";
            //firewallAppRule.ApplicationName = @"C:\project\chenhuan\InstrumentServer\TesterClient_WPF\bin\Debug\TesterClient_WPF.exe";
            //firewallAppRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            //firewallAppRule.Enabled = true;
            //firewallPolicy.Rules.Add(firewallAppRule);

            // add inbound rules for TCP ports
            // first to check if there is a rule with the same name
            // https://stackoverflow.com/questions/37226050/how-to-check-if-firewall-rule-existed
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "WCF_inbound_rule_FG_SPAN").FirstOrDefault();
            if (firewallRule == null)
            {
                INetFwRule firewallInRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallInRule.Name = "WCF_inbound_rule_FG_SPAN";
                firewallInRule.Description = "WCF service port inbound rule";
                firewallInRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallInRule.LocalPorts = "8001";
                firewallInRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                firewallInRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallInRule.Enabled = true;
                firewallPolicy.Rules.Add(firewallInRule);
            }

            // add outbound rules for TCP ports
            // first to check if there is a rule with the same name
            firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "WCF_outbound_rule_FG_SPAN").FirstOrDefault();
            if (firewallRule == null)
            {
                INetFwRule firewallOutRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallOutRule.Name = "WCF_outbound_rule_FG_SPAN";
                firewallOutRule.Description = "WCF service port outbound rule";
                firewallOutRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallOutRule.LocalPorts = "8001";
                firewallOutRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                firewallOutRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallOutRule.Enabled = true;
                firewallPolicy.Rules.Add(firewallOutRule);
            }
        }

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        /// </summary>
        public void wshttpClientEndPoint()
        {
            //_client = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            // wsHttp
            Uri baseAddress = new Uri("http://localhost:8080/");
            var myBinding = new WSHttpBinding();
            var myEndpoint = new EndpointAddress(baseAddress);
            var myChannelFactory = new ChannelFactory<InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade>(myBinding, myEndpoint);

            try
            {
                _client = myChannelFactory.CreateChannel();
                //((ICommunicationObject)_client).Close();
                //myChannelFactory.Close();
            }
            catch
            {
                (_client as ICommunicationObject)?.Abort();
            }
        }

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        /// </summary>
        public void netTcpClientEndPoint()
        {
            //_client = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            // net.tcp
            Uri baseAddress = new Uri("net.tcp://localhost:8001/");

            var myBinding = new NetTcpBinding();
            var myEndpoint = new EndpointAddress(baseAddress);
            var myChannelFactory = new ChannelFactory<InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade>(myBinding, myEndpoint);

            try
            {
                _client = myChannelFactory.CreateChannel();
                //((ICommunicationObject)_client).Close();
                //myChannelFactory.Close();
            }
            catch
            {
                (_client as ICommunicationObject)?.Abort();
            }
        }
    }

  
}
