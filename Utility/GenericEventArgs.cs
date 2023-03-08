using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public T EventData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventData"></param>
        public GenericEventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
