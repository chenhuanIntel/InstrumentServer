using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstrumentLockService;
using InstrumentLockServiceHost;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;
using System.Reflection.Emit;
using NetFwTypeLib; // Located in FirewallAPI.dll

namespace InstrumentLockServiceHost_WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class InstrumentLockServiceHost_WPF : IInstrumentLockServiceHost
    {
        // https://stackoverflow.com/questions/40591726/can-i-get-set-data-into-wcf-service-by-host
        // in order to pass to host and show the client request value at host whenever there is a client request
        // an event "RequestFromClient" is invoked within each service method
        // the customized event arguments, CustomEventArgs, consisting of ClientRequestValue in the client request are associated with each RequestFromClient event
        // a WCF service instance is defined with event handler; such a WCF service instance is used to define a service host 
        // once the service host receives such an event, host's event handler will show the values in CustomEventArgs to console or WPF form

        /// <summary>
        /// for data binding to WPF, will be assigned with the client request values from client in event handler
        /// </summary>
        public List<ClientRequestValue> _clientRequestValue;
        /// <summary>
        /// a class global variable of Service Host
        /// </summary>
        private ServiceHost _host;
        /// <summary>
        /// a class global variable of WCF service instance to work with event from client.
        /// </summary>
        private InstrumentLockService.InstrumentLockService _instance;
        /// <summary>
        /// a varibale to access the WPF DataGrid defined in MainWindow class
        /// </summary>
        public DataGrid _dgFromMainWindow { get; set; }
        /// <summary>
        /// a varibale to access the WPF TextBlock defined in MainWindow class
        /// </summary>
        public TextBlock _tbFromMainWindow { get; set; }
        /// <summary>
        /// a class variable to MainWindow Dispatcher
        /// </summary>
        public Dispatcher _dpFromMainWindow { get; set; }

        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        //public void initialize(DataGrid dgFromMainWindow, TextBlock tbFromMainWindow)
        public void initialize()
        {
            try
            {
                // in MainWindow(), the Dispatcher and WPF DataGrid/TextBlock of the MainWindow class are assigned to this InstrumentLockServiceHost_WPF class
                // _dpFromMainWindow, _dgFromMainWindow, _tbFromMainWindow

                // init _clientRequestValue and baseAddress
                _clientRequestValue = new List<ClientRequestValue>();
                Uri baseAddress = new Uri("http://localhost:8080/");
                //Uri baseAddress = new Uri("net.tcp://localhost:8001/");

                //// if define the following Singleton _instance, then WCF behavior is limited to Single which is not what we want
                //// that is, in order to use one of the ServiceHost constructors that takes a service instance, the InstanceContextMode of the service must be set to InstanceContextMode.Single.
                //// define the WCF service instance and event handler
                //_instance = new InstrumentLockService.InstrumentLockService();
                //_instance.EventFromClient += HandleEventFromClient;
                //// open ServiceHost of the above defined instance
                //_host = new ServiceHost(_instance, baseAddress);

                //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
                // instead of using Singleton _instance of InstrumentLockService,
                // we develop InstrumentLockServiceFacade to allow WCF Per Call behavior by using the Type, not an instance
                _host = new ServiceHost(typeof(InstrumentLockServiceFacade), baseAddress);
                _host.Open();
                // in InstrumentLockServiceFacade, we can still access the actual service class
                var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
                serviceInstance.EventFromClient += HandleEventFromClient;

                // The service can now be accessed.
                //MessageBox.Show($"The service is ready at {baseAddress}.", "HOST");
                _tbFromMainWindow.Text = $"The service host is ready at {baseAddress}";
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
                Console.ReadLine();
                _tbFromMainWindow.Text = timeProblem.Message;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
                Console.ReadLine();
                _tbFromMainWindow.Text = commProblem.Message;
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
                _tbFromMainWindow.Text = ex.Message;
            }
        }


        /// <summary>
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            try
            {
                // do whatever an evant handler is supposed to do
                // such as print the value on host console or WPF forms
                var varValue = e.Value;
                // handling the event by adding client request value to the list; and it will trigger WPF to add one more column after refresh
                _clientRequestValue.Add(new ClientRequestValue(dInputA: varValue.dInputA, dInputB: varValue.dInputB, dResult: varValue.dResult, sService: varValue.sService, sClient: varValue.sClient, ServiceStart: varValue.ServiceStart, ServiceFinish: varValue.ServiceFinish));
                // must refresh; otherwise ItemsSource will not be updated when the corresponding list (such as the above list) is updated
                // https://stackoverflow.com/questions/7059070/why-does-the-datagrid-not-update-when-the-itemssource-is-changed
                // to avoid the exception of "The calling thread cannot access this object because a different thread owns it" on _dgFromMainWindow
                // we use Dispatcher.BeginInvoke() to update the WPF UI in UI thread at this WCF service thread which is generated by the WCF service behavior
                // https://www.c-sharpcorner.com/article/dispatcher-in-a-single-threaded-wpf-app/
                _dpFromMainWindow.BeginInvoke(new Action(() =>
                {
                    _dgFromMainWindow.Items.Refresh();
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
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
                if (_instance != null)
                {
                    _instance.Dispose();
                }
                // it takes about 10 sec to close the host
                _host.Close();
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
            }
        }

    }

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
            // first to remove the rule with the same name
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "WCF_inbound_rule").FirstOrDefault();
            if (firewallRule != null)
                firewallPolicy.Rules.Remove(firewallRule.Name);
            INetFwRule firewallInRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallInRule.Name = "WCF_inbound_rule";
            firewallInRule.Description = "WCF service port inbound rule";
            firewallInRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallInRule.LocalPorts = "8001";
            firewallInRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallInRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallInRule.Enabled = true;
            firewallPolicy.Rules.Add(firewallInRule);

            // add outbound rules for TCP ports
            // first to remove the rule with the same name
            firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "WCF_outbound_rule").FirstOrDefault();
            if (firewallRule != null)
                firewallPolicy.Rules.Remove(firewallRule.Name);
            INetFwRule firewallOutRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallOutRule.Name = "WCF_outbound_rule";
            firewallOutRule.Description = "WCF service port outbound rule";
            firewallOutRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallOutRule.LocalPorts = "8001";
            firewallOutRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallOutRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallOutRule.Enabled = true;
            firewallPolicy.Rules.Add(firewallOutRule);
        }

        /// <summary>
        /// a class variable to InstrumentLockServiceHost_WPF
        /// </summary>
        public InstrumentLockServiceHost_WPF _server { get; set; }

        public MainWindow()
        {
            setFirewall();

            // InitializeComponent() is auto generated by the corresponsing xaml
            // needs to be placed in front of the rest of codes in MainWindow()
            InitializeComponent();

            _server = new InstrumentLockServiceHost_WPF();
            // dgServerRequest is the DataGrid defined in MainWindow.xaml
            // tbMainWindow is the TextBlock defined in MainWindow.xaml
            // assign all the class variables of InstrumentLockServiceHost_WPF to the variables of MainWindow
            // that is, to assign InstrumentLockServiceHost_WPF.dgFromMainWindow = MainWindow.dgServerRequest, etc
            // i.e. pass the WPF DataGrid of the MainWindow class to this InstrumentLockServiceHost_WPF class
            _server._tbFromMainWindow = tbMainWindow;
            _server._dgFromMainWindow = dgServerRequest;
            _server._dpFromMainWindow = Dispatcher.CurrentDispatcher;
            // no need to get a worker thread or task because WCF behavior UseSynchronizationContext = false
            // WCF will get a new thread when a client requests WCF services
            _server.initialize();

            // binding the WPF DataGrid to List<ClientRequestValue>
            dgServerRequest.ItemsSource = _server._clientRequestValue;
        }


        /// <summary>
        /// to close the WPF window and stop the host
        /// because stopping host take much longer time (almost 10 sec), we will close the window first then close the host.
        /// Otherwise, user may think that clicking the button does not close the host and window --- only because closing host takes rather long before the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // close the WPF form
            this.Close();

            // stop service host (take > 10sec) after closing the window
            _server.Dispose();
        }

    }
}
