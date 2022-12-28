﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TesterClient_Console.InstrumentLockServiceFacadeClient {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade")]
    public interface IInstrumentLockServiceFacade {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/Add", ReplyAction="http://tempuri.org/IInstrumentLockService/AddResponse")]
        double Add(double a, double b, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/Add", ReplyAction="http://tempuri.org/IInstrumentLockService/AddResponse")]
        System.Threading.Tasks.Task<double> AddAsync(double a, double b, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/AddAndDelay", ReplyAction="http://tempuri.org/IInstrumentLockService/AddAndDelayResponse")]
        double AddAndDelay(double a, double b, int delayInSec, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/AddAndDelay", ReplyAction="http://tempuri.org/IInstrumentLockService/AddAndDelayResponse")]
        System.Threading.Tasks.Task<double> AddAndDelayAsync(double a, double b, int delayInSec, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/intDivide", ReplyAction="http://tempuri.org/IInstrumentLockService/intDivideResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(InstrumentLockServices.MathFault), Action="http://tempuri.org/IInstrumentLockService/intDivideMathFaultFault", Name="MathFault", Namespace="http://schemas.datacontract.org/2004/07/InstrumentLockServices")]
        int intDivide(double a, double b, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/intDivide", ReplyAction="http://tempuri.org/IInstrumentLockService/intDivideResponse")]
        System.Threading.Tasks.Task<int> intDivideAsync(double a, double b, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getInstrumentLock", ReplyAction="http://tempuri.org/IInstrumentLockService/getInstrumentLockResponse")]
        bool getInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getInstrumentLock", ReplyAction="http://tempuri.org/IInstrumentLockService/getInstrumentLockResponse")]
        System.Threading.Tasks.Task<bool> getInstrumentLockAsync(InstrumentLockServices.sharedInstrument instr, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/releaseInstrumentLock", ReplyAction="http://tempuri.org/IInstrumentLockService/releaseInstrumentLockResponse")]
        bool releaseInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/releaseInstrumentLock", ReplyAction="http://tempuri.org/IInstrumentLockService/releaseInstrumentLockResponse")]
        System.Threading.Tasks.Task<bool> releaseInstrumentLockAsync(InstrumentLockServices.sharedInstrument instr, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getProtocolLock", ReplyAction="http://tempuri.org/IInstrumentLockService/getProtocolLockResponse")]
        bool getProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getProtocolLock", ReplyAction="http://tempuri.org/IInstrumentLockService/getProtocolLockResponse")]
        System.Threading.Tasks.Task<bool> getProtocolLockAsync(InstrumentLockServices.sharedProtocol protocol, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/releaseProtocolLock", ReplyAction="http://tempuri.org/IInstrumentLockService/releaseProtocolLockResponse")]
        bool releaseProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/releaseProtocolLock", ReplyAction="http://tempuri.org/IInstrumentLockService/releaseProtocolLockResponse")]
        System.Threading.Tasks.Task<bool> releaseProtocolLockAsync(InstrumentLockServices.sharedProtocol protocol, string ThreadID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getConnectedInfo", ReplyAction="http://tempuri.org/IInstrumentLockService/getConnectedInfoResponse")]
        void getConnectedInfo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInstrumentLockService/getConnectedInfo", ReplyAction="http://tempuri.org/IInstrumentLockService/getConnectedInfoResponse")]
        System.Threading.Tasks.Task getConnectedInfoAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInstrumentLockServiceFacadeChannel : TesterClient_Console.InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InstrumentLockServiceFacadeClient : System.ServiceModel.ClientBase<TesterClient_Console.InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade>, TesterClient_Console.InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade {
        
        public InstrumentLockServiceFacadeClient() {
        }
        
        public InstrumentLockServiceFacadeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public InstrumentLockServiceFacadeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InstrumentLockServiceFacadeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InstrumentLockServiceFacadeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public double Add(double a, double b, string ThreadID) {
            return base.Channel.Add(a, b, ThreadID);
        }
        
        public System.Threading.Tasks.Task<double> AddAsync(double a, double b, string ThreadID) {
            return base.Channel.AddAsync(a, b, ThreadID);
        }
        
        public double AddAndDelay(double a, double b, int delayInSec, string ThreadID) {
            return base.Channel.AddAndDelay(a, b, delayInSec, ThreadID);
        }
        
        public System.Threading.Tasks.Task<double> AddAndDelayAsync(double a, double b, int delayInSec, string ThreadID) {
            return base.Channel.AddAndDelayAsync(a, b, delayInSec, ThreadID);
        }
        
        public int intDivide(double a, double b, string ThreadID) {
            return base.Channel.intDivide(a, b, ThreadID);
        }
        
        public System.Threading.Tasks.Task<int> intDivideAsync(double a, double b, string ThreadID) {
            return base.Channel.intDivideAsync(a, b, ThreadID);
        }
        
        public bool getInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID) {
            return base.Channel.getInstrumentLock(instr, ThreadID);
        }
        
        public System.Threading.Tasks.Task<bool> getInstrumentLockAsync(InstrumentLockServices.sharedInstrument instr, string ThreadID) {
            return base.Channel.getInstrumentLockAsync(instr, ThreadID);
        }
        
        public bool releaseInstrumentLock(InstrumentLockServices.sharedInstrument instr, string ThreadID) {
            return base.Channel.releaseInstrumentLock(instr, ThreadID);
        }
        
        public System.Threading.Tasks.Task<bool> releaseInstrumentLockAsync(InstrumentLockServices.sharedInstrument instr, string ThreadID) {
            return base.Channel.releaseInstrumentLockAsync(instr, ThreadID);
        }
        
        public bool getProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID) {
            return base.Channel.getProtocolLock(protocol, ThreadID);
        }
        
        public System.Threading.Tasks.Task<bool> getProtocolLockAsync(InstrumentLockServices.sharedProtocol protocol, string ThreadID) {
            return base.Channel.getProtocolLockAsync(protocol, ThreadID);
        }
        
        public bool releaseProtocolLock(InstrumentLockServices.sharedProtocol protocol, string ThreadID) {
            return base.Channel.releaseProtocolLock(protocol, ThreadID);
        }
        
        public System.Threading.Tasks.Task<bool> releaseProtocolLockAsync(InstrumentLockServices.sharedProtocol protocol, string ThreadID) {
            return base.Channel.releaseProtocolLockAsync(protocol, ThreadID);
        }
        
        public void getConnectedInfo() {
            base.Channel.getConnectedInfo();
        }
        
        public System.Threading.Tasks.Task getConnectedInfoAsync() {
            return base.Channel.getConnectedInfoAsync();
        }
    }
}
