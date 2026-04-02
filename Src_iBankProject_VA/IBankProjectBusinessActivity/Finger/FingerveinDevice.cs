using LogProcessorService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBankProjectBusinessActivity
{
    public class FingerveinDevice
    {
        private IntPtr m_hDevHandle = new IntPtr();
        private tDevReturnArray m_oArrayStatus = new tDevReturnArray(null);
        

        public FingerveinDevice()
        {
        }

        public int GetFingerveinFeature(out string outFeatureData)
        {
            Log.Action.LogDebug("Run GetFingerveinFeature()");

            outFeatureData = string.Empty;

            int emResult = 0;
            
            long sentTime = currentTime();

            if (!InitFingerveinDevice())
            {
                Log.Action.LogDebug("InitFingerveinDevice failed!");
                return 1;
            }
            int nRet = FingerveinDllFun.iSetLampPara(m_hDevHandle,3, 200, out m_oArrayStatus);
            int waitForPutFingerTimout = 100;
            Log.Action.LogInfoFormat("GetFingerveinFeature() waitForPutFingerTimout is:{0} seconds", waitForPutFingerTimout);
            long TimeOut = waitForPutFingerTimout*1000;
            tFingerVenaDev_StatusInfo statusInfo = new tFingerVenaDev_StatusInfo();
            while(currentTime() - sentTime < TimeOut)
            {
                nRet = FingerveinDllFun.iGetStatusInfo(m_hDevHandle, ref statusInfo, out m_oArrayStatus);
                if (statusInfo.ucBackSenser == 1 && statusInfo.ucFrontSenser == 1)
                {
                    FingerveinDllFun.iSetLampPara(m_hDevHandle, 0xFF, 200, out m_oArrayStatus);

                    byte[] bufInfo = new byte[15000];
                    int infoNo = 0;
                    int bufLen = 15000;
                    nRet = FingerveinDllFun.iGetFingerVenaInfo(m_hDevHandle, 1, out infoNo, bufInfo, ref bufLen, out m_oArrayStatus);
                    if (0 != nRet && 0 != m_oArrayStatus.arrDevReturn[0].LogicCode)
                    {
                        Log.Action.LogInfoFormat("iGetFingerVenaInfo() error is:{0} seconds", m_oArrayStatus.arrDevReturn[0].LogicCode);
                        emResult = 1;
                        break;
                    }
                    outFeatureData = Convert.ToBase64String(bufInfo, 0, 6000);
                    if (!string.IsNullOrEmpty(outFeatureData))
                        break;
                }
            }
            
            if(currentTime() - sentTime >= TimeOut)
            {
                emResult = 2;
            }

            CloseFingerveinDevice();

            return emResult;
        }

        

        private DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        public long currentTime()
        {
            return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
        }

        
        public int CreateFingerveinTemplate(out string outImgBase64, out string outTemplateData)
        {
            Log.Action.LogDebug("Run CreateFingerveinTemplate()");
            int emResult = 0;
            outImgBase64 = string.Empty;
            outTemplateData = string.Empty;
            long sentTime = currentTime();
            string[] strFeatureValue = new string[3];
            byte[][] bFeature = new byte[3][];
            
            int scanTimes = 0;

            if (!InitFingerveinDevice())
            {
                Log.Action.LogDebug("InitFingerveinDevice failed!");
                return 1;
            }
            int nRet = FingerveinDllFun.iSetLampPara(m_hDevHandle, 3, 200, out m_oArrayStatus);
            int waitForPutFingerTimout = 100;
            Log.Action.LogInfoFormat("GetFingerveinFeature() waitForPutFingerTimout is:{0} seconds", waitForPutFingerTimout);
            long TimeOut = waitForPutFingerTimout * 1000;
            tFingerVenaDev_StatusInfo statusInfo = new tFingerVenaDev_StatusInfo();
            while (scanTimes < 3)
            {
                if (currentTime() - sentTime > TimeOut)
                {
                    CloseFingerveinDevice();
                    Log.Action.LogDebug("Scan Fingervein timeout!");
                    return 2;
                }
                    
                nRet = FingerveinDllFun.iGetStatusInfo(m_hDevHandle, ref statusInfo, out m_oArrayStatus);
                if (statusInfo.ucBackSenser == 1 && statusInfo.ucFrontSenser == 1 /*&& statusInfo.ucPicQualified==1*/)
                {
                    FingerveinDllFun.iSetLampPara(m_hDevHandle, 0xFF, 200, out m_oArrayStatus);
                    byte[] bufInfo = new byte[15000];
                    int infoNo = 0;
                    int bufLen = 15000;
                    nRet = FingerveinDllFun.iGetFingerVenaInfo(m_hDevHandle, 1, out infoNo, bufInfo, ref bufLen, out m_oArrayStatus);
                    if (0 != nRet && 0 != m_oArrayStatus.arrDevReturn[0].LogicCode)
                    {
                        Log.Action.LogInfoFormat("iGetFingerVenaInfo() error is:{0} seconds", m_oArrayStatus.arrDevReturn[0].LogicCode);
                        emResult = 1;
                        break;
                    }
                    bFeature[scanTimes] = bufInfo;
                    strFeatureValue[scanTimes] = Convert.ToBase64String(bufInfo, 0, 6000);
                    Log.Action.LogInfoFormat("strFeatureValue length = {0}", strFeatureValue[scanTimes].Length);
                    if (!string.IsNullOrEmpty(strFeatureValue[scanTimes]))
                    {
                        
                        int intSame = 255;
                        byte[] statusNum1 = new byte[3];
                        if(scanTimes>0)
                        {

                            FingerveinDllFun.GRGSameFinger(bFeature[scanTimes - 1], bFeature[scanTimes], ref intSame, statusNum1);
                            //Log.Action.LogDebugFormat("intSame={0},statusNum={1}", intSame, statusNum1[0]);
                            if (intSame != 1 || statusNum1[0]!= 0)
                            {
                                continue;
                            }
                        }
                        scanTimes++;
                        FingerveinDllFun.iSetLampPara(m_hDevHandle, 3, 200, out m_oArrayStatus);
                    }
                }
            }
            byte[] bufInfo1 = new byte[45000];
            byte resultInfo = 255;
            if (emResult == 0)
            {
                
                byte[,] info = ToChangeByte(bFeature);
                nRet = FingerveinDllFun.GRGRegister(info, bufInfo1,ref resultInfo);
                outTemplateData = Convert.ToBase64String(bufInfo1, 0, 18000);
            }
            CloseFingerveinDevice();
            return emResult;
        }

        public static void Bytes2File(byte[] buff, string savepath)
        {
            if (File.Exists(savepath))
            {
                File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff, 0, 6000);
            bw.Close();
            fs.Close();
        }
        public static void String2File(string buff, string savepath)
        {
            if (File.Exists(savepath))
            {
                File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff);
            bw.Close();
            fs.Close();
        }

        public static void TBytes2File(byte[] buff, string savepath)
        {
            if (File.Exists(savepath))
            {
                File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff, 0, 18000);
            bw.Close();
            fs.Close();
        }

        private byte[,] ToChangeByte(byte[][] text)
        {
            byte[,] array = new byte[3, 15000];
            for (int i = 0; i < text.Length && i < 120; i++)
            {
                for (int j = 0; j < text[i].Length; j++)
                {
                    //array[i, j] = charToByte(text[i][j])[1];
                    array[i, j] = text[i][j];
                }

            }
            return array;
        }

        private void CloseFingerveinDevice()
        {
            FingerveinDllFun.iSetLampPara(m_hDevHandle, 0xFF, 200, out m_oArrayStatus);
            int nRet = FingerveinDllFun.iCloseComm(m_hDevHandle, out m_oArrayStatus);
            if (m_hDevHandle != null)
            {
                FingerveinDllFun.vCloseLogicDevice(m_hDevHandle);
            }
        }

        private bool InitFingerveinDevice()
        {
            try
            {
                m_hDevHandle = FingerveinDllFun.hOpenLogicDevice("FingerVenaDev");

                Log.Action.LogDebug("hOpenLogicDevice");
                m_oArrayStatus.ReSet();
                int nRet = FingerveinDllFun.iSetCommPara(m_hDevHandle, out m_oArrayStatus);
                Log.Action.LogDebug("iSetCommPara");

                m_oArrayStatus.ReSet();
                nRet = FingerveinDllFun.iInit(m_hDevHandle, out m_oArrayStatus);
                Log.Action.LogDebug("iInit");

                if (nRet == 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                Log.Action.LogDebug("hOpenLogicDevice Error");
                return false;
            }
            
        }

    }
}
