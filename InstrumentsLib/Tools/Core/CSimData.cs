using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSimGenericData<T>
    {
        private int nDataIdx;
        /// <summary>
        /// 
        /// </summary>
        public List<T> DataList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T getSimData()
        {
            T DataValue = DataList[nDataIdx];
            if (nDataIdx >= DataList.Count)
            {
                nDataIdx = 0;
            }
            else
            {
                nDataIdx++;
            }
            return DataValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public CSimGenericData()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CSimData
    {
        /// <summary>
        /// 
        /// </summary>
        public CSimGenericData<string> StringData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CSimGenericData<int> IntData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CSimGenericData<double> DoubleData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CSimData()
        {
            StringData = new CSimGenericData<string>();
            IntData = new CSimGenericData<int>();
            DoubleData = new CSimGenericData<double>();
        }
    }

}
