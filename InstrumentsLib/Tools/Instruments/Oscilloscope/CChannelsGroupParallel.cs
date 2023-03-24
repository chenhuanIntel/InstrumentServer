using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace InstrumentsLib.Tools.Instruments.Oscilloscope
{
    /// <summary>
    /// 
    /// </summary>
    public class CChannelsGroupParallel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool bLockCRU { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CChannelConfigParallel> arChannels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected List<int> _myListChannels;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetChannels()
        {
            if (_myListChannels == null || _myListChannels.Count() == 0)
            {
                _myListChannels = new List<int>();
                for (int n = 0; n < arChannels.Count; n++)
                {
                    _myListChannels.Add(arChannels[n].nChannel);
                }
            }

            return _myListChannels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nChannel"></param>
        public void RemoveChannel(int nChannel)
        {
            _myListChannels.Remove(nChannel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arParams"></param>
        public void SetMeasParams(List<string> arParams)
        {
            for (int c = 0; c < this.arChannels.Count; c++)
            {
                this.arChannels[c].arParams = CloneUtil.DeepCopy<List<string>>(arParams);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arParams"></param>
        public void GetMeasParams(ref List<string> arParams)
        {
            for (int c = 0; c < this.arChannels.Count; c++)
            {
                arParams = CloneUtil.DeepCopy<List<string>>(this.arChannels[c].arParams);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CChannelsGroupParallel()
        {
            bLockCRU = true;

            //By default, we have measure groups of 2 channel, please override this by resetting arChannels to desired configuration
            arChannels = new List<CChannelConfigParallel>
            {
                new CChannelConfigParallel(),
                new CChannelConfigParallel()
            };
        }
    }
}
