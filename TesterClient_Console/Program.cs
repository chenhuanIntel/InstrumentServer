﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InstrumentLockServices;
using NetFwTypeLib; // Located in FirewallAPI.dll
using InstrumentsLib.Tools.Instruments.Oscilloscope;
using System.Runtime.InteropServices;
using System.Timers;

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


        protected static string getThreadID()
        {
            int nProcessID = Process.GetCurrentProcess().Id; // one FG-SPAN process will hold many threads, including all DUT threads
            //Thread thread = Thread.CurrentThread;
            //int nManagedID = thread.ManagedThreadId; //https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread.managedthreadid?view=net-8.0
            int nManagedID = System.Environment.CurrentManagedThreadId; //https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1840
            // OS thread is no longer easy to get, hence, use process ID + managed thread ID
            // https://github.com/dotnet/runtime/issues/63535
            return (nProcessID.ToString() + "-" + nManagedID.ToString());
        }

        private static System.Timers.Timer aTimer;
        private void SetTimer(int nDelayInSec)
        {
            bool bTimeIsUp = false;
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(nDelayInSec*1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += (s, args) => bTimeIsUp = true;
            aTimer.AutoReset = false; //so that it only calls the method once
            aTimer.Start();
            while (bTimeIsUp == false) { };
        }


        protected void dutThread()
        {
            bool ret = false;
            string sThreadID = getThreadID();
            string sMachineName = Environment.MachineName;
            ConsoleKey key = ConsoleKey.F1;
            //Uri baseAddress = new Uri("net.tcp://localhost:8001/");
            //Uri baseAddress = new Uri("http://172.25.93.250:8080/");
            Uri baseAddress = new Uri("net.tcp://172.25.93.250:8001/");

            try
            {
                do
                {
                    // use client function class to set up firewall rules
                    _clientFct.setFirewall();

                    // the reason to place the endpoint function within the do-while loop:
                    // if server restarts in between two loops, must set up endpoint again
                    // programmably set up EndPoint
                    TimeSpan myTS = TimeSpan.FromMinutes(20);
                    //_clientFct.wshttpClientEndPoint(baseAddress, myTS);
                    _clientFct.netTcpClientEndPoint(baseAddress, myTS);

                    // assign the _iClient obtained within the above Endpoint functions
                    _client = _clientFct._iClient;

                    //sum = _client.Add(a, b, sThreadID, sMachineName);
                    //Console.WriteLine($"\nMachine={sMachineName}, Thread={sThreadID} Add({a}, {b}) = {sum.ToString()}\n");

                    //delay = 1; //seconds
                    //Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} AddAndDelay({a}, {b}) with delay = {delay.ToString()} seconds.");
                    //sum = _client.AddAndDelay(a, b, delay, sThreadID, sMachineName);
                    //Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} AddAndDelay({a}, {b}) = {sum.ToString()} with delay = {delay.ToString()} seconds.\n");

                    WCFProtocolXConfig WCFProtocol = null;
                    WCFScopeConfig WCFDCA = null;
                    int nChannelInEachMeasurementGroup = 2;

                    // request DCA with return
                    ret = _client.getInstrumentLockWithReturn(sharedInstrument.DCA, sThreadID, sMachineName, ref WCFDCA, ref WCFProtocol, nChannelInEachMeasurementGroup);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLockWithReturn ------ gets {WCFDCA.strName}");

                    // first measurement group switch via DiCon
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} entering getProtocolLock");
                    ret = _client.getProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    ;
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} exiting getProtocolLock");
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} first measurement group switch via DiCon");
                    SetTimer(10);
                    ret = _client.releaseProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseProtocolLock");

                    // nested DCA lock request
                    ret = _client.getInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName, nChannelInEachMeasurementGroup);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLock");
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} 1st measurement group with DCA");
                    SetTimer(10);
                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseInstrumentLock");

                    // second measurement group switch via DiCon
                    ret = _client.getProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getProtocolLock");
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} second measurement group switch via DiCon");
                    SetTimer(1);
                    ret = _client.releaseProtocolLock(sharedProtocol.DiCon, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseProtocolLock");

                    // nested DCA lock request
                    ret = _client.getInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName, nChannelInEachMeasurementGroup);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} getInstrumentLock(sharedInstrument.DCA)");
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} 2nd measurement group with DCA");
                    SetTimer(10);
                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseInstrumentLock");

                    // release the first DCA request with return
                    ret = _client.releaseInstrumentLock(sharedInstrument.DCA, sThreadID, sMachineName);
                    Console.WriteLine($"Machine={sMachineName}, Thread={sThreadID} releaseInstrumentLock");

                    Console.WriteLine($"...........................................................................");
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

        static void Main(string[] args)
        {
            var program = new Program();
            Thread DUT1 = null;
            DUT1 = new Thread(() => { program.dutThread(); });
            Thread DUT2 = null;
            DUT2 = new Thread(() => { program.dutThread(); });

            DUT1.Start();
            DUT2.Start();

        }
    }
}
