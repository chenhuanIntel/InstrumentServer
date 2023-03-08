using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Reflection;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class ActivatorBasedFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="strTypeName"></param>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static T BuildObject<T, P, Q>(string strTypeName, P param, Q param2)
        {
            T obj = default(T);

            Type myType = Type.GetType(strTypeName);
            object[] args = { param, param2 };

            obj = (T)Activator.CreateInstance(myType, args);

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="strTypeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T BuildObject<T, P>(string strTypeName, P param)
        {
            T obj = default(T);

            Type myType = Type.GetType(strTypeName);
            object[] args = { param };

            obj = (T)Activator.CreateInstance(myType, args);

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTypeName"></param>
        /// <returns></returns>
        public static T BuildObject<T>(string strTypeName)
        {
            T obj = default(T);

            Type myType = Type.GetType(strTypeName);
            obj = (T)Activator.CreateInstance(myType);

            return obj;
        }
    }
}
