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
    public static class CCypher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string Encrypt(string strValue)
        {
            var hashBytes = Encoding.UTF8.GetBytes(strValue);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashBytes[i] += 15;
            }
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strHash"></param>
        /// <returns></returns>
        public static string Decrypt(string strHash)
        {
            var hashBytes = Convert.FromBase64String(strHash);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashBytes[i] -= 15;
            }
            return Encoding.UTF8.GetString(hashBytes);
        }
    }
}
