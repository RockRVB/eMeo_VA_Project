using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace IBankProjectBusinessActivity
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct tDevReturn
    {
        // Logical Code
        public int LogicCode;

        // Physical Code
        public int PhyCode;

        // Handle: 0 - none, 1 - Initialize, 2 - Resent Command
        public int Handle;

        // Error type: 0 - Warning, 1 - Fatal
        public int Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DevReturn;	    // 硬件返回信息

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Reserve;		// 保留信息

        public void ReSet()
        {
            LogicCode = 0;
            PhyCode = 0;
            Handle = 0;
            Type = 0;

            DevReturn = null;
            Reserve = null;
        }
    }

    public struct tDevReturnArray
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 8)]
        public tDevReturn[] arrDevReturn;
        public void ReSet()
        {
            foreach (tDevReturn devReturn in arrDevReturn)
            {
                devReturn.ReSet();
            }
        }
        //C#结构体不能包含无参构造函数，并且自定义的带参数构造函数所有字段必须初始化
        public tDevReturnArray(object argObj = null)
        {
            arrDevReturn = new tDevReturn[8];
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct tFingerVenaDev_StatusInfo
    {
        // 前传感器状态,   0:无手指;  1:有手指 
        public byte ucFrontSenser;

        // 后传感器状态,   0:无手指;  1:有手指
        public byte ucBackSenser;

        // 移动传感器状态, 0:正常;    1:异常
        public byte ucMovingSenser;

        // 图像质量状态,	0:质量分数未执行或者分数低于阈值;	1:质量分数高于阈值
        public byte ucPicQualified;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserve;// 保留

    }
}
