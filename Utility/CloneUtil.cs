using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class CloneUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToCopy"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T objectToCopy)
        {
            MemoryStream memoryStream = new MemoryStream();
            NetDataContractSerializer netFormatter = new NetDataContractSerializer();

            netFormatter.Serialize(memoryStream, objectToCopy);

            memoryStream.Position = 0;
            T returnValue = (T)netFormatter.Deserialize(memoryStream);

            memoryStream.Close();
            memoryStream.Dispose();

            return returnValue;
        } 

    }
}
