using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Polenter.Serialization;
using Polenter.Serialization.Core;

namespace Utility
{
    /// <summary>
    /// A generic class used to serialize objects using an open source serializer which is more efficient and flexible.
    /// </summary>
    public static class GenericSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        public static Property _property;                                           // Introducing _property for further reference: AS 09/02/2020
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="strFileName"></param>
        /// <param name="settings"></param>
        public static void SerializeToXML<T>(T obj, String strFileName, AdvancedSharpSerializerXmlSettings settings)
        {
            SharpSerializer serializer = new SharpSerializer(settings);
            serializer.Serialize(obj, strFileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="strFileName"></param>
        public static void SerializeToXML<T>(T obj, String strFileName)
        {
            // create instance of sharpSerializer
            // with the standard constructor it serialize/deserialize to xml
            var serializer = new SharpSerializer();

            // *************************************************************************************
            // For advanced serialization you create SharpSerializer with an overloaded constructor
            //
            //  SharpSerializerXmlSettings settings = createXmlSettings();
            //  serializer = new SharpSerializer(settings);
            //
            // Scroll the page to the createXmlSettings() method for more details
            // *************************************************************************************


            // *************************************************************************************
            // You can alter the SharpSerializer with its settings, you can provide your custom readers
            // and writers as well, to serialize data into Json or other formats.
            //
            // var serializer = createSerializerWithCustomReaderAndWriter();
            //
            // Scroll the page to the createSerializerWithCustomReaderAndWriter() method for more details
            // *************************************************************************************

            serializer.Serialize(obj, strFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static T DeserializeFromXML<T>(String strFileName)
        {
            T obj = default(T);

            // create instance of sharpSerializer
            // with the standard constructor it serialize/deserialize to xml
            var serializer = new SharpSerializer();

            obj = (T)serializer.Deserialize(strFileName);

            _property = serializer._xmlProperty;                                    // Saving _property for further reference: AS 09/02/2020

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static object DeserializeFromXML(String strFileName)
        {
            // create instance of sharpSerializer
            // with the standard constructor it serialize/deserialize to xml
            var serializer = new SharpSerializer();

            return serializer.Deserialize(strFileName);
        }
    }
}
