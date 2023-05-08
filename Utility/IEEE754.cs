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
    public class IEEE754
    {
        /// <summary>
        /// 
        /// </summary>
        public const double EXPONENT_BITS = 6.0;
        /// <summary>
        /// 
        /// </summary>
        public const double MANTISSA_BITS = 9.0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] convertSingleToHalfPrecisionByteArray(Single s)
        {
            Debug.WriteLine(string.Format("convertSingleToHalfPrecisionByteArray({0})", s));

            byte[] arBytes = new byte[2];

            byte[] arByteSingle = BitConverter.GetBytes(s);
            UInt32 uDataIn = BitConverter.ToUInt32(arByteSingle, 0);
            UInt16 nDataOut;
            byte bTemp;

            //zero check
            if (s == 0)
            {
                arBytes[0] = 0;
                arBytes[1] = 0;
                return arBytes;
            }

            //mantissa
            nDataOut = (UInt16)((uDataIn & 0x007FFFFF) >> 14);
            bTemp = (byte)((uDataIn >> 23) & 0xFF);

            //overflow check
            if ((bTemp & 0x80) != 0x0)
            {
                if ((bTemp & 0x60) != 0x0)
                {
                    arBytes[0] = 0;
                    arBytes[1] = 0;
                    return arBytes;
                }
            }
            else
            {
                if ((bTemp & 0x60) == 0x0)
                {
                    arBytes[0] = 0;
                    arBytes[1] = 0;
                    return arBytes;
                }
            }

            //exponent
            nDataOut |= (UInt16)(((bTemp - 96) & 0x3F) << 9);

            //signbit
            if ((uDataIn & 0x80000000) != 0x0)
            {
                nDataOut |= 0x8000;
            }

            arBytes[0] = (byte)((nDataOut & 0xFF00) >> 8); //High Byte
            arBytes[1] = (byte)(nDataOut & 0xFF); //Low Byte

            return arBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arBytes"></param>
        /// <returns></returns>
        public static Single convertHalfPrecisionByteArrayToSingle(byte[] arBytes)
        {
            Debug.WriteLine(string.Format("convertHalfPrecisionByteArrayToSingle({0})", BitConverter.ToString(arBytes)));

            Single s = 0.0F;
            UInt16 nDataIn = (UInt16)bytesToInt32(arBytes);
            UInt32 lDataOut = 0;

            //zero check
            if (nDataIn == 0 || nDataIn == 0xffffU)
            {
                return 0.0F;
            }

            //mantissa
            lDataOut = ((UInt32)nDataIn & 0x01FF) << 14;

            //exponent
            lDataOut |= (((((UInt32)nDataIn >> 9) & 0x3F) + 96) & 0xFF) << 23;

            //signbit
            if ((nDataIn & 0x8000) != 0)
            {
                lDataOut |= 0x80000000;
            }

            //Convert to byte array
            byte[] arByteSingle = BitConverter.GetBytes(lDataOut);

            //Convert byte array to Single
            s = BitConverter.ToSingle(arByteSingle, 0);

            return s;
        }

        /// <summary>
        /// Converts a double precision number to a half precision representation in a 16 byte byte array (Big-Endian)
        /// This method uses the calculated algorithm which takes longer
        /// </summary>
        /// <param name="d"></param>
        /// <returns>Array of bytes in Big-Endian format (16-bit value)</returns>
        public static byte[] convertDoubleToByteArrayClassic(Double d)
        {
            byte[] arBytes = new byte[2];

            if (double.IsPositiveInfinity(d))
                throw new Exception("+Infinity");
            if (double.IsNegativeInfinity(d))
                throw new Exception("-Infinity");
            if (double.IsNaN(d))
                throw new Exception("NaN");


            bool negative = (d < 0);
            int exponent = (int)Math.Floor(Math.Log(Math.Abs(d), 2.0));
            double dFraction = Math.Abs(d / Math.Pow(2.0, exponent));

            //Now convert our Fraction into the mantissa
            dFraction = dFraction - 1.0;

            int nMantissa = 0;
            int nDig = 0;

            int n = 0;
            for (n = 0; n < 9; n++)
            {
                dFraction = (dFraction * 2.0);
                nDig = (int)(dFraction);
                dFraction = dFraction - (double)nDig;
                nMantissa = nMantissa * 2 + nDig;
            }

            int nRem = 0;

            //Get the next 4 digits in the mantissa to get extra precision
            for (n = 0; n <= 3; n++)
            {
                dFraction = (dFraction * 2.0);
                nDig = (int)(dFraction);
                dFraction = dFraction - (double)nDig;
                nRem = nRem * 2 + nDig;
            }

            Debug.WriteLine("Before nMantissa", nMantissa.ToString("X"));

            //Do some digital rounding to buy us some extra precision
            if (nRem >= 8)
            {
                nMantissa = nMantissa + 1;//Round up in the LSB of the mantissa
            }

            Debug.WriteLine("d value", d.ToString());
            Debug.WriteLine("exponent", exponent.ToString("X"));
            Debug.WriteLine("nMantissa", nMantissa.ToString("X"));
            Debug.WriteLine("nRem", nRem.ToString("X"));

            //Now lets map this into the 16-bit half precision number
            //Half Precision Value = (-1)^signbit * 2^(El-31) * 1.b8b7...b0
            //El = exponent + 31

            //Sign bit size = 1
            //Exponent = 6
            //Mantissa = 9

            int nReg = 0;
            int nSign = negative ? (1 << 15) : 0;
            int El = (exponent + 31) & (0x3F);

            nReg = nReg | nSign | (El << 9) | nMantissa;

            Debug.WriteLine("El", El.ToString("X"));
            Debug.WriteLine("nReg", nReg.ToString("X"));

            arBytes[0] = (byte)((nReg & 0xFF00) >> 8); //High Byte
            arBytes[1] = (byte)(nReg & 0xFF); //Low Byte

            Debug.WriteLine(string.Format("IEEE754.convertDoubleToByteArrayClassic -> {0}", BitConverter.ToString(arBytes)));

            return arBytes;
        }

        /// <summary>
        /// Converts a double precision number to a half precision representation in a 16 byte byte array (Big-Endian)
        /// This method uses the internal IEEE754 representation of a 64-bit double 
        /// </summary>
        /// <param name="d"></param>
        /// <returns>Array of bytes in Big-Endian format (16-bit value)</returns>
        public static byte[] convertDoubleToByteArray(double d)
        {
            byte[] arBytes = new byte[2];

            if (double.IsPositiveInfinity(d))
                throw new Exception("+Infinity");
            if (double.IsNegativeInfinity(d))
                throw new Exception("-Infinity");
            if (double.IsNaN(d))
                throw new Exception("NaN");

            // Translate the double into sign, exponent and mantissa.
            long bits = BitConverter.DoubleToInt64Bits(d);
            // Note that the shift is sign-extended, hence the test against -1 not 1
            bool negative = (bits < 0);
            int Ed = (int)((bits >> 52) & 0x7ffL);
            long mantissa = bits & 0xfffffffffffffL;

            Debug.WriteLine("d value = {0}", d.ToString());
            Debug.WriteLine("bits = {0}", bits.ToString("X"));
            Debug.WriteLine("exponent = {0}", Ed.ToString("X"));
            Debug.WriteLine("mantissa = {0}", mantissa.ToString("X"));

            // Now lets map this into the 16-bit half precision number

            //64-bit Precision Value = (-1)^signbit * 2^(Ed-1023) * 1.b51b50...b0
            //Half Precision Value = (-1)^signbit * 2^(El-31) * 1.b8b7...b0
            //U1 = U2 = (Ed-1023) = (El-31) -> El = Ed - 992

            int nReg = 0;
            int nSign = negative ? 0x8000 : 0;
            int El = Ed - 992;
            int nChoppedMantissa = (int)((mantissa >> 43) & 0x1FF);
            int nExtendedRemainder = (int)((mantissa >> 39) & 0xF);//Grab the last four bits of the extended remainder

            Debug.WriteLine("Before nChoppedMantissa = {0}", nChoppedMantissa.ToString("X"));

            //Round up to obtain a extra bit of precision
            if (nExtendedRemainder >= 8)
            {
                nChoppedMantissa = nChoppedMantissa + 0x1;
            }

            nReg = nReg | nSign | (El << 9) | nChoppedMantissa;

            Debug.WriteLine("nExtendedRemainder = {0}", nExtendedRemainder.ToString("X"));
            Debug.WriteLine("nChoppedMantissa = {0}", nChoppedMantissa.ToString("X"));
            Debug.WriteLine("El = {0}", El.ToString("X"));
            Debug.WriteLine("nReg = {0}", nReg.ToString("X"));

            //Target is big endian
            arBytes[0] = (byte)((nReg & 0xFF00) >> 8); //High Byte
            arBytes[1] = (byte)(nReg & 0xFF); //Low Byte

            Debug.WriteLine(string.Format("IEEE754.convertDoubleToByteArray -> {0}", BitConverter.ToString(arBytes)));

            return arBytes;
        }

        /// <summary>
        /// Converts values to double (Big Endian)
        /// </summary>
        /// <param name="arBytes">Array of bytes in Big-Endian format (16-bit value)</param>
        /// <returns>the double precision value</returns>
        private static int bytesToInt32(byte[] arBytes)
        {
            int value = 0;
            for (int i = 0; i < arBytes.Count(); i++)
            {
                value = value * 256 + (int)arBytes[i];
            }

            return value;
        }

        /// <summary>
        /// Converts to double from an array of bytes in half-precision IEEE754 (not to protocol -- Mark's implementation)
        /// </summary>
        /// <param name="arBytes">Array of bytes in Big-Endian format (16-bit value)</param>
        /// <returns>the double precision value</returns>
        public static double convertToDouble(byte[] arBytes)
        {
            Debug.WriteLine(string.Format("IEEE754.convertToDouble({0}", BitConverter.ToString(arBytes)));

            double dVal = 0.0;

            //Big Endian format
            int nReg = bytesToInt32(arBytes);

            //b15 := sign
            int sign = ((nReg >> 15) & 0x1);

            //b14-b8 := exponent part
            int Exp = ((nReg & 0x7F00) >> 9);

            //b7-b0 := fraction
            int nFraction = nReg & 0x1FF;


            if (Exp == 0)
            {
                dVal = Math.Pow(2.0, -30) * ((double)nFraction / (double)512.0F);
            }
            else
            {

                dVal = Math.Pow(2.0, Exp - 31) * (1.0 + (double)nFraction / (double)512.0F);
            }

            if (sign != 0)
                dVal = dVal * -1.0;

            Debug.WriteLine("dVal = {0}; nReg = {1}; sign = {2}; Exp = {3}; nFraction = {4}", dVal, nReg, sign, Exp, nFraction);

            return dVal;
        }
    }
}
