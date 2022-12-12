using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
        public double dResult { get; set; }
        public string sService { get; set; }
        public string sClient { get; set; }
        public DateTime CheckOutTime { get; set; }

        public ClientRequestValue(double dInputA, double dInputB, double dResult, string sService, DateTime CheckOutTime)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.dResult = dResult;
            this.sService = sService;
            this.CheckOutTime = CheckOutTime;
        }
        public ClientRequestValue(double dInputA, double dInputB, double dResult, string sService, string sClient, DateTime CheckOutTime)
        {
            this.dInputA = dInputA;
            this.dInputB = dInputB;
            this.dResult = dResult;
            this.sService = sService;
            this.sClient = sClient;
            this.CheckOutTime = CheckOutTime;
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

    // Enable one of the following instance modes to compare instancing behaviors.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]

    // PerCall creates a new instance for each operation.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]

    // Singleton creates a single instance for application lifetime.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class InstrumentLockService : IInstrumentLockService
    {
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
        public double Add(double a, double b)
        {
            Console.WriteLine("InstrumentLockService.Add");
            double sum = a + b;
            // contruct the client request values to be sent to host
            var value = new ClientRequestValue(a, b, sum, "InstrumentLockService.Add", DateTime.Now);
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
        public int intDivide(double a, double b)
        {
            try
            {
                Console.WriteLine("service intDivide");
                int intDiv = (int)a / (int)b;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(a, b, intDiv, "InstrumentLockService.intDivide", DateTime.Now);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
                return intDiv;
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine("exception=", e);
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(a, b, a/b, "InstrumentLockService.intDivide" + " " + e.Message, DateTime.Now);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
                MathFault mf = new MathFault();
                mf.Operation = "division";
                mf.ProblemType = "divide by zero";
                throw new FaultException<MathFault>(mf, new FaultReason("Divide_By_Zero_Exception"));
            }
        }

        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        public void getInstrumentLock()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        public void getProtocolLock()
        {
            throw new NotImplementedException();
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
