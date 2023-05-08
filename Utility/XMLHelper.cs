using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Utility
{
    /// <summary>
    /// XML Helper to Read and Parse xml documentation file
    /// </summary>
    public class XMLHelper
    {
        private Dictionary<string, Dictionary<string, string>> dictXML = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Check Whether XML documentation file is existed
        /// </summary>
        /// <param name="xmlFile">XML Documentation File Name</param>
        /// <returns>true: XML loaded; false: XML not loaded yet</returns>
        public bool IsXMLExist(string xmlFile)
        {
            return dictXML.ContainsKey(xmlFile);
        }

        /// <summary>
        /// Load XML Documentation File to memory
        /// </summary>
        /// <param name="ProjectName">Project Name of Project to Parse</param>
        /// <param name="xmlDocumentationFilePath">XML Documentation File Path</param>
        public void LoadXmlDocumentation(string ProjectName, string xmlDocumentationFilePath)
        {
            Dictionary<string, string> loadedXmlDocumentation = new Dictionary<string, string>();
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xmlDocumentationFilePath)))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
                    {
                        string raw_name = xmlReader["name"];
                        if (raw_name.StartsWith("P:"))
                        {
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "summary")
                                {
                                    loadedXmlDocumentation[raw_name] = xmlReader.ReadInnerXml().Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            loadedXmlDocumentation[raw_name] = xmlReader.ReadInnerXml();
                        }
                    }
                }
            }
            dictXML[ProjectName] = loadedXmlDocumentation;
        }

        /// <summary>
        /// Re-format the Key Property Name
        /// </summary>
        /// <param name="typeFullNameString">Full Property Name</param>
        /// <param name="memberNameString">Property Name</param>
        /// <returns>Key Property Name in Expected Format</returns>
        private string XmlDocumentationKeyHelper(string typeFullNameString, string memberNameString)
        {
            string key = Regex.Replace(typeFullNameString, @"\[.*\]", string.Empty).Replace('+', '.');
            if (memberNameString != null)
            {
                key += "." + memberNameString;
            }
            return key;
        }

        /// <summary>
        /// Get document of a type
        /// </summary>
        /// <param name="ProjectName">Project Name</param>
        /// <param name="type">Type Object</param>
        /// <returns>Document of the Type Object</returns>
        public string GetDocumentation(string ProjectName, Type type)
        {
            string key = "T:" + XmlDocumentationKeyHelper(type.FullName, null);
            dictXML[ProjectName].TryGetValue(key, out string documentation);
            return documentation;
        }

        /// <summary>
        /// Get document of a property
        /// </summary>
        /// <param name="ProjectName">Project Name</param>
        /// <param name="propertyInfo">Property Object</param>
        /// <param name="AlternateProjectname">Project to find if can't find in ProjectName</param>
        /// <returns>Document of the Property</returns>
        public string GetDocumentation(string ProjectName, PropertyInfo propertyInfo, string AlternateProjectname = "")
        {
            string documentation;
            string key = "P:" + XmlDocumentationKeyHelper(propertyInfo.DeclaringType.FullName, propertyInfo.Name);
            if (dictXML[ProjectName].TryGetValue(key, out documentation))
            {
                return documentation;
            }
            else if (AlternateProjectname != "")
            {
                dictXML[AlternateProjectname].TryGetValue(key, out documentation);
                return documentation;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get document of a property
        /// </summary>
        /// <param name="ProjectName">Project Name</param>
        /// <param name="propertyInfo">Property Object</param>
        /// <returns>Document of the Property</returns>
        public bool DocumentationExist(string ProjectName, PropertyInfo propertyInfo)
        {
            string key = "P:" + XmlDocumentationKeyHelper(propertyInfo.DeclaringType.FullName, propertyInfo.Name);
            return dictXML[ProjectName].TryGetValue(key, out string documentation);
        }
    }
}
