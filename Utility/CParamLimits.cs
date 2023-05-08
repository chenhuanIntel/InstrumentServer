using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// Holds limits for a parameter of some data type and provides a limits checking reference function
    /// </summary>
    /// <typeparam name="T">Must implement the IComparable interface (e.g., double, float, int)</typeparam>
    public class CParamLimits<T> where T : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public T High = default(T);
        /// <summary>
        /// 
        /// </summary>
        public T Low = default(T);

        /// <summary>
        /// Generic class constructor for CParamLimits
        /// </summary>
        /// <param name="tHigh">Upper bound on the parameter</param>
        /// <param name="tLow">Lower bound on the parameter</param>
        public CParamLimits(T tLow, T tHigh)
        {
            High = tHigh;
            Low = tLow;
        }

        /// <summary>
        /// Checks a value against the limits stored in the structure and changes the value if it falls outside of those limits
        /// </summary>
        /// <param name="val">Value to check</param>
        public void CheckAgainstLimits(ref T val)
        {
            if (val.CompareTo(Low) < 0)
            {
                val = Low;
            }
            else if (val.CompareTo(High) > 0)
            {
                val = High;
            }
        }
    }
}
