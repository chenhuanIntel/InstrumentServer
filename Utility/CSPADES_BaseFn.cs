using System.Linq;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CSPADES_BaseFn
    {

        /// <summary>
        /// Compare two string dictionaries contents
        /// </summary>
        /// <param name="Dict1"></param>
        /// <param name="Dict2"></param>
        /// <returns></returns>
        public bool DictListStringComparison(Dictionary<string, List<string>> Dict1, Dictionary<string, List<string>> Dict2)
        {
            List<string> sListDict1Key = new List<string>();

            foreach (KeyValuePair<string, List<string>> elem in Dict1)
            {
                try
                {
                    sListDict1Key.Add(elem.Key);
                    bool dict21 = Dict2[elem.Key].Except(elem.Value).ToList().Any();
                    bool dict12 = elem.Value.Except(Dict2[elem.Key]).ToList().Any();
                    if (dict21 || dict12)
                    {
                        //Condition Dict1 and Dict2 values are not equal
                        return false;
                    }
                }
                catch
                {
                    //Condition Dict2 doesn't contain Dict1 key
                    return false;
                }
            }

            //Condition Dict1 doesn't contain Dict2 key.
            return !Dict2.Where(entry => !Dict1.ContainsKey(entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value).Any();
        }
    }
}
