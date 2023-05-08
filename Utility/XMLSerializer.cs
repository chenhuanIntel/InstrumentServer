using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class XMLSerializer
    {
        #region Functions
        /// <summary>         
        /// This function writes the serialized XML to the file name passed in.         
        /// </summary>         
        /// <typeparam name="T">The object type to serialize.</typeparam>         
        /// <param name="t">The instance of the object.</param>         
        /// <param name="outFilename">The file name. It can be a full path.</param> 
        /// <param name="inNameSpaces"></param>
        public static void SerializeToXML<T>(T t, String outFilename, XmlSerializerNamespaces inNameSpaces = null)
        {
            XmlSerializerNamespaces ns = inNameSpaces;
            if (ns == null)
            {
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");
            }

            XmlSerializer serializer = new XmlSerializer(t.GetType());
            TextWriter textWriter = (TextWriter)new StreamWriter(outFilename);
            serializer.Serialize(textWriter, t, ns);
            textWriter.Close();
        }

        /// <summary>         
        /// This function returns the serialized XML as a string         
        /// </summary>         
        /// <typeparam name="T">The object type to serialize.</typeparam>         
        /// <param name="t">The instance of the object.</param>         
        /// <param name="inNameSpaces">The string that will be passed the XML.</param>         
        public static String SerializeToXML<T>(T t, XmlSerializerNamespaces inNameSpaces = null)
        {
            XmlSerializerNamespaces ns = inNameSpaces;
            if (ns == null)
            {
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");
            }
            XmlSerializer serializer = new XmlSerializer(t.GetType());
            TextWriter textWriter = (TextWriter)new StringWriter();
            serializer.Serialize(textWriter, t, ns);
            return textWriter.ToString();
        }

        /// <summary>         
        /// This function deserializes the XML file passed in.         
        /// </summary>         
        /// <typeparam name="T">The object type to serialize.</typeparam>         
        /// <param name="strFilename">The file or full path to the file.</param>         
        /// <returns>The object that was deserialized from xml.</returns>         
        public static T DeserializeFromXML<T>(String strFilename)
        {
            // File should exist by now.             
            if (File.Exists(strFilename))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                TextReader textReader = (TextReader)new StreamReader(strFilename);
                XmlTextReader reader = new XmlTextReader(textReader);
                reader.Read();

                T retVal = (T)deserializer.Deserialize(reader);
                textReader.Close();
                return retVal;
            }
            else
            {
                throw new FileNotFoundException(strFilename);
            }
        }

        #endregion
    }
}
