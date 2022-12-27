using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockServices;
using InstrumentLockServiceHosts;
using NetFwTypeLib; // Located in FirewallAPI.dll

// namespace name uses plural
namespace InstrumentLockServiceHost_NETs
{
    internal class Program
    {
        /// <summary>
        /// console app Main()
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var myHost = new InstrumentLockServiceHost();
            // create firewall rules
            myHost.setFirewall();
            // initialize host
            myHost.initialize();

            // press ENTER to terminate
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();

            myHost.Dispose();
        }
    }
}
