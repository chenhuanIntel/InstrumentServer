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
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="source"></param>
        /// <param name="collection"></param>
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
                                          TKey oldKey, TKey newKey)
        {
            dict[newKey] = dict[oldKey];

            if (!dict.Remove(oldKey))
                return false;

            return true;
        }
    }
}
