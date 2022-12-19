using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockService;
using InstrumentLockServiceHost;

namespace InstrumentLockServiceHost_NET
{
    public class InstrumentLockServiceHostConsole : IInstrumentLockServiceHost
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
        //private static InstrumentLockService.InstrumentLockService _instance;

        /// <summary>
        /// Stop Host and dispose Instance
        /// </summary>
        public void Dispose()
        {
            try
            {
                //if (_instance != null)
                //{
                //    _instance.Dispose();
                //}
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
        public void HandleEventFromClient(object sender, CustomEventArgs e)
        {
            // do whatever you want here
            // such as print the value on host console or WPF forms
            var varValue = e.Value;
            Console.WriteLine($"WCF client thread = {varValue.sClient}.");
            Console.WriteLine($"\tinput A = {varValue.dInputA}.");
            Console.WriteLine($"\tinput B = {varValue.dInputB}.");
            Console.WriteLine($"\tservice = {varValue.sService}.");
            Console.WriteLine($"\tresult Sum = {varValue.dResult}.");
            Console.WriteLine($"\tServiceStart = {varValue.ServiceStart}.");
            Console.WriteLine($"\tServiceFinish = {varValue.ServiceFinish}.");
        }

        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        public void initialize()
        {
            try
            {
                Uri baseAddress = new Uri("http://localhost:8080/");

                //// if define the following Singleton _instance, then WCF behavior is limited to Single which is not what we want
                //// that is, in order to use one of the ServiceHost constructors that takes a service instance, the InstanceContextMode of the service must be set to InstanceContextMode.Single.
                //// define the WCF service instance and event handler
                //_instance = new InstrumentLockService.InstrumentLockService();
                //_instance.EventFromClient += HandleEventFromClient;
                //// open ServiceHost of the above defined instance
                //_host = new ServiceHost(_instance, baseAddress);
                //_host = new ServiceHost(typeof(InstrumentLockService.InstrumentLockService), baseAddress);


                //https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
                // instead of using Singleton _instance of InstrumentLockService,
                // we develop InstrumentLockServiceFacade to allow WCF Per Call behavior by using the Type, not an instance
                _host = new ServiceHost(typeof(InstrumentLockServiceFacade), baseAddress);
                _host.Open();
                // in InstrumentLockServiceFacade, we can still access the actual service class
                var serviceInstance = InstrumentLockServiceFacade.ServiceInstance;
                serviceInstance.EventFromClient += HandleEventFromClient;

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
    }
    internal class Program
    {
        /// <summary>
        /// console app Main()
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var myHost = new InstrumentLockServiceHostConsole();
            myHost.initialize();

            // press ENTER to terminate
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();

            myHost.Dispose();
        }
    }
}
