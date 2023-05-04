using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockServices;
using NetFwTypeLib; // Located in FirewallAPI.dll

// namespace name uses plural
namespace TesterClient_Consoles
{
    internal class Program
    {
        // to avoid the error "Unable to cast transparent proxy to type" in which 
        // _client was defined as InstrumentLockServiceFacadeClient.IInstrumentLockServiceFacade
        // should be              InstrumentLockServices.IInstrumentLockServiceFacade, or simply IInstrumentLockServiceFacade
        private static IInstrumentLockServiceFacade _client;
        private static clientFunctions _clientFct = new clientFunctions();

        static void Main(string[] args)
        {

            //InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient obj = new InstrumentLockServiceFacadeClient.InstrumentLockServiceFacadeClient();
            double a = 0;
            double b = 0;
            double sum = 0;
            int delay = 0;
            bool ret = false;
            string sThreadID = Process.GetCurrentProcess().Id.ToString();
            string sMachineName = Environment.MachineName;
            ConsoleKey key;
            //Uri baseAddress = new Uri("net.tcp://localhost:8001/");
            //Uri baseAddress = new Uri("http://172.25.93.250:8080/");
            Uri baseAddress = new Uri("net.tcp://172.25.93.250:8001/");

            try
            {
                // use client function class to set up firewall rules
                _clientFct.setFirewall();

                do
                {
                    // the reason to place the endpoint function within the do-while loop:
                    // if server restarts in between two loops, must set up endpoint again
                    // programmably set up EndPoint
                    TimeSpan myTS = TimeSpan.FromMinutes(20);
                    //_clientFct.wshttpClientEndPoint(baseAddress, myTS);
                    _clientFct.netTcpClientEndPoint(baseAddress, myTS);

                    // assign the _iClient obtained within the above Endpoint functions
                    _client = _clientFct._iClient;

                    sum = _client.Add(a, b, sThreadID, sMachineName);
                    Console.WriteLine($"\nMachine={sMachineName}, Thread={sThreadID} Add({a}, {b}) = {sum.ToString()}\n");

                    delay = 1; //seconds
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} AddAndDelay({a}, {b}) with delay = {delay.ToString()} seconds.");
                    sum = _client.AddAndDelay(a, b, delay, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} AddAndDelay({a}, {b}) = {sum.ToString()} with delay = {delay.ToString()} seconds.\n");

                    WCFProtocolXConfig WCFProtocol = null;
                    WCFScopeConfig WCFDCA = null;
                    int nChannelInEachMeasurementGroup = 2;
                    List<string> lsDCAChannelName = null; 
                    ret = _client.getInstrumentLockWithReturn(sharedInstrument.DCA, sThreadID, sMachineName, WCFDCA, WCFProtocol, nChannelInEachMeasurementGroup, lsDCAChannelName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLock(sharedInstrument.DCA)");

                    ret = _client.getInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLock(sharedInstrument.DCA)");

                    // doing something with DCA
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} doing something with DCA");
                    Thread.Sleep(2 * 60000);

                    // nested DCA lock request
                    ret = _client.getInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLock(sharedInstrument.DCA)");

                    // doing something with DCA
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} doing something with DCA");
                    Thread.Sleep(5 * 60000);

                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseInstrumentLock(sharedInstrument.DCA)");

                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseInstrumentLock(sharedInstrument.DCA)\n");

                    ret = _client.getProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getProtocolLock(sharedProtocol.DiCon)");

                    // doing something with DiCon
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} doing something with DiCon");
                    Thread.Sleep(1000);

                    ret = _client.releaseProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseProtocolLock(sharedProtocol.DiCon)\n");


                    Console.WriteLine($"Press ENTER to close the console window; other keys to repeat ...........");
                    key = Console.ReadKey().Key;
                } while (key != ConsoleKey.Enter);
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine(timeProblem.Message);
                Console.ReadLine();
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine(commProblem.Message);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Logging
                Console.WriteLine(ex.Message);
            }
        }
    }
}
