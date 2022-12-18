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

namespace InstrumentLockServiceHost_WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //public class InstrumentLockServiceHost_WPF : IInstrumentLockServiceHost
    //{
    //    // https://stackoverflow.com/questions/40591726/can-i-get-set-data-into-wcf-service-by-host
    //    // in order to pass to host and show the client request value at host whenever there is a client request
    //    // an event "RequestFromClient" is invoked within each service method
    //    // the customized event arguments, CustomEventArgs, consisting of ClientRequestValue in the client request are associated with each RequestFromClient event
    //    // a WCF service instance is defined with event handler; such a WCF service instance is used to define a service host 
    //    // once the service host receives such an event, host's event handler will show the values in CustomEventArgs to console or WPF form

    //    /// <summary>
    //    /// for data binding to WPF, will be assigned with the client request values from client in event handler
    //    /// </summary>
    //    public List<ClientRequestValue> _clientRequestValue;
    //    /// <summary>
    //    /// a class global variable of Service Host
    //    /// </summary>
    //    private ServiceHost _host;
    //    /// <summary>
    //    /// a class global variable of WCF service instance to work with event from client.
    //    /// </summary>
    //    private InstrumentLockService.InstrumentLockService _instance;
    //    /// <summary>
    //    /// a varibale to access the WPF DataGrid defined in MainWindow class
    //    /// </summary>
    //    public DataGrid _dgFromMainWindow { get; set; }
    //    /// <summary>
    //    /// a varibale to access the WPF TextBlock defined in MainWindow class
    //    /// </summary>
    //    public TextBlock _tbFromMainWindow { get; set; }


    //    /// <summary>
    //    /// define the global variable of WCF service instance
    //    /// and start the service host of the instance
    //    /// </summary>
    //    //public void initialize(DataGrid dgFromMainWindow, TextBlock tbFromMainWindow)
    //    public void initialize()
    //    {
    //        try
    //        {
    //            // To assign InstrumentLockServiceHost_WPF.dgFromMainWindow = MainWindow.dgServerRequest
    //            //_dgFromMainWindow = dgFromMainWindow;
    //            //_tbFromMainWindow = tbFromMainWindow;
    //            // i.e. pass the WPF DataGrid of the MainWindow class to this InstrumentLockServiceHost_WPF class
    //            _clientRequestValue = new List<ClientRequestValue>();

    //            // define the WCF service instance and event handler
    //            //_instance = new InstrumentLockService.InstrumentLockService();
    //            //_instance.EventFromClient += HandleEventFromClient;

    //            // open ServiceHost of the above defined instance
    //            Uri baseAddress = new Uri("http://localhost:8080/");
    //            //Uri baseAddress = new Uri("net.tcp://localhost:8001/");
    //            //_host = new ServiceHost(typeof(InstrumentLockService.InstrumentLockService), baseAddress);
    //            //_host = new ServiceHost(_instance, baseAddress);


    //            //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
    //            // then host the Facade
    //            _host = new ServiceHost(typeof(InstrumentLockServiceFacade), baseAddress);
    //            // but you can still access the actual service class
    //            var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
    //            serviceInstance.EventFromClient += HandleEventFromClient;

    //            //// In order to use one of the ServiceHost constructors that takes a service instance, the InstanceContextMode of the service must be set to InstanceContextMode.Single.  This can be configured via the ServiceBehaviorAttribute.  Otherwise, please consider using the ServiceHost constructors that take a Type argument.
    //            /// instead of the following two lines, we now define the ServiceBehaviorAttribute before its class defnition in InstrumentLockService.cs
    //            //var behavior = _host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
    //            //behavior.InstanceContextMode = InstanceContextMode.Single;
    //            _host.Open();

    //            // The service can now be accessed.
    //            //MessageBox.Show($"The service is ready at {baseAddress}.", "HOST");
    //            _tbFromMainWindow.Text = $"The service host is ready at {baseAddress}";
    //        }
    //        catch (TimeoutException timeProblem)
    //        {
    //            Console.WriteLine(timeProblem.Message);
    //            Console.ReadLine();
    //            _tbFromMainWindow.Text = timeProblem.Message;
    //        }
    //        catch (CommunicationException commProblem)
    //        {
    //            Console.WriteLine(commProblem.Message);
    //            Console.ReadLine();
    //            _tbFromMainWindow.Text = commProblem.Message;
    //        }
    //        catch (Exception ex)
    //        {
    //            // Logging
    //            Console.WriteLine(ex.Message);
    //            _tbFromMainWindow.Text = ex.Message;
    //        }
    //    }


    //    /// <summary>
    //    /// Host will handle the event from client here
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    public void HandleEventFromClient(object sender, CustomEventArgs e)
    //    {
    //        try
    //        {
    //            // do whatever you want here
    //            // such as print the value on host console or WPF forms
    //            var varValue = e.Value;
    //            // handling the event by adding client request value to the list; and it will trigger WPF to add one more column after refresh
    //            _clientRequestValue.Add(new ClientRequestValue(dInputA: varValue.dInputA, dInputB: varValue.dInputB, dResult: varValue.dResult, sService: varValue.sService, sClient: varValue.sClient, ServiceStart: varValue.ServiceStart, ServiceFinish: varValue.ServiceFinish));
    //            // must refresh; otherwise ItemsSource will not be updated when the corresponding list (such as the above list) is updated
    //            // https://stackoverflow.com/questions/7059070/why-does-the-datagrid-not-update-when-the-itemssource-is-changed
    //            //_dgFromMainWindow.Items.Refresh();
    //            // to avoid the exception of "The calling thread cannot access this object because a different thread owns it" on _dgFromMainWindow
    //            // we will update the WPF UI in UI thread

    //            //_dgFromMainWindow.Items.Refresh();


    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Stop Host and dispose Instance
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        try
    //        {
    //            if (_instance != null)
    //            {
    //                _instance.Dispose();
    //            }
    //            // it takes about 10 sec to close the host
    //            _host.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            // Logging
    //            Console.WriteLine(ex.Message);
    //        }
    //    }

    //}

    public partial class MainWindow : Window
    {
        /// <summary>
        /// a class variable to InstrumentLockServiceHost_WPF
        /// </summary>
        //public InstrumentLockServiceHost_WPF _server { get; set; }

        public MainWindow()
        {
            // InitializeComponent() is auto generated by the corresponsing xaml
            // needs to be placed in front of the rest of codes in MainWindow()
            InitializeComponent();

            // _server = new InstrumentLockServiceHost_WPF();
            //// dgServerRequest is the DataGrid defined in MainWindow.xaml
            //// tbMainWindow is the TextBlock defined in MainWindow.xaml
            //_server._tbFromMainWindow = tbMainWindow;
            //_server._dgFromMainWindow = dgServerRequest;

            //_server.initialize();
            // no need to get a worker thread or task because WCF behavior UseSynchronizationContext = false
            // WCF will get a new thread when a client requests WCF services
            //Task.Factory.StartNew(() => initialize());
            initialize();
            //_server.initialize(dgServerRequest, tbMainWindow);

            //this.Dispatcher.Invoke(() =>
            //{
            //    _server.initialize(dgServerRequest, tbMainWindow);
            //});

            //Thread oThread = new Thread(new ThreadStart(() => _server.initialize(dgServerRequest, tbMainWindow)));
            //// Start the thread
            //oThread.Start();

            // binding the WPF DataGrid to List<ClientRequestValue>
            //dgServerRequest.ItemsSource = _server._clientRequestValue;
            dgServerRequest.ItemsSource = _clientRequestValue;
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
            dgServerRequest.Items.Refresh();

            //// close the WPF form
            //this.Close();

            //// stop service host (take > 10sec) after closing the window
            //_server.Dispose();

        }


        private List<ClientRequestValue> _clientRequestValue;
        private ServiceHost _host;
        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        //public void initialize(DataGrid dgFromMainWindow, TextBlock tbFromMainWindow)
        public void initialize()
        {
            try
            {
                // To assign InstrumentLockServiceHost_WPF.dgFromMainWindow = MainWindow.dgServerRequest
                //_dgFromMainWindow = dgFromMainWindow;
                //_tbFromMainWindow = tbFromMainWindow;
                // i.e. pass the WPF DataGrid of the MainWindow class to this InstrumentLockServiceHost_WPF class
                _clientRequestValue = new List<ClientRequestValue>();

                // define the WCF service instance and event handler
                //_instance = new InstrumentLockService.InstrumentLockService();
                //_instance.EventFromClient += HandleEventFromClient;

                // open ServiceHost of the above defined instance
                Uri baseAddress = new Uri("http://localhost:8080/");
                //Uri baseAddress = new Uri("net.tcp://localhost:8001/");
                //_host = new ServiceHost(typeof(InstrumentLockService.InstrumentLockService), baseAddress);
                //_host = new ServiceHost(_instance, baseAddress);


                //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
                // then host the Facade
                _host = new ServiceHost(typeof(InstrumentLockServiceFacade), baseAddress);
                // but you can still access the actual service class
                var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
                serviceInstance.EventFromClient += HandleEventFromClient;

                //// In order to use one of the ServiceHost constructors that takes a service instance, the InstanceContextMode of the service must be set to InstanceContextMode.Single.  This can be configured via the ServiceBehaviorAttribute.  Otherwise, please consider using the ServiceHost constructors that take a Type argument.
                /// instead of the following two lines, we now define the ServiceBehaviorAttribute before its class defnition in InstrumentLockService.cs
                //var behavior = _host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                //behavior.InstanceContextMode = InstanceContextMode.Single;
                _host.Open();

                // The service can now be accessed.
                //MessageBox.Show($"The service is ready at {baseAddress}.", "HOST");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbMainWindow.Text = $"The service host is ready at {baseAddress}";
                }), DispatcherPriority.Background);
                
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
                Console.ReadLine();
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbMainWindow.Text = timeProblem.Message;
                }), DispatcherPriority.Background);
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
                Console.ReadLine();
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbMainWindow.Text = commProblem.Message;
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
               
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbMainWindow.Text = ex.Message;
                }), DispatcherPriority.Background);
            }
        }


        /// <summary>
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            try
            {
                // do whatever you want here
                // such as print the value on host console or WPF forms
                var varValue = e.Value;
                // handling the event by adding client request value to the list; and it will trigger WPF to add one more column after refresh
                _clientRequestValue.Add(new ClientRequestValue(dInputA: varValue.dInputA, dInputB: varValue.dInputB, dResult: varValue.dResult, sService: varValue.sService, sClient: varValue.sClient, ServiceStart: varValue.ServiceStart, ServiceFinish: varValue.ServiceFinish));
                // must refresh; otherwise ItemsSource will not be updated when the corresponding list (such as the above list) is updated
                // https://stackoverflow.com/questions/7059070/why-does-the-datagrid-not-update-when-the-itemssource-is-changed
                //_dgFromMainWindow.Items.Refresh();
                // to avoid the exception of "The calling thread cannot access this object because a different thread owns it" on _dgFromMainWindow
                // we will update the WPF UI in UI thread


                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //dgServerRequest.ItemsSource = _clientRequestValue;
                    dgServerRequest.Items.Refresh();
                }), DispatcherPriority.Background);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbMainWindow.Text = ex.Message;
                }), DispatcherPriority.Background);
            }
        }
    }
}
