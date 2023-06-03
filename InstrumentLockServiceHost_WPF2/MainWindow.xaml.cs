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
using InstrumentLockServices;
using InstrumentLockServiceHosts;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;
using System.Reflection.Emit;
using NetFwTypeLib; // Located in FirewallAPI.dll
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using InstrumentsLib;
using Newtonsoft.Json;
using InstrumentsLib.Tools.Instruments.Oscilloscope;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using InstrumentsLib.Tools.Core;
using InstrumentsLib.Tools.Instruments.InstrumentServer;

namespace InstrumentLockServiceHosts_WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class InstrumentLockServiceHost_WPF : InstrumentLockServiceHost
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
        public ObservableCollection<ClientRequestValue> _clientRequestValue;
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

        public Object _itemsLockMainWindow { get; set; }

        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        //public void initialize(DataGrid dgFromMainWindow, TextBlock tbFromMainWindow)
        public override void initialize(Uri baseAddress)
        {
            try
            {
                // in MainWindow(), the Dispatcher and WPF DataGrid/TextBlock of the MainWindow class are assigned to this InstrumentLockServiceHost_WPF class
                // _dpFromMainWindow, _dgFromMainWindow, _tbFromMainWindow

                // init _clientRequestValue and baseAddress
                _clientRequestValue = new ObservableCollection<ClientRequestValue>();

                //wsHttpEndPoint(baseAddress);
                netTcpEndPoint(baseAddress);

                // in InstrumentLockServiceFacade, we can still access the actual service class
                var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
                serviceInstance.EventFromClient += HandleEventFromClient;

                // The service can now be accessed.
                //MessageBox.Show($"The service is ready at {baseAddress}.", "HOST");
                _tbFromMainWindow.Text = $"The service host is ready at {_baseAddress}";
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
        public override void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            try
            {
                // do whatever an evant handler is supposed to do
                // such as print the value on host console or WPF forms
                var varValue = e.Value;
                // handling the event by adding client request value to the list; and it will trigger WPF to add one more column after refresh
                // https://stackoverflow.com/questions/21720638/using-bindingoperations-enablecollectionsynchronization
                // a syn-lock is placed to synchronize multiple (clients) threads to access the server to display the activities
                lock (_itemsLockMainWindow)
                {
                    // Once locked, you can manipulate the collection safely from another thread
                    _clientRequestValue.Add(new ClientRequestValue(dInputA: varValue.dInputA, dInputB: varValue.dInputB, delayInSec: 0, dResult: varValue.dResult, sService: varValue.sService, sThreadID: varValue.sThreadID, sMachineName: varValue.sMachineName, ServiceStart: varValue.ServiceStart, ServiceFinish: varValue.ServiceFinish));
                }
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

    }

    public partial class MainWindow : Window
    {
        /// <summary>
        /// a class variable to InstrumentLockServiceHost_WPF
        /// </summary>
        public InstrumentLockServiceHost_WPF _server { get; set; }

        //lock object for synchronization;
        private static object _itemsLock = new object();
        /// <summary>
        /// Station instance for TEC temperature indicator
        /// </summary>
        protected StationHardware _stationInstance;

        /// <summary>
        /// arInstrConfig
        /// </summary>
        /// <param name="arInstrConfig"></param>
        /// <returns></returns>
        protected InstrumentServer_Config getServerConfig(List<InstrumentXConfig> arInstrConfig)
        {
            InstrumentServer_Config serverConfig = null;
            foreach (var instrument in arInstrConfig)
            {
                serverConfig = JsonConvert.DeserializeObject<InstrumentServer_Config>(JsonConvert.SerializeObject(instrument));
                if (serverConfig.strServerAddress != null)
                    return serverConfig;
            }
            return null;
        }
        protected void CreateServer()
        {
            // InitializeComponent() is auto generated by the corresponsing xaml
            // needs to be placed in front of the rest of codes in MainWindow()
            InitializeComponent();

            // initialize station config file
            //Get station instance
            _stationInstance = StationHardware.Instance();
            _stationInstance.Initialize();

            // after reading station config and initialize station, let's build DCA queue
            buildDCAQueue();

            //ITraceWriter traceWriter = new MemoryTraceWriter();
            //var temp = JsonConvert.SerializeObject(_stationInstance.myConfig.arInstConfig[0], new JsonSerializerSettings { TraceWriter = traceWriter, Converters = { new JavaScriptDateTimeConverter() } });
            //Console.WriteLine(traceWriter);
            //ITraceWriter traceWriter1 = new MemoryTraceWriter();
            //var temp1 = JsonConvert.DeserializeObject<InstrumentLockServices.WCFScopeConfig>(temp, new JsonSerializerSettings { TraceWriter = traceWriter1, Converters = { new JavaScriptDateTimeConverter() } });
            //Console.WriteLine(traceWriter1);
            //ITraceWriter traceWriter2 = new MemoryTraceWriter();
            //var temp2 = JsonConvert.DeserializeObject<InstrumentsLib.Tools.Instruments.Oscilloscope.DCA_A86100CFlex400GConfig>(temp, new JsonSerializerSettings { TraceWriter = traceWriter2, Converters = { new JavaScriptDateTimeConverter() } });
            //Console.WriteLine(traceWriter2);
            //ITraceWriter traceWriter3 = new MemoryTraceWriter();
            //var temp3 = JsonConvert.SerializeObject(temp1, new JsonSerializerSettings { TraceWriter = traceWriter3, Converters = { new JavaScriptDateTimeConverter() } });
            //Console.WriteLine(traceWriter);
            //ITraceWriter traceWriter4 = new MemoryTraceWriter();
            //var temp4 = JsonConvert.DeserializeObject<InstrumentsLib.Tools.Instruments.Oscilloscope.DCA_A86100CFlex400GConfig>(temp, new JsonSerializerSettings { TraceWriter = traceWriter4, Converters = { new JavaScriptDateTimeConverter() } });
            //// cannot deserialized to ScopeConfig which is an abstract class
            ////var temp5 = JsonConvert.DeserializeObject<InstrumentsLib.Tools.Instruments.Oscilloscope.ScopeConfig>(temp, new JsonSerializerSettings { TraceWriter = traceWriter4, Converters = { new JavaScriptDateTimeConverter() } });
            //Console.WriteLine(traceWriter4);

            // initialize server
            _server = new InstrumentLockServiceHost_WPF();
            _server.setFirewall(getServerConfig(_stationInstance.myConfig.arInstConfig));
            // init/assign all class variables of _server            
            // if to assign a value, assigning all the class variables of _server(InstrumentLockServiceHost_WPF) with the values of MainWindow's variables
            // that is, to assign InstrumentLockServiceHost_WPF.dgFromMainWindow = MainWindow.dgServerRequest, etc
            // i.e. pass the WPF DataGrid of the MainWindow class to this InstrumentLockServiceHost_WPF class
            _server._clientRequestValue = new ObservableCollection<ClientRequestValue>();
            _server._tbFromMainWindow = tbMainWindow; // tbMainWindow is the TextBlock defined in MainWindow.xaml
            _server._dgFromMainWindow = dgServerRequest; // dgServerRequest is the DataGrid defined in MainWindow.xaml
            _server._dpFromMainWindow = Dispatcher.CurrentDispatcher;
            _server._itemsLockMainWindow = _itemsLock; // synchronization lock defined above
            // no need to get a worker thread or task because WCF behavior UseSynchronizationContext = false
            // WCF will get a new thread when a client requests WCF services
            string strServerAddress = getServerConfig(_stationInstance.myConfig.arInstConfig).strServerAddress;
            Uri baseAddress = new Uri(strServerAddress);
            //Uri baseAddress = new Uri("net.tcp://172.25.93.250:8001/");
            //Uri baseAddress = new Uri("http://172.25.93.250:8080/");
            _server.initialize(baseAddress);

            // https://stackoverflow.com/questions/21720638/using-bindingoperations-enablecollectionsynchronization
            //Enable the cross acces to this collection elsewhere
            BindingOperations.EnableCollectionSynchronization(_server._clientRequestValue, _itemsLock);

            // binding the WPF DataGrid to List<ClientRequestValue>
            dgServerRequest.ItemsSource = _server._clientRequestValue;
        }

        public MainWindow()
        {
            CreateServer();
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

        private void dgServerRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private static IInstrumentLockServiceFacade _client;
        private static clientFunctions _clientFct = new clientFunctions();
        private void ResetDCAQueue()
        {
            InstrumentLockService.dictDCAQueue.Clear();
            InstrumentLockService.dictDCAQueue = null;
        }
        /// <summary>
        /// during the creation of server, to build a queue of all DCAs and their corresponding Protocols in the server station config file
        /// </summary>
        private void buildDCAQueue()
        {
            InstrumentLockService clientService= new InstrumentLockService();
            clientService.buildDCAandProtocolQueue();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // close the current server first, then create a new one
            // do not close the WPH form

            // stop service host (take > 10sec) after closing the window
            _server.Dispose();

            // reset DCAQueue
            ResetDCAQueue();
            // re-create server
            CreateServer();
        }
    }
}
