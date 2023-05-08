using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{

    /// <summary>
    /// 
    /// </summary>
    public class PMICHexParserConfig
    {
        /// <summary>
        /// Character of line ID
        /// </summary>
        public string str_lineID;
        /// <summary>
        /// Character of FirstByte
        /// </summary>
        public string str_FirstByte;

        /// <summary>
        /// Number of line hex character 
        /// </summary>
        public UInt16 num_char_lineID;
        /// <summary>
        /// Number of address hex character 
        /// </summary>
        public UInt16 num_char_address;
        /// <summary>
        /// Number of tail byte to ignore
        /// </summary>
        public UInt16 num_tail_byte_ignore;
        /// <summary>
        /// Setting percentage of meaningless hex content
        /// </summary>
        public UInt16 setPercentMeaninglessHex;

        /// <summary>
        /// PMICHexParserConfig default constructor
        /// </summary>
        public PMICHexParserConfig()
        {
            str_lineID = ":20";  // From DR4 PMIC hex data
            str_FirstByte = "00";  // From DR4 PMIC hex data

            num_char_lineID = 3;  // Ex ":20" ==> num_char_line = 3
            num_char_address = 4;  // Ex: "F000" ==> num_char_address = 4
            num_tail_byte_ignore = 2;  // Ex: For DR4 PMIC hex data, we need to ignore last 2 bytes
            setPercentMeaninglessHex = 5;  // Default meaningless hex content = 5%
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PMICUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHexFileName"></param>
        /// <returns></returns>
        public static string ParsePMICHexData(string sHexFileName)
        {
            // Get Hex Parser Config
            PMICHexParserConfig configParser = new PMICHexParserConfig();

            // Import all lines from Hex file
            var lines = File.ReadAllLines(sHexFileName);

            // Loop for all lines
            string strHexDataLines = "";
            for (var i = 0; i < lines.Length; i += 1)
            {
                var line = lines[i];

                // Skip if first line not begin with str_lineID 
                string str = line.Substring(0, configParser.num_char_lineID); // Get first specific strings
                if (str != configParser.str_lineID)
                {
                    continue;
                }

                // Exclude line characters e.g. ":20" (3 strings) and also address part (i.e. first 2 bytes, 4 strings)
                line = line.Remove(0, configParser.num_char_lineID + configParser.num_char_address); // exclude 3+4 strings

                // Exclude first byte that equal to "00"
                string strFirstBytesRd = line.Substring(0, 2); // Get first 2 strings
                if (strFirstBytesRd == configParser.str_FirstByte)
                {
                    line = line.Remove(0, 2);
                }

                // Skip meaningless data section i.e. continuous contains most of FFFFF...FFFF (exclude last two bytes)
                string strHexToCheck = line.Remove(line.Length - configParser.num_tail_byte_ignore); // Get hex strings to verify 
                var strExclude = new String('F', line.Length - configParser.num_tail_byte_ignore); // Create exclude string pattern
                var countDiffHex = strHexToCheck.Zip(strExclude, (c1, c2) => c1 == c2 ? 0 : 1).Sum(); // Count diff between two strings  
                int totalPercentMeaninglessHex = (int)((double)countDiffHex / strExclude.Length * 100); // Check int % matching pattern of useless 0xFF..FF content

                if ((strHexToCheck == strExclude) || (totalPercentMeaninglessHex < configParser.setPercentMeaninglessHex))
                {
                    continue;
                }

                // For target data section, rotate every 4 bytes
                var numPairHex = line.Length / 2;
                var numFourBytes = numPairHex / 4;
                string newline = "";
                string tmp_line = line;
                for (var j = 1; j <= numFourBytes; j += 1)
                {
                    Console.WriteLine("Perform 4 bytes rotation, round# " + j);

                    string strTmpFourBytes = tmp_line.Substring(0, 8); // Get 4 bytes

                    // split string in to pairs then convert string to list
                    var enumFourBytes = strTmpFourBytes.TakeEvery(2);
                    List<string> listFourBytes = enumFourBytes.Cast<string>().ToList();

                    // Reverses the order of the list elements
                    listFourBytes.Reverse();

                    // list to string
                    string combindedHexString = string.Join("", listFourBytes.ToArray());

                    // Concat hex string to newline
                    newline = newline + combindedHexString;

                    // Once done, remove current 4 bytes
                    tmp_line = tmp_line.Remove(0, 8);
                }
                strHexDataLines = strHexDataLines + newline;
            }

            // Converse Hex to Ascii
            string strAscii = "";
            for (int i = 0; i < strHexDataLines.Length; i += 2)
            {
                string hs = strHexDataLines.Substring(i, 2);
                strAscii = strAscii + Convert.ToChar(Convert.ToUInt32(hs, 16));
            }

            // Remove "+" from strAscii
            strAscii = strAscii.Replace("+", "");

            // Return Asscii string
            return strAscii;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<string> TakeEvery(this string s, int count)
        {
            int index = 0;
            while (index < s.Length)
            {
                if (s.Length - index >= count)
                {
                    yield return s.Substring(index, count);
                }
                else
                {
                    yield return s.Substring(index, s.Length - index);
                }
                index += count;
            }
        }
    }
}
