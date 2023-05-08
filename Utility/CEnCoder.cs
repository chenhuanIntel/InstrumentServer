using System;
using System.Linq;
using System.Text;

namespace Utility
{
	/// <summary>
	/// MD5_CTX class
	/// </summary>
	public class MD5_CTX
	{
		/// <summary>
		/// number of _bits_ handled mod 2^64
		/// </summary>
		public UInt32[] i;
		/// <summary>
		/// 
		/// ///
		/// </summary>
		public UInt32[] buf;
		/// <summary>
		/// input buffer
		/// </summary>
		public Byte[] inb;
		/// <summary>
		/// actual digest after MD5Final call
		/// </summary>
		public Byte[] digest;

		/// <summary>
		/// MD5_CTX default constructor
		/// </summary>
		public MD5_CTX()
		{
			i = new UInt32[2] { 0, 0 };
			buf = new UInt32[4] { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };
			inb = new Byte[64];
			digest = new Byte[16];

		}
		/* The routine MD5Init initializes the message-digest context
mdContext. All fields are set to zero.

void MD5Init (MD5_CTX mdContext)
{
	mdContext.i[0] = mdContext.i[1] = 0;
	/* Load magic initialization constants.

	mdContext.buf[0] = (UInt32)0x67452301;
	mdContext.buf[1] = (UInt32)0xefcdab89;
	mdContext.buf[2] = (UInt32)0x98badcfe;
	mdContext.buf[3] = (UInt32)0x10325476;
}
*/
	}

