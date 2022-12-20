using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstrumentLockService;
using NetFwTypeLib; // Located in FirewallAPI.dll

namespace TesterClient_WPF
{
    public class Service
    {
        public static string Add => "Add";
        public static string AddAndDelay => "AddAndDelay";
        public static string IntDivide => "IntDivide";
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        private InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade _client;
        public MainWindow()
        {
            setFirewall();
            InitializeComponent();

            //_client = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            // wsHttp
            Uri baseAddress = new Uri("http://localhost:8080/");
            // net.tcp
            //Uri baseAddress = new Uri("net.tcp://localhost:8001/");

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

            // cmbService is defined in xaml
            cmbService.ItemsSource = typeof(Service).GetProperties();
        }

        private void Request_Service_Button_Click(object sender, RoutedEventArgs e)
        {
            //InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient obj = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            double a = double.Parse(aTextBox.Text);
            double b = double.Parse(bTextBox.Text);
            int delay = Int32.Parse(delayTextBox.Text);
            string ThreadID = Process.GetCurrentProcess().Id.ToString();


            try
            {
                if (cmbService.SelectedItem == typeof(Service).GetProperty(Service.Add))
                {
                    double sum = _client.Add(a, b, ThreadID);
                    tbOut.Text = $"Add({a}, {b}) = {sum.ToString()}";
                }
                else if (cmbService.SelectedItem == typeof(Service).GetProperty(Service.AddAndDelay))
                {
                    double sum = _client.AddAndDelay(a, b, delay, ThreadID);
                    tbOut.Text = $"Add({a}, {b}) = {sum.ToString()}";
                }
                else if (cmbService.SelectedItem == typeof(Service).GetProperty(Service.IntDivide))
                {
                    try
                    {
                        tbOut.Text = $"intDivide({a}, {b})";
                        int div0 = _client.intDivide(a, b, ThreadID);
                        tbOut.Text = tbOut.Text + $" = {div0.ToString()}";
                    }
                    catch (FaultException<MathFault> error)
                    {
                        tbOut.Text = tbOut.Text + "\n" + $"FaultException<MathFault>: Math fault while doing " + error.Detail.Operation + ". Problem: " + error.Detail.ProblemType;
                        //_client.Abort();
                    }
                }
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
                Console.ReadLine();
                tbOut.Text = timeProblem.Message;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
                Console.ReadLine();
                tbOut.Text = commProblem.Message;
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
                tbOut.Text = ex.Message;
            }
        }
    }
}
