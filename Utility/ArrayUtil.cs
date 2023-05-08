using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    static class ListExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public static string ToString(IEnumerable ar)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Object item in ar)
            {
                sb.Append(item.ToString());
                sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ar"></param>
        /// <returns></returns>
        public static bool str2num(string str, ref List<double> ar)
        {
            string[] arData = null;
            arData = str.Split(',');
            string myString;
            double dVal = 0.0;

            foreach (string strEntry in arData)
            {
                myString = strEntry;
                if (strEntry.StartsWith("0+") || strEntry.StartsWith("1+"))
                    myString = strEntry.Substring(2, strEntry.Length - 2);

                if (double.TryParse(myString, out dVal))
                    ar.Add(dVal);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ar"></param>
        /// <returns></returns>
        public static bool str2num(string str, ref List<int> ar)
        {
            string[] arData = null;
            arData = str.Split(',');

            foreach (string strEntry in arData)
            {
                ar.Add(int.Parse(strEntry));
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
		public static List<double> convertArrayListString2ListArrayDouble(ArrayList ar)
        {
            List<double> result = new List<double>();

            for (int n = 0; n < ar.Count; n++)
            {
                result.Add(Convert.ToDouble(ar[n]));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMedian(this IEnumerable<double> list)
        {
            var enumerable = list.ToList();
            var halfIndex = enumerable.Count / 2;
            if (enumerable.Count % 2 == 0)
                return (enumerable.ElementAt(halfIndex) + enumerable.ElementAt(halfIndex - 1)) / 2;
            return enumerable.ElementAt(halfIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetStdDeviation(this IEnumerable<double> list)
        {
            var variance = 0.0;
            var enumerable = list.ToList();
            var avg = enumerable.Average();
            foreach (var rssi in enumerable)
            {
                variance += Math.Pow(rssi - avg, 2);
            }
            return Math.Sqrt(variance / enumerable.Count);
        }
    }
}
