using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataLinkStamp
    {
        /// <summary>
        /// 
        /// </summary>
        int nSampleFECIdx { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string strDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mySampleIdx"></param>
        /// <param name="strDateTime"></param>
        void SetStamp(int mySampleIdx, string strDateTime);
    }
}
