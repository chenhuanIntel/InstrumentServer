using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CData
    {
        /// <summary>
        /// 
        /// </summary>
        public string strName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string strUnits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMyName"></param>
        /// <param name="strMyData"></param>
        /// <param name="strMyUnits"></param>
        public CData(string strMyName, string strMyData, string strMyUnits)
        {
            this.strName = strMyName;
            this.strData = strMyData;
            this.strUnits = strMyUnits;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CGroupData
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, CData> mapData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CGroupData()
        {
            mapData = new Dictionary<string, CData>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SharedGlobalData
    {
        #region GROUP NAME CONSTS

        /// <summary>
        /// 
        /// </summary>
        public const string CURRENT_POS = "CURRENT_POS";
        /// <summary>
        /// 
        /// </summary>
        public const string TX_CH_ = "TX_CH_";
        /// <summary>
        /// 
        /// </summary>
        public const string RX_CH_ = "RX_CH_";
        /// <summary>
        /// 
        /// </summary>
        public const string DEVICE_DATA = "DEVICE_DATA";

        /// <summary>
        /// 
        /// </summary>
        public const string VOLTAGE_EXTPDS = "VOLTAGE_EXTPDS";
        /// <summary>
        /// 
        /// </summary>
        public const string POWER_EXTPDS = "POWER_EXTPDS";

        /// <summary>
        /// 
        /// </summary>
        public const string SAMPLED_POWERS = "SAMPLED_POWERS";
        /// <summary>
        /// 
        /// </summary>
        public const string SAMPLED_VOLTAGES = "SAMPLED_VOLTAGES";

        #endregion


        //Add event for telemetry data...
        /// <summary>
        /// Telemetry Data changed event
        /// </summary>
        public event EventHandler<GenericEventArgs<bool>> DataChanged;

        /// <summary>
        /// Group data collection
        /// </summary>
		public Dictionary<string, CGroupData> _myDataCol = new Dictionary<string, CGroupData>();

        /// <summary>
        /// Clear Data Cache
        /// </summary>
        /// <returns></returns>
        public bool clearDataCache()
        {
            _myDataCol.Clear();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strGroup"></param>
        /// <returns></returns>
        public CGroupData getGroupData(string strGroup)
        {
            if (!this._myDataCol.Keys.Contains(strGroup))
            {
                return null;
            }

            return _myDataCol[strGroup];
        }

        /// <summary>
        /// update Data
        /// </summary>
        /// <param name="strGroup"></param>
        /// <param name="strSubGroup"></param>
        /// <param name="strData"></param>
        /// <param name="strUnits"></param>
        /// <returns></returns>
        public bool updateData(string strGroup, string strSubGroup, string strData, string strUnits)
        {
            CGroupData groupData;
            if (!_myDataCol.Keys.Contains(strGroup))
            {
                lock (this)
                {
                    groupData = new CGroupData();
                    _myDataCol[strGroup] = groupData;
                }
            }
            else
            {
                groupData = _myDataCol[strGroup];
            }


            lock (this)
            {
                CData myData = new CData(strSubGroup, strData, strUnits);
                groupData.mapData[strSubGroup] = myData;
            }

            ////Raise data changed event...
            //EventsHelper.Fire(this.DataChanged, this, new GenericEventArgs<bool>(true));

            return true;
        }

        /// <summary>
        /// Sends the data changed event...
        /// </summary>
        public void sendDataChangedEvent()
        {
            EventsHelper.Fire(this.DataChanged, this, new GenericEventArgs<bool>(true));
        }

        //Singleton design pattern...
        #region SharedData Singleton Design Pattern

        private static SharedGlobalData _instance;
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static SharedGlobalData Instance()
        {
            if (null == _instance)
            {
                _instance = new SharedGlobalData();
            }

            return _instance;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private SharedGlobalData()
        {

        }

        #endregion
    }
}
