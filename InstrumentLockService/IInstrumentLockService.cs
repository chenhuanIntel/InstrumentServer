using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace InstrumentLockService
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
        double Add(double a, double b);

        /// <summary>
        /// demo/try-out WCF service
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [OperationContract]
        double AddAndDelay(double a, double b, int delayInSec);

        // Arithmetic operations with the float and double types never throw an exception.
        // The result of arithmetic operations with those types can be one of special values that represent infinity and not-a-number:
        [OperationContract]
        [FaultContract(typeof(MathFault))]
        int intDivide(double a, double b);

        /// <summary>
        /// getInstrumentLock() of an instrument
        /// Such as getIntrumentLock(ATT1)
        /// </summary>
        [OperationContract]
        void getInstrumentLock();

        /// <summary>
        /// getProtocolLock() of a mutex
        /// Such as getProtocolLock(Dicon1)
        /// Mutex of Dicon1 is shared by PM1, SW1 and ATT1
        /// Because the DiCon box contains 3 different functional instruments, SW, ATT and PowerMeter.
        /// </summary>
        [OperationContract]
        void getProtocolLock();

        /// <summary>
        /// getConnectedInfo()
        /// Returned info such as (SPAN_Station1_Slot1, DCA1)
        /// </summary>
        [OperationContract]
        void getConnectedInfo();
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
