using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
//using System.Windows.Forms;
using LogProcessorService;

namespace IBankProjectBusinessActivity
{
    public static class ImgProc
    {
        /// <summary>
        /// Base64编码的字符串转为图片
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static Bitmap Base64StringToImage(byte[] arrBase64, string fileName)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(System.Text.Encoding.ASCII.GetString(arrBase64));
                MemoryStream memStream = new MemoryStream(arr);
                Bitmap bitmap = new Bitmap(memStream);

                bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                memStream.Close();
                return bitmap;
            }
            catch (System.ArgumentException ex)
            {
                Log.BusinessService.LogErrorFormat("byte[] Base64String convert to image failed:{0}", ex.Message);
                return null;
            }
        }

        public static Bitmap Base64StringToImage(string imgDataBase64, string fileName)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(imgDataBase64);
                MemoryStream memStream = new MemoryStream(arr);
                Bitmap bitmap = new Bitmap(memStream);

                bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                memStream.Close();
                return bitmap;
            }
            catch (System.ArgumentException ex)
            {
                Log.BusinessService.LogErrorFormat("Base64String convert to image failed:{0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 图片转为base64编码的字符串
        /// </summary>
        /// <param name="Imagefilename"></param>
        /// <returns></returns>
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                if (!File.Exists(Imagefilename))
                {
                    Log.BusinessService.LogErrorFormat("The file is not exists. " + Imagefilename);
                    return null;
                }

                Bitmap bitmap = new Bitmap(Imagefilename);

                MemoryStream memStream = new MemoryStream();
                bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(arr, 0, (int)memStream.Length);
                memStream.Close();
                bitmap.Dispose();
                return Convert.ToBase64String(arr);
            }
            catch (System.Exception ex)
            {
                Log.BusinessService.LogErrorFormat("Imagefilename Base64String convert to image failed:{0}", ex.Message);
                return null;
            }
        }

        public static string GetStringFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                return Encoding.ASCII.GetString(bytes);
            }
            return string.Empty;
        }

        public static string BuildPath(string format)
        {
            return string.Format("{0}{1}\\{2}.{3}", AppDomain.CurrentDomain.BaseDirectory, "Temp", Guid.NewGuid().ToString(), format);
        }

    }
}
