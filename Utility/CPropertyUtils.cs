using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Utilities for properties
    /// </summary>
    public class CPropertyUtils
    {
        /// <summary>
        /// Get value of property
        /// </summary>
        /// <param name="src">object with properties</param>
        /// <param name="propName">name of property - accepts nested properties e.g. Address.Street</param>
        /// <returns>object value of property</returns>
        public static object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
    }
}
