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
    [GrgActivity("{98797A8B-AFEE-42D7-9BF4-1860E738AAD8}",
                  Name = "DispenseCard4Exhibition",
                  NodeNameOfConfiguration = "DispenseCard4Exhibition",
                  Author = "ltfei1")]
    public class DispenseCard4Exhibition : BusinessActivityVTMBase
    {
        #region constructor
        private DispenseCard4Exhibition()
        {

        }
        #endregion
        #region fild
        private SupplyBin supplyBin = SupplyBin.First;
        private string[] cardUnits;
        private DevResult devResult;
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new DispenseCard4Exhibition();
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
                object objtem;
                VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_CARDSUPPLY_FLAG, out objtem, GetType());
                if (null != objtem)
                {
                    cardUnits = objtem.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Supply))
                    {
                        cardUnits = Supply.Split(',');
                    }
                }

                if (!CheckDispense(cardUnits))
                {
                    Log.Action.LogDebug("No availble card unit");
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;

                    Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                    return emBusActivityResult_t.Success;
                }
                devResult = VTMContext.CardDispenser.DispenseCard(supplyBin, DispenseType.CONSUMER);
                if (devResult.IsFailure)
                {
                    LogDevCommandResult(CardDispenserCmd.s_DispenseCardCmd, devResult);
                    VTMContext.TransactionDataCache.Set("VTM_IsDispenCardFailue", true, GetType());
                    if (m_objContext.UIState.ContainsKey("DispenseCardError"))
                    {
                        SwitchUIState(m_objContext.MainUI, "DispenseCardError", 3000);
                        WaitSignal();
                    }
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                }
                else
                {
                    VTMContext.TransactionDataCache.Set("VTM_IsDispenCardFailue", false, GetType());
                    VTMContext.NextCondition = EventDictionary.s_EventContinue;

                }

                //打印剩余张数
                List<CardUnitInfo> carUnitList;
                VTMContext.CardDispenser.GetCardUnitInfo(out carUnitList);
                if (null != carUnitList && carUnitList.Count > 0)
                {
                    foreach (CardUnitInfo cardUnit in carUnitList)
                    {
                        if (cardUnit.Type == emCRDType.SupplyBin)
                        {
                            m_objContext.LogJournalWithTime(string.Format("{0} : {1},{2} : {3} ", m_objContext.CurrentJPTRResource.LoadString("IDS_CardNumber"), cardUnit.Number, m_objContext.CurrentJPTRResource.LoadString("IDS_CardCount"), cardUnit.Count));
                        }
                    }
                }
            }
            else
            {
                VTMContext.TransactionDataCache.Set("VTM_IsDispenCardFailue", true, GetType());
                if (m_objContext.UIState.ContainsKey("DispenseCardError"))
                {
                    SwitchUIState(m_objContext.MainUI, "DispenseCardError", 3000);
                    WaitSignal();
                }
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        private bool CheckDispense(string[] CardUnits)
        {
            if (null != VTMContext.CardDispenser)
            {
                VTMContext.CardDispenser.GetCardUnitInfo(out var cardUnitsInfo, m_devTimeout);
                if (CardUnits.Length >= 1)
                {
                    foreach (var cardUnit in CardUnits)
                    {
                        foreach (var cardUnitInfo in cardUnitsInfo)
                        {
                            if (cardUnit == cardUnitInfo.Number.ToString() && cardUnitInfo.Type == emCRDType.SupplyBin && cardUnitInfo.Status != "EMPTY" && cardUnitInfo.Count > 0)
                            {
                                supplyBin = (SupplyBin)int.Parse(cardUnit);
                                VTMContext.TransactionDataCache.Set("proj_SEM_CardType", cardUnitInfo.CardName, GetType());
                                return true;
                            }
                        }
                    }
                    return false;
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
        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            if (argUIEvent.Key is SpecButtonFlag)
            {
                return emBusiCallbackResult_t.Swallowd;
            }

            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCard can not Terminate";
            return false;
        }

        private string m_supply = string.Empty;
        [GrgBindTarget("Supply", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string Supply
        {
            get
            {
                return m_supply;
            }
            set
            {
                m_supply = value;
                OnPropertyChanged("Supply");
            }
        }
        #endregion
    }
}
