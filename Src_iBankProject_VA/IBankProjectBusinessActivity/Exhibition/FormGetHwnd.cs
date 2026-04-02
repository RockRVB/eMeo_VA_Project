using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using LogProcessorService;

namespace VTMBusinessActivity
{
    public partial class FormGetHwnd : Form
    {

        [DllImport("User32.Dll")]
        private static extern IntPtr WindowFromPoint(Point p);
        [DllImport("User32.Dll")]
        private static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out RectStruct lpRect);
        private struct RectStruct
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        RectStruct winRect = new RectStruct();
        int WindowY = 0; int WindowX = 0;
        StringBuilder sbClassName = new StringBuilder();
        string className = "Afx:400000:0";

        public FormGetHwnd()
        {
            InitializeComponent();
        }


        public IntPtr GetHwndFromPoint(int x, int y)
        {
            this.WindowX = x; this.WindowY = y;
            IntPtr hwnd = WindowFromPoint(new Point(WindowX + 20, WindowY + 20));
            if (CheckClassName(hwnd) && CheckWindowRect(hwnd))
            {
                return hwnd;
            }
            return IntPtr.Zero;
        }

        private void LogWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                Log.Action.LogDebug(string.Format("class name:{0},handle:{1}", GetWindowsClass(hwnd), hwnd.ToString()));
        }

        private bool CheckClassName(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return false;
            GetClassName(hwnd, sbClassName, 2000);
            if (sbClassName.ToString().Equals(className, StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }

        private bool CheckWindowRect(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return false;
            GetWindowRect(hwnd, out winRect);
            if (winRect.Top == WindowY && winRect.Left == WindowX)
                return true;
            else
                return false;
        }
        private string GetWindowsClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return string.Empty;
            GetClassName(hwnd, sbClassName, 2000);
            return sbClassName.ToString();
        }

    }
}
