using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstrumentsLib.Tools.Instruments.Switch
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISwitch
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        bool IsConnect(string strChannel1, string strChannel2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        bool Connect(string strChannel1, string strChannel2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        bool Disconnect(string strChannel1, string strChannel2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Slot"></param>
        /// <param name="intInput"></param>
        /// <param name="intOutput"></param>
        /// <returns></returns>
        bool GetSwitchSetting(int Slot, ref int intInput, ref int intOutput);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        bool ObtainLock(string strChannel1, string strChannel2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strChannel1"></param>
        /// <param name="strChannel2"></param>
        /// <returns></returns>
        bool ReleaseLock(string strChannel1, string strChannel2);

    }
}
