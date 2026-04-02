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
using FeedChannelDeviceProtocol;
using System.Threading;
using ResourceManagerProtocol;
using RemoteTellerServiceProtocol;
using VTMModelLibrary;
namespace VTMBusinessActivity
{
    [GrgActivity("{F90EA78D-8004-4473-A9EE-9F8FAEE7F3F2}",
                  Name = "StartFeed",
                  NodeNameOfConfiguration = "StartFeed",
                  Author = "ltfei1")]
    public class StartFeed : BusinessActivityVTMBase
    {
        #region field

        private const string m_SignalCancel = "Cancel";
        private const string m_SignalSuccess = "Success";
        private const string m_SignalFailure = "Failure";
        private const string m_SignalAgain = "Again";
        private const string m_SignalMove = "Move";
        private int m_StratFeedTimes = 0;
        private bool isSendMove = true;
        private emDevCmdResult emResult = new emDevCmdResult();
        private bool m_UserCancel = false;
        private string m_BarcodeData = string.Empty; //读取的二维码内容 
        private short m_FeedPaperandImage = 0; // 回收纸张是否扫描二维码
        private bool isAgain = false;
        private string feedImgPath = string.Empty;

        [GrgBindTarget("feedimg", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short FeedPaperandImage
        {
            get { return m_FeedPaperandImage; }
            set
            {
                m_FeedPaperandImage = value;
                OnPropertyChanged("FeedPaperandImage");
            }
        }

        #endregion

        #region constructor
        private StartFeed()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new StartFeed();
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.LogJournalKey("IDS_SystemException", argSymbol: LogSymbol.Alert);
                return emRet;
            }
            if (null != VTMContext.FeedChannel)
            {
                string strSignalNames = string.Format("{0},{1},{2},{3},{4}", m_SignalCancel, m_SignalFailure, m_SignalSuccess, m_SignalAgain, m_SignalMove);
                AddSignal(strSignalNames);
                SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
                _ReStartFeed:
                if (FeedPaperandImage == 1)
                {
                    GrgFeedPaperAndReadImageInArgs objInArg = new GrgFeedPaperAndReadImageInArgs()
                    {
                        UserState = Thread.CurrentThread.ManagedThreadId,
                        codeLineFormat = CodelineFormat.CODELINECMC7,
                        Timeout = m_devTimeout
                    };
                    if (ReadType == 0)
                    {
                        objInArg.FrontImagePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp\\" + Guid.NewGuid().ToString() + ".jpg");
                        objInArg.ReadFrontImage = true;
                        feedImgPath = objInArg.FrontImagePath;

                    }
                    else if (ReadType == 1)
                    {
                        objInArg.ReadBackImage = true;
                        objInArg.BackImagePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp\\" + Guid.NewGuid().ToString() + ".jpg");
                        feedImgPath = objInArg.BackImagePath;
                    }
                    else
                    {
                        objInArg.FrontImagePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp\\" + Guid.NewGuid().ToString() + ".jpg");
                        objInArg.ReadFrontImage = true;
                        objInArg.ReadBackImage = true;
                        objInArg.BackImagePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp\\" + Guid.NewGuid().ToString() + ".jpg");
                        feedImgPath = objInArg.FrontImagePath + "|" + objInArg.BackImagePath;
                    }
                    if (IsCodeline != 0)
                    {
                        objInArg.ReadCodeLine = true;
                    }
                    emResult = VTMContext.FeedChannel.HostedDevice.ExecuteCommandAsyn(FeedChannelCmds.s_FeedPaperAndReadImageCmd, objInArg);
                }
                else
                {
                    GrgCmdInArg objInArg = new GrgCmdInArg()
                    {
                        UserState = Thread.CurrentThread.ManagedThreadId,
                        Timeout = m_devTimeout
                    };
                    emResult = VTMContext.FeedChannel.HostedDevice.ExecuteCommandAsyn(FeedChannelCmds.s_FeedPaperWithoutReadImageCmd, objInArg);

                }
                if (emResult != emDevCmdResult.Pending && emResult != emDevCmdResult.Success)
                {
                    if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                    {
                        SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                        WaitSignal();
                    }
                    VTMContext.LogJournalKey("IDS_StartFeedFailure", argSymbol: LogSymbol.DeviceFailure);
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    return emBusActivityResult_t.Success;
                }
                SetLight(GuidLight.Scanner, GuidLightFlashMode.Continuous);
                string strCurSignalName;
                emWaitSignalResult_t emWaitResult;
                while (true)
                {
                    if (WaitPopu == 1)
                    {
                        emWaitResult = VTMWaitSignal(strSignalNames, out strCurSignalName, false);
                    }
                    else
                    {
                        emWaitResult = WaitSignal(strSignalNames, out strCurSignalName, false);
                    }
                    if (emWaitResult == emWaitSignalResult_t.Timeout)
                    {
                        VTMContext.LogJournalKey("IDS_FeedPaperTimeout", argSymbol: LogSymbol.Alert);
                        VTMContext.FeedChannel.CancelFeedPaper();
                        VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                        VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                        break;
                    }
                    else if (emWaitResult == emWaitSignalResult_t.Success)
                    {
                        if (strCurSignalName.Equals(m_SignalSuccess, StringComparison.OrdinalIgnoreCase))
                        {
                            isAgain = false;
                            if (IsAccept == 0)//只读不进
                            {
                                if (m_objContext.UIState.ContainsKey("ScanFormSuccess"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "ScanFormSuccess", 3000);
                                    WaitSignal();
                                }
                                VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_FEEDSCANIMG, feedImgPath, GetType());
                                VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_FEEDBARCODE, m_BarcodeData, GetType());
                                VTMContext.ActionResult = emBusActivityResult_t.Success;
                                VTMContext.NextCondition = EventDictionary.s_EventContinue;
                            }
                            else
                            {
                                DevResult devre = VTMContext.FeedChannel.RetractPaper();
                                if (devre.IsFailure)
                                {
                                    if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                                    {
                                        SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                                        WaitSignal();
                                    }
                                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                    VTMContext.LogJournalKey("IDS_RetractPaperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                }
                                else
                                {
                                    if (m_objContext.UIState.ContainsKey("RetractPaperSuccess"))
                                    {
                                        SwitchUIState(m_objContext.MainUI, "RetractPaperSuccess", 3000);
                                        WaitSignal();
                                    }
                                    VTMContext.LogJournalKey("IDS_RetractPaperSuccess", TextCategory.s_journal, LogSymbol.None);
                                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_FEEDSCANIMG, feedImgPath, GetType());
                                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_FEEDBARCODE, m_BarcodeData, GetType());
                                    VTMContext.ActionResult = emBusActivityResult_t.Success;
                                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                }
                            }
                            break;
                        }
                        else if (strCurSignalName.Equals(m_SignalMove, StringComparison.OrdinalIgnoreCase))
                        {

                            if (isAgain)
                            {
                                goto _ReStartFeed;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (strCurSignalName.Equals(m_SignalAgain, StringComparison.OrdinalIgnoreCase))
                        {
                            isAgain = false;
                            if (m_StratFeedTimes < Restart)
                            {
                                m_StratFeedTimes++;
                                DevResult devResult = VTMContext.FeedChannel.EjectPaper(); //退纸 
                                if (devResult.IsFailure)
                                {
                                    if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                                    {
                                        SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                                        WaitSignal();
                                    }
                                    VTMContext.LogJournalKey("IDS_ReturnPaperFailure", argSymbol: LogSymbol.DeviceFailure);
                                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                    break;
                                }
                                else
                                {
                                    isAgain = true;
                                    SwitchUIState(m_objContext.MainUI, "ReStartFeed");
                                    continue;
                                }
                            }
                            else
                            {
                                isAgain = false;
                                isSendMove = true;
                                DevResult devResult = VTMContext.FeedChannel.EjectPaper(); //退纸  

                                SwitchUIState(m_objContext.MainUI, "TakePaper", 30000);
                                VTMContext.ActionResult = emBusActivityResult_t.GeneralError;
                                VTMContext.NextCondition = "OnRePrint";
                                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                                continue;
                            }

                        }
                        else if (strCurSignalName.Equals(m_SignalFailure, StringComparison.OrdinalIgnoreCase))
                        {
                            if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                            {
                                SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                                WaitSignal();
                            }
                            isAgain = false;
                            DevResult devResult = VTMContext.FeedChannel.EjectPaper(); //退纸  
                            VTMContext.LogJournalKeyWithTime("IDS_StartFeedFailure", argSymbol: LogSymbol.DeviceFailure);
                            VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                            VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                            VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                            if (devResult.IsFailure)
                            {
                                //if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                                //{
                                //    SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                                //    WaitSignal();
                                //}
                                break;
                            }
                            else
                            {
                                SwitchUIState(m_objContext.MainUI, "TakePaper", 30000);
                                continue;
                            }

                        }
                        else if (strCurSignalName.Equals(m_SignalCancel, StringComparison.OrdinalIgnoreCase))
                        {
                            isAgain = false;
                            GrgCmdFeedChannelStatusInfo feedStates = null;
                            VTMContext.FeedChannel.GetStatusInfo(out feedStates);
                            if (null != feedStates && feedStates.MediaState.Equals(MediaState.Present))
                            {
                                DevResult devResult = VTMContext.FeedChannel.EjectPaper(); //退纸  
                            }
                            isSendMove = false;
                            VTMContext.LogJournalKeyWithTime("IDS_StartFeedCancel");
                            VTMContext.NextCondition = EventDictionary.s_EventCancel;
                            VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                            break;
                        }
                        else
                        {
                            isAgain = false;
                            if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                            {
                                SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                                WaitSignal();
                            }
                            VTMContext.LogJournalKeyWithTime("IDS_StartFeedCancel");
                            VTMContext.FeedChannel.CancelFeedPaper();
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                            VTMContext.ActionResult = emBusActivityResult_t.Failure;
                            VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                            break;
                        }
                    }
                }
                VTMContext.FeedChannel.CancelFeedPaper();
                SetLight(GuidLight.Scanner, GuidLightFlashMode.Off);
            }
            else
            {
                if (m_objContext.UIState.ContainsKey("StartFeedFailure"))
                {
                    SwitchUIState(m_objContext.MainUI, "StartFeedFailure", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_FeedChannelNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;

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
            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    if (key.Equals(EventDictionary.s_EventCancel, StringComparison.OrdinalIgnoreCase))
                    {
                        SendSignal(m_SignalCancel);
                        return emBusiCallbackResult_t.Swallowd;
                    }
                }
            }
            return emBusiCallbackResult_t.Bypass;
        }


        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService iDev, DeviceEventArg objArg)
        {
            if (VTMContext.FeedChannel != null && iDev == VTMContext.FeedChannel.HostedDevice)
            {
                if (objArg.Event == FeedChannelEvent.s_MediaMoved)
                {
                    m_objContext.LogJournalWithDateTime(m_objContext.CurrentJPTRResource.LoadString("IDS_IDTakePaper", TextCategory.s_journal), LogSymbol.None);
                    VTMContext.ActionResult = emBusActivityResult_t.Success;
                    if (isSendMove)
                    {
                        SendSignal(m_SignalMove);
                    }
                    return emBusiCallbackResult_t.Swallowd;
                }
                if (objArg.CommandCompleted)
                {
                    if (objArg.ResultOfCommandCompleted != null)
                    {
                        if (objArg.ResultOfCommandCompleted.IsSuccess)
                        {
                            if (FeedPaperandImage == 1)
                            {
                                GrgFeedPaperAndReadImageOutArg outObj = (GrgFeedPaperAndReadImageOutArg)objArg.ResultOfCommandCompleted;
                                if (null != outObj)
                                {
                                    if (IsCodeline == 0)
                                    {
                                        if (ReadType == 0)
                                        {
                                            m_BarcodeData = outObj.FrontImageData;
                                        }
                                        else if (ReadType == 1)
                                        {
                                            m_BarcodeData = outObj.BackImageData;
                                        }
                                        else
                                        {
                                            m_BarcodeData = outObj.FrontImageData + "|" + outObj.BackImageData;
                                        }
                                    }
                                    else
                                    {
                                        m_BarcodeData = outObj.CodeLineData;
                                    }
                                }
                                if (IsCheckBarCode == 1)
                                {
                                    object strPostBarCode = null;
                                    VTMContext.TransactionDataCache.Get(VTMDataDictionary.s_QRCode4SignPaper, out strPostBarCode, GetType());
                                    if (!string.IsNullOrWhiteSpace(strPostBarCode as string) && !string.IsNullOrWhiteSpace(m_BarcodeData))
                                    {
                                        if (m_BarcodeData.Equals(strPostBarCode.ToString()))
                                        {
                                            SendSignal(m_SignalSuccess);
                                        }
                                        else
                                        {
                                            SendSignal(m_SignalAgain);
                                        }
                                    }
                                    else
                                    {
                                        SendSignal(m_SignalAgain);
                                    }
                                }
                                else
                                {
                                    SendSignal(m_SignalSuccess);
                                }
                            }
                            else
                            {
                                SendSignal(m_SignalSuccess);
                            }

                        }
                        else if (objArg.ResultOfCommandCompleted.IsFailure)
                        {
                            if (m_UserCancel)
                            {
                                SendSignal(m_SignalCancel);
                            }
                            SendSignal(m_SignalFailure);
                        }
                    }
                }
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnDevEvtHandle(iDev, objArg);
        }

        protected override void InnerTerminate(bool argIsUserCancel)
        {
            isAgain = false;
            isSendMove = false;
            GrgCmdFeedChannelStatusInfo feedStates = null;
            VTMContext.FeedChannel.GetStatusInfo(out feedStates);
            if (null != feedStates && feedStates.MediaState.Equals(MediaState.Present))
            {
                DevResult devResult = VTMContext.FeedChannel.EjectPaper(); //退纸  
            }
            SetLight(GuidLight.Scanner, GuidLightFlashMode.Off);
            VTMContext.LogJournalKeyWithTime("IDS_StartFeedCancel");
            base.InnerTerminate(argIsUserCancel);
        }

        #endregion

        private int m_Restart = 0;
        [GrgBindTarget("restart", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int Restart
        {
            get
            {
                return m_Restart;
            }
            set
            {
                m_Restart = value;
                OnPropertyChanged("Restart");
            }
        }

        private short m_checkBarCode = 1;
        [GrgBindTarget("chkbar", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsCheckBarCode
        {
            get
            {
                return m_checkBarCode;
            }
            set
            {
                m_checkBarCode = value;
                OnPropertyChanged("IsCheckBarCode");
            }
        }

        private short m_IsAccept = 1;
        [GrgBindTarget("accept", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsAccept
        {
            get
            {
                return m_IsAccept;
            }
            set
            {
                m_IsAccept = value;
                OnPropertyChanged("IsAccept");
            }
        }

        private short m_IsCodeline = 0;
        [GrgBindTarget("codeline", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short IsCodeline
        {
            get
            {
                return m_IsCodeline;
            }
            set
            {
                m_IsCodeline = value;
                OnPropertyChanged("IsCodeline");
            }
        }

        private int m_ReadType = 0;
        [GrgBindTarget("readtype", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public int ReadType
        {
            get
            {
                return m_ReadType;
            }
            set
            {
                m_ReadType = value;
                OnPropertyChanged("ReadType");
            }
        }
    }
}
