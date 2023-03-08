using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Utility
{
    /// <summary>
    /// Utility class for setting system time and time zone
    /// </summary>
    public static class SystemTimeUtil
    {
        /// <summary>
        /// SYSTEMTIME structure used by SetSystemTime
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        [DllImport("kernel32.dll")]
        static extern bool SetLocalTime(ref SYSTEMTIME time);

        /// <summary>
        /// Check system time zone. 
        /// If it is not correct, try to adjust it.
        /// </summary>
        public static void UpdateSystemTimeZone(List<string> timezoneList)
        {
            //Get the first group of the station name
            var site = ApplicationUtil.GetRegistryStation().Split('-')[0].ToUpper();

            //Get the time zone
            string timezone = "";
            foreach (string str in timezoneList)
            {
                string[] strArr = str.Split('=');
                if (strArr[0].Trim() == site)
                {
                    timezone = strArr[1].Trim();
                    break;
                }
            }

            //Check if we need to set the time zone
            if (timezone == "")
                return;

            //Adjusting the timezone if it is not correct
            if (!TimeZoneInfo.Local.StandardName.Equals(timezone))
            {
                clog.Info("The system time zone is different. Try to set correct time zone.");
                try
                {
                    SetSystemTimeZone(timezone);
                }
                catch (Exception ex)
                {
                    throw new Exception("Time zone set failed.", ex);
                }
            }

        }

        /// <summary>
        /// Check system time. 
        /// If they are not correct, try to adjust it.
        /// </summary>
        /// <param name="ntpServerList">time server host name or ip</param>
        /// <param name="ntpTimeDiff">time difference to trigger the update. 0 is always sync</param>
        public static void UpdateSystemTime(List<string> ntpServerList, int ntpTimeDiff)
        {
            //Get the first group of the station name
            var site = ApplicationUtil.GetRegistryStation().Split('-')[0].ToUpper();

            //Get the time server ip
            string ntpServer = "";
            foreach (string str in ntpServerList)
            {
                string[] strArr = str.Split('=');
                if (strArr[0].Trim() == site)
                {
                    UpdateHostsFile(strArr[1].Trim(), strArr[2].Trim());
                    ntpServer = strArr[1].Trim();
                    break;
                }
            }

            //Check if we need to set the time
            if (ntpServer == "")
            {
                clog.Info("No NTP server compatible for this station.");
                return;
            }

            clog.Info($"NTP server is {ntpServer}.");

            //Get time from an NTP server. 3 max tries. If can't be retrieved, throw an exception.
            DateTime? ntpTime = null;

            for (int tryNum = 0; tryNum < 3; tryNum++)
            {
                try
                {
                    clog.Info($"Trying to get the time from NTP server. Try #{tryNum + 1}");
                    ntpTime = GetTimeFromNTPServer(ntpServer);
                    break;
                }
                catch
                {
                    if (tryNum == 2)
                        throw;
                }
            }

            //Check the time difference between PC and the time server
            if (ntpTimeDiff == 0)
            {
                clog.Info($"Try to sync the time with the NTP server: {ntpServer}.");
                SetTime((DateTime)ntpTime);
            }
            else if (Math.Abs((DateTime.Now - (DateTime)ntpTime).TotalSeconds) >= ntpTimeDiff)
            {
                clog.Info($"The system time is different more than {ntpTimeDiff}s. Try to set the time with the NTP server: {ntpServer}.");
                SetTime((DateTime)ntpTime);
            }

            clog.Info("Time sync done.");

        }

        /// <summary>
        /// Set system time
        /// </summary>
        /// <param name="dateTime"></param>
        public static void SetTime(DateTime dateTime)
        {
            SYSTEMTIME st;

            st.year = (short)(dateTime.Year);
            st.month = (short)dateTime.Month;
            st.dayOfWeek = (short)dateTime.DayOfWeek;
            st.day = (short)dateTime.Day;
            st.hour = (short)dateTime.Hour;
            st.minute = (short)dateTime.Minute;
            st.second = (short)dateTime.Second;
            st.milliseconds = (short)dateTime.Millisecond;

            SetLocalTime(ref st);
        }

        /// <summary>
        /// Get time from NTP server
        /// </summary>
        /// <param name="timeHost">Hostname or IP address for a time server</param>
        /// <returns></returns>
        public static DateTime GetTimeFromNTPServer(string timeHost)
        {
            string ntpServer = timeHost;

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var address = Dns.GetHostEntry(ntpServer).AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(address, 123);

            //NTP uses UDP
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        /// <summary>
        /// Set system timezone by time zone id
        /// For the complete list of time zone id, please visit 
        /// https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/default-time-zones?view=windows-11
        /// </summary>
        /// <param name="timeZoneId">Specific time zone for setting. For example, SE Asia Standard Time.</param>
        public static void SetSystemTimeZone(string timeZoneId)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "tzutil.exe",
                Arguments = "/s \"" + timeZoneId + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            process.WaitForExit();
            TimeZoneInfo.ClearCachedData();
        }

        /// <summary>
        /// stackoverflow.com/a/3294698/162671
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        /// <summary>
        /// Add ip-hostname definition to the hosts file
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="hostName">host name</param>
        public static void UpdateHostsFile(string ip, string hostName)
        {
            //Hosts file in Windows
            string hostFile = "C:/windows/system32/drivers/etc/hosts";

            //Always use uppercase because host name is case insensitive
            hostName = hostName.ToUpper();

            //Read all lines because normally hosts files are small
            string[] lines = File.ReadAllLines(hostFile);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArr = Regex.Split(lines[i].Trim(), @"\s+");

                //Ignore empty or commented lines
                if (strArr.Length == 0 || strArr[0].StartsWith("#"))
                    continue;

                if (strArr[0] == ip)
                {
                    if (strArr[1].ToUpper() != hostName)
                    {
                        //Update the line that host name is wrong
                        lines[i] = $"{ip}\t{hostName}";
                        
                        //Write the updated line array into the hosts file and then return
                        File.WriteAllLines(hostFile, lines);
                        return;
                    }
                    else
                        return; //Return because ip-hostname definition exists
                }
            }

            //Add ip-hostname definition if not exists
            using (StreamWriter w = File.AppendText(hostFile))
            {
                w.WriteLine($"{Environment.NewLine}{ip}\t{hostName}");
            }
            
        }

    }
}
