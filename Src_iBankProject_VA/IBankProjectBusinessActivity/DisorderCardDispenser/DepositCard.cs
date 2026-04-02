using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMBusinessServiceProtocol;
using VTMBusinessServiceProtocol.DataGateway;
using VTMModelLibrary;

namespace VTMBusinessActivity
{
    [GrgActivity("{335F3B2C-3B84-4ED2-87DA-8E5F51DEC78E}",
                  Name = "DepositCard",
                  NodeNameOfConfiguration = "DepositCard")]
    public class DepositCard : BusinessActivityVTMBase
    {
        #region constructor
        protected DepositCard()
        {

        }
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new DepositCard();
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }

            if (null != VTMContext.CardDispenser)
            {
                SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
                DevResult emDevRet = VTMContext.CardDispenser.GetSlotInfo(out var slotInfo, m_devTimeout);
                if (emDevRet.IsSuccess)
                {
                    string[] slotStatus = slotInfo.SlotStatus.Split(new[] { ',' }, StringSplitOptions.None);
                    int Index = slotStatus.ToList().IndexOf("NOTPRESENT");
                    if (Index >= 0)
                    {
                        emDevRet = VTMContext.CardDispenser.SlotDeposit((ushort)(Index), m_devTimeout);
                        if (emDevRet.IsSuccess)
                        {
                            Log.Action.LogInfo("Deposit Card Successfully");
                            VTMContext.NextCondition = EventDictionary.s_EventContinue;
                            VTMContext.ActionResult = emBusActivityResult_t.Success;
                            VTMContext.CurrentTransactionResult = TransactionResult.Success;

                            VTMContext.TransactionDataCache.Get(DataDictionary.s_corePAN, out var corePAN, GetType());
                            string PAN = corePAN.ToString();
                            VTMContext.TransactionDataCache.Get("proj_SEM_AddCardList", out var obj, GetType());
                            List<DisorderCardInfo> DisorderCardInfoList;
                            if (null != obj && obj is List<DisorderCardInfo>)
                            {
                                DisorderCardInfoList = (List<DisorderCardInfo>)obj;
                            }
                            else
                            {
                                DisorderCardInfoList = new List<DisorderCardInfo>();
                            }
                            VTMContext.TransactionDataCache.Get("proj_SEM_CardType", out var CardType, GetType());
                            DisorderCardInfoList.Add(new DisorderCardInfo
                            {
                                slotNumber = Index,
                                cardID = Guid.NewGuid().ToString("N"),
                                cardNumber = PAN,
                                depositTime = DateTime.Now.ToString("yyyy-MM-dd"),
                                status = 0,
                                cardType = CardType?.ToString()
                            });
                            VTMContext.TransactionDataCache.Set("proj_SEM_AddCardList", DisorderCardInfoList, GetType());
                            VTMContext.VTMDataGateway.AddDisorderCardInfo(DisorderCardInfoList);
                        }
                        else
                        {
                            Log.Action.LogInfo("Deposit Card Failed");
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                            VTMContext.ActionResult = emBusActivityResult_t.Failure;
                            VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        }
                    }
                    else
                    {
                        Log.Action.LogInfo("Slot is full, can not deposit card");
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                        VTMContext.ActionResult = emBusActivityResult_t.Failure;
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    }
                }
                else
                {
                    if (m_objContext.UIState.ContainsKey("DepositCardError"))
                    {
                        SwitchUIState(m_objContext.MainUI, "DepositCardError", 3000);
                        WaitSignal();
                    }
                    Log.Action.LogError("CardDispenser GetSlotInfo Failed");
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                }
            }
            else
            {
                if (m_objContext.UIState.ContainsKey("DepositCardError"))
                {
                    SwitchUIState(m_objContext.MainUI, "DepositCardError", 3000);
                    WaitSignal();
                }
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCard can not Terminate";
            return false;
        }
        #endregion
    }
}
