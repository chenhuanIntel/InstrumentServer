using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class CDateTimeUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string getDateTimeString(DateTime dt)
        {
            string strTime = "";
            //DateTime UTCtime = dt.ToUniversalTime();

            //20130327092423,Qb1MaxBit,TxCH1,0 -> yyyyMMddHHmmss
            strTime = dt.ToString("yyyyMMddHHmmss");
            return strTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
		public static int date2WWday(DateTime dt)
        {
            return (int)dt.DayOfWeek;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int date2WW(DateTime dt)
        {
            DateTime FirstDayOfYear = new DateTime(dt.Year, 1, 1);

            if ((dt.Month == 12) && ((31 - dt.Day) < (int)FirstDayOfYear.DayOfWeek))
            {
                return 1;
            }

            //Normalize the number of days since 1st day of year
            TimeSpan ts = dt - FirstDayOfYear;
            int nDaysSince1stOfYear = ts.Days;
            int nNormDays = nDaysSince1stOfYear + (int)FirstDayOfYear.DayOfWeek;
            int nWW = (nNormDays / 7) + 1;  //WW is 1-based
            return nWW;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
		public static int date2WWyear(DateTime dt)
        {
            DateTime FirstDayOfYear = new DateTime(dt.Year, 1, 1);

            if ((dt.Month == 12) && ((31 - dt.Day) < (int)FirstDayOfYear.DayOfWeek))
            {
                return dt.Year + 1;
            }

            return dt.Year;
        }
    }
}
