using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using VTMBusinessActivityBase;

namespace IBankProjectBusinessActivity.DisorderCardDispenser
{
    [GrgActivity("{45863DDF-1C3D-0A32-700E-04E2D7DB766E}",
        Name = "UpdateDisorderCardInfo",
        NodeNameOfConfiguration = "UpdateDisorderCardInfo")]
    public class UpdateDisorderCardInfo : BusinessActivityVTMBase
    {
        #region constructor

        protected UpdateDisorderCardInfo()
        {

        }

        #endregion

        #region create

        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new UpdateDisorderCardInfo();
        }

        #endregion

        #region properties

        [GrgBindTarget("status", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string Status { get; set; }

        [GrgBindTarget("cardId", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string CardId { get; set; }

        #endregion
        
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogInfo($"Enter {nameof(UpdateDisorderCardInfo)}.{nameof(InnerRun)}()...");

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

                if (string.IsNullOrWhiteSpace(Status) || string.IsNullOrWhiteSpace(CardId))
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Parameter error! Status=[{Status}], CardId=[{CardId}].");
                    return emBusActivityResult_t.Failure;
                }

                Status = Status.Trim();
                CardId = CardId.Trim();

                if (Status.StartsWith("{") && Status.EndsWith("}"))
                {
                    Status = GetBindData(Status.TrimStart('{').TrimEnd('}'))?.ToString();
                }
                if (CardId.StartsWith("{") && CardId.EndsWith("}"))
                {
                    CardId = GetBindData(CardId.TrimStart('{').TrimEnd('}'))?.ToString();
                }

                if (!int.TryParse(Status, out var status))
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Parameter error! Status=[{Status}].");
                    return emBusActivityResult_t.Failure;
                }

                if (string.IsNullOrWhiteSpace(CardId))
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Parameter error! CardId=[{CardId}].");
                    return emBusActivityResult_t.Failure;
                }

                if (!VTMContext.VTMDataGateway.UpdateDisorderCardInfo(status, CardId))
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Failed to call UpdateDisorderCardInfo! Status=[{status}], CardId=[{CardId}].");
                    return emBusActivityResult_t.Failure;
                }

                Log.Action.LogInfo($"Succeeded to call UpdateDisorderCardInfo! Status=[{status}], CardId=[{CardId}].");

                VTMContext.NextCondition = EventDictionary.s_EventContinue;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
                return emBusActivityResult_t.Success;
            }
            finally
            {
                Log.Action.LogInfo($"Leave {nameof(UpdateDisorderCardInfo)}.{nameof(InnerRun)}().");
            }
        }
    }
}
