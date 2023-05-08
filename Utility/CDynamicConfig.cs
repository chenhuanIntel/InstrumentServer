using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    // data contract must be applied to any class that is intended to be seralized, even in the all parent classes
    // data member must also be applied to any property of any class if the property is intended to be serialed
    [DataContract] // must declar DataContract to allow JSON to serialize it in WCF
    public abstract class CDynamicConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string strTargetClassType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string strTargetAssembly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int nIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string strName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myType"></param>
        protected void buildTargetClassInfo(Type myType)
        {
            strTargetClassType = myType.AssemblyQualifiedName;
            strTargetAssembly = myType.Assembly.FullName;
        }
    }
}
