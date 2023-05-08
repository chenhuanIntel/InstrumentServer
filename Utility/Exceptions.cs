using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class HardwareErrorException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorMsg"></param>
        /// <param name="strDeviceName"></param>
        public HardwareErrorException(string strErrorMsg, string strDeviceName) : base(strErrorMsg)
        {
            ErrorMsg = strErrorMsg;
            strDeviceName = DeviceName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CRURelockFailException : HardwareErrorException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorMsg"></param>
        /// <param name="strDeviceName"></param>
        public CRURelockFailException(string strErrorMsg, string strDeviceName) : base(strErrorMsg, strDeviceName)
        {
            ErrorMsg = strErrorMsg;
            strDeviceName = DeviceName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MSIDMismatchException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Testname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorMsg"></param>
        /// <param name="strTestname"></param>
        public MSIDMismatchException(string strErrorMsg, string strTestname)
            : base(strErrorMsg)
        {
            ErrorMsg = strErrorMsg;
            Testname = strTestname;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SiPTestException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Testname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorMsg"></param>
        /// <param name="strTestname"></param>
        public SiPTestException(string strErrorMsg, string strTestname)
            : base(strErrorMsg)
        {
            ErrorMsg = strErrorMsg;
            Testname = strTestname;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DCAImageDriverNotmapped : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Testname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorMsg"></param>
        /// <param name="strTestname"></param>
        public DCAImageDriverNotmapped(string strErrorMsg, string strTestname)
            : base(strErrorMsg)
        {
            ErrorMsg = strErrorMsg;
            Testname = strTestname;
        }
    }
}
