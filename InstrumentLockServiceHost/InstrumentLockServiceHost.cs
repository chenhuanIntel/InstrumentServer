using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentLockServices;
using NetFwTypeLib; // Located in FirewallAPI.dll
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using InstrumentsLib.Tools.Instruments.InstrumentServer;

// namespace name uses plural
namespace InstrumentLockServiceHosts
{
    public class InstrumentLockServiceHost: IInstrumentLockServiceHost
    {
        // https://stackoverflow.com/questions/40591726/can-i-get-set-data-into-wcf-service-by-host
        // in order to pass to host and show the client request value at host whenever there is a client request
        // an event "RequestFromClient" is invoked within each service method
        // the customized event arguments, CustomEventArgs, consisting of ClientRequestValue in the client request are associated with each RequestFromClient event
        // a WCF service instance is defined with event handler; such a WCF service instance is used to define a service host 
        // once the service host receives such an event, host's event handler will show the values in CustomEventArgs to console or WPF form

        /// <summary>
        /// a class global variable of Service Host
        /// </summary>
        public ServiceHost _host;
        /// <summary>
        /// a class global variable of Base Address
        /// </summary>
        public Uri _baseAddress;

        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        public virtual void initialize(Uri baseAddress)
        {
            try
            {
                // programmably set endpoint, either wsHTTP or netTCP
                //wsHttpEndPoint(baseAddress);
                netTcpEndPoint(baseAddress);

                // in InstrumentLockServiceFacade, we can still access the actual service class
                var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
                serviceInstance.EventFromClient += HandleEventFromClient;

                // The service can now be accessed.
                Console.WriteLine($"The service is ready at {_baseAddress}");
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
                Console.ReadLine();
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Stop Host and dispose Instance
        /// </summary>
        public void Dispose()
        {
            try
            {
                _host.Close();
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            // do whatever you want here
            // such as print the value on host console or WPF forms
            var varValue = e.Value;
            Console.WriteLine($"WCF client machine= {varValue.sMachineName}.");
            Console.WriteLine($"WCF client Thread= {varValue.sThreadID}.");
            Console.WriteLine($"\tinput A = {varValue.dInputA}.");
            Console.WriteLine($"\tinput B = {varValue.dInputB}.");
            Console.WriteLine($"\tservice = {varValue.sService}.");
            Console.WriteLine($"\tresult Sum = {varValue.dResult}.");
            Console.WriteLine($"\tServiceStart = {varValue.ServiceStart}.");
            Console.WriteLine($"\tServiceFinish = {varValue.ServiceFinish}.");
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        /// programmably to set firewall rules
        /// </summary>
        public void setFirewall(InstrumentServer_Config serverConfig)
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
            // first to remove the rule with the same name
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == serverConfig.strFirewallInRuleName).FirstOrDefault();
            //if (firewallRule != null) //need to first check if rule exists
            //    firewallPolicy.Rules.Remove(firewallRule.Name);
            INetFwRule firewallInRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallInRule.Name = serverConfig.strFirewallInRuleName;
            firewallInRule.Description = serverConfig.strFirewallInRuleDescription;
            firewallInRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallInRule.LocalPorts = serverConfig.strFirewallInRuleLocalPorts;
            firewallInRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallInRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallInRule.Enabled = true;
            firewallPolicy.Rules.Add(firewallInRule);

            // add outbound rules for TCP ports
            // first to remove the rule with the same name
            firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == serverConfig.strFirewallOutRuleName).FirstOrDefault();
            //if (firewallRule != null) need to first check if rule exists
            //    firewallPolicy.Rules.Remove(firewallRule.Name);
            INetFwRule firewallOutRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallOutRule.Name = serverConfig.strFirewallOutRuleName;
            firewallOutRule.Description = serverConfig.strFirewallOutRuleDescription;
            firewallOutRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallOutRule.LocalPorts = serverConfig.strFirewallOutRuleLocalPorts;
            firewallOutRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallOutRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallOutRule.Enabled = true;
            firewallPolicy.Rules.Add(firewallOutRule);
        }


        /// <summary>
        /// this function should only be used if not reading server station config
        /// https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        /// programmably set firewall rules, identical to that of server side
        /// </summary>
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
        ///  programmably to set up endpoint for clients
        ///  this method is to use wshttp
        /// </summary>
        public void wsHttpEndPoint(Uri baseAddress)
        {
            // wsHttp
            _baseAddress = baseAddress;  

            //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
            // instead of using Singleton _instance of InstrumentLockService,
            // we develop InstrumentLockServiceFacade to allow WCF Per Call behavior by using the Type, not an instance
            _host = new ServiceHost(typeof(InstrumentLockServiceFacade), _baseAddress);

            // Check to see if the service host already has a ServiceMetadataBehavior
            ServiceMetadataBehavior smb = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
                smb = new ServiceMetadataBehavior();
            // To avoid disclosing metadata information, set the values below to false before deployment
            smb.HttpGetEnabled = true; // when using net tcp binding, HttpGetEnabled need to be false
            smb.HttpsGetEnabled = true;
            _host.Description.Behaviors.Add(smb);

            // You can't actually ADD a ServiceDebugBehavior to a ServiceHost, what you have to do is modify the existing ServiceDebugBehavior (was having the same issue)
            // https://stackoverflow.com/questions/21443347/how-to-programatically-add-servicedebug-behavior-with-endpoint
            _host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

            //Add MEX metadata endpoint
            _host.AddServiceEndpoint(
                ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexHttpBinding(),
                "mex"
            );
            // Add application endpoint
            WSHttpBinding wsHttpBinding = new WSHttpBinding();
            _host.AddServiceEndpoint(typeof(IInstrumentLockServiceFacade), wsHttpBinding, _baseAddress);

            _host.Open();
        }

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        ///  programmably to set up endpoint for clients
        ///  this method is to use TCP
        /// </summary>
        public void netTcpEndPoint(Uri baseAddress)
        {
            // net.tcp
            _baseAddress = baseAddress;

            //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
            // instead of using Singleton _instance of InstrumentLockService,
            // we develop InstrumentLockServiceFacade to allow WCF Per Call behavior by using the Type, not an instance
            _host = new ServiceHost(typeof(InstrumentLockServiceFacade), _baseAddress);

            // Check to see if the service host already has a ServiceMetadataBehavior
            ServiceMetadataBehavior smb = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
                smb = new ServiceMetadataBehavior();
            // To avoid disclosing metadata information, set the values below to false before deployment
            smb.HttpGetEnabled = false; // when using net tcp binding, HttpGetEnabled need to be false
            smb.HttpsGetEnabled = false;
            _host.Description.Behaviors.Add(smb);

            // You can't actually ADD a ServiceDebugBehavior to a ServiceHost, what you have to do is modify the existing ServiceDebugBehavior (was having the same issue)
            // https://stackoverflow.com/questions/21443347/how-to-programatically-add-servicedebug-behavior-with-endpoint
            _host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

            //Add MEX endpoint
            _host.AddServiceEndpoint(
              ServiceMetadataBehavior.MexContractName,
              MetadataExchangeBindings.CreateMexTcpBinding(),
              "mex"
            );

            // Add application endpoint
            NetTcpBinding netTCPbinding = new NetTcpBinding();
            _host.AddServiceEndpoint(typeof(IInstrumentLockServiceFacade), netTCPbinding, _baseAddress);

            _host.Open();
        }
    }
}
