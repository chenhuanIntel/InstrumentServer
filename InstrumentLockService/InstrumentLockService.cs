﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using NetFwTypeLib; // Located in FirewallAPI.dll
using InstrumentsLib;
using InstrumentsLib.Tools.Instruments.Switch;
using InstrumentsLib.Tools.Instruments.Oscilloscope;
using System.Diagnostics;
using Utility;
using Newtonsoft.Json;
using InstrumentsLib.Tools.Core;
using System.CodeDom;

// namespace name uses plural
namespace InstrumentLockServices
{
    /// <summary>
    /// DCAQueue contains a semaophore with counts = number of elements in lstDCAQueueElement
    /// </summary>
    public class DCAQueue
    {
        public Semaphore semaphoreDCA { get; set; } // contain a count = number of DCA of the same channel number
        public List<DCAQueueElement> lstDCAQueueElement { get; set; }
    }
    public class DCAQueueElement
    {
        public WCFScopeConfig DCA { get; set; }
        public WCFProtocolXConfig Protocol { get; set; }
        public SemaphoreOwner ownerSemaphoreDCA { get; set; }
        
        public DCAQueueElement(WCFScopeConfig DCAConfig, WCFProtocolXConfig ProtocolConfig)
        {
            DCA = new WCFScopeConfig();
            DCA = DCAConfig;
            Protocol = new WCFProtocolXConfig();
            Protocol = ProtocolConfig;
            ownerSemaphoreDCA= new SemaphoreOwner();
        }
    }

    public class DiConQueue
    {
        public Semaphore semaphoreDiCon { get; set; } 
        public SemaphoreOwner ownerSemaphoreDiCon { get; set; }

        public DiConQueue()
        {
            semaphoreDiCon = new Semaphore(initialCount: 1, maximumCount: 1);
            ownerSemaphoreDiCon = new SemaphoreOwner();
        }
    }

    public enum sharedProtocol
    {
        DiCon
    }

