using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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

namespace TesterClient_WPF
{
    public class Service
    {
        public static string Add => "Add";
        public static string IntDivide => "IntDivide";
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // cmbService is defined in xaml
            cmbService.ItemsSource = typeof(Service).GetProperties();
        }

        private void Request_Service_Button_Click(object sender, RoutedEventArgs e)
        {
            InstrumentLockServiceClient.InstrumentLockServiceClient obj = new InstrumentLockServiceClient.InstrumentLockServiceClient();
            double a = double.Parse(aTextBox.Text);
            double b = double.Parse(bTextBox.Text);

            try
            {
                if (cmbService.SelectedItem == typeof(Service).GetProperty(Service.Add))
                {
                    double sum = obj.Add(a, b);
                    tbOut.Text = $"Add({a}, {b}) = {sum.ToString()}";
                }
                else if (cmbService.SelectedItem == typeof(Service).GetProperty(Service.IntDivide))
                {
                    try
                    {
                        tbOut.Text = $"intDivide({a}, {b})";
                        int div0 = obj.intDivide(a, b);
                        tbOut.Text = tbOut.Text + $" = {div0.ToString()}";
                    }
                    catch (FaultException<MathFault> error)
                    {
                        tbOut.Text = tbOut.Text + "\n" + $"FaultException<MathFault>: Math fault while doing " + error.Detail.Operation + ". Problem: " + error.Detail.ProblemType;
                        obj.Abort();
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
