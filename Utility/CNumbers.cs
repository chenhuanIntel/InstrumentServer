using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
	/// <summary>
	/// 
	/// </summary>
	public class CNumbers
	{
		/// <summary>
		/// Number constant for big number
		/// </summary>
		public const int DEFAULTVALUE = -1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
		public static string ExtractNumbers(string expr)
		{
			return string.Join(null, System.Text.RegularExpressions.Regex.Split(expr, "[^\\d]"));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="binlist"></param>
		/// <returns></returns>
		public static string GetStringFromBinList(List<bool> binlist)
		{
			String strbyte = null;
			for (int x = 0; x != binlist.Count; x++) //tmpboolist is the 90+- boolean list
			{
				//this loop checks for true then puts a 1 or a 0 in the string(strbyte)
				if (binlist[x] == true)
				{
					strbyte = strbyte + '1';
				}
				else
				{
					strbyte = strbyte + '0';
				}
			}
			return strbyte;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static string GetIntBinaryStringRemoveZeros(int n)
		{
			char[] b = new char[32];
			int pos = 31;
			int i = 0;

			while (i < 32)
			{
				if ((n & (1 << i)) != 0)
				{
					b[pos] = '1';
				}
				else
				{
					b[pos] = '0';
				}
				pos--;
				i++;
			}
			return new string(b).TrimStart('0');
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static string GetIntBinaryString(int n)
		{
			char[] b = new char[32];
			int pos = 31;
			int i = 0;

			while (i < 32)
			{
				if ((n & (1 << i)) != 0)
				{
					b[pos] = '1';
				}
				else
				{
					b[pos] = '0';
				}
				pos--;
				i++;
			}
			return new string(b);
		}

	}
}
