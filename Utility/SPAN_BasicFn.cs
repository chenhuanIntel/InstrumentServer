using System.Text.RegularExpressions;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class SPAN_BasicFn
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SKU"></param>
        /// <returns></returns>
        public bool SKUCheck(ref string SKU)
        {
            string SKUPattern = @"^(\w+){7}";
            Regex rx = new Regex(SKUPattern);
            if (!rx.IsMatch("SKU"))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Function to validate MSID format
        /// </summary>
        /// <param name="MSID">MSID that need to validate format</param>
        /// <returns>TRUE:MSID format is correct/FALSE:MSID format is wrong</returns>
        public static bool MSIDFormatCheck(string MSID)
        {
            string sInteralSNPattern = @"^[A-Z]{3}[A-Z0-9]{1}(\d{4})(\w){5}$";
            Regex rxIntSN = new Regex(sInteralSNPattern);

            return (MSID == null) ? false : rxIntSN.IsMatch(MSID);

        }
        /// <summary>
        /// 
        /// </summary>
        public SPAN_BasicFn()
        {
        }
    }
}
