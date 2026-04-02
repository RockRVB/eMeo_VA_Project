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
                  Name = "CheckAddCardTimer",
                  NodeNameOfConfiguration = "CheckAddCardTimer")]
    public class CheckAddCardTimer : BusinessActivityVTMBase
    {
        #region constructor
        protected CheckAddCardTimer()
        {

        }
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new CheckAddCardTimer();
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
            //检查是否在闲时
            if (VTMContext.CheckAddCardTimer())
            {
                VTMContext.NextCondition = EventDictionary.s_EventContinue;
                VTMContext.ActionResult = emBusActivityResult_t.Success;
                VTMContext.CurrentTransactionResult = TransactionResult.Success;
            }
            else
            {
                Log.Action.LogInfo("Current time is out of range");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
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
