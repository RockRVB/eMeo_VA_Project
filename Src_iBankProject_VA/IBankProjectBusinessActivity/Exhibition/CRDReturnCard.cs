using System;
using System.Collections.Generic;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using ResourceManagerProtocol;
using FeelViewServiceProtocol;
using CardReaderDeviceProtocol;
using CardDispenserDeviceProtocol;
using RemoteTellerServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{96D1A18B-233A-4212-A112-E9DC99C66CD5}",
                  Name = "CRDReturnCard",
                  NodeNameOfConfiguration = "CRDReturnCard",
                  Author = "gxbao")]
    public class CRDReturnCard : BusinessActivityVTMBase
    {
        #region field
        public const int s_CaptureCardForTimeout = 1;
        public const int s_CaptureCardForHardwareError = 2;
        public const int s_CaptureCardForHostDemand = 3;
        public const int s_CaptureCardForOther = 4;

        #endregion

        #region constructor
        private CRDReturnCard()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new CRDReturnCard();
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
            //发卡交易中如果发生吞卡，则需要屏蔽发卡交易,设置发卡交易失败标志
            VTMContext.DatabaseCache.Set("IsNewAccountApplyFail", "1", GetType());

            HandleCaptureCard(s_CaptureCardForOther);
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

        private bool ReturnCardMeth()
        {
            VTMContext.CRDReader.UpdateStatus();
            GrgCmdIDCStatusInfo grgidc = null;
            VTMContext.CRDReader.GetStatusInfo(out grgidc);
            DevResult devResult;
            if (null != grgidc && (HardwareState.Online.Equals(grgidc.DeviceState) || HardwareState.Busy.Equals(grgidc.DeviceState)))
            {
                devResult = VTMContext.CRDReader.RetainCard();
                if (devResult.IsFailure)
                {
                    devResult = VTMContext.CRDReader.Reset();
                    isResetReturn = true;
                    return devResult.IsFailure == false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                isResetReturn = true;
                devResult = VTMContext.CRDReader.Reset();
                return devResult.IsFailure == false;
            }
        }

        // handle capture card
        public virtual void HandleCaptureCard(int argReason)
        {
            // show "capturing card"
            m_objContext.LogJournalWithTime(m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardStart", TextCategory.s_journal), LogSymbol.Alert);

            // capture card
            if (ReturnCardMeth())
            {
                if (isResetReturn)
                {
                    VTMContext.LogJournalKey("IDS_CardResetReturn", TextCategory.s_journal, LogSymbol.DeviceFailure);
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CardReturn", TextCategory.s_journal, LogSymbol.DeviceFailure);
                }
                VTMContext.NextCondition = EventDictionary.s_EventContinue;
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
