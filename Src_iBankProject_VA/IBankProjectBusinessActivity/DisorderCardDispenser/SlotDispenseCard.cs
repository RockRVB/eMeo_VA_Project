using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using System.Diagnostics.CodeAnalysis;
using VTMBusinessActivityBase;

namespace IBankProjectBusinessActivity.DisorderCardDispenser
{
    [GrgActivity("{C333AD21-853E-065C-F30A-3AE69AC8B9D0}",
        Name = "SlotDispenseCard",
        NodeNameOfConfiguration = "SlotDispenseCard")]
    public class SlotDispenseCard : BusinessActivityVTMBase
    {
        #region constructor

        protected SlotDispenseCard()
        {

        }

        #endregion

        #region create

        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new SlotDispenseCard();
        }

        #endregion

        #region properties

        [GrgBindTarget("slotNo", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int SlotNo { get; set; } = -1;

        [GrgBindTarget("present", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int Present { get; set; } = 0;

        /// <summary>
        /// 卡槽号数据池名称
        /// </summary>
        [GrgBindTarget("slotNumberDatacacheKey", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string SlotNumberDatacacheKey { get; set; } = "proj_SEM_SlotNumber";

        #endregion

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogInfo($"Enter {nameof(SlotDispenseCard)}.{nameof(InnerRun)}()...");

            try
            {
                var baseRet = base.InnerRun(argContext);
                if (baseRet != emBusActivityResult_t.Success)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError("Failed to execute base.InnerRun()!");
                    return baseRet;
                }

                SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
                var failedStep = 0;
                var stepResult = new bool[5];
                GrgCmdDispenserCapInfo argCap = null;
                if (!(stepResult[0] = ++failedStep > 0 && (SlotNo > -1 || (VTMContext.TransactionDataCache.Get(SlotNumberDatacacheKey, out var objSlotNo) && objSlotNo!=null&&(SlotNo = int.Parse(objSlotNo.ToString())) > -1))) || //Check parameter
                    !(stepResult[1] = ++failedStep > 0 && VTMContext.CardDispenser != null) || //Check whether card dispenser is enabled
                    !(stepResult[2] = ++failedStep > 0 && VTMContext.CardDispenser.GetCapabilitiesInfo(out argCap).IsSuccess && argCap != null) || //Get capabilities
                    !(stepResult[3] = ++failedStep > 0 && argCap.IsDisorderCardDispenser == 1) || //Check whether it is a disorder card dispenser
                    !(stepResult[4] = ++failedStep > 0 && VTMContext.CardDispenser.SlotDispense((ushort)SlotNo, Present).IsSuccess)) //Dispense card from slot
                {
                    if (failedStep == 1)
                    {
                        Log.Action.LogError($"Invalid SlotNo=[{SlotNo}]!");
                    }
                    else if (failedStep == 2)
                    {
                        Log.Action.LogError("Card Dispenser is null!");
                    }
                    else if (failedStep == 3)
                    {
                        Log.Action.LogError("Failed to get capabilities info of card dispenser!");
                    }
                    else if (failedStep == 4)
                    {
                        Log.Action.LogError("Card Dispenser is not disorder card dispenser!");
                    }
                    else if (failedStep == 5)
                    {
                        Log.Action.LogError($"Failed to dispense card from slot! SlotNo=[{SlotNo}], Present=[{Present}].");
                    }
                    /*
                    else
                    {
                        Log.Action.LogError($"Unknown failure step [{failedStep}]!");
                    }*/

                    if (m_objContext.UIState.ContainsKey("HardwareError"))
                    {
                        SwitchUIState(m_objContext.MainUI, "HardwareError", 5000);
                    }
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    return emBusActivityResult_t.Failure;
                }

                Log.Action.LogInfo($"Succeeded to dispense card from slot! SlotNo=[{SlotNo}], Present=[{Present}].");
                
                VTMContext.NextCondition = EventDictionary.s_EventContinue;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
                return emBusActivityResult_t.Success;
            }
            finally
            {
                Log.Action.LogInfo($"Leave {nameof(SlotDispenseCard)}.{nameof(InnerRun)}().");
            }
        }
    }
}
