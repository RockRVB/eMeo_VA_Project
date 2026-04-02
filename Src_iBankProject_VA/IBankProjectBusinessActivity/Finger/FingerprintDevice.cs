using LogProcessorService;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace IBankProjectBusinessActivity
{
    public class FingerprintDevice
    {
        private string imageBase64 = string.Empty;
        private string featureData = string.Empty;
        private BioneFPAPILib.FPAPICtrlClass fp = null;
        private AutoResetEvent autoResetEvent = null;
        private bool IsInit = false;

        public FingerprintDevice()
        {
            OpenFingerprint();
        }

        public void OpenFingerprint()
        {
            fp = new BioneFPAPILib.FPAPICtrlClass();
            IsInit = InitFingerprintDevice();
            fp.FeatureCallbackOfBae64 += Fp_FeatureCallbackOfBae64;
            fp.FingerPrintStateOfBae64 += Fp_FingerPrintStateOfBae64;
            fp.TemplateCallbackOfBae64 += Fp_TemplateCallbackOfBae64;
        }

        public void CloseFingerprint()
        {
            fp.CloseDevice();
            fp.UnInitDevice();
            fp.FeatureCallbackOfBae64 -= Fp_FeatureCallbackOfBae64;
            fp.FingerPrintStateOfBae64 -= Fp_FingerPrintStateOfBae64;
            fp.TemplateCallbackOfBae64 -= Fp_TemplateCallbackOfBae64;
            IsInit = false;
        }

        public int GetFingerprintFeature(out string outFeatureData, out string outimageBase64)
        {
            Log.Action.LogDebug("Run GetFingerprintFeature().");

            ClearFeature();

            outimageBase64 = string.Empty;
            outFeatureData = string.Empty;

            int emResult = 0;

            if (!CheckDeviceStatus())
            {
                Log.Action.LogDebug("InitFingerprintDevice failed.");
                return 700550;
            }

            if (fp.StartGetFeature() != 0)
            {
                Log.Action.LogDebug("StartGetFeature() failed.");
                fp.StopGetFeature();
                return 700552;
            }

            int waitForPutFingerTimout = 100;
            Log.Action.LogInfoFormat("GetFingerprintFeature() waitForPutFingerTimout is:{0} seconds", waitForPutFingerTimout);
            autoResetEvent = new AutoResetEvent(false);
            bool waitResult = autoResetEvent.WaitOne(TimeSpan.FromSeconds(waitForPutFingerTimout));

            if (!waitResult)
            {
                emResult = 700553;
            }

            outFeatureData = featureData;
            outimageBase64 = imageBase64;

            if (string.IsNullOrEmpty(featureData) || string.IsNullOrEmpty(imageBase64))
                emResult = 700402;

            fp.StopGetFeature();

            CloseFingerprint();

            return emResult;
        }

        public int CreateFingerprintTemplate(out string outImgBase64, out string outTemplateData)
        {
            Log.Action.LogDebug("Run CreateFingerprintTemplate().");
            int emResult = 0;
            ClearFeature();
            outImgBase64 = string.Empty; outTemplateData = string.Empty;
            if (!CheckDeviceStatus())
            {
                Log.Action.LogDebug("InitFingerprintDevice failed.");
                return 700550;
            }

            if (fp.StartGetTemplate() != 0)
            {
                Log.Action.LogDebug("StartGetTemplate failed.");
                fp.StopGetTemplate();
                return 700551;
            }

            int waitForPutFingerTimout = 100;
            autoResetEvent = new AutoResetEvent(false);
            bool waitResult = autoResetEvent.WaitOne(TimeSpan.FromSeconds(waitForPutFingerTimout));

            if (!waitResult)
            {
                emResult = 700553;
            }


            outImgBase64 = imageBase64;
            outTemplateData = featureData;

            if (string.IsNullOrEmpty(featureData) || string.IsNullOrEmpty(imageBase64))
                emResult = 700402;

            fp.StopGetTemplate();

            CloseFingerprint();

            return emResult;
        }

        private void ClearFeature()
        {
            Log.Action.LogDebug("Run ClearFeature()");
            imageBase64 = string.Empty;
            featureData = string.Empty;
        }

        private bool CheckDeviceStatus()
        {
            if (!IsInit)// try to open again
            {
                CloseFingerprint();
                OpenFingerprint();
            }
            return IsInit;
        }

        private void Fp_FeatureCallbackOfBae64(string featureDataBuf, short featureSize, short pressTimes, short featureNum, short gType)
        {
            Log.Action.LogDebug("Run Fp_FeatureCallbackOfBae64()");
            featureData = featureDataBuf;
            autoResetEvent.Set();
        }

        private bool InitFingerprintDevice()
        {
            try
            {
                int result = fp.AutoSelectDevice();
                if (result != 0)
                {
                    Log.Action.LogInfoFormat("AutoSelectDevice falied:" + result.ToString());
                    return false;
                }

                result = fp.InitDevice();
                if (result != 0)
                {
                    Log.Action.LogInfoFormat("InitDevice falied:" + result.ToString());
                    return false;
                }

                result = fp.OpenDevice();
                if (result != 0)
                {
                    Log.Action.LogInfoFormat("OpenDevice falied:" + result.ToString());
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Log.Action.LogInfoFormat("InitFingerprintDevice:" + ex.Message);
                return false;
            }

            return true;
        }

        private void Fp_FingerPrintStateOfBae64(string imgDataBuf, uint imgDataBufLength, short imgWidth, short imgHeight, short nowStep, short nowState)
        {
            Log.Action.LogDebug("Run Fp_FingerPrintStateOfBae64()");
            if (nowStep == 0)
            {
                char[] charBuffer = imgDataBuf.ToCharArray();
                byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                using (Bitmap bmp = CreateBitmap(bytes, imgWidth, imgHeight))
                using (MemoryStream memStream = new MemoryStream())
                {
                    bmp.Save(memStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] arr = new byte[memStream.Length];
                    memStream.Position = 0;
                    memStream.Read(arr, 0, (int)memStream.Length);
                    imageBase64 = Convert.ToBase64String(arr);
                }
            }
        }

        private void Fp_TemplateCallbackOfBae64(string featureDataBuf, short featureSize, short pressTimes, short featureNum, short gType)
        {
            Log.Action.LogDebug("Run Fp_TemplateCallbackOfBae64()");
            featureData = featureDataBuf;
            autoResetEvent.Set();
        }

        private static Bitmap CreateBitmap(byte[] originalImageData, int originalWidth, int originalHeight)
        {
            using (Bitmap resultBitmap = new Bitmap(originalWidth, originalHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                MemoryStream curImageStream = new MemoryStream();
                resultBitmap.Save(curImageStream, System.Drawing.Imaging.ImageFormat.Bmp);
                curImageStream.Flush();
                int curPadNum = ((originalWidth * 8 + 31) / 32 * 4) - originalWidth;
                int bitmapDataSize = ((originalWidth * 8 + 31) / 32 * 4) * originalHeight;
                int dataOffset = ReadData(curImageStream, 10, 4);
                int paletteStart = 54;
                int paletteEnd = dataOffset;
                int color = 0;

                for (int i = paletteStart; i < paletteEnd; i += 4)
                {
                    byte[] tempColor = new byte[4];
                    tempColor[0] = (byte)color;
                    tempColor[1] = (byte)color;
                    tempColor[2] = (byte)color;
                    tempColor[3] = (byte)0;
                    color++;

                    curImageStream.Position = i;
                    curImageStream.Write(tempColor, 0, 4);
                }
                byte[] destImageData = new byte[bitmapDataSize];
                int destWidth = originalWidth + curPadNum;
                for (int originalRowIndex = originalHeight - 1; originalRowIndex >= 0; originalRowIndex--)
                {
                    int destRowIndex = originalHeight - originalRowIndex - 1;

                    for (int dataIndex = 0; dataIndex < originalWidth; dataIndex++)
                    {
                        destImageData[destRowIndex * destWidth + dataIndex] = originalImageData[originalRowIndex * originalWidth + dataIndex];
                    }
                }
                curImageStream.Position = dataOffset;
                curImageStream.Write(destImageData, 0, bitmapDataSize);

                curImageStream.Flush();

                return new Bitmap(curImageStream);
            }
        }

        private static int ReadData(MemoryStream curStream, int startPosition, int length)
        {
            int result = -1;
            byte[] tempData = new byte[length];
            curStream.Position = startPosition;
            curStream.Read(tempData, 0, length);
            result = BitConverter.ToInt32(tempData, 0);
            return result;
        }


        public static class AratekAUFDecrypt
        {
            /// <summary>
            /// 特征数据格式
            /// </summary>
            public enum FeatureFormat : int
            {
                /// <summary>
                /// 二进制
                /// </summary>
                BinaryArray = 0,

                /// <summary>
                /// base64字符串
                /// </summary>
                Base64String = 1,

                /// <summary>
                /// 十六进制字符串
                /// </summary>
                HexString = 2
            }

            /// <summary>
            /// 对FPAPI得到的Bione指纹特征进行解密
            /// </summary>
            /// <param name="DataIn_Format">Bione特征数据格式，参考FeatureFormat枚举</param>
            /// <param name="AUFFeature">Bione特征数据</param>
            /// <param name="AUFFeature_Length">Bione特征数据长度</param>
            /// <param name="DataOut_Format">指定输出解密后特征数据的格式，参考FeatureFormat枚举</param>
            /// <param name="DecryptFeature">解密后特征数据，需要自己分配内存（BinaryArray格式下建议分配1024字节，Base64String和HexString格式下建议分配2048字节）</param>
            /// <param name="DecryptFeature_Length">解密后的特征数据实际长度</param>
            /// <returns>错误码。0: 成功；-1: 参数错误；-2: 数据错误；-4: 数据长度不对；-5: 内存分配失败。</returns>
            private delegate int ARA_AUFDecrypt(int DataIn_Format, byte[] AUFFeature, int AUFFeature_Length, int DataOut_Format, IntPtr DecryptFeature, ref int DecryptFeature_Length);

            [DllImport(@"Lib\Fingerprint\x64\AratekAUFDecrypt.dll", EntryPoint = "ARA_AUFDecrypt")]
            private static extern int ARA_AUFDecryptX64(int DataIn_Format, byte[] AUFFeature, int AUFFeature_Length, int DataOut_Format, IntPtr DecryptFeature, ref int DecryptFeature_Length);

            [DllImport(@"Lib\Fingerprint\x86\AratekAUFDecrypt.dll", EntryPoint = "ARA_AUFDecrypt")]
            private static extern int ARA_AUFDecryptX86(int DataIn_Format, byte[] AUFFeature, int AUFFeature_Length, int DataOut_Format, IntPtr DecryptFeature, ref int DecryptFeature_Length);

            private static ARA_AUFDecrypt DecryptAPI
            {
                get
                {
                    return Environment.Is64BitProcess ? (ARA_AUFDecrypt)ARA_AUFDecryptX64 : ARA_AUFDecryptX86;
                }
            }

            public static int Decrypt(FeatureFormat dataInFormat, byte[] encryptedFeature, FeatureFormat dataOutFormat, out byte[] decryptedFeature)
            {
                var actualLen = -1;
                var bufferPtr = Marshal.AllocHGlobal(dataInFormat == FeatureFormat.BinaryArray ? 1024 : 2048);

                try
                {
                    var retCode = DecryptAPI((int)dataInFormat, encryptedFeature, encryptedFeature.Length, (int)dataOutFormat, bufferPtr, ref actualLen);

                    if (retCode != 0)
                    {
                        decryptedFeature = null;
                        return retCode;
                    }

                    decryptedFeature = new byte[actualLen];
                    Marshal.Copy(bufferPtr, decryptedFeature, 0, actualLen);
                    return retCode;
                }
                finally
                {
                    Marshal.FreeHGlobal(bufferPtr);
                }
            }
        }

    }
}
