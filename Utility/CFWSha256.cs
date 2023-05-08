using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// data class for SHA 256 calculation
    /// </summary>
    public class SHA_256_t
    {
        /// <summary>
        /// sha value holder
        /// </summary>
        public uint[] h;
        /// <summary>
        /// sha input data
        /// </summary>
        public uint[] w;
        /// <summary>
        /// data index
        /// </summary>
        public int w_byte_index;
        /// <summary>
        /// constructor
        /// </summary>
        public SHA_256_t()
        {
            h = new uint[8];
            w = new uint[16];
            w_byte_index = 0;
        }
    }

    /// <summary>
    /// Firmware sha256 calculation
    /// </summary>
    public class CFWSha256
    {

        private SHA_256_t sha_256 = new SHA_256_t();
        private const int SHA_HASH_BYTES = 32;
        private const int HASH_BUF_SIZE = 8;
        private const int KEY_BYTES_MAX = 64;
        private const int MESSAGE_FILL_SIZE = 64;
        private const int MESSAGE_BUF_SIZE = 64;
        private uint[] h_init = new uint[]{ 0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
            0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19 };
        private uint[]  k = new uint[] {
	        0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
	        0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
	        0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
	        0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
	        0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
	        0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
	        0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
	        0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
	        0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
	        0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
	        0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
	        0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
	        0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
	        0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
	        0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
	        0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2,
        };

        /// <summary>
        /// constructor
        /// </summary>
        public CFWSha256()
        {
        }

        /// <summary>
        /// FW function to generate HMAC 256 password 
        /// </summary>
        /// <param name="passwordkey">PMIC key</param>
        /// <param name="product_id">module product id</param>
        /// <param name="serial_num">module serial number</param>
        /// <param name="password">generated password</param>
        public void hmac_password_256bit_gen(byte[] passwordkey, byte[] product_id, byte[] serial_num, ref byte[]password)
        {
            byte[] buf = new byte[32];
            hmac_256bit_gen(passwordkey, 0x36, product_id, serial_num, ref buf);
            byte[] pd_id = new byte[16];
            byte[] sn = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                pd_id[i] = buf[i];
                sn[i] = buf[i + 16];
            }
            hmac_256bit_gen(passwordkey, 0x5C, pd_id, sn, ref password);
        }

        private void hmac_256bit_gen(byte[] passwordkey, byte mask, byte[] product_id, byte[] serial_num, ref byte[] password)
        {
            app_sha_256_init(passwordkey, mask, 32); // init for SHA-256 algorithm
            app_sha_256_update(product_id, 16); // update hash table with product ID
            app_sha_256_update(serial_num, 16); // update hash table with product ID
            app_sha_256_complete(); // complete SHA-256 algorithm
            app_sha_256_get_hash(ref password); // get calculated hash value.
        }


        private void app_sha_256_init(byte[] key, byte hmac_mask, int length)
        {
            int i;
            byte[] buf = new byte[SHA_HASH_BYTES];

            for (i = 0; i < HASH_BUF_SIZE; i++)
            { // init hash table with init value
                sha_256.h[i] = h_init[i];
            }
            sha_256.w_byte_index = 0;

            if (length > KEY_BYTES_MAX)
                length = KEY_BYTES_MAX;

            // updat hash table with key
            for (i = 0; i < SHA_HASH_BYTES; i++)
            {
                buf[i] =(byte)( key[i] ^ hmac_mask);
            }

            app_sha_256_update(buf, length);
        }

        
        private void app_sha_256_update(byte[] data, int length)
        {
            int i, j;

            j = sha_256.w_byte_index;
            for (i = 0; i < length; i+=4)
            {
                sha_256.w[j/4] = (uint)(data[i + 3] << 24) + (uint)(data[i + 2] << 16) + (uint)(data[i+1]<<8)+data[i];
                j+=4;
                if (j >= MESSAGE_FILL_SIZE)
                { // 512-bit message has been filled, update the hash table based on the new data.
                    app_sha_256_calculate();
                    j = 0; // reset index to 0 for more data
                }
            }
            sha_256.w_byte_index = j; // save current position for next.
        }

        
        private void app_sha_256_complete()
        {
            int i;
            if (sha_256.w_byte_index != 0)
            { // need to fill buffer and update hash table with left data
                for (i = sha_256.w_byte_index; i < MESSAGE_BUF_SIZE; i+=4)
                { // fill the unfilled buffer with 0x49.
                    sha_256.w[i/4] = 0x49494949;
                }
                app_sha_256_calculate();
                sha_256.w_byte_index = 0;
            }
        }

        private void app_sha_256_get_hash(ref byte[] hash)
        {
            int i;
            for (i = 0; i < (HASH_BUF_SIZE * 4); i+=4)
            { // copy hash table content
                hash[i] =(byte)( sha_256.h[i/4] &0xff);
                hash[i+1] = (byte)(sha_256.h[i/4]>>8 & 0xff);
                hash[i+2] = (byte)(sha_256.h[i/4]>>16 & 0xff);
                hash[i+3] = (byte)(sha_256.h[i/4] >>24 & 0xff);
            }
        }

        private void app_sha_256_calculate()
        {
            int i;
            uint a, b, c, d, e, f, g, h;
            uint s0, s1;
            uint ch, temp1, temp2, maj;
            uint[] w = new uint[64];

            for (i = 0; i < 16; i++)
            { // 1. copy first 16 words to local buffer.
                w[i] = sha_256.w[i];
            }

            // 2. expend first 16 32-bit words to 64 words.
            for (i = 16; i < 64; i++)
            {
                s0 = (w[i - 15] >> 7 | w[i - 15] << 25);
                s0 ^= (w[i - 15] >> 18 | w[i - 15] << 14);
                s0 ^= (w[i - 15] >> 3);

                s1 = (w[i - 2] >> 17 | w[i - 2] << 15);
                s1 ^= (w[i - 2] >> 19 | w[i - 2] << 13);
                s1 ^= (w[i - 2] >> 10);

                w[i] = w[i - 16] + s0 + w[i - 7] + s1;
            }

            // 3. load current hash table
            a = sha_256.h[0];
            b = sha_256.h[1];
            c = sha_256.h[2];
            d = sha_256.h[3];
            e = sha_256.h[4];
            f = sha_256.h[5];
            g = sha_256.h[6];
            h = sha_256.h[7];

            // 4. Apply compression function
            for (i = 0; i < 64; i++)
            {
                s1 = (e >> 6 | e << 26);
                s1 ^= (e >> 11 | e << 21);
                s1 ^= (e >> 25 | e << 7);

                ch = (e & f) ^ (~e & g);

                temp1 = h + s1 + ch + k[i] + w[i];

                s0 = (a >> 2 | a << 30);
                s0 ^= (a >> 13 | a << 19);
                s0 ^= (a >> 22 | a << 10);

                maj = (a & b) ^ (a & c) ^ (b & c);

                temp2 = s0 + maj;

                h = g;
                g = f;
                f = e;
                e = d + temp1;
                d = c;
                c = b;
                b = a;
                a = temp1 + temp2;
            }

            // 5. Update hash table
            sha_256.h[0] += a;
            sha_256.h[1] += b;
            sha_256.h[2] += c;
            sha_256.h[3] += d;
            sha_256.h[4] += e;
            sha_256.h[5] += f;
            sha_256.h[6] += g;
            sha_256.h[7] += h;
        }
    }
}
