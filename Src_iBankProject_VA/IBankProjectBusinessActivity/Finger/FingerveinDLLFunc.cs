using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Configuration;

namespace IBankProjectBusinessActivity
{
    public class FingerveinDllFun
    {
        private const string s_DLLPathRegister = "Drivers/Fingervein/GRGfingerVeinIDAlgorithm.dll";

       
        //private const string s_DLLPath = "FingerVenaDevDll.dll";
        private const string s_DLLPath = "Drivers/Fingervein/FingerVeinDevDll.dll";
        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iGRGRegister(IntPtr argHandle, byte[,] p_aucsrcModel, byte[] p_aucdstModel, byte[] p_pucstatusNum, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPathRegister, EntryPoint = "GRG_Register", CharSet = CharSet.Auto)]
        public static extern void GRGRegister(byte[] model1, byte[] model2, byte[] model3, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string dstModel, ref byte statusNum);
        [DllImport(s_DLLPathRegister, CharSet = CharSet.Ansi)]
        public static extern int GRGRegister( byte[,] p_aucsrcModel, byte[] p_aucdstModel, ref byte p_pucstatusNum);

        [DllImport(s_DLLPathRegister, CharSet = CharSet.Ansi)]
        public static extern int GRGIdentify( byte[] indentifyModel, byte[] registerModel, ref byte similarity, ref byte statusNum);

        [DllImport(s_DLLPathRegister, CharSet = CharSet.Ansi)]
        public static extern void GRGSameFinger(byte[] model1, byte[] model2, ref int same, byte[] statusNum);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern IntPtr hOpenLogicDevice(string argcLogDevName);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern void vCloseLogicDevice(IntPtr argHandle);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iSetCommPara(IntPtr argHandle, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iCloseComm(IntPtr argHandle, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iInit(IntPtr argHandle, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iGetStatusInfo(IntPtr argHandle, ref tFingerVenaDev_StatusInfo p_psStatusInfo, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iGetFingerVenaInfo(IntPtr argHandle, int p_iInfoType, out int p_puiFVInfoNum, byte[] p_pInfoBuf, ref int p_iInfoBufLen, out tDevReturnArray p_psrStatus);

        [DllImport(s_DLLPath, CharSet = CharSet.Ansi)]
        public static extern int iSetLampPara(IntPtr argHandle, byte p_byMode, int p_iFlashfreq, out tDevReturnArray p_psrStatus);
    }
}
