using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentLockServiceShared
{
    /// <summary>
    /// this class contains methods that are used by both hosts and clients
    /// </summary>
    public interface IInstrumentLockServiceShared
    {
        /// <summary>
        /// https://stackoverflow.com/questions/1242566/any-way-to-turn-the-internet-off-in-windows-using-c/1243026#1243026
        /// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ics/using-windows-firewall-with-advanced-security
        /// </summary>
        void setFirewall();

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        /// </summary>
        void wshttpClientEndPoint();

        /// <summary>
        ///  https://stackoverflow.com/questions/6118221/how-do-i-add-wcf-client-endpoints-programmatically
        ///  https://stackoverflow.com/questions/2943148/how-to-programmatically-connect-a-client-to-a-wcf-service
        /// </summary>
        void netTcpClientEndPoint();

    }

}
