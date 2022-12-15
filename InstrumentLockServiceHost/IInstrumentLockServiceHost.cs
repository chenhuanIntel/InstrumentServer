using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentLockService;

namespace InstrumentLockServiceHost
{
    public interface IInstrumentLockServiceHost
    {
        /// <summary>
        /// define the global variable of WCF service instance
        /// and start the service host of the instance
        /// </summary>
        void initialize();

        /// <summary>
        /// Host will handle the event from client here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleEventFromClient(object sender, CustomEventArgs e);

        /// <summary>
        /// Stop Host and dispose Instance
        /// </summary>
        void Dispose();
    }
}
