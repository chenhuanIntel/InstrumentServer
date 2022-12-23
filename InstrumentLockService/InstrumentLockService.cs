using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;

namespace InstrumentLockService
{
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
        public string sClient { get; set; }
        public DateTime ServiceStart { get; set; }
        public DateTime ServiceFinish { get; set; }

        public ClientRequestValue(double dInputA, double dInputB, double dResult, string sService, DateTime ServiceStart, DateTime ServiceFinish)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;     
            this.dResult = dResult;
            this.sService = sService;
            this.ServiceStart = ServiceStart;
            this.ServiceFinish = ServiceFinish;
        }
        public ClientRequestValue(double dInputA, double dInputB, double dResult, string sService, string sClient, DateTime ServiceStart, DateTime ServiceFinish)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.dResult = dResult;
            this.sService = sService;
            this.sClient = sClient;
            this.ServiceStart = ServiceStart;
            this.ServiceFinish = ServiceFinish;
        }
        public ClientRequestValue(double dInputA, double dInputB, int delayInSec, double dResult, string sService, DateTime ServiceStart, DateTime ServiceFinish)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.delayInSec = delayInSec;
            this.dResult = dResult;
            this.sService = sService;
            this.ServiceStart = ServiceStart;
            this.ServiceFinish = ServiceFinish;
        }
        public ClientRequestValue(double dInputA, double dInputB, int delayInSec, double dResult, string sService, string sClient, DateTime ServiceStart, DateTime ServiceFinish)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.delayInSec = delayInSec;
            this.dResult = dResult;
            this.sService = sService;
            this.sClient = sClient;
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

        public double Add(double a, double b, string ThreadID)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            sum=_serviceInstance.Add(a, b, ThreadID);
            if (sum != -999)
            {
                sService = "InstrumentLockService.Add";
            }

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, sum, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        public double AddAndDelay(double a, double b, int delayInSec, string ThreadID)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1); 
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            sum = _serviceInstance.AddAndDelay(a, b, delayInSec, ThreadID);
            if (sum != -999)
            {
                sService = "InstrumentLockService.AddAndDelay";
            }

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, sum, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return sum;
        }

        public void getConnectedInfo()
        {
            _serviceInstance.getConnectedInfo();
        }

        public bool getInstrumentLock(sharedInstrument instr, string ThreadID)
        {
            return _serviceInstance.getInstrumentLock(instr, ThreadID);
        }

        public bool releaseInstrumentLock(sharedInstrument instr, string ThreadID)
        {
            return _serviceInstance.releaseInstrumentLock(instr, ThreadID);
        }

        public bool getProtocolLock(sharedProtocol protocol, string ThreadID)
        {
            return _serviceInstance.getProtocolLock(protocol, ThreadID);
        }

        public bool releaseProtocolLock(sharedProtocol protocol, string ThreadID)
        {
            return _serviceInstance.releaseProtocolLock(protocol, ThreadID);
        }

        public int intDivide(double a, double b, string ThreadID)
        {
            return _serviceInstance.intDivide(a, b, ThreadID);
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
        private static Semaphore semaphoreDiCon = new Semaphore(initialCount: 1, maximumCount: 1);

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
        public double Add(double a, double b, string ThreadID)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1); 
            string sClient = ThreadID;
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
                Console.WriteLine($"{sClient} will not acquire the mutex");
            }


            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, sum, sService, sClient, ServiceStart, ServiceFinish);
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
        public double AddAndDelay(double a, double b, int delayInSec, string ThreadID)
        {
            double sum = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            if (mutexLockAddAndDelay.WaitOne())
            {
                Console.WriteLine($"{ThreadID} InstrumentLockService.AddAndDelay");
                sService = "InstrumentLockService.AddAndDelay";
                sum = a + b;

                //// send an event before holding the resource
                //// contruct the client request values to be sent to host
                //var value1 = new ClientRequestValue(a, b, delayInSec, -888, sService, sClient, ServiceStart, ServiceFinish);
                //// each WCF service fires the event EventFromClient with the values from WCF client
                //EventFromClient?.Invoke(this, new CustomEventArgs(value1));

                // delay to hold the mutex
                Thread.Sleep(delayInSec * 1000);
                // Release the Mutex.
                mutexLockAddAndDelay.ReleaseMutex();
            }
            else
            {
                Console.WriteLine($"{sClient} will not acquire the mutex");
            }      
            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, delayInSec, sum, sService, sClient, ServiceStart, ServiceFinish);
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
        public int intDivide(double a, double b, string ThreadID)
        {
            int intDiv = -999;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish;
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";
            if (mutexLockintDivide.WaitOne(10))
            {
                try
                {
                    Console.WriteLine("InstrumentLockService.intDivide");
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
                    var valueEx = new ClientRequestValue(a, b, a / b, "InstrumentLockService.intDivide" + " " + e.Message, sClient, ServiceStart, ServiceFinish);
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
                Console.WriteLine($"{sClient} will not acquire the mutex");
            }
            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, intDiv, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return intDiv;
        }

        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        public bool getInstrumentLock(sharedInstrument instr, string ThreadID)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            semaphoreDCA.WaitOne();

            Console.WriteLine("InstrumentLockService.getInstrumentLock");
            sService = "InstrumentLockService.getInstrumentLock";
            ret = true;

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(0, 0, 0, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return ret;
        }

        public bool releaseInstrumentLock(sharedInstrument instr, string ThreadID)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            semaphoreDCA.Release();

            Console.WriteLine("InstrumentLockService.releaseInstrumentLock");
            sService = "InstrumentLockService.releaseInstrumentLock";
            ret = true;

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(0, 0, 0, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return ret;
        }

        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        public bool getProtocolLock(sharedProtocol protocol, string ThreadID)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            semaphoreDiCon.WaitOne();

            Console.WriteLine("InstrumentLockService.getProtocolLock");
            sService = "InstrumentLockService.getProtocolLock";
            ret = true;

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(0, 0, 0, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
            return ret;
        }

        public bool releaseProtocolLock(sharedProtocol protocol, string ThreadID)
        {
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sClient = ThreadID;
            string sService = "InstrumentLockService.null";

            semaphoreDiCon.Release();

            Console.WriteLine("InstrumentLockService.releaseProtocolLock");
            sService = "InstrumentLockService.releaseProtocolLock";
            ret = true;

            ServiceFinish = DateTime.Now;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(0, 0, 0, sService, sClient, ServiceStart, ServiceFinish);
            // each WCF service fires the event EventFromClient with the values from WCF client
            EventFromClient?.Invoke(this, new CustomEventArgs(value));
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
