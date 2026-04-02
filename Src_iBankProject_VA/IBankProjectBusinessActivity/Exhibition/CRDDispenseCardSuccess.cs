using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using System.Configuration;
using eCATBusinessServiceProtocol;
using DevServiceProtocol;
using System.Globalization;
using System.Threading;
using ResourceManagerProtocol;
using VTMModelLibrary.common;
using CardReaderDeviceProtocol;
using System.Diagnostics;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{4DEDE1B4-E682-4BBE-B1A7-0757677EDA7B}",
                 NodeNameOfConfiguration = "CRDDispenseCardSuccess",
                 Name = "CRDDispenseCardSuccess",
                 Author = "gxbao")]
    public class CRDDispenseCardSuccess : BusinessActivityVTMBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new CRDDispenseCardSuccess() as IBusinessActivity;
        }
        #endregion
        #region constructor
        protected CRDDispenseCardSuccess()
        {

        }
        #endregion

        private CardAccountInfo cardInfo = new CardAccountInfo();
        public CardAccountInfo CardInfo
        {
            get { return cardInfo; }
            set
            {
                cardInfo = value;
                OnPropertyChanged("CardInfo");
            }
        }
        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            else
            {

                //CardInfo.OperMessage = m_objContext.CurrentUIResource.LoadString("IDS_DispenseCard1", TextCategory.s_UI);
                string format ="#### #### #### " ;
                string outString = string.Empty;
                //获得卡号
                object obj;
                m_objContext.CardHolderDataCache.Get(DataDictionary.s_corePAN, out obj, GetType());
                if (null != obj)
                {
                    StringFormatter.StringFormatterImp.Create().Format(obj.ToString().Replace(" ", "").Trim(), format, out outString);
                    CardInfo.CardNumber = string.IsNullOrEmpty(outString) ? obj.ToString() : outString;
                    m_objContext.TransactionDataCache.Set("core_CardNumber", CardInfo.CardNumber, GetType());
                }
                CardInfo.CardClient = new Random().Next(10000, 99999).ToString();
                CardInfo.CardDate = DateTime.Now.ToString("yyyy-MM-dd");
                m_objContext.TransactionDataCache.Set("core_ClientNo", CardInfo.CardClient, GetType());
                m_objContext.TransactionDataCache.Set("core_CardDate", CardInfo.CardDate, GetType());
              //  CardInfo.OperMessage = m_objContext.CurrentUIResource.LoadString("IDS_DispenseCard2", TextCategory.s_UI);
                SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState, 3000);
                WaitSignal();
                if (HandleEjectCard())
                {
                    SwitchUIState(m_objContext.MainUI, "TakeCard");
                    emWaitSignalResult_t signalResult = WaitSignal();
                    if (emWaitSignalResult_t.Timeout == signalResult)
                    {
                        SwitchUIState(m_objContext.MainUI, "EjectCardError", 3000);
                        WaitSignal();
                        VTMContext.LogJournalKey("IDS_EjectCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        Log.Action.LogError("execute EjectCard failure");
                        VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    }
                    else
                    {
                        VTMContext.NextCondition = EventDictionary.s_EventContinue;
                    }

                }
                else
                {
                    SwitchUIState(m_objContext.MainUI, "EjectCardError", 3000);
                    WaitSignal();
                    VTMContext.LogJournalKey("IDS_EjectCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    Log.Action.LogError("execute EjectCard failure");
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                }


            }
            SetLight(GuidLight.EnvDepository, GuidLightFlashMode.Off);
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCardSuccess can not Terminate";
            return false;
        }

        public virtual bool HandleEjectCard()
        {
            SwitchUIState(m_objContext.MainUI, "EjectWait");
            // eject card

           DevResult devResult= VTMContext.CardDispenser.EjectCard();
           if (devResult.IsSuccess)
            {
                SetLight(GuidLight.EnvDepository, GuidLightFlashMode.Slow);
                //退卡时，设置core_CardInserted数据池为false，以便后续换卡交易检查IDC状态能通过，否则会Device Error
                m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreCardInserted, false, GetType());
                // eject card success
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService argiDev, DeviceEventArg argDevEvent)
        {
            try
            {
                Debug.Assert(null != argiDev && null != argDevEvent);
                emBusiCallbackResult_t result = base.InnerOnDevEvtHandle(argiDev, argDevEvent);
                if (emBusiCallbackResult_t.Swallowd == result)
                {
                    return result;
                }

                if (null != VTMContext.CardDispenser && argDevEvent.Source == VTMContext.CardDispenser.HostedDevice)
                {
                    switch (argDevEvent.Event)
                    {
                        // card take event
                        case "MediaRemoved":
                            Log.Action.LogInfo("Device event: card media moved");
                            m_objContext.LogJournalWithDateTime(m_objContext.CurrentJPTRResource.LoadString("IDS_CardTaken", TextCategory.s_journal), LogSymbol.TakeCard);
                            //退卡时，设置core_CardInserted数据池为false，以便后续换卡交易检查IDC状态能通过，否则会Device Error
                            m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreCardInserted, false, GetType());
                            SignalCancel();
                            break;
                        default:
                            break;
                    }
                    return emBusiCallbackResult_t.Swallowd;
                }
                return emBusiCallbackResult_t.Bypass;
            }
            catch (System.Exception ex)
            {
                Log.Action.LogFatalFormat("Close transaction InnerOnDevEvtHandle exception", ex);
                return emBusiCallbackResult_t.Swallowd;
            }
        }

        #endregion

    }
}
