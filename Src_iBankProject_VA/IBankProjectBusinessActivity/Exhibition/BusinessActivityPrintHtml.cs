using System;
using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using UIServiceProtocol;
using System.IO;
using DevServiceProtocol;
using System.Collections.Generic;
using VTMBusinessActivityBase;
using System.Windows.Forms;
using System.Printing;
using DocPrinterDeviceProtocol;
using VTMModelLibrary;

namespace BankInterface
{
    [GrgActivity("{ACDC2CF0-20E1-4B87-9754-17CBC240D377}",
             NodeNameOfConfiguration = "PrintHtml",
             Name = "PrintHtml",
             Author = "")]
    public class BusinessActivityPrintHtml : BusinessActivityVTMBase
    {
        private int m_preTime = 10*1000;
        [GrgBindTarget("preTime", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int PreTime
        {
            get { return m_preTime; }

            set
            {
                m_preTime = value;
            }
        }

        private int m_oneTime = 1000;
        [GrgBindTarget("oneTime", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int OneTime
        {
            get { return m_oneTime; }

            set
            {
                m_oneTime = value;
            }
        }

        private string m_paperType = "A4";
        [GrgBindTarget("paperType", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string PaperType
        {
            get { return m_paperType; }

            set
            {
                m_paperType = value;
            }
        }

        /// <summary>
        /// 打印文件的路径
        /// </summary>
        private string m_fileName = null;
        [GrgBindTarget("fileName", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string FileName
        {
            get
            {
                return m_fileName;
            }
            set
            {
                m_fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// 是否需要盖章
        /// </summary>
        private short m_fileWithStamp = 0;
        [GrgBindTarget("fileWithStamp", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short FileWithStamp
        {
            get
            {
                return m_fileWithStamp;
            }
            set
            {
                m_fileWithStamp = value;
                OnPropertyChanged("FileWithStamp");
            }
        }

        private short m_xPos = 15;
        [GrgBindTarget("x", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short XPos
        {
            get
            {
                return m_xPos;
            }
            set
            {
                m_xPos = value;
                OnPropertyChanged("XPos");
            }
        }

        private short m_yPos = 30;
        [GrgBindTarget("y", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short YPos
        {
            get
            {
                return m_yPos;
            }
            set
            {
                m_yPos = value;
                OnPropertyChanged("YPos");
            }
        }

        /// <summary>
        /// 打印文件的路径
        /// </summary>
        private string m_stampName = "Stamp1";
        [GrgBindTarget("stampName", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string StampName
        {
            get
            {
                return m_stampName;
            }
            set
            {
                m_stampName = value;
                OnPropertyChanged("StampName");
            }
        }

        private int m_paperSource = 1;
        [GrgBindTarget("paperSource", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int Paper
        {
            get { return m_paperSource; }

            set
            {
                m_paperSource = value;
            }
        }

        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityPrintHtml() as IBusinessActivity;
        }
        #endregion

        #region constructor
        public BusinessActivityPrintHtml()
        {
        }
        #endregion

        #region feild
        //private WebBrowser webBrowser1;// = new WebBrowser();
        private bool navigated = false;
        private int printCount = 1;

        //打印份数属性，预留
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerPreRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter InnerPreRun Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerPreRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                return result;
            }
            SetPrintData();
            return emBusActivityResult_t.Success;
        }
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t result = base.InnerRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            //webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(this.webBrowser1_Navigated);
            ////在这儿加一个disposed委托，处理打印指令调用后，函数你自己加了
            //webBrowser1.Disposed += new EventHandler();
            
            //webBrowser1.Navigate(@"D:\Projects\eCAT\BCM_DEMO_II\Execute\Resource\Common\HTML\Print\CN\PrintA5Template.html");

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);
            VTMContext.NextCondition = EventDictionary.s_EventContinue;

            if (BuilderPath(ref argPath, ref tempPath))
            {
                if (VTMContext.A4PrinterService.SaveDocument(argPath, tempPath, VTMContext.FormData))
                {
                    //SetLight(GuidLight.PassbookPrinter, GuidLightFlashMode.Continuous);
                    SetLight(GuidLight.DocumentPrinter, GuidLightFlashMode.Slow);
                    //webBrowser1 = new WebBrowser();
                    //this.webBrowser1.Navigated+=new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
                    //this.webBrowser1.Navigate(tempPath);
                    //Thread.Sleep(10 * 1000);
                    //while (true)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(this.webBrowser1.DocumentText))
                    //    {
                    //        int printCount = 1;
                    //        object objPrintCount = null;
                    //        MCRDContext.TransactionDataCache.Get("proj_PrintCount", out objPrintCount, GetType());
                    //        if (objPrintCount != null)
                    //        {
                    //            printCount = Convert.ToInt32(objPrintCount.ToString());
                    //        }

                    //        for (int i = 0; i < printCount; i++)
                    //        {
                    //            webBrowser1.Document.InvokeScript("printHtml");
                    //            //webBrowser1.Print();

                    //            Log.Action.LogDebugFormat("webBrowser1.Document.InvokeScript(printHtml)  - totalPage={0}, currentPage={1}", printCount, i);
                    //        }

                    //        break;
                    //    }
                    //}

                    object objPrintCount = null;
                    VTMContext.TransactionDataCache.Get("proj_PrintCount", out objPrintCount, GetType());
                    if (objPrintCount != null)
                    {
                        printCount = Convert.ToInt32(objPrintCount.ToString());
                    }

                    //try
                    //{
                    //    MyPrinterSettings ps = new MyPrinterSettings();
                    //    if (ps.ChangePrinterSetting(new PrintDocument().PrinterSettings.PrinterName, PaperType == "A4" ? 9 : 11, PaperType == "A4" ? 1 : 2, printCount))
                    //    {
                    //        MCRDContext.A4PrinterService.PrintDocument(tempPath, printCount, PaperType);

                    //        WaitSignal(false, 1000);
                    //        MCRDContext.NextCondition = EventDictionary.s_EventContinue;
                    //    }
                    //    else
                    //    {
                    //        if (m_objContext.UIState.ContainsKey("PrintFormDataError"))
                    //        {
                    //            SwitchUIState(m_objContext.MainUI, "PrintFormDataError", 3000);
                    //            WaitSignal();
                    //        }
                    //        MCRDContext.LogJournalKey("IDS_StamperFailure", argSymbol: LogSymbol.DeviceFailure);

                    //        Log.Action.LogError("call PrintDocument failed");
                    //        MCRDContext.NextCondition = EventDictionary.s_EventFail;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.Action.LogError(ex.Message, ex);
                    //    if (m_objContext.UIState.ContainsKey("PrintFormDataError"))
                    //    {
                    //        SwitchUIState(m_objContext.MainUI, "PrintFormDataError", 3000);
                    //        WaitSignal();
                    //    }
                    //    MCRDContext.LogJournalKey("IDS_StamperFailure", argSymbol: LogSymbol.DeviceFailure);

                    //    Log.Action.LogError("call PrintDocument failed");
                    //    MCRDContext.NextCondition = EventDictionary.s_EventFail;
                    //}

                    DevResult printResult = null;
                    if (FileWithStamp == 0)
                    {
                        printResult = VTMContext.DocPrinter.PrintDocumentWithoutStamp(string.Format("{0};copies={1};pagecount={1}", tempPath, printCount), 1, (DocPrinterDeviceProtocol.PaperSource)Paper, m_devTimeout);
                    }
                    else
                    {
                        Log.Action.LogDebug(" PrintDocumentWithStamp start");
                        GrgStampInfo stapInfo = new GrgStampInfo();
                        List<GrgStampInfo> stapInfoList = new List<GrgStampInfo>();
                        stapInfo.StampName = StampName;
                        stapInfo.xPos = XPos;
                        stapInfo.yPos = YPos;
                        stapInfoList.Add(stapInfo);
                        printResult = VTMContext.DocPrinter.PrintDocumentWithStamp(string.Format("{0};copies={1};pagecount={1}", tempPath, printCount), 1, stapInfoList, (DocPrinterDeviceProtocol.PaperSource)Paper, m_devTimeout);
                    }

                    if (printResult.IsSuccess)
                    {
                        VTMContext.NextCondition = EventDictionary.s_EventContinue;
                    }
                    else
                    {
                        if (m_objContext.UIState.ContainsKey("PrintFormDataError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "PrintFormDataError", 3000);
                            WaitSignal();
                        }
                        Log.Action.LogError(" PrintDocument failed");
                        VTMContext.LogJournalKey("IDS_StamperFailure", argSymbol: LogSymbol.DeviceFailure);
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }

                        //SetLight(GuidLight.PassbookPrinter, GuidLightFlashMode.Off);
                        SetLight(GuidLight.DocumentPrinter, GuidLightFlashMode.Off);
                }
                else
                {
                    if (m_objContext.UIState.ContainsKey("PrintFormDataError"))
                    {
                        SwitchUIState(m_objContext.MainUI, "PrintFormDataError", 3000);
                        WaitSignal();
                    }
                    VTMContext.LogJournalKey("IDS_StamperFailure", argSymbol: LogSymbol.DeviceFailure);
                    Log.Action.LogError(" savePrint failed");
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                }
            }
            else
            {
                if (m_objContext.UIState.ContainsKey("PrintFormDataError"))
                {
                    SwitchUIState(m_objContext.MainUI, "PrintFormDataError", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_StamperFailure", argSymbol: LogSymbol.DeviceFailure);

                Log.Action.LogError("call PrintDocument failed");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }

            SetLight(GuidLight.DocumentPrinter, GuidLightFlashMode.Off);
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, argUIEvent);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }

            if (argUIEvent.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (argUIEvent.Key != null)
                {
                    if (argUIEvent.Key is string)
                    {
                        string key = (string)argUIEvent.Key;
                        if (key.Equals("OnCancel", StringComparison.OrdinalIgnoreCase))
                        {
                            VTMContext.NextCondition = EventDictionary.s_EventCancel;
                            SignalCancel();
                            return emBusiCallbackResult_t.Swallowd;
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region methods
        public override void OnTimeoutUnderWait(int nInterval)
        {
            base.OnTimeoutUnderWait(nInterval);

            //PrintQueue printQueue = GetPrintQueue(new PrintDocument().PrinterSettings.PrinterName);
            //Log.Action.LogDebugFormat("PrintQueue.IsProcessing-{0}", printQueue.IsProcessing);
            if (Count * 1000 > PreTime && Count * 1000 > printCount * OneTime + PreTime)
            {
                SignalCancel();


                //PrintQueue printQueue = GetPrintQueue(new PrintDocument().PrinterSettings.PrinterName);
                //if (printQueue != null && printQueue.NumberOfJobs == 0 && !printQueue.IsBusy)
                //{
                //    SignalCancel();
                //}
            }
        }

        protected override void InnerTerminate(bool argIsUserCancel)
        {
            SetLight(GuidLight.DocumentPrinter, GuidLightFlashMode.Off);
            base.InnerTerminate(argIsUserCancel);
        }

        private string tempPath = string.Empty;
        private string argPath = string.Empty;

        private string basePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, @"Resource\Common\HTML\Print\");
        protected virtual bool BuilderPath(ref string path, ref string tempPath)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return false;
            }
            else
            {
                path = string.Concat(basePath, m_objContext.CurrentRPTRLanguage, "\\", FileName);
                if (File.Exists(path))
                {
                    tempPath = string.Concat(basePath, m_objContext.CurrentRPTRLanguage, "\\", "temp", FileName);
                    //tempPath = string.Concat(basePath, m_objContext.CurrentRPTRLanguage, "\\", System.Guid.NewGuid().ToString(), FileName);
                    return true;
                }
                else
                {
                    Log.Action.LogErrorFormat("file[{0}] is not exists", path);
                    return false;
                }
            }
        }

        private PrintQueue GetPrintQueue(string PrinterName)
        {
            LocalPrintServer pr = new LocalPrintServer();
            pr.Refresh();
            EnumeratedPrintQueueTypes[] enumerationFlags = {EnumeratedPrintQueueTypes.Local,
                                                            EnumeratedPrintQueueTypes.Connections,
                                                           };
            foreach (PrintQueue pq in pr.GetPrintQueues(enumerationFlags))
            {
                if (pq.Name == PrinterName)
                {
                    return pq;
                }
            }

            return null;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {


            navigated = true;
            //webBrowser1.Dispose();
        }

        public void SetPrintData()
        {
            try
            {
                object obj;
                m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_CARDFORM, out obj, GetType());
                var creadCardInputInfo = obj as CreadCardInputInfo;
                if (null != creadCardInputInfo)
                {
                    //1、创建及初始化
                    Dictionary<string, string> temDictionary = new Dictionary<string, string>();
                    //2、添加元素
                    temDictionary.Add("txtMobilePhone", creadCardInputInfo.PhoneNo);
                    temDictionary.Add("txtName", creadCardInputInfo.IdCard_Name);
                    temDictionary.Add("txtSex", creadCardInputInfo.IdCard_Sex);
                    temDictionary.Add("txtNationality", creadCardInputInfo.IdCard_Nation);
                    temDictionary.Add("txtbirthday", creadCardInputInfo.IdCard_Birthday);
                    temDictionary.Add("txtIDCardNumber", creadCardInputInfo.IdCard_IDNo);
                    temDictionary.Add("txtEmailAddress", creadCardInputInfo.EmailAddress);
                    temDictionary.Add("txtContactAddress", creadCardInputInfo.CustomAddress);
                    VTMContext.FormData = temDictionary;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("SetPrintData is fail", ex);
            }
        }
        #endregion
    }
}
