using Attribute4ECAT;
using BusinessServiceProtocol;
using CardReaderDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivity;
using IDCardReaderProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Threading;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMModelLibrary;

namespace VTMBusinessActivity
{
    [GrgActivity("{3A69D06A-B9B7-4E34-9BAF-6E2DD9AC4FDD}",
                  Name = "ScanIDCard4Exhibition",
                  NodeNameOfConfiguration = "ScanIDCard4Exhibition",
                  Author = "ltfei1")]
    public class ScanIDCard4Exhibition : BusinessActivityVTMBase
    {
        #region attribute
        private ScanIDCardInfo currentIDCard = new ScanIDCardInfo(); //身份证信息
        public ScanIDCardInfo VTM_IDCARD
        {
            get { return currentIDCard; }
            set
            {
                currentIDCard = value;
                OnPropertyChanged("VTM_IDCARD");
            }
        }
        private short m_CallingMethodType = 0; //是否第一次进入便触发身份证读卡
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

        private CallingMethod callMethod; //0 Immediately = 1 进入便触发,  WaitConfirm = 0 等待按钮触发调用


        string strCurSignalName;
        private const string SignalConfirm = "SignalConfirm";
        private const string SignalInputID = "SignalInputID";
        emWaitSignalResult_t emWaitResult;
        string strSignalNames = string.Format("{0},{1},{2},{3},{4}", Signal.Cancel, Signal.Failure, Signal.Success, SignalConfirm, SignalInputID);
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new ScanIDCard4Exhibition();
        }
        #endregion
        #region methods
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            if (null != VTMContext.IDReader)
            {
                //callMethod = (CallingMethod)CallingMethodType;
                //添加信号量
                AddSignal(strSignalNames);

                //获取读卡器中媒介状态
                GrgCmdIDCStatusInfo grgCmdIDCStatusInfo;
                VTMContext.IDReader.GetStatusInfo(out grgCmdIDCStatusInfo);
                if (grgCmdIDCStatusInfo != null)
                {
                    //身份证在读卡器内,则吞卡回收
                    //Log.Action.LogDebug("IDCard MediaState：" + grgCmdIDCStatusInfo.MediaState);
                    if (grgCmdIDCStatusInfo.MediaState == MediaState.Present)
                    {
                        VTMContext.IDReader.RetainCard();
                    }
                    else if (grgCmdIDCStatusInfo.DeviceState == HardwareState.HWError)
                    {
                        //读卡器硬件故障
                        Log.Action.LogDebug("IDCardReader DeviceState：" + grgCmdIDCStatusInfo.DeviceState);
                        if (m_objContext.UIState.ContainsKey("IDCardError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "IDCardError", 3000);
                            WaitSignal();
                        }
                        VTMContext.LogJournalKey("IDS_IDCardNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                        return emBusActivityResult_t.Success;
                    }
                }


                //界面提示插入身份证
                SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
                GrgIDCardScanCmdInArg objInArg = new GrgIDCardScanCmdInArg()
                {
                    UserState = Thread.CurrentThread.ManagedThreadId,
                    PhotoPath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp"),
                    PhotoName = Guid.NewGuid().ToString()
                };

                //读取身份证
                Log.Action.LogDebug("Read IDCard Begin");
                emDevCmdResult emResult = VTMContext.IDReader.HostedDevice.ExecuteCommandAsyn(CardReaderDeviceProtocol.CARDREADERDEVICECMD.s_ReadRawDataCmd, objInArg);
                Log.Action.LogDebug("Read IDCard End");

                if (emResult != emDevCmdResult.Pending && emResult != emDevCmdResult.Success)
                {
                    VTMContext.LogJournalKey("IDS_ScanIDCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    VTMContext.IDCardInfo = null;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    return emBusActivityResult_t.Success;

                }
                SetLight(GuidLight.IDCard, GuidLightFlashMode.Slow);

                bool IsReadCardSuccess = false;
                string StateUI = string.Empty;

                //等待完成信号
                emWaitResult = WaitPopu == 1 ? VTMWaitSignal(strSignalNames, out strCurSignalName, false) : WaitSignal(strSignalNames, out strCurSignalName, false);
                if (emWaitResult == emWaitSignalResult_t.Timeout)
                {
                    VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                    VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                    VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                    Cancel();
                    return emBusActivityResult_t.Success;
                }
                else if (strCurSignalName.Equals(Signal.Success, StringComparison.OrdinalIgnoreCase))
                {
                    //身份证读取成功
                    IsReadCardSuccess = true;
                    StateUI = "TakeIDCard";
                    Log.Action.LogDebug("read id card success");
                }
                else if (strCurSignalName.Equals(Signal.Cancel, StringComparison.OrdinalIgnoreCase))
                {
                    //取消身份证读取
                    Log.Action.LogDebug("read id card cancel");
                    IsReadCardSuccess = false;
                    VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                    VTMContext.NextCondition = EventDictionary.s_EventCancel;

                }
                else if (strCurSignalName.Equals(SignalInputID, StringComparison.OrdinalIgnoreCase))
                {
                    //取消身份证读取
                    Log.Action.LogDebug("Ready to input ID number.");
                    VTMContext.NextCondition = "OnInput";
                    Cancel();
                    return emBusActivityResult_t.Success;
                }
                else
                {
                    //身份证读取失败
                    IsReadCardSuccess = false;
                    StateUI = "TakeIDCardFail";
                    Log.Action.LogDebug("read id card fail");
                }

                //界面提示请拿走身份证
                if (!strCurSignalName.Equals(Signal.Cancel, StringComparison.OrdinalIgnoreCase))
                {
                    SwitchUIState(m_objContext.MainUI, StateUI, 30000);
                }


                //退出身份证
                //先取读卡器状态
                VTMContext.IDReader.GetStatusInfo(out grgCmdIDCStatusInfo);
                if (grgCmdIDCStatusInfo != null)
                {
                    Log.Action.LogInfoFormat("IDCard Media State is {0}", grgCmdIDCStatusInfo.MediaState);
                    //读卡器中是否有身份证存在，有则退卡，否则直接结束流程
                    if (grgCmdIDCStatusInfo.MediaState == MediaState.Present || grgCmdIDCStatusInfo.MediaState == MediaState.Entering)
                    {
                        DevResult emDevResult = VTMContext.IDReader.EjectCard();
                        if (emDevResult.IsFailure)
                        {
                            VTMContext.IDReader.RetainCard();
                            //退卡失败，将结束流程
                            SwitchUIState(m_objContext.MainUI, "EjectCardFail",3000);
                            WaitSignal();
                            VTMContext.NextCondition = EventDictionary.s_EventCancel;
                            Cancel();
                            return emBusActivityResult_t.Success;
                        }
                        SetLight(GuidLight.IDCard, GuidLightFlashMode.Off);
                    }
                    else
                    {
                        //读卡器中无身份证，或用户插入身份证一半，强行拔出，将结束流程
                        SwitchUIState(m_objContext.MainUI, "TransProcessing",3000);//add by lmjun2 取消之后，跳转到正在处理中界面。
                        WaitSignal();
                        VTMContext.NextCondition = EventDictionary.s_EventCancel;
                        Cancel();
                        return emBusActivityResult_t.Success;
                    }
                }

                //等待身份证被拿走的信号
                //emWaitResult = WaitPopu == 1 ? VTMWaitSignal(strSignalNames, out strCurSignalName, false) : WaitSignal(strSignalNames, out strCurSignalName, false);
                emWaitResult = WaitSignal(strSignalNames, out strCurSignalName, false);
                //超时未拿走身份证，则吞卡
                if (emWaitResult == emWaitSignalResult_t.Timeout)
                {
                    VTMContext.IDReader.RetainCard();
                    VTMContext.LogJournalKey("IDS_ScanIDCardTimeOut", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                    VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                    VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                }
                //成功拿走身份证，并判断是否需要校验身份证的有效期
                else if (strCurSignalName.Equals(Signal.Success, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Action.LogDebug("takeaway id card success");
                    if (IsReadCardSuccess)
                    {
                        if (IsCheck == 1)
                        {
                            //校验身份证的有效期
                            try
                            {
                                if (!"长期".Equals(VTM_IDCARD.IdCard_EndDate)) //非长期，则校验
                                {
                                    DateTime endDt = DateTime.ParseExact(VTM_IDCARD.IdCard_EndDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                                    if (endDt.CompareTo(DateTime.Now) < 0)
                                    {
                                        //超过有效期，界面提示
                                        if (m_objContext.UIState.ContainsKey("OutDateError"))
                                        {
                                            SwitchUIState(m_objContext.MainUI, "OutDateError", 3000);
                                            WaitSignal();
                                        }
                                        VTMContext.LogJournalKey("IDS_ScanIDCardOutDateFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                    }
                                    else
                                    {
                                        //未超过有效期
                                        VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                    }
                                }
                                else
                                {
                                    //长期，则不校验
                                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                }
                            }
                            catch (WorkflowIllegalException ex)
                            {
                                throw ex;
                            }
                            catch (WorkflowTerminateException ex)
                            {
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                Log.Action.LogError("time compare is fail", ex);
                                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                if (m_objContext.UIState.ContainsKey("OutDateError"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "OutDateError", 3000);
                                    WaitSignal();
                                }
                                VTMContext.LogJournalKey("IDS_ScanIDCardOutDateFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                            }
                        }
                        else
                        {
                            //不校验有效期
                            VTMContext.NextCondition = EventDictionary.s_EventContinue;
                        }
                    }
                    else
                    {
                        //读取身份证信息失败，转到下一个Action提示是否需要重新读取
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }
                }

            }
            else
            {
                //读卡器硬件故障
                if (m_objContext.UIState.ContainsKey("IDCardError"))
                {
                    SwitchUIState(m_objContext.MainUI, "IDCardError", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_IDCardNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }



        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }
            if (callMethod == CallingMethod.WaitConfirm)//等待按钮触发调用
            {
                if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
                {
                    if (objArg.Key is string)
                    {
                        string key = (string)objArg.Key;
                        if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                        {
                            SendSignal(SignalConfirm);
                            return emBusiCallbackResult_t.Swallowd;
                        }
                        else
                        {
                            m_objContext.NextCondition = key;
                            if (key.Equals("OnInput"))
                            {
                                SendSignal(SignalInputID);
                            }
                            else
                            {
                                VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                                SendSignal(Signal.Cancel);
                            }
                            return emBusiCallbackResult_t.Swallowd;
                        }
                    }
                }
            }
            else
            {
                if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
                {
                    if (objArg.Key is string)
                    {
                        string key = (string)objArg.Key;
                        m_objContext.NextCondition = key;

                        if (key.Equals("OnInput"))
                        {
                            SendSignal(SignalInputID);
                        }
                        else
                        {
                            VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                            SendSignal(Signal.Cancel);
                        }
                        return emBusiCallbackResult_t.Swallowd;
                    }
                }
            }
            return emBusiCallbackResult_t.Bypass;
        }
        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService iDev, DeviceEventArg objArg)
        {
            if (null != VTMContext.IDReader && objArg.Source == VTMContext.IDReader.HostedDevice)
            {
                switch (objArg.Event)
                {
                    // card take event
                    case CARDREADEREVENT.s_MediaMoved:
                        Log.Action.LogInfo("Device event: IDcard media moved");
                        m_objContext.LogJournalWithDateTime(m_objContext.CurrentJPTRResource.LoadString("IDS_IDCardTaken", TextCategory.s_journal), LogSymbol.TakeCard);
                        SendSignal(Signal.Success);
                        return emBusiCallbackResult_t.Swallowd;
                    case CARDREADEREVENT.s_MediaInserted:
                        m_objContext.LogJournalWithDateTime(m_objContext.CurrentJPTRResource.LoadString("IDS_IDCardInsert", TextCategory.s_journal), LogSymbol.InsertCard);
                        if (m_objContext.UIState.ContainsKey("ReadingCard"))
                        {
                            SwitchUIState(m_objContext.MainUI, "ReadingCard", -int.MaxValue);
                        }
                        return emBusiCallbackResult_t.Swallowd;
                    default:
                        break;
                }
                if (objArg.CommandCompleted)
                {
                    if (objArg.ResultOfCommandCompleted != null)
                    {
                        if (objArg.ResultOfCommandCompleted.CommandString.Equals(CardReaderDeviceProtocol.CARDREADERDEVICECMD.s_ReadRawDataCmd))
                        {
                            if (objArg.ResultOfCommandCompleted.IsSuccess)
                            {
                                GrgCmdReadRawDataOutArg outObj = (GrgCmdReadRawDataOutArg)objArg.ResultOfCommandCompleted;
                                if (null != outObj)
                                {
                                    if (!SaveRawData(outObj))
                                    {
                                        SendSignal(Signal.Failure);
                                    }
                                    else
                                    {
                                        SendSignal(Signal.Success);
                                    }
                                }
                                else
                                {
                                    SendSignal(Signal.Failure);
                                    Log.Action.LogDebug("Scan IDCard failed,outObj(return value) is null");
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
                            VTMContext.IDReader.CancelReadRawData();
                        }
                    }
                }
                return emBusiCallbackResult_t.Swallowd;
            }

            return base.InnerOnDevEvtHandle(iDev, objArg);
        }



        protected override void InnerTerminate(bool argIsUserCancel)
        {
            VTMContext.IDReader.CancelReadRawData();
            SetLight(GuidLight.IDCard, GuidLightFlashMode.Off);
            base.InnerTerminate(argIsUserCancel);
        }
        /// <summary>
        /// 取消身份证上读取相应的数据
        /// </summary>
        public virtual void Cancel()
        {
            SetLight(GuidLight.IDCard, GuidLightFlashMode.Off);
            VTMContext.IDReader.CancelReadRawData();
        }

        /// <summary>
        /// 读取相应的身份证上的信息
        /// </summary>
        /// <param name="argRawData"></param>
        /// <returns></returns>
        private bool SaveRawData(GrgCmdReadRawDataOutArg argRawData)
        {
            try
            {
            
                if (OnlyImg == 1)
                {
                    string imageDes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                                                 m_objContext.TerminalConfig.Terminal.ATMNumber + "A.jpg");
                    File.Copy(argRawData.StrTrack2, imageDes, true);
                    VTM_IDCARD.IdCard_ScanImg1 = imageDes;
                    imageDes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                                                      m_objContext.TerminalConfig.Terminal.ATMNumber + "B.jpg");
                    File.Copy(argRawData.StrTrack3, imageDes, true);
                    VTM_IDCARD.IdCard_ScanImg2 = imageDes;
                }
                else if (!string.IsNullOrEmpty(argRawData.StrTrack1))
                {
                    string IDInfoName = string.Empty;
                    IDInfoName = argRawData.StrTrack1;
                    Dictionary<string, string> iDInfoKey = new Dictionary<string, string>(); //

                    if (!string.IsNullOrWhiteSpace(IDInfoName))
                    {
                        string[] strIDInfoArray = IDInfoName.Split('|');
                        for (int i = 0; i < strIDInfoArray.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(strIDInfoArray[i]))
                            {
                                string[] keyValue = strIDInfoArray[i].Split('=');
                                iDInfoKey.Add(keyValue[0], keyValue[1]);
                            }
                        }
                    }

                    //获取身份证信息
                    string strdestFileName = iDInfoKey["PhotoFileName"].ToString();
                    if (!string.IsNullOrWhiteSpace(iDInfoKey["PhotoFileName"].ToString()) && File.Exists(iDInfoKey["PhotoFileName"].ToString()))
                    {
                        try
                        {
                            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Temp\\"))
                            {
                                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Temp\\");
                            }
                            string strExtension = Path.GetExtension(iDInfoKey["PhotoFileName"].ToString());
                            strdestFileName = AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + "ID" + strExtension;
                            File.Copy(iDInfoKey["PhotoFileName"].ToString(), strdestFileName, true);
                            Log.Action.LogDebug("Copy File Success");
                        }
                        catch
                        { }

                        VTM_IDCARD.IdCard_PhotoPath = strdestFileName;// iDInfoKey["PhotoFileName"].ToString();
                        VTM_IDCARD.IdCard_Name = iDInfoKey["Name"].ToString();

                        VTM_IDCARD.IdCard_Sex = iDInfoKey["Sex"].ToString();
                        VTM_IDCARD.IdCard_Nation = iDInfoKey["Nation"].ToString();

                        string strBirthday = iDInfoKey["Born"].ToString();
                        VTM_IDCARD.IdCard_Birthday = strBirthday.Trim();
                        VTM_IDCARD.IdCard_Year = strBirthday.Substring(0, 4);
                        VTM_IDCARD.IdCard_Month = strBirthday.Substring(4, 2);
                        VTM_IDCARD.IdCard_Day = strBirthday.Substring(6, 2);

                        VTM_IDCARD.IdCard_Address = iDInfoKey["Address"].ToString();
                        VTM_IDCARD.IdCard_IDNo = iDInfoKey["IDCardNo"].ToString();
                        VTM_IDCARD.IdCard_IDOrg = iDInfoKey["GrantDept"].ToString();
                        VTM_IDCARD.IdCard_BeginDate = iDInfoKey["UserLifeBegin"].ToString();
                        VTM_IDCARD.IdCard_EndDate = iDInfoKey["UserLifeEnd"].ToString();
                    }
                    if (!String.IsNullOrEmpty(argRawData.StrTrack2))
                    {
                        VTM_IDCARD.IdCard_ScanImg1 = argRawData.StrTrack2;

                    }

                    if (!String.IsNullOrEmpty(argRawData.StrTrack3))
                    {
                        VTM_IDCARD.IdCard_ScanImg2 = argRawData.StrTrack3;
                    }


                    //added by clyun 生成身份证正反面图片
                    if (GenerateImg == 1)
                    {
                        if (string.IsNullOrWhiteSpace(VTM_IDCARD.IdCard_ScanImg1))
                        {
                            VTM_IDCARD.IdCard_ScanImg1 = string.Format("{0}Temp\\{1}.png",
                                AppDomain.CurrentDomain.BaseDirectory, System.Guid.NewGuid().ToString());
                        }
                        //TODO: Just for test  --yslin3
                        else
                        {
                            VTM_IDCARD.IdCard_ScanImg1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                                m_objContext.TerminalConfig.Terminal.ATMNumber + "A.jpg");
                        }

                        if (string.IsNullOrWhiteSpace(VTM_IDCARD.IdCard_ScanImg2))
                        {
                            VTM_IDCARD.IdCard_ScanImg2 = string.Format("{0}Temp\\{1}.png", AppDomain.CurrentDomain.BaseDirectory, System.Guid.NewGuid().ToString());
                        }
                        //TODO: Just for test  --yslin3
                        else
                        {
                            VTM_IDCARD.IdCard_ScanImg2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                                m_objContext.TerminalConfig.Terminal.ATMNumber + "B.jpg");
                        }

                        //  if (!File.Exists(VTM_IDCARD.IdCard_ScanImg1))
                        //{
                        GenerateIdCardFrontImage();
                        // }


                        // if (!File.Exists(VTM_IDCARD.IdCard_ScanImg2))
                        // {
                        GenerateIdCardBackImage();
                        //  }

                    }

                    //add by gxbao 将身份证正反面从C盘复制到ecatTempt目录下
                    //if (m_objContext.GeneralConfig.IsCSharpSTMA == 1)
                    //{
                    //    string tempScanImg1 = string.Format("{0}Temp\\{1}.jpg", AppDomain.CurrentDomain.BaseDirectory, "IDCard_Front");
                    //    string tempScanImg2 = string.Format("{0}Temp\\{1}.jpg", AppDomain.CurrentDomain.BaseDirectory, "IDCard_Back");
                    //    string tempScanImg3 = string.Format("{0}Temp\\{1}.bmp", AppDomain.CurrentDomain.BaseDirectory, "Head_Photo");
                    //    if (!string.IsNullOrWhiteSpace(VTM_IDCARD.IdCard_ScanImg1))
                    //    {
                    //        File.Copy(VTM_IDCARD.IdCard_ScanImg1, tempScanImg1, true);
                    //    }

                    //    if (!string.IsNullOrWhiteSpace(VTM_IDCARD.IdCard_ScanImg2))
                    //    {
                    //        File.Copy(VTM_IDCARD.IdCard_ScanImg2, tempScanImg2, true);
                    //    }

                    //    if (!string.IsNullOrWhiteSpace(VTM_IDCARD.IdCard_PhotoPath))
                    //    {
                    //        File.Copy(VTM_IDCARD.IdCard_PhotoPath, tempScanImg3, true);
                    //    }

                    //    VTM_IDCARD.IdCard_ScanImg1 = tempScanImg1;
                    //    VTM_IDCARD.IdCard_ScanImg2 = tempScanImg2;

                    //}
                }
                m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreIDCardFgImgPath, VTM_IDCARD.IdCard_ScanImg1, GetType());                

                m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreIDCardBgImgPath, VTM_IDCARD.IdCard_ScanImg2, GetType());

                m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_IDCARD, VTM_IDCARD, GetType());

                if (File.Exists(VTM_IDCARD.IdCard_PhotoPath))
                {
                    string imgBase64String = ImgProc.ImgToBase64String(VTM_IDCARD.IdCard_PhotoPath);
                    m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreIDAvatarImgBase64String, imgBase64String, GetType());
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("Get ID Info Fail" + ex.ToString());
                return false;
            }

            return true;
        }

        private bool GenerateIdCardFrontImage()
        {
            try
            {
                Log.Action.LogDebug("Generate ID card front image");
                using (Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Resource\Common\Image\CN\Common\idcard_front_bg.png"))
                {
                    using (Image img2 = Image.FromFile(VTM_IDCARD.IdCard_PhotoPath))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            using (Font fnt = new Font("微软雅黑", 22, GraphicsUnit.Pixel))
                            {
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                g.DrawString(VTM_IDCARD.IdCard_Name, fnt, Brushes.Black, new PointF(98, 56));
                                var fnt2 = new Font("微软雅黑", 20, GraphicsUnit.Pixel);
                                g.DrawString(VTM_IDCARD.IdCard_Sex, fnt2, Brushes.Black, new PointF(98, 98));
                                g.DrawString(VTM_IDCARD.IdCard_Nation, fnt2, Brushes.Black, new PointF(228, 98));
                                g.DrawString(VTM_IDCARD.IdCard_Year, fnt2, Brushes.Black, new PointF(98, 139));
                                g.DrawString(VTM_IDCARD.IdCard_Month, fnt2, Brushes.Black, new PointF(191, 139));
                                g.DrawString(VTM_IDCARD.IdCard_Day, fnt2, Brushes.Black, new PointF(247, 139));
                                int addressLine = Convert.ToInt32(Math.Ceiling(VTM_IDCARD.IdCard_Address.Length / 10.0));
                                for (int i = 0; i < addressLine; i++)
                                {
                                    if (i != addressLine - 1)
                                    {
                                        g.DrawString(VTM_IDCARD.IdCard_Address.Substring(i * 10, 10), fnt, Brushes.Black, new PointF(98, 178 + i * 25));
                                    }
                                    else
                                    {
                                        g.DrawString(VTM_IDCARD.IdCard_Address.Substring(i * 10), fnt, Brushes.Black, new PointF(98, 178 + i * 25));
                                    }
                                }
                                StringBuilder idBuilder = new StringBuilder();
                                foreach (var ch in VTM_IDCARD.IdCard_IDNo)
                                {
                                    idBuilder.Append(ch);
                                    idBuilder.Append(' ');
                                }
                                idBuilder.Remove(idBuilder.Length - 1, 1);
                                var fnt3 = new Font("Arial", 22, GraphicsUnit.Pixel);
                                g.DrawString(idBuilder.ToString(), fnt3, Brushes.Black, new PointF(190, 288));

                                ImageAttributes attri = new ImageAttributes();
                                Color transparentClr = Color.FromArgb(254, 254, 254);
                                attri.SetColorKey(transparentClr, transparentClr);
                                g.DrawImage(img2, new Rectangle(360, 56, 156, 208), 0, 0, img2.Width, img2.Height, GraphicsUnit.Pixel, attri);

                                g.Flush();
                            }
                        }

                        img.Save(VTM_IDCARD.IdCard_ScanImg1, ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

        private bool GenerateIdCardBackImage()
        {
            try
            {
                Log.Action.LogDebug("Generate ID card back image");
                using (Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Resource\Common\Image\CN\Common\idcard_back_bg.png"))
                {
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        using (Font fnt = new Font("微软雅黑", 20, GraphicsUnit.Pixel))
                        {
                            var fnt1 = new Font("Arial", 20, GraphicsUnit.Pixel);
                            g.DrawString(VTM_IDCARD.IdCard_IDOrg, fnt, Brushes.Black, new PointF(220, 245));

                            string dateformat = m_objContext.CurrentUIResource.LoadString("IDS_DateFormater", TextCategory.s_UI);
                            string enddtime = string.Empty;
                            if (currentIDCard.IdCard_EndDate.Equals("长期"))
                            {
                                enddtime = "长期";
                            }
                            else
                            {
                                enddtime = DateTime.ParseExact(currentIDCard.IdCard_EndDate, "yyyyMMdd", null).ToString(dateformat);
                            }

                            DateTime begindtime = DateTime.ParseExact(currentIDCard.IdCard_BeginDate, "yyyyMMdd", null);
                            string IdCard_ValidateDate = string.Concat(begindtime.ToString(dateformat), m_objContext.CurrentUIResource.LoadString("IDS_DateConcat", TextCategory.s_UI), enddtime);

                            g.DrawString(IdCard_ValidateDate, fnt1, Brushes.Black, new PointF(220, 290));
                            g.Flush();
                        }
                        img.Save(VTM_IDCARD.IdCard_ScanImg2, ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

        private short m_currentCk = 0; //是否校验有效期
        [GrgBindTarget("check", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsCheck
        {
            get
            {
                return m_currentCk;
            }
            set
            {
                m_currentCk = value;
                OnPropertyChanged("IsCheck");
            }
        }
        private short m_ejcard = 0; //是否校验有效期
        [GrgBindTarget("ejcard", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsEjcard
        {
            get
            {
                return m_ejcard;
            }
            set
            {
                m_ejcard = value;
                OnPropertyChanged("IsEjcard");
            }
        }

        private short m_again = 0; //是否重新读取
        [GrgBindTarget("again", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsAgain
        {
            get
            {
                return m_again;
            }
            set
            {
                m_again = value;
                OnPropertyChanged("IsAgain");
            }
        }

        private int m_generateImg = 0; //是否生成身份证图片
        [GrgBindTarget("generateImg", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int GenerateImg
        {
            get
            {
                return m_generateImg;
            }
            set
            {
                m_generateImg = value;
                OnPropertyChanged("GenerateImg");
            }
        }
        private int m_onlyImg = 0; //是否仅要身份证图片
        [GrgBindTarget("onlyImg", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int OnlyImg
        {
            get
            {
                return m_onlyImg;
            }
            set
            {
                m_onlyImg = value;
                OnPropertyChanged("OnlyImg");
            }
        }
        #endregion
    }
}
