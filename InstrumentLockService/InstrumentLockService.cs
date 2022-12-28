using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using NetFwTypeLib; // Located in FirewallAPI.dll

// namespace name uses plural
namespace InstrumentLockServices
{
    public enum sharedInstrument
    {
        DCA
    }
    public enum sharedProtocol
    {
        DiCon
    }

    /// <summary>
    /// this clientFunctions class contains all the functions that are common to all clients
    /// such as firewall rules, endpoint setup etc
    /// </summary>
    public class clientFunctions
    {
        public IInstrumentLockServiceFacade _iClient;

        /// <summary>
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
        /// </summary>
        public void wshttpClientEndPoint()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");
            var myBinding = new WSHttpBinding();
            var myEndpoint = new EndpointAddress(baseAddress);
            var myChannelFactory = new ChannelFactory<IInstrumentLockServiceFacade>(myBinding, myEndpoint);

            try
            {
                _iClient = myChannelFactory.CreateChannel();
                //((ICommunicationObject)_client).Close();
                //myChannelFactory.Close();
            }
            catch
            {
                (_iClient as ICommunicationObject)?.Abort();
            }
        }

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        /// </summary>
        public void netTcpClientEndPoint()
        {
            Uri baseAddress = new Uri("net.tcp://localhost:8001/");

            var myBinding = new NetTcpBinding();
            var myEndpoint = new EndpointAddress(baseAddress);
            var myChannelFactory = new ChannelFactory<IInstrumentLockServiceFacade>(myBinding, myEndpoint);

            try
            {
                _iClient = myChannelFactory.CreateChannel();
                //((ICommunicationObject)_client).Close();
                //myChannelFactory.Close();
            }
            catch
            {
                (_iClient as ICommunicationObject)?.Abort();
            }
        }
    }

    // https://stackoverflow.com/questions/40591726/can-i-get-set-data-into-wcf-service-by-host
    // in order to pass to host and show the client request value at host whenever there is a client request
    // an event "RequestFromClient" is invoked within each service method
    // the customized event arguments, CustomEventArgs, consisting of ClientRequestValue in the client request are associated with each RequestFromClient event
    // a WCF service instance is defined with event handler; such a WCF service instance is used to define a service host 
    // once the service host receives such an event, host's event handler will show the values in CustomEventArgs to console or WPF form

    /// <summary>
    ///  define the client request values that are sent to host for showing at host console or WPF form
    /// </summary>
    public class ClientRequestValue
    {
        public double dInputA { get; set; }
        public double dInputB { get; set; }
        public int delayInSec { get; set; }
        public double dResult { get; set; }
        public string sService { get; set; }
        public string sThreadID { get; set; }
        public string sMachineName { get; set; }
        public DateTime ServiceStart { get; set; }
        public DateTime ServiceFinish { get; set; }

        public ClientRequestValue(double dInputA, double dInputB, int delayInSec, double dResult, string sService, string sThreadID, string sMachineName, DateTime ServiceStart, DateTime ServiceFinish)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.delayInSec = delayInSec;
            this.dResult = dResult;
            this.sService = sService;
            this.sThreadID = sThreadID;
            this.sMachineName = sMachineName;
            this.ServiceStart = ServiceStart;
            this.ServiceFinish = ServiceFinish;
        }
    }

    /// <summary>
    /// Create event args that can pass the values from service to host
    /// </summary>
    public class CustomEventArgs : EventArgs
    {
        public ClientRequestValue Value { get; set; }

        public CustomEventArgs(ClientRequestValue value)
        {
            Value = value;
        }
    }


    // https://stackoverflow.com/questions/3469044/self-hosted-wcf-service-how-to-access-the-objects-implementing-the-service-co
    // dummy Facade
    // PerCall creates a new instance for each operation.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    public class InstrumentLockServiceFacade : IInstrumentLockServiceFacade
    {
        public static InstrumentLockService _serviceInstance = new InstrumentLockService();

        public static InstrumentLockService ServiceInstance { get { return _serviceInstance; } }

        /// <summary>
        /// WCF service will fire EventFromClient togerther with the values sent from WCF client
        /// </summary>
        public event EventHandler<CustomEventArgs> EventFromClient;

        public double Add(double a, double b, string sThreadID, string sMachineName)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";

            sum=_serviceInstance.Add(a, b, sThreadID, sMachineName);
            if (sum != -999)
            {
                sService = "InstrumentLockService.Add";
            }

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(dInputA:a, dInputB:b, delayInSec:0, dResult:sum, sService:sService, sThreadID:sThreadID, sMachineName:sMachineName, ServiceStart:ServiceStart, ServiceFinish:ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        public double AddAndDelay(double a, double b, int delayInSec, string sThreadID, string sMachineName)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1); 
            string sService = "InstrumentLockService.null";

            sum = _serviceInstance.AddAndDelay(a, b, delayInSec, sThreadID, sMachineName);
            if (sum != -999)
            {
                sService = "InstrumentLockService.AddAndDelay";
            }

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(dInputA: a, dInputB: b, delayInSec: 0, dResult: sum, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        public void getConnectedInfo()
        {
            _serviceInstance.getConnectedInfo();
        }

        public bool getInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName)
        {
            return _serviceInstance.getInstrumentLock(instr, sThreadID, sMachineName);
        }

        public bool releaseInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName)
        {
            return _serviceInstance.releaseInstrumentLock(instr, sThreadID, sMachineName);
        }

        public bool getProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            return _serviceInstance.getProtocolLock(protocol, sThreadID, sMachineName);
        }

        public bool releaseProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            return _serviceInstance.releaseProtocolLock(protocol, sThreadID, sMachineName);
        }

        public int intDivide(double a, double b, string sThreadID, string sMachineName)
        {
            return _serviceInstance.intDivide(a, b, sThreadID, sMachineName);
        }

    }

    public class SemaphoreOwner
    {
        public Semaphore mySemaphore { get; set; }
        public string sMachineName { get; set; }
        public string sThreadID { get; set; }
        public SemaphoreOwner(Semaphore mySemaphore)
        {
            this.mySemaphore = mySemaphore;
            sMachineName = null;
            sThreadID = null;
        }
    }

    // Enable one of the following instance modes to compare instancing behaviors.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]

    // PerCall creates a new instance for each operation.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]

    // Singleton creates a single instance for application lifetime.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class InstrumentLockService : IInstrumentLockService
    {
        // Create a new Mutex. The creating thread does not own the mutex.
        private static Mutex mutexLockAdd = new Mutex();
        private static Mutex mutexLockAddAndDelay = new Mutex();
        private static Mutex mutexLockintDivide = new Mutex();
        private static Semaphore semaphoreDCA = new Semaphore(initialCount: 1, maximumCount: 1);
        private static SemaphoreOwner ownerSemaphoreDCA = new SemaphoreOwner(semaphoreDCA);
        private static Semaphore semaphoreDiCon = new Semaphore(initialCount: 1, maximumCount: 1);
        private static SemaphoreOwner ownerSemaphoreDiCon = new SemaphoreOwner(semaphoreDiCon);

        /// <summary>
        /// WCF service will fire EventFromClient togerther with the values sent from WCF client
        /// </summary>
        public event EventHandler<CustomEventArgs> EventFromClient;

        // ===========================================================================================================
        // the followning are the implementations of service contracts defined in the interface IInstrumentLockService
        // ===========================================================================================================

        /// <summary>
        /// demo/try-out method between client and server
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Add(double a, double b, string sThreadID, string sMachineName)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1); 
            string sService = "InstrumentLockService.null";

            if (mutexLockAdd.WaitOne())
            {
                Console.WriteLine("InstrumentLockService.Add");
                sService = "InstrumentLockService.Add";
                sum = a + b;
                // Release the Mutex.
                mutexLockAdd.ReleaseMutex();
            }
            else
            {
                Console.WriteLine($"{sThreadID} will not acquire the mutex");
            }

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(dInputA: a, dInputB: b, delayInSec: 0, dResult: sum, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        /// <summary>
        /// demo/try-out method between client and server
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double AddAndDelay(double a, double b, int delayInSec, string sThreadID, string sMachineName)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";

            if (mutexLockAddAndDelay.WaitOne())
            {
                Console.WriteLine($"{sThreadID} InstrumentLockService.AddAndDelay");
                sService = "InstrumentLockService.AddAndDelay";
                sum = a + b;

                // delay to hold the mutex
                Thread.Sleep(delayInSec * 1000);
                // Release the Mutex.
                mutexLockAddAndDelay.ReleaseMutex();
            }
            else
            {
                Console.WriteLine($"{sThreadID} will not acquire the mutex");
            }      
            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(dInputA: a, dInputB: b, delayInSec: delayInSec, dResult: sum, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        /// <summary>
        /// demo/try-out method between client and server
        /// Arithmetic operations with the float and double types never throw an exception.
        /// The result of arithmetic operations with those types can be one of special values that represent infinity and not-a-number:
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int intDivide(double a, double b, string sThreadID, string sMachineName)
        {
            int intDiv = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish;
            string sService = "InstrumentLockService.null";
            if (mutexLockintDivide.WaitOne(10))
            {
                try
                {
                    Console.WriteLine("InstrumentLockService.intDivide");
                    sService = "InstrumentLockService.intDivide";
                    intDiv = (int)a / (int)b;
                    // Release the Mutex.
                    mutexLockintDivide.ReleaseMutex();
                }
                catch (DivideByZeroException e)
                {
                    // Release the Mutex.
                    mutexLockintDivide.ReleaseMutex();
                    Console.WriteLine("exception=", e);
                    ServiceFinish = DateTime.Now;
                    // contruct the client request values to be sent to host
                    sService = sService + e.Message;
                    var valueEx = new ClientRequestValue(dInputA: a, dInputB: b, delayInSec: 0, dResult: a / b, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                    // each WCF service fires the event EventFromClient with the values from WCF client
                    EventFromClient?.Invoke(this, new CustomEventArgs(valueEx));
                    MathFault mf = new MathFault();
                    mf.Operation = "division";
                    mf.ProblemType = "divide by zero";
                    throw new FaultException<MathFault>(mf, new FaultReason("Divide_By_Zero_Exception"));
                }
            }
            else
            {
                Console.WriteLine($"{sThreadID} will not acquire the mutex");
            }
            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(dInputA: a, dInputB: b, delayInSec: 0, dResult: intDiv, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return intDiv;
        }

        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        public bool getInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";

            try
            {
                // check if the client already owns the semaphore
                // if not the owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
                if (!(ownerSemaphoreDCA.sThreadID == sThreadID && ownerSemaphoreDCA.sMachineName == sMachineName))          
                    semaphoreDCA.WaitOne();

                // if the client already owns the semaphore or just obtains via WaitOne() 
                // first to re-assign the ownership
                ownerSemaphoreDCA.sThreadID = sThreadID;
                ownerSemaphoreDCA.sMachineName = sMachineName;
                // then grant the InstrumentLock
                Console.WriteLine("InstrumentLockService.getInstrumentLock");
                sService = "InstrumentLockService.getInstrumentLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
            }
            catch (Exception ex)
            {
                ret = false;
                // Logging
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        public bool releaseInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
      
            try
            {
                // first to reset the ownership to null
                ownerSemaphoreDCA.sThreadID = null;
                ownerSemaphoreDCA.sMachineName = null;
                // then release the semaphore
                semaphoreDCA.Release();

                Console.WriteLine("InstrumentLockService.releaseInstrumentLock");
                sService = "InstrumentLockService.releaseInstrumentLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
            }
            catch (Exception ex)
            {
                ret=false;
                // Logging
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        public bool getProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
     
            try
            {
                // check if the client already owns the semaphore
                // if not the owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
                if (!(ownerSemaphoreDiCon.sThreadID == sThreadID && ownerSemaphoreDiCon.sMachineName == sMachineName))
                    semaphoreDiCon.WaitOne();

                // if the client already owns the semaphore or just obtains via WaitOne() 
                // first to re-assign the ownership
                ownerSemaphoreDiCon.sThreadID = sThreadID;
                ownerSemaphoreDiCon.sMachineName = sMachineName;
                // then grant the InstrumentLock
                Console.WriteLine("InstrumentLockService.getProtocolLock");
                sService = "InstrumentLockService.getProtocolLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
            }
            catch (Exception ex)
            {
                ret = false;
                // Logging
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        public bool releaseProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
          
            try
            {
                // first to reset the ownership to null
                ownerSemaphoreDiCon.sThreadID = null;
                ownerSemaphoreDiCon.sMachineName = null;
                // then release the semaphore
                semaphoreDiCon.Release();

                Console.WriteLine("InstrumentLockService.releaseProtocolLock");
                sService = "InstrumentLockService.releaseProtocolLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
            }
            catch (Exception ex)
            {
                ret=false;
                // Logging
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// getConnectedInfo()
        /// Returned info such as (SPAN_Station1_Slot1, DCA1)
        /// </summary>
        public void getConnectedInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Dispose method performs all object cleanup
        /// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        public void Dispose()
        {
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

    }
}
