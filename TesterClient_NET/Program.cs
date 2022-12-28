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
            string ThreadID = Process.GetCurrentProcess().Id.ToString();

            try
            {
                // use client function class to set up firewall rules
                _clientFct.setFirewall();

                do
                {
                    // the reason to place the endpoint function within the do-while loop:
                    // if server restarts in between two loops, must set up endpoint again
                    // programmably set up EndPoint
                    _clientFct.wshttpClientEndPoint();
                    //_clientFct.netTcpClientEndPoint();

                    // assign the _iClient obtained within the above Endpoint functions
                    _client = _clientFct._iClient;

                    sum = _client.Add(a, b, ThreadID);
                    Console.WriteLine($"\nThread={ThreadID} Add({a}, {b}) = {sum.ToString()}\n");

                    delay = 1; //seconds
                    Console.WriteLine($"Thread={ThreadID} AddAndDelay({a}, {b}) with delay = {delay.ToString()} seconds.");
                    sum = _client.AddAndDelay(a, b, delay, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} AddAndDelay({a}, {b}) = {sum.ToString()} with delay = {delay.ToString()} seconds.\n");

                    ret = _client.getInstrumentLock(sharedInstrument.DCA, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} getInstrumentLock(sharedInstrument.DCA)");

                    // doing something with DCA
                    Console.WriteLine($"Thread={ThreadID} doing something with DCA");
                    Thread.Sleep(10000);

                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} releaseInstrumentLock(sharedInstrument.DCA)\n");

                    ret = _client.getProtocolLock(sharedProtocol.DiCon, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} getProtocolLock(sharedProtocol.DiCon)");

                    // doing something with DiCon
                    Console.WriteLine($"Thread={ThreadID} doing something with DiCon");
                    Thread.Sleep(1000);

                    ret = _client.releaseProtocolLock(sharedProtocol.DiCon, ThreadID);
                    Console.WriteLine($"Thread={ThreadID} releaseProtocolLock(sharedProtocol.DiCon)\n");


                    Console.WriteLine($"Press ENTER to close the console window; other keys to repeat ...........");
                } while (Console.ReadKey().Key != ConsoleKey.Enter);
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
