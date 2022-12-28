using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using InstrumentLockServices;
using NetFwTypeLib; // Located in FirewallAPI.dll

namespace TesterClient_WPF
{
    public class Service
    {
        public static string Add => "Add";
        public static string AddAndDelay => "AddAndDelay";
        public static string IntDivide => "IntDivide";
        public static string getInstrumentLock => "getInstrumentLock";
        public static string releaseInstrumentLock => "releaseInstrumentLock";
        public static string getProtocolLock => "getProtocolLock";
        public static string releaseProtocolLock => "releaseProtocolLock";
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // to avoid the error "Unable to cast transparent proxy to type" in which 
        // _client was defined as InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade
        // should be              InstrumentLockServices.IInstrumentLockServiceFacade, or simply IInstrumentLockServiceFacade
        private static IInstrumentLockServiceFacade _client;
        private static clientFunctions _clientFct = new clientFunctions();
        public MainWindow()
        {
            // auto-generated via xaml
            InitializeComponent();

            // use client function class to set up firewall rules
            _clientFct.setFirewall();

            // programmably set up EndPoint
            _clientFct.wshttpClientEndPoint();
            //_clientFct.netTcpClientEndPoint();

            // assign the _iClient obtained within the above Endpoint functions
            _client = _clientFct._iClient;

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
                if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.Add))
                {
                    double sum = _client.Add(a, b, ThreadID);
                    tbOut.Text = $"Add({a}, {b}) = {sum.ToString()}";
                }
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.AddAndDelay))
                {
                    double sum = _client.AddAndDelay(a, b, delay, ThreadID);
                    tbOut.Text = $"Add({a}, {b}) = {sum.ToString()}";
                }
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.IntDivide))
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
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.getInstrumentLock))
                {
                    bool ret = _client.getInstrumentLock(sharedInstrument.DCA, ThreadID);
                    tbOut.Text = $"getInstrumentLock(sharedInstrument.DCA)";
                }
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.releaseInstrumentLock))
                {
                    bool ret = _client.releaseInstrumentLock(sharedInstrument.DCA, ThreadID);
                    tbOut.Text = $"releaseInstrumentLock(sharedInstrument.DCA)";
                }
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.getProtocolLock))
                {
                    bool ret = _client.getProtocolLock(sharedProtocol.DiCon, ThreadID);
                    tbOut.Text = $"getProtocolLock(sharedProtocol.DiCon)";
                }
                else if (cmbService.SelectedItem == (Object)typeof(Service).GetProperty(Service.releaseProtocolLock))
                {
                    bool ret = _client.releaseProtocolLock(sharedProtocol.DiCon, ThreadID);
                    tbOut.Text = $"releaseProtocolLock(sharedProtocol.DiCon)";
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
