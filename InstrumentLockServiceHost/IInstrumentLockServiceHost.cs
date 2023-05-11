using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentLockServices;
using InstrumentsLib.Tools.Instruments.InstrumentServer;

// namespace name uses plural
namespace InstrumentLockServiceHosts
{
    public interface IInstrumentLockServiceHost
    {
        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        void initialize(Uri baseAddress);

        /// <summary>
        /// Stop Host and dispose Instance
        /// </summary>
        void Dispose();

        /// <summary>
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleEventFromClient(object sender, CustomEventArgs e);

        /// <summary>
        /// https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        /// programmably to set firewall rules
        /// </summary>
        void setFirewall(InstrumentServer_Config serverConfig);

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        ///  programmably to set up endpoint for clients
        ///  this method is to use wshttp
        /// </summary>
        void wsHttpEndPoint(Uri baseAddress);

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        ///  programmably to set up endpoint for clients
        ///  this method is to use TCP
        /// </summary>
        void netTcpEndPoint(Uri baseAddress);
    }
}
