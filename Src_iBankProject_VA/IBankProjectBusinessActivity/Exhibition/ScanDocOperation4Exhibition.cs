using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using IDCardReaderProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using VTMBusinessServiceProtocol;
using UIServiceProtocol;
using System.Threading;

using ResourceManagerProtocol;
using System.Collections.ObjectModel;
using RemoteTellerServiceProtocol;
using VTMBusinessActivity.devcommon;
using VTMBusinessActivity.common;
using VTMModelLibrary;
using CameraDeviceProtocol;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace VTMBusinessActivity
{
    [GrgActivity("{083C02B1-A4F6-413E-AAEE-2AEA4F3C57B8}",
                  Name = "ScanDocOperation4Exhibition",
                  NodeNameOfConfiguration = "ScanDocOperation4Exhibition",
                  Author = "ltfei1")]
    public class ScanDocOperation4Exhibition : BusinessActivityVTMBase
    {

        #region property
        private CallingMethod callMethod;
        private ScanDocOperation4Exhibition sonObj;
        private short m_CallingMethodType = 0;
        //第一次拍照时才展示拍照页面，当连续拍照时，不用重新刷屏，否则会导致页面倒计时重新计时
        private int intScanCount = 0;
        [GrgBindTarget("CallingMethodType", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short CallingMethodType
        {
            get
            {
                return m_CallingMethodType;
            }
            set
            {
                m_CallingMethodType = value;
                OnPropertyChanged("CallingMethodType");
            }
        }
        private bool m_IsUnEnable = false;
        public bool IsUnEnable
        {
            get
            {
                return m_IsUnEnable;
            }
            set
            {
                m_IsUnEnable = value;
                OnPropertyChanged("IsUnEnable");
            }

        }
        private bool m_ColorValue = false;
        public bool ColorValue
        {
            get
            {
                return m_ColorValue;
            }
            set
            {
                m_ColorValue = value;
                OnPropertyChanged("ColorValue");
            }

        }
        private ObservableCollection<ScanDocImage> m_ScanImageList = null;
        public ObservableCollection<ScanDocImage> ScanImageList
        {
            get
            {
                if (null == m_ScanImageList) m_ScanImageList = new ObservableCollection<ScanDocImage>();
                return m_ScanImageList;
            }
            set
            {
                m_ScanImageList = value;
                //Log.Action.LogDebug("setting property scanlist count:" + m_ScanImageList.Count);
                IsSignImageindex = m_ScanImageList.Count;
                if (m_ScanImageList.Count < DefaulNum)
                {
                    CanScan = true;
                    CanScanColor = true;
                }
                OnPropertyChanged("ScanImageList");
            }
        }
        private int m_IsSignImageindex = 0;//已经扫描的图片索引
        private const string m_SignalConfirm = "Confirm";
        private const string m_SingalScan = "Scan";
        private const string m_SingalCancel = "Cancel";
        public int IsSignImageindex
        {
            get { return m_IsSignImageindex; }
            set
            {
                //Log.Action.LogDebug("SignImageIndex is:" + m_IsSignImageindex);
                m_IsSignImageindex = value;
                if (m_IsSignImageindex < DefaulNum)
                {
                    CanScan = true;
                    CanScanColor = true;
                }
                //if (m_IsSignImageindex <= 0)
                //{
                //    ScanImageList.Clear();
                //    InitImagePath();
                //}
                //if (m_IsSignImageindex <= 0)
                //{
                //    IsUnEnable = false;
                //    ColorValue = false;
                //    CanScan = true;
                //    CanScanColor = true;
                //}
                //if (m_IsSignImageindex <= 0)
                //{
                //    ScanImageList.Clear(); 
                //    InitImagePath();                    
                //}
                OnPropertyChanged("IsSignImageindex");
            }
        }
        private bool _canScan = true;
        public bool CanScan
        {
            get { return _canScan; }
            set
            {
                _canScan = value;

                OnPropertyChanged("CanScan");
            }
        }
        private bool _canScanColor = true;
        public bool CanScanColor
        {
            get { return _canScanColor; }
            set
            {
                _canScanColor = value;

                OnPropertyChanged("CanScanColor");
            }
        }

        private int m_DefaulNum = 6;
        [GrgBindTarget("defaulnum", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int DefaulNum
        {
            get
            {
                return m_DefaulNum;
            }
            set
            {
                m_DefaulNum = value;
                OnPropertyChanged("DefaulNum");
            }
        }
        private int m_CountNum = 0;
        [GrgBindTarget("CountNum", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int CountNum
        {
            get
            {
                return m_CountNum;
            }
            set
            {
                m_CountNum = value;
                OnPropertyChanged("CountNum");
            }
        }
        private short m_WindowX = 0;
        [GrgBindTarget("windowx", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowX
        {
            get
            {
                return m_WindowX;
            }
            set
            {
                m_WindowX = value;
                OnPropertyChanged("WindowX");
            }
        }
        private short m_WindowY = 0;
        [GrgBindTarget("windowy", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowY
        {
            get
            {
                return m_WindowY;
            }
            set
            {
                m_WindowY = value;
                OnPropertyChanged("WindowY");
            }
        }
        private short m_WindowHeight = 0;
        [GrgBindTarget("windowheight", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowHeight
        {
            get
            {
                return m_WindowHeight;
            }
            set
            {
                m_WindowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }
        private short m_WindowWidth = 0;
        [GrgBindTarget("windowwidth", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowWidth
        {
            get
            {
                return m_WindowWidth;
            }
            set
            {
                m_WindowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }

        // for crop id card
        private short m_CW = 980;
        [GrgBindTarget("CW", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short CW
        {
            get
            {
                return m_CW;
            }
            set
            {
                m_CW = value;
                OnPropertyChanged("CW");
            }
        }
        private short m_CH = 770;
        [GrgBindTarget("CH", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short CH
        {
            get
            {
                return m_CH;
            }
            set
            {
                m_CH = value;
                OnPropertyChanged("CH");
            }
        }
        private short m_CX = 55;
        [GrgBindTarget("CX", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short CX
        {
            get
            {
                return m_CX;
            }
            set
            {
                m_CX = value;
                OnPropertyChanged("CX");
            }
        }
        private short m_CY = 618;
        [GrgBindTarget("CY", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short CY
        {
            get
            {
                return m_CY;
            }
            set
            {
                m_CY = value;
                OnPropertyChanged("CY");
            }
        }


        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new ScanDocOperation4Exhibition() as IBusinessActivity;
        }
        #endregion

        #region methods
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        IntPtr HWND_BOTTOM = new IntPtr(1);
        IntPtr HWND_TOPMOST = new IntPtr(-1);
        IntPtr HWND_TOP = new IntPtr(0);  //set to button
        IntPtr HWND_NOTOPMOST = new IntPtr(-2);//set to topmost
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_HIDEWINDOW = 0x0080;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOACTIVATE = 0x0010;
        IntPtr handle = IntPtr.Zero;
        System.Timers.Timer getWindTimer = null;
        FormGetHwnd gethwndForm = new FormGetHwnd();
        private void StartGetPreviewWindow()
        {
            getWindTimer = new System.Timers.Timer();
            getWindTimer.Interval = 1000;
            getWindTimer.Elapsed += new System.Timers.ElapsedEventHandler(GetPreviewWindow);
            getWindTimer.Start();
        }

        private void GetPreviewWindow(object sender, EventArgs e)
        {
            if (handle != IntPtr.Zero)
            {
                getWindTimer.Stop();
                getWindTimer.Dispose();
                getWindTimer = null;
                gethwndForm.Dispose();
            }
            else
            {
                IntPtr hwnd = gethwndForm.GetHwndFromPoint(WindowX, WindowY);
                if (hwnd != IntPtr.Zero)
                {
                    handle = hwnd;
                    Log.Action.LogDebug("Geted scanner handle:" + handle.ToString());
                }
                else
                    Log.Action.LogDebug("Can't not get scanner handle");
            }

        }

        public void SetPreviewWinTop()
        {
            if (handle != IntPtr.Zero)
            {
                //SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
                SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        public void SetPreviewWinBotton()
        {
            if (handle != IntPtr.Zero)
            {
                //SetWindowPos(handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_HIDEWINDOW | SWP_NOMOVE | SWP_NOSIZE);
                SetWindowPos(handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            sonObj = (ScanDocOperation4Exhibition)m_objContext.CurrentRunningAction;
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            VTMContext.TransactionDataCache.Set("ScanApplicationFormPath", "", GetType());

            if (null != VTMContext.CameraService)
            {
                SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
                CanScan = true;
                CanScanColor = true;
                InitImagePath();

                callMethod = (CallingMethod)CallingMethodType;
                if (callMethod == CallingMethod.Immediately)
                {
                    if (OpenVideoMethod())
                    {
                        //当未拍照时，提交按钮为灰色
                        IsUnEnable = false;
                        ColorValue = false;
                        string strCurSignalName;
                        string strWaitSignalNames = string.Format("{0},{1},{2},{3},{4}", Signal.Success, m_SignalConfirm, m_SingalScan, Signal.Failure, m_SingalCancel);
                        AddSignal(strWaitSignalNames);

                        emWaitSignalResult_t emWaitResult;
                        if (WaitPopu == 1)
                        {
                            emWaitResult = VTMWaitSignal(strWaitSignalNames, out strCurSignalName, false);

                        }
                        else
                        {

                            emWaitResult = WaitSignal(strWaitSignalNames, out strCurSignalName, false);
                        }
                        if (emWaitResult == emWaitSignalResult_t.Timeout)
                        {
                            VTMContext.LogJournalKey("IDS_ScanDocTimeOut", argSymbol: LogSymbol.Alert);

                            VTMContext.NextCondition = EventDictionary.s_EventTimeout;

                        }
                        else if (emWaitResult == emWaitSignalResult_t.Success)
                        {
                            if (strCurSignalName.Equals(m_SignalConfirm, StringComparison.OrdinalIgnoreCase))
                            {
                                VTMContext.NextCondition = EventDictionary.s_EventContinue;

                            }
                            else if (strCurSignalName.Equals(Signal.Success))
                            {
                                //SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
                                string path = BuildPath();

                                DevResult devResult = VTMContext.CameraService.TakePictureEx(CameraName.HIGHCAMERA, string.Empty, string.Empty, path);

                                //if is not id card , tunover 
                                //if (IsIDCard != 1)
                                {
                                    Log.Action.LogDebug("not id card , we need to tunover it");
                                    FileInfo file = new FileInfo(path);
                                    if (GrgOverturn(path))
                                    {
                                        path = Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(path) + "_TurnOver.jpg");
                                        //Log.Action.LogDebug("turn over ok, set to new path:" + path);
                                        VTMContext.TransactionDataCache.Set("ScanApplicationFormPath", path, GetType());
                                        VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_CustomSignPath, path, GetType());
                                        m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                                    }
                                }

                                if (devResult.IsFailure)
                                {
                                    if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                    {
                                        SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                        WaitSignal();
                                    }
                                    VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                    VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;

                                }
                                else
                                {
                                    //增加图片列表
                                    SaveDocInfo(path);
                                    IsUnEnable = true;
                                    ColorValue = true;
                                    //CanScan = false;
                                    //CanScanColor = false;
                                    m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_SCANIMG_MUCH, ScanImageList, GetType());
                                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                }
                            }
                            else if (strCurSignalName.Equals(Signal.Cancel))
                            {
                                VTMContext.NextCondition = EventDictionary.s_EventCancel;
                            }
                            else
                            {
                                if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                    WaitSignal();
                                }
                                VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                            }
                        }
                        else
                        {
                            if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                WaitSignal();
                            }
                            VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                            VTMContext.ActionResult = emBusActivityResult_t.Failure;
                            VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                        }

                    }
                    else
                    {
                        if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                            WaitSignal();
                        }
                        VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        VTMContext.ActionResult = emBusActivityResult_t.Failure;
                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    }
                }
                else
                {
                    //初始化设备 call methor is 0, the defalult here
                    if (OpenVideoMethod())
                    {
                        //当未拍照时，提交按钮为灰色
                        IsUnEnable = false;
                        ColorValue = false;
                        intScanCount = 0;
                        while (true)
                        {
                            string strCurSignalName;
                            string strWaitSignalNames = string.Format("{0},{1},{2},{3},{4}", Signal.Success, m_SignalConfirm, m_SingalScan, Signal.Failure, m_SingalCancel);
                            AddSignal(strWaitSignalNames);
                            emWaitSignalResult_t emWaitResult;
                            if (WaitPopu == 1)
                            {
                                emWaitResult = VTMWaitSignal(strWaitSignalNames, out strCurSignalName, false);
                            }
                            else
                            {
                                emWaitResult = WaitSignal(strWaitSignalNames, out strCurSignalName, false);
                            }
                            if (emWaitResult == emWaitSignalResult_t.Timeout)
                            {
                                VTMContext.LogJournalKey("IDS_ScanDocTimeOut", argSymbol: LogSymbol.Alert);

                                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                                break;
                            }
                            else if (emWaitResult == emWaitSignalResult_t.Success)
                            {
                                if (strCurSignalName.Equals(m_SignalConfirm, StringComparison.OrdinalIgnoreCase))
                                {
                                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                    break;
                                }
                                else if (strCurSignalName.Equals(Signal.Success))
                                {
                                    continue;
                                }
                                else if (strCurSignalName.Equals(Signal.Failure, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                    {
                                        SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                        WaitSignal();
                                    }
                                    VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                    VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                    break;

                                }
                                else if (strCurSignalName.Equals(Signal.Cancel))
                                {
                                    VTMContext.NextCondition = EventDictionary.s_EventCancel;
                                    break;
                                }
                                else if (strCurSignalName.Equals(m_SingalScan, StringComparison.OrdinalIgnoreCase))
                                {
                                    Log.Action.LogDebug("start takePic");
                                    string path = BuildPath();
                                    DevResult devResult = VTMContext.CameraService.TakePictureEx(CameraName.HIGHCAMERA, string.Empty, string.Empty, path);
                                    // if (IsIDCard != 1)//if is not idcard scan, nneed to turn over
                                    {
                                        Log.Action.LogDebug("not id card , we need to tunover it");
                                        FileInfo file = new FileInfo(path);
                                        if (GrgOverturn(path))
                                        {
                                            path = Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(path) + "_TurnOver.jpg");
                                            //Log.Action.LogDebug("turn over ok, set to new path:" + path);
                                            VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_CustomSignPath, path, GetType());
                                        }
                                    }


                                    if (devResult.IsFailure)
                                    {
                                        if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                        {
                                            SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                            WaitSignal();
                                        }
                                        VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                        VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                        break;
                                    }
                                    else
                                    {
                                        //增加图片列表
                                        SaveDocInfo(path);

                                        //当拍照成功后提交按钮为可用
                                        IsUnEnable = true;
                                        ColorValue = true;

                                        //CanScan = false;
                                        //CanScanColor = false;
                                        //调用还原
                                        Log.Action.LogDebug("start ResumeTakePicture");

                                        GrgCmdInArg objInArg = new GrgCmdInArg()
                                        {
                                            Timeout = m_devTimeout,
                                            UserState = Thread.CurrentThread.ManagedThreadId
                                        };

                                        emDevCmdResult emResult = VTMContext.CameraService.HostedDevice.ExecuteCommandAsyn(CameraDeviceProtocol.CameraCmds.s_ResumeTakePictureCmd, objInArg);
                                        if (emResult != emDevCmdResult.Pending && emResult != emDevCmdResult.Success)
                                        {
                                            if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                            {
                                                SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                                WaitSignal();
                                            }
                                            VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                            Log.Action.LogError("SetWindowPos failure");
                                            VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                            VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                            VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                            break;

                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {

                                    break;
                                }

                            }
                            else
                            {
                                if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                                    WaitSignal();
                                }
                                VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                break;
                            }
                        }

                    }
                    intScanCount = 0;
                }
                CloseScanner();
            }
            else
            {
                if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                {
                    SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_CAMNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;

            }


            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;

        }

        private void InitImagePath()
        {
            ObservableCollection<VTMModelLibrary.ScanDocImage> scanLists = new ObservableCollection<VTMModelLibrary.ScanDocImage>();
            for (int i = 0; i < CountNum; i++)
            {
                scanLists.Add(new ScanDocImage() { Index = i.ToString(), ImageParh = "", ImagebgVisibility = Visibility.Visible, CloseVisibility = Visibility.Collapsed });
            }
            ScanImageList = scanLists;
        }
        private bool OpenVideoMethod()
        {
            //SetLight(GuidLight.DocScanner, GuidLightFlashMode.Continuous);
            SetLight(GuidLight.EnvDepository, GuidLightFlashMode.Continuous);
            SetLight(GuidLight.PassbookPrinter, GuidLightFlashMode.Continuous);

            GrgStartTakePictureCmdInArgs objInArg = new GrgStartTakePictureCmdInArgs()
            {
                CameraNamePara = CameraName.HIGHCAMERA,
                Wigth = (ushort)WindowWidth,
                Height = (ushort)WindowHeight,
                hWnd = VTMContext.MainUI.HostService.HandleOfMainWindow,
                // hWnd = IntPtr.Zero,
                UserState = Thread.CurrentThread.ManagedThreadId,
                X = (ushort)WindowX,
                Y = (ushort)WindowY
            };

            // var devResult = VTMContext.CameraService.StartTakePicture(CameraName.HIGHCAMERA, (ushort)WindowWidth, (ushort)WindowHeight, (ushort)WindowX, (ushort)WindowY, IntPtr.Zero);

            emDevCmdResult emResult = VTMContext.CameraService.HostedDevice.ExecuteCommandAsyn(CameraDeviceProtocol.CameraCmds.s_StartTakePictureCmd, objInArg);

            if (emResult != emDevCmdResult.Pending && emResult != emDevCmdResult.Success)
            {
                if (m_objContext.UIState.ContainsKey("OpenScanDocError"))
                {
                    SwitchUIState(m_objContext.MainUI, "OpenScanDocError", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                Log.Action.LogError("SetWindowPos failure");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                return false;
            }
            else
            {
                StartGetPreviewWindow();
                return true;
            }


        }
        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService iDev, DeviceEventArg objArg)
        {
            if (null != VTMContext.CameraService && objArg.Source == VTMContext.CameraService.HostedDevice)
            {

                if (objArg.CommandCompleted)
                {
                    if (objArg.ResultOfCommandCompleted != null)
                    {

                        if (objArg.ResultOfCommandCompleted.IsSuccess)
                        {
                            GrgCmdOutArg outObj = (GrgCmdOutArg)objArg.ResultOfCommandCompleted;
                            if (outObj.CommandString.Equals(CameraDeviceProtocol.CameraCmds.s_StartTakePictureCmd))
                            {
                                SendSignal(Signal.Success);
                            }
                            else if (outObj.CommandString.Equals(CameraDeviceProtocol.CameraCmds.s_ResumeTakePictureCmd))
                            {
                                SendSignal(Signal.Success);
                            }

                        }
                        else if (objArg.ResultOfCommandCompleted.IsFailure)
                        {
                            SendSignal(Signal.Failure);
                        }
                        else if (objArg.ResultOfCommandCompleted.Cancelled)
                        {
                            SendSignal(Signal.Cancel);
                        }
                    }
                }
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnDevEvtHandle(iDev, objArg);
        }
        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {

            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }
            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    Log.Action.LogDebug("Key is " + key);
                    if (key.Equals(DataCacheKey.OnSnapDoc, StringComparison.OrdinalIgnoreCase))
                    {
                        //Log.Action.LogDebug("CanScan is " + CanScan);
                        if (CanScan)
                        {
                            SendSignal(m_SingalScan);
                        }
                    }
                    else if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                    {
                        PrintSrcPath();
                        if (DefaulNum > 0)
                        {
                            if (ScanImageList.Count == DefaulNum)
                            {
                                if (!string.IsNullOrEmpty(ScanImageList[0].ImageParh))
                                {
                                    m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_SCANIMG_MUCH, ScanImageList, GetType());
                                    //SetPathToIDCard();
                                    SendSignal(m_SignalConfirm);
                                }
                            }
                            //else
                                //Log.Action.LogDebug("when confirm press the scanimagelist count is:" + ScanImageList.Count);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ScanImageList[0].ImageParh))
                            {
                                m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_SCANIMG_MUCH, ScanImageList, GetType());
                                SendSignal(m_SignalConfirm);
                            }
                        }
                    }
                    else if (key.Equals(EventDictionary.s_EventCancel, StringComparison.OrdinalIgnoreCase))
                    {
                        SendSignal(m_SingalCancel);
                    }
                    else if (key.Equals(EventDictionary.s_EventClear, StringComparison.OrdinalIgnoreCase))
                    {
                        Clear();
                        if (IsUnEnable)
                        {
                            IsUnEnable = false;
                            ColorValue = false;
                        }
                        CanScan = true;
                        CanScanColor = true;
                    }
                    else if (key.StartsWith("OnF", StringComparison.OrdinalIgnoreCase))//handle new del event from html
                    {
                        DelImage(key);
                    }
                    else if (key.Equals("OnZoomOpen", StringComparison.OrdinalIgnoreCase))
                    {
                        SetPreviewWinBotton();
                    }
                    else if (key.Equals("OnZoomClose", StringComparison.OrdinalIgnoreCase))
                    {
                        SetPreviewWinTop();
                    }
                    //else
                    //{
                    //    m_objContext.NextCondition = key;
                    //    SignalCancel();
                    //}
                }
            }
            return emBusiCallbackResult_t.Swallowd;

        }
        private void Clear()
        {
            IsSignImageindex = 0;
            InitImagePath();

            SetPathForHtml();
        }
        private void PrintSrcPath()
        {
            //foreach (ScanDocImage sc in ScanImageList)
            //{
            //    Log.Action.LogDebug("scan src:" + sc.ImageParh);
            //}
        }

        public static bool GrgOverturn(string imgPath)
        {
            try
            {
                using (Bitmap bitMap = new Bitmap(imgPath))
                {
                    //Log.Action.LogDebug("destFile: " + imgPath);
                    KiRotate90(bitMap);
                    string savePath = imgPath.Substring(0, imgPath.Length - 4) + "_TurnOver.jpg";
                    bitMap.Save(savePath, ImageFormat.Jpeg);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Action.LogError("KiRotateImg error: " + ex);
                return false;

            }
        }

        public static Bitmap KiRotate90(Bitmap img)
        {
            try
            {
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                return img;
            }
            catch
            {
                return null;
            }
        }

        private void DelImage(string strIndex)
        {
            if (strIndex.Length < 4)
                return;
            int index = 0;
            if (int.TryParse(strIndex.Remove(0, 3), out index))
            {
                if (ScanImageList.Count >= index)
                {
                    ScanImageList.RemoveAt(index - 1);
                    SetPathForHtml();
                }
            }
            if (ScanImageList.Count < DefaulNum)
                CanScan = true;
            else
                CanScan = false;
        }

        private void SetPathForHtml()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ScanDocImage image in ScanImageList)
            {
                sb.Append(image.ImageParh + ",");
            }
            //Log.Action.LogDebug("imgList= " + sb.ToString());
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            m_objContext.MainUI.ExecuteScriptCommand("setImage", sb.ToString());
        }

        private void SaveDocInfo(string path)
        {
            //Log.Action.LogDebug("save doc info:" + path);
            //and all image to the list  first.
            ObservableCollection<VTMModelLibrary.ScanDocImage> scanLists = new ObservableCollection<VTMModelLibrary.ScanDocImage>();
            foreach (ScanDocImage sc in ScanImageList)
            {
                ScanDocImage scin = new VTMModelLibrary.ScanDocImage() { CloseVisibility = sc.CloseVisibility, ImagebgVisibility = sc.ImagebgVisibility, ImageParh = sc.ImageParh, Index = sc.Index };
                scanLists.Add(scin);
            }

            if (scanLists.Count < DefaulNum)
            {
                Log.Action.LogDebug("Into add scanlist");
                ScanDocImage docImage = new ScanDocImage();
                docImage.Index = (scanLists.Count() + 1).ToString();
                docImage.ImageParh = path;
                docImage.CloseVisibility = Visibility.Visible;
                scanLists.Add(docImage);
            }

            int IsSignImageindextemp = 0;
            foreach (ScanDocImage sc in scanLists)
            {
                if (!string.IsNullOrEmpty(sc.ImageParh))
                {
                    IsSignImageindextemp = IsSignImageindextemp + 1;
                }
            }
            IsSignImageindex = IsSignImageindextemp;
            ScanImageList = scanLists;
            if (ScanImageList.Count >= DefaulNum)
            {
                CanScan = false;
                CanScanColor = false;
            }
            else
            {
                CanScan = true;
                CanScanColor = true;
            }
            SetPathForHtml();
            //Log.Action.LogDebug("ScanImageList count:" + ScanImageList.Count);
        }
        public string BuildPath()
        {
            return string.Format("{0}{1}\\{2}.jpg", AppDomain.CurrentDomain.BaseDirectory, "Temp", Guid.NewGuid().ToString());
        }
        private void CloseScanner()
        {
            try
            {
                //SetLight(GuidLight.DocScanner, GuidLightFlashMode.Off);
                SetLight(GuidLight.EnvDepository, GuidLightFlashMode.Off);
                SetLight(GuidLight.PassbookPrinter, GuidLightFlashMode.Off);
                var devResult = VTMContext.CameraService.Reset();
                if (devResult.IsFailure)
                {
                    Log.Action.LogError("CloseVideo failure");
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    VTMContext.ActionResult = emBusActivityResult_t.Failure;
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                }
                if (getWindTimer != null)
                {
                    getWindTimer.Stop();
                    getWindTimer.Dispose();
                    getWindTimer = null;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("CloseVideo failure");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                Log.BusinessService.LogError(ex.Message, ex);
            }
        }
        protected override void InnerTerminate(bool argIsUserCancel)
        {
            CloseScanner();
            base.InnerTerminate(argIsUserCancel);
        }
        #endregion

    }
}
