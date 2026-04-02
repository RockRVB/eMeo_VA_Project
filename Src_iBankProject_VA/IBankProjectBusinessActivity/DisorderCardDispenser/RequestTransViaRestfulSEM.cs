using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using RequestTrans;
using System;
using VTMBusinessServiceProtocol;
using VTMModelLibrary;
using VTMModelLibrary.Packmodels;

namespace IBankProjectBusinessActivity.DisorderCardDispenser
{
    [GrgActivity("{EAAF1156-E4C0-CCDF-11DC-51C2F49F06A6}",
        Name = "RequestTransViaRestfulSEM",
        NodeNameOfConfiguration = "RequestTransViaRestfulSEM")]
    public class RequestTransViaRestfulSEM : RequestTransViaRestful
    {
        #region constructor

        protected RequestTransViaRestfulSEM()
        {

        }

        #endregion

        #region create

        [GrgCreateFunction("create")]
        public new static IBusinessActivity Create()
        {
            return new RequestTransViaRestfulSEM();
        }

        #endregion

        #region properties

        /// <summary>
        /// 消息序列号数据池名称（必填）
        /// </summary>
        [GrgBindTarget("serialNumberDatacacheKey", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string SerialNumberDatacacheKey { get; set; } = "proj_SEM_SerialNumber";

        /// <summary>
        /// 终端号数据池名称（必填）
        /// </summary>
        [GrgBindTarget("terminalIdDatacacheKey", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string TerminalIdDatacacheKey { get; set; } = "proj_SEM_TerminalId";

        /// <summary>
        /// 用户ID数据池名称（选填）。
        /// </summary>
        [GrgBindTarget("customerIdDatacacheKey", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string CustomerIdDatacacheKey { get; set; }

        protected string SerialNumber { get; set; }

        protected VTMBusinessContext VTMContext { get; set; }

        #endregion

        #region methods

        protected override emBusActivityResult_t InnerPreRun(BusinessContext argContext)
        {
            var activityResult = base.InnerPreRun(argContext);
            if (activityResult != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("Failed to execute base.InnerPreRun()!");
                return activityResult;
            }

            VTMContext = (VTMBusinessContext)argContext;
            SerialNumber = Guid.NewGuid().ToString("N");
            return emBusActivityResult_t.Success;
        }

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogInfo($"Enter {nameof(RequestTransViaRestfulSEM)}.{nameof(InnerRun)}()...");

            try
            {
                if (string.IsNullOrWhiteSpace(SerialNumberDatacacheKey) || string.IsNullOrWhiteSpace(TerminalIdDatacacheKey))
                {
                    VTMContext.NextCondition = "OnPackFail";
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Parameter error! Status=[{SerialNumberDatacacheKey}], CardId=[{TerminalIdDatacacheKey}].");
                    return emBusActivityResult_t.Failure;
                }

                if (!VTMContext.TransactionDataCache.Set(SerialNumberDatacacheKey, SerialNumber, GetType()))
                {
                    VTMContext.NextCondition = "OnPackFail";
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError("Failed to set serial number to datacache!");
                    return emBusActivityResult_t.Failure;
                }

                if (!VTMContext.TransactionDataCache.Set(TerminalIdDatacacheKey, VTMContext.TerminalConfig.Terminal.ATMNumber, GetType()))
                {
                    VTMContext.NextCondition = "OnPackFail";
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError("Failed to set terminal ID to datacache!");
                    return emBusActivityResult_t.Failure;
                }

                if (!string.IsNullOrWhiteSpace(CustomerIdDatacacheKey))
                {
                    if (!VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_PASSPORT, out var objPassportData) ||
                        !(objPassportData is PassportDetailInfo passportData) || string.IsNullOrWhiteSpace(passportData.MRZdocumentID) ||
                        !VTMContext.TransactionDataCache.Set(CustomerIdDatacacheKey, passportData.MRZdocumentID, GetType()))
                    {
                        VTMContext.NextCondition = "OnPackFail";
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        Log.Action.LogError($"Failed to get/set customer ID from/to datacache!");
                        return emBusActivityResult_t.Failure;
                    }
                }

                var baseRet = base.InnerRun(argContext);
                if (baseRet != emBusActivityResult_t.Success)
                {
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError("Failed to execute base.InnerRun()!");
                    return baseRet;
                }

                if (!VTMContext.TransactionDataCache.Get(SerialNumberDatacacheKey, out var objSerialNumber) || objSerialNumber as string != SerialNumber)
                {
                    VTMContext.NextCondition = "OnUnpackFail";
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Response serial number [{objSerialNumber}] is not matched with sent [{SerialNumber}]!");
                    return emBusActivityResult_t.Failure;
                }

                if (!VTMContext.TransactionDataCache.Get(TerminalIdDatacacheKey, out var objTerminalId) || objTerminalId as string != VTMContext.TerminalConfig.Terminal.ATMNumber)
                {
                    VTMContext.NextCondition = "OnUnpackFail";
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    Log.Action.LogError($"Response Terminal Id [{objTerminalId}] is not matched with sent [{VTMContext.TerminalConfig.Terminal.ATMNumber}]!");
                    return emBusActivityResult_t.Failure;
                }

                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
                return emBusActivityResult_t.Success;
            }
            finally
            {
                Log.Action.LogInfo($"Leave {nameof(RequestTransViaRestfulSEM)}.{nameof(InnerRun)}().");
            }
        }

        #endregion
    }
}
