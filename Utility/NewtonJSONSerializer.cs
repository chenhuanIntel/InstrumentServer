using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Utility
{
    /// <summary>
    /// A generic class used to serialize objects using an open source serializer which is more efficient and flexible.
    /// </summary>
    public static class NewtonJSONSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strFileName"></param>
        public static void SerializeToJSON(object obj, string strFileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(strFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }

            //string output = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static T DeserializeFromJSON<T>(String strFileName)
        {
            T obj = default(T);
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(strFileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                obj = (T)serializer.Deserialize(file, typeof(T));
            }
            return obj;
        }
    }
}
