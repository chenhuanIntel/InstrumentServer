using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Utility
{
    /// <summary>
    /// Class to help access network with provided network path and credential
    /// </summary>
    public class NetworkConnection : IDisposable
    {
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        private string _networkName;

        /// <summary>
        /// Constructor of Network Connection
        /// </summary>
        /// <param name="networkName">Network Path</param>
        /// <param name="credentials">User Name and Password</param>
        public NetworkConnection(string networkName,
            NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0 && result != 1219)
            {
                throw new Exception($"Error @ NetworkConnection Construction: Get Result = {result}");
            }
        }

        /// <summary>
        /// Disconnect from the network
        /// </summary>
        ~NetworkConnection()
        {
            Dispose();
        }

        /// <summary>
        /// Disconnect network connection
        /// </summary>
        public void Dispose()
        {
            WNetCancelConnection2(_networkName, 0, true);
        }
    }

    /// <summary>
    /// Stucture of Network Resource
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        /// <summary>
        /// Resource Scope
        /// </summary>
        public ResourceScope Scope;
        /// <summary>
        /// Resource Type
        /// </summary>
        public ResourceType ResourceType;
        /// <summary>
        /// Resource Display Type
        /// </summary>
        public ResourceDisplaytype DisplayType;
        /// <summary>
        /// Number of Usage
        /// </summary>
        public int Usage;
        /// <summary>
        /// Local Network Name
        /// </summary>
        public string LocalName;
        /// <summary>
        /// Remote Network Name
        /// </summary>
        public string RemoteName;
        /// <summary>
        /// Comment
        /// </summary>
        public string Comment;
        /// <summary>
        /// Network Provider
        /// </summary>
        public string Provider;
    }

    /// <summary>
    /// Resouce Scope Status
    /// </summary>
    public enum ResourceScope : int
    {
        /// <summary>
        /// Connected
        /// </summary>
        Connected = 1,
        /// <summary>
        /// Global Network
        /// </summary>
        GlobalNetwork,
        /// <summary>
        /// Remembered
        /// </summary>
        Remembered,
        /// <summary>
        /// Recent
        /// </summary>
        Recent,
        /// <summary>
        /// Context
        /// </summary>
        Context
    };

    /// <summary>
    /// Resource Type Option
    /// </summary>
    public enum ResourceType : int
    {
        /// <summary>
        /// Any
        /// </summary>
        Any = 0,
        /// <summary>
        /// Disk
        /// </summary>
        Disk = 1,
        /// <summary>
        /// Print
        /// </summary>
        Print = 2,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 8,
    }

    /// <summary>
    /// Resource Display Type Option
    /// </summary>
    public enum ResourceDisplaytype : int
    {
        /// <summary>
        /// Generic
        /// </summary>
        Generic = 0x0,
        /// <summary>
        /// Domain
        /// </summary>
        Domain = 0x01,
        /// <summary>
        /// Server
        /// </summary>
        Server = 0x02,
        /// <summary>
        /// Share
        /// </summary>
        Share = 0x03,
        /// <summary>
        /// File
        /// </summary>
        File = 0x04,
        /// <summary>
        /// Group
        /// </summary>
        Group = 0x05,
        /// <summary>
        /// Network
        /// </summary>
        Network = 0x06,
        /// <summary>
        /// Root
        /// </summary>
        Root = 0x07,
        /// <summary>
        /// Share Admin
        /// </summary>
        Shareadmin = 0x08,
        /// <summary>
        /// Directory
        /// </summary>
        Directory = 0x09,
        /// <summary>
        /// Tree
        /// </summary>
        Tree = 0x0a,
        /// <summary>
        /// NDS Container
        /// </summary>
        Ndscontainer = 0x0b
    }
}
