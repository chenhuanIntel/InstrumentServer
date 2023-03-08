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
    public static class CReverseUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string reverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //public static Dictionary<object, object> reverseDictionary(Dictionary<object, object> dic)
        //{
        //    return dic.Reverse().ToDictionary(x => x.Key, x => x.Value);
        //}
    }
}