	/// <summary>
	/// Cisco_Magic_Code class
	/// </summary>
	public class Cisco_Magic_Code
	{
		//static Byte[] magicKey = Encoding.ASCII.GetBytes("cccccccccccccccc");
		/*This requires the following array definition: */
		static UInt32[] EthCrc32Table_ = {
		0x00000000,0x96300777,0x2c610eee,0xba510999,0x19c46d07,0x8ff46a70,0x35a563e9,
		0xa395649e,0x3288db0e,0xa4b8dc79,0x1ee9d5e0,0x88d9d297,0x2b4cb609,0xbd7cb17e,
		0x072db8e7,0x911dbf90,0x6410b71d,0xf220b06a,0x4871b9f3,0xde41be84,0x7dd4da1a,
		0xebe4dd6d,0x51b5d4f4,0xc785d383,0x56986c13,0xc0a86b64,0x7af962fd,0xecc9658a,
		0x4f5c0114,0xd96c0663,0x633d0ffa,0xf50d088d,0xc8206e3b,0x5e10694c,0xe44160d5,
		0x727167a2,0xd1e4033c,0x47d4044b,0xfd850dd2,0x6bb50aa5,0xfaa8b535,0x6c98b242,
		0xd6c9bbdb,0x40f9bcac,0xe36cd832,0x755cdf45,0xcf0dd6dc,0x593dd1ab,0xac30d926,
		0x3a00de51,0x8051d7c8,0x1661d0bf,0xb5f4b421,0x23c4b356,0x9995bacf,0x0fa5bdb8,
		0x9eb80228,0x0888055f,0xb2d90cc6,0x24e90bb1,0x877c6f2f,0x114c6858,0xab1d61c1,
		0x3d2d66b6,0x9041dc76,0x0671db01,0xbc20d298,0x2a10d5ef,0x8985b171,0x1fb5b606,
		0xa5e4bf9f,0x33d4b8e8,0xa2c90778,0x34f9000f,0x8ea80996,0x18980ee1,0xbb0d6a7f,
		0x2d3d6d08,0x976c6491,0x015c63e6,0xf4516b6b,0x62616c1c,0xd8306585,0x4e0062f2,
		0xed95066c,0x7ba5011b,0xc1f40882,0x57c40ff5,0xc6d9b065,0x50e9b712,0xeab8be8b,
		0x7c88b9fc,0xdf1ddd62,0x492dda15,0xf37cd38c,0x654cd4fb,0x5861b24d,0xce51b53a,
		0x7400bca3,0xe230bbd4,0x41a5df4a,0xd795d83d,0x6dc4d1a4,0xfbf4d6d3,0x6ae96943,
		0xfcd96e34,0x468867ad,0xd0b860da,0x732d0444,0xe51d0333,0x5f4c0aaa,0xc97c0ddd,
		0x3c710550,0xaa410227,0x10100bbe,0x86200cc9,0x25b56857,0xb3856f20,0x09d466b9,
		0x9fe461ce,0x0ef9de5e,0x98c9d929,0x2298d0b0,0xb4a8d7c7,0x173db359,0x810db42e,
		0x3b5cbdb7,0xad6cbac0,0x2083b8ed,0xb6b3bf9a,0x0ce2b603,0x9ad2b174,0x3947d5ea,
		0xaf77d29d,0x1526db04,0x8316dc73,0x120b63e3,0x843b6494,0x3e6a6d0d,0xa85a6a7a,
		0x0bcf0ee4,0x9dff0993,0x27ae000a,0xb19e077d,0x44930ff0,0xd2a30887,0x68f2011e,
		0xfec20669,0x5d5762f7,0xcb676580,0x71366c19,0xe7066b6e,0x761bd4fe,0xe02bd389,
		0x5a7ada10,0xcc4add67,0x6fdfb9f9,0xf9efbe8e,0x43beb717,0xd58eb060,0xe8a3d6d6,
		0x7e93d1a1,0xc4c2d838,0x52f2df4f,0xf167bbd1,0x6757bca6,0xdd06b53f,0x4b36b248,
		0xda2b0dd8,0x4c1b0aaf,0xf64a0336,0x607a0441,0xc3ef60df,0x55df67a8,0xef8e6e31,
		0x79be6946,0x8cb361cb,0x1a8366bc,0xa0d26f25,0x36e26852,0x95770ccc,0x03470bbb,
		0xb9160222,0x2f260555,0xbe3bbac5,0x280bbdb2,0x925ab42b,0x046ab35c,0xa7ffd7c2,
		0x31cfd0b5,0x8b9ed92c,0x1daede5b,0xb0c2649b,0x26f263ec,0x9ca36a75,0x0a936d02,
		0xa906099c,0x3f360eeb,0x85670772,0x13570005,0x824abf95,0x147ab8e2,0xae2bb17b,
		0x381bb60c,0x9b8ed292,0x0dbed5e5,0xb7efdc7c,0x21dfdb0b,0xd4d2d386,0x42e2d4f1,
		0xf8b3dd68,0x6e83da1f,0xcd16be81,0x5b26b9f6,0xe177b06f,0x7747b718,0xe65a0888,
		0x706a0fff,0xca3b0666,0x5c0b0111,0xff9e658f,0x69ae62f8,0xd3ff6b61,0x45cf6c16,
		0x78e20aa0,0xeed20dd7,0x5483044e,0xc2b30339,0x612667a7,0xf71660d0,0x4d476949,
		0xdb776e3e,0x4a6ad1ae,0xdc5ad6d9,0x660bdf40,0xf03bd837,0x53aebca9,0xc59ebbde,
		0x7fcfb247,0xe9ffb530,0x1cf2bdbd,0x8ac2baca,0x3093b353,0xa6a3b424,0x0536d0ba,
		0x9306d7cd,0x2957de54,0xbf67d923,0x2e7a66b3,0xb84a61c4,0x021b685d,0x942b6f2a,
		0x37be0bb4,0xa18e0cc3,0x1bdf055a,0x8def022d};
		/// <summary>
		/// It will take 28 Bytes 1 byte GBIC "05", 1 Byte reserved "0", 1 byte Vendor ID "0x10",
		/// 16 byte magic code, and 9 bytes reserved "0"
		/// </summary>
		/// <param name="data_v"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public UInt32 EthCrc32(Byte[] data_v, int size)
		{
			int i = 0;
			UInt32 answer = 0xffffffff;
			while ((size--) > 0)
			{
				answer = EthCrc32Table_[((answer >> 24) ^ data_v[i++]) & 0xff] ^ (answer << 8);

			}
			answer = ~answer;
			return answer;
		}

		/// <summary>
		/// md5sfp(int vendorId, string vendorName, string serialNumber)
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="vendorName"></param>
		/// <param name="serialNumber"></param>
		/// <returns></returns>
		public Byte[] md5sfp(int vendorId, string vendorName, string serialNumber)
		{
			Byte[] magicKey = { 0x12, 0xa7, 0xdd, 0x48, 0x7a, 0x32, 0x29, 0x67, 0xf4, 0xea, 0xf0, 0x8c, 0xf2, 0xa9, 0x22, 0x02 };

			return md5sfp(vendorId, vendorName, serialNumber, magicKey);
		}

