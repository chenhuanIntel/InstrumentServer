using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CAssemblyLoad
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSourcePath"></param>
        /// <returns></returns>
        public bool LoadAssemblies(string strSourcePath)
        {
            //Load the assemblies in the order specified
            int n = 0;
            string strAssemblyFullPath;
            for (n = 0; n < arAssemblies.Count; n++)
            {
                strAssemblyFullPath = Path.Combine(strSourcePath, arAssemblies[n]);
                Assembly.LoadFrom(strAssemblyFullPath);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> arAssemblies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CAssemblyLoad()
        {
            arAssemblies = new List<string>();
        }
    }
}
