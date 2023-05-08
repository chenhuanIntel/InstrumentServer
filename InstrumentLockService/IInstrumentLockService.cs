using InstrumentsLib.Tools.Core;
using InstrumentsLib.Tools.Instruments.Oscilloscope;
using InstrumentsLib.Tools.Instruments.Switch;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Utility;

// namespace name uses plural
namespace InstrumentLockServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IInstrumentLockService
    {
        /// <summary>
        /// demo/try-out WCF service
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [OperationContract]
        double Add(double a, double b, string ThreadID, string MachineName);

        /// <summary>
        /// demo/try-out WCF service
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [OperationContract]
        double AddAndDelay(double a, double b, int delayInSec, string ThreadID, string MachineName);

        // Arithmetic operations with the float and double types never throw an exception.
        // The result of arithmetic operations with those types can be one of special values that represent infinity and not-a-number:
        [OperationContract]
        [FaultContract(typeof(MathFault))]
        int intDivide(double a, double b, string ThreadID, string MachineName);

        ///// <summary>
        ///// getInstrumentLock() of an instrument
        ///// Such as getIntrumentLock(ATT1)
        ///// </summary>
        [OperationContract]
        bool getInstrumentLock(sharedInstrument instr, string ThreadID, string MachineName);


        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        [OperationContract]   
        bool getInstrumentLockWithReturn(sharedInstrument instr, string sThreadID, string sMachineName, ref WCFScopeConfig DCA, ref WCFProtocolXConfig Protocol, int nChannelInEachMeasurementGroup);

        /// <summary>
        /// releaseInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        [OperationContract]
        bool releaseInstrumentLock(sharedInstrument instr, string ThreadID, string MachineName);

        ///// <summary>
        ///// getProtocolLock() of a mutex
        ///// Such as getProtocolLock(Dicon1)
        ///// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        ///// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        ///// </summary>
        [OperationContract]
        bool getProtocolLock(sharedProtocol protocol, string ThreadID, string MachineName);

        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        //[OperationContract]
        //bool getProtocolLock(sharedProtocol protocol, string sThreadID, string sMachineName, WCFMapSwitchConfig TxSwitch);

        /// <summary>
        /// releaseProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        [OperationContract]
        bool releaseProtocolLock(sharedProtocol protocol, string ThreadID, string MachineName);

        /// <summary>
        /// getConnectedInfo()
        /// Returned info such as (SPAN_Station1_Slot1, DCA1)
        /// </summary>
        [OperationContract]
        void getConnectedInfo();
    }

    /// <summary>
    /// IInstrumentLockServiceFacade
    /// The issue of the (singleton instance with event handler) is that the service contract is limited to “singleton” as well. It means the behavior of the WCF service is single; that is, once WCF service is used by one client, no other clients can use it. The mutex implemented inside WCF service can never be used. There are a few solutions, such as creating a facade
    /// </summary>
    [ServiceContract]
    public interface IInstrumentLockServiceFacade : IInstrumentLockService
    {

    }

    [DataContract]
    /// <summary>
    /// 
    /// </summary>
    public class WCFProtocolXConfig : ProtocolVISADotNetConfig
    {
        /// <summary>
        /// Contructor of ProtocolXConfig
        /// </summary>
        public WCFProtocolXConfig()
        {
        }
    }


    [DataContract]
    /// <summary>
    /// 
    /// </summary>
    public class WCFMapSwitchConfig
    {
        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int nSlotAddr { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public int nDeviceAddr { get; set; }

        [DataMember]
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, int> channelMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WCFMapSwitchConfig()
        {
            channelMap = new Dictionary<string, int>();
            //buildTargetClassInfo(typeof(MapSwitch));
        }
    }

    //[DataContract]
    ///// <summary>
    ///// 
    ///// </summary>
    //public class parentCmd
    //{
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string pSLOT { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string pSOURCE { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string pPreset { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public double pdMaxBusyTimeoutSecs { get; set; }


    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bReLock"></param>
    //    /// <returns></returns>
    //    public bool pSetup()
    //    {

    //        return true;
    //    }


    //}

    //[DataContract]
    ///// <summary>
    ///// 
    ///// </summary>
    //public class tstCRUSetupcmd : parentCmd
    //{
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string SLOT { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string SOURCE { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Preset { get; set; }
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public double dMaxBusyTimeoutSecs { get; set; }


    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bReLock"></param>
    //    /// <returns></returns>
    //    public bool Setup()
    //    {

    //        return true;
    //    }


    //}

    //[DataContract]
    //public class myTestSettings
    //{
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public const string INIT_SETTING = "INIT_SETTING";
    //    [DataMember]
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public tstCRUSetupcmd test { get; set; }
    //}


    // data contract must be applied to any class that is intended to be seralized, even in all the parent classes
    // data member must also be applied to any property of any class if the property is intended to be serialed
    [DataContract]
    /// <summary>
    /// Generic Scope Configuration Property Class
    /// </summary>
    public class WCFScopeConfig : ScopeConfig
    {

        //[DataMember]
        ///// <summary>
        ///// 
        ///// </summary>
        //public Dictionary<string, myTestSettings> mapTestSettings { get; set; }

        /// <summary>
        /// Default constructor for ScopeConfig
        /// </summary>
        public WCFScopeConfig()
        {
        }
    }


    [DataContract]
    public class MathFault
    {
        private string operation;
        private string problemType;

        [DataMember]
        public string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        [DataMember]
        public string ProblemType
        {
            get { return problemType; }
            set { problemType = value; }
        }
    }

}
