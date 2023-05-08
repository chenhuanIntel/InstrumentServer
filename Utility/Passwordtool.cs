using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
	/// <summary>
	/// 
	/// </summary>
	public class Passwordtool
	{
		/* secret keys to generate module passwords */
		//byte[] MFR_PASSWORD_KEY   = { 0x71, 0x0b, 0xac, 0xca };
		byte[] MFR_PASSWORD_KEY = { 0xba, 0x07, 0xac, 0x81 };
		byte[] INTEL_PASSWORD_KEY = { 0xb1, 0xa6, 0x6a, 0x08 };


		/* passwords to access calibration and Intel pages */
		/// <summary>
		/// 
		/// </summary>
		public UInt32 guiMfrPassword { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public UInt32 guiIntelPassword { get; set; }

		static UInt32 hash(UInt32 uiStart, Byte uiByte)
		{
			int i;
			UInt32 uiHash;

			uiHash = uiStart ^ uiByte;
			for (i = 0; i < 8; i++)
			{
				if (1 == (uiHash & 0x1))
				{
					uiHash >>= 1;
					uiHash ^= 0xedb88320;   // CRC_32 polynomial
				}
				else
				{
					uiHash >>= 1;
				}
			}
			return uiHash;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serialNumber"></param>
		public Passwordtool(string serialNumber)
		{
			int i;
			UInt32 uiHash, uiKey;

			uiHash = 0x0;
			uiHash = ~uiHash;

			//for (i=0; i<16; i++)
			for (i = 0; i < serialNumber.Length; i++)
			{
				//uiHash = hash(uiHash, argv[1][i]);
				//byte[] val = ASCIIEncoding.ASCII.GetBytes("0");
				uiHash = hash(uiHash, Convert.ToByte(serialNumber[i]));
			}

			guiMfrPassword = uiHash;
			//uiKey = Convert.ToUInt32( MFR_PASSWORD_KEY);
			for (i = 0; i < 4; i++)
			{
				uiKey = Convert.ToUInt32(MFR_PASSWORD_KEY[3 - i]);
				guiMfrPassword = hash(guiMfrPassword, Convert.ToByte((uiKey & 0xff)));
				uiKey >>= 8;
			}

			//guiMfrPassword ^= ~0;
			UInt32 uInt = 0x0;
			uInt = ~uInt;
			guiMfrPassword ^= uInt;
			//if (false)
			//{
			//    guiIntelPassword = uiHash;
			//    //uiKey = Convert.ToUInt32(INTEL_PASSWORD_KEY);
			//    for (i = 0; i < 4; i++)
			//    {
			//        uiKey = Convert.ToUInt32(INTEL_PASSWORD_KEY[3 - i]);
			//        guiIntelPassword = hash(guiIntelPassword, Convert.ToByte(uiKey & 0xff));
			//        uiKey >>= 8;
			//    }
			//    //guiIntelPassword ^= ~0;
			//    uInt = 0x0;
			//    uInt = ~uInt;
			//    guiIntelPassword ^= uInt;

			//}
			/* free-side passwords must have top bit set, according to SFF-8636 */
			guiMfrPassword |= 0x80000000;
			//guiIntelPassword |= 0x80000000;

			// 0000000000000000
			// Manufacturer password: 0xe43e6b4a
			// Intel password       : 0xad95a721

			//printf("Manufacturer password: 0x%08x\n", guiMfrPassword);
			//printf("Intel password       : 0x%08x\n", guiIntelPassword);
		}
	}
}
