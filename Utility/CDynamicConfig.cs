using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class CDynamicConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string strTargetClassType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strTargetAssembly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
