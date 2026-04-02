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
using VTMModelLibrary;

namespace VTMBusinessActivity
{
    [GrgActivity("{CEAB2A80-72D1-4E49-93C1-02714DF39830}",
                  Name = "CheckAddCardCondition",
                  NodeNameOfConfiguration = "CheckAddCardCondition")]
    public class CheckAddCardCondition : BusinessActivityVTMBase
    {
        #region constructor
        protected CheckAddCardCondition()
        {

        }
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new CheckAddCardCondition();
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
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);

            VTMContext.TransactionDataCache.Set("proj_SEM_Process", "TRUE", GetType());

            //超出时间段；卡箱是否还能出卡；卡库是否还有位置
            if (VTMContext.CheckAddCardCondition())
            {
                VTMContext.NextCondition = EventDictionary.s_EventContinue;
                VTMContext.ActionResult = emBusActivityResult_t.Success;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
            }
            else
            {
                Log.Action.LogInfo("Not meet condition for add card");
                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                VTMContext.ActionResult = emBusActivityResult_t.Success;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCard can not Terminate";
            return false;
        }

        public virtual bool CheckSlotIsAvailable()
        {
            if (null != VTMContext.CardDispenser)
            {

                DevResult emDevRet = VTMContext.CardDispenser.GetSlotInfo(out var slotInfo, m_devTimeout);
                if (emDevRet.IsSuccess)
                {
                    string[] slotStatus = slotInfo.SlotStatus.Split(new[] { ',' }, StringSplitOptions.None);
                    int notPresentIndex = slotStatus.ToList().IndexOf("NOTPRESENT");
                    if (notPresentIndex >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        Log.Action.LogInfo("Slot is full");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        //检查发卡机状态,卡箱是否能发卡
        public virtual bool CheckDispenseCard()
        {            
            if (null != VTMContext.CardDispenser)
            {
                GrgCmdDispenserStatus cardDispenserInfo = null;
                DevResult rets = VTMContext.CardDispenser.GetStatusInfo(out cardDispenserInfo, 1000);
                if (cardDispenserInfo != null)
                {
                    Log.Action.LogInfo("CardDispenser DeviceState is:" + cardDispenserInfo.DeviceState);
                }
                if (cardDispenserInfo == null || (HardwareState.Online != cardDispenserInfo.DeviceState && HardwareState.Busy != cardDispenserInfo.DeviceState))
                {
                    Log.Action.LogInfo("CardDispenser is not online");
                    return false;
                }
                //检查是否有可用卡箱
                VTMContext.CardDispenser.GetCardUnitInfo(out var cardUnitsInfo, m_devTimeout);
                foreach(var cardUnit in cardUnitsInfo)
                {
                    if(cardUnit.Type == emCRDType.SupplyBin && cardUnit.Count != 0 && cardUnit.Status != "EMPTY")
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
