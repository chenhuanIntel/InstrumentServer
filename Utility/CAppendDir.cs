using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CAppendDir
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDir"></param>
        /// <param name="strAddSubDir"></param>
        /// <returns></returns>
        public static string appendDir(string strDir, string strAddSubDir)
        {
            return strDir + (strDir.EndsWith(@"\") ? strAddSubDir : @"\" + strAddSubDir);
        }
    }
}