    public enum sharedInstrument
    {
        DCA
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
        public void wshttpClientEndPoint(Uri baseAddress, TimeSpan TimeOutSpan)
        {
            try
            {
                var myBinding = new WSHttpBinding();
                myBinding.SendTimeout = TimeOutSpan;
                myBinding.ReceiveTimeout = TimeOutSpan;
                myBinding.OpenTimeout = TimeOutSpan;

                var myEndpoint = new EndpointAddress(baseAddress);
                var myChannelFactory = new ChannelFactory<IInstrumentLockServiceFacade>(myBinding, myEndpoint);
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
        public void netTcpClientEndPoint(Uri baseAddress, TimeSpan TimeOutSpan)
        {
            try
            {
                var myBinding = new NetTcpBinding();
                // to avoid net.tcp OperationTimeout
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/dc4a6bdc-4dd7-4e68-b24d-cd83a3bfece5/nettcp-operationtimeout-question?forum=wcf
                myBinding.SendTimeout = TimeOutSpan;
                myBinding.ReceiveTimeout = TimeOutSpan;
                myBinding.ReliableSession.InactivityTimeout = TimeOutSpan;

                var myEndpoint = new EndpointAddress(baseAddress);
                var myChannelFactory = new ChannelFactory<IInstrumentLockServiceFacade>(myBinding, myEndpoint);

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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
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

        public bool getInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName, int nChannelInEachMeasurementGroup)
        {
            return _serviceInstance.getInstrumentLock(instr, sThreadID, sMachineName, nChannelInEachMeasurementGroup);
        }
        public bool getInstrumentLockWithReturn(sharedInstrument instr, string sThreadID, string sMachineName, ref WCFScopeConfig DCA, ref WCFProtocolXConfig Protocol, int nChannelInEachMeasurementGroup)
        {
            return _serviceInstance.getInstrumentLockWithReturn(instr, sThreadID, sMachineName, ref DCA, ref Protocol, nChannelInEachMeasurementGroup);
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
        public int nestedCount { get; set; }
        public string sMachineName { get; set; }
        public string sThreadID { get; set; }
        public SemaphoreOwner()
        {
            nestedCount = 0;
            sMachineName = null;
            sThreadID = null;
        }
    }


    // Enable one of the following instance modes to compare instancing behaviors: per session, per call or single
    // InstanceContextMode = PerCall     A new InstanceContext object is created prior to and recycled subsequent to each call.
    // InstanceContextMode = PerSession  A new InstanceContext object is created for each session. If the channel does not create a session this value behaves as if it were PerCall.
    // InstanceContextMode = Single      Only one InstanceContext object is used for all incoming calls and is not recycled subsequent to the calls. If a service object does not exist, one is created.
    // ConcurrencyMode = Multiple        The service instance is multi-threaded. No synchronization guarantees are made. Because other threads can change your service object at any time, you must handle synchronization and state consistency at all times.
    // ConcurrencyMode = Reentrant       The service instance is single-threaded and accepts reentrant calls. The reentrant service accepts calls when you call another service; it is therefore your responsibility to leave your object state consistent before callouts and you must confirm that operation-local data is valid after callouts. Note that the service instance is unlocked only by calling another service over a WCF channel. In this case, the called service can reenter the first service via a callback. If the first service is not reentrant, the sequence of calls results in a deadlock. For details, see ConcurrencyMode.
    // ConcurrencyMode = Single          The service instance is single-threaded and does not accept reentrant calls. If the InstanceContextMode property is Single, and additional messages arrive while the instance services a call, these messages must wait until the service is available or until the messages time out.
    // UseSynchronizationContext = false will generate a new work thread for the service, regardless if the previous thread is completed or not
    // UseSynchronizationContext = true  will wait for the previous service to finish and use the same thread
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)] 
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class InstrumentLockService : IInstrumentLockService
    {
        // key = number of DCA channels
        private static Dictionary<int, DCAQueue> _dictDCAQueue = new Dictionary<int, DCAQueue>();   
        public static Dictionary<int, DCAQueue> dictDCAQueue
        {
            get
            {
                return _dictDCAQueue;
            }
            set
            {
                _dictDCAQueue = value;
            }
        }

        // Create a new Mutex. The creating thread does not own the mutex.
        private static Mutex mutexLockAdd = new Mutex();
        private static Mutex mutexLockAddAndDelay = new Mutex();
        private static Mutex mutexLockintDivide = new Mutex();
        //private static Semaphore semaphoreDCA;// = new Semaphore(initialCount: 1, maximumCount: 1);
        //private static SemaphoreOwner ownerSemaphoreDCA = new SemaphoreOwner(semaphoreDCA);
        private static Semaphore semaphoreDiCon = new Semaphore(initialCount: 1, maximumCount: 1);
        private static SemaphoreOwner ownerSemaphoreDiCon = new SemaphoreOwner();

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

        ///// <summary>
        ///// getInstrumentLock() of an instrument
        ///// Such as getIntrumentLock(ATT1)
        ///// </summary>
        public bool getInstrumentLock(sharedInstrument instr, string sThreadID, string sMachineName, int nChannelInEachMeasurementGroup)
        {
            Console.WriteLine($"Thread={sThreadID} enters getInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
            bool bOwnSemaphore = false; 
            try
            {
                DCAQueueElement dca = null;
                // first, to traverse _dictDCAQueue[nChannelInEachMeasurementGroup] to see if the client already owns one of the semaphore
                foreach (var element in _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement) 
                {
                    if ((element.ownerSemaphoreDCA.sThreadID == sThreadID && element.ownerSemaphoreDCA.sMachineName == sMachineName))
                    {
                        dca = element;
                        bOwnSemaphore = true;
                        break;
                    }
                }
 
                // secondly, depending if the clients already owns a semaphore or not
                // if yes, add nested count
                // if not, find a first available semaphore
                if (bOwnSemaphore)
                {
                    // if the client already owns the semaphore , no need to obtain semaphore again
                    // or if the client just obtains the semaphore via WaitOne() 
                    // we will first re-afrim or re-assign the ownership and increase nestedCount
                    dca.ownerSemaphoreDCA.nestedCount++;
                    dca.ownerSemaphoreDCA.sThreadID = sThreadID;
                    dca.ownerSemaphoreDCA.sMachineName = sMachineName;
                    clog.Log($"Thread={sThreadID} already owned a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} already owned a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                }
                else
                {
                    clog.Log($"Thread={sThreadID} semaphoreDCA.WaitOne() processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} semaphoreDCA.WaitOne() processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

                    // if not a semaphore owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
                    _dictDCAQueue[nChannelInEachMeasurementGroup].semaphoreDCA.WaitOne();

                    // once passing the WaitOne(), it means there is one DCA with channel of nChannelInEachMeasurementGroup, let's find a first available one
                    for (int i = 0; i < _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement.Count; i++)
                    {
                        dca = _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement[i];
                        // to find an available one
                        if ((dca.ownerSemaphoreDCA.sThreadID == null && dca.ownerSemaphoreDCA.sMachineName == null))
                        {
                            // assign the ownership and increase nestedCount
                            dca.ownerSemaphoreDCA.nestedCount++;
                            dca.ownerSemaphoreDCA.sThreadID = sThreadID;
                            dca.ownerSemaphoreDCA.sMachineName = sMachineName;
                            bOwnSemaphore = true;
                            clog.Log($"Thread={sThreadID} getting a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                            Console.WriteLine($"Thread={sThreadID} getting a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                            break;
                        }
                        if (i == _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement.Count - 1) // no DCA is available, impossibleex
                        {
                            throw new Exception("No DCA available, even got a semaphore processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                        }
                    }                 
                }
                // then grant the InstrumentLock
                Console.WriteLine($"Thread={sThreadID} InstrumentLockService.getInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                sService = "InstrumentLockService.getInstrumentLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));

                Console.WriteLine($"Thread={sThreadID} exits getInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
            }
            catch (Exception ex)
            {
                ret = false;
                // Logging
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        public bool getInstrumentLockWithReturn(sharedInstrument instr, string sThreadID, string sMachineName, ref WCFScopeConfig DCA, ref WCFProtocolXConfig Protocol, int nChannelInEachMeasurementGroup)
        {
            Console.WriteLine($"Thread={sThreadID} enters getInstrumentLockWithReturn processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
            bool bOwnSemaphore = false;
            try
            {
                DCAQueueElement dca = null;
                // first, to traverse _dictDCAQueue[nChannelInEachMeasurementGroup] to see if the client already owns one of the semaphore
                foreach (var element in _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement)
                {
                    if ((element.ownerSemaphoreDCA.sThreadID == sThreadID && element.ownerSemaphoreDCA.sMachineName == sMachineName))
                    {
                        dca = element;
                        bOwnSemaphore = true;
                        //Get the DCA and its protocol
                        DCA = dca.DCA;
                        Protocol = dca.Protocol;
                        clog.Log($"Thread={sThreadID} already owned a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                        Console.WriteLine($"Thread={sThreadID} already owned a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                        break;
                    }
                }

                // secondly, depending if the clients already owns a semaphore or not
                // if yes, add nested count
                // if not, find a first available semaphore
                if (bOwnSemaphore)
                {
                    // if the client already owns the semaphore , no need to obtain semaphore again
                    // or if the client just obtains the semaphore via WaitOne() 
                    // we will first re-afrim or re-assign the ownership and increase nestedCount
                    dca.ownerSemaphoreDCA.nestedCount++;
                    dca.ownerSemaphoreDCA.sThreadID = sThreadID;
                    dca.ownerSemaphoreDCA.sMachineName = sMachineName;
                    clog.Log($"Thread={sThreadID} increases nestedCount of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} increases nestedCount of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                }
                else
                {
                    clog.Log($"Thread={sThreadID} semaphoreDCA.WaitOne() processID= {Process.GetCurrentProcess().Id}  managedID= {System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} semaphoreDCA.WaitOne() processID= {Process.GetCurrentProcess().Id}  managedID= {System.Environment.CurrentManagedThreadId}");

                    // if not a semaphore owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
                    _dictDCAQueue[nChannelInEachMeasurementGroup].semaphoreDCA.WaitOne();

                    // once passing the WaitOne(), it means there is one DCA with channel of nChannelInEachMeasurementGroup, let's find a first available one
                    for (int i = 0; i < _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement.Count; i++)
                    {
                        dca = _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement[i];
                        // to find an available one
                        if ((dca.ownerSemaphoreDCA.sThreadID == null && dca.ownerSemaphoreDCA.sMachineName == null))
                        {
                            // assign the ownership and increase nestedCount
                            dca.ownerSemaphoreDCA.nestedCount++;
                            dca.ownerSemaphoreDCA.sThreadID = sThreadID;
                            dca.ownerSemaphoreDCA.sMachineName = sMachineName;
                            bOwnSemaphore = true;
                            //Get the DCA and its protocol
                            DCA = dca.DCA;
                            Protocol = dca.Protocol;
                            clog.Log($"Thread={sThreadID} claiming a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                            Console.WriteLine($"Thread={sThreadID} claiming a semaphore of DCA {dca.DCA.strName} processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                            break;
                        }
                        if (i == _dictDCAQueue[nChannelInEachMeasurementGroup].lstDCAQueueElement.Count - 1) // no DCA is available, impossibleex
                        {
                            throw new Exception($"Thread={sThreadID} No DCA available, even got a semaphore processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                        }
                    }
                }

                // then grant the InstrumentLock
                Console.WriteLine($"Thread={sThreadID} InstrumentLockService.getInstrumentLockWithReturn processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                sService = "InstrumentLockService.getInstrumentLockWithReturn";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));

                Console.WriteLine($"Thread={sThreadID} exits getInstrumentLockWithReturn processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
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
            Console.WriteLine($"Thread={sThreadID} enters releaseInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
            bool bOwnSemaphore = false;
            try
            {
                DCAQueueElement dca = null;
                DCAQueue queue = null;
                // because of no nChannelInEachMeasurementGroup as input parameter
                // first to traverse all the Keys
                // traverse arDCAQueue to see if the client already owns one of the semaphore
                foreach (var entry in _dictDCAQueue)
                {
                    foreach (var element in entry.Value.lstDCAQueueElement)
                    {
                        // check if the client already owns the semaphore
                        if ((element.ownerSemaphoreDCA.sThreadID == sThreadID && element.ownerSemaphoreDCA.sMachineName == sMachineName))
                        {
                            bOwnSemaphore = true;
                            dca = element;
                            queue = entry.Value;
                            break;
                        }
                    }
                    if (bOwnSemaphore)
                        break;
                }
                // now found the dca owned by the tester client, so first to descrease nestedCount
                dca.ownerSemaphoreDCA.nestedCount--;
                Console.WriteLine($"Thread={sThreadID} decrease nested semaphore count processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

                // if nestedCount==0, then reset the ownership to null
                // and release semaphore
                // otherwise, keep the sThreadID and sMachineName as owner of the semaphore; i.e., no release of semaphore
                if (dca.ownerSemaphoreDCA.nestedCount == 0)
                {
                    dca.ownerSemaphoreDCA.sThreadID = null;
                    dca.ownerSemaphoreDCA.sMachineName = null;
                    // release one semaphore of the entry
                    Console.WriteLine($"Thread={sThreadID} releasing semaphore processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    queue.semaphoreDCA.Release();
                }            

                Console.WriteLine($"Thread={sThreadID} InstrumentLockService.releaseInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
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

            Console.WriteLine($"Thread={sThreadID} exits releaseInstrumentLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
            return ret;
        }

        /// <summary>
        /// My station config
        /// </summary>
        protected StationHardware _stationInstance;
        
        /// <summary>
        /// build a queue of all DCAs and their corresponding Protocols in the server station config file
        /// </summary>
        public void buildDCAandProtocolQueue()
        {
            // set StationHardware.Instance
            _stationInstance = StationHardware.Instance();

            string sDCA = "DCA";
            _dictDCAQueue = new Dictionary<int, DCAQueue>();
            WCFScopeConfig DCAConfig;
            WCFProtocolXConfig ProtocolConfig;

            foreach (var instrument in _stationInstance.myConfig.arInstConfig)
            {
                DCAConfig = JsonConvert.DeserializeObject<WCFScopeConfig>(JsonConvert.SerializeObject(instrument));
                if (DCAConfig.strName.Contains(sDCA)) // the key of the current instrument in _stationInstance.myConfig.arInstConfig is a DCA
                {
                    // DCAConfig.ProtocolObjectRefName is the key to use to look up the associated protocol of the current DCA instrument
                    foreach (var protocol in _stationInstance.myConfig.arProtocolConfig)
                    {
                        ProtocolConfig = JsonConvert.DeserializeObject<WCFProtocolXConfig>(JsonConvert.SerializeObject(protocol));
                        if (ProtocolConfig.strName.Equals(DCAConfig.ProtocolObjectRefName)) // if the current protocol strName matches the one of the DCA
                        {
                            DCAQueueElement element = new DCAQueueElement(DCAConfig, ProtocolConfig);
                            int key = DCAConfig.arChannels.Count;
                            if (_dictDCAQueue.ContainsKey(key))
                            {
                                _dictDCAQueue[key].lstDCAQueueElement.Add(element);
                            }
                            else
                            {
                                DCAQueue value = new DCAQueue();
                                value.lstDCAQueueElement = new List<DCAQueueElement>() { element };
                                _dictDCAQueue.Add(key, value);
                            }
                        }
                    }
                }
            }

            // according to the number of DCA in the lstDCAQueueElement, set the semaphore and its couter
            foreach (var value in _dictDCAQueue.Values) 
            { 
                int count = value.lstDCAQueueElement.Count;
                value.semaphoreDCA = new Semaphore(initialCount: count, maximumCount: count);
            }
            
    }


        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        public bool getProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            Console.WriteLine($"Thread={sThreadID} enters getProtocolLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
            try
            {
                // first, check ownerSemaphoreDiCon to see if the client already owns the semaphore
                if ((ownerSemaphoreDiCon.sThreadID == sThreadID && ownerSemaphoreDiCon.sMachineName == sMachineName))
                {
                    clog.Log($"Thread={sThreadID} already owned DiCon semaphore processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} already owned DiCon semaphore processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");

                    // if the client already owns the semaphore , no need to obtain semaphore again
                    // or if the client just obtains the semaphore via WaitOne() 
                    // we will first re-afrim or re-assign the ownership and increase nestedCount
                    ownerSemaphoreDiCon.nestedCount++;
                    ownerSemaphoreDiCon.sThreadID = sThreadID;
                    ownerSemaphoreDiCon.sMachineName = sMachineName;
                    clog.Log($"Thread={sThreadID} increases nestedCount of DiCon processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} increases nestedCount of DiCon processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                }
                else
                {
                    clog.Log($"Thread={sThreadID} semaphoreDiCon.WaitOne() processID= {Process.GetCurrentProcess().Id}  managedID= {System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} semaphoreDiCon.WaitOne() processID= {Process.GetCurrentProcess().Id}  managedID= {System.Environment.CurrentManagedThreadId}");

                    // if not a semaphore owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
                    semaphoreDiCon.WaitOne();

                    // once passing the WaitOne(), it means DiCon available
                    // assign the ownership and increase nestedCount
                    ownerSemaphoreDiCon.nestedCount++;
                    ownerSemaphoreDiCon.sThreadID = sThreadID;
                    ownerSemaphoreDiCon.sMachineName = sMachineName;
                    clog.Log($"Thread={sThreadID} claiming a semaphore of DiCon processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Thread={sThreadID} claiming a semaphore of DiCon processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                }

                // then grant the getProtocolLock
                Console.WriteLine($"Thread={sThreadID} InstrumentLockService.getProtocolLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                sService = "InstrumentLockService.getProtocolLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));

                Console.WriteLine($"Thread={sThreadID} exits getProtocolLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
            }
            catch (Exception ex)
            {
                ret = false;
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
        //public bool getProtocolLock_OLD(sharedProtocol protocol, string sThreadID, string sMachineName)
        //{
        //    Console.WriteLine($"Thread={sThreadID} enters getProtocolLock at server processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
        //    bool ret = false;
        //    DateTime ServiceStart = DateTime.Now;
        //    DateTime ServiceFinish = new DateTime(1, 1, 1);
        //    string sService = "InstrumentLockService.null";

        //    try
        //    {
        //        // check if the client already owns the semaphore
        //        // if not the owner, WaitOne() which blocks the current thread until the current WaitHandle receives a signal.
        //        if (!(ownerSemaphoreDiCon.sThreadID == sThreadID && ownerSemaphoreDiCon.sMachineName == sMachineName))
        //        {
        //            //if (ownerSemaphoreDiCon.sThreadID == null)
        //                semaphoreDiCon.WaitOne();
        //            //else
        //            //{
        //            //    bool test=true;
        //            //    while (test)
        //            //        ;
        //            //}
        //        }

        //        // if the client already owns the semaphore , no need to obtain semaphore again
        //        // or if the client just obtains the semaphore via WaitOne() 
        //        // we will first re-afrim or re-assign the ownership and increase nestedCount
        //        ownerSemaphoreDiCon.nestedCount++;
        //        ownerSemaphoreDiCon.sThreadID = sThreadID;
        //        ownerSemaphoreDiCon.sMachineName = sMachineName;
        //        // then grant the InstrumentLock
        //        Console.WriteLine($"Thread={sThreadID} InstrumentLockService.getProtocolLock at server processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
        //        sService = "InstrumentLockService.getProtocolLock";
        //        ret = true;

        //        ServiceFinish = DateTime.Now;
        //        // contruct the client request values to be sent to host
        //        var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
        //        // each WCF service fires the event EventFromClient with the values from WCF client
        //        EventFromClient?.Invoke(this, new CustomEventArgs(value));
        //    }
        //    catch (Exception ex)
        //    {
        //        ret = false;
        //        // Logging
        //        Console.WriteLine(ex.Message);
        //    }
        //    Console.WriteLine($"Thread={sThreadID} exits getProtocolLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
        //    return ret;
        //}


        public bool releaseProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName)
        {
            Console.WriteLine($"Thread={sThreadID} enters releaseProtocolLock at server processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
            bool ret = false;
            DateTime ServiceStart = DateTime.Now;
            DateTime ServiceFinish = new DateTime(1, 1, 1);
            string sService = "InstrumentLockService.null";
          
            try
            {
                // first to descrease nestedCount
                ownerSemaphoreDiCon.nestedCount--;

                // if nestedCount==0, then reset the ownership to null
                // otherwise, keep the sThreadID and sMachineName as owner of the semaphore; i.e., no release of semaphore
                if (ownerSemaphoreDiCon.nestedCount == 0)
                {
                    ownerSemaphoreDiCon.sThreadID = null;
                    ownerSemaphoreDiCon.sMachineName = null;
                    // then release the semaphore
                    semaphoreDiCon.Release();
                }

                Console.WriteLine($"Thread={sThreadID} InstrumentLockService.releaseProtocolLock processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
                sService = "InstrumentLockService.releaseProtocolLock";
                ret = true;

                ServiceFinish = DateTime.Now;
                // contruct the client request values to be sent to host
                var value = new ClientRequestValue(dInputA: 0, dInputB: 0, delayInSec: 0, dResult: 0, sService: sService, sThreadID: sThreadID, sMachineName: sMachineName, ServiceStart: ServiceStart, ServiceFinish: ServiceFinish);
                // each WCF service fires the event EventFromClient with the values from WCF client
                EventFromClient?.Invoke(this, new CustomEventArgs(value));
                Console.WriteLine($"Thread={sThreadID} exits releaseProtocolLock at server processID={Process.GetCurrentProcess().Id} managedID={System.Environment.CurrentManagedThreadId}");
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
