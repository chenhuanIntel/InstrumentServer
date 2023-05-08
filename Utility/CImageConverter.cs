using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NLog;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class CImageConverter
    {
        /// <summary>
        /// Get a bitmap. The using statement ensures objects  
        /// are automatically disposed from memory after use.
        /// </summary>
        /// <param name="jpgFileInputPath"></param>
        /// <param name="jpgFileOutputPath"></param>
        /// <param name="CompressLevel"></param>
        public static bool ChangeJPGImageQuality(string jpgFileInputPath, string jpgFileOutputPath, long CompressLevel)
        {

            try
            {
                //using (Bitmap bmp1 = new Bitmap(@"C:\Users\kanghe\Desktop\TestLocalV1.1\resize\TestPhoto.jpg"))
                using (Bitmap bmp1 = new Bitmap(jpgFileInputPath))
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  
                    // An EncoderParameters object has an array of EncoderParameter  
                    // objects. In this case, there is only one  
                    // EncoderParameter object in the array.  
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, CompressLevel);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    bmp1.Save(jpgFileOutputPath, jpgEncoder, myEncoderParameters);
                }

            }
            catch (HardwareErrorException ex)
            {
                clog.Error(ex, "CImageConverter get exception");
            }

            return true;
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
