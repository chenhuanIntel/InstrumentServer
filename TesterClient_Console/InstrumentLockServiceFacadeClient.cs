using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using InstrumentLockServices;

namespace TesterClient_Console
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class InstrumentLockServiceFacadeClient : System.ServiceModel.ClientBase<IInstrumentLockServiceFacade>, IInstrumentLockServiceFacade
    {

        public InstrumentLockServiceFacadeClient()
        {
        }

        public InstrumentLockServiceFacadeClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public InstrumentLockServiceFacadeClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public InstrumentLockServiceFacadeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public InstrumentLockServiceFacadeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public double Add(double a, double b, string ThreadID, string MachineName)
        {
            return base.Channel.Add(a, b, ThreadID, MachineName);
        }

        public double AddAndDelay(double a, double b, int delayInSec, string ThreadID, string MachineName)
        {
            return base.Channel.AddAndDelay(a, b, delayInSec, ThreadID, MachineName);
        }

        public int intDivide(double a, double b, string ThreadID, string MachineName)
        {
            return base.Channel.intDivide(a, b, ThreadID, MachineName);
        }

        public bool getInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID, string MachineName, int nChannelInEachMeasurementGroup)
        {
            return base.Channel.getInstrumentLock(instr, ThreadID, MachineName, nChannelInEachMeasurementGroup);
        }

        public bool getInstrumentLockWithReturn(InstrumentLockServices.sharedInstrument instr, string sThreadID, string sMachineName, ref InstrumentLockServices.WCFScopeConfig DCA, ref InstrumentLockServices.WCFProtocolXConfig Protocol, int nChannelInEachMeasurementGroup)
        {
            return base.Channel.getInstrumentLockWithReturn(instr, sThreadID, sMachineName, ref DCA, ref Protocol, nChannelInEachMeasurementGroup);
        }

        public bool releaseInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID, string MachineName)
        {
            return base.Channel.releaseInstrumentLock(instr, ThreadID, MachineName);
        }


        public bool getProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID, string MachineName)
        {
            return base.Channel.getProtocolLock(protocol, ThreadID, MachineName);
        }


        public bool releaseProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID, string MachineName)
        {
            return base.Channel.releaseProtocolLock(protocol, ThreadID, MachineName);
        }


        public void getConnectedInfo()
        {
            base.Channel.getConnectedInfo();
        }

    }

}
