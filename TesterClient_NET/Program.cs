using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockServices;
using NetFwTypeLib; // Located in FirewallAPI.dll

namespace TesterClient_NET
{
    internal class Program
    {
        // https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        private static void setFirewall()
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
        private static void wshttpClientEndPoint()
        {
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
        private static void netTcpClientEndPoint()
        {
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

        private static InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade _client;
        static void Main(string[] args)
        {

            //InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient obj = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            double a = 0;
            double b = 0;
            double sum = 0;
            int delay = 0;
            bool ret = false;
            string ThreadID = Process.GetCurrentProcess().Id.ToString();

            setFirewall();
            wshttpClientEndPoint();
            //netTcpClientEndPoint();

            try
            {
                do
                {
                    sum = _client.Add(a, b, ThreadID);
                    Console.WriteLine($"\nThread={ThreadID} Add({a}, {b}) = {sum.ToString()}\n");

                    delay = 1; //seconds
                    Console.WriteLine($"Thread={ThreadID} AddAndDelay({a}, {b}) with delay = {delay.ToString()} seconds.");
                    sum = _client.AddAndDelay(a, b, delay, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} AddAndDelay({a}, {b}) = {sum.ToString()} with delay = {delay.ToString()} seconds.\n");

                    ret = _client.getInstrumentLock(sharedInstrument.DCA, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} getInstrumentLock(sharedInstrument.DCA)");

                    // doing something with DCA
                    Console.WriteLine($"Thread={ThreadID} doing something with DCA");
                    Thread.Sleep(10000);

                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} releaseInstrumentLock(sharedInstrument.DCA)\n");

                    ret = _client.getProtocolLock(sharedProtocol.DiCon, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} getProtocolLock(sharedProtocol.DiCon)");

                    // doing something with DiCon
                    Console.WriteLine($"Thread={ThreadID} doing something with DiCon");
                    Thread.Sleep(1000);

                    ret = _client.releaseProtocolLock(sharedProtocol.DiCon, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} releaseProtocolLock(sharedProtocol.DiCon)\n");


                    Console.WriteLine($"Press ENTER to close the console window; other keys to repeat ...........");
                } while (Console.ReadKey().Key != ConsoleKey.Enter);
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
    }
}
