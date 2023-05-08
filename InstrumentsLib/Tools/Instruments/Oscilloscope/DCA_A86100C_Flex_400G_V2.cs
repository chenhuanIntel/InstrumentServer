//
// V2 differences between the original
// 1. allow seperation of acquire and TDECQ Equalizaer, and AVerage operators
// 2. quite a few unused procedures are removed
//
// when using V2, no need to change PAM4 test lib
// but it will wait for the completion of one measurement group before advancing to the next
// however, in V3, change PAM4 test lib is needed. 
// then as soon as waveform is sent for offload computing, DiCon switch adavces to the next measurement group for waveform acqusition and so on
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InstrumentsLib.Tools.Core;
using Utility;
using System.IO;
using NationalInstruments.VisaNS;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumDCAOptions
    {
        /// <summary>
        /// as of selected Equalization preset file
        /// </summary>
        Default, 
        /// <summary>
        /// override with switching xxx off
        /// </summary>
        OFF,
        /// <summary>
        /// override forcefully switching xxx on
        /// </summary>
        ON
    }


    /// <summary>
    ///  Note: need to send as strings in SCPI!
    /// </summary>
    public enum EnumTEQualizerPreset
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("Fiber Channel PI - 7 Rev 0.05")]
        FCPI05,
        /// <summary>
        /// 
        /// </summary>
        [Description("Fiber Channel PI - 7 Rev 0.13")]
        FCPI13,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3bs Draft 2.2")]
        BS22,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3bs Draft 3.2")]
        BS32,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3bs Final")]
        BSFinal,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3cd Draft 3.0")]
        CD30,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3cd Draft 3.5")]
        CD,
        /// <summary>
        /// 
        /// </summary>
        [Description("IEEE 802.3cd Final")]
        CDFinal
    }

    /// <summary>
    ///  Note: need to send as strings in SCPI!
    /// </summary>
    public enum EnumEyeLinearityDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        RLMC94,
        /// <summary>
        /// 
        /// </summary>
        RLMA120,
        /// <summary>
        /// 
        /// </summary>
        EYE
    }

    /// <summary>
    ///  Note: need to send as strings in SCPI!
    /// </summary>
    public enum EnumEyeWidthOpeningDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        ZHITs,
        /// <summary>
        /// 
        /// </summary>
        PROBability
    }


    /// <summary>
    /// 
    /// </summary>
    public class DCA_A86100CFlex400GConfig_V2 : DCA_A86100CFlex400GConfig
    {

        /// <summary>
        /// 
        /// </summary>
        public DCA_A86100CFlex400GConfig_V2()
		{
			dMaxBusyTimeoutSecs = 2.0 * 5.0;
            arChannels = new List<string>
            {
                "1A",
                "1B",
                "1C",
                "1D"
            };

            dImageSaveWaitTime = 0;

            bShrinkEyeFile = true;

            CompressLevel = 8;

            mapDCASettings = new Dictionary<string, DCASettings>();

            TDECQOffset = 0.0;
            OffsetTDECQMinLimit = 1.0;

            buildTargetClassInfo(typeof(DCA_A86100C_Flex400G));

        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class DCA_A86100C_Flex400G_V2 : InstrumentsLib.Tools.Core.InstrumentX, IScope
	{
        #region Computation offloaded




        class ComputeHostAllocator
        {
            private readonly Dictionary<ComputeInstrumentBase, Semaphore> m_computeHostsSemaphore = new Dictionary<ComputeInstrumentBase, Semaphore>();

            static private readonly ComputeHostAllocator m_instance = new ComputeHostAllocator();
            static public ComputeHostAllocator Instance
            {
                get => m_instance;
            }

            public ComputeInstrumentBase Wait(List<ComputeInstrumentBase> computeHosts)
            {
                var semaphores = new Semaphore[computeHosts.Count];
                lock (m_computeHostsSemaphore)
                {
                    // ensure all computeHosts in semaphores
                    for (int i = 0; i < computeHosts.Count; i++)
                    {
                        if (!m_computeHostsSemaphore.TryGetValue(computeHosts[i], out Semaphore semaphore))
                        {
                            semaphore = new Semaphore(1, 1);
                            m_computeHostsSemaphore.Add(computeHosts[i], semaphore);
                        }
                        semaphores[i] = semaphore;
                    }
                }

                // give the first free compute host out of computeHosts
                int index = WaitHandle.WaitAny(semaphores);
                return computeHosts[index];
                //return computeHosts[WaitHandle.WaitAny(semaphores)];
            }

            public void Release(ComputeInstrumentBase computeHost)
            {
                m_computeHostsSemaphore[computeHost].Release();
            }
        }

        class DCAMAllocator
        {
            private readonly Dictionary<ComputeInstrumentBase, Semaphore> m_computeHostsSemaphore = new Dictionary<ComputeInstrumentBase, Semaphore>();

            static private readonly DCAMAllocator m_instance = new DCAMAllocator();
            static public DCAMAllocator Instance
            {
                get => m_instance;
            }

            public ComputeInstrumentBase Wait(List<ComputeInstrumentBase> computeHosts)
            {
                var semaphores = new Semaphore[computeHosts.Count];
                lock (m_computeHostsSemaphore)
                {
                    // ensure all computeHosts in semaphores
                    for (int i = 0; i < computeHosts.Count; i++)
                    {
                        if (!m_computeHostsSemaphore.TryGetValue(computeHosts[i], out Semaphore semaphore))
                        {
                            semaphore = new Semaphore(1, 1);
                            m_computeHostsSemaphore.Add(computeHosts[i], semaphore);
                        }
                        semaphores[i] = semaphore;
                    }
                }

                // give the first free compute host out of computeHosts
                int index = WaitHandle.WaitAny(semaphores);
                return computeHosts[index];
                //return computeHosts[WaitHandle.WaitAny(semaphores)];
            }

            public void Release(ComputeInstrumentBase computeHost)
            {
                m_computeHostsSemaphore[computeHost].Release();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class ComputeInstrumentBase
        {
            /// <summary>
            /// 
            /// </summary>
            public string HostName { get; set; }
        }
        private static readonly List<ComputeInstrumentBase> _DCAMNodes = new List<ComputeInstrumentBase>();
        static List<ComputeInstrumentBase> arDCAMHost = new List<ComputeInstrumentBase>()
        {
            new ComputeInstrumentBase { HostName = "TCPIP0::MLTLab-06::5025::SOCKET" } // only DCAM at main pc
        };
        private readonly List<ComputeInstrumentBase> _computeNodes = new List<ComputeInstrumentBase>();
        //// the following class instances could be read from inout XML, but we define here for unitTest
        static List<ComputeInstrumentBase> arComputeHost = new List<ComputeInstrumentBase>()
        {
            //new ComputeInstrumentBase { HostName = "172.25.93.233" } // sc lab disaggregation         
            //new ComputeInstrumentBase { HostName = "172.25.93.241" }, // MDB server#2
            //new ComputeInstrumentBase { HostName = "172.25.93.252" }, // sc lab switch test
            new ComputeInstrumentBase { HostName = "172.25.50.71" },  // sc lab FGPA dev   
            new ComputeInstrumentBase { HostName = "172.25.93.243" }, // MDB server#1
            new ComputeInstrumentBase { HostName = "172.25.93.234" },  // sc lab Inphi 400G
            new ComputeInstrumentBase { HostName = "10.3.226.176" } // sc lab engine
        };

        private ComputeInstrumentBase waitDCAM()
        {
            // check if DCAM is available before waveform acquisition
            var DCAMHost = DCAMAllocator.Instance.Wait(_DCAMNodes);

            return DCAMHost;
        }



        private Tuple<List<string>, List<string>> Compute(List<byte[]> patternWaves, int waveCountBase, List<string> myOffloadChannels, 
                                                            CChannelsGroupParallel channelsGroupConfig,
                                                            List<Dictionary<string, string>> arMapMeasuredValues,
                                                            List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus)
        {
            var ret = new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
            List<string> arLogHeaderTask = new List<string>();
            List<string> arLogMsgTask = new List<string>();

            // Don't use Resource Arbiter, do some simple round robin scheduling, needs locking
            var tasks = new Task<Tuple<List<string>, List<string>>>[myOffloadChannels.Count];
            for (int i = 0; i < myOffloadChannels.Count; i++)
                tasks[i] = ComputeSocketUsingComputeNodeAllocatedRoundRobin(patternWaves, waveNames, waveCountBase, i, channelsGroupConfig, arMapMeasuredValues[i], arMapMeasuredValuesStatus[i]);
            Task.WaitAll(tasks);
            // append log msg
            for (int i = 0; i < myOffloadChannels.Count; i++)
            {
                arLogHeaderTask.AddRange(tasks[i].Result.Item1);
                arLogMsgTask.AddRange(tasks[i].Result.Item2);
            }

            ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
            return ret;
        }

        private Task<Tuple<List<string>, List<string>>> ComputeSocketUsingComputeNodeAllocatedRoundRobin(List<byte[]> patternWaves, IReadOnlyList<string> waveNames, int waveCountBase, int i, 
                                        CChannelsGroupParallel channelsGroupConfig,
                                        Dictionary<string, string> MapMeasuredValues,
                                        Dictionary<string, DCAcmd.eMeasureDataStatus> MapMeasuredValuesStatus)
        {
            return Task.Run(() =>
            {
                var ret = new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
                List<string> arLogHeaderTask = new List<string>();
                List<string> arLogMsgTask = new List<string>();
                Stopwatch _stopWatchTask = new Stopwatch();
                _stopWatchTask.Restart();

                var computeHost = ComputeHostAllocator.Instance.Wait(_computeNodes);

                _stopWatchTask.Stop();            
                arLogHeaderTask.Add("wait _computeNodes");
                arLogMsgTask.Add(_stopWatchTask.Elapsed.TotalSeconds.ToString());

                try
                {
                    _stopWatchTask.Restart();
                    Task<Tuple<List<string>, List<string>>> task = ComputeSocket(computeHost, patternWaves?[i], waveNames?[i], waveCountBase + i + 1, channelsGroupConfig, MapMeasuredValues, MapMeasuredValuesStatus);
                    task.Wait();
                    // merge returned log tuple
                    arLogHeaderTask.AddRange(task.Result.Item1);
                    arLogMsgTask.AddRange(task.Result.Item2);

                    _stopWatchTask.Stop();                 
                    arLogHeaderTask.Add("ComputeSocket()");
                    arLogMsgTask.Add(_stopWatchTask.Elapsed.TotalSeconds.ToString());

                }
                finally
                {
                    ComputeHostAllocator.Instance.Release(computeHost);
                }

                ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
                return ret;
            });
        }
        #endregion // end of Computation offloaded

        #region Shared Data Structure




        /// <summary>
        /// 
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// 
            /// </summary>
            Correct,
            /// <summary>
            /// 
            /// </summary>
            Questionable,
            /// <summary>
            /// 
            /// </summary>
            Invalid
        }

#pragma warning disable CA1815 // Override equals and operator equals on value types
        /// <summary>
        /// 
        /// </summary>
        public struct Result
#pragma warning restore CA1815 // Override equals and operator equals on value types
        {
            private double value;
            private string unit;
            private Status status;
            private string reason;

            /// <summary>
            /// 
            /// </summary>
            public string Reason { get => reason; set => reason = value; }
            /// <summary>
            /// 
            /// </summary>
            public double Value { get => value; set => this.value = value; }
            /// <summary>
            /// 
            /// </summary>
            public string Unit { get => unit; set => unit = value; }
            /// <summary>
            /// 
            /// </summary>
            public Status Status { get => status; set => status = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct ResultList
        {
            private List<double> value;
            private string unit;
            private Status status;
            private string reason;

            /// <summary>
            /// 
            /// </summary>
            public string Reason { get => reason; set => reason = value; }
            /// <summary>
            /// 
            /// </summary>
            public List<double> Value { get => value; set => this.value = value; }
            /// <summary>
            /// 
            /// </summary>
            public string Unit { get => unit; set => unit = value; }
            /// <summary>
            /// 
            /// </summary>
            public Status Status { get => status; set => status = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public enum TransferMode
        {
            /// <summary>
            /// 
            /// </summary>
            FileContent,
            /// <summary>
            /// 
            /// </summary>
            Ftp,
            /// <summary>
            /// 
            /// </summary>
            FileServer
        }

        static class SocketTransfer
        {
            [Serializable]
            public class ConnectionClosedException : Exception
            {
                public ConnectionClosedException(string message) : base(message)
                {
                }

                public ConnectionClosedException(string message, Exception innerException) : base(message, innerException)
                {
                }

                public ConnectionClosedException()
                {
                }

                protected ConnectionClosedException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
                {
                    throw new NotImplementedException();
                }
            }

            public static byte[] ReadBytes(Socket socket, int num)
            {
                byte[] bytes = new byte[num];
                for (int offset = 0; offset < num; /**/)
                {
                    int bytesRec = socket.Receive(bytes, offset, num - offset, SocketFlags.None);
                    if (bytesRec == 0)
                        throw new ConnectionClosedException();
                    offset += bytesRec;
                }
                return bytes;
            }

            public static int ReadInt(Socket socket)
            {
                return BitConverter.ToInt32(ReadBytes(socket, 4), 0);
            }

            public static string ReadString(Socket socket)
            {
                return Encoding.ASCII.GetString(ReadIntBytes(socket));
            }

            public static double ReadDouble(Socket socket)
            {
                return BitConverter.ToDouble(ReadBytes(socket, 8), 0);
            }

            public static byte[] ReadIntBytes(Socket socket)
            {
                return ReadBytes(socket, ReadInt(socket));
            }


            public static Result ReadResult(Socket socket)
            {
                return new Result
                {
                    Value = ReadDouble(socket),
                    Status = (Status)ReadInt(socket),
                    Unit = ReadString(socket),
                    Reason = ReadString(socket)
                };
            }

            public static byte[] ConvertToByteArray(IList<ArraySegment<byte>> list)
            {
                var bytes = new byte[list.Sum(asb => asb.Count)];
                int pos = 0;

                foreach (var asb in list)
                {
                    Buffer.BlockCopy(asb.Array, asb.Offset, bytes, pos, asb.Count);
                    pos += asb.Count;
                }

                return bytes;
            }
            /// <summary>
            /// read socket for a list of array segment, which are byte array
            /// then convert each 8-byte byte array as a double number
            /// finally return the list of double number where each double number is a coeefficient in Taps
            /// </summary>
            /// <param name="socket"></param>
            /// <returns></returns>
            public static List<double> ReadDoubleList(Socket socket)
            {
                List<double> lstRet = new List<double>();
                List<ArraySegment<byte>> lstArSegment = new List<ArraySegment<byte>>();
                // first MUST Build the buffers for the receive
                int numTaps = 5; // number of coefficient in Taps
                int numBytes = 8; // double takes 8 bytes while integer takes 4 bytes 
                byte[] buf = new byte[numTaps * numBytes];              
                for (int i = 0; i < numTaps; i++)
                {
                    ArraySegment<byte> arSeg = new ArraySegment<byte>(buf, i * numBytes, numBytes); // use multiples of 8 bytes in buf as receive buffer
                    lstArSegment.Add(arSeg);
                }
                // once receive buffer is built, now to receive from socket
                int bytesReceived = socket.Receive(lstArSegment);
                // convert list of array segment to byte array
                var BA = ConvertToByteArray(lstArSegment);
                for (int i = 0; i < numTaps; i++)
                {
                    var doubleNum = BitConverter.ToDouble(BA, numBytes * i); // for each 8 bytes received, convert to a double number
                    lstRet.Add(doubleNum);
                }
                return lstRet;
            }

            public static ResultList ReadResultList(Socket socket)
            {
                return new ResultList
                {
                    Value = ReadDoubleList(socket), 
                    Status = (Status)ReadInt(socket),
                    Unit = ReadString(socket),
                    Reason = ReadString(socket)
                };
            }

            public static void SendInt(Socket socket, int value)
            {
                int bytesSent = socket.Send(BitConverter.GetBytes(value));
                Debug.Assert(bytesSent == 4);
            }

            public static void SendDouble(Socket socket, double value)
            {
                int bytesSent = socket.Send(BitConverter.GetBytes(value));
                Debug.Assert(bytesSent == 8);
            }

            public static void SendIntBytes(Socket socket, byte[] bytes)
            {
                SendInt(socket, bytes.Length);
                int bytesSent = socket.Send(bytes);
                Debug.Assert(bytesSent == bytes.Length);
            }

            public static void SendString(Socket socket, string str)
            {
                SendIntBytes(socket, Encoding.ASCII.GetBytes(str));
            }

            public static void SendStringList(Socket socket, List<string> arStr)
            {
                foreach (var str in arStr)
                {
                    SendIntBytes(socket, Encoding.ASCII.GetBytes(str));
                }
            }
            public static void SendResult(Socket socket, Result result)
            {
                SendDouble(socket, result.Value);
                SendInt(socket, (int)result.Status);
                SendString(socket, result.Unit);
                SendString(socket, result.Reason);
            }


            public static string GetLocalIpAddressFromAdapter(string adapter)
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                var networkInterface = networkInterfaces.FirstOrDefault(_ => _.Name.Equals(adapter, StringComparison.OrdinalIgnoreCase));
                UnicastIPAddressInformation mostSuitableIp = null;
                var address = GetIpAddress(ref mostSuitableIp, networkInterface);
                if (!string.IsNullOrEmpty(address))
                    return address;
                if (mostSuitableIp != null)
                    return mostSuitableIp.Address.ToString();
                return string.Empty;
            }

            /// <summary>
            /// Used to retrieve the local IP address the Resource Arbiter is running on. 
            /// This is then used to populate the Unlock URL field in the LockResponses.
            /// </summary>
            /// <returns></returns>
            public static string GetLocalIpAddress()
            {
                // see https://stackoverflow.com/questions/6803073/get-local-ip-address
                UnicastIPAddressInformation mostSuitableIp = null;

                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var network in networkInterfaces)
                {
                    var address = GetIpAddress(ref mostSuitableIp, network);
                    if (!string.IsNullOrEmpty(address))
                        return address;
                }

                return mostSuitableIp != null
                    ? mostSuitableIp.Address.ToString()
                    : "";
            }

            private static string GetIpAddress(ref UnicastIPAddressInformation mostSuitableIp, NetworkInterface network)
            {
                if (network == null)
                    return string.Empty;

                if (network.OperationalStatus != OperationalStatus.Up)
                    return string.Empty;

                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    return string.Empty;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (System.Net.IPAddress.IsLoopback(address.Address))
                        continue;

                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }

                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }

                    return address.Address.ToString();
                }

                return string.Empty;
            }
        }
        #endregion // Shared Data Structure

        #region Compute with remote PAM4-SDK
        private string GetHostName(string visaString)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"^TCPIP\d+::(?<hostName>[a-zA-Z_0-9\-\+\.]+)::.+");
            var matchResult = r.Match(visaString);

            return matchResult.Success ? matchResult.Groups["hostName"].Value : string.Empty;
        }
        private string StatusAndReason(Result result)
        {
            if (result.Status != Status.Correct)
                return $"Status: {result.Status}, Reason: {result.Reason}";
            else
                return $"Status: {result.Status}";
        }
        private string StatusAndReason(ResultList result)
        {
            if (result.Status != Status.Correct)
                return $"Status: {result.Status}, Reason: {result.Reason}";
            else
                return $"Status: {result.Status}";
        }
        private void WriteResult(string waveName, string name, Result result, 
                                    Dictionary<string, string> MapMeasuredValues,
                                    Dictionary<string, DCAcmd.eMeasureDataStatus> MapMeasuredValuesStatus)
        {
            MapMeasuredValues.Add(name, result.Value.ToString());
            if (result.Status == Status.Correct)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eCorrect);
            else if (result.Status == Status.Questionable)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eQues);
            else if (result.Status == Status.Invalid)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eError);
            Log($"{waveName} {name} Value: {result.Value}{result.Unit}, {StatusAndReason(result)}");
        }

        private void WriteResultList(string waveName, string name, ResultList result,
                            Dictionary<string, string> MapMeasuredValues,
                            Dictionary<string, DCAcmd.eMeasureDataStatus> MapMeasuredValuesStatus)
        {
            for (int i = 0; i < result.Value.Count; i++)
            {
                MapMeasuredValues.Add(name + i.ToString(), result.Value[i].ToString());
            }
            if (result.Status == Status.Correct)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eCorrect);
            else if (result.Status == Status.Questionable)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eQues);
            else if (result.Status == Status.Invalid)
                MapMeasuredValuesStatus.Add(name, DCAcmd.eMeasureDataStatus.eError);
            Log($"{waveName} {name} Value: {result.Value}{result.Unit}, {StatusAndReason(result)}");
        }

        private void WriteResult(string waveName, string name, Result[] results)
        {
            for (int i = 0; i < results.Length; i++)
                Log($"{waveName} {name} Value[{i}]: {results[i].Value}{results[i].Unit}, {StatusAndReason(results[i])}");
        }


        private Task<Tuple<List<string>, List<string>>> ComputeSocket(ComputeInstrumentBase host, byte[] patternWave, string waveName, int id, 
                                        CChannelsGroupParallel channelsGroupConfig,
                                        Dictionary<string, string> MapMeasuredValues,
                                        Dictionary<string, DCAcmd.eMeasureDataStatus> MapMeasuredValuesStatus)
        //private Task<List<string>> ComputeSocket(ComputeInstrumentBase host, byte[] patternWave, string waveName, int id)
        {
            return Task.Run(() =>
            {
                var ret = new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
                List<string> arLogMsgTask = new List<string>();
                List<string> arLogHeaderTask = new List<string>();
                arLogHeaderTask.Add("TDECQ");
                arLogHeaderTask.Add("server IP");
                arLogHeaderTask.Add("server start time");
                arLogHeaderTask.Add("server wave id");
                arLogHeaderTask.Add("server FTP");
                arLogHeaderTask.Add("server bytes");
                arLogHeaderTask.Add("server Compute()");
                arLogHeaderTask.Add("server sendResult");
                arLogHeaderTask.Add("server end time");

                // Connect to a remote device
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    //host.Log.Info(sw, "Waveform {0}", id);
                    // Establish the remote endpoint for the socket.  
                    // This example uses port 11000 on the local computer.  
                    //IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
                    //IPAddress ipAddress = ipHostInfo.AddressList[0];
                    if (!IPAddress.TryParse(host.HostName, out IPAddress ipAddress))
                    {
                        Log("Invalid IP Address '{0}'.", host.HostName);
                        ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
                        return ret;
                    }
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                    // Create a TCP/IP  socket.  
                    using (Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                    {
                        // Connect the socket to the remote endpoint. Catch any errors.  
                        try
                        {
                            sender.Connect(remoteEP);
                        }
                        catch (Exception e)
                        {
                            Log("Connection to {0}:11000 failed. Check that Server is running and that firewall is not blocking the connection.", host.HostName);
                            Log(e.Message);
                            ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
                            return ret;
                        }

                        string strLog = string.Format("Socket connected to {0} with waveform id={1}", sender.RemoteEndPoint?.ToString(), id);
                        Log(strLog);

                        try
                        {
                            // Send the data through the socket.
                            SocketTransfer.SendInt(sender, id);
                            SocketTransfer.SendString(sender, GetEnumDescription(_DCAsettings.TEQsetup.TdecqConfigurationPreset));
                            SocketTransfer.SendString(sender, GetEnumDescription(_DCAsettings.TEQsetup.TdecqEqualizerPreset));
                            SocketTransfer.SendInt(sender, (int)_DCAsettings.TEQsetup.IterativeOptimization);
                            SocketTransfer.SendInt(sender, (int)_DCAsettings.TEQsetup.EyeLinearityDefinition);
                            SocketTransfer.SendInt(sender, (int)_DCAsettings.TEQsetup.EyeWidthOpeningDefinition);
                            SocketTransfer.SendDouble(sender, _DCAsettings.TEQsetup.EyeWidthOpeningProbability);
                            //TransferMode.FileContent
                            SocketTransfer.SendInt(sender, (int)TransferMode.FileContent);
                            SocketTransfer.SendIntBytes(sender, patternWave);
                            Log("Data Transmitted");

                            Log("waiting results from offload Compute()");
                            // Receive the response from the remote device.
                            Result tdecq = SocketTransfer.ReadResult(sender);
                            WriteResult(waveName, "TDEQ", tdecq, MapMeasuredValues, MapMeasuredValuesStatus);
                            arLogMsgTask.Add(tdecq.Value.ToString());
                            Result ooma = SocketTransfer.ReadResult(sender);
                            WriteResult(waveName, "OOMA", ooma, MapMeasuredValues, MapMeasuredValuesStatus);
                            Result oer = SocketTransfer.ReadResult(sender);
                            WriteResult(waveName, "OER", oer, MapMeasuredValues, MapMeasuredValuesStatus);
                            Result eyeLinearity = SocketTransfer.ReadResult(sender);
                            WriteResult(waveName, "EyeLinearity", eyeLinearity, MapMeasuredValues, MapMeasuredValuesStatus);
                            //Result eyeWidthZeroOne = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeWidthZeroOne", eyeWidthZeroOne);
                            //Result eyeWidthOneTwo = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeWidthOneTwo", eyeWidthOneTwo);
                            //Result eyeWidthTwoThree = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeWidthTwoThree", eyeWidthTwoThree);
                            //Result eyeSkewZeroOne = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeSkewZeroOne", eyeSkewZeroOne);
                            //Result eyeSkewOneTwo = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeSkewOneTwo", eyeSkewOneTwo);
                            //Result eyeSkewTwoThree = SocketTransfer.ReadResult(sender);
                            //WriteResult(waveName, "EyeSkewTwoThree", eyeSkewTwoThree);
                            ResultList tdecqEqualizerTaps = SocketTransfer.ReadResultList(sender);
                            WriteResultList(waveName, "Taps", tdecqEqualizerTaps, MapMeasuredValues, MapMeasuredValuesStatus);

                            // read log strings from server
                            string retLog;
                            const int numLogRetFromServer = 8;
                            for (int i = 0; i < numLogRetFromServer; i++)
                            {
                                retLog = SocketTransfer.ReadString(sender);
                                Log(retLog);
                                arLogMsgTask.Add(retLog);
                            }

                            // Release the socket.  
                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();
                        }
                        catch (Exception e) when (e is SocketException || e is SocketTransfer.ConnectionClosedException)
                        {
                            Log("No or incomplete Results from {0}. Check messages on Server.", sender.RemoteEndPoint?.ToString());
                        }
                        catch (Exception e)
                        {
                            Log(e.ToString());
                        }

                    }
                    string logTmp = string.Format("Waveform id={0} done", id);
                    Log(logTmp);
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
                return ret;
            });
        }

        #endregion // end of Compute with remote PAM4-SDK



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected bool initFunctor(DCAcmd cmd)
        {
            base.initFunctor(cmd);

            cmd.WriteAndCheckCmdStatus = this.WriteAndCheckCmdStatus;
            cmd.ExtractParam = this.ExtractParam;
            cmd._myConfig = this._myConfig;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public class ChannelInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, Eyecmd> mapEyeCmds;

            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, DCAcmd> mapPAM4Cmds;

            /// <summary>
            /// 
            /// </summary>
            public ChannelInfo()
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, ChannelInfo> _mapChannelInfo = new Dictionary<string, ChannelInfo>();

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, Eyecmd> mapEyeCmds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, DCAcmd> mapPAM4Cmds { get; set; }


        /// <summary>
        /// Delay in seconds for ALERT query
        /// </summary>
        protected int DEFAULT_POLLING_DELAY = 1; 
        /// <summary>
        /// 
        /// </summary>
        protected DCA_A86100CFlex400GConfig_V2 _myConfig;
        /// <summary>
        /// 
        /// </summary>
        protected List<int> _channels;

        /// <summary>
        /// 
        /// </summary>
		public string sRun_err { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nDUTidx { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strTestName { get; set; }
        /// <summary>
        /// add this parameter for test time analysis
        /// </summary>
        public bool bSeperateAcqTEQ { get; set; }
        /// <summary>
        /// Property of ScopeConfig for Read Only
        /// </summary>
        public ScopeConfig objScopeConfig => throw new NotImplementedException();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public DCA_A86100C_Flex400G_V2(DCA_A86100CFlex400GConfig_V2 config)
			: base(config)
		{
			_myConfig = config;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="config"></param>
		/// <param name="protocol"></param>
        public DCA_A86100C_Flex400G_V2(DCA_A86100CFlex400GConfig_V2 config, ProtocolX protocol)
			: base(config, protocol)
		{
			_myConfig = config;
		}

		private const int SNIDX = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string query(string cmd)
        {
            cmd += "\n";
            return _ProtocolX.query(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool write(string cmd)
        {
            cmd += "\n";
            return _ProtocolX.write(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool WriteAndCheckCmdStatus(string cmd)
        {
            write(cmd);

            string sRun_err;
            bool bHasError = DetectErrorAndClear(out sRun_err);

            return !bHasError;
        }

        /// <summary>
        /// Initialize the DCA
        /// </summary>
        /// <returns></returns>
        public override bool initialize()
		{
			base.initialize();

			if( _myConfig.bSimulation )
			{
				return true;
			}

			//Query instruments
			string strResult = this.query("*IDN?");

			//Now parse the ID of the DCA frame
			string[] ar = strResult.Split(',');
			this._myConfig.strDCA_ID = ar[SNIDX];

            //setTimeout(600);
			this.SoftReset();

            // no need to load DCA set up during initialize()
            //this.LoadSetupFile(_myConfig.DCAConfigFile);

            // no need to apply any setting during initialize()
            //this.setDCAsettings(DCASettings.INIT_SETTING);

            buildDCAcommands();

            ChannelInfo myChannelInfoRef;
            //Build dictionary of channel info
            for (int i = 0; i < this._myConfig.arChannels.Count; i++)
            {
                myChannelInfoRef = new ChannelInfo();
                myChannelInfoRef.mapEyeCmds = CloneUtil.DeepCopy<Dictionary<string, Eyecmd>>(this.mapEyeCmds);
                myChannelInfoRef.mapPAM4Cmds = CloneUtil.DeepCopy<Dictionary<string, DCAcmd>>(this.mapPAM4Cmds);
                this._mapChannelInfo[this._myConfig.arChannels[i]] = myChannelInfoRef;
               
                this.initializeFunctorCol(myChannelInfoRef.mapEyeCmds);
                this.initializeFunctorCol(myChannelInfoRef.mapPAM4Cmds);
            }

			return true;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapCmd"></param>
        /// <returns></returns>
        protected bool initializeFunctorCol(Dictionary<string, Eyecmd> mapCmd)
        {
            foreach (Eyecmd cmd in mapCmd.Values)
            {
                this.initFunctor(cmd);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapCmd"></param>
        /// <returns></returns>
        protected bool initializeFunctorCol(Dictionary<string, DCAcmd> mapCmd)
        {
            foreach (DCAcmd cmd in mapCmd.Values)
            {
                this.initFunctor(cmd);
            }

            return true;
        }

        #region DCA specific functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetPatLock(string state)
		{
			if( state.Equals("AUTO") )
			{
				SetPatternLock("ON");
			}
			else
			{
				SetPatternLock("OFF");
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern_lock"></param>
		public void SetPatternLock(string pattern_lock)
		{
			if( _config.bSimulation )
			{
				return;
			}

			write(":TRIG:PLOC " + pattern_lock); // 'ON' 'OFF'
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger_src"></param>
        protected void SetTriggerSRC(string trigger_src)
		{
			write(":TRIG:SOUR " + trigger_src); // FPAN
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger_bw"></param>
		protected void SetTriggerBW(string trigger_bw)
		{
			write(":TRIG:BWLimit " + trigger_bw);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points_per_waveform"></param>
		protected void SetPointsPerWaveform(string points_per_waveform)
		{
            if (points_per_waveform.Equals("AUTO"))
            {
                write(":ACQuire:RLENGTH:MODE AUT");
            }
            else
            {
                write(":ACQuire:RLENGTH:MODE " + points_per_waveform);
            }
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sAmount"></param>
        protected void SetMaskMarginValue(int sAmount)
		{
			write(":MTESt1:MARG:PERC " + sAmount);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bWait"></param>
        public void Setautoscale(bool bWait = true)
        {
            clog.MarkStart(strTestName, clog.TimeKey.AutoScale, nDUTidx);
            string res = "";
            DetectErrorAndClear(out res);

            //for (int nTry = 1; nTry < 3; nTry++)
            //{
            write("SYSTEM:AUT");

            if (bWait)
            {
                Busy();
                //write("*CLS");
            }
            clog.MarkEnd(strTestName, clog.TimeKey.AutoScale, nDUTidx);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void turnOffAllChannelDisplay()
        {
            if (null != this._myConfig.arChannels)
            {
                for (int n = 0; n < this._myConfig.arChannels.Count; n++)
                {
                    write(":CHAN" + this._myConfig.arChannels[n] + ":DISPlay OFF");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string EyeParamMap(string param)
		{
			string res = "";

			if( param.Equals("Rise9010") ||
				 param.Equals("Rise8020") ||
				 param.Equals("Rise6040") )
				res = ":MEAS:EYE:RIS";

			if( param.Equals("Fall9010") ||
				 param.Equals("Fall8020") ||
				 param.Equals("Fall6040") )
				res = ":MEAS:EYE:FALL";

			if( param.Equals("EyeHeight") )
				res = ":MEAS:CGR:EHE";

			if( param.Equals("EEyeHeight") )
				res = ":MEAS:EYE:EHE";

			if( param.Equals("EEyeHeightRat") )
				res = ":MEAS:EYE:EHEight:FORMat RATio";

			if( param.Equals("EEyeHeightAmp") )
				res = ":MEAS:EYE:EHEight:FORMat AMPL";

			if( param.Equals("EyeAmp") )
				res = ":MEAS:EYE:AMPL";

			if( param.Equals("ZeroLevel") )
				res = ":MEAS:EYE:ZLEV";

			if( param.Equals("OneLevel") )
				res = ":MEAS:EYE:OLEV";

			if( param.Equals("Xing") )
				res = ":MEAS:EYE:CROS";

			if( param.Equals("JitterDcd") )
				res = ":MEAS:EYE:DCD";

			if( param.Equals("Jitter") )
				res = ":MEAS:EYE:JITT";

			if( param.Equals("JitterPp") )
				res = ":MEAS:EYE:JITTer";

			if( param.Equals("JitterRms") )
				res = ":MEAS:EYE:JITT";

			if( param.Equals("ExtRatioDb") )
				res = ":MEAS:EYE:ERAT";

			if( param.Equals("ExtRatio") )
				res = ":MEAS:EYE:ERAT";

			if( param.Equals("Crossing") )
				res = ":MEAS:EYE:CROS";

			if( param.Equals("AOP") || param.Equals("AOP_WATT") )
				res = ":MEASure:EYE:APOWer";

			if( param.Equals("DCDistortion") )
				res = ":MEASure:EYE:DCDistortion";

			return res;
		}


        /// <summary>
        /// Detects if there are errors in the command buffer and clears at the end of the function if has errors
        /// </summary>
        /// <param name="res">The name of the error response</param>
        /// <returns>true if has error(s); otherwise false</returns>
        protected bool DetectErrorAndClear(out string res)
        {
            bool bHasError = false;
            res = "";

            int error_cnt = 0;
            int.TryParse(query(":SYST:ERR:COUNT?"), out error_cnt);
            if (error_cnt > 0)
            {
                bHasError = true;
                for (int i = 0; i < error_cnt; i++)
                {
                    res = query(":SYST:ERR:NEXT?");
                }
                write("*CLS");
            }

            return bHasError;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string TrigPatLocked()
        {
            if (_config.bSimulation)
            {
                return "";
            }

            string res = "";

            // Poll for completion
            int nValue = 0;
            TimeSpan ts;
            DateTime tmStart = DateTime.UtcNow;
            ts = DateTime.UtcNow - tmStart;
            bool bSucceded = false;
            try
            {
                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    while (false == bSucceded)
                    {
                        if (int.TryParse(query(":TRIGger:PLOCk?"), out nValue))
                        {
                            if (1 == nValue)
                            {

                                bSucceded = true;
                                break;
                                
                            }
                            else
                            {
                                Pause(DEFAULT_POLLING_DELAY);
                            }
                        }
                        else
                        {
                            //todo log error
                            Pause(DEFAULT_POLLING_DELAY);
                        }

                        ts = DateTime.UtcNow - tmStart;
                        Pause(500); // wait for 0.5 sec before the next try
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex, string.Format("Visa Query throw Exception with {0}", ex.GetType().ToString()));
            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Busy()
		{
			if( _config.bSimulation )
			{
				return "";
			}

			string res = "";
			if( sRun_err == "UNABLE_CHAR_EDGE_ERROR" )
			{
				res = sRun_err;
				return res;
			}

			// Poll for completion
			int nValue = 0;
			TimeSpan ts;
			DateTime tmStart = DateTime.UtcNow;
			ts = DateTime.UtcNow - tmStart;
			bool bSucceded = false;
			try
			{
				while( false == bSucceded && ts.TotalSeconds < _myConfig.dMaxBusyTimeoutSecs )
				{
                    if (int.TryParse(query("*OPC?"), out nValue))
                    {
                        if (1 == nValue)
                        {
                            bSucceded = true;
                            res = "1";
                            break;
                        }
                        else
                        {
                            Pause(DEFAULT_POLLING_DELAY);
                            res = "-1";
                        }
                    }
                    else
                    {
                        //todo log error
                        Pause(DEFAULT_POLLING_DELAY);
                        res = "-1";
                    }

                    ts = DateTime.UtcNow - tmStart;
				}
			}
			catch( Exception ex )
			{
				Error(ex, string.Format("Visa Query throw Exception with {0}", ex.GetType().ToString()));
			}

            // Check error outside this function

			return res;
		}

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <param name="mapMeasuredValuesStatus"></param>
        /// <returns></returns>
        protected bool ExtractPAM4Param(CChannelSettings channelConfig, List<string> arParams, 
                                        Dictionary<string, string> mapMeasuredValues,
                                        Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus)
        {
            ChannelInfo myChannelInfo = _mapChannelInfo[channelConfig.strChannelName];

            for (int i = 0; i < arParams.Count; i++)
            {
                myChannelInfo.mapPAM4Cmds[arParams[i]].MapParam(channelConfig.strChannelName, mapMeasuredValues, mapMeasuredValuesStatus);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subSystem"></param>
        /// <param name="param"></param>
        /// <param name="paramStatus"></param>
        /// <returns></returns>
        protected string ExtractParam(string subSystem, string param, out string paramStatus)
        {
            paramStatus = "";

            try
            {
                string strTransition = "", strDatum = "", strRootMeasCmd = "";
                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    // after measurement source changed, now to query the measurment value
                    if ((param == ScopeConst.TRANsitionRISing) || (param == ScopeConst.TRANsitionFALLing))
                    {
                        strDatum = query($":MEAS:{subSystem}:TTIMe?").Trim();
                        strTransition = query($":MEAS:{subSystem}:TTIMe:TRANsition?").Trim(); // RIS, FALL, SLOW
                        if (!param.Contains(strTransition))// check if we are not measuring the correct RIS or FALL transition time
                        {
                            return "-999";
                        }
                        strRootMeasCmd = $":MEASure:{subSystem}:TTIMe"; // :MEASure: EYE: TTIMe: STATus ?

                    }
                    else
                    {
                        strDatum = query($":MEAS:{subSystem}:{param}?").Trim();
                        strRootMeasCmd = $":MEASure:{subSystem}:{param}";
                    }

                    //Add some telemetry debug queries
                    paramStatus = query(strRootMeasCmd + ":STATus?");
                    Log("ExtractParam, strRootMeasCmd={0}, datum={1}, strResult={2}", strRootMeasCmd, strDatum, paramStatus);
                    if ((strDatum.Length > 0) &&
                        (!strDatum.Equals("9.91E+37")) &&
                        (paramStatus.ToUpper().Contains("CORR"))) // if datum == 9.91E+37, NaN,  it implies the measuremnet is not yet ready
                    {
                        return strDatum;
                    }

                    if (paramStatus.ToUpper().Contains("QUES")) // questionable measurement, then autoscale (decided not to, to save test time and allow DCAM to recover itself)
                    {
                        string strReason = query(strRootMeasCmd + ":STATus:REASon?");
                        Log("Questionable reason is " + strReason);
                        //if (nTry < _myConfig.nRetry - 1) // skip the autoscale for the nTry-th try because no need for the last autoscale
                        //{
                        //    Setautoscale(true); // true means must wait till OPC?
                        //}
                    }
                    else if (paramStatus.ToUpper().Contains("INV")) // invalid measurement
                    {
                        string strReason = query(strRootMeasCmd + ":STATus:REASon?");
                        Log("Invalid reason is " + strReason);
                    }
                }
                // still questionable measurement after nRetry
                if (paramStatus.ToUpper().Contains("QUES")) 
                {
                    return strDatum;
                }

                return "-999"; // for invalid measurement
                
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected string GetMaskHitCount(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return "0";
            }

            string res = query(string.Format(":MEAS:MTESt{0}:HITS?", nMask));
            res = res.TrimEnd();

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:HITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:HITS:STATus:REASon?", nMask));
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected string GetMarginHitCount(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return "0";
            }

            string res = query(string.Format(":MEAS:MTESt{0}:MHITS?", nMask));
            res = res.TrimEnd();

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus:REASon?", nMask));
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected double GetMaskHitRatio(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return 0.0;
            }

            double fMaskHitRatio = _ProtocolX.queryDouble(string.Format(":MEASure:MTESt{0}:HRatio?\n", nMask));

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MHITS:STATus:REASon?", nMask));
            }

            return fMaskHitRatio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMask"></param>
        /// <returns></returns>
        protected double GetMaskMarginPct(int nMask = 1)
        {
            if (_myConfig.bSimulation)
            {
                return 0.0;
            }

            double fMaskMarginPct = _ProtocolX.queryDouble(string.Format(":MEASure:MTESt{0}:MARGin?\n", nMask));

            //Diagnostics...
            string strResults = query(string.Format(":MEASure:MTESt{0}:MARGin:STATus?", nMask));
            if (!strResults.Contains("CORR"))
            {
                strResults = query(string.Format(":MEASure:MTESt{0}:MARGin:STATus:REASon?", nMask));
            }
            return fMaskMarginPct;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSetupFile"></param>
        /// <returns></returns>
        public bool LoadSetupFile(string strSetupFile)
        {
            //// override DCA set-up file full file path
            //if (StationHardware.Instance().myConfig.myAppConfig.bUseDynamicSeq)
            //{
            //    // check if set-up file has full path, disregard path just get filename - maintain backward compatibility
            //    string[] sFile = strSetupFile.Split('\\');
            //    if (sFile.Count() > 1)
            //    {
            //        // disregard file path & get filename only
            //        strSetupFile = Path.Combine(StationHardware.Instance().myConfig.myAppConfig.strHardwareFilesLocation, sFile.Last());
            //    }
            //    else
            //    {
            //        // get filename
            //        strSetupFile = Path.Combine(StationHardware.Instance().myConfig.myAppConfig.strHardwareFilesLocation, sFile[0]);
            //    }
            //}

            try
            {
                clog.MarkStart(strTestName, clog.TimeKey.LoadSetupFile, nDUTidx);

                // Erases all data from display memory and resets the data acquisition to the starting data point
                // we have found out that if there are residual data in display memory, when loading setx file, DCAM will not only relock but also carry out TDECQ calculation
                write(":ACQuire:CDISplay");

                // to clear any residual error from previous DCAM operations, in hope to avoid long *OPC? check            
                //Clear all the operation for prevent stopper operation.
                write("*CLS");
                this._ProtocolX.clearBuffer();

                if (_myConfig.bSimulation)
                {
                    return true;
                }

                if (!File.Exists(strSetupFile))
                {
                    Log("Error: file not found" + strSetupFile);
                    throw new Exception("Error: file not found" + strSetupFile);
                }

                for (int nTry = 0; nTry < _myConfig.nRetry; nTry++)
                {
                    try
                    {
                        write(string.Format(":DISK:SETup:RECall:HCONfig OFF")); // Load setup file without reading in DCAM module configuration
                        write(string.Format(":DISK:SETup:RECall \"{0}\"", strSetupFile));

                        // in case loading setx file incurs acquisition, issue a acquire:stop command before checking OPC
                        write(":ACQUIRE:STOP");

                        if (Busy() == "")
                            break;
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString());
                        if (nTry == (_myConfig.nRetry - 1)) throw;
                    }
                }
                return true;
            }
            finally
            {
                clog.MarkEnd(strTestName, clog.TimeKey.LoadSetupFile, nDUTidx);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void SoftReset()
		{
			write("*RST");
		}


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public string getID()
		{
			return this._myConfig.strDCA_ID;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="filter_state"></param>
		protected void SetFilterState(string strChannelName, string filter_state)
		{
			write(":CHAN" + strChannelName + ":FILT " + filter_state);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetFilterState(string strChannelName)
		{
			return query(":CHAN" + strChannelName + ":FILT?").Trim();
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="filter_number"></param>
        protected void SetFilterType(string strChannelName, string filter_number)
		{
			write(":CHAN" + strChannelName + ":FSELect FILT" + filter_number);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetFilterType(string strChannelName)
		{
            try
            {
                string fil = query(":CHAN" + strChannelName + ":FSELect?");
                string[] filter = fil.Split('T');
                int nFilterIndex = Convert.ToInt32(filter[1]);

                return nFilterIndex.ToString();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }

            return "1";
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <param name="strWavelength"></param>
        protected void SetWavelength(string strChannelName, string strWavelength)// WAV1=850 WAV2=1310 WAV3=1550 USER->USER
		{
			try
            {
                write(":CHANnel" + strChannelName + ":WAV " + strWavelength);
            }
            catch (Exception ex)
            {
				Log(ex.ToString());
			}
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannelName"></param>
        /// <returns></returns>
        protected string GetWavelength(string strChannelName)
		{
			if( _config.bSimulation )
			{
				return "1.31E-6";
			}

			string res = "";
			try
			{
				res = query(":CHANnel" + strChannelName + ":WAVelength?").ToUpper();

				if( res.Contains("WAV") )
				{
					res = res.Substring(3);
				}
				else if( res.Contains("USER") )
				{
					res = "1310";
				}
			}
			catch( Exception ex )
			{
				Error(ex, "DCA_A86100C_Flex.GetWavelength");
			}


			res = res.TrimEnd();
			return res;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel"></param>
        /// <param name="param"></param>
        /// <returns></returns>
		protected bool SetupEyeParamMap(string strChannel, string param)
		{
			if( param.Equals("Rise9010") ||
				param.Equals("Rise8020") ||
				param.Equals("Rise6040") )
			{
                write(":MEAS:EYE:RIS:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:RIS");
			}

			//:MEASure:TBASe:METHod STANdard
			//:MEASure:THReshold:METHod UDEFined
			//:MEASure:THReshold:DISTal 6.00E+1
			//:MEASure:THReshold:PROXimal 4.00E+1
			//:MEASure:EBOundary:LEFT 4.90E+1
			//:MEASure:EBOundary:RIGHt 5.10E+1
			if( param.Equals("Rise6040") )
			{
				write(":MEASure:TBASe:METHod STANdard");
				write(":MEASure:THReshold:METHod UDEFined");
				write(":MEASure:THReshold:DISTal 6.00E+1");
				write(":MEASure:THReshold:PROXimal 4.00E+1");
				write(":MEASure:EBOundary:LEFT 4.90E+1");
				write(":MEASure:EBOundary:RIGHt 5.10E+1");
				return true;
			}

			if( param.Equals("Fall9010") ||
				param.Equals("Fall8020") ||
				param.Equals("Fall6040") )
			{
                write(":MEAS:EYE:FALL:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:FALL");
			}

			if( param.Equals("EyeHeight") )
			{
                write(":MEAS:EYE:EHE:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:EHE");
			}

			if( param.Equals("EyeAmp") )
			{
                write(":MEAS:EYE:AMPL:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:AMPL");
			}

			if( param.Equals("ZeroLevel") )
			{
                write(":MEAS:EYE:ZLEV:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:ZLEV");
			}

			if( param.Equals("OneLevel") )
			{
                write(":MEAS:EYE:OLEV:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:OLEV");
			}

			if( param.Equals("Xing") )
			{
                write(":MEAS:EYE:CROS:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:CROS");
			}

			if( param.Equals("JitterDcd") )
			{
                write(":MEAS:EYE:DCD:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:DCD");
			}

			if( param.Equals("JitterPp") )
			{
                write(":MEAS:EYE:JITTer:SOUR CHAN" + strChannel);
				write(":MEASure:EYE:JITTer:FORMat PP");
				return write(":MEAS:EYE:JITT");
			}

			if( param.Equals("JitterRms") )
			{
                write(":MEAS:EYE:JITTer:SOUR CHAN" + strChannel);
				write(":MEASure:EYE:JITTer:FORMat RMS");
				return write(":MEAS:EYE:JITT");
			}

			if( param.Equals("ExtRatioDb") )
			{
                write(":MEAS:EYE:ERAT:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:ERAT");
			}

			if( param.Equals("ExtRatio") )
			{
                write(":MEAS:EYE:ERAT:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:ERAT RAT");
			}

			if( param.Equals("Crossing") )
			{
                write(":MEAS:EYE:CROS:SOUR CHAN" + strChannel);
				return write(":MEAS:EYE:CROS");
			}

			if( param.Equals("AOP") )
			{
                write(":MEAS:EYE:APOWer:SOUR CHAN" + strChannel);
				write(":MEASure:EYE:APOWer");
				return write(":MEASure:EYE:APOWer:UNITs dBm");
			}

			if( param.Equals("AOP_WATT") )
			{
                write(":MEAS:EYE:APOWer:SOUR CHAN" + strChannel);
				write(":MEASure:EYE:APOWer");
				return write(":MEASure:EYE:APOWer:UNITs WATT");
			}

			if( param.Equals("DCDistortion") )
			{
                write(":MEAS:EYE:DCDistortion:SOUR CHAN" + strChannel);
				write(":MEASure:EYE:DCDistortion");
				return write(":MEASure:EYE:DCDistortion:FORMat PERCent");
			}

            if (param.Equals("TDEC"))
            {
                write(":MEAS:EYE:TDEC:SOUR CHAN" + strChannel);
                return write(":MEASure:EYE:TDEC");
            }


            return false;
		}


        /// <summary>
        /// 
        /// </summary>
		public const string Min = "Min";
        /// <summary>
        /// 
        /// </summary>
		public const string Max = "Max";
        /// <summary>
        /// 
        /// </summary>
		public const string Mean = "Mean";
        /// <summary>
        /// 
        /// </summary>
		public const string Std = "Std";

        /// <summary>
        /// 
        /// </summary>
		protected static Dictionary<string, string> s_mapSCPI_StatsMeasType;
        /// <summary>
        /// 
        /// </summary>
		public static List<string> s_arMeasurementStatisticalType;
        /// <summary>
        /// 
        /// </summary>
		public static void build_arMeasurementStatisticalType()
		{
			if( s_arMeasurementStatisticalType != null )
			{
                s_arMeasurementStatisticalType = new List<string>
                {
                    Min,
                    Max,
                    Mean,
                    Std
                };

                s_mapSCPI_StatsMeasType = new Dictionary<string, string>
                {
                    { Min, ":MIN?" },
                    { Max, ":MAX?" },
                    { Mean, ":MEAN?" },
                    { Std, ":SDEV?" }
                };
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool buildDCAcommands()
        {
            this.mapPAM4Cmds = new Dictionary<string, DCAcmd>
            {
                [ScopeConst.AOP] = new AOPcmd(),
                [ScopeConst.TDEQ] = new TDEQcmd(),
                [ScopeConst.OOMA] = new OOMAcmd(),
                [ScopeConst.OMAX] = new OMAXcmd(),
                [ScopeConst.TTIME] = new TTIMEcmd(),
                [ScopeConst.CEQ] = new CEQcmd(),
                [ScopeConst.OER] = new OERcmd(),
                [ScopeConst.NMARGIN] = new NMARGINcmd(),
                [ScopeConst.PSER] = new PSERcmd(),
                [ScopeConst.PTDEQ] = new PTDEQcmd(),
                [ScopeConst.PNMARGIN] = new PNMARGINcmd(),
                [ScopeConst.TAPS] = new Tapscmd(),
                [ScopeConst.TAPS_FIXED_PRECURSER] = new Tapscmd(1),
                [ScopeConst.PAM4LIN] = new PAM4LINcmd(),
                [ScopeConst.PAM4LINSOURCE] = new PAM4LINSOURCEcmd(),
                [ScopeConst.EyeHeightPAM4] = new EyeHeightPAM4cmd(),
                [ScopeConst.PAM4LEVEL] = new PAM4Levelcmd(),
                [ScopeConst.PAM4LEVELSOURCE] = new PAM4LevelSOURCEcmd(),
                [ScopeConst.TRANsitionRISing] = new TRANsitionRISingcmd(),
                [ScopeConst.TRANsitionFALLing] = new TRANsitionFALLingcmd(),
                [ScopeConst.TRANsitionRISingMin] = new TRANsitionRISingcmd(),
                [ScopeConst.TRANsitionFALLingMin] = new TRANsitionFALLingcmd()
            };

            this.mapEyeCmds = new Dictionary<string, Eyecmd>
            {
                [ScopeConst.AOP_WATT] = new AOP_WATTcmd(),
                [ScopeConst.EyeAmp] = new EyeAmpcmd(),
                [ScopeConst.ZeroLevel] = new ZeroLevelcmd(),
                [ScopeConst.OneLevel] = new OneLevelcmd(),
                [ScopeConst.Xing] = new Xingcmd(),
                [ScopeConst.JitterDcd] = new JitterDcdcmd(),
                [ScopeConst.JitterPp] = new JitterPpcmd(),
                [ScopeConst.ExtRatioDb] = new ExtRatioDbcmd(),
                [ScopeConst.ExtRatio] = new ExtRatiocmd(),
                [ScopeConst.Crossing] = new Crossingcmd(),
                [ScopeConst.DCDistortion] = new DCDistortioncmd(),
                [ScopeConst.TDEC] = new TDECcmd()
            };

            // Now override the DCAM commands if applicable...
            if (null != this._myConfig.mapOverridenMeasCmds)
            {
                foreach(string cmdName in this._myConfig.mapOverridenMeasCmds.Keys)
                {
                    if (this._myConfig.mapOverridenMeasCmds[cmdName] is Eyecmd)
                    {
                        this.mapEyeCmds[cmdName] = (Eyecmd)this._myConfig.mapOverridenMeasCmds[cmdName];
                    }

                    if (this._myConfig.mapOverridenMeasCmds[cmdName] is DCAcmd)
                    {
                        this.mapPAM4Cmds[cmdName] = (DCAcmd)this._myConfig.mapOverridenMeasCmds[cmdName];
                    }
                }
            }

            // Set TDECQ Offset for TDECQ command
            if (this.mapPAM4Cmds.ContainsKey(ScopeConst.TDEQ))
            {
                TDEQcmd cmd = (TDEQcmd) this.mapPAM4Cmds[ScopeConst.TDEQ];
                cmd.TDECQOffset = _myConfig.TDECQOffset;
                cmd.OffsetTDECQMinLimit = _myConfig.OffsetTDECQMinLimit;
                if (_myConfig.mapTDECQPiecewiseLinearFit != null)
                {
                    if (_myConfig.mapTDECQPiecewiseLinearFit.Count >= 2)
                    {
                        cmd.mapTDECQPiecewiseLinearFit = _myConfig.mapTDECQPiecewiseLinearFit;
                    }
                    else
                    {
                        MessageBox.Show($"Error in DCA_A86100C_Flex400G {_myConfig.strName}. mapTDECQPiecewiseLinearFit has {_myConfig.mapTDECQPiecewiseLinearFit.Count}, map needs to have >= 2 entries ");
                    }
                }
            }

            // Set OER Offset for OER command
            if (this.mapPAM4Cmds.ContainsKey(ScopeConst.OER))
            {
                OERcmd cmd = (OERcmd)this.mapPAM4Cmds[ScopeConst.OER];
                cmd.OERFactor = _myConfig.OERFactor;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDCAFileName"></param>
        public void saveScreen(string strDCAFileName)
        {
            Log("saveScreen()");
            if (_config.bSimulation)
            {
                return;
            }

            int nSaveImageTrial = 0;
            bool bImageEx = false;

            StationHardware _stationInstance;
            //ModuleLevelTestAppConfig _myAppConfig;
            _stationInstance = StationHardware.Instance();
            //_myAppConfig = (ModuleLevelTestAppConfig)_stationInstance.myConfig.myAppConfig;

            int nRemotePathLength = -1;
            //string strRawDCAFolderPath, strRawDCAFileFullName, strRawDCAFilePath, strlocalFilePath, strLocalFileRawDir, strLocalFileRawName = "";

            //if (_myAppConfig != null)
            //{
            //    //Create a new Raw folder (ON DCA)
            //    strRawDCAFolderPath = CAppendDir.appendDir(Path.GetDirectoryName(strDCAFileName), "Raw");
            //    strRawDCAFileFullName = Path.Combine(strRawDCAFolderPath, Path.GetFileName(strDCAFileName));
            //    strRawDCAFilePath = Path.GetDirectoryName(strRawDCAFileFullName);//Just a sanity check

            //    //Now build Local Directories and file anmes
            //    nRemotePathLength = _myAppConfig.DCA_RemoteImagePath.Length;
            //    strlocalFilePath = _myAppConfig.LocalHSOutputDir + strDCAFileName.Substring(nRemotePathLength);
            //    strLocalFileRawDir = CAppendDir.appendDir(Path.GetDirectoryName(strlocalFilePath), "Raw");
            //    strLocalFileRawName = Path.Combine(strLocalFileRawDir, Path.GetFileName(strlocalFilePath));

            //    if (!System.IO.Directory.Exists(strLocalFileRawDir))
            //    {
            //        System.IO.Directory.CreateDirectory(strLocalFileRawDir);
            //    }
            //}
            //else
            //{
            //    Exception ex = new Exception("DCA_A86100C_FlexV2.SaveScreen: _myAppConfig is null");
            //    Error(ex, "_myAppConfig is null");
            //    throw new Exception("DCA_A86100C_FlexV2.SaveScreen: _myAppConfig is null");
            //}


            string noImageMes = "No image saved!";
            if (nRemotePathLength > 0)
            {
                do
                {
                    if (nSaveImageTrial > 5)
                    {
                        Log(noImageMes);
                        throw new Exception(noImageMes);
                    }

                    write(":DISK:SIMage:INVert ON");
                    //write(":DISK:SIM:FNAME '" + strRawDCAFileFullName + "'");
                    write(":DISK:SIM:SAVE");
                    Busy(); // wait till image is saved to avoid IO exception during JPEG shrinking or File.Copy(), or File.Exists

                    //bImageEx = File.Exists(strLocalFileRawName);

                    nSaveImageTrial++;
                    if (!bImageEx)
                    {
                        Pause(Convert.ToInt16(_myConfig.dImageSaveWaitTime) * nSaveImageTrial);
                        Log("Time delay {0}ms", _myConfig.dImageSaveWaitTime * nSaveImageTrial);
                    }
                } while (bImageEx == false);
                Log("Image is saved in #" + nSaveImageTrial + " times.");
            }
            else
            {
                bImageEx = true;
                Log("No save image directory assigned and no verification");
            }


            //if (this._myConfig.bShrinkEyeFile)
            //{
            //    Utility.CImageConverter.ChangeJPGImageQuality(strLocalFileRawName, strlocalFilePath, _myConfig.CompressLevel);
            //}
            //else
            //{
            //    // Will overwrite if the destination file already exists.
            //    File.Copy(strLocalFileRawName, strlocalFilePath, true);
            //}

        }

        /// <summary>
        /// Function to save waveform data
        /// </summary>
        /// <param name="filename">Filename to save as</param>
        /// <param name="mapOptions">Dictionary of Options to save</param>
        /// <returns>true: success; false: fail</returns>
        public virtual bool SaveWaveformData(string filename, Dictionary<string, string> mapOptions)
        {
            throw new NotImplementedException();

            //return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="strImageChannelName"></param>
        /// <param name="bSelectMask"></param>
        public void saveScreenMultiChannel(string filename, string strImageChannelName, bool bSelectMask = false)
        {
            Log("saveScreenMultiChanne()");
            
            write(":DISPLAY:WINDOW:TIME1:DMODE ZTILE");
            write(":DISPLAY:WINDOW:TIME1:ZSIGNAL " + strImageChannelName);
            
            // Select the mask if applicable
            if (bSelectMask)
            {
                int nMask = _mapChannelToMask[strImageChannelName];
                write(string.Format(":DISPlay:TMASk MASK{0}", nMask));
            }

            saveScreen(filename); // removed the delay inside (duplicate)
        }



        private string SetupEyeChannelConfig(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            string sResponse;
            //Display Channel
            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            //Apply ATTEN (i.e. station calibration)
            if (channelConfig.strAttenState.Contains("ON"))
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE ON");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("1"))
                {
                    Log("Error: :ATT:STATE ON failed");
                    return null;
                }
                write(":CHAN" + channelConfig.strChannelName + ":ATT:DEC " + channelConfig.strAttenVal);
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:DEC?");
                if (Convert.ToDouble(sResponse) != Convert.ToDouble(channelConfig.strAttenVal))
                {
                    Log("Error: ::ATT:DEC failed");
                    return null;
                }
            }
            else
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE OFF");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("0"))
                {
                    Log("Error: :ATT:STATE OFF failed");
                    return null;
                }
            }

            string strKey = "Wavelength";
            //Setup channels wavelength

            SetWavelength(channelConfig.strChannelName, channelConfig.strWavelength);
            string strWavelength = GetWavelength(channelConfig.strChannelName);
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strWavelength);
            

            //Setup filters
            //Turn ON filter if applicable
            SetFilterState(channelConfig.strChannelName, channelConfig.strFilterState);
            SetFilterType(channelConfig.strChannelName, channelConfig.strFilter);

            string strFilterState = GetFilterState(channelConfig.strChannelName).Trim();
            strKey = "FilterState";
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strFilterState);

            string strFilterType = GetFilterType(channelConfig.strChannelName).Trim();
            strKey = "FilterType";
            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
            mapMeasuredValues.Add(strKey, strFilterType);

            string strParam = "";
            for (int i = 0; i < arParams.Count; i++)
            {
                strParam = arParams[i];
                SetupEyeParamMap(channelConfig.strChannelName, strParam);
            }
            return strKey;
        }


        private bool SetupPAM4ChannelConfig(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            string sResponse;
            //Display Channel
            write(":CHAN" + channelConfig.strChannelName + ":DISP ON");

            //Apply ATTEN (i.e. station calibration)
            if (channelConfig.strAttenState.Contains("ON"))
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE ON");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("1"))
                {
                    Log("Error: :ATT:STATE ON failed");
                    return false;
                }
                write(":CHAN" + channelConfig.strChannelName + ":ATT:DEC " + channelConfig.strAttenVal);
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:DEC?");
                if (Convert.ToDouble(sResponse) != Convert.ToDouble(channelConfig.strAttenVal))
                {
                    Log("Error: ::ATT:DEC failed");
                    return false;
                }
            }
            else
            {
                write(":CHAN" + channelConfig.strChannelName + ":ATT:STATE OFF");
                sResponse = query(":CHAN" + channelConfig.strChannelName + ":ATT:STATE?");
                if (!sResponse.Contains("0"))
                {
                    Log("Error: :ATT:STATE OFF failed");
                    return false;
                }
            }

            ChannelInfo myChannelInfo = _mapChannelInfo[channelConfig.strChannelName];

            // if bSetupParam is true, then clear setting in .setx file and SetupParam by SW
            // otherwise use settings in *.setx file (default)
            if (_DCAsettings.bSetupParam)
            {
                for (int i = 0; i < arParams.Count; i++)
                {
                    myChannelInfo.mapPAM4Cmds[arParams[i]].SetupParam(channelConfig.strChannelName);
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskHits = "MaskHits";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskHitRatio = "MaskHitRatio";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MarginHits = "MarginHits";
        /// <summary>
        /// 
        /// </summary>
        public const string params_MaskMarginPct = "MaskMarginPct";

        private Dictionary<string, int> _mapChannelToMask = new Dictionary<string, int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="arMapMaskValues"></param>
        /// <returns></returns>
        public bool captureEyeMeasureMaskMarginParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues, out List<Dictionary<string, string>> arMapMaskValues)
        {
            // lock CRU
            if (channelsGroupConfig.bLockCRU)
            {
                if (null != _DCAsettings.CRUsetup)
                {
                    _DCAsettings.CRUsetup.Setup();
                }
                //_DCAsettings.PAM4setup.setup();
            }

            arMapMeasuredValues = new List<Dictionary<string, string>>();
            arMapMaskValues = new List<Dictionary<string, string>>();

            CChannelSettings channelConfig;
            List<string> arParams;
            Dictionary<string, string> mapMeasuredValues;
            Dictionary<string, string> mapMaskValues;

            //:CHAN3A:FILTer ON
            //:CHAN3A:WAVelength WAVelength2
            //:MEASure:THReshold:METHod P205080
            //:MEASure:EYE:LIST:REMove 1
            //:MEASure:EYE:ERATio
            //:MEASure:EYE:APOWer
            //:MEASure:EYE:RISetime
            //:MEASure:EYE:FALLtime
            //:MTESt1:LOAD
            //:MTESt1:MARGin:STATe ON
            //:MTESt1:MARGin:AUTo:METHod HRATio
            //:ACQuire:RUN
            //*OPC?
            //:SYSTem:AUToscale
            //*OPC?
            //:LTESt:ACQuire:STATe ON
            //*OPC?
            //:LTESt:ACQuire:CTYPe:WAVeforms 840
            //:ACQuire:CDISplay

            // DCA setup file was loaded in HS_Test runHSBaseTest()
            ////Load the setup file if neccessary
            //if (null != _DCAsettings)
            //{
            //    if (null != _DCAsettings.sDCAConfigFile)
            //    {
            //        if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
            //        {
            //            LoadSetupFile(_DCAsettings.sDCAConfigFile);
            //        }
            //    }
            //}

            write("*CLS");

            write(":LTESt:ACQuire:STATe OFF");

            write(":MEAS:THR:METHOD P205080");

            write(":SYSTem:MODE EYE");
            write(":SLOT2:TRIGger:TRACking ON");
            SetTriggerSRC("FPAN");
            SetTriggerBW("DIV");

            turnOffAllChannelDisplay();

            SetPointsPerWaveform(_DCAsettings.EyePointsPerWaveform);
            SetMaskMarginValue(_DCAsettings.MaskMarginValue);

            SetPatLock("MAN");
            //Setautoscale();
            //query("*OPC?");
            //write("*CLS");

            int c;
            int nMask = 1;

            write(":MEASure:EYE:LIST:CLEAR");

            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = new Dictionary<string, string>();
                arMapMeasuredValues.Add(mapMeasuredValues);

                mapMaskValues = new Dictionary<string, string>();
                arMapMaskValues.Add(mapMaskValues);

                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;
                SetupEyeChannelConfig(channelConfig, arParams, mapMeasuredValues);
                nMask = c + 1;//channelsGroupConfig.arChannels[c].nChannel + 1;
                if (channelConfig.bEnableMask)
                {
                    _mapChannelToMask[$"CHAN{channelConfig.strChannelName}"] = nMask;
                    write(string.Format(":MTEST{0}:SOUR CHAN{1}", nMask, channelConfig.strChannelName));
                    write(string.Format(":MTESt{0}:ALIGnment:X AUTomatic", nMask));
                    query(string.Format(":MTESt{0}:ALIGnment:X?", nMask));

                    //Define and Load Mask File
                    write(string.Format(":MTESt{0}:LOAD:FNAMe \"{1}\"", nMask, channelConfig.MaskFileNameWithPath));
                    write(string.Format(":MTESt{0}:LOAD", nMask));

                    //setup mask margin
                    write(string.Format(":MTESt{0}:MARGin:STATe ON", nMask));
                    write(string.Format(":MTESt{0}:MARGin:METHod AUTO", nMask));
                    write(string.Format(":MTESt{0}:MARGin:AUTO:METHod HRATio", nMask));
                    write(string.Format(":MTESt{0}:MARGin:AUTO:HRATio 5e-5", nMask));
                }
                else
                {
                    write(string.Format(":MTESt{0}:MARGin:STATe OFF", nMask));
                }
            }

            //The 4 tiles split windows view
            write(":DISPLAY:WINDOW:TIME1:DMODE TILED");

            Pause(1000);

            write(":ACQuire:REYE ON");

            write(":ACQuire:REYE:ALIGn ON");

            write(":TIMebase:BRATe 2.5781250E+10");


            Setautoscale();

            //set acquisition limit
            write(":ACQuire:CDISplay"); // clear display
            // the following are now defined in DCA setup file and we do not want it to be overwritten by _DCAsettings.EyeAcqWavLimVal
            //// the DCA setting file defines limit using samples, here changed to waveforms
            //write(string.Format(":LTESt:ACQuire:CTYPe  WAVeforms")); // to switch to use waveform for limit test
            //write(string.Format(":LTESt:ACQuire:CTYPe:WAVeforms {0}", _DCAsettings.EyeAcqWavLimVal)); // to set up the number of waveforms in eye test
            // enable limit test so that we can use OPC to check status upon limit is reached
            write(":LTESt:ACQuire:STATe ON");

            //Select Eye/Mask Mode and Autoscale
            write(":ACQ:RUN");

            // wait for limit to reach
            string strResult = Busy();

            //Retrieves the params
            for (c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = arMapMeasuredValues[c];
                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                arParams = channelsGroupConfig.arChannels[c].arParams;

                ExtractEyeMeasurements(channelConfig, arParams, mapMeasuredValues);

                nMask = c + 1;//channelsGroupConfig.arChannels[c].nChannel + 1;

                mapMaskValues = arMapMaskValues[c];
                //Get the Mask Values...
                try
                {
                    mapMaskValues[params_MaskHits] = "9999";
                    mapMaskValues[params_MarginHits] = "9999";
                    mapMaskValues[params_MaskHitRatio] = "9999";
                    mapMaskValues[params_MaskMarginPct] = "9999";

                    //Specify Channel Source
                    //write(string.Format("MTEST{0}:SOUR CHAN{1}", nMask, channelConfig.strChannelName));
                    write(string.Format(":DISPlay:TMASk MASK{0}", nMask));

                    if (channelConfig.bEnableMask)
                    {
                        mapMaskValues[params_MaskHits] = GetMaskHitCount(nMask);
                        mapMaskValues[params_MarginHits] = GetMarginHitCount(nMask);
                        mapMaskValues[params_MaskMarginPct] = GetMaskMarginPct(nMask).ToString();
                        mapMaskValues[params_MaskHitRatio] = GetMaskHitRatio(nMask).ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }

            // do not turn off so that limit will show in screen capture
            // turn off limit test only after measurement
            //write(":LTESt:ACQuire:STATe OFF");

            return true;
        }



        private readonly HashSet<int> _functAlreadyWired = new HashSet<int>();
        /// <summary>
        /// add an function operator whose output is func (a function number)
        /// define the function's single input operand;
        /// then add the function output as a measurement to display
        /// </summary>
        /// <param name="func"></param> function number
        /// <param name="Operator"></param> an function operator
        /// <param name="Operand1"></param> the function's single input operand
        private void addFunct(int func, string Operator, string Operand1)
        {
            // add a function operator
            write(string.Format(":FUNC{0}:FOPerator {1}", func, Operator));
            // define the input operand of the operator
            write(string.Format(":FUNC{0}:OPERand1 CHAN{1}", func, Operand1));
            // turn on display to func
            write(string.Format(":FUNC{0}:DISPlay ON", func));
            // Install a measurement on the newly generated func
            write(string.Format(":MEAS:EYE:TDEQ:SOUR FUNC{0}", func));
            write(":MEAS:EYE:TDEQ");
            // add the newly generated func to _functAlreadyWired
            _functAlreadyWired.Add(func);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public void WireTdecq(int i)
        {
            int funcTEQ = GetFunc(i);
            if (!_functAlreadyWired.Contains(funcTEQ))
            {
                // add TDECQ operator to function number func
                // whose input operand is this._myConfig.arChannels[i]
                // also add to measurement
                addFunct(funcTEQ, "TEQualizer", this._myConfig.arChannels[i]);

                // check which TEQ preset to use
                string strTdecqEqualizerPreset = GetEnumDescription(_DCAsettings.TEQsetup.TdecqEqualizerPreset);
                if (!string.IsNullOrEmpty(strTdecqEqualizerPreset))
                {
                    write(string.Format(":SPRocess{0}:TEQualizer:PRESets '{1}'", funcTEQ, strTdecqEqualizerPreset));

                    // after selecting a pre-defined TEQualizer Preset
                    // we change the following 5 parameters
                    // Taps per UI
                    // Number of Taps
                    // Max Precursors
                    // TDECQ threshold optimization
                    // Iterative Optimization
                    if (_DCAsettings.TEQsetup.TapsPerUI != (int)EnumDCAOptions.Default)
                    {
                        write(string.Format(":SPRocess{0}:TEQualizer:TSPacing:TPUI {1}", funcTEQ, _DCAsettings.TEQsetup.TapsPerUI));
                    }
                    if (_DCAsettings.TEQsetup.NumOfTaps != (int)EnumDCAOptions.Default)
                    {
                        write(string.Format(":SPRocess{0}:TEQualizer:TAPS:COUNt {1}", funcTEQ, _DCAsettings.TEQsetup.NumOfTaps));
                    }
                    if (_DCAsettings.TEQsetup.MaxPrecursors != (int)EnumDCAOptions.Default)
                    {
                        write(string.Format(":SPRocess{0}:TEQualizer:MNPRecursors {1}", funcTEQ, _DCAsettings.TEQsetup.MaxPrecursors));
                    }
                    if (_DCAsettings.TEQsetup.TDEQThresholdOptimization != EnumDCAOptions.Default)
                    {
                        write(string.Format(":MEASure:TDEQ:OHTHresholds {0}", Enum.GetName(typeof(EnumDCAOptions), _DCAsettings.TEQsetup.TDEQThresholdOptimization)));
                    }
                    if (_DCAsettings.TEQsetup.IterativeOptimization != EnumDCAOptions.Default)
                    {
                        write(string.Format(":SPR{0}:TEQ:TAPS:IOPT {1}", funcTEQ, Enum.GetName(typeof(EnumDCAOptions), _DCAsettings.TEQsetup.IterativeOptimization)));
                    }
                }
                // average operator applied to TDECQ optimization only
                // must not apply for TDECQ measurement in PAM4 testing
                if (_DCAsettings.TEQsetup.AverageCount != 0)
                {
                    // get a new function number
                    // BUT resevre the first this._myConfig.arChannels.Count for TDECQ operator output
                    int funcAVE = GetFunc(i + this._myConfig.arChannels.Count);
                    //int funcAVE = GetFunc(i + _DCAsettings.TEQsetup.nChannelUsed);
                    // add AVE operator to function number funcAVE
                    // whose input operand is this._myConfig.arChannels[i]
                    // also add to measurement
                    addFunct(funcAVE, "AVErage", this._myConfig.arChannels[i]);
                    // make AVE output become TEQ's input
                    write(string.Format(":FUNCtion{0}:OPERand1 FUNCtion{1}", funcTEQ, funcAVE));
                    // configure the average count
                    write(string.Format(":SPRocess{0}:AVERage:ECOunt {1}", funcAVE, _DCAsettings.TEQsetup.AverageCount));
                }
               
            }
        }

        // channel-0 to function-1, 0-base to 1-base
        // if a channel uses more than one functions, such as one for TDECQ, one for AVERAGE
        // GetFunc will advance to the next available function number
        private int GetFunc(int i) 
        {
            int func = i + 1; // channel-0 to function-1, 0-base to 1-base
            while (_functAlreadyWired.Contains(func))  // if a channel uses more than one functions, such as one for TDECQ, one for AVERAGE                                       
                func++; // GetFunc will advance to the next available function number
            return func;
        }
        /// <summary>
        /// 
        /// </summary>
        public void removeTEQFromDCASetting()
        {
            // must reset at the beginning of removeTEQFromDCASetting()
            // otherwise, the residual _functAlreadyWired from last time this DCA driver is called will remain in place
            _functAlreadyWired.Clear(); // reset _functAlreadyWired

            string strResult;
            // check to ensure there is no TDECQ equalizer in the setx file
            // if TEQ is in setx file, remove it
            //for (int ch = 0; ch < this._myConfig.arChannels.Count; ch++)
            for (int ch = 0; ch < _DCAsettings.TEQsetup.nChannelUsed; ch++)
            {
                int func = GetFunc(ch);
                strResult = query(string.Format(":FUNCtion{0}:FOPerator?", func)).Trim();// strResult =TEQ is a TEQualizer preset is defined
                if (!strResult.Equals("NONE"))
                {
                    write(string.Format(":FUNCtion{0}:FOPerator NONE", func)); // remove TDECQ equalizer on func
                    //remove AVE if they exist in the setx file; for those setx file with AVE, the corresponding TEQsetup.AverageCount must NOT be equal to 0
                    if (_DCAsettings.TEQsetup.AverageCount != 0)
                    {
                        // get a new function number
                        // BUT resevre the first this._myConfig.arChannels.Count for TDECQ operator output
                        int funcAVE = GetFunc(ch + this._myConfig.arChannels.Count);
                        //int funcAVE = GetFunc(ch + _DCAsettings.TEQsetup.nChannelUsed);
                        write(string.Format(":FUNCtion{0}:FOPerator NONE", funcAVE)); // remove Averge operator on funcAVE
                    }
                }
            }

            // must reset at the end of removeTEQFromDCASetting() as well
            // it is to clear the func used above
            _functAlreadyWired.Clear(); // reset _functAlreadyWired
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        public void DCASetUp(CChannelsGroupParallel channelsGroupConfig)
        {
            string res = "";

            // note that the DCA file was loaded in HS_Test runHSBaseTest()
            ////Load the setup file here is NOT neccessary
            //if (null != _DCAsettings)
            //{
            //    if (null != _DCAsettings.sDCAConfigFile)
            //    {
            //        if (_DCAsettings.sDCAConfigFile.Trim().Length > 0)
            //        {
            //            LoadSetupFile(_DCAsettings.sDCAConfigFile);
            //        }
            //    }
            //}

            if (channelsGroupConfig.bLockCRU)
            {
                if (null != _DCAsettings.CRUsetup)
                {
                    _DCAsettings.CRUsetup.Setup();
                }
                //_DCAsettings.PAM4setup.setup();
            }

            // if acquisition and TEQualizer are seperated processes
            // check to ensure there is no TDECQ equalizer in the setx file 
            bSeperateAcqTEQ = _DCAsettings.TEQsetup.bSeperateAcqTEQ; // add this parameter for test time analysis
            if (_DCAsettings.TEQsetup.bSeperateAcqTEQ)
            {
                removeTEQFromDCASetting(); // if TEQ is in setx file, remove it                 
            }

            DetectErrorAndClear(out res);
            write("*CLS");

            // set acquisition setup => smoothing 
            // default is none
            if (_DCAsettings.bACQSmoothAverage)
            {
                WriteAndCheckCmdStatus(":ACQuire:SMOothing AVERage");
                WriteAndCheckCmdStatus(":ACQuire: ECOunt " + _DCAsettings.ACQSmoothAveNumWaveform.ToString());
            }
            else
            {
                WriteAndCheckCmdStatus(":ACQuire:SMOothing NONE");
            }

            if (_DCAsettings.bSetupParam)
            {
                WriteAndCheckCmdStatus(":MEASure:EYE:LIST:CLEAR");
            }

            //set acquisition limit

            //// acquisition limit for eye test is sample which is defined in DCA setting file
            //// we changed to waveforms TDEQ pattern limit for PAM4 test, it is already defined in DCA setting file
            //// but we repeat to make sure it again
            //// then apply the number of pattern limit,PAM4AcqPatLimVal, defined in test sequencce
            //WriteAndCheckCmdStatus(string.Format(":LTESt:ACQuire:CTYPe PATTerns"));
            //WriteAndCheckCmdStatus(string.Format(":LTESt:ACQuire:CTYPe:PATTerns {0}", _DCAsettings.PAM4AcqPatLimVal));

            // enable limit test so that we can use OPC to check status upon limit is reached
            WriteAndCheckCmdStatus(":LTESt:ACQuire:STATe ON");

            // trigger pattern lock
            for (int nTry = 0; nTry < 2; nTry++)
            {
                WriteAndCheckCmdStatus(":TRIGger:PLOCk ON");
                //Needed to ensure that pattern is locked. 
                // there are two ways to check, one is when bCheckBusyAfterPatternLock==true, to use a slower Busy()
                // the other is only check if tigger is locked by TrigPatLocked
                if (this._myConfig.bCheckBusyAfterPatternLock)
                    this.Busy();
                else // only to check if Triger pattern locked to skip the much longer OPC check which also waits for TDEQ iterative optimiztion
                    this.TrigPatLocked();
            }
        }

        List<string> waveNames = new List<string>();
        List<byte[]> patternWaves = null;
        List<byte[]> patternWavesAllMeasureGroups = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thread"></param>
        public void ExecuteOffloadJoin(Thread thread)
        {
            thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternWavesAllMeasureGroups"></param>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="arMapMeasuredValuesStatus"></param>
        /// <returns></returns>
        public Tuple<List<string>, List<string>> ExecuteOffloadCompute(List<byte[]> patternWavesAllMeasureGroups, 
                                                                       CChannelsGroupParallel channelsGroupConfig,
                                                                       List<Dictionary<string, string>> arMapMeasuredValues,
                                                                       List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus)
        {
            var ret = new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
            List<string> arLogHeaderTask = new List<string>();
            List<string> arLogMsgTask = new List<string>();

            Stopwatch _stopWatchExecuteSingleOffloadCompute = new Stopwatch();
            _stopWatchExecuteSingleOffloadCompute.Restart();
            // patternWavesAllMeasureGroups is for TransferMode.FileContent; hence in FTP mode, it is null
            // waveNamesAllMeasureGroups is for TransferMode.Ftp; it will have values in FileContent mode, but not used in Compute()
            ret = Compute(patternWavesAllMeasureGroups, 0, _DCAsettings.TEQsetup.DCAMeasureChannels, channelsGroupConfig, arMapMeasuredValues, arMapMeasuredValuesStatus);
            // merge returned log tuple
            arLogHeaderTask.AddRange(ret.Item1);
            arLogMsgTask.AddRange(ret.Item2);

            _stopWatchExecuteSingleOffloadCompute.Stop();      
            arLogHeaderTask.Add("OffLoad Compute()");
            arLogMsgTask.Add(_stopWatchExecuteSingleOffloadCompute.Elapsed.TotalSeconds.ToString());

            ret = new Tuple<List<string>, List<string>>(arLogHeaderTask, arLogMsgTask);
            return ret;
        }


        /// <inheritdoc/>
        public bool measurePAM4Parallel(CChannelsGroupParallel channelsGroupConfig, 
                                        out List<Dictionary<string, string>> arMapMeasuredValues, 
                                        out List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus,
                                        bool bMeasureOnly=false, bool bTurnOffAllDispFirst = false)
        {
            clog.MarkStart(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            string res = "";
            arMapMeasuredValues = new List<Dictionary<string, string>>();
            arMapMeasuredValuesStatus = new List<Dictionary<string, DCAcmd.eMeasureDataStatus>>();
            // because cannot use out type in the lambda expresssion, define the following Tmp
            List<Dictionary<string, string>> arMapMeasuredValuesTmp = new List<Dictionary<string, string>>();
            List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatusTmp = new List<Dictionary<string, DCAcmd.eMeasureDataStatus>>();
            Dictionary<string, string> mapMeasuredValuesTmp;
            Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatusTmp;

            CChannelSettings channelConfig;
            Dictionary<string, string> mapMeasuredValues;
            Dictionary<string, DCAcmd.eMeasureDataStatus> mapMeasuredValuesStatus;

            ComputeInstrumentBase DCAMHost = new ComputeInstrumentBase();
            if (_DCAsettings.TEQsetup.bOffloadCompute)
            {
                // clear, then set up _computeNodes
                _computeNodes.Clear();
                _computeNodes.AddRange(arComputeHost);

                // clear, then set up _DCAMNodes
                _DCAMNodes.Clear();
                _DCAMNodes.AddRange(arDCAMHost);
                
                // wait for DCAMHost before AcquireWaveform()
                DCAMHost = waitDCAM();
            }

            // Determine if CRU is locked...
            if (null != _DCAsettings.CRUsetup)
            {
                if (!_DCAsettings.CRUsetup.IsLocked())
                {
                    // Force the measurement to do relocking if there is no CRU Lock and CRU is enabled
                    bMeasureOnly = false;
                    channelsGroupConfig.bLockCRU = true;
                }
            }
            

            clog.MarkStart(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);

            // if bMeasureOnly==false, then do the following DCA setup steps
            // bMeasureOnly is false at the very first time calling this method
            // that is, the following DCA setup is only done once at the very first time calling this method
            if (bMeasureOnly==false) 
            {
                DCASetUp(channelsGroupConfig);
            }

            // when bSetupParam is true, we will ignore the measurement setting in setx file and use the configuration below
            if (_DCAsettings.bSetupParam)
            {
                write(":MEASure:EYE:LIST:CLEAR");
            }

            // the following steps (return array setup, autoscale, run, extract and map measurement) will be executed whether bMeasureOnly or bSetupParam is true or flase
            // to do the following till autoscale to allow DCA to re-lock the number of patterns defined in test sequence
            for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
            {
                mapMeasuredValues = new Dictionary<string, string>();
                arMapMeasuredValues.Add(mapMeasuredValues);

                mapMeasuredValuesTmp = new Dictionary<string, string>();
                arMapMeasuredValuesTmp.Add(mapMeasuredValuesTmp);

                mapMeasuredValuesStatus = new Dictionary<string, DCAcmd.eMeasureDataStatus>();
                arMapMeasuredValuesStatus.Add(mapMeasuredValuesStatus);

                mapMeasuredValuesStatusTmp = new Dictionary<string, DCAcmd.eMeasureDataStatus>();
                arMapMeasuredValuesStatusTmp.Add(mapMeasuredValuesStatusTmp);

                channelConfig = channelsGroupConfig.arChannels[c].channelSetting;
                if (!_DCAsettings.TEQsetup.bOffloadCompute) // only to set up measurements if POR local TDECQ; for offload, the measurements are supposed to take at remote PC, not at DCAFlex
                    SetupPAM4ChannelConfig(channelConfig, channelsGroupConfig.arChannels[c].arParams, mapMeasuredValues); // HY
            }
            // at this point we will encounter conflicts and errors during above a few channel setting FOR loops
            // let's clear them before autoscale
            DetectErrorAndClear(out res);
            clog.MarkEnd(strTestName, clog.TimeKey.SetupMeasurement, nDUTidx);

            // this autoscale must be done otherwise PAM4 eye diagrams are shifted up after the above few channel setting FOR loops
            for (int nTry = 0; nTry < 3; nTry++)
            {
                Setautoscale(); // this autoscale must be done otherwise PAM4 eye diagrams are shifted up after the above few channel setting FOR loops
                if (!DetectErrorAndClear(out res))
                {
                    break;
                }
            }

            Thread thread = null;
            // if acquisition and TEQualizer are seperated processes
            if (_DCAsettings.TEQsetup.bSeperateAcqTEQ)
            {
                
                if (_DCAsettings.TEQsetup.bOffloadCompute)
                {
                    clog.MarkStart(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                    // run acquire run which will acquire additional (PAM4AcqPatLimVal-nTry) patterns to reach the pattern limit
                    // each autoscale() will capture one pattern
                    WriteAndCheckCmdStatus(":ACQ:RUN");
                    // *OPC? in Busy() will ensure all measurements of :ACQ:RUN to complete before proceeding to next section of retrieval
                    // wait for acquisition limit to reach
                    res = Busy();
                    clog.MarkEnd(strTestName, clog.TimeKey.AcquireRun, nDUTidx);

                    // set up waveNames with FileContent file names
                    for (int j = 0; j < _DCAsettings.TEQsetup.DCAMeasureChannels.Count; j++)
                        waveNames.Add($"FileContent_{j + 1}.wfmx"); // +1 is to use 1-based for filename
                    string[] ConfiguredChannels = _DCAsettings.TEQsetup.DCAMeasureChannels.ToArray();
                    patternWaves = GetPatternWaves(waveNames, ConfiguredChannels);
                    // now is ok to release DCAM
                    // release DCAMHost after acquire and pattern save
                    DCAMAllocator.Instance.Release(DCAMHost);

                    if (patternWavesAllMeasureGroups == null) // for the very first time, it is a null; we cannot define it as empty string because of null condition check in ComputeSocket()
                        patternWavesAllMeasureGroups = new List<byte[]>();
                    else
                        patternWavesAllMeasureGroups.Clear(); // reset patternWavesAllMeasureGroups
                    // add the local measuregroup patternWaves to global patternWavesAllMeasureGroups
                    patternWavesAllMeasureGroups.AddRange(patternWaves);

                    // start the offload computing using all the waveforms captured in the above measurement group loop
                    // sending wavefom for offload TDECQ in a different thread
                    // lambda expression (() => { ExecuteOffloadJoin(nMG, threads[nMG]); }) captures the nMG variable by reference.
                    // Therefore, when the lambda expression runs after nMG++ in the for loop, it picks up the new value of nMG.
                    // Hence, must declare a separate variable inside the loop so that each lambda gets its own variable             
                    thread = new Thread(() => { ExecuteOffloadCompute(patternWavesAllMeasureGroups, channelsGroupConfig, arMapMeasuredValuesTmp, arMapMeasuredValuesStatusTmp); }); // must use Tmp, not out in the lambda expresssion
                    thread.IsBackground = true;
                    thread.Start();
                }
                else // local compute with seperate ACQ and TEQ
                {
                    clog.MarkStart(strTestName, clog.TimeKey.TEQualizer, nDUTidx);
                    // connect and configure TEQualizer for each channel defined in DCA
                    for (int ch = 0; ch < this._myConfig.arChannels.Count; ch++)
                        WireTdecq(ch);
                    clog.MarkEnd(strTestName, clog.TimeKey.TEQualizer, nDUTidx);
                    clog.MarkStart(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                    // after applying TEQ preset, run and acquire single pattern waveform for all channels.
                    write(":ACQ:RUN");
                    // *OPC? in Busy() will ensure all measurements of :ACQ:RUN to complete before proceeding to next section of retrieval
                    res = Busy();
                    clog.MarkEnd(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                }
                
            }
            else // original process where TDEQualizer is inside setx file
            {
                //run and acquire 
                clog.MarkStart(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
                write(":ACQ:RUN");
                // *OPC? in Busy() will ensure all measurements of :ACQ:RUN to complete before proceeding to next section of retrieval
                // wait for acquisition limit to reach
                res = Busy();
                clog.MarkEnd(strTestName, clog.TimeKey.AcquireRun, nDUTidx);
            }

            try
            {
                if (_DCAsettings.TEQsetup.bOffloadCompute)
                {
                    clog.MarkStart(strTestName, clog.TimeKey.OffloadJoin, nDUTidx);
                    // Join Thread
                    ExecuteOffloadJoin(thread);
                    clog.MarkEnd(strTestName, clog.TimeKey.OffloadJoin, nDUTidx);
                    clog.MarkStart(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                    // clear the waveNames List
                    waveNames.Clear();
                    // copy tmp to out
                    for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                    {
                        arMapMeasuredValues[c] = arMapMeasuredValuesTmp[c];
                        arMapMeasuredValuesStatus[c] = arMapMeasuredValuesStatusTmp[c];
                    }
                    clog.MarkEnd(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                }
                else
                {
                    //Retrieves the params
                    clog.MarkStart(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                    for (int c = 0; c < channelsGroupConfig.arChannels.Count; c++)
                    {
                        mapMeasuredValues = arMapMeasuredValues[c];
                        mapMeasuredValuesStatus = arMapMeasuredValuesStatus[c];
                        channelConfig = channelsGroupConfig.arChannels[c].channelSetting;

                        ExtractPAM4Param(channelConfig, channelsGroupConfig.arChannels[c].arParams,
                                            mapMeasuredValues, mapMeasuredValuesStatus);
                    }
                    clog.MarkEnd(strTestName, clog.TimeKey.ExtractMea, nDUTidx);
                }
            }
            catch(Exception ex)
            {
                Log(Convert.ToString(ex));
                throw ex;
            }

            // do not turn off so that limit will show in screen capture
            // turn off limit test only after measurement
            //write(":LTESt:ACQuire:STATe OFF");

            clog.MarkEnd(strTestName, clog.TimeKey.measurePAM4Parallel, nDUTidx);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        protected void ExtractEyeMeasurements(CChannelSettings channelConfig, List<string> arParams, Dictionary<string, string> mapMeasuredValues)
        {
            string strKey, strResult, strRootMeasCmd, strCmd, strParam;
            for (int i = 0; i < arParams.Count; i++)
            {
                try
                {
                    strParam = arParams[i];
                    strRootMeasCmd = EyeParamMap(strParam);
                    write(strRootMeasCmd + ":SOUR CHAN" + channelConfig.strChannelName);

                    //Special case setup
                    //Set the particular Jitter Type if we are making Jitter Measurements...
                    if (strParam.Contains("JitterPp"))
                    {
                        write(":MEASure:EYE:JITTer:FORMat PP");
                    }
                    else if (strParam.Contains("JitterRms"))
                    {
                        write(":MEASure:EYE:JITTer:FORMat RMS");
                    }

                    //Add some telemetry debug queries
                    strResult = query(strRootMeasCmd + ":STATus?").Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    
                    if (!strResult.ToUpper().Contains("COR"))
                    {
                        strResult = query(strRootMeasCmd + ":STATus:REASon?").Trim();
                        //strResult = removeNewLine(strResult);
                        //strResult = Regex.Replace(strResult, @"\n", "");
                    }

                    if (channelConfig.bOnlySampleSelectedParams)
                    {
                        if (channelConfig.arSelectedParamsToOnlySample.Contains(strParam))
                        {
                            strCmd = EyeParamMap(strParam) + "?";
                            strResult = query(strCmd).Trim();
                            //strResult = removeNewLine(strResult);
                            //strResult = Regex.Replace(strResult, @"\n", "");
                            strKey = strParam + "Sample";
                            Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Min;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Max;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Mean;
                            mapMeasuredValues.Add(strKey, strResult);

                            strKey = strParam + Std;
                            mapMeasuredValues.Add(strKey, "0");
                            continue;
                        }
                    }

                    //General statistical query for all params... 
                    strCmd = EyeParamMap(strParam) + ":MIN?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + Min;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":MAX?";
                    strResult = query(strCmd).Trim();
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    //strResult = removeNewLine(strResult);
                    strKey = strParam + Max;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":MEAN?";
                    strResult = query(strCmd).Trim();
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    //strResult = removeNewLine(strResult);
                    strKey = strParam + Mean;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + ":SDEV?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + Std;
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);

                    strCmd = EyeParamMap(strParam) + "?";
                    strResult = query(strCmd).Trim();
                    //strResult = removeNewLine(strResult);
                    //strResult = Regex.Replace(strResult, @"\n", "");
                    strKey = strParam + "Sample";
                    Debug.Assert(!mapMeasuredValues.ContainsKey(strKey));
                    mapMeasuredValues.Add(strKey, strResult);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());

                    strParam = arParams[i];

                    strKey = strParam + Min;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Max;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Mean;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + Std;
                    mapMeasuredValues.Add(strKey, "9999");

                    strKey = strParam + "Sample";
                    mapMeasuredValues.Add(strKey, "9999");
                }
            }

        }



        /// <summary>
        /// 
        /// </summary>
		public DCASettings _DCAsettings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAsettings"></param>
		public void setDCAsettings(DCASettings DCAsettings)
		{
            _DCAsettings = DCAsettings;

			//Load the setup file if neccessary
			if( null != _DCAsettings )
			{
				if( null != _DCAsettings.sDCAConfigFile )
				{
					if( _DCAsettings.sDCAConfigFile.Trim().Length > 0 )
					{
						LoadSetupFile(_DCAsettings.sDCAConfigFile);
					}
				}

                if (null != _DCAsettings.PAM4setup)
                {
                    this.initFunctor(_DCAsettings.PAM4setup);
                    _DCAsettings.PAM4setup.Setup();
                }

                if (null != _DCAsettings.CRUsetup)
                {
                    this.initFunctor(_DCAsettings.CRUsetup);
                    //_DCAsettings.CRUsetup.setup();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKeyDCAsettings"></param>
        public void setDCAsettings(string strKeyDCAsettings)
        {
            _DCAsettings = this._myConfig.mapDCASettings[strKeyDCAsettings];
            setDCAsettings(_DCAsettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool captureMeasureJitter(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <returns></returns>
        public bool setupRINmeas(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arJitterParams"></param>
        /// <param name="arAmplitudeParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool measureRIN(CChannelSettings channelConfig, List<string> arJitterParams, List<string> arAmplitudeParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool captureEyeMeasureMaskMargin(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelConfig"></param>
        /// <param name="arParams"></param>
        /// <param name="mapMeasuredValues"></param>
        /// <returns></returns>
        public bool measureSCOPEmode(CChannelSettings channelConfig, List<string> arParams, ref Dictionary<string, string> mapMeasuredValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <returns></returns>
        public bool measureSCOPEmodeParallel(CChannelsGroupParallel channelsGroupConfig, out List<Dictionary<string, string>> arMapMeasuredValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string CheckVerticalCalCmd(string sCh)
        {
            return $":CALibrate:CHANnel{sCh}:STATus?";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public string EnableVerticalCalCmd(string sCh, bool bEnable)
        {
            string sEnabled = bEnable ? "ENABled" : "DISabled";

            return $":CALibrate:CHANnel{sCh}:ENABled {sEnabled}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSlot"></param>
        /// <returns></returns>
        public string StartVerticalCalCmd(string sSlot)
        {
            return $":CALibrate:SLOT{sSlot}:STARt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public bool VerticalCal(List<string> ChanList, bool bForce)
        {
            bool bEnable;
            string sRet = string.Empty;
            List<string> lstSlot = new List<string>();

            foreach (string Chan in ChanList)
            {
                bEnable = !(query(CheckVerticalCalCmd(Chan)).Trim().Trim('\"') == "CALIBRATED") || bForce;
                write(EnableVerticalCalCmd(Chan, bEnable));
                if (bEnable)
                {
                    if (!lstSlot.Contains(Chan.Substring(0, 1)))
                    {
                        lstSlot.Add(Chan.Substring(0, 1));
                    }
                }
            }

            if (lstSlot.Count > 0)
            {
                foreach (string sSlot in lstSlot)
                {
                    write(StartVerticalCalCmd(sSlot));
                    sRet = query(":CALibrate:SDONe?");
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    sRet = query(":CALibrate:SDONe?");
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    Busy();
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string CheckDarkCalCmd(string sCh)
        {
            return $"CALibrate:DARK:CHANnel{sCh}:STATus?";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCh"></param>
        /// <returns></returns>
        public string StartDarkCalCmd(string sCh)
        {
            return $":CALibrate:DARK:CHANnel{sCh}:STARt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChanList"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        public bool DarkCal(List<string> ChanList, bool bForce)
        {
            bool bEnable;
            string sRet = string.Empty;

            foreach (string Chan in ChanList)
            {
                bEnable = !(query(CheckDarkCalCmd(Chan)).Trim().Trim('\"') == "CALIBRATED") || bForce;

                if (bEnable)
                {
                    write(StartDarkCalCmd(Chan));
                    sRet = query(":CALibrate:SDONe?");
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    sRet = query(":CALibrate:SDONe?");
                    Log(sRet);
                    write(":CALibrate:CONTinue");
                    Busy();
                }
            }

            return true;
        }

        /// <summary>
        /// Measure AOP
        /// </summary>
        /// <param name="DCAChan">DCA channel name</param>
        /// <param name="AcqDelay">delay setting acquisition</param>
        public double MeasureAOP(string DCAChan, int AcqDelay = 0)
        {
            string cmd;
            string result;

            write("*CLS");
            write(":SYSTem:MODE EYE");


            write(":ACQ:RUN");
            Busy();
            Pause(AcqDelay);
            write(":MEASure:EYE:APOWer:UNITs DBM");
            cmd = ":MEASure:EYE:APOWer";
            write(cmd + ":SOUR CHAN" + DCAChan);
            result = query(cmd + "?");
            double dPowerLevel;
            double.TryParse(result, out dPowerLevel);
            write(":ACQ:STOP");
            Busy();
            return dPowerLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DCAChan"></param>
        /// <param name="MeasureParams"></param>
        /// <param name="arMapMeasuredValues"></param>
        /// <param name="AcqDelay"></param>
        public void SimpleMeasurement(string DCAChan, List<string> MeasureParams, out Dictionary<string, string> arMapMeasuredValues, int AcqDelay = 0)
        {
            string cmd;
            string result;
            arMapMeasuredValues = new Dictionary<string, string>();
            foreach (string Param in MeasureParams)
            {
                switch (Param)
                {
                    case "AOP":
                        cmd = ":MEASure:EYE:APOWer";
                        break;
                    case "OMA":
                        cmd = ":MEASure:EYE:OOMA";
                        break;
                    default:
                        throw new Exception($"{Param} is unknown Measurement Parameter");
                }
                write(cmd + ":SOUR CHAN" + DCAChan);
                result = query(cmd + "?");
                arMapMeasuredValues.Add(Param, result);
            }
        }

        string waveformDir = @"C:/Users/lab_spsgtest/Documents/Keysight/FlexDCA/Waveforms";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="ConfiguredChannels"></param>
        public void SavePatternWaves(IReadOnlyList<string> fileNames, string[] ConfiguredChannels)
        {
            if (_myConfig.bSimulation)
            {
                return;
            }

            if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));

            write(":DISK:WAV:SAVE:FTYP WPIN");
            for (int i = 0; i < ConfiguredChannels.Length; i++)
            {
                write(string.Format(@":DISK:WAV:FNAM '{0}/{1}'", waveformDir, fileNames[i]));
                write(string.Format(":DISK:WAV:SAVE:SOUR {0}", ConfiguredChannels[i]));
                write(":DISK:WAV:SAVE");
            }
            Busy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="ConfiguredChannels"></param>
        /// <returns></returns>
        public List<byte[]> GetPatternWaves(IReadOnlyList<string> fileNames, string[] ConfiguredChannels)
        {
            bool ret, ret1;

            List<byte[]> waveforms = new List<byte[]>();
            if (_myConfig.bSimulation)
            {
                for (int i = 0; i < ConfiguredChannels.Length; i++)
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SimulatedWaveform.wfmx");
                    var bytes = File.ReadAllBytes(fileName);
                    waveforms.Add(bytes);
                    System.Threading.Thread.Sleep(100);
                }
            }
            else
            {
                if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));

                // if FlexDCA runs locally, it is faster to let FlexDCA store the waveform to a file and read the bytes from there!
                // was WINT but because pattern acquire is on, changed to WPIN write(":DISK:WAV:SAVE:FTYP WINT");
                // http://rfmw.em.keysight.com/DigitalPhotonics/flexdca/PG/Content/Topics/Commands/DISK/WAVeform_SAVE_FTYPe.htm 
                write(":DISK:WAV:SAVE:FTYP WPIN");
                for (int i = 0; i < ConfiguredChannels.Length; i++)
                {
                    write(string.Format(@":DISK:WAV:FNAM '{0}/{1}'", waveformDir, fileNames[i]));
                    ret = WriteAndCheckCmdStatus(string.Format(@":DISK:WAV:SAVE:SOUR {0}", ConfiguredChannels[i]));
                    ret1 = WriteAndCheckCmdStatus(":DISK:WAV:SAVE");
              
                }
                Busy();
                foreach (string strFN in fileNames)
                {
                    string strPath = string.Format(@"{0}/{1}", waveformDir, strFN);
                    var bytes = File.ReadAllBytes(strPath);
                    waveforms.Add(bytes);
                    File.Delete(strPath);
                }
            }

            return waveforms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelsGroupConfig"></param>
        /// <param name="bMeasureOnly"></param>
        /// <returns></returns>
        public List<byte[]> acqWaveform(CChannelsGroupParallel channelsGroupConfig, bool bMeasureOnly = false)
        {
            throw new NotImplementedException();
        }

        Task<Tuple<List<string>, List<string>>> IScope.ExecuteOffloadComputeAsync(List<byte[]> patternWavesAllMeasureGroups, CChannelsGroupParallel channelsGroupConfig, List<Dictionary<string, string>> arMapMeasuredValues, List<Dictionary<string, DCAcmd.eMeasureDataStatus>> arMapMeasuredValuesStatus)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iChannel"></param>
        public void TapsReCalc(int iChannel)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// RF switch object
        ///// </summary>
        //protected IRFSwitch _RFSwitch;

        /// <summary>
        /// Station hardware object
        /// </summary>
        protected StationHardware _stationInstance;

        /// <summary>
        /// Return instrument name of RF switch for trigger signal of DCA
        /// </summary>
        public string sRFSwitchNameForTrigger => _myConfig.sRFSwitchNameForTrigger;
    }

}

