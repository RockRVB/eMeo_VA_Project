using System;
using System.Collections.Generic;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using ResourceManagerProtocol;
using RemoteTellerServiceProtocol;
using FeelViewServiceProtocol;
using CardReaderDeviceProtocol;
using CardDispenserDeviceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{40A4043D-7D7E-4917-89DD-DD67066C3C89}",
                  Name = "ReturnCard",
                  NodeNameOfConfiguration = "ReturnCard",
                  Author = "ltfei1")]
    public class ReturnCard : BusinessActivityVTMBase
    {
        #region field
        public const int s_CaptureCardForTimeout = 1;
        public const int s_CaptureCardForHardwareError = 2;
        public const int s_CaptureCardForHostDemand = 3;
        public const int s_CaptureCardForOther = 4;
        #endregion

        #region properties
        private int m_CaptureCardReason = s_CaptureCardForOther;
        [GrgBindTarget("captureCardReason", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int CaptureCardReason
        {
            get
            {
                return m_CaptureCardReason;
            }
            set
            {
                m_CaptureCardReason = value;
                OnPropertyChanged("CaptureCardReason");
            }
        }
        #endregion

        #region constructor
        private ReturnCard()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new ReturnCard();
        }
        #endregion

        #region methods
        private bool isResetReturn = false;
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

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

            HandleCaptureCard(m_CaptureCardReason);
            if (null != VTMContext.CardDispenser)
            {
                List<CardUnitInfo> carUnitList = null;
                VTMContext.CardDispenser.GetCardUnitInfo(out carUnitList);
                if (null != carUnitList && carUnitList.Count > 0)
                {
                    foreach (CardUnitInfo cardUnit in carUnitList)
                    {
                        if (cardUnit.Type == emCRDType.RetainBin)
                        {
                            m_objContext.LogJournalWithTime(string.Format("{0} : {1} ", m_objContext.CurrentJPTRResource.LoadString("IDS_ReturnCardCount"), cardUnit.Count));
                        }
                    }
                }
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        bool isRetainCardFailed = false;
        private bool ReturnCardMeth()
        {
            VTMContext.CardReader.UpdateStatus();
            GrgCmdIDCStatusInfo grgidc = null;
            VTMContext.CardReader.GetStatusInfo(out grgidc);
            DevResult devResult;
            if (null != grgidc && (HardwareState.Online.Equals(grgidc.DeviceState) || HardwareState.Busy.Equals(grgidc.DeviceState)))
            {
                if (!ExecuteDevCommand(CARDREADERDEVICECMD.s_RetainCardCmd))
                {
                    VTMContext.CardReader.GetStatusInfo(out grgidc);
                    if(grgidc.MediaState==MediaState.NotPresent)
                    {
                        isRetainCardFailed = true;
                    }
                    devResult = VTMContext.CardReader.Reset();
                    isResetReturn = true;
                    return devResult.IsFailure == false;
                }
                else
                {
                    //针对i630c机型，成功也调一次复位吞卡
                    devResult = VTMContext.CardReader.Reset();
                    isResetReturn = true;
                    return true;
                }
            }
            else
            {
                isResetReturn = true;
                devResult = VTMContext.CardReader.Reset();
                return devResult.IsFailure == false;
            }
        }

        // handle capture card
        public virtual void HandleCaptureCard(int argReason)
        {
            // show "capturing card"
            m_objContext.LogJournalWithTime(m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardStart", TextCategory.s_journal), LogSymbol.Alert);

            //是否挖卡成功
            object objtem;
            bool isDispenseCardFail=false;
            VTMContext.TransactionDataCache.Get("VTM_IsDispenCardFailue", out objtem, GetType());
            if(objtem!=null)
            { isDispenseCardFail = (bool)objtem; }
            

            // capture card
            if (ReturnCardMeth()&& !isDispenseCardFail)
            {
                m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreCardInserted, false, GetType());
                m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreCardInsertedInZeroState, false, GetType());
                m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreCardNotInserted, true, GetType());

                if (isResetReturn)
                {
                    VTMContext.LogJournalKey("IDS_CardResetReturn", TextCategory.s_journal, LogSymbol.DeviceFailure);
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CardReturn", TextCategory.s_journal, LogSymbol.DeviceFailure);
                }

                VTMContext.NextCondition = EventDictionary.s_EventContinue;

                //add by lmjun 如果退卡拉住卡导致吞卡失败，则走发卡成功流程
                if (isRetainCardFailed)
                {
                    VTMContext.NextCondition = "OnTakeCardSuccess";
                    return;
                }
                // set capture card flag
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreCaptureCardFlag, true, GetType());

                // for feel view
                m_objContext.TransactionDataCache.Set("F_CAPTURE_TIME", DateTime.Now.ToString("yyyyMMddHHmmss"), GetType());

                // capture card success
                m_objContext.LogJournalWithTime(m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardSuccess", TextCategory.s_journal), LogSymbol.CaptureCard);

                // capture card number
                object value = null;
                m_objContext.CardHolderDataCache.Get(DataDictionary.s_corePAN, out value, GetType());
                if (null != value)
                {
                    m_objContext.LogJournal(string.Format("PAN:{0}", value), LogSymbol.None);
                }

                // capture card reason
                switch (argReason)
                {
                    case s_CaptureCardForTimeout:
                        m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReasonTakeCardTimeout", TextCategory.s_journal), LogSymbol.None);
                        if (null != value)
                        {
                            m_objContext.CaptureCardGateway.AddCaptureRecord(value.ToString(), m_objContext.CurrentJPTRResource.LoadString("IDS_TakeCardTimeout", TextCategory.s_journal));
                        }
                        // for feel view
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASON", "01", GetType());
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASONCODE", m_objContext.CurrentJPTRResource.LoadString("IDS_TakeCardTimeout", TextCategory.s_journal), GetType());

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            // 1-超时吞卡
                            m_objContext.CurrentRecord.CaptureCardFlag = CaptureCardFlag.s_Timeout;
                            m_objContext.CurrentRecord.Submit();
                        }
                        break;
                    case s_CaptureCardForHardwareError:
                        m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReasonHardwareError", TextCategory.s_journal), LogSymbol.None);
                        if (null != value)
                        {
                            m_objContext.CaptureCardGateway.AddCaptureRecord(value.ToString(), m_objContext.CurrentJPTRResource.LoadString("IDS_HardwareError", TextCategory.s_journal));
                        }
                        // for feel view
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASON", "03", GetType());
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASONCODE", m_objContext.CurrentJPTRResource.LoadString("IDS_HardwareError", TextCategory.s_journal), GetType());

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            // 2-设备故障吞卡
                            m_objContext.CurrentRecord.CaptureCardFlag = CaptureCardFlag.s_HardwareError;
                            m_objContext.CurrentRecord.Submit();
                        }
                        break;
                    case s_CaptureCardForHostDemand:
                        m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReasonCaptureCardForHostDemand", TextCategory.s_journal), LogSymbol.None);
                        if (null != value)
                        {
                            m_objContext.CaptureCardGateway.AddCaptureRecord(value.ToString(), m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardForHostDemand", TextCategory.s_journal));
                        }
                        // for feel view
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASON", "02", GetType());
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASONCODE", m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardForHostDemand", TextCategory.s_journal), GetType());

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            // 3-主机要求吞卡
                            m_objContext.CurrentRecord.CaptureCardFlag = CaptureCardFlag.s_HostDemand;
                            m_objContext.CurrentRecord.Submit();
                        }
                        break;
                    case s_CaptureCardForOther:
                        m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReasonCaptureCardForOther", TextCategory.s_journal), LogSymbol.None);
                        if (null != value)
                        {
                            m_objContext.CaptureCardGateway.AddCaptureRecord(value.ToString(), m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardForOther", TextCategory.s_journal));
                        }
                        // for feel view
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASON", "03", GetType());
                        m_objContext.TransactionDataCache.Set("F_CAPTURE_REASONCODE", m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardForOther", TextCategory.s_journal), GetType());

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            // 5-其他原因吞卡
                            m_objContext.CurrentRecord.CaptureCardFlag = CaptureCardFlag.s_OtherReason;
                            m_objContext.CurrentRecord.Submit();
                        }
                        break;
                    default:
                        break;
                }

                // send capture card msg to feel view
                if (null != m_objContext.FeelViewService)
                {
                    m_objContext.FeelViewService.SendFeelViewMsg(FeelViewMsgType.CaptureCardMsg);
                }

                // backup transType data cache
                value = null;
                m_objContext.TransactionDataCache.Get(DataDictionary.s_coreTransType, out value, GetType());
                m_objContext.TransactionDataCache.Set("core_TransTypeBackup", value, GetType());

                // set capture card time
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreTransDateAndTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), GetType());
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreTransDate, DateTime.Now.ToString("yyyy-MM-dd"), GetType());
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreTransTime, DateTime.Now.ToString("HH:mm:ss"), GetType());


                #region 发送吞卡报文 gyb add at 20130628
                emCaptureCardReason emReason;
                switch (argReason)
                {
                    case s_CaptureCardForTimeout:
                        emReason = emCaptureCardReason.Timeout;
                        break;
                    case s_CaptureCardForHostDemand:
                        emReason = emCaptureCardReason.HostDemand;
                        break;
                    case s_CaptureCardForHardwareError:
                        emReason = emCaptureCardReason.HardwareError;
                        break;
                    case s_CaptureCardForOther:
                        emReason = emCaptureCardReason.Other;
                        break;
                    default:
                        emReason = emCaptureCardReason.Timeout;
                        break;
                }

                emRetCode emRet = emRetCode.Default;
                if (m_objContext.BankInterface != null)
                {
                    try
                    {
                        emRet = m_objContext.BankInterface.SendCaptureCardMessage(emReason);
                        Log.Action.LogInfoFormat("Execute SendCaptureCardMessage return {0}", emRet);
                    }
                    catch (Exception ex)
                    {
                        Log.Action.LogError("Execute SendCaptureCardMessage fail", ex);
                    }
                }

                if (emRet == emRetCode.Default)
                {
                    SendCaptureCardMessage(emReason);
                }

                #endregion
            }
            else
            {
                m_objContext.LogJournalWithTime(m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardFail", TextCategory.s_journal), LogSymbol.Alert);
                if (m_objContext.UIState.ContainsKey("ReturnCardError"))
                {
                    SwitchUIState(m_objContext.MainUI, "ReturnCardError", 3000);
                    WaitSignal();
                }
                VTMContext.LogJournalKey("IDS_RetainCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
            }
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "ReturnCard can not Terminate";
            return false;
        }


        #endregion
    }
}