		/// <summary>
		/// md5sfp(int vendorId, string vendorName, string serialNumber, string newMagicKey)
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="vendorName"></param>
		/// <param name="serialNumber"></param>
		/// <param name="newMagicKey"></param>
		/// <returns></returns>
		public Byte[] md5sfp(int vendorId, string vendorName, string serialNumber, string newMagicKey)
		{
			if (newMagicKey.Count() < 16)
				return null;
			Byte[] magicKey = Encoding.ASCII.GetBytes(newMagicKey);
			return md5sfp(vendorId, vendorName, serialNumber, magicKey);
		}

		/// <summary>
		/// md5sfp(int vendorId, string vendorName, string serialNumber, Byte[] newMagicKey)
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="vendorName"></param>
		/// <param name="serialNumber"></param>
		/// <param name="newMagicKey"></param>
		/// <returns></returns>
		public Byte[] md5sfp(int vendorId, string vendorName, string serialNumber, Byte[] newMagicKey)
		{
			MD5_CTX context = new MD5_CTX();
			if (newMagicKey.Count() < 16)
			{
				return null;
			}
			Byte[] byteVendorId = BitConverter.GetBytes(vendorId);
			Byte[] bytevendorName = Encoding.ASCII.GetBytes(vendorName);
			Byte[] byteserialNumber = Encoding.ASCII.GetBytes(serialNumber);


			MD5Update(context, byteVendorId, 1);
			MD5Update(context, bytevendorName, 16);
			MD5Update(context, byteserialNumber, 16);
			MD5Update(context, newMagicKey, 16);
			MD5Final(context);

			return context.digest;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="sz"></param>
		public void spaceFill(ref string buf, int sz)
		{
			buf = buf.PadRight(sz, ' ');
		}
		/*
	***********************************************************************
	** md5.c -- the source code for MD5 routines **
	** RSA Data Security, Inc. MD5 Message-Digest Algorithm **
	Create PDF files without this message by purchasing novaPDF printer (http://www.novapdf.com)
	A printed version of this document is an uncontrolled copy.
	Cisco Systems, Inc. Page 12 of 19 Company Confidential
	March 22, 2010 GBIC Security & Expanded Identification :ENG-107393, Rev. A.6
	** Created: 2/17/90 RLR **
	** Revised: 1/91 SRD,AJ,BSK,JT Reference C ver., 7/10 constant corr. **
	** 1992.2.13 Jouko Holopainen, 80x86 version **
	***********************************************************************
	*/
		/*
		***********************************************************************
		** Copyright (C) 1990, RSA Data Security, Inc. All rights reserved. **
		** **
		** License to copy and use this software is granted provided that **
		** it is identified as the "RSA Data Security, Inc. MD5 Message- **
		** Digest Algorithm" in all material mentioning or referencing this **
		** software or this function. **
		** **
		** License is also granted to make and use derivative works **
		** provided that such works are identified as "derived from the RSA **
		** Data Security, Inc. MD5 Message-Digest Algorithm" in all **
		** material mentioning or referencing the derived work. **
		** **
		** RSA Data Security, Inc. makes no representations concerning **
		** either the merchantability of this software or the suitability **
		** of this software for any particular purpose. It is provided "as **
		** is" without express or implied warranty of any kind. **
		** **
		** These notices must be retained in any copies of any part of this **
		** documentation and/or software. **
		***********************************************************************
		*/

		/*
		***********************************************************************
		** Message-digest routines: **
		** To form the message digest for a message M **
		** (1) Initialize a context buffer mdContext using MD5Init **
		** (2) Call MD5Update on mdContext and M **
		Create PDF files without this message by purchasing novaPDF printer (http://www.novapdf.com)
		A printed version of this document is an uncontrolled copy.
		Cisco Systems, Inc. Page 13 of 19 Company Confidential
		March 22, 2010 GBIC Security & Expanded Identification :ENG-107393, Rev. A.6
		** (3) Call MD5Final on mdContext **
		** The message digest is now in mdContext.digest[0...15] **
		***********************************************************************
		*/
		static Byte[] PADDING = new Byte[64] {
		0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
		};
		/** F, G, H and I are basic MD5 functions */
		public UInt32 F(UInt32 x, UInt32 y, UInt32 z)
		{
			return (((x) & (y)) | ((~x) & (z)));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public UInt32 G(UInt32 x, UInt32 y, UInt32 z)
		{
			return (((x) & (z)) | ((y) & (~z)));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public UInt32 H(UInt32 x, UInt32 y, UInt32 z)
		{
			return ((x) ^ (y) ^ (z));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public UInt32 I(UInt32 x, UInt32 y, UInt32 z)
		{
			return ((y) ^ ((x) | (~z)));
		}
		/** ROTATE_LEFT rotates x left n bits */
		public UInt32 ROTATE_LEFT(UInt32 x, int n)
		{
			return (((x) << (n)) | ((x) >> (32 - (n))));
		}
		/// <summary>
		/// FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4
		/// Rotation is separate from addition to prevent recomputation
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="x"></param>
		/// <param name="s"></param>
		/// <param name="ac"></param>
		public void FF(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, UInt32 x, int s, UInt32 ac)
		{
			(a) += F((b), (c), (d)) + (x) + (ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		/// <summary>
		/// FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4
		/// Rotation is separate from addition to prevent recomputation
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="x"></param>
		/// <param name="s"></param>
		/// <param name="ac"></param>
		public void GG(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, UInt32 x, int s, UInt32 ac)
		{
			(a) += G((b), (c), (d)) + (x) + (ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		/// <summary>
		/// FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4
		/// Rotation is separate from addition to prevent recomputation
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="x"></param>
		/// <param name="s"></param>
		/// <param name="ac"></param>
		public void HH(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, UInt32 x, int s, UInt32 ac)
		{
			(a) += H((b), (c), (d)) + (x) + (ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		/// <summary>
		/// FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4
		/// Rotation is separate from addition to prevent recomputation
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="x"></param>
		/// <param name="s"></param>
		/// <param name="ac"></param>
		public void II(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, UInt32 x, int s, UInt32 ac)
		{
			(a) += I((b), (c), (d)) + (x) + (ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}

		/* The routine MD5Update updates the message-digest context to
		account for the presence of each of the characters inBuf[0..inLen-1]
		in the message whose digest is being computed.
		*/
		void MD5Update(MD5_CTX mdContext, Byte[] inBuf, int inLen)
		{

			UInt32[] BIn = new UInt32[16];
			int i, ii, j = 0;

			/* compute number of bytes mod 64 */
			int mdi = (int)((mdContext.i[0] >> 3) & 0x3F);
			/* update number of bits */
			if ((mdContext.i[0] + ((UInt32)inLen << 3)) < mdContext.i[0])
				mdContext.i[1]++;
			mdContext.i[0] += ((UInt32)inLen << 3);
			mdContext.i[1] += ((UInt32)inLen >> 29);

			while ((inLen--) > 0)
			{
				/* add new character to buffer, increment mdi */
				mdContext.inb[mdi++] = inBuf[j++];
				/* transform if necessary */
				if (mdi == 0x40)
				{
					for (i = 0, ii = 0; i < 16; i++, ii += 4)
						BIn[i] = (((UInt32)mdContext.inb[ii + 3]) << 24) |
						(((UInt32)mdContext.inb[ii + 2]) << 16) |
						(((UInt32)mdContext.inb[ii + 1]) << 8) |
						((UInt32)mdContext.inb[ii]);
					MD5Transform(mdContext.buf, BIn);
					mdi = 0;
				}
			}
		}
		/* The routine MD5Final terminates the message-digest computation and
		Create PDF files without this message by purchasing novaPDF printer (http://www.novapdf.com)
		A printed version of this document is an uncontrolled copy.
		Cisco Systems, Inc. Page 16 of 19 Company Confidential
		March 22, 2010 GBIC Security & Expanded Identification :ENG-107393, Rev. A.6
		ends with the desired message digest in mdContext.digest[0...15].
		*/
		void MD5Final(MD5_CTX mdContext)
		{
			UInt32[] inbuf = new UInt32[16];
			int mdi;
			int i, ii;
			int padLen;
			/* save number of bits */
			inbuf[14] = mdContext.i[0];
			inbuf[15] = mdContext.i[1];
			/* compute number of bytes mod 64 */
			mdi = (int)((mdContext.i[0] >> 3) & 0x3F);
			/* pad out to 56 mod 64 */
			padLen = (mdi < 56) ? (56 - mdi) : (120 - mdi);
			MD5Update(mdContext, PADDING, padLen);
			/* append length in bits and transform */
			for (i = 0, ii = 0; i < 14; i++, ii += 4)
				inbuf[i] = (((UInt32)mdContext.inb[ii + 3]) << 24) |
				(((UInt32)mdContext.inb[ii + 2]) << 16) |
				(((UInt32)mdContext.inb[ii + 1]) << 8) |
				((UInt32)mdContext.inb[ii]);

			MD5Transform(mdContext.buf, inbuf);
			/* store buffer in digest */
			for (i = 0, ii = 0; i < 4; i++, ii += 4)
			{
				mdContext.digest[ii] = (Byte)(mdContext.buf[i] & 0xFF);
				mdContext.digest[ii + 1] =
				(Byte)((mdContext.buf[i] >> 8) & 0xFF);
				mdContext.digest[ii + 2] =
				(Byte)((mdContext.buf[i] >> 16) & 0xFF);
				mdContext.digest[ii + 3] =
				(Byte)((mdContext.buf[i] >> 24) & 0xFF);

			}
		}
		/* Basic MD5 step. Transforms buf based on in.
		*/
		void MD5Transform(UInt32[] buf, UInt32[] inb)
		{
			UInt32 a = buf[0], b = buf[1], c = buf[2], d = buf[3];
			/* Round 1 */
			int S11 = 7;
			int S12 = 12;
			int S13 = 17;
			int S14 = 22;
			FF(ref a, ref b, ref c, ref d, inb[0], S11, 3614090360); /* 1 */
			FF(ref d, ref a, ref b, ref c, inb[1], S12, 3905402710); /* 2 */
			FF(ref c, ref d, ref a, ref b, inb[2], S13, 606105819); /* 3 */
			FF(ref b, ref c, ref d, ref a, inb[3], S14, 3250441966); /* 4 */
			FF(ref a, ref b, ref c, ref d, inb[4], S11, 4118548399); /* 5 */
			FF(ref d, ref a, ref b, ref c, inb[5], S12, 1200080426); /* 6 */
			FF(ref c, ref d, ref a, ref b, inb[6], S13, 2821735955); /* 7 */
			FF(ref b, ref c, ref d, ref a, inb[7], S14, 4249261313); /* 8 */
			FF(ref a, ref b, ref c, ref d, inb[8], S11, 1770035416); /* 9 */
			FF(ref d, ref a, ref b, ref c, inb[9], S12, 2336552879); /* 10 */
			FF(ref c, ref d, ref a, ref b, inb[10], S13, 4294925233); /* 11 */
			FF(ref b, ref c, ref d, ref a, inb[11], S14, 2304563134); /* 12 */
			FF(ref a, ref b, ref c, ref d, inb[12], S11, 1804603682); /* 13 */
			FF(ref d, ref a, ref b, ref c, inb[13], S12, 4254626195); /* 14 */
			FF(ref c, ref d, ref a, ref b, inb[14], S13, 2792965006); /* 15 */
			FF(ref b, ref c, ref d, ref a, inb[15], S14, 1236535329); /* 16 */
			/* Round 2 */
			int S21 = 5;
			int S22 = 9;
			int S23 = 14;
			int S24 = 20;

			GG(ref a, ref b, ref c, ref d, inb[1], S21, 4129170786); /* 17 */
			GG(ref d, ref a, ref b, ref c, inb[6], S22, 3225465664); /* 18 */
			GG(ref c, ref d, ref a, ref b, inb[11], S23, 643717713); /* 19 */
			GG(ref b, ref c, ref d, ref a, inb[0], S24, 3921069994); /* 20 */
			GG(ref a, ref b, ref c, ref d, inb[5], S21, 3593408605); /* 21 */
			GG(ref d, ref a, ref b, ref c, inb[10], S22, 38016083); /* 22 */
			GG(ref c, ref d, ref a, ref b, inb[15], S23, 3634488961); /* 23 */
			GG(ref b, ref c, ref d, ref a, inb[4], S24, 3889429448); /* 24 */
			GG(ref a, ref b, ref c, ref d, inb[9], S21, 568446438); /* 25 */
			GG(ref d, ref a, ref b, ref c, inb[14], S22, 3275163606); /* 26 */
			GG(ref c, ref d, ref a, ref b, inb[3], S23, 4107603335); /* 27 */
			GG(ref b, ref c, ref d, ref a, inb[8], S24, 1163531501); /* 28 */
			GG(ref a, ref b, ref c, ref d, inb[13], S21, 2850285829); /* 29 */
			GG(ref d, ref a, ref b, ref c, inb[2], S22, 4243563512); /* 30 */
			GG(ref c, ref d, ref a, ref b, inb[7], S23, 1735328473); /* 31 */
			GG(ref b, ref c, ref d, ref a, inb[12], S24, 2368359562); /* 32 */
			/* Round 3 */
			int S31 = 4;
			int S32 = 11;
			int S33 = 16;
			int S34 = 23;
			HH(ref a, ref b, ref c, ref d, inb[5], S31, 4294588738); /* 33 */
			HH(ref d, ref a, ref b, ref c, inb[8], S32, 2272392833); /* 34 */
			HH(ref c, ref d, ref a, ref b, inb[11], S33, 1839030562); /* 35 */
			HH(ref b, ref c, ref d, ref a, inb[14], S34, 4259657740); /* 36 */
			HH(ref a, ref b, ref c, ref d, inb[1], S31, 2763975236); /* 37 */
			HH(ref d, ref a, ref b, ref c, inb[4], S32, 1272893353); /* 38 */
			HH(ref c, ref d, ref a, ref b, inb[7], S33, 4139469664); /* 39 */
			HH(ref b, ref c, ref d, ref a, inb[10], S34, 3200236656); /* 40 */
			HH(ref a, ref b, ref c, ref d, inb[13], S31, 681279174); /* 41 */
			HH(ref d, ref a, ref b, ref c, inb[0], S32, 3936430074); /* 42 */
			HH(ref c, ref d, ref a, ref b, inb[3], S33, 3572445317); /* 43 */
			HH(ref b, ref c, ref d, ref a, inb[6], S34, 76029189); /* 44 */
			HH(ref a, ref b, ref c, ref d, inb[9], S31, 3654602809); /* 45 */
			HH(ref d, ref a, ref b, ref c, inb[12], S32, 3873151461); /* 46 */
			HH(ref c, ref d, ref a, ref b, inb[15], S33, 530742520); /* 47 */
			HH(ref b, ref c, ref d, ref a, inb[2], S34, 3299628645); /* 48 */

			/* Round 4 */
			int S41 = 6;
			int S42 = 10;
			int S43 = 15;
			int S44 = 21;
			II(ref a, ref b, ref c, ref d, inb[0], S41, 4096336452); /* 49 */
			II(ref d, ref a, ref b, ref c, inb[7], S42, 1126891415); /* 50 */
			II(ref c, ref d, ref a, ref b, inb[14], S43, 2878612391); /* 51 */
			II(ref b, ref c, ref d, ref a, inb[5], S44, 4237533241); /* 52 */
			II(ref a, ref b, ref c, ref d, inb[12], S41, 1700485571); /* 53 */
			II(ref d, ref a, ref b, ref c, inb[3], S42, 2399980690); /* 54 */
			II(ref c, ref d, ref a, ref b, inb[10], S43, 4293915773); /* 55 */
			II(ref b, ref c, ref d, ref a, inb[1], S44, 2240044497); /* 56 */
			II(ref a, ref b, ref c, ref d, inb[8], S41, 1873313359); /* 57 */
			II(ref d, ref a, ref b, ref c, inb[15], S42, 4264355552); /* 58 */
			II(ref c, ref d, ref a, ref b, inb[6], S43, 2734768916); /* 59 */
			II(ref b, ref c, ref d, ref a, inb[13], S44, 1309151649); /* 60 */
			II(ref a, ref b, ref c, ref d, inb[4], S41, 4149444226); /* 61 */
			II(ref d, ref a, ref b, ref c, inb[11], S42, 3174756917); /* 62 */
			II(ref c, ref d, ref a, ref b, inb[2], S43, 718787259); /* 63 */
			II(ref b, ref c, ref d, ref a, inb[9], S44, 3951481745); /* 64 */
			buf[0] += a;
			buf[1] += b;
			buf[2] += c;
			buf[3] += d;
		}
	}

}
