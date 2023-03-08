using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class MyLogManager
    {
        /// <summary>
        /// 
        /// </summary>
        static public NLog.LogFactory Factory = new LogFactory(new XmlLoggingConfiguration("./Utility/NLogTC.config"));
    }
}
