using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockService;

namespace InstrumentLockServiceHost_NET
{
    internal class Program
    {
        // https://stackoverflow.com/questions/40591726/can-i-get-set-data-into-wcf-service-by-host
        // in order to pass to host and show the client request value at host whenever there is a client request
        // an event "RequestFromClient" is invoked within each service method
        // the customized event arguments, CustomEventArgs, consisting of ClientRequestValue in the client request are associated with each RequestFromClient event
        // a WCF service instance is defined with event handler; such a WCF service instance is used to define a service host 
        // once the service host receives such an event, host's event handler will show the values in CustomEventArgs to console or WPF form

        /// <summary>
        /// a global variable of Service Host
        /// </summary>
        private ServiceHost _host;
        /// <summary>
        /// a global variable of WCF service instance to work with event from client.
        /// </summary>
        private static InstrumentLockService.InstrumentLockService _instance;

        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        /// <param name="args"></param>
        protected void initialize(string[] args)
        {
            try
            {
                // define the WCF service instance and event handler
                _instance = new InstrumentLockService.InstrumentLockService();
                _instance.EventFromClient += HandleEventFromClient;

                // open ServiceHost of the above defined instance
                Uri baseAddress = new Uri("http://localhost:8080/");
                _host = new ServiceHost(_instance, baseAddress);
                //// In order to use one of the ServiceHost constructors that takes a service instance, the InstanceContextMode of the service must be set to InstanceContextMode.Single.  This can be configured via the ServiceBehaviorAttribute.  Otherwise, please consider using the ServiceHost constructors that take a Type argument.
                /// instead of the following two lines, we now define the ServiceBehaviorAttribute before its class defnition in InstrumentLockService.cs
                //var behavior = _host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                //behavior.InstanceContextMode = InstanceContextMode.Single;
                _host.Open();
                // The service can now be accessed.
                Console.WriteLine($"The service is ready at {baseAddress}");
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
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            // do whatever you want here
            // such as print the value on host console or WPF forms
            var varValue = e.Value;
            string userName = null;
            // https://stackoverflow.com/questions/7312224/accessing-wcf-client-identity-on-service
            if (ServiceSecurityContext.Current != null && ServiceSecurityContext.Current.PrimaryIdentity != null)
            {
                userName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            }
            Console.WriteLine($"WCF client userName = {userName}.");
            Console.WriteLine($"\tinput A = {varValue.dInputA}.");
            Console.WriteLine($"\tinput B = {varValue.dInputB}.");
            Console.WriteLine($"\tservice = {varValue.sService}.");
            Console.WriteLine($"\tresult Sum = {varValue.dResult}.");
            Console.WriteLine($"\tCheckOutTime = {varValue.CheckOutTime}.");
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
                _host.Close();
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// console app Main()
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var myProgram = new Program();
            myProgram.initialize(args);

            // press ENTER to terminate
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();

            myProgram.Dispose();
        }
    }
}
